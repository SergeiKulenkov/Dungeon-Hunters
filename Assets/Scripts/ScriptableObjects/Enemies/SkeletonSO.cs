using UnityEngine;

[CreateAssetMenu(fileName = Definitions.SKELETON_CONFIG,
menuName = Definitions.SCRIPTABLE_OBJECTS_PATH + Definitions.SKELETON_CONFIG)]
public class SkeletonSO : ScriptableObject
{
    public float SeekCooldown;
    public float AttackCooldown;
    public int Health;
    public int Damage;
    public float MaxSpeed;
    public float Acceleration;
    public float Decceleration;
    public float AttackRadius;
}
