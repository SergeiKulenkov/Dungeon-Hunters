using System;

public class FireRateUpgradeController : InteractableUpgrade
{
    ///////////////////////////////////////////
    // Fields
    private float upgradeAmount;

    public static event Action<float> OnFireRateUpgraded;

    ///////////////////////////////////////////
    // Methods

    protected override void Start()
    {
        FireRateUpgradeSO upgradeConfig = ConfigManager.GetFireRateUpgradeConfig();
        upgradeAmount = upgradeConfig.UpgradeAmount;
        price = upgradeConfig.Price;
        SetImage(upgradeConfig.Image);
        base.Start();
    }

    protected override void SendUpgradedEvent()
    {
        OnFireRateUpgraded?.Invoke(upgradeAmount);
    }
}