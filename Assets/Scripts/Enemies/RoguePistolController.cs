using UnityEngine;

public class RoguePistolController : MovingAndShootingEnemy
{
    ///////////////////////////////////////////
    // Fields
    [SerializeField] private RoguePistolSO config;
    [SerializeField] private PistolSO pistolConfig;
    [SerializeField] private Transform pistolBullet;
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

        projectileSpeed = pistolConfig.BuletSpeed;
        damage = pistolConfig.Damage;
        recoilMin = pistolConfig.RecoilMin;
        recoilMax = pistolConfig.RecoilMax;

        projectile = pistolBullet;
        muzzleFlash = muzzleFlashObject;
        shotSound = sound;
        
        base.Start();
    }
}
