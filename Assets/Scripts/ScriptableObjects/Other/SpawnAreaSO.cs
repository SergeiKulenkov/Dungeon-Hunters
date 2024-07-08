using UnityEngine;

[CreateAssetMenu(fileName = Definitions.SPAWN_AREA_CONFIG,
menuName = Definitions.SCRIPTABLE_OBJECTS_PATH + Definitions.SPAWN_AREA_CONFIG)]
public class SpawnAreaSO : ScriptableObject
{
    public float NoOverlapRadius;
    public Vector2 NewPositionOffset;
    public Vector2 EnemyOffset;

    public int MaxNumberOfTriesToMoveObject;
    [Header("Percent values from number of tries")]
    public float WhenMoveFromSideToSideFirstTime;
    public float WhenMoveFromSideToSideSecondTime;
}
