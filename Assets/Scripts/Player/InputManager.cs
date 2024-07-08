using UnityEngine;
using System;

public class InputManager : MonoBehaviour
{
    ///////////////////////////////////////////
    // Fields
    private bool isNearInteractableItem;
    private bool isPlayerDead;

    // events
    public static event Action OnPausePressed;
    public static event Action OnResumePressed;
    public static event Action OnInteractPressed;

    ///////////////////////////////////////////
    // Methods

    private void Start()
    {
        Player.OnPlayerDied += () => isPlayerDead = true;
        InteractableItemController.OnNearInteractableItem += (bool isNear, Transform item) => isNearInteractableItem = isNear;
    }

    private void OnDestroy()
    {
        Player.OnPlayerDied -= () => isPlayerDead = true;
        InteractableItemController.OnNearInteractableItem -= (bool isNear, Transform item) => isNearInteractableItem = isNear;
    }

    private void Update()
    {
        if (!isPlayerDead)
        {
            if (!UIManager.IsOptionsOpened && Input.GetKeyDown(KeyCode.Escape))
            {
                if (GameState.IsPaused) OnResumePressed?.Invoke();
                else OnPausePressed?.Invoke();
            }
            else if (isNearInteractableItem && Input.GetKeyDown(KeyCode.E))
            {
                OnInteractPressed?.Invoke();
            }
        }
    }

    public static bool IsShootButtonPressed()
    {
        return !GameState.IsPaused && Input.GetMouseButton(0);
    }

    public static Vector2 GetMovementInput()
    {
        float inputHorizontal = Input.GetAxisRaw(Definitions.INPUT_HORIZONTAL);
        float inputVertical = Input.GetAxisRaw(Definitions.INPUT_VERTICAL);
        return new Vector2(inputHorizontal, inputVertical);
    }
}
