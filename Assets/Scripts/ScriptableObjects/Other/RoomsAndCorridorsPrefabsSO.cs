using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = Definitions.ROOMS_CORRIDORS_PREFABS_CONFIG,
menuName = Definitions.SCRIPTABLE_OBJECTS_PATH + Definitions.ROOMS_CORRIDORS_PREFABS_CONFIG)]
public class RoomsAndCorridorsPrefabsSO : ScriptableObject
{   
    [Header("Rooms")]
    public Transform EntranceRoom;
    public List<Transform> Rooms;
    public Transform FinishRoom;
    
    [Header("Corridors")]
    public Transform CorridorMiddleDown;
    public Transform CorridorMiddleLeft;
    public Transform CorridorMiddleRight;
    public Transform CorridorMiddleUp;

    public Transform CorridorEdgeDown;
    public Transform CorridorEdgeLeft;
    public Transform CorridorEdgeRight;
    public Transform CorridorEdgeUp;
    
    [Header("Closed walls")]
    public Transform ClosedWallDown;
    public Transform ClosedWallLeft;
    public Transform ClosedWallRight;
    public Transform ClosedWallUp;

    [Header("Blocking Walls")]
    public Transform BlockingWallDown;
    public Transform BlockingWallLeft;
    public Transform BlockingWallRight;
    public Transform BlockingWallUp;
}
