using UnityEngine;

public class CanonController : RangedWeaponPlayer
{
    ///////////////////////////////////////////
    // Fields
    [SerializeField] private CanonSO config;
    [SerializeField] private Transform pumpkin;
    [SerializeField] private Transform muzzleFlash;
    [SerializeField] private Definitions.Sounds sound;

    ///////////////////////////////////////////
    // Methods

    protected override void Start()
    {
        projectileSpeed = config.PumpkinSpeed;
        damage = config.Damage;
        cooldownAmount = config.Cooldown;
        recoilMin = config.RecoilMin;
        recoilMax = config.RecoilMax;

        muzzleFlashObject = muzzleFlash;
        projectileObject = pumpkin;
        shotSound = sound;
        base.Start();
    }
}
