using UnityEngine;
using System.Collections.Generic;
using Pathfinding;

public class MapManager : MonoBehaviour
{
    ///////////////////////////////////////////
    // Fields
    [SerializeField] private Transform fullHeart;
    [SerializeField] private Transform halfHeart;
    [SerializeField] private Transform coin;

    [SerializeField] private float weaponPositionY;
    [SerializeField] private int numberOfWeapons;
    [SerializeField] private int maxNumberOfHealthDropsInARow;
    [SerializeField] private int numberOfHealthDropsToSkip;
    [SerializeField] private int maxNumberOfNotSpawnedHealthDrops;
    [SerializeField] private int maxNumberOfNotSpawnedCoins;
    
    private int initializedRoomsCount;
    private int actionRoomsCount;

    public static bool IsSkippingHealthDrops { get; private set; }
    public static int HealthDropsCount { get; private set; }
    public static int NotSpawnedCoinsCount { get; private set; }
    public static int NotSpawnedHearts { get; private set; }

    // const
    private const string WEAPONS = "Weapons";

    ///////////////////////////////////////////
    // Methods

    private void OnEnable()
    {
        RoomWithEnemiesController.OnMovingEnemyInTheRoom += OnMovingEnemyInTheRoom;
        Player.OnWeaponPickedUp += OnWeaponPickedUp;
        Enemy.OnEnemyDied += OnEnemyDied;
    }

    private void Start()
    {
        RoomSpawner.InitializeConfigs();
        if (!GameState.IsLoaded) InitializeRooms();
        else
        {
            List<RoomData> rooms = SaveLoad.LoadRooms();
            InitializeRooms(rooms);
        }
        RoomSpawner.ResetConfigs();

        if (Utilities.IsFirstLevel()) PlaceWeapons();
        else
        {
            SoundManager.Instance.PlaySound(Definitions.Sounds.Exit);
        }

        Utilities.SetIgnoredLayerCollisions();
        HealthDropsCount = GameState.HealthDropsCount;
        NotSpawnedCoinsCount = GameState.NotSpawnedCoinsCount;
        NotSpawnedHearts = GameState.NotSpawnedHearts;
        IsSkippingHealthDrops = GameState.IsSkippingHealthDrops;
    }

    private void OnDestroy()
    {
        Enemy.OnEnemyDied -= OnEnemyDied;
        Player.OnWeaponPickedUp -= OnWeaponPickedUp;
        RoomWithEnemiesController.OnMovingEnemyInTheRoom -= OnMovingEnemyInTheRoom;
    }

    private void OnRoomWithEnemiesInitialized()
    {
        initializedRoomsCount++;
        if ((initializedRoomsCount == actionRoomsCount) &&
            (AstarPath.active.data.gridGraph.center == new Vector3()))
        {
            AstarPath.active.gameObject.SetActive(false);
        }
    }

    private void InitializeRooms()
    {
        // reset config after last level
        if (!Utilities.IsFirstLocation() && Utilities.IsFirstLevel()) CoverSpawner.ResetConfig();
        List<Transform> rooms = new List<Transform>();
        Transform entranceRoom = null;
        Transform finishRoom = null;

        RoomSpawner.SpawnRooms(ref entranceRoom, ref rooms, ref finishRoom);
        actionRoomsCount = rooms.Count;
        foreach (Transform room in rooms)
        {
            room.GetComponent<Collider2D>().enabled = false;
        }
        
        EnemySpawner.SpawnEnemies(rooms);
        CoverSpawner.SpawnCovers(rooms, coin);
        TrapSpawner.SpawnTraps(rooms);

        rooms.Insert(0, entranceRoom);
        rooms.Add(finishRoom);

        for (int i = 0; i < rooms.Count; i++)
        {
            rooms[i].name = Utilities.RemoveCloneString(rooms[i].name);
            rooms[i].SetParent(transform);
            if ((i != 0) && (i != (rooms.Count - 1)))
            {
                rooms[i].GetComponent<Collider2D>().enabled = true;
                RoomWithEnemiesController roomController = rooms[i].GetComponent<RoomWithEnemiesController>();
                roomController.OnRoomWithEnemiesInitialized += OnRoomWithEnemiesInitialized;
                roomController.InitializeRoom();
            }
        }
        ChangeFinishRoom(rooms[rooms.Count - 1]);
    }

    public List<RoomData> GetRoomsData()
    {
        List<RoomData> roomsData = new List<RoomData>();
        List<Transform> rooms = new List<Transform>();

        foreach (Transform child in transform)
        {
            if (child.name.Contains(Definitions.ROOM))
            {
                rooms.Add(child);
            }
        }

        for (int i = 0; i < rooms.Count; i++)
        {
            RoomData data = new RoomData();
            data.InitializeRoomData(rooms[i]);
            roomsData.Add(data);
        }
        return roomsData;
    }

    private void InitializeRooms(List<RoomData> roomsData)
    {
        List<Transform> rooms = new List<Transform>();
        Room roomScript;
        actionRoomsCount = roomsData.Count - 2;
        
        for (int i = 0; i < roomsData.Count; i++)
        {
            rooms.Add(RoomSpawner.SpawnRoom(roomsData[i].PrefabName, roomsData[i].Position));
            roomScript = rooms[i].GetComponent<Room>();
            roomScript.SpawnCorridorsFromData(roomsData[i].UsedCorridors);
            roomScript.SpawnClosedWalls();
            rooms[i].name = roomsData[i].PrefabName;
            rooms[i].SetParent(transform);

            if ((i != 0) && (i != (roomsData.Count - 1)))
            {
                rooms[i].GetComponent<Collider2D>().enabled = false;
                EnemySpawner.SpawnEnemies(roomsData[i].Enemies, rooms[i]);
                CoverSpawner.SpawnCovers(roomsData[i].Covers, rooms[i], coin);
                TrapSpawner.SpawnTraps(roomsData[i].Traps, rooms[i]);
                rooms[i].GetComponent<Collider2D>().enabled = true;

                RoomWithEnemiesController roomController = rooms[i].GetComponent<RoomWithEnemiesController>();
                roomController.OnRoomWithEnemiesInitialized += OnRoomWithEnemiesInitialized;
                roomController.InitializeRoom();
                roomScript.SpawnBlockingWalls();
            }
        }

        ChangeFinishRoom(rooms[rooms.Count - 1]);
    }

    private void ChangeFinishRoom(Transform finishRoom)
    {
        Vector2 lastCorridorPosition = finishRoom.GetComponent<Room>().GetLastUsedCorridorData().GlobalPosition;
        foreach (Transform child in finishRoom)
        {
            if (child.name.Contains(Definitions.CORRIDOR) &&
                ((Vector2) child.position == lastCorridorPosition))
            {
                child.gameObject.AddComponent<LastCorridorController>();
                break;
            }
        }
    }

    private void OnEnemyDied(Vector2 position)
    {
        if (IsSkippingHealthDrops && (HealthDropsCount < numberOfHealthDropsToSkip) ||
            (HealthDropsCount < maxNumberOfHealthDropsInARow))
        {
            if (Utilities.ShouldDropFullHealth())
            {
                if (!IsSkippingHealthDrops) SpawnHeart(true, position);
                HealthDropsCount++;
            }
            else if (Utilities.ShouldDropHalfHealth())
            {
                if (!IsSkippingHealthDrops) SpawnHeart(false, position);
                HealthDropsCount++;
            }
            else
            {
                NotSpawnedHearts++;
                if (NotSpawnedHearts == maxNumberOfNotSpawnedHealthDrops)
                {
                    SpawnHeart(false, position);
                }
            }

            if (IsSkippingHealthDrops && (HealthDropsCount == numberOfHealthDropsToSkip))
            {
                IsSkippingHealthDrops = false;
                HealthDropsCount = 0;
            }
        }
        else
        {
            IsSkippingHealthDrops = true;
            HealthDropsCount = 0;
        }
        
        if (Utilities.ShouldDropCoin()) SpawnCoin(position);
        else
        {
            NotSpawnedCoinsCount++;
            if (NotSpawnedCoinsCount == maxNumberOfNotSpawnedCoins)
            {
                SpawnCoin(position);
            }
        }
    }

    private void SpawnHeart(bool isFullHeart, Vector2 position)
    {
        if (!isFullHeart) Instantiate(halfHeart, position, halfHeart.rotation);
        else Instantiate(fullHeart, position, halfHeart.rotation);
        NotSpawnedHearts = 0;
    }

    private void SpawnCoin(Vector2 position)
    {
        position.y += Random.Range(0.4f, 0.8f);
        Instantiate(coin, position, coin.rotation);
        NotSpawnedCoinsCount = 0;
    }

    private void OnWeaponPickedUp()
    {
        Transform weaponsObject = transform.Find(WEAPONS);
        if (weaponsObject != null)
        {
            if (weaponsObject.childCount != numberOfWeapons)
            {
                foreach (Transform weapon in weaponsObject)
                {
                    weapon.GetComponent<Collider2D>().enabled = false;
                    StartCoroutine(Utilities.DisableObjectWithFade(weapon.Find(Definitions.SPRITE), true));
                    Destroy(weapon.gameObject, Definitions.SPRITE_FADE_TIME);
                }
            }
        }
        else Debug.Log("ERROR no weapons object found");
    }

    private void PlaceWeapons()
    {
        WeaponsSO weaponsConfig = ConfigManager.GetWeaponsConfig();
        float roomSizeX = 0;
        foreach (Transform child in transform)
        {
            if (child.name.Contains(Definitions.ENTRANCE_ROOM))
            {
                roomSizeX = child.GetComponent<Room>().GetSize().x;
                break;
            }
        }
        Vector2 position = new Vector2((roomSizeX / 2) * (-1), weaponPositionY);
        // when dividing we get a number of parts (divide by 4 - get 4 parts)
        // a number of points is parts + 1 (divide by 4 - get 3 points)
        // and here we need point where to place weapons so +1
        float offsetX = roomSizeX / (numberOfWeapons + 1);
        
        position.x += offsetX;
        SpawnWeapon(weaponsConfig.Canon, position);
        position.x += offsetX;
        SpawnWeapon(weaponsConfig.Pistol, position);
        position.x += offsetX;
        SpawnWeapon(weaponsConfig.LaserRifle, position);
        position.x += offsetX;
        SpawnWeapon(weaponsConfig.AutoRifle, position);
    }

    private void SpawnWeapon(Transform weaponToSpawn, Vector2 position)
    {
        Transform weapon = Instantiate(weaponToSpawn, position, weaponToSpawn.rotation);
        weapon.SetParent(transform.Find(WEAPONS));
        weapon.name = weaponToSpawn.name;
    }

    private void OnMovingEnemyInTheRoom(Vector2 roomPosition, Vector2 roomSize)
    {
        GridGraph gridGraph = AstarPath.active.data.gridGraph;
        float nodeSize = gridGraph.nodeSize;
        roomPosition.y -= 0.5f;

        if (gridGraph.center != new Vector3())
        {
            GridGraph newGraph = AstarPath.active.data.AddGraph(typeof(GridGraph)) as GridGraph;
            // divide cause node is 0.5, need roomsize * 2
            newGraph.SetDimensions((int) (roomSize.x / nodeSize), (int) (roomSize.y / nodeSize), nodeSize);
            newGraph.center = roomPosition;
            newGraph.collision.use2D = true;
            newGraph.is2D = true;
            newGraph.collision.diameter = gridGraph.collision.diameter;
            newGraph.collision.type = gridGraph.collision.type;
            newGraph.collision.mask = gridGraph.collision.mask;
            AstarPath.active.Scan(newGraph);
        }
        else
        {
            gridGraph.center = roomPosition;
            gridGraph.SetDimensions((int) roomSize.x * 2, (int) roomSize.y * 2, nodeSize);
            AstarPath.active.Scan(gridGraph);
        }
    }
}
