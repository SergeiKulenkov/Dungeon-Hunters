using UnityEngine;

[CreateAssetMenu(fileName = Definitions.DUMMY_CONFIG,
menuName = Definitions.SCRIPTABLE_OBJECTS_PATH + Definitions.DUMMY_CONFIG)]
public class DummySO : ScriptableObject
{
    public float AttackCooldown;
    public int Health;
    public int Damage;
    public float ProjectileSpeed;
    public int ProbabilityToFireLessThanMaxShots;
    public int MaxNumberOfShotsInARow;
    public float NextShotCooldown;
}
