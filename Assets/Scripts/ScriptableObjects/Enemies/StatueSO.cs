using UnityEngine;

[CreateAssetMenu(fileName = Definitions.STATUE_CONFIG,
menuName = Definitions.SCRIPTABLE_OBJECTS_PATH + Definitions.STATUE_CONFIG)]
public class StatueSO : ScriptableObject
{
    public float AttackCooldown;
    public int Health;
    public int Damage;
    public float ProjectileSpeed;
    public int ProbabilityToFireLessThanMaxShots;
    public int MaxNumberOfShotsInARow;
    public float NextShotCooldown;
}