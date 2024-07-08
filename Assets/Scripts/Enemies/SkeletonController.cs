using UnityEngine;

public class SkeletonController : MovingEnemy
{
    ///////////////////////////////////////////
    // Fields
    [SerializeField] private SkeletonSO config;
    private Collider2D[] hitColliders = new Collider2D[MAX_HIT_COLLIDERS];
    
    // const
    private const int MAX_HIT_COLLIDERS = 5;

    ///////////////////////////////////////////
    // Methods

    protected override void Start()
    {
        health = config.Health;
        attackCooldown = config.AttackCooldown;
        seekCooldown = new WaitForSeconds(config.SeekCooldown);
        speed = config.MaxSpeed;
        acceleration = config.Acceleration;
        decceleration = config.Decceleration;
        attackRadius = config.AttackRadius;
        
        base.Start();
    }

    // gets called from animation event
    private void DealDamage()
    {
        int count = Physics2D.OverlapCircleNonAlloc(attackPoint.position, attackRadius, hitColliders);
        for (int i = 0; i < count; i++)
        {
            if (hitColliders[i].TryGetComponent<IPlayer>(out IPlayer player))
            {
                player.TakeDamage(config.Damage);
                break;
            }
        }
    }
}
