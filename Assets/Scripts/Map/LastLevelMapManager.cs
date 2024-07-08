using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class LastLevelMapManager : MonoBehaviour
{
    ///////////////////////////////////////////
    // Fields
    [SerializeField] private Transform fullHeart;
    [SerializeField] private Transform halfHeart;
    [SerializeField] private Transform coin;
    [SerializeField] private Vector2 roomSize;

    [SerializeField] private int maxNumberOfHealthDropsInARow;
    [SerializeField] private int numberOfHealthDropsToSkip;
    [SerializeField] private int maxNumberOfNotSpawnedHealthDrops;
    [SerializeField] private int maxNumberOfNotSpawnedCoins;
    [SerializeField] private int numberOfFirstEnemies;
    [SerializeField] private int minNumberOfEnemies;
    
    protected List<Transform> enemyObjects = new List<Transform>();
    protected Transform room;

    private List<Transform> spawners = new List<Transform>();

    private int healthDropsCount;
    private bool isSkippingHealthDrops;
    private int notSpawnedCoinsCount;
    private int notSpawnedHearts;
    private int currentEnemyCount;
    private int spawnedEnemyCount;
    private int enemyMaxCount;
    
    private List<int> enemyTypeProbability;
    private WaitForSeconds enemySpawnDelay;
    private WaitForSeconds enemyCountCheckDelay;
    private Coroutine countCheckCoroutine;

    // const 
    private const string LAST_LEVEL_ROOM = "LastLevelRoom";
    private const string ENEMY_SPAWNERS = "EnemySpawners";
    private const string UPGRADES = "Upgrades";

    ///////////////////////////////////////////
    // Methods
    
    protected virtual void Start()
    {
        if (room == null)
        {
            foreach (Transform child in transform)
            {
                if (child.name.Contains(LAST_LEVEL_ROOM))
                {
                    room = child;
                    break;
                }
            }
        }

        Transform spawnersObject = transform.Find(ENEMY_SPAWNERS);
        if (spawnersObject != null)
        {
            foreach (Transform spawner in spawnersObject)
            {
                spawners.Add(spawner);
            }
        }
        else Debug.Log("ERROR no spawners object found");
        
        room.GetComponent<Collider2D>().enabled = false;
        SpawnOjects();
        LastLevelEnemySpawner.PlaceSpawners(spawners, room.position, roomSize);
        room.GetComponent<Collider2D>().enabled = true;
        InitializeSpawnerConfig();
        StartSpawningEnemies();

        room.GetComponent<RoomWithEnemiesController>().InitializeRoom();
        SetBlockingWals();
        room.GetComponent<Collider2D>().enabled = true;

        AstarPath.active.Scan(AstarPath.active.data.gridGraph);
        Utilities.SetIgnoredLayerCollisions();
        SoundManager.Instance.PlaySound(Definitions.Sounds.Exit);
        
        healthDropsCount = GameState.HealthDropsCount;
        notSpawnedCoinsCount = GameState.NotSpawnedCoinsCount;
        notSpawnedHearts = GameState.NotSpawnedHearts;
        isSkippingHealthDrops = GameState.IsSkippingHealthDrops;

        Enemy.OnEnemyDied += OnEnemyDied;
        RoomWithEnemiesController.OnLastRoomEntered += OnLastRoomEntered;
        NoTrapDamageUpgradeController.OnNoTrapDamageUpgraded += OnNoTrapDamageUpgraded;
    }
    
    private void OnDestroy()
    {
        Enemy.OnEnemyDied -= OnEnemyDied;
        RoomWithEnemiesController.OnLastRoomEntered -= OnLastRoomEntered;
        NoTrapDamageUpgradeController.OnNoTrapDamageUpgraded -= OnNoTrapDamageUpgraded;
    }

    private void InitializeSpawnerConfig()
    {
        if (enemyObjects.Count == 0) enemyObjects = ConfigManager.GetRoomConfig().Enemies;
        LastLevelSpawnerSO lastLevelConfig = Utilities.IsFinalLevel() ? ConfigManager.GetFinalLevelSpawnerConfig() : ConfigManager.GetLastLevelSpawnerConfig();
        enemyMaxCount = lastLevelConfig.EnemyCount;
        enemyTypeProbability = lastLevelConfig.EnemyTypeProbability;
        enemySpawnDelay = new WaitForSeconds(lastLevelConfig.EnemySpawnDelay);
        enemyCountCheckDelay = new WaitForSeconds(lastLevelConfig.EnemyCountCheckDelay);
    }

    private void SpawnOjects()
    {
        // spawn area config is static so need to reset it before spawning in a last level
        CoverSpawner.ResetConfig();
        Vector2 randomOffset = new Vector2(Random.Range(-0.85f, 0.85f), Random.Range(-0.35f, 0.35f));
        CoverSpawner.SpawnCovers((Vector2) room.position + randomOffset, roomSize, room, coin);
        randomOffset = new Vector2(Random.Range(-1.85f, 1.85f), Random.Range(-0.75f, 0.75f));
        TrapSpawner.SpawnTraps((Vector2) room.position + randomOffset, roomSize, room);
        randomOffset = new Vector2(Random.Range(-1.85f, 1.85f), Random.Range(-0.75f, 0.75f));
        CoverSpawner.SpawnCovers((Vector2) room.position + randomOffset, roomSize, room, coin);
        randomOffset = new Vector2(Random.Range(-1.85f, 1.85f), Random.Range(-0.75f, 0.75f));
        TrapSpawner.SpawnTraps((Vector2) room.position + randomOffset, roomSize, room);
    }

    private void OnEnemyDied(Vector2 position)
    {
        currentEnemyCount--;
        if (isSkippingHealthDrops && (healthDropsCount < numberOfHealthDropsToSkip) ||
            (healthDropsCount < maxNumberOfHealthDropsInARow))
        {
            if (Utilities.ShouldDropFullHealth())
            {
                if (!isSkippingHealthDrops) SpawnHeart(true, position);
                healthDropsCount++;
            }
            else if (Utilities.ShouldDropHalfHealth())
            {
                if (!isSkippingHealthDrops) SpawnHeart(false, position);
                healthDropsCount++;
            }
            else
            {
                notSpawnedHearts++;
                if (notSpawnedHearts == maxNumberOfNotSpawnedHealthDrops)
                {
                    SpawnHeart(false, position);
                }
            }

            if (isSkippingHealthDrops && (healthDropsCount == numberOfHealthDropsToSkip))
            {
                isSkippingHealthDrops = false;
                healthDropsCount = 0;
            }
        }
        else
        {
            isSkippingHealthDrops = true;
            healthDropsCount = 0;
        }
        
        if (Utilities.ShouldDropCoin()) SpawnCoin(position);
        else
        {
            notSpawnedCoinsCount++;
            if (notSpawnedCoinsCount == maxNumberOfNotSpawnedCoins)
            {
                SpawnCoin(position);
            }
        }

        if (!Utilities.IsFinalLevel() && (currentEnemyCount == 0))
        {
            InitializeUpgrades();
        }
    }

    private void SpawnHeart(bool isFullHeart, Vector2 position)
    {
        if (!isFullHeart) Instantiate(halfHeart, position, halfHeart.rotation);
        else Instantiate(fullHeart, position, halfHeart.rotation);
        notSpawnedHearts = 0;
    }

    private void SpawnCoin(Vector2 position)
    {
        position.y += Random.Range(0.4f, 0.8f);
        Instantiate(coin, position, coin.rotation);
        notSpawnedCoinsCount = 0;
    }

    private void SetBlockingWals()
    {
        RoomWithEnemiesController roomController = room.GetComponent<RoomWithEnemiesController>();
        foreach (Transform child in room)
        {
            if (child.name.Contains(Definitions.BLOCKING_WALL)) roomController.AddBlockingWall(child);
        }
    }

    private void StartSpawningEnemies()
    {
        if (numberOfFirstEnemies == 0) numberOfFirstEnemies = spawners.Count;
        for (int i = 0; i < numberOfFirstEnemies; i++)
        {
            SpawnEnemy(spawners[i].position);
        }
    }

    private void SpawnEnemy(Vector2 position)
    {
        int index = Utilities.GetObjectTypeByProbability(enemyTypeProbability);
        Transform newEnemy = Instantiate(enemyObjects[index], position, Quaternion.identity);
        newEnemy.SetParent(room.Find(Definitions.ROOM_ENEMIES_PATH));
        spawnedEnemyCount++;
        currentEnemyCount++;
        
        if (spawnedEnemyCount > numberOfFirstEnemies)
        {
            Color newColor = newEnemy.GetComponent<SpriteRenderer>().color;
            newColor.a = 0;
            newEnemy.GetComponent<SpriteRenderer>().color = newColor;
            newEnemy.gameObject.SetActive(false);
            room.GetComponent<RoomWithEnemiesController>().AddEnemy(newEnemy);
            StopCoroutine(countCheckCoroutine);
            countCheckCoroutine = StartCoroutine(CheckIfEnemyCountChanged());
        }
    }

    private Vector2 ChooseSpawner()
    {
        Vector2 position = spawners[0].position;
        foreach (Transform spawner in spawners)
        {
            if (Physics2D.OverlapCircle(spawner.position, 1.5f, 1 << Definitions.LAYER_ENEMY) == null)
            {
                position = spawner.position;
                break;
            }
        }
        return position;
    }

    private IEnumerator SpawnMoreEnemies()
    {
        while (spawnedEnemyCount < enemyMaxCount)
        {
            yield return enemyCountCheckDelay;
            if (currentEnemyCount <= minNumberOfEnemies)
            {
                SpawnEnemy(ChooseSpawner());
            }
        }
    }

    private IEnumerator CheckIfEnemyCountChanged()
    {
        int prevEnemyCount = currentEnemyCount;
        yield return enemySpawnDelay;
        if ((prevEnemyCount == currentEnemyCount) &&
            (spawnedEnemyCount < enemyMaxCount))
        {
            SpawnEnemy(ChooseSpawner());
        }
    }

    private void InitializeUpgrades()
    {
        int randomUpgrade = 0;
        int upgradesCount = (int) Definitions.Upgrades.Limit;
        List<int> placedUpgrades = new List<int>();
        List<int> usedUpgrades = new List<int>();
        bool isOkay = false;

        if (GameState.MaxHealthUpgraded) usedUpgrades.Add((int) Definitions.Upgrades.Health);
        if (GameState.MaxSpeedUpgraded) usedUpgrades.Add((int) Definitions.Upgrades.Speed);
        if (GameState.MaxFireRateUpgraded) usedUpgrades.Add((int) Definitions.Upgrades.FireRate);
        if (GameState.PlayerSettings.NoTrapDamageUpgraded) usedUpgrades.Add((int) Definitions.Upgrades.NoTrapDamage);

        Transform upgradesObject = transform.Find(UPGRADES);
        if (upgradesObject != null)
        {
            foreach (Transform upgradeObject in upgradesObject)
            {
                while (!isOkay)
                {
                    isOkay = true;
                    randomUpgrade = Random.Range(0, upgradesCount);
                    for (int i = 0; i < placedUpgrades.Count; i++)
                    {
                        if (randomUpgrade == placedUpgrades[i])
                        {
                            isOkay = false;
                            break;
                        }
                    }
                    for (int i = 0; i < usedUpgrades.Count; i++)
                    {
                        if (randomUpgrade == usedUpgrades[i])
                        {
                            isOkay = false;
                            break;
                        }
                    }
                }

                switch((Definitions.Upgrades) randomUpgrade)
                {
                    case Definitions.Upgrades.Health:
                        upgradeObject.gameObject.AddComponent<HealthUpgradeController>();
                        break;
                    case Definitions.Upgrades.Speed:
                        upgradeObject.gameObject.AddComponent<SpeedUpgradeController>();
                        break;
                    case Definitions.Upgrades.FireRate:
                        upgradeObject.gameObject.AddComponent<FireRateUpgradeController>();
                        break;
                    case Definitions.Upgrades.NoTrapDamage:
                        upgradeObject.gameObject.AddComponent<NoTrapDamageUpgradeController>();
                        break;
                }

                isOkay = false;
                placedUpgrades.Add(randomUpgrade);
            }
        }
        else Debug.Log("ERROR no upgrades object found");
    }

    private void OnLastRoomEntered(Vector2 roomPosition)
    {
        countCheckCoroutine = StartCoroutine(CheckIfEnemyCountChanged());
        StartCoroutine(SpawnMoreEnemies());
    }

    private void OnNoTrapDamageUpgraded()
    {
        foreach (Transform trap in room.Find(Definitions.ROOM_TRAPS_PATH))
        {
            trap.GetComponent<Collider2D>().enabled = false;
        }
    }
}
