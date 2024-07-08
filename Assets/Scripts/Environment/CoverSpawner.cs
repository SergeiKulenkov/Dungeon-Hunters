using UnityEngine;
using System.Collections.Generic;

public class CoverSpawner : ObjectInRoomSpawner
{
    ///////////////////////////////////////////
    // Fields
    // const
    private const float OFFSET_TO_NOT_SPAWN_NEAR_WALLS_X = 1.5f;
    private const float OFFSET_TO_NOT_SPAWN_NEAR_WALLS_Y = 1.75f;

    ///////////////////////////////////////////
    // Methods

    private static void InitializeCoversData(ref int coverCount, ref int coversChangeMin, ref int coversChangeMax, ref List<Transform> covers)
    {
        covers = Utilities.IsFinalLevel() ? ConfigManager.GetFinalLevelConfig().Covers : ConfigManager.GetRoomConfig().Covers;
        if (Utilities.IsLastLevel() || Utilities.IsFinalLevel())
        {
            LastLevelSpawnerSO lastLevelConfig = Utilities.IsFinalLevel() ? ConfigManager.GetFinalLevelSpawnerConfig() : ConfigManager.GetLastLevelSpawnerConfig();
            coverCount = lastLevelConfig.CoverCount;
            coversChangeMin = lastLevelConfig.CoversChangeMin;
            coversChangeMax = lastLevelConfig.CoversChangeMax;
        }
        else
        {
            SpawnersSO spawnerConfig = ConfigManager.GetSpawnerConfig();
            coverCount = spawnerConfig.CoverCount;
            coversChangeMin = spawnerConfig.CoversChangeMin;
            coversChangeMax = spawnerConfig.CoversChangeMax;
        }
    }

    public static void SpawnCovers(List<Transform> rooms, Transform coin)
    {
        if (spawnAreaConfig == null)
        {
            InitializeConfig();
        }

        int randomIndex = 0;
        int coverCount = 0;
        int coversChangeMin = 0;
        int coversChangeMax = 0;
        List<Transform> covers = new List<Transform>();
        InitializeCoversData(ref coverCount, ref coversChangeMin, ref coversChangeMax, ref covers);
        
        Transform newCover;
        Transform newCoin;
        Vector2 coinScale = coin.localScale;
        int count = coverCount;
        Vector2 offsetFromCenter = new Vector2();

        for (int i = 0; i < rooms.Count; i++)
        {
            offsetFromCenter = rooms[i].GetComponent<Room>().GetSize() / 2;
            offsetFromCenter.x -= OFFSET_TO_NOT_SPAWN_NEAR_WALLS_X;
            offsetFromCenter.y -= OFFSET_TO_NOT_SPAWN_NEAR_WALLS_Y;
            for (int j = 0; j < count; j++)
            {
                randomIndex = Random.Range(0, covers.Count);
                newCover = SpawnObject(rooms[i].position, offsetFromCenter, covers[randomIndex]);
                newCover.SetParent(rooms[i].Find(Definitions.ROOM_COVERS_PATH));
                if (Utilities.ShouldDropCoin())
                {
                    newCoin = Transform.Instantiate(coin, newCover);
                    newCoin.localScale = new Vector3((1 / newCover.localScale.x) * coinScale.x, (1 / newCover.localScale.y) * coinScale.y);
                    newCoin.name = Utilities.RemoveCloneString(newCoin.name);
                    newCoin.gameObject.SetActive(false);
                }
            }
            count = coverCount;
            count += Random.Range(coversChangeMin, coversChangeMax);
        }
    }

    public static void SpawnCovers(Vector2 startPosition, Vector2 roomSize, Transform room, Transform coin)
    {
        if (spawnAreaConfig == null)
        {
            InitializeConfig();
        }

        int randomIndex = 0;
        int coverCount = 0;
        int coversChangeMin = 0;
        int coversChangeMax = 0;
        List<Transform> covers = new List<Transform>();
        InitializeCoversData(ref coverCount, ref coversChangeMin, ref coversChangeMax, ref covers);
        
        Transform newCover;
        Transform newCoin;
        Vector2 coinScale = coin.localScale;
        int count = coverCount;
        Vector2 offsetFromCenter = roomSize / 2;
        offsetFromCenter.x -= OFFSET_TO_NOT_SPAWN_NEAR_WALLS_X;
        offsetFromCenter.y -= OFFSET_TO_NOT_SPAWN_NEAR_WALLS_Y;

        for (int j = 0; j < count; j++)
        {
            randomIndex = Random.Range(0, covers.Count);
            newCover = SpawnObject(startPosition, offsetFromCenter, covers[randomIndex]);
            newCover.SetParent(room.Find(Definitions.ROOM_COVERS_PATH));
            if (Utilities.ShouldDropCoin())
            {
                newCoin =  Transform.Instantiate(coin, newCover);
                newCoin.localScale = new Vector3((1 / newCover.localScale.x) * coinScale.x, (1 / newCover.localScale.y) * coinScale.y);
                newCoin.name = Utilities.RemoveCloneString(newCoin.name);
                newCoin.gameObject.SetActive(false);
            }
        }
    }

    public static void SpawnCovers(List<CoverData> covers, Transform room, Transform coin)
    {
        Transform newCover;
        Vector2 coinScale = coin.localScale;
        List<Transform> coverObjects = ConfigManager.GetRoomConfig().Covers;

        for (int i = 0; i < covers.Count; i++)
        {
            foreach (Transform cover in coverObjects)
            {
                if (cover.name == covers[i].PrefabName)
                {
                    newCover = Transform.Instantiate(cover, covers[i].Position, Quaternion.identity);
                    newCover.name = cover.name;
                    newCover.SetParent(room.Find(Definitions.ROOM_COVERS_PATH));
                    if (covers[i].hasCoin)
                    {
                        Transform newCoin = Transform.Instantiate(coin, newCover);
                        newCoin.localScale = new Vector3((1 / newCover.localScale.x) * coinScale.x, (1 / newCover.localScale.y) * coinScale.y);
                        newCoin.name = Utilities.RemoveCloneString(newCoin.name);
                        newCoin.gameObject.SetActive(false);
                    }
                    break;
                }
            }
        }
    }
}
