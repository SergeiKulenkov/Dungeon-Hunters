using UnityEngine;
using System;
using System.Collections;

public abstract class Player : MonoBehaviour, IPlayer
{
    ///////////////////////////////////////////
    // Fields
    private Transform aimingHand;
    private GunRotationController gunRotationController;
    private CharacterAnimationController animationController;

    private Vector2 aimDirection;
    private float aimAngle;
    private float newAimAngle;
    private string pickedUpItem;
    private bool isDirectionLeft;

    protected InvulnerabilityController invulnerabilityController;
    
    public static int Health { get; private set; }
    public static int MaxHealth { get; protected set; }

    // events
    public static event Action OnPlayerDied;
    public static event Action OnWeaponPickedUp;
    public static event Action OnFullDamageTaken;
    public static event Action OnHalfDamageTaken;
    public static event Action OnFullHeartPickedUp;
    public static event Action OnHalfHeartPickedUp;
    public static event Action OnHealthSet;
    
    ///////////////////////////////////////////
    // Methods

    private void Awake()
    {
        gunRotationController = GetComponent<GunRotationController>();
        gunRotationController.OnAimingHandChanged += ChangeAimingHands;

        animationController = GetComponent<CharacterAnimationController>();
        animationController.OnDirectionChangedToLeft += OnDirectionChangedToLeft;
        animationController.OnDirectionChangedFromLeft += OnDirectionChangedFromLeft;

        invulnerabilityController = GetComponent<InvulnerabilityController>();
        aimingHand = transform.Find(Definitions.RIGHT_HAND_AIM_PATH);
    }

    protected virtual void Start()
    {
        aimingHand.GetComponent<InvulnerabilityController>().SetNumberOfFlashes(invulnerabilityController.NumberOfFlashes);
        aimingHand.GetComponent<InvulnerabilityController>().SetFramesTimer(invulnerabilityController.IFramesTimer);
        Transform otherHand = transform.Find(Definitions.LEFT_HAND_AIM_PATH);
        otherHand.GetComponent<InvulnerabilityController>().SetNumberOfFlashes(invulnerabilityController.NumberOfFlashes);
        otherHand.GetComponent<InvulnerabilityController>().SetFramesTimer(invulnerabilityController.IFramesTimer);

        int currentHealth = GameState.CurrentHealth;
        if (currentHealth != 0 && currentHealth != MaxHealth) Health = currentHealth;
        else Health = (GameState.PlayerSettings.LastSavedHealth != 0) ? GameState.PlayerSettings.LastSavedHealth : MaxHealth;
        OnHealthSet?.Invoke();

        GetLastWeapon();
        HealthUpgradeController.OnHealthUpgraded += OnHealthUpgraded;
    }

    private void OnDestroy()
    {
        HealthUpgradeController.OnHealthUpgraded -= OnHealthUpgraded;
    }

    private void Update()
    {
        if (InputManager.IsShootButtonPressed())
        {
            Shoot();
        }

        HadnleAiming();
        //reset that string after all collistions checks, so items not picked up twice
        pickedUpItem = "";
    }

    public string GetWeaponName() => aimingHand.GetComponentInChildren<RangedWeaponPlayer>().transform.name;

    private void Shoot()
    {
        if ((aimingHand != null) &&
            (aimingHand.childCount > 0))
        {
            aimingHand.GetComponentInChildren<RangedWeaponPlayer>().Shoot(aimDirection);
        }
    }

    private void HadnleAiming()
    {
        aimDirection = (Utilities.GetMousePosition() - transform.position).normalized;
        newAimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        
        if (aimAngle != newAimAngle)
        {
            aimAngle = newAimAngle;
            gunRotationController.AimAngle = aimAngle;
            animationController.AimAngle = aimAngle;
        }
    }

    private void ChangeAimingHands(bool isGunInRightHand)
    {
        aimingHand = !isGunInRightHand ? transform.Find(Definitions.LEFT_HAND_AIM_PATH)
                                    : transform.Find(Definitions.RIGHT_HAND_AIM_PATH);

        if (!isDirectionLeft && !isGunInRightHand) GetComponent<SpriteRenderer>().flipX = true;
        else GetComponent<SpriteRenderer>().flipX = false;
    }

    private void OnDirectionChangedToLeft()
    {
       GetComponent<SpriteRenderer>().flipX = false;
       isDirectionLeft = true;
       Vector2 offset = GetComponent<CircleCollider2D>().offset;
       GetComponent<CircleCollider2D>().offset = new Vector2(offset.x * (-1), offset.y);
    }

    private void OnDirectionChangedFromLeft()
    {
       GetComponent<SpriteRenderer>().flipX = true;
       isDirectionLeft = false;
       Vector2 offset = GetComponent<CircleCollider2D>().offset;
       GetComponent<CircleCollider2D>().offset = new Vector2(offset.x * (-1), offset.y);
    }

    public void TakeDamage(int damage)
    {
        if (!invulnerabilityController.IsActive)
        {
            invulnerabilityController.StartInvulnerability();
            aimingHand.GetComponent<InvulnerabilityController>().StartInvulnerability();
            Transform otherHand = (aimingHand.name != Definitions.LEFT_HAND_AIM_PATH) ? transform.Find(Definitions.LEFT_HAND_AIM_PATH)
                                    : transform.Find(Definitions.RIGHT_HAND_AIM_PATH);
            otherHand.GetComponent<InvulnerabilityController>().StartInvulnerability();

            Health -= damage;
            SoundManager.Instance.PlaySound(Definitions.Sounds.Hit);

            if (damage == Definitions.ONE_HEALTH_POINT) OnFullDamageTaken?.Invoke();
            else if (damage == Definitions.HALF_HEALTH_POINT) OnHalfDamageTaken?.Invoke();
            else
            {
                bool isAlsoHalfHeartDamage = (damage % Definitions.ONE_HEALTH_POINT != 0);
                int numberOfFullHeartDamages = damage / Definitions.ONE_HEALTH_POINT;
                
                for (int i = 0; i < numberOfFullHeartDamages; i++)
                {
                    OnFullDamageTaken?.Invoke();
                }

                if (isAlsoHalfHeartDamage) OnHalfDamageTaken?.Invoke();
            }

            if (Health <= 0)
            {
                DestroyAll();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (pickedUpItem != collider.transform.name)
        {
            pickedUpItem = collider.transform.name;
            if (collider.TryGetComponent<RangedWeapon>(out RangedWeapon rangedWeapon))
            {
                EquipWeapon(collider.transform);
                SoundManager.Instance.PlaySound(Definitions.Sounds.Item);
            }

            if (collider.CompareTag(Definitions.TAG_FULL_HEART))
            {
                if (Health <= MaxHealth - Definitions.ONE_HEALTH_POINT)
                {
                    Health += Definitions.ONE_HEALTH_POINT;
                    OnFullHeartPickedUp?.Invoke();
                }
                else if (Health == MaxHealth - Definitions.HALF_HEALTH_POINT)
                {
                    ApplyHalfHeart();
                }
                Destroy(collider.gameObject);
                SoundManager.Instance.PlaySound(Definitions.Sounds.Item);
            }
            else if (collider.CompareTag(Definitions.TAG_HALF_HEART))
            {
                if (Health <= MaxHealth - Definitions.HALF_HEALTH_POINT)
                {
                    ApplyHalfHeart();
                }
                Destroy(collider.gameObject);
                SoundManager.Instance.PlaySound(Definitions.Sounds.Item);
            }
            else if (collider.CompareTag(Definitions.TAG_COIN))
            {
                GameState.AddCoin();
                Destroy(collider.gameObject);
                SoundManager.Instance.PlaySound(Definitions.Sounds.Item);
            }
        }
    }

    private void ApplyHalfHeart()
    {
        Health += Definitions.HALF_HEALTH_POINT;
        OnHalfHeartPickedUp?.Invoke();
    }

    private void GetLastWeapon()
    {
        string lastWeaponName = GameState.PlayerSettings.Weapon;
        if (!string.IsNullOrEmpty(lastWeaponName))
        {
            Transform lastWeapon = Utilities.GetWeaponByName(lastWeaponName);
            Transform weapon = Instantiate(lastWeapon, aimingHand.position, lastWeapon.rotation);
            weapon.name = lastWeapon.name;
            EquipWeapon(weapon);
        }
    }

    private void EquipWeapon(Transform weapon)
    {
        if (!Utilities.IsFirstLocation() && aimingHand.childCount > 0)
        {
            GameObject currentWeapon = aimingHand.GetComponentInChildren<RangedWeaponPlayer>().gameObject;
            currentWeapon.SetActive(false);
            Destroy(currentWeapon);
        }
        Vector2 scale = weapon.localScale;
        weapon.SetParent(aimingHand);
        Destroy(weapon.GetComponent<Collider2D>());
        weapon.localPosition = new Vector3();
        weapon.localScale = new Vector3((1 / transform.localScale.x) * scale.x, (1 / transform.localScale.y) * scale.y);
        OnWeaponPickedUp?.Invoke();
        gunRotationController.SetWeapon();
    }

    private void DestroyAll()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Collider2D ownCollider in transform.GetComponents<Collider2D>())
        {
            ownCollider.enabled = false;
        }

        gunRotationController.enabled = false;
        animationController.PlayDeath();
        animationController.enabled = false;
        GetComponent<MovementController>().StopMovement();
        StartCoroutine(DelayDestroy());
    }

    private IEnumerator DelayDestroy()
    {
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
        OnPlayerDied?.Invoke();
    }

    private void OnHealthUpgraded(int upgradeAmount)
    {
        MaxHealth += upgradeAmount;
    }
}
