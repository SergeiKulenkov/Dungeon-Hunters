using UnityEngine;

[CreateAssetMenu(fileName = Definitions.AUTO_RIFLE_CONFIG,
menuName = Definitions.SCRIPTABLE_OBJECTS_PATH + Definitions.PLAYER + "/" + Definitions.AUTO_RIFLE_CONFIG)]
public class AutoRifleSO : ScriptableObject
{
    public float BuletSpeed;
    public int Damage;
    public float Cooldown;
    public float RecoilMin;
    public float RecoilMax;
}
