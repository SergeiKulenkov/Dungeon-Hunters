using UnityEngine;

[CreateAssetMenu(fileName = Definitions.WEAPONS_CONFIG,
menuName = Definitions.SCRIPTABLE_OBJECTS_PATH + Definitions.WEAPONS_CONFIG)]
public class WeaponsSO : ScriptableObject
{
    [Header("Weapons")]
    public Transform Pistol;
    public Transform Canon;
    public Transform LaserRifle;
    public Transform AutoRifle;

    // [Header("VFX")]
    // public Transform MuzzleFlash;
}
