using UnityEngine;
using System;

public class LastCorridorController : MonoBehaviour
{
    ///////////////////////////////////////////
    // Fields
    private bool entered;

    // events
    public static event Action OnExited;

    ///////////////////////////////////////////
    // Methods

    private void Awake()
    {
        gameObject.layer = Definitions.LAYER_ROOM;
        Definitions.Sides side = Utilities.GetSideByName(transform.name);
        BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;

        Vector2 offset = Utilities.GetBlockingWallOffset(side);
        offset += Utilities.ConvertSideToVector(side);
        collider.offset = offset;
        collider.size = new Vector2(2, 2);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.TryGetComponent<IPlayer>(out IPlayer player))
        {
            if (!entered)
            {
                entered = true;
                OnExited?.Invoke();
            }
        }
    }
}
