using System.Collections.Generic;
using UnityEngine;

public class RoomsSO : ScriptableObject
{
    public int NumberOfRoomsOfSameType;
    
    [Header("Rooms")]
    public List<Sprite> EntranceRoomsWalls;
    public List<Sprite> EntranceRoomsGrounds;
    public List<Sprite> RoomsWalls;
    public List<Sprite> RoomsGrounds;
    public List<Sprite> FinishRoomsWalls;
    public List<Sprite> FinishRoomsGrounds;

    [Header("Lights")]
    public float BonfireLightIntensity;
    public float TorchLightIntensity;
    
    [Header("Corridors")]
    public Sprite CorridorMiddleDown;
    public Sprite CorridorMiddleLeft;
    public Sprite CorridorMiddleRight;
    public Sprite CorridorMiddleUp;
    public Sprite CorridorEdgeDown;
    public Sprite CorridorEdgeLeft;
    public Sprite CorridorEdgeRight;
    public Sprite CorridorEdgeUp;
    
    [Header("Closed walls")]
    public Sprite ClosedWallDown;
    public Sprite ClosedWallLeft;
    public Sprite ClosedWallRight;
    public Sprite ClosedWallUp;

    [Header("Blocking Walls")]
    public Sprite BlockingWallDown;
    public Sprite BlockingWallLeft;
    public Sprite BlockingWallRight;
    public Sprite BlockingWallUp;

    [Header("Objects")]
    public List<Transform> Enemies;
    public List<Transform> Covers;
    public List<Transform> Traps;
}
