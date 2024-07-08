using UnityEngine;

public class RogueLaserRifleController : MovingAndShootingEnemy
{
    ///////////////////////////////////////////
    // Fields
    [SerializeField] private RogueLaserRifleSO config;
    [SerializeField] private LaserRifleSO laserRifleConfig;
    [SerializeField] private Transform laser;
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

        projectileSpeed = laserRifleConfig.LaserSpeed;
        damage = laserRifleConfig.Damage;
        recoilMin = laserRifleConfig.RecoilMin;
        recoilMax = laserRifleConfig.RecoilMax;

        projectile = laser;
        muzzleFlash = null;
        shotSound = sound;
        
        base.Start();
    }
}
