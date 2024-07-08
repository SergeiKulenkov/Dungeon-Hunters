using UnityEngine;

public class DummyController : Enemy
{
    ///////////////////////////////////////////
    // Fields
    [SerializeField] private DummySO config;
    [SerializeField] private Transform spikedBallObject;
    
    private Transform shootingPoint;
    private Vector2 shootingDirection;
    private int shotsCount;

    ///////////////////////////////////////////
    // Methods

    protected override void Start()
    {
        health = config.Health;
        attackCooldown = config.AttackCooldown;
        damage = config.Damage;

        shootingPoint = transform.Find(Definitions.SHOOTING_POINT);

        base.Start();
    }

    protected override void Attack()
    {
        if (state == State.Attacking)
        {
            Shoot();

            shotsCount++;
            if ((shotsCount == config.MaxNumberOfShotsInARow) ||
                (Random.Range(0, 100) < config.ProbabilityToFireLessThanMaxShots))
            {
                shotsCount = 0;
            }
            else attackTimer = config.NextShotCooldown;
        }
    }

    private void Shoot()
    {
        shootingDirection = target.position - shootingPoint.position;
        shootingPoint.eulerAngles = new Vector3(0, 0, Mathf.Atan2(shootingDirection.y, shootingDirection.x) * Mathf.Rad2Deg);
        Transform spikedBall = Instantiate(spikedBallObject, shootingPoint.position, shootingPoint.rotation);
        
        Rigidbody2D ballRigidbody = spikedBall.GetComponent<Rigidbody2D>();
        ballRigidbody.AddForce(shootingDirection.normalized * config.ProjectileSpeed, ForceMode2D.Impulse);

        Projectile spikedBallController = spikedBall.GetComponent<Projectile>();
        spikedBallController.IsFromEnemy = true;
        spikedBallController.Damage = damage;
    }
}
