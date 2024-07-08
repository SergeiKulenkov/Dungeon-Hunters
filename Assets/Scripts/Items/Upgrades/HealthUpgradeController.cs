using System;

public class HealthUpgradeController : InteractableUpgrade
{
    ///////////////////////////////////////////
    // Fields
    private int upgradeAmount;

    public static event Action<int> OnHealthUpgraded;

    ///////////////////////////////////////////
    // Methods

    protected override void Start()
    {
        HealthUpgradeSO upgradeConfig = ConfigManager.GetMaxHealthUpgradeConfig();
        upgradeAmount = upgradeConfig.UpgradeAmount;
        price = upgradeConfig.Price;
        SetImage(upgradeConfig.Image);
        base.Start();
    }

    protected override void SendUpgradedEvent()
    {
        OnHealthUpgraded?.Invoke(upgradeAmount);
    }
}
