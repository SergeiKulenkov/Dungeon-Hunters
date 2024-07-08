using UnityEngine;

public class AutoRifleController : RangedWeaponPlayer
{
    ///////////////////////////////////////////
    // Fields
    [SerializeField] private AutoRifleSO config;
    [SerializeField] private Transform rifleBullet;
    [SerializeField] private Transform muzzleFlash;
    [SerializeField] private Definitions.Sounds sound;
    
    ///////////////////////////////////////////
    // Methods

    protected override void Start()
    {
        projectileSpeed = config.BuletSpeed;
        damage = config.Damage;
        cooldownAmount = config.Cooldown;
        recoilMin = config.RecoilMin;
        recoilMax = config.RecoilMax;

        muzzleFlashObject = muzzleFlash;
        projectileObject = rifleBullet;
        shotSound = sound;
        base.Start();
    }
}
