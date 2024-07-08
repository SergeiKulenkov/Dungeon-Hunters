using UnityEngine;

[CreateAssetMenu(fileName = Definitions.CAMERA_CONFIG,
menuName = Definitions.SCRIPTABLE_OBJECTS_PATH + Definitions.CAMERA_CONFIG)]
public class CameraSO : ScriptableObject
{
    public float DefaultSpeed;
    public float PlayerFollowingSpeed;
    public float ZoomTime;
    public float MoveWhenEnteredLongRoomTime;
    public float CameraSize5;
    public float CameraSize6;
    public float CameraSize8;
    public float CameraSizeLastLevel;
    public float LongHorizontalRoomEntranceOffset;
    public float LongVerticalRoomEntranceOffset;
}
