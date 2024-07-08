using UnityEngine;

[CreateAssetMenu(fileName = Definitions.FIRE_RATE_CONFIG,
menuName = Definitions.SCRIPTABLE_OBJECTS_PATH + Definitions.PLAYER + "/" + Definitions.FIRE_RATE_CONFIG)]
public class FireRateUpgradeSO : ScriptableObject
{
    public Sprite Image;
    public int Price;
    [Header("Percents")]
    public float UpgradeAmount;
}
