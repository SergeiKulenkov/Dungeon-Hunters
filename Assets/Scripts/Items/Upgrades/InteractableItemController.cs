using UnityEngine;
using System;

public abstract class InteractableItemController : MonoBehaviour, IInteractable
{
    ///////////////////////////////////////////
    // Fields
    protected bool isEntered;
    protected bool isUsed;

    public Definitions.InteractableItems itemType { get; private set; }

    // events
    public static event Action<bool, Transform> OnNearInteractableItem;

    ///////////////////////////////////////////
    // Methods

    protected virtual void Start()
    {
        InputManager.OnInteractPressed += OnInteractPressed;
    }

    private void OnDestroy()
    {
        InputManager.OnInteractPressed -= OnInteractPressed;
    }
    
    protected void SetType(Definitions.InteractableItems newType) => itemType = newType;
    protected abstract void OnInteractPressed();

    public void SetCanvasState(bool isActive)
    {
        OnNearInteractableItem?.Invoke(isActive, transform);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!isEntered && !isUsed &&
            collider.TryGetComponent<IPlayer>(out IPlayer player))
        {
            isEntered = true;
            SetCanvasState(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (isEntered && collider.TryGetComponent<IPlayer>(out IPlayer player))
        {
            isEntered = false;
            SetCanvasState(false);
        }
    }
}
