using UnityEngine;
using System.Collections;
using System;

public abstract class Enemy : MonoBehaviour, IEnemy
{
    ///////////////////////////////////////////
    // Fields
    protected static Transform target;
    protected InvulnerabilityController invulnerabilityController;

    protected int health;
    protected float attackCooldown;
    protected int damage;
    protected float attackTimer;
    protected State state;

    // events
    public static event Action<Vector2> OnEnemyDied;

    // enums
    protected enum State { NotAlert, Moving, Attacking, MovingAfterAttack, Dying, }

    // const
    protected const float IFRAMES_TIMER = 0.15f; // const cause no point making it SO or serialized
    protected const int NUMBER_OF_FLASHES = 1;

    ///////////////////////////////////////////
    // Methods

    protected virtual void Start()
    {
        state = State.NotAlert;
        attackTimer = attackCooldown;
        target = GameObject.FindObjectOfType<Player>().transform;

        invulnerabilityController = GetComponent<InvulnerabilityController>();
        invulnerabilityController.SetNumberOfFlashes(NUMBER_OF_FLASHES);
        invulnerabilityController.SetFramesTimer(IFRAMES_TIMER);
        
        Player.OnPlayerDied += OnPlayerDied;
    }

    protected virtual void OnDestroy()
    {
        Player.OnPlayerDied -= OnPlayerDied;
    }

    protected virtual void Update()
    {
        if ((state == State.Attacking) && (health > 0))
        {
            if (attackTimer > 0) attackTimer -= Time.deltaTime;
            else
            {
                if (target != null)
                {
                    attackTimer = attackCooldown + UnityEngine.Random.Range(-0.2f, 0.2f);
                    Attack();
                }
            }
        }
    }

    private void OnPlayerDied()
    {
        target = null;
        state = State.NotAlert;
    }

    protected abstract void Attack();
    protected virtual void OnAlertChanged(bool isAlert) {}
    protected virtual void StartInvulnerability() => invulnerabilityController.StartInvulnerability();
    
    public void TakeDamage(int damage)
    {
        health -= damage;
        StartInvulnerability();

        if (health <= 0)
        {
            PlayDeathAnimation();
            OnEnemyDied?.Invoke(transform.position);
            DestroyAll();
            StartCoroutine(FadeAndDestroy());
        }
    }

    public void SetAlert(bool isAlert)
    {
        state = isAlert ? State.Attacking : State.NotAlert;
        OnAlertChanged(isAlert);
    }

    protected virtual void PlayDeathAnimation()
    {
        StartCoroutine(Fall());
    }

    protected virtual void DestroyAll()
    {
        foreach (Collider2D collider in transform.GetComponents<Collider2D>())
        {
            collider.enabled = false;
        }
    }

    private IEnumerator Fall()
    {
        state = State.Dying;
        float timer = 0;
        Quaternion targetRot = new Quaternion();
        float fallAngle = -75;
        targetRot.eulerAngles = new Vector3(0, 0, fallAngle);

        while (timer < 1)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, 15 * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }
        state = State.NotAlert;
    }

    private IEnumerator FadeAndDestroy()
    {
        WaitForSeconds delay = new WaitForSeconds(0.1f);
        while (state == State.Dying)
        {
            yield return delay;
        }

        StartCoroutine(Utilities.DisableObjectWithFade(transform, true));
        Destroy(gameObject, Definitions.SPRITE_FADE_TIME);
    }
}
