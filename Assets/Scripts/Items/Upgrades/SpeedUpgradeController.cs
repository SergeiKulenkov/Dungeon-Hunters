using System;

public class SpeedUpgradeController : InteractableUpgrade
{
    ///////////////////////////////////////////
    // Fields
    protected float upgradeAmount;

    public static event Action<float> OnSpeedUpgraded;

    ///////////////////////////////////////////
    // Methods

    protected override void Start()
    {
        SpeedUpgradeSO upgradeConfig = ConfigManager.GetMaxSpeedUpgradeConfig();
        upgradeAmount = upgradeConfig.UpgradeAmount;
        price = upgradeConfig.Price;
        SetImage(upgradeConfig.Image);
        base.Start();
    }

    protected override void SendUpgradedEvent()
    {
        OnSpeedUpgraded?.Invoke(upgradeAmount);
    }
}
