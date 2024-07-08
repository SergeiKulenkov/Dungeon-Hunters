using UnityEngine;
using System;

public class GunRotationController : MonoBehaviour
{
    ///////////////////////////////////////////
    // Fields
    [SerializeField] private Vector2 gunOffsetRightPosition;
    [SerializeField] private Vector2 gunOffsetLeftPosition;
    [SerializeField] private Vector2 gunOffsetDownRightPosition;
    [SerializeField] private Vector2 gunOffsetDownLeftPosition;
    [SerializeField] private Vector3 gunOffsetLeftAngle;
    [SerializeField] private Vector3 gunOffsetDownAngle;
    [SerializeField] private Sprite armDownRight;
    [SerializeField] private Sprite armDownLeft;
    [SerializeField] private Sprite armRight;
    [SerializeField] private Sprite armLeft;

    private Transform aimingHand;
    private Transform gun;
    private SpriteRenderer gunSprite;
    private SpriteRenderer handSprite;

    private Vector3 newGunAngle = new Vector3();
    private bool isGunInRightHand = true;
    private bool isRightArmSpriteDown;
    private bool isLeftArmSpriteDown;
    
    public float AimAngle { set; private get; }

    // events
    public event Action<bool> OnAimingHandChanged;

    // const
    private const int LEFT_SWITCH_ANGLE_DOWN = -100;
    private const int LEFT_SWITCH_ANGLE_UP = 105;
    private const int RIGHT_SWITCH_ANGLE_DOWN = -80;
    private const int RIGHT_SWITCH_ANGLE_UP = 75;
    private const int FLIP_ANGLE = 180;

    ///////////////////////////////////////////
    // Methods

    private void OnEnable()
    {
        transform.GetComponent<CharacterAnimationController>().OnDirectionChangedToDown += SwapArmSpriteToDown;
        transform.GetComponent<CharacterAnimationController>().OnDirectionChangedFromDown += SwapArmSpriteBack;
    }

    private void Start()
    {
        aimingHand = transform.Find(Definitions.RIGHT_HAND_AIM_PATH);
        handSprite = aimingHand.GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if (aimingHand != null)
        {
            // AimAngle comes from player
            newGunAngle.z = AimAngle;
            if (isGunInRightHand)
            {
                if (!((AimAngle <= LEFT_SWITCH_ANGLE_UP) && (AimAngle >= LEFT_SWITCH_ANGLE_DOWN)))
                {
                    ChangeHands();
                }
            }
            else
            {
                if (!(((AimAngle >= RIGHT_SWITCH_ANGLE_UP) && (AimAngle <= FLIP_ANGLE)) ||
                    ((AimAngle <= RIGHT_SWITCH_ANGLE_DOWN) && (AimAngle >= -FLIP_ANGLE))))
                {
                    ChangeHands();
                }
            }

            aimingHand.eulerAngles = newGunAngle;
        }
    }

    private void SetGunOffset()
    {
        if (!isGunInRightHand)
        {
            gun.localPosition = gunOffsetLeftPosition;
            gun.localEulerAngles = gunOffsetLeftAngle;
        }
        else
        {
            gun.localPosition = gunOffsetRightPosition;
            gun.localEulerAngles = new Vector3();
        }
    }

    private void ChangeHands()
    {
        aimingHand.GetComponent<SpriteRenderer>().enabled = false;
        aimingHand = isGunInRightHand ? transform.Find(Definitions.LEFT_HAND_AIM_PATH)
                                    : transform.Find(Definitions.RIGHT_HAND_AIM_PATH);
        aimingHand.GetComponent<SpriteRenderer>().enabled = true;

        handSprite = aimingHand.GetComponent<SpriteRenderer>();
        // need to check the gun cause arms can rotate without it
        if (gun != null) gun.SetParent(aimingHand, true);
        isGunInRightHand = !isGunInRightHand;

        if (isRightArmSpriteDown || isLeftArmSpriteDown)
        {
            if ((AimAngle < -70) && (AimAngle > -110)) SwapArmSpriteToDown();
            else SwapArmSpriteBack();
        }
        else if (gun != null) SetGunOffset();
        
        OnAimingHandChanged?.Invoke(isGunInRightHand);
    }

    private void SwapArmSpriteToDown()
    {
        if (gunSprite != null) gunSprite.sortingOrder = Definitions.SPRITE_LAYER_ABOVE_PLAYER;
        if (isGunInRightHand)
        {
            handSprite.sprite = armDownRight;
            if (gun != null)
            {
                gun.localPosition = gunOffsetDownRightPosition;
                gun.localEulerAngles = gunOffsetDownAngle;
            }
            isRightArmSpriteDown = true;
        }
        else
        {
            handSprite.sprite = armDownLeft;
            if (gun != null)
            {
                gun.localPosition = gunOffsetDownLeftPosition;
                gun.localEulerAngles = new Vector3();
            }
            isLeftArmSpriteDown = true;
        }
    }

    private void SwapArmSpriteBack()
    {
        if (gunSprite != null) gunSprite.sortingOrder = Definitions.SPRITE_LAYER_BELOW_PLAYER;
        if (isGunInRightHand)
        {
            handSprite.sprite = armRight;
            if (gun != null) SetGunOffset();
            isRightArmSpriteDown = false;
        }
        else
        {
            handSprite.sprite = armLeft;
            if (gun != null) SetGunOffset();
            isLeftArmSpriteDown = false;
        }
    }

    public void SetWeapon()
    {
        gun = aimingHand.GetComponentInChildren<RangedWeapon>().transform;
        gunSprite = gun.Find(Definitions.SPRITE).GetComponent<SpriteRenderer>();
        SetGunOffset();
    }
}
