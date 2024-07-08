using UnityEngine;

public abstract class ObjectInRoomSpawner
{
    ///////////////////////////////////////////
    // Fields
    protected static SpawnAreaSO spawnAreaConfig;
    private static int maxNumberOfTriesToMoveObject; // SO fields cause can't be serialized
    private static float whenMoveFromSideToSideFirstTime;
    private static float whenMoveFromSideToSideSecondTime;

    ///////////////////////////////////////////
    // Methods

    public static void ResetConfig()
    {
        spawnAreaConfig = null;
    }

    protected static void InitializeConfig()
    {
        if (Utilities.IsLastLevel() || Utilities.IsFinalLevel())
        {
            spawnAreaConfig = SpawnAreaSO.CreateInstance<SpawnAreaSO>();
            LastLevelSpawnerSO lastLevelConfig = Utilities.IsFinalLevel() ? ConfigManager.GetFinalLevelSpawnerConfig() : ConfigManager.GetLastLevelSpawnerConfig();
            spawnAreaConfig.NewPositionOffset = lastLevelConfig.NewPositionOffset;
            spawnAreaConfig.NoOverlapRadius = lastLevelConfig.NoOverlapRadius;

            maxNumberOfTriesToMoveObject = lastLevelConfig.MaxNumberOfTriesToMoveObject;
            whenMoveFromSideToSideFirstTime = lastLevelConfig.WhenMoveFromSideToSideFirstTime;
            whenMoveFromSideToSideSecondTime = lastLevelConfig.WhenMoveFromSideToSideSecondTime;
        }
        else
        {
            spawnAreaConfig = ConfigManager.GetSpawnAreaConfig();
            maxNumberOfTriesToMoveObject = spawnAreaConfig.MaxNumberOfTriesToMoveObject;
            whenMoveFromSideToSideFirstTime = spawnAreaConfig.WhenMoveFromSideToSideFirstTime;
            whenMoveFromSideToSideSecondTime = spawnAreaConfig.WhenMoveFromSideToSideSecondTime;
        }
    }

    protected static Transform SpawnObject(Vector2 roomPosition, Vector2 spawnOffsetFromCenter, Transform objectToSpawn)
    {
        Transform newObject = Transform.Instantiate(objectToSpawn, MovePosition(roomPosition, spawnOffsetFromCenter), Quaternion.identity);
        newObject.name = objectToSpawn.name;
        return newObject;
    }

    protected static Vector2 MovePosition(Vector2 roomPosition, Vector2 spawnOffsetFromCenter)
    {
        float randomX = 0;
        float randomY = 0;
        Vector2 newPosition = roomPosition;
        if (Utilities.IsLastLevel())
        {
            newPosition = MakeLastLevelMove(newPosition, spawnAreaConfig.NewPositionOffset);
            // Debug.Log("new pos - " + newPosition);
        }
        bool isOkay = (Physics2D.OverlapCircle(newPosition, spawnAreaConfig.NoOverlapRadius) == null);
        for (int i = 0; i < maxNumberOfTriesToMoveObject; i++)
        {
            if (!isOkay)
            {
                randomX = Random.Range(-spawnAreaConfig.NewPositionOffset.x, spawnAreaConfig.NewPositionOffset.x);
                randomY = Random.Range(-spawnAreaConfig.NewPositionOffset.y, spawnAreaConfig.NewPositionOffset.y);
                newPosition.x = Mathf.Clamp(newPosition.x + randomX, roomPosition.x - spawnOffsetFromCenter.x, roomPosition.x + spawnOffsetFromCenter.x);
                newPosition.y = Mathf.Clamp(newPosition.y + randomY, roomPosition.y - spawnOffsetFromCenter.y, roomPosition.y + spawnOffsetFromCenter.y);
                // Debug.Log("new pos = " + newPosition);
                isOkay = (Physics2D.OverlapCircle(newPosition, spawnAreaConfig.NoOverlapRadius) == null);
                // if (i == maxNumberOfTriesToMoveObject - 1) Debug.Log("last try - " + newPosition);
                if (!isOkay)
                {
                    if (i == (maxNumberOfTriesToMoveObject * whenMoveFromSideToSideFirstTime))
                    {
                        MoveFromSideToSide(ref newPosition, roomPosition, spawnOffsetFromCenter);
                    }
                    else if (i == (int)(maxNumberOfTriesToMoveObject * whenMoveFromSideToSideSecondTime))
                    {
                        MoveInAWeirdRandomWay(ref newPosition, roomPosition, spawnOffsetFromCenter);
                    }
                }
            }
            else
            {
                // Debug.Log("number of try on exit = " + i);
                break;
            }
        }

        return newPosition;
    }

    private static void MoveFromSideToSide(ref Vector2 newPosition, Vector2 roomPosition, Vector2 spawnOffsetFromCenter)
    {
        if (newPosition.x >= roomPosition.x + (spawnOffsetFromCenter.x / 2))
        {
            newPosition.x -= spawnOffsetFromCenter.x;
        }
        else if (newPosition.x <= roomPosition.x - (spawnOffsetFromCenter.x / 2))
        {
            newPosition.x += spawnOffsetFromCenter.x;
        }
        else if (newPosition.y >= roomPosition.y + (spawnOffsetFromCenter.y / 2))
        {
            newPosition.y -= spawnOffsetFromCenter.y;
        }
        else if (newPosition.y <= roomPosition.y - (spawnOffsetFromCenter.y / 2))
        {
            newPosition.y += spawnOffsetFromCenter.y;
        }
    }

    private static void MoveInAWeirdRandomWay(ref Vector2 newPosition, Vector2 roomPosition, Vector2 spawnOffsetFromCenter)
    {
        if (newPosition.x <= roomPosition.x + (spawnOffsetFromCenter.x / 3) &&
            newPosition.x >= roomPosition.x - (spawnOffsetFromCenter.x / 3))
        {
            if (Random.Range(0, 100) > 50)
            {
                newPosition.x -= spawnOffsetFromCenter.x / 2;
            }
            else
            {
                newPosition.x += spawnOffsetFromCenter.x / 2;
            }
        }
        else
        {
            if (newPosition.x >= roomPosition.x + (spawnOffsetFromCenter.x / 2))
            {
                newPosition.x = spawnOffsetFromCenter.x;
            }
            else if (newPosition.x <= roomPosition.x - (spawnOffsetFromCenter.x / 2))
            {
                newPosition.x = -spawnOffsetFromCenter.x;
            }
        }

        if (newPosition.y <= roomPosition.y + (spawnOffsetFromCenter.y / 3) &&
            newPosition.y >= roomPosition.y - (spawnOffsetFromCenter.y / 3))
        {
            if (Random.Range(0, 100) > 50)
            {
                newPosition.y -= spawnOffsetFromCenter.y / 2;
            }
            else
            {
                newPosition.y += spawnOffsetFromCenter.y / 2;
            }
        }
    }

    private static Vector2 MakeLastLevelMove(Vector2 position, Vector2 newPositionOffset)
    {
        Vector2 newPosition = position;
        if (Random.Range(0, 100) > 50) newPosition.x -= newPositionOffset.x / 2;
        else newPosition.x += newPositionOffset.x / 2;

        return newPosition;
    }
}
