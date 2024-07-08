using UnityEngine;
using System;

public class CharacterAnimationController : MonoBehaviour
{
    ///////////////////////////////////////////
    // Fields
    private Animator animator;
    private Vector2 movement = new Vector2();

    private bool isMoving;
    private bool isLookingUp;

    public float AimAngle { set; private get; }

    // events
    public event Action OnDirectionChangedToDown;
    public event Action OnDirectionChangedFromDown;
    public event Action OnDirectionChangedToLeft;
    public event Action OnDirectionChangedFromLeft;

    // enums
    private enum VerticalStates { Down = -1, Inactive, Up, }
    private enum HorizontalStates { Left = -1, Inactive, Right, }

    // const
    private const string DEATH = "Death";
    private const string IS_MOVING = "IsMoving";
    private const string RUN_HORIZONTAL = "RunHorizontal";
    private const string RUN_VERTICAL = "RunVertical";
    private const string IDLE_HORIZONTAL = "IdleHorizontal";
    private const string IDLE_VERTICAL = "IdleVertical";
    private const string RUN_HOR = "RunHor";
    private const string RUN_VER = "RunVer";
    private const string IDLE_HOR = "IdleHor";
    private const string IDLE_VER = "IdleVer";

    ///////////////////////////////////////////
    // Methods

    public void InitializeAnimator(RuntimeAnimatorController newAnimator)
    {
        animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = newAnimator;
    }

    private void FixedUpdate()
    {
        HandleDirectionChange();
    }

    private void HandleDirectionChange()
    {
        // the new AimAngle comes from player
        if ((animator.GetFloat(IDLE_VER) != (int) VerticalStates.Up) &&
            ((AimAngle >= 60) && (AimAngle < 120)))
        {
            SetAnimatorDirectionStates(HorizontalStates.Inactive, VerticalStates.Up);
        }
        else if ((animator.GetFloat(IDLE_HOR) != (int) HorizontalStates.Left) &&
                ((AimAngle >= 120) || (AimAngle < -110)))
        {
            SetAnimatorDirectionStates(HorizontalStates.Left, VerticalStates.Inactive);
        }
        else if ((animator.GetFloat(IDLE_VER) != (int) VerticalStates.Down) &&
                ((AimAngle >= -110) && (AimAngle < -70)))
        {
            SetAnimatorDirectionStates(HorizontalStates.Inactive, VerticalStates.Down);
            OnDirectionChangedToDown?.Invoke();
        }
        else if ((animator.GetFloat(IDLE_HOR) != (int) HorizontalStates.Right) &&
                ((AimAngle < 60) && (AimAngle >= -70)))
        {
            SetAnimatorDirectionStates(HorizontalStates.Right, VerticalStates.Inactive);
        }
    }

    public void HandleMovementChange(Vector2 moveInput)
    {
        if ((movement == Vector2.zero) &&
            (moveInput != Vector2.zero))
        {
            isMoving = true;
            animator.SetBool(IS_MOVING, isMoving);
        }
        else if ((movement != Vector2.zero) &&
                (moveInput == Vector2.zero))
        {
            isMoving = false;
            animator.SetBool(IS_MOVING, isMoving);
        }
        movement = moveInput;
    }

    public void PlayDeath() => animator.SetTrigger(DEATH);    

    private void SetAnimatorDirectionStates(HorizontalStates horizontalState, VerticalStates verticalState)
    {
        if (horizontalState == HorizontalStates.Left) OnDirectionChangedToLeft?.Invoke();
        else if ((animator.GetFloat(IDLE_HOR) == (int) HorizontalStates.Left) &&
                ((verticalState == VerticalStates.Up) ||
                (verticalState == VerticalStates.Down))) OnDirectionChangedFromLeft?.Invoke();
        
        if ((animator.GetFloat(IDLE_VER) == (int) VerticalStates.Down) &&
            ((horizontalState == HorizontalStates.Left) ||
            (horizontalState == HorizontalStates.Right)))
        {
            OnDirectionChangedFromDown?.Invoke();
        }

        animator.SetFloat(IDLE_HOR, (int) horizontalState);
        animator.SetFloat(RUN_HOR, (int) horizontalState);

        animator.SetFloat(IDLE_VER, (int) verticalState);
        animator.SetFloat(RUN_VER, (int) verticalState);
    }
}
