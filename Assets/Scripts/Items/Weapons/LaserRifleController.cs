using UnityEngine;

public class LaserRifleController : RangedWeaponPlayer
{
    ///////////////////////////////////////////
    // Fields
    [SerializeField] private LaserRifleSO config;
    [SerializeField] private Transform purpleLaser;
    [SerializeField] private Definitions.Sounds sound;

    ///////////////////////////////////////////
    // Methods

    protected override void Start()
    {
        projectileSpeed = config.LaserSpeed;
        damage = config.Damage;
        recoilMin = config.RecoilMin;
        recoilMax = config.RecoilMax;
        
        cooldownAmount = config.Cooldown;
        projectileObject = purpleLaser;
        shotSound = sound;
        base.Start();
    }
}
