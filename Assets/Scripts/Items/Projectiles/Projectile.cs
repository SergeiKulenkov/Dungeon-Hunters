using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour, IProjectile
{
    ///////////////////////////////////////////
    // Fields
    private bool isHit;
    
    public bool IsFromEnemy { set; get; }
    public int Damage { set; get; }

    ///////////////////////////////////////////
    // Methods

    private void Awake()
    {
        StartCoroutine(DestroyAfterDelay());
    }

    protected virtual void OnTriggerEnter2D(Collider2D collider)
    {
        if (!isHit)
        {
            bool destroy = true;
            if (collider.TryGetComponent<ICover>(out ICover cover))
            {
                cover.ChangeVariant(Damage);
            }
            else if (collider.TryGetComponent<IPlayer>(out IPlayer player))
            {
                if (IsFromEnemy)
                {
                    player.TakeDamage(Damage);
                }
                else
                {
                    destroy = false;
                }
            }
            else if (collider.TryGetComponent<IEnemy>(out IEnemy enemy))
            {
                if (IsFromEnemy)
                {
                    destroy = false;
                }
                else
                {
                    enemy.TakeDamage(Damage);
                }
            }
            else if (collider.TryGetComponent<IProjectile>(out IProjectile projectile))
            {
                destroy = false;
            }

            if (destroy)
            {
                isHit = true;
                Destroy(gameObject);
            }
        }
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(Definitions.DELAY_TO_DESTROY_PROJECTILE);
        Destroy(gameObject);
    }
}
