using UnityEngine;

public class MovementController : MonoBehaviour
{
    ///////////////////////////////////////////
    // Fields
    private Rigidbody2D rigidBody;
    private Vector2 moveInput;
    private CharacterAnimationController animationController;

    private Vector2 targetSpeed;
    private Vector2 speedDiff;
    private float acceleration;
    private float decceleration;

    public float Speed { get; private set; }

    ///////////////////////////////////////////
    // Methods

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animationController = GetComponent<CharacterAnimationController>();
    }

    public void InitializeMovement(PlayerSO config)
    {
        acceleration = config.Acceleration;
        decceleration = config.Decceleration;
        
        if (GameState.MaxSpeedUpgraded) Speed = GameState.PlayerSettings.MaxSpeed;
        else Speed = config.Speed;
    }

    private void Update()
    {
        moveInput = InputManager.GetMovementInput();
    }

    private void FixedUpdate()
    {
        Move();
        animationController.HandleMovementChange(moveInput);
    }

    private void Move()
    {
        targetSpeed = moveInput * Speed;
        speedDiff = targetSpeed - rigidBody.velocity;
        float accelRate = (Mathf.Abs(moveInput.x) > 0.01f || Mathf.Abs(moveInput.y) > 0.01f)
                        ? acceleration : decceleration;
        rigidBody.AddForce(speedDiff * accelRate);
    }

    public void StopMovement()
    {
        rigidBody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
    }

    public void ChangeSpeed(float amount)
    {
        Speed += amount;
        int accelRateChange = (int) (Speed / acceleration);
        acceleration += accelRateChange;
        decceleration += accelRateChange;
    }
}
