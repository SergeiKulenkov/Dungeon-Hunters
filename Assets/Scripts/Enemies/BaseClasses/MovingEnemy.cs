using UnityEngine;
using Pathfinding;
using System.Collections;

public class MovingEnemy : Enemy
{
    ///////////////////////////////////////////
    // Fields
    private Seeker seeker;
    private Rigidbody2D rigidBody;
    private Path path;
    private Animator animator;

    private int currentWaypoint;
    private Vector2 direction;
    private Vector2 targetSpeed;
    private Vector2 speedDiff;
    private Vector2 retreatPosition;
    private Vector2 directionToTarget;
    private int leftRetreatCount;
    private int rightRetreatCount;

    protected Transform attackPoint;
    protected bool isFacingRight;
    protected float acceleration;
    protected float decceleration;
    protected float speed;
    protected float attackRadius;
    protected WaitForSeconds seekCooldown;

    // const
    private const float MIN_ATTACK_COOLDOWN = 0.1f;
    private const float NEXT_WAYPOINT_DISTANCE = 1f;
    private const int MAX_SAME_RETREAT_CHANGE = 3;

    private const string DEATH = "Death";
    private const string IS_MOVING = "IsMoving";
    private const string DIRECTION = "Direction";
    private const string ATTACK = "Attack";

    private const string ATTACK_POINT_LEFT = "AttackPointLeft";
    private const string ATTACK_POINT_RIGHT = "AttackPointRight";

    ///////////////////////////////////////////
    // Methods

    protected override void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        seeker = GetComponent<Seeker>();
        
        base.Start();
        attackPoint = transform.Find(ATTACK_POINT_LEFT);
        isFacingRight = true;
    }

    protected virtual void FixedUpdate()
    {
        if ((state == State.Moving) &&
            (Vector2.Distance(attackPoint.position, target.position) <= attackRadius))
        {
            Stop();
        }
        else if ((state == State.Attacking) &&
                (Vector2.Distance(attackPoint.position, target.position) > attackRadius))
        {
            animator.SetBool(IS_MOVING, true);
            StartCoroutine(CreatePath());
        }
        else if ((state == State.MovingAfterAttack) &&
                ((Vector2) transform.position == retreatPosition))
        {
            StartCoroutine(CreatePath());
        }
        
        if (((state == State.Moving) || (state == State.MovingAfterAttack)) &&
            (path != null))
        {
            if (currentWaypoint < path.vectorPath.Count)
            {
                Move();
                if (Vector2.Distance(transform.position, path.vectorPath[currentWaypoint]) < NEXT_WAYPOINT_DISTANCE)
                {
                    currentWaypoint++;
                }

                if (direction.x > 0 && !isFacingRight)
                {
                    ChangeFacingDirection();
                    ChangeAttackPoint();
                }
                else if (direction.x < 0 && isFacingRight)
                {
                    ChangeFacingDirection();
                    ChangeAttackPoint();
                }
            }
            else
            {
                Stop();
            }
        }
    }

    private void Move()
    {
        direction = (path.vectorPath[currentWaypoint] - transform.position).normalized;
        targetSpeed = direction * speed;
        speedDiff = targetSpeed - rigidBody.velocity;
        float accelRate = (Mathf.Abs(direction.x) > 0.01f || Mathf.Abs(direction.y) > 0.01f)
                        ? acceleration : decceleration;
        rigidBody.AddForce(speedDiff * accelRate);
    }

    private void Stop()
    {
        if (state != State.MovingAfterAttack)
        {
            attackTimer = MIN_ATTACK_COOLDOWN;
        }
        state = State.Attacking;
        rigidBody.velocity = Vector2.zero;
        animator.SetBool(IS_MOVING, false);
    }

    private void OnPathComplete(Path newPath)
    {
        if (!newPath.error)
        {
            path = newPath;
            currentWaypoint = 0;
        }
    }

    protected override void Attack()
    {
        if (state == State.Attacking) animator.SetTrigger(ATTACK);
    }

    protected override void OnAlertChanged(bool isAlert)
    {
        if (animator == null) animator = GetComponent<Animator>();
        animator.SetBool(IS_MOVING, isAlert);
        StartCoroutine(CreatePath());
    }

    protected virtual void ChangeAttackPoint()
    {
        if (isFacingRight) attackPoint = transform.Find(ATTACK_POINT_RIGHT);
        else attackPoint = transform.Find(ATTACK_POINT_LEFT);
    }

    protected virtual void ChangeFacingDirection()
    {
        isFacingRight = !isFacingRight;
        animator.SetFloat(DIRECTION, rigidBody.velocity.normalized.x);
    }

    protected virtual void TurnAround()
    {
        isFacingRight = !isFacingRight;
        direction = (target.position - transform.position).normalized;
        animator.SetFloat(DIRECTION, direction.x);
    }

    private IEnumerator CreatePath()
    {
        state = State.Moving;
        path = null;
        while (state == State.Moving)
        {
            yield return seekCooldown;
            if (seeker.IsDone() && (target != null) && (state == State.Moving)) seeker.StartPath(transform.position, target.position, OnPathComplete);
        }
    }

    protected void MoveAfterShooting()
    {
        state = State.MovingAfterAttack;
        path = null;
        retreatPosition = transform.position;
        directionToTarget = (target.position - transform.position).normalized;
        // so if direction's x and y have the same sign - vec to new pos has different signs
        // else - vec to new pos has same signes
        if (((directionToTarget.x > 0) && (directionToTarget.y > 0)) ||
            ((directionToTarget.x < 0) && (directionToTarget.y < 0)))
        {
            if (Random.Range(0, 100) > 50)
            {
                DoLeftRetreat(true);
            }
            else
            {
                DoRightRetreat(true);
            }
        }
        else
        {
            if (Random.Range(0, 100) > 50)
            {
                DoLeftRetreat(false);
            }
            else
            {
                DoRightRetreat(false);
            }
        }

        animator.SetBool(IS_MOVING, true);
        if (seeker.IsDone()) seeker.StartPath(transform.position, retreatPosition, OnPathComplete);
    }

    private void DoLeftRetreat(bool isDirectionSameSign)
    {
        if (rightRetreatCount != 0)
        {
            leftRetreatCount = 0;
            rightRetreatCount = 0;
        }

        if (leftRetreatCount == MAX_SAME_RETREAT_CHANGE) DoRightRetreat(isDirectionSameSign);
        else
        {
            if (isDirectionSameSign)
            {
                retreatPosition.x -= Random.Range(2f, 2.75f);
                retreatPosition.y += Random.Range(2f, 2.75f);
            }
            else
            {
                retreatPosition.x -= Random.Range(2f, 2.75f);
                retreatPosition.y -= Random.Range(2f, 2.75f);
            }
        }
        leftRetreatCount++;
    }

    private void DoRightRetreat(bool isDirectionSameSign)
    {
        if (leftRetreatCount != 0)
        {
            leftRetreatCount = 0;
            rightRetreatCount = 0;
        }

        if (rightRetreatCount == MAX_SAME_RETREAT_CHANGE) DoLeftRetreat(isDirectionSameSign);
        else
        {
            if (isDirectionSameSign)
            {
                retreatPosition.x += Random.Range(2f, 2.75f);
                retreatPosition.y -= Random.Range(2f, 2.75f);
            }
            else
            {
                retreatPosition.x += Random.Range(2f, 2.75f);
                retreatPosition.y += Random.Range(2f, 2.75f);
            }
        }
        rightRetreatCount++;
    }

    protected override void PlayDeathAnimation()
    {
        StartCoroutine(DeathAnimation());
    }

    protected override void DestroyAll()
    {
        rigidBody.velocity = Vector2.zero;
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        base.DestroyAll();
    }

    private IEnumerator DeathAnimation()
    {
        animator.SetTrigger(DEATH);
        state = State.Dying;
        yield return new WaitForSeconds(1f);
        state = State.NotAlert;
    }
}
