using UnityEngine;

[CreateAssetMenu(fileName = "ScarecrowConfig",
menuName = Definitions.SCRIPTABLE_OBJECTS_PATH + "ScarecrowConfig")]
public class ScarecrowSO : ScriptableObject
{
    public float SeekCooldown;
    public float Speed;
    public int Health;
}

