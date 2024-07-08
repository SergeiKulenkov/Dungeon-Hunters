using UnityEngine;

[CreateAssetMenu(fileName = Definitions.PISTOL_CONFIG,
menuName = Definitions.SCRIPTABLE_OBJECTS_PATH + Definitions.PLAYER + "/" + Definitions.PISTOL_CONFIG)]
public class PistolSO : ScriptableObject
{
    public float BuletSpeed;
    public int Damage;
    public float Cooldown;
    public float RecoilMin;
    public float RecoilMax;
}