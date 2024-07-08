using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    ///////////////////////////////////////////
    // Fields
    [SerializeField] private Vector2 size;
    [SerializeField] private float minCorridorOffsetX;
    [SerializeField] private float minCorridorOffsetY;
    [SerializeField] private Definitions.RoomTypes type;
    private List<CorridorData> corridors = new List<CorridorData>();
    private List<CorridorData> usedCorridors = new List<CorridorData>();
    private Rigidbody2D rigidBody;

    private const string CORRIDOR_POSITIONS = "CorridorPositions";

    ///////////////////////////////////////////
    // Methods

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        Transform corridorPositionsObject = transform.Find(CORRIDOR_POSITIONS);
        foreach (Transform corridor in corridorPositionsObject)
        {
            corridors.Add(new CorridorData(corridor));
        }
        for (int i = 0; i < corridors.Count; i++)
        {
            corridors[i].Id = i;
        }
        Destroy(corridorPositionsObject.gameObject);
    }

    public Vector2 GetSize() => size;
    public Definitions.RoomTypes GetRoomType() => type;
    public CorridorData GetLastUsedCorridorData() => usedCorridors[usedCorridors.Count - 1];
    public List<CorridorData> GetCorridorDatas() => corridors;
    public List<CorridorData> GetUsedCorridorDatas() => usedCorridors;

    public CorridorData GetCorridorDataBySide(Definitions.Sides side)
    {
        CorridorData corridor = null;
        string sideName = Utilities.GetNameOfSide(side);
        bool isUsed = false;
        for (int i = 0; i < corridors.Count; i++)
        {
            if (corridors[i].Name.Contains(sideName))
            {
                isUsed = false;
                foreach (CorridorData used in usedCorridors)
                {
                    if (used.Id == i)
                    {
                        isUsed = true;
                        break;
                    }
                }

                if (!isUsed)
                {
                    corridor = corridors[i];
                    break;
                }
            }
        }

        return corridor;
    }

    public void MovePosition(Vector2 offset)
    {
        transform.position += (Vector3) offset;
        rigidBody.position += offset;

        foreach (CorridorData corridor in corridors)
        {
            corridor.UpdatePosition(offset);
        }
        foreach (CorridorData used in usedCorridors)
        {
            used.UpdatePosition(offset);
        }
    }

    public void MovePosition(Definitions.Sides side)
    {
        // if side up need to use corridor down position
        CorridorData corridor = GetCorridorDataBySide(Utilities.RevertSide(side));
        if (corridor != null)
        {
            // but corridor down has negative position and we need to move up, so *-1
            Vector2 offset = corridor.LocalPosition * (-1) + GetCorridorOffset(side);
            MovePosition(offset);
            // Debug.Log("new room pos = " + transform.position);
        }
    }

    public Vector2 GetCorridorOffset(Definitions.Sides side)
    {
        Vector2 offset = new Vector2();
        switch (side)
        {
            case Definitions.Sides.Up: offset.y += minCorridorOffsetY;
                break;
            case Definitions.Sides.Right: offset.x += minCorridorOffsetX;
                break;
            case Definitions.Sides.Down: offset.y -= minCorridorOffsetY;
                break;
            case Definitions.Sides.Left: offset.x -= minCorridorOffsetX;
                break;
        }

        return offset;
    }

    public void SpawnCorridorExtension(CorridorData corridorData, Definitions.Sides side)
    {
        // TO REMOVE
        // Transform corridorObject = Testing.GetCorridorObject(side);
        SpawnCorridorObject(RoomSpawner.GetCorridorMiddleObject(side), corridorData.GlobalPosition);

        for (int i = 0; i < usedCorridors.Count; i++)
        {
            // Debug.Log(i + " = index, used pos = " + usedCorridors[i].GlobalPosition);
            if (usedCorridors[i].Id == corridorData.Id)
            {
                usedCorridors[i].GlobalPosition = corridorData.GlobalPosition;
                usedCorridors[i].LocalPosition = corridorData.GlobalPosition - (Vector2) transform.position;
                // Debug.Log("new used glob pos = " + usedCorridors[i].GlobalPosition);
                // Debug.Log("new used loc pos = " + usedCorridors[i].LocalPosition);
                break;
            }
        }
    }

    public void SpawnCorridor(Vector2 position, Definitions.Sides side)
    {
        // position is border, if side down need to use offset up
        Vector2 spawnPosition = position + GetCorridorOffset(Utilities.RevertSide(side));
        // Debug.Log("new cor pos = " + corridorPosition);
        SpawnCorridorObject(RoomSpawner.GetCorridorEdgeObject(side), spawnPosition);

        foreach (CorridorData corridor in corridors)
        {
            // Debug.Log("cor pos = " + corridor.position);
            if (corridor.GlobalPosition == spawnPosition)
            {
                // Debug.Log("cor id = " + corridor.Id);
                usedCorridors.Add(new CorridorData(corridor));
                break;
            }
        }
    }

    public Vector2 SpawnCorridor(Definitions.Sides side)
    {
        CorridorData corridor = GetCorridorDataBySide(side);
        Vector2 corridorBorderPosition = new Vector2();
        if (corridor != null)
        {
            corridorBorderPosition = corridor.GlobalPosition + GetCorridorOffset(side);
            SpawnCorridorObject(RoomSpawner.GetCorridorEdgeObject(side), corridor.GlobalPosition);
            // Debug.Log("cor id = " + corridor.Id);
            usedCorridors.Add(new CorridorData(corridor));
        }
        // Debug.Log("cor border pos = " + corridorBorderPosition);
        return corridorBorderPosition;
    }

    public void SpawnCorridorsFromData(List<CorridorData> usedCorridorDatas)
    {
        Definitions.Sides side = Definitions.Sides.Limit;
        Vector2 corridorPosition = new Vector2();
        Vector2 corridorOffset = new Vector2();

        foreach (CorridorData usedCorridor in usedCorridorDatas)
        {
            usedCorridors.Add(new CorridorData(usedCorridor));
            side = Utilities.GetSideByName(usedCorridor.Name);
            corridorPosition = usedCorridor.GlobalPosition;

            foreach (CorridorData corridor in corridors)
            {
                if (usedCorridor.Id == corridor.Id &&
                    (usedCorridor.GlobalPosition != corridor.GlobalPosition))
                {
                    corridorOffset = GetCorridorOffset(Utilities.RevertSide(side));
                    while (corridorPosition + corridorOffset != corridor.GlobalPosition)
                    {
                        SpawnCorridorObject(RoomSpawner.GetCorridorMiddleObject(side), corridorPosition);
                        corridorPosition += Utilities.ConvertSideToVector(Utilities.RevertSide(side));
                    }
                    SpawnCorridorObject(RoomSpawner.GetCorridorMiddleObject(side), corridorPosition);
                    corridorPosition += corridorOffset;
                    break;
                }
            }

            SpawnCorridorObject(RoomSpawner.GetCorridorEdgeObject(side), corridorPosition);
        }        
    }

    private void SpawnCorridorObject(Transform corridorObject, Vector2 position)
    {
        Transform newCorridor = Instantiate(corridorObject, position, Quaternion.identity);
        if (corridorObject.name.Contains(Definitions.CORRIDOR_EDGE))
        {
            if (RoomSpawner.GetCorridorEdgeSprite(Utilities.GetSideByName(corridorObject.name), out Sprite newSprite))
            {
                newCorridor.GetComponent<SpriteRenderer>().sprite = newSprite;
            }
        }
        else
        {
            if (RoomSpawner.GetCorridorMiddleSprite(Utilities.GetSideByName(corridorObject.name), out Sprite newSprite))
            {
                newCorridor.GetComponent<SpriteRenderer>().sprite = newSprite;
            }
        }
        newCorridor.name = corridorObject.name;
        newCorridor.SetParent(transform);
    }

    public void FillUsedListTEST()
    {
        foreach (Transform child in transform)
        {
            if (child.name.Contains(Definitions.CORRIDOR) &&
                child.name != CORRIDOR_POSITIONS)
            {
                string sideName = Utilities.GetSideNameFromObjectName(child.name);
                foreach (CorridorData corridor in corridors)
                {
                    if (corridor.Name.Contains(sideName))
                    {
                        usedCorridors.Add(new CorridorData(corridor));
                        break;
                    }
                }
            }
        }
    }

    public Definitions.Sides FindWhereToExtendCorridor(ref CorridorData extensionCorridorData)
    {
        int index = Random.Range(0, usedCorridors.Count);
        Definitions.Sides extensionSide = Utilities.GetSideByName(usedCorridors[index].Name);
        extensionCorridorData = new CorridorData(usedCorridors[index]);
        foreach (CorridorData corridor in corridors)
        {
            if (corridor.Id == usedCorridors[index].Id && corridor.LocalPosition == usedCorridors[index].LocalPosition)
            {
                extensionCorridorData.UpdatePosition(GetCorridorOffset(extensionSide) / 2);
                break;
            }
        }
        return extensionSide;
    }

    public void SpawnClosedWalls()
    {
        Definitions.Sides side;
        Transform wall;
        Transform wallObject;
        bool isUsed = false;
        for (int i = 0; i < corridors.Count; i++)
        {
            isUsed = false;
            foreach (CorridorData usedCorridor in usedCorridors)
            {
                if (usedCorridor.Id == i)
                {
                    isUsed = true;
                    break;
                }
            }

            if (!isUsed)
            {
                side = Utilities.GetSideByName(corridors[i].Name);
                wallObject = RoomSpawner.GetClosedWallObject(side);
                wall = Instantiate(wallObject, corridors[i].GlobalPosition, Quaternion.identity);
                wall.name = wallObject.name;
                if (RoomSpawner.GetCloseddWallSprite(Utilities.GetSideByName(wallObject.name), out Sprite newSprite))
                {
                    wall.GetComponent<SpriteRenderer>().sprite = newSprite;
                }
                wall.SetParent(transform);
            }
        }
    }

    public void SpawnBlockingWalls()
    {
        Transform wallObject;
        Transform blockingWall;
        Definitions.Sides side;
        Vector2 wallPosition = new Vector2();
        foreach (CorridorData used in usedCorridors)
        {
            side = Utilities.GetSideByName(used.Name);
            wallPosition = used.GlobalPosition + Utilities.GetBlockingWallOffset(side);
            wallObject = RoomSpawner.GetBlockingWallObject(side);
            blockingWall = Instantiate(wallObject, wallPosition, wallObject.rotation);
            if (RoomSpawner.GetBlockingWallSprite(Utilities.GetSideByName(wallObject.name), out Sprite newSprite))
            {
                blockingWall.GetComponent<SpriteRenderer>().sprite = newSprite;
            }
            transform.GetComponent<RoomWithEnemiesController>().AddBlockingWall(blockingWall);
        }
    }
}

[System.Serializable]
public class CorridorData
{
    public Vector2 GlobalPosition;
    public Vector2 LocalPosition;
    public string Name;
    public float Length; // maybe use for corridor pos offset, like middle 1, edge 2
    /// <summary>
    /// used to find the same corridor in another list
    /// or in the same list but after it got changed a bit
    /// </summary>
    public int Id;

    public CorridorData(Transform transform)
    {
        GlobalPosition = transform.position;
        LocalPosition = transform.localPosition;
        Name = transform.name;
        Length = 2f;
    }

    public CorridorData(CorridorData data)
    {
        GlobalPosition = data.GlobalPosition;
        LocalPosition = data.LocalPosition;
        Name = data.Name;
        Length = data.Length;
        Id = data.Id;
    }

    public void UpdatePosition(Vector2 offset)
    {
        GlobalPosition += offset;
        LocalPosition += offset;
    }
}