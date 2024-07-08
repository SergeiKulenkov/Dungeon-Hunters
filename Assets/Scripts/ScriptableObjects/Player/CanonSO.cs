using UnityEngine;

[CreateAssetMenu(fileName = Definitions.CANON_CONFIG,
menuName = Definitions.SCRIPTABLE_OBJECTS_PATH + Definitions.PLAYER + "/" + Definitions.CANON_CONFIG)]
public class CanonSO : ScriptableObject
{
    public float PumpkinSpeed;
    public int Damage;
    public float Cooldown;
    public float RecoilMin;
    public float RecoilMax;
}
