using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class RoomWithEnemiesController : RoomController
{
    ///////////////////////////////////////////
    // Fields
    private List<Transform> blockingWalls = new List<Transform>();
    private List<Transform> enemies = new List<Transform>();
    private GameObject deadEnemy;

    // events
    public event Action OnRoomWithEnemiesInitialized;
    public static event Action<Vector2, Vector2> OnMovingEnemyInTheRoom;
    public static event Action<Vector2> OnLastRoomEntered;

    ///////////////////////////////////////////
    // Methods

    private void Start()
    {
        Enemy.OnEnemyDied += OnEnemyDied;
    }

    private void OnDestroy()
    {
        Enemy.OnEnemyDied -= OnEnemyDied;
    }

    public void InitializeRoom()
    {
        bool isMovingEnemyEvenSent = false;
        Transform enemiesObject = transform.Find(Definitions.ROOM_ENEMIES_PATH);
        if (enemiesObject != null)
        {
            foreach (Transform child in enemiesObject)
            {
                if (child.TryGetComponent<IEnemy>(out IEnemy enemy))
                {
                    StartCoroutine(DelayEnemyDisable(child));
                    enemies.Add(child);
                    if (!Utilities.IsLastLevel() && !Utilities.IsFinalLevel() &&
                        !isMovingEnemyEvenSent && child.TryGetComponent<MovingEnemy>(out MovingEnemy melee))
                    {
                        Vector2 size = transform.GetComponent<Room>().GetSize();
                        OnMovingEnemyInTheRoom?.Invoke(transform.position, size);
                        isMovingEnemyEvenSent = true;
                    }
                }
            }
        }
        else Debug.Log("ERROR no enemies object found");

        OnRoomWithEnemiesInitialized?.Invoke();
    }

    private void EnableWalls()
    {
        foreach (Transform wall in blockingWalls)
        {
            wall.GetComponent<Collider2D>().enabled = true;
            StartCoroutine(Utilities.EnableObjectWithFade(wall, false));
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.TryGetComponent<IPlayer>(out IPlayer player))
        {
            if (!entered)
            {
                entered = true;
                if (enemies != null)
                {
                    foreach (Transform enemy in enemies)
                    {
                        StartCoroutine(Utilities.EnableObjectWithFade(enemy, false));
                        enemy.GetComponent<IEnemy>().SetAlert(true);
                    }
                    EnableWalls();
                }
                
                if (Utilities.IsLastLevel())
                {
                    OnLastRoomEntered?.Invoke(transform.position);
                }
                else
                {
                    SendEnteredEvent();
                }
            }
        }
    }

    protected override void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.TryGetComponent<IPlayer>(out IPlayer player))
        {
            if (entered && (enemies == null))
            {
                entered = false;
                SendExitedEvent();
            }
        }
    }

    private void DestroyWalls()
    {
        foreach (Transform wall in blockingWalls)
        {
            StartCoroutine(Utilities.DisableObjectWithFade(wall, true));
            Destroy(wall.gameObject, Definitions.SPRITE_FADE_TIME);
        }
    }

    private void OnEnemyDied(Vector2 position)
    {
        if (entered)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                if ((Vector2) enemies[i].position == position)
                {
                    if (!Utilities.IsLastLevel())
                    {
                        deadEnemy = new GameObject();
                        deadEnemy.transform.position = position;
                        deadEnemy.name = enemies[i].name;
                        deadEnemy.transform.SetParent(transform.Find(Definitions.ROOM_ENEMIES_PATH));
                        deadEnemy.SetActive(false);
                    }
                    
                    enemies.RemoveAt(i);
                    break;
                }
            }
            if (enemies.Count == 0)
            {
                enemies = null;
                DestroyWalls();
            }
        }
    }

    public void AddBlockingWall(Transform wall)
    {
        wall.gameObject.SetActive(false);
        wall.SetParent(transform);
        blockingWalls.Add(wall);
    }

    public void AddEnemy(Transform enemy)
    {
        StartCoroutine(Utilities.EnableObjectWithFade(enemy, false));
        enemies.Add(enemy);
        StartCoroutine(DelayAlert(enemy));
    }

    public void DestroyBoss()
    {
        enemies = null;
        DestroyWalls();
    }

    private IEnumerator DelayAlert(Transform enemy)
    {
        yield return new WaitForSeconds(1f);
        enemy.GetComponent<IEnemy>().SetAlert(true);
    }

    private IEnumerator DelayEnemyDisable(Transform enemy)
    {
        yield return null;
        StartCoroutine(Utilities.DisableObjectWithFade(enemy, false));
    }
}
