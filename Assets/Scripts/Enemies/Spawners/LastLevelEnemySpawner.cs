using System.Collections.Generic;
using UnityEngine;

public class LastLevelEnemySpawner : ObjectInRoomSpawner
{
    ///////////////////////////////////////////
    // Fields
    // const
    private const float OFFSET_TO_NOT_SPAWN_NEAR_WALLS_X = 1.5f;
    private const float OFFSET_TO_NOT_SPAWN_NEAR_WALLS_Y = 1.75f;
    
    ///////////////////////////////////////////
    // Methods

    public static void PlaceSpawners(List<Transform> spawners, Vector2 roomPosition, Vector2 roomSize)
    {
        if (spawnAreaConfig == null)
        {
            InitializeConfig();
        }

        Vector2 offsetFromCenter = roomSize / 2;
        offsetFromCenter.x -= OFFSET_TO_NOT_SPAWN_NEAR_WALLS_X;
        offsetFromCenter.y -= OFFSET_TO_NOT_SPAWN_NEAR_WALLS_Y;

        for (int i = 0; i < spawners.Count; i++)
        {
            spawners[i].position = MovePosition(roomPosition, offsetFromCenter);
        }
    }
}
