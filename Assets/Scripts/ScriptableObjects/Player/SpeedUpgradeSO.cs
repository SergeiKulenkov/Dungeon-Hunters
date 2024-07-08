using UnityEngine;

[CreateAssetMenu(fileName = Definitions.SPEED_UPGRADE_CONFIG,
menuName = Definitions.SCRIPTABLE_OBJECTS_PATH + Definitions.PLAYER + "/" + Definitions.SPEED_UPGRADE_CONFIG)]
public class SpeedUpgradeSO : ScriptableObject
{
    public Sprite Image;
    public int Price;
    public float UpgradeAmount;
}
