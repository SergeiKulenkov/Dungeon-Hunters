using UnityEngine;

[CreateAssetMenu(fileName = Definitions.HEALTH_UPGRADE_CONFIG,
menuName = Definitions.SCRIPTABLE_OBJECTS_PATH + Definitions.PLAYER + "/" + Definitions.HEALTH_UPGRADE_CONFIG)]
public class HealthUpgradeSO : ScriptableObject
{
    public Sprite Image;
    public int Price;
    public int UpgradeAmount;
}
