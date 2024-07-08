using UnityEngine;

public class RangedWeapon : MonoBehaviour
{
    ///////////////////////////////////////////
    // Fields
    private int negativeAccuracyChangeCount;
    private int positiveAccuracyChangeCount;

    protected Definitions.Sounds shotSound;
    protected bool isFromEnemy;
    protected Transform muzzle;
    protected Transform projectileObject;
    protected Transform muzzleFlashObject;
    protected float projectileSpeed;
    protected int damage;
    protected float recoilMin;
    protected float recoilMax;

    // const
    private const int MAX_SAME_ACCURACY_CHANGE = 3;

    ///////////////////////////////////////////
    // Methods
    
    protected virtual void Start() { }

    public void SetParameters(bool isFromEnemy, int damage, float projectileSpeed, float recoilMin, float recoilMax, Transform muzzleFlash, Transform projectile, Transform muzzle, Definitions.Sounds sound)
    {
        this.isFromEnemy = isFromEnemy;
        this.damage = damage;
        this.projectileSpeed = projectileSpeed;
        this.recoilMin = recoilMin;
        this.recoilMax = recoilMax;
        this.muzzleFlashObject = muzzleFlash;
        this.projectileObject = projectile;
        this.muzzle = muzzle;
        shotSound = sound;
    }

    public virtual void Shoot(Vector2 direction)
    {
        if (muzzleFlashObject != null)
        {
            Transform muzzleFlash = Instantiate(muzzleFlashObject, muzzle.position, transform.rotation);
            muzzleFlash.SetParent(transform);
            Destroy(muzzleFlash.gameObject, Definitions.MUZZLE_FLASH_TIME);
        }

        Transform bullet = Instantiate(projectileObject, muzzle.position, transform.rotation);
        if (!isFromEnemy) direction = bullet.right;
        if (Random.Range(0, 100) > 50)
        {
            direction = DoNegativeAccuracyChange(direction);
        }
        else
        {
            direction = DoPositiveAccuracyChange(direction);
        }
        Rigidbody2D bulletRigidbody = bullet.GetComponent<Rigidbody2D>();
        bulletRigidbody.AddForce(direction * projectileSpeed, ForceMode2D.Impulse);

        Projectile projectile = bullet.GetComponent<Projectile>();
        projectile.IsFromEnemy = isFromEnemy;
        projectile.Damage = damage;

        SoundManager.Instance.PlaySound(shotSound);
    }

    private Vector2 DoPositiveAccuracyChange(Vector2 direction)
    {
        Vector2 newDirection = direction;
        if (negativeAccuracyChangeCount != 0)
        {
            positiveAccuracyChangeCount = 0;
            negativeAccuracyChangeCount = 0;
        }
        
        if (positiveAccuracyChangeCount == MAX_SAME_ACCURACY_CHANGE) newDirection = DoNegativeAccuracyChange(direction);
        else
        {
            newDirection.y += Random.Range(recoilMin, recoilMax);
            newDirection.x += Random.Range(recoilMin, recoilMax);
        }
        positiveAccuracyChangeCount++;

        return newDirection;
    }

    private Vector2 DoNegativeAccuracyChange(Vector2 direction)
    {
        Vector2 newDirection = direction;
        if (positiveAccuracyChangeCount != 0)
        {
            negativeAccuracyChangeCount = 0;
            positiveAccuracyChangeCount = 0;
        }
        
        if (negativeAccuracyChangeCount == MAX_SAME_ACCURACY_CHANGE) newDirection = DoPositiveAccuracyChange(direction);
        else
        {
            newDirection.y += Random.Range(recoilMax * (-1), recoilMin * (-1));
            newDirection.x += Random.Range(recoilMax * (-1), recoilMin * (-1));
        }
        negativeAccuracyChangeCount++;

        return newDirection;
    }
}
