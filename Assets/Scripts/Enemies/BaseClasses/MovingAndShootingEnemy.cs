using UnityEngine;

public class MovingAndShootingEnemy : MovingEnemy
{
    ///////////////////////////////////////////
    // Fields
    [SerializeField] private Vector2 gunOffsetRightPosition;
    [SerializeField] private Vector2 gunOffsetLeftPosition;
    [SerializeField] private Vector3 gunOffsetLeftAngle;
    private Transform gun;
    private Transform aimingHand;
    private RangedWeapon gunScript;

    private Vector2 shootingDirection = new Vector2();
    private Vector3 newGunAngle = new Vector3();
    private float aimAngle;
    private int shotsCount;
    
    protected Definitions.Sounds shotSound;
    protected Transform muzzleFlash;
    protected Transform projectile;
    protected float projectileSpeed;
    protected int maxNumberOfShots;
    protected int probabilityToFireLessThanMaxShots;
    protected float nextShotCooldown;
    protected float recoilMin;
    protected float recoilMax;

    // const
    private const string MUZZLE = "Muzzle";
    private const int RIGHT_ANGLE_DOWN = -105;
    private const int RIGHT_ANGLE_UP = 105;
    private const int LEFT_ANGLE_DOWN = -75;
    private const int LEFT_ANGLE_UP = 75;
    private const int FLIP_ANGLE = 180;

    ///////////////////////////////////////////
    // Methods

    protected override void Start()
    {
        base.Start();
        aimingHand = transform.Find(Definitions.RIGHT_HAND_AIM_PATH);
        aimingHand.GetComponent<InvulnerabilityController>().SetNumberOfFlashes(NUMBER_OF_FLASHES);
        aimingHand.GetComponent<InvulnerabilityController>().SetFramesTimer(IFRAMES_TIMER);
        Transform otherHand = transform.Find(Definitions.LEFT_HAND_AIM_PATH);
        otherHand.GetComponent<InvulnerabilityController>().SetNumberOfFlashes(NUMBER_OF_FLASHES);
        otherHand.GetComponent<InvulnerabilityController>().SetFramesTimer(IFRAMES_TIMER);

        gun = aimingHand.GetComponentInChildren<RangedWeapon>().transform;
        Transform muzzle = gun.Find(MUZZLE);
        foreach (Component component in gun.GetComponents<Component>())
        {
            if (!(component is Transform)) Destroy(component);
        }

        attackPoint = gun;
        gunScript = gun.gameObject.AddComponent<RangedWeapon>();
        gunScript.SetParameters(true, damage, projectileSpeed, recoilMin, recoilMax, muzzleFlash, projectile, muzzle, shotSound);

        // OnAlertChanged(true);
        // AstarPath.active.Scan(AstarPath.active.data.gridGraph);
        // Utilities.SetIgnoredLayerCollisions();
    }

    // private void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawWireSphere(transform.position, attackRadius);
    // }

    protected override void FixedUpdate()
    {
        if ((state > State.NotAlert) && (state < State.Dying) &&
            (target != null))
        {
            base.FixedUpdate();
            shootingDirection = (target.position - transform.position).normalized;
            aimAngle = Mathf.Atan2(shootingDirection.y, shootingDirection.x) * Mathf.Rad2Deg;

            if (isFacingRight)
            {
                if (!((aimAngle <= RIGHT_ANGLE_UP) && (aimAngle >= RIGHT_ANGLE_DOWN)))
                {
                    TurnAround();
                }
            }
            else
            {
                if (!(((aimAngle >= LEFT_ANGLE_UP) && (aimAngle <= FLIP_ANGLE)) ||
                    ((aimAngle <= LEFT_ANGLE_DOWN) && (aimAngle >= -FLIP_ANGLE))))
                {
                    TurnAround();
                }
            }
                
            newGunAngle.z = aimAngle;
            aimingHand.eulerAngles = newGunAngle;
        }
    }

    private void SetGunOffset()
    {
        if (!isFacingRight)
        {
            gun.localPosition = gunOffsetLeftPosition;
            gun.localEulerAngles = gunOffsetLeftAngle;
        }
        else
        {
            gun.localPosition = gunOffsetRightPosition;
            gun.localEulerAngles = new Vector3();
        }
    }

    protected override void ChangeFacingDirection() { }
    protected override void ChangeAttackPoint() { }

    protected override void TurnAround()
    {
        base.TurnAround();
        if (!isFacingRight && (aimingHand.name == Definitions.RIGHT_HAND_AIM_PATH) ||
            isFacingRight && (aimingHand.name == Definitions.LEFT_HAND_AIM_PATH))
        {
            aimingHand.GetComponent<SpriteRenderer>().enabled = false;
            aimingHand = !isFacingRight ? transform.Find(Definitions.LEFT_HAND_AIM_PATH)
                                        : transform.Find(Definitions.RIGHT_HAND_AIM_PATH);
            aimingHand.GetComponent<SpriteRenderer>().enabled = true;
            gun.SetParent(aimingHand, true);
            SetGunOffset();
        }
    }

    protected override void Attack()
    {
        if (state == State.Attacking)
        {
            gunScript.Shoot(shootingDirection);

            shotsCount++;
            if ((shotsCount == maxNumberOfShots) || (Random.Range(0, 100) < probabilityToFireLessThanMaxShots))
            {
                MoveAfterShooting();
                shotsCount = 0;
            }
            else attackTimer = nextShotCooldown;
        }
    }

    protected override void StartInvulnerability()
    {
        base.StartInvulnerability();
        aimingHand.GetComponent<InvulnerabilityController>().StartInvulnerability();
        Transform otherHand = (aimingHand.name != Definitions.LEFT_HAND_AIM_PATH) ? transform.Find(Definitions.LEFT_HAND_AIM_PATH)
                                : transform.Find(Definitions.RIGHT_HAND_AIM_PATH);
        otherHand.GetComponent<InvulnerabilityController>().StartInvulnerability();
    }
}
