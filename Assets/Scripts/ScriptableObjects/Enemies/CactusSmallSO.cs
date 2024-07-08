using UnityEngine;

[CreateAssetMenu(fileName = Definitions.CACTUS_SMALL_CONFIG,
menuName = Definitions.SCRIPTABLE_OBJECTS_PATH + Definitions.CACTUS_SMALL_CONFIG)]
public class CactusSmallSO : ScriptableObject
{
    public float AttackCooldown;
    public int Health;
    public int Damage;
    public float ProjectileSpeed;
    public int ProbabilityToFireLessThanMaxShots;
    public int MaxNumberOfShotsInARow;
    public float NextShotCooldown;
}