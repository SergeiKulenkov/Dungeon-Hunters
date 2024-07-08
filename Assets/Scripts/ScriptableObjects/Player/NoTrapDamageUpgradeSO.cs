using UnityEngine;

[CreateAssetMenu(fileName = Definitions.NO_TRAP_DAMAGE_CONFIG,
menuName = Definitions.SCRIPTABLE_OBJECTS_PATH + Definitions.PLAYER + "/" + Definitions.NO_TRAP_DAMAGE_CONFIG)]
public class NoTrapDamageUpgradeSO : ScriptableObject
{
    public Sprite Image;
    public int Price;
}