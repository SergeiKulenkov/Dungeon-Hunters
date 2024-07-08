using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : ObjectInRoomSpawner
{
    ///////////////////////////////////////////
    // Fields
    // const
    private const float OFFSET_TO_NOT_SPAWN_NEAR_WALLS_X = 1.5f;
    private const float OFFSET_TO_NOT_SPAWN_NEAR_WALLS_Y = 1.75f;

    ///////////////////////////////////////////
    // Methods

    public static void SpawnEnemies(List<Transform> rooms)
    {
        if (spawnAreaConfig == null)
        {
            InitializeConfig();
        }

        SpawnersSO spawnerConfig = ConfigManager.GetSpawnerConfig();
        Transform newEnemy;
        List<Transform> enemies = new List<Transform>();
        RoomsSO roomConfig = ConfigManager.GetRoomConfig();
        for (int i = 0; i < spawnerConfig.NumberOfEnemyTypes; i++)
        {
            enemies.Add(roomConfig.Enemies[i]);
        }

        int index = 0;
        int count = spawnerConfig.EnemyCount;
        Vector2 randomOffset = new Vector2();
        Vector2 offsetFromCenter = new Vector2();

        for (int i = 0; i < rooms.Count; i++)
        {
            offsetFromCenter = rooms[i].GetComponent<Room>().GetSize() / 2;
            offsetFromCenter.x -= OFFSET_TO_NOT_SPAWN_NEAR_WALLS_X;
            offsetFromCenter.y -= OFFSET_TO_NOT_SPAWN_NEAR_WALLS_Y;
            randomOffset.x = Random.Range(-spawnAreaConfig.EnemyOffset.x, spawnAreaConfig.EnemyOffset.x);
            randomOffset.y = Random.Range(-spawnAreaConfig.EnemyOffset.y, spawnAreaConfig.EnemyOffset.y);
            
            for (int j = 0; j < count; j++)
            {
                index = Utilities.GetObjectTypeByProbability(spawnerConfig.EnemyTypeProbability);
                newEnemy = SpawnObject((Vector2) rooms[i].position + randomOffset, offsetFromCenter, enemies[index]);
                newEnemy.SetParent(rooms[i].Find(Definitions.ROOM_ENEMIES_PATH));
            }
            count = spawnerConfig.EnemyCount;
            count += Random.Range(spawnerConfig.EnemiesChangeMin, spawnerConfig.EnemiesChangeMax);
        }
    }

    public static void SpawnEnemies(List<EnemyData> enemies, Transform room)
    {
        Transform newEnemy;
        List<Transform> enemyObjects = new List<Transform>();
        enemyObjects = ConfigManager.GetRoomConfig().Enemies;
        
        for (int i = 0; i < enemies.Count; i++)
        {
            foreach (Transform enemy in enemyObjects)
            {
                if (enemy.name == enemies[i].PrefabName)
                {
                    newEnemy = Transform.Instantiate(enemy, enemies[i].Position, Quaternion.identity);
                    newEnemy.name = enemy.name;
                    // newEnemy = SpawnObject(enemies[i].Position, room.GetComponent<Room>().GetSize() / 2, enemy);
                    newEnemy.SetParent(room.Find(Definitions.ROOM_ENEMIES_PATH));
                    break;
                }
            }
        }
    }
}
