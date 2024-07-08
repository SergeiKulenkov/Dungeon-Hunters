using UnityEngine;
public class PistolController : RangedWeaponPlayer
{
    ///////////////////////////////////////////
    // Fields
    [SerializeField] private PistolSO config;
    [SerializeField] private Transform pistolBullet;
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
        projectileObject = pistolBullet;
        shotSound = sound;
        base.Start();
    }
}
