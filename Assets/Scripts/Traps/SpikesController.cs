using UnityEngine;

public class SpikesController : MonoBehaviour
{
    ///////////////////////////////////////////
    // Fields
    ///////////////////////////////////////////
    // Methods

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.TryGetComponent<IPlayer>(out IPlayer player))
        {
            player.TakeDamage(Definitions.HALF_HEALTH_POINT);
        }
    }
}
