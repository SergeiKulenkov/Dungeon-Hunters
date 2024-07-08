using UnityEngine;
using System.Collections.Generic;

public class TrapSpawner : ObjectInRoomSpawner
{
    ///////////////////////////////////////////
    // Fields
    // const
    private const float OFFSET_TO_NOT_SPAWN_NEAR_WALLS_X = 1.5f;
    private const float OFFSET_TO_NOT_SPAWN_NEAR_WALLS_Y = 1.75f;

    ///////////////////////////////////////////
    // Methods

    private static void InitializeTrapsData(ref int trapCount, ref int trapsChangeMin, ref int trapsChangeMax, ref List<Transform> traps)
    {
        traps = Utilities.IsFinalLevel() ? ConfigManager.GetFinalLevelConfig().Traps : ConfigManager.GetRoomConfig().Traps;
        if (Utilities.IsLastLevel() || Utilities.IsFinalLevel())
        {
            LastLevelSpawnerSO lastLevelConfig = Utilities.IsFinalLevel() ? ConfigManager.GetFinalLevelSpawnerConfig() : ConfigManager.GetLastLevelSpawnerConfig();
            trapCount = lastLevelConfig.TrapCount;
            trapsChangeMin = lastLevelConfig.TrapsChangeMin;
            trapsChangeMax = lastLevelConfig.TrapsChangeMax;
        }
        else
        {
            SpawnersSO spawnerConfig = ConfigManager.GetSpawnerConfig();
            trapCount = spawnerConfig.TrapCount;
            trapsChangeMin = spawnerConfig.TrapsChangeMin;
            trapsChangeMax = spawnerConfig.TrapsChangeMax;
        }
    }

    public static void SpawnTraps(List<Transform> rooms)
    {
        if (spawnAreaConfig == null)
        {
            InitializeConfig();
        }

        int randomIndex = 0;
        int trapCount = 0;
        int trapsChangeMin = 0;
        int trapsChangeMax = 0;
        Transform newTrap;
        List<Transform> traps = new List<Transform>();
        InitializeTrapsData(ref trapCount, ref trapsChangeMin, ref trapsChangeMax, ref traps);
        
        int count = trapCount;
        Vector2 offsetFromCenter = new Vector2();
        bool isNoTrapDamageUpgraded = GameState.PlayerSettings.NoTrapDamageUpgraded;

        for (int i = 0; i < rooms.Count; i++)
        {
            offsetFromCenter = rooms[i].GetComponent<Room>().GetSize() / 2;
            offsetFromCenter.x -= OFFSET_TO_NOT_SPAWN_NEAR_WALLS_X;
            offsetFromCenter.y -= OFFSET_TO_NOT_SPAWN_NEAR_WALLS_Y;
            for (int j = 0; j < count; j++)
            {
                randomIndex = Random.Range(0, traps.Count);
                newTrap = SpawnObject(rooms[i].position, offsetFromCenter, traps[randomIndex]);
                newTrap.SetParent(rooms[i].Find(Definitions.ROOM_TRAPS_PATH));
                if (isNoTrapDamageUpgraded)
                {
                    newTrap.GetComponent<Collider2D>().enabled = false;
                }
            }
            count = trapCount;
            count += Random.Range(trapsChangeMin, trapsChangeMax);
        }
    }

    public static void SpawnTraps(Vector2 startPosition, Vector2 roomSize, Transform room)
    {
        if (spawnAreaConfig == null)
        {
            InitializeConfig();
        }

        int randomIndex = 0;
        int trapCount = 0;
        int trapsChangeMin = 0;
        int trapsChangeMax = 0;
        Transform newTrap;
        List<Transform> traps = new List<Transform>();
        InitializeTrapsData(ref trapCount, ref trapsChangeMin, ref trapsChangeMax, ref traps);
        
        int count = trapCount;
        Vector2 offsetFromCenter = roomSize / 2;
        offsetFromCenter.x -= OFFSET_TO_NOT_SPAWN_NEAR_WALLS_X;
        offsetFromCenter.y -= OFFSET_TO_NOT_SPAWN_NEAR_WALLS_Y;
        bool isNoTrapDamageUpgraded = GameState.PlayerSettings.NoTrapDamageUpgraded;

        for (int j = 0; j < count; j++)
        {
            randomIndex = Random.Range(0, traps.Count);
            newTrap = SpawnObject(startPosition, offsetFromCenter, traps[randomIndex]);
            newTrap.SetParent(room.Find(Definitions.ROOM_TRAPS_PATH));
            if (isNoTrapDamageUpgraded)
            {
                newTrap.GetComponent<Collider2D>().enabled = false;
            }
        }
        count = trapCount;
        count += Random.Range(trapsChangeMin, trapsChangeMax);
    }

    public static void SpawnTraps(List<TrapData> traps, Transform room)
    {
        Transform newTrap;
        List<Transform> trapObjects = ConfigManager.GetRoomConfig().Traps;
        
        for (int i = 0; i < traps.Count; i++)
        {
            foreach (Transform trap in trapObjects)
            {
                if (trap.name == traps[i].PrefabName)
                {
                    newTrap = Transform.Instantiate(trap, traps[i].Position, Quaternion.identity);
                    newTrap.name = trap.name;
                    newTrap.SetParent(room.Find(Definitions.ROOM_TRAPS_PATH));
                    break;
                }
            }
        }
    }
}
