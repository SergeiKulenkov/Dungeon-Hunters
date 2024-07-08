using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;

public static class RoomSpawner
{
    ///////////////////////////////////////////
    // Fields
    private static RoomsSO roomsConfig;
    private static RoomsAndCorridorsPrefabsSO roomsAndCorridorsPrefabsConfig;
    private static List<Definitions.Sides> lastRepeatingSides = new List<Definitions.Sides>();

    private static bool isFirstLocation;

    // const
    private const int MAX_NUMBER_OF_REPEATING_SIDES = 3;

    ///////////////////////////////////////////
    // Methods

    public static void InitializeConfigs()
    {
        roomsConfig = ConfigManager.GetRoomConfig();
        roomsAndCorridorsPrefabsConfig = ConfigManager.GetRoomsAndCorridorsPrefabsConfig();
        isFirstLocation = Utilities.IsFirstLocation();
    }
    public static void ResetConfigs()
    {
        roomsConfig = null;
        roomsAndCorridorsPrefabsConfig = null;
    }

    public static Transform SpawnRoom(string prefabName, Vector2 position)
    {
        Transform room = Transform.Instantiate(GetRoomByName(prefabName), position, Quaternion.identity);
        if (!isFirstLocation)
        {
            Room roomScript = room.GetComponent<Room>();
            ChangeRoomsLook(room, (int) roomScript.GetRoomType());
        }
        return room;
    }

    public static void SpawnRooms(ref Transform entranceRoom, ref List<Transform> actionRooms, ref Transform finishRoom)
    {
        Vector2 currentPosition = new Vector2();
        Definitions.Sides currentSide = Utilities.GetRandomSide();
        Definitions.Sides prevSide = currentSide;
        lastRepeatingSides.Add(currentSide);

        // entrance room
        entranceRoom = Transform.Instantiate(roomsAndCorridorsPrefabsConfig.EntranceRoom, currentPosition, Quaternion.identity);
        if (!isFirstLocation)
        {
            ChangeRoomsLook(entranceRoom, -1);
        }
        Room roomScript = entranceRoom.GetComponent<Room>();
        currentPosition = roomScript.SpawnCorridor(currentSide);
        roomScript.SpawnClosedWalls();

        // action rooms
        Transform newRoom;
        Transform prevRoom = entranceRoom;
        SpawnersSO spawnerConfig = ConfigManager.GetSpawnerConfig();
        int roomCount = spawnerConfig.RoomCounnt;
        int numberOfRoomTypes = spawnerConfig.NumberOfRoomTypes;
        int randomIndex = 0;
        for (int i = 0; i < roomCount; i++)
        {
            randomIndex = Random.Range(0, numberOfRoomTypes);
            newRoom = Transform.Instantiate(roomsAndCorridorsPrefabsConfig.Rooms[randomIndex], currentPosition, Quaternion.identity);
            if (!isFirstLocation)
            {
                ChangeRoomsLook(newRoom, randomIndex);
            }
            roomScript = newRoom.GetComponent<Room>();
            roomScript.MovePosition(currentSide);

            // revert cause prev up is new down
            prevSide = Utilities.RevertSide(currentSide);
            currentSide = Utilities.GetNewSide(prevSide);
            UpdateRepeatingSides(currentSide);

            if (i >= 1)
            {
                CheckRepeatingSides(ref currentSide, prevSide);
                CheckOverlay(prevRoom.GetComponent<Room>(), ref newRoom, ref currentPosition, Utilities.RevertSide(prevSide));
                currentSide = CheckCorridorLeadingToAnotherRoom(roomScript, currentSide, prevSide);
            }

            roomScript.SpawnCorridor(currentPosition, prevSide);
            currentPosition = roomScript.SpawnCorridor(currentSide);
            
            roomScript.SpawnClosedWalls();
            roomScript.SpawnBlockingWalls();
            prevRoom = newRoom;
            actionRooms.Add(newRoom);
        }

        // finish room
        finishRoom = Transform.Instantiate(roomsAndCorridorsPrefabsConfig.FinishRoom, currentPosition, Quaternion.identity);
        if (!isFirstLocation)
        {
            ChangeRoomsLook(finishRoom, -1);
        }
        roomScript = finishRoom.GetComponent<Room>();
        roomScript.MovePosition(currentSide);

        prevSide = Utilities.RevertSide(currentSide);
        CheckOverlay(prevRoom.GetComponent<Room>(), ref finishRoom, ref currentPosition, Utilities.RevertSide(prevSide));
        currentSide = Utilities.GetNewSide(prevSide);
        CheckRepeatingSides(ref currentSide, prevSide);
        currentSide = CheckCorridorLeadingToAnotherRoom(roomScript, currentSide, prevSide);
        
        roomScript.SpawnCorridor(currentPosition, prevSide);
        roomScript.SpawnCorridor(currentSide);
        roomScript.SpawnClosedWalls();

        lastRepeatingSides.Clear();
    }

    private static void ChangeRoomsLook(Transform room, int roomTypeIndex)
    {
        int random = 0;
        int spriteIndex = roomTypeIndex * roomsConfig.NumberOfRoomsOfSameType;
        foreach (Transform child in room)
        {
            if (child.name.Contains(Definitions.WALLS))
            {
                if (spriteIndex < 0)
                {
                    if (room.name.Contains(Definitions.ENTRANCE_ROOM))
                    {
                        child.GetComponent<SpriteRenderer>().sprite = roomsConfig.EntranceRoomsWalls[random];
                    }
                    else if (room.name.Contains(Definitions.FINISH_ROOM))
                    {
                        child.GetComponent<SpriteRenderer>().sprite = roomsConfig.FinishRoomsWalls[random];
                    }
                }
                else
                {
                    random = Random.Range(spriteIndex, spriteIndex + roomsConfig.NumberOfRoomsOfSameType);
                    child.GetComponent<SpriteRenderer>().sprite = roomsConfig.RoomsWalls[random];
                }
            }
            else if (child.name.Contains(Definitions.GROUND))
            {
                if (spriteIndex < 0)
                {
                    if (room.name.Contains(Definitions.ENTRANCE_ROOM))
                    {
                        child.GetComponent<SpriteRenderer>().sprite = roomsConfig.EntranceRoomsGrounds[random];
                    }
                    else if (room.name.Contains(Definitions.FINISH_ROOM))
                    {
                        child.GetComponent<SpriteRenderer>().sprite = roomsConfig.FinishRoomsGrounds[random];
                    }
                }
                else
                {
                    random = Random.Range(spriteIndex, spriteIndex + roomsConfig.NumberOfRoomsOfSameType);
                    child.GetComponent<SpriteRenderer>().sprite = roomsConfig.RoomsGrounds[random];
                }
            }
            else if (child.name.Contains(Definitions.LIGHTS))
            {
                if (room.name.Contains(Definitions.ENTRANCE_ROOM))
                {
                    foreach (Transform lightSource in child)
                    {
                        lightSource.GetComponentInChildren<Light2D>().intensity = roomsConfig.BonfireLightIntensity;
                    }
                }
                else
                {
                    foreach (Transform lightSource in child)
                    {
                        lightSource.GetComponentInChildren<Light2D>().intensity = roomsConfig.TorchLightIntensity;
                    }
                }
                break;
            }
        }
    }

    private static void UpdateRepeatingSides(Definitions.Sides currentSide)
    {
        if (currentSide == lastRepeatingSides[lastRepeatingSides.Count - 1])
        {
            lastRepeatingSides.Add(currentSide);
        }
        else
        {
            lastRepeatingSides.Clear();
            lastRepeatingSides.Add(currentSide);
        }
    }

    private static void CheckRepeatingSides(ref Definitions.Sides currentSide, Definitions.Sides prevSide)
    {
        while (lastRepeatingSides.Count == MAX_NUMBER_OF_REPEATING_SIDES)
        {
            currentSide = Utilities.GetNewSide(prevSide);
            if (currentSide != lastRepeatingSides[lastRepeatingSides.Count - 1])
            {
                lastRepeatingSides.Clear();
                lastRepeatingSides.Add(currentSide);
            }
        }
    }

    private static Definitions.Sides CheckCorridorLeadingToAnotherRoom(Room roomScript, Definitions.Sides currentSide, Definitions.Sides prevSide)
    {
        Definitions.Sides newSide = currentSide;
        List<Definitions.Sides> usedSides = new List<Definitions.Sides>();
        usedSides.Add(currentSide);
        usedSides.Add(prevSide);

        CorridorData exitCorridor;
        Vector2 boxSize = new Vector2();
        Vector2 direction = new Vector2();
        float distance = 0;
        bool isOkay = false;
        bool isUsed = false;

        for (int i = 0; i < (int) Definitions.Sides.Limit; i++)
        {
            if (isOkay) break;
            isOkay = true;
            direction = Utilities.ConvertSideToVector(newSide);
            boxSize = new Vector2(3, 3);
            distance = 20f;
            if (direction.x == 0)
            {
                boxSize *= Vector2.right;
            }
            else if (direction.y == 0)
            {
                distance += 10f;
                boxSize *= Vector2.up;
            }
            boxSize += new Vector2(Mathf.Abs(direction.x) * 0.1f, Mathf.Abs(direction.y) * 0.1f);
            exitCorridor = roomScript.GetCorridorDataBySide(newSide);

            RaycastHit2D raycast = Physics2D.BoxCast(exitCorridor.GlobalPosition, boxSize, 0, direction, distance, 1 << Definitions.LAYER_ROOM);
            if (raycast.collider != null)
            {
                usedSides.Add(newSide);
                isOkay = false;
                isUsed = true;
                for (int newSideIndex = 0; newSideIndex < (int) Definitions.Sides.Limit; newSideIndex++)
                {
                    if (!isUsed) break;
                    isUsed = false;
                    newSide = (Definitions.Sides) newSideIndex;
                    foreach (Definitions.Sides usedSide in usedSides)
                    {
                        if (newSide == usedSide)
                        {
                            isUsed = true;
                            break;
                        }
                    }
                }
            }
        }

        return newSide;
    }

    private static void CheckOverlay(Room prevRoomScript, ref Transform newRoom, ref Vector2 currentPosition, Definitions.Sides prevSide)
    {
        Vector2 offset = new Vector2();
        CorridorData newCorridorData = null;
        Definitions.Sides side = Definitions.Sides.Limit;

        bool hasMoved = false;
        Room newRoomScript = newRoom.GetComponent<Room>();
        RaycastHit2D[] raycasts = new RaycastHit2D[2];
        Vector2 origin = new Vector2();
        Vector2 boxSize = new Vector2();
        Vector2 halfRoomSize = newRoomScript.GetSize() / 2;
        float distance = 0;

        List<Vector2> directions = new List<Vector2>();
        directions.Add(Vector2.up);
        directions.Add(Vector2.left);
        directions.Add(Vector2.down);
        directions.Add(Vector2.right);

        bool isOkay = false;
        // for (int j = 0; j < 5; j++)
        while (!isOkay)
        {
            isOkay = true;
            hasMoved = false;
            foreach (Vector2 direction in directions)
            {
                if (hasMoved) break;

                boxSize = newRoomScript.GetSize();
                distance = 1.5f;
                if (direction.x == 0)
                {
                    distance += 1f;
                    boxSize *= Vector2.right;
                    boxSize.x += 1f;
                }
                else if (direction.y == 0)
                {
                    boxSize *= Vector2.up;
                    boxSize.y += 1f;
                }
                boxSize += new Vector2(Mathf.Abs(direction.x) * 0.1f, Mathf.Abs(direction.y) * 0.1f);
                origin = newRoomScript.GetCorridorDataBySide(Utilities.ConvertVectorToSide(direction)).GlobalPosition;
                if ((direction == Vector2.up || direction == Vector2.down) &&
                    origin.x != newRoom.position.x)
                {
                    origin.x = newRoom.position.x;
                }
                else if ((direction == Vector2.right || direction == Vector2.left) &&
                        origin.y != newRoom.position.y)
                {
                    origin.y = newRoom.position.y;
                }
                
                int size = Physics2D.BoxCastNonAlloc(origin, boxSize, 0, direction, raycasts, distance, 1 << Definitions.LAYER_ROOM);
                for (int i = 0; i < size; i++)
                {
                    // Debug.Log("origin = " + origin);
                    // Debug.Log("box size = " + boxSize);
                    // Debug.Log("overlapped room name - " + raycasts[i].transform.name);
                    // Debug.Log("hit point = " + raycasts[i].point);
                    if (raycasts[i].transform.position != newRoom.position)
                    {
                        isOkay = false;
                        // if (prevRoomScript.transform.name == raycasts[i].transform.name)
                        // {
                            // Debug.Log("new room pos = " + newRoom.position);
                            // Debug.Break();
                        // }

                        // Debug.Log("origin = " + origin);
                        // Debug.Log("box size = " + boxSize);
                        // Debug.Log("overlapped room name - " + raycasts[i].transform.name);
                        // Debug.Log("hit point = " + raycasts[i].point);
                        side = prevRoomScript.FindWhereToExtendCorridor(ref newCorridorData);
                        offset = Utilities.ConvertSideToVector(side);

                        if (side != prevSide)
                        {
                            offset *= -1;
                            prevRoomScript.MovePosition(offset);
                        }
                        else
                        {
                            newCorridorData.UpdatePosition(offset);
                        }
                        
                        prevRoomScript.SpawnCorridorExtension(newCorridorData, side);
                        currentPosition += offset;
                        newRoomScript.MovePosition(offset);
                        hasMoved = true;
                        break;
                    }
                }
            }
        }
    }

    //////////////////////////////////////////////////////////////////////
    // Object Getters

    public static Transform GetCorridorMiddleObject(Definitions.Sides side)
    {
        Transform corridor = null;
        switch (side)
        {
            case Definitions.Sides.Up: corridor = roomsAndCorridorsPrefabsConfig.CorridorMiddleUp;
                break;
            case Definitions.Sides.Right: corridor = roomsAndCorridorsPrefabsConfig.CorridorMiddleRight;
                break;
            case Definitions.Sides.Down: corridor = roomsAndCorridorsPrefabsConfig.CorridorMiddleDown;
                break;
            case Definitions.Sides.Left: corridor = roomsAndCorridorsPrefabsConfig.CorridorMiddleLeft;
                break;
        }

        // if (!isFirstLocation)
        // {
        //     switch (side)
        //     {
        //         case Definitions.Sides.Up: corridor.GetComponent<SpriteRenderer>().sprite = roomsConfig.CorridorMiddleUp;
        //             break;
        //         case Definitions.Sides.Right: corridor.GetComponent<SpriteRenderer>().sprite = roomsConfig.CorridorMiddleRight;
        //             break;
        //         case Definitions.Sides.Down: corridor.GetComponent<SpriteRenderer>().sprite = roomsConfig.CorridorMiddleDown;
        //             break;
        //         case Definitions.Sides.Left: corridor.GetComponent<SpriteRenderer>().sprite = roomsConfig.CorridorMiddleLeft;
        //             break;
        //     }
        // }

        return corridor;
    }

    public static bool GetCorridorMiddleSprite(Definitions.Sides side, out Sprite newSprite)
    {
        newSprite = null;
        if (!isFirstLocation)
        {
            switch (side)
            {
                case Definitions.Sides.Up: newSprite = roomsConfig.CorridorMiddleUp;
                    break;
                case Definitions.Sides.Right: newSprite = roomsConfig.CorridorMiddleRight;
                    break;
                case Definitions.Sides.Down: newSprite = roomsConfig.CorridorMiddleDown;
                    break;
                case Definitions.Sides.Left: newSprite = roomsConfig.CorridorMiddleLeft;
                    break;
            }
        }

        return (newSprite != null);
    }
    
    public static Transform GetCorridorEdgeObject(Definitions.Sides side)
    {
        Transform corridor = null;
        switch (side)
        {
            case Definitions.Sides.Up: corridor = roomsAndCorridorsPrefabsConfig.CorridorEdgeUp;
                break;
            case Definitions.Sides.Right: corridor = roomsAndCorridorsPrefabsConfig.CorridorEdgeRight;
                break;
            case Definitions.Sides.Down: corridor = roomsAndCorridorsPrefabsConfig.CorridorEdgeDown;
                break;
            case Definitions.Sides.Left: corridor = roomsAndCorridorsPrefabsConfig.CorridorEdgeLeft;
                break;
        }
        
        // if (!isFirstLocation)
        // {
        //     switch (side)
        //     {
        //         case Definitions.Sides.Up: corridor.GetComponent<SpriteRenderer>().sprite = roomsConfig.CorridorEdgeUp;
        //             break;
        //         case Definitions.Sides.Right: corridor.GetComponent<SpriteRenderer>().sprite = roomsConfig.CorridorEdgeRight;
        //             break;
        //         case Definitions.Sides.Down: corridor.GetComponent<SpriteRenderer>().sprite = roomsConfig.CorridorEdgeDown;
        //             break;
        //         case Definitions.Sides.Left: corridor.GetComponent<SpriteRenderer>().sprite = roomsConfig.CorridorEdgeLeft;
        //             break;
        //     }
        // }
        return corridor;
    }

    public static bool GetCorridorEdgeSprite(Definitions.Sides side, out Sprite newSprite)
    {
        newSprite = null;
        if (!isFirstLocation)
        {
            switch (side)
            {
                case Definitions.Sides.Up: newSprite = roomsConfig.CorridorEdgeUp;
                    break;
                case Definitions.Sides.Right: newSprite = roomsConfig.CorridorEdgeRight;
                    break;
                case Definitions.Sides.Down: newSprite = roomsConfig.CorridorEdgeDown;
                    break;
                case Definitions.Sides.Left: newSprite = roomsConfig.CorridorEdgeLeft;
                    break;
            }
        }

        return (newSprite != null);
    }

    public static Transform GetClosedWallObject(Definitions.Sides side)
    {
        Transform wall = null;
        switch (side)
        {
            case Definitions.Sides.Up: wall = roomsAndCorridorsPrefabsConfig.ClosedWallUp;                
                break;
            case Definitions.Sides.Right: wall = roomsAndCorridorsPrefabsConfig.ClosedWallRight; 
                break;
            case Definitions.Sides.Down: wall = roomsAndCorridorsPrefabsConfig.ClosedWallDown;                
                break;
            case Definitions.Sides.Left: wall = roomsAndCorridorsPrefabsConfig.ClosedWallLeft;
                break;
        }
        
        // if (!isFirstLocation)
        // {
        //     switch (side)
        //     {
        //         case Definitions.Sides.Up: wall.GetComponent<SpriteRenderer>().sprite = roomsConfig.ClosedWallUp;
        //             break;
        //         case Definitions.Sides.Right: wall.GetComponent<SpriteRenderer>().sprite = roomsConfig.ClosedWallRight;
        //             break;
        //         case Definitions.Sides.Down: wall.GetComponent<SpriteRenderer>().sprite = roomsConfig.ClosedWallDown;
        //             break;
        //         case Definitions.Sides.Left: wall.GetComponent<SpriteRenderer>().sprite = roomsConfig.ClosedWallLeft;
        //             break;
        //     }
        // }
        return wall;
    }

    public static bool GetCloseddWallSprite(Definitions.Sides side, out Sprite newSprite)
    {
        newSprite = null;
        if (!isFirstLocation)
        {
            switch (side)
            {
                case Definitions.Sides.Up: newSprite = roomsConfig.ClosedWallUp;
                    break;
                case Definitions.Sides.Right: newSprite = roomsConfig.ClosedWallRight;
                    break;
                case Definitions.Sides.Down: newSprite = roomsConfig.ClosedWallDown;
                    break;
                case Definitions.Sides.Left: newSprite = roomsConfig.ClosedWallLeft;
                    break;
            }
        }

        return (newSprite != null);
    }

    public static Transform GetBlockingWallObject(Definitions.Sides side)
    {
        Transform wall = null;
        switch (side)
        {
            case Definitions.Sides.Up: wall = roomsAndCorridorsPrefabsConfig.BlockingWallUp;                
                break;
            case Definitions.Sides.Right: wall = roomsAndCorridorsPrefabsConfig.BlockingWallRight;
                break;
            case Definitions.Sides.Down: wall = roomsAndCorridorsPrefabsConfig.BlockingWallDown;
                break;
            case Definitions.Sides.Left: wall = roomsAndCorridorsPrefabsConfig.BlockingWallLeft;
                break;
        }
        
        // if (!isFirstLocation)
        // {
        //     switch (side)
        //     {
        //         case Definitions.Sides.Up: wall.GetComponent<SpriteRenderer>().sprite = roomsConfig.BlockingWallUp;
        //             break;
        //         case Definitions.Sides.Right: wall.GetComponent<SpriteRenderer>().sprite = roomsConfig.BlockingWallRight;
        //             break;
        //         case Definitions.Sides.Down: wall.GetComponent<SpriteRenderer>().sprite = roomsConfig.BlockingWallDown;
        //             break;
        //         case Definitions.Sides.Left: wall.GetComponent<SpriteRenderer>().sprite = roomsConfig.BlockingWallLeft;
        //             break;
        //     }
        // }

        return wall;
    }

    public static bool GetBlockingWallSprite(Definitions.Sides side, out Sprite newSprite)
    {
        newSprite = null;
        if (!isFirstLocation)
        {
            switch (side)
            {
                case Definitions.Sides.Up: newSprite = roomsConfig.BlockingWallUp;
                    break;
                case Definitions.Sides.Right: newSprite = roomsConfig.BlockingWallRight;
                    break;
                case Definitions.Sides.Down: newSprite = roomsConfig.BlockingWallDown;
                    break;
                case Definitions.Sides.Left: newSprite = roomsConfig.BlockingWallLeft;
                    break;
            }
        }

        return (newSprite != null);
    }

    private static Transform GetRoomByName(string name)
    {
        Transform room = null;
        List<Transform> rooms = new List<Transform>();
        if (name.Contains(Definitions.ENTRANCE_ROOM)) room = roomsAndCorridorsPrefabsConfig.EntranceRoom;
        else if (name.Contains(Definitions.FINISH_ROOM)) room = roomsAndCorridorsPrefabsConfig.FinishRoom;
        // if (name.Contains(Definitions.ENTRANCE_ROOM)) rooms = roomsAndCorridorsPrefabsConfig.EntranceRooms;
        // else if (name.Contains(Definitions.FINISH_ROOM)) rooms = roomsAndCorridorsPrefabsConfig.FinishRooms;
        else rooms = roomsAndCorridorsPrefabsConfig.Rooms;

        foreach (Transform newRoom in rooms)
        {
            if (newRoom.name == name)
            {
                room = newRoom;
                break;
            }
        }
        return room;
    }
}
