using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class RoomData
{
    public string PrefabName;
    public Vector2 Position = new Vector2();
    public List<EnemyData> Enemies = new List<EnemyData>();
    public List<CoverData> Covers = new List<CoverData>();
    public List<TrapData> Traps = new List<TrapData>();
    public List<CorridorData> UsedCorridors = new List<CorridorData>();

    public void InitializeRoomData(Transform room)
    {
        Position = room.position;
        PrefabName = room.name;

        foreach (Transform child in room)
        {
            if (child.name.Contains(Definitions.ROOM_ENEMIES_PATH))
            {
                foreach (Transform obj in child)
                {
                    EnemyData enemy = new EnemyData();
                    enemy.PrefabName = obj.name;
                    enemy.Position = obj.position;
                    Enemies.Add(enemy);
                }
            }
            else if (child.name.Contains(Definitions.ROOM_COVERS_PATH))
            {
                foreach (Transform obj in child)
                {
                    CoverData cover = new CoverData();
                    cover.PrefabName = obj.name;
                    cover.Position = obj.position;
                    cover.hasCoin = (obj.Find(Definitions.COIN_PICK_UP) != null);
                    Covers.Add(cover);
                }
            }
            else if (child.name.Contains(Definitions.ROOM_TRAPS_PATH))
            {
                foreach (Transform obj in child)
                {
                    TrapData trap = new TrapData();
                    trap.PrefabName = obj.name;
                    trap.Position = obj.position;
                    Traps.Add(trap);
                }
            }
        }

        Room roomScript = room.GetComponent<Room>();
        foreach (CorridorData usedCorridor in roomScript.GetUsedCorridorDatas())
        {
            UsedCorridors.Add(new CorridorData(usedCorridor));
        }
    }
}

[System.Serializable]
public class EnemyData
{
    public string PrefabName;
    public Vector2 Position;
}

[System.Serializable]
public class CoverData
{
    public string PrefabName;
    public Vector2 Position;
    public bool hasCoin;
}

[System.Serializable]
public class TrapData
{
    public string PrefabName;
    public Vector2 Position;
}
