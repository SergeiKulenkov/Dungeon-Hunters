using UnityEngine;

public class RogueCanonController : MovingAndShootingEnemy
{
    ///////////////////////////////////////////
    // Fields
    [SerializeField] private RogueCanonSO config;
    [SerializeField] private CanonSO canonConfig;
    [SerializeField] private Transform pumpkin;
    [SerializeField] private Transform muzzleFlashObject;
    [SerializeField] private Definitions.Sounds sound;

    ///////////////////////////////////////////
    // Methods

    protected override void Start()
    {
        health = config.Health;
        seekCooldown = new WaitForSeconds(config.SeekCooldown);
        speed = config.MaxSpeed;
        acceleration = config.Acceleration;
        decceleration = config.Decceleration;
        attackRadius = config.AttackRadius;
        attackCooldown = config.AttackCooldown;
        maxNumberOfShots = config.MaxNumberOfShotsInARow;
        probabilityToFireLessThanMaxShots = config.ProbabilityToFireLessThanMaxShots;
        nextShotCooldown = config.NextShotCooldown;

        projectileSpeed = canonConfig.PumpkinSpeed;
        damage = canonConfig.Damage;
        recoilMin = canonConfig.RecoilMin;
        recoilMax = canonConfig.RecoilMax;

        projectile = pumpkin;
        muzzleFlash = muzzleFlashObject;
        shotSound = sound;
        
        base.Start();
    }
}
