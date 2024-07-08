using UnityEngine;

[CreateAssetMenu(fileName = Definitions.CACTUS_BIG_CONFIG,
menuName = Definitions.SCRIPTABLE_OBJECTS_PATH + Definitions.CACTUS_BIG_CONFIG)]
public class CactusBigSO : ScriptableObject
{
    public float AttackCooldown;
    public int Health;
    public int Damage;
    public float ProjectileSpeed;
    public int ProbabilityToFireLessThanMaxShots;
    public int MaxNumberOfShotsInARow;
    public float NextShotCooldown;
}