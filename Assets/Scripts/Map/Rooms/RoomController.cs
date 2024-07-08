using UnityEngine;
using System;

public class RoomController : MonoBehaviour
{
    ///////////////////////////////////////////
    // Fields
    protected bool entered;
    private Definitions.RoomTypes type;

    // events
    public static event Action<Vector2, Definitions.RoomTypes> OnRoomEntered;
    public static event Action OnRoomExited;

    ///////////////////////////////////////////
    // Methods

    private void Awake()
    {
        Room roomScript = transform.GetComponent<Room>();
        // Last room doesn't need Room script
        if (roomScript != null) type = roomScript.GetRoomType();
        else type = Definitions.RoomTypes.LastLevel;
    }

    protected void SendEnteredEvent() => OnRoomEntered?.Invoke(transform.position, type);
    protected void SendExitedEvent() => OnRoomExited?.Invoke();

    protected virtual void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.TryGetComponent<IPlayer>(out IPlayer player))
        {
            if (!entered)
            {
                // Debug.Log("entered");
                entered = true;
                SendEnteredEvent();
            }
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.TryGetComponent<IPlayer>(out IPlayer player))
        {
            if (entered)
            {
                // Debug.Log("exited");
                entered = false;
                SendExitedEvent();
            }
        }
    }
}
