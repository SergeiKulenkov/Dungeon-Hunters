using UnityEngine;

[CreateAssetMenu(fileName = Definitions.ROGUE_CANON_CONFIG,
menuName = Definitions.SCRIPTABLE_OBJECTS_PATH + Definitions.ROGUE_CANON_CONFIG)]
public class RogueCanonSO : ScriptableObject
{
    public float SeekCooldown;
    public int Health;
    public float MaxSpeed;
    public float Acceleration;
    public float Decceleration;
    public float AttackRadius;
    public float AttackCooldown;
    public int ProbabilityToFireLessThanMaxShots;
    public int MaxNumberOfShotsInARow;
    public float NextShotCooldown;
}