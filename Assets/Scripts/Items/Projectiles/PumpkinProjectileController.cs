using UnityEngine;

public class PumpkinProjectileController : Projectile
{
    ///////////////////////////////////////////
    // Fields
    [SerializeField] private int pumpkinHealth;
    // private int health;

    ///////////////////////////////////////////
    // Methods

    // private void Awake()
    // {
    //     health = pumpkinHealth;
    // }

    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.TryGetComponent<IProjectile>(out IProjectile projectile))
        {
            Destroy(collider.gameObject);
            pumpkinHealth--;
            if (pumpkinHealth <= 0)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            base.OnTriggerEnter2D(collider);
        }
    }
}
