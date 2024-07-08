using UnityEngine;

public class RangedWeaponPlayer : RangedWeapon
{
    ///////////////////////////////////////////
    // Fields
    private bool canShoot = true;
    private float timer;

    protected float cooldownAmount;

    // const
    private const string MUZZLE_PATH = "Muzzle";

    ///////////////////////////////////////////
    // Methods

    protected override void Start()
    {
        isFromEnemy = false;
        muzzle = transform.Find(MUZZLE_PATH);
        if (GameState.MaxFireRateUpgraded)
        {
            cooldownAmount *= (1 - GameState.PlayerSettings.MaxFireRateChange / 100);
        }

        InputManager.OnInteractPressed += OnInteractPressed;
        base.Start();
    }
    
    private void OnDestroy()
    {
        InputManager.OnInteractPressed -= OnInteractPressed;
    }

    private void Update()
    {
        if (timer >  0) timer -= Time.deltaTime;
        else canShoot = true;
    }

    private void OnInteractPressed()
    {
        ChestController chest = GameObject.FindObjectOfType<ChestController>();
        if (chest != null) chest.OnChestOpened += () => canShoot = false;
    }

    public void ChangeFireRate(float upgradeAmount)
    {
        cooldownAmount *= (1 - upgradeAmount / 100);
    }

    public override void Shoot(Vector2 direction)
    {
        if (canShoot)
        {
            canShoot = false;
            base.Shoot(direction);
            timer = cooldownAmount;
        }
    }
}
