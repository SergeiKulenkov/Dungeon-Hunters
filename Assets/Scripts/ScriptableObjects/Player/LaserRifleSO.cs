using UnityEngine;

[CreateAssetMenu(fileName = Definitions.LASER_RIFLE_CONFIG,
menuName = Definitions.SCRIPTABLE_OBJECTS_PATH + Definitions.PLAYER + "/" + Definitions.LASER_RIFLE_CONFIG)]
public class LaserRifleSO : ScriptableObject
{
    public float LaserSpeed;
    public int Damage;
    public float Cooldown;
    public float RecoilMin;
    public float RecoilMax;
}
