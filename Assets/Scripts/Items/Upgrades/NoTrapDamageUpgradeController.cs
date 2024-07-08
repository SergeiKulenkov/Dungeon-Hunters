using System;

public class NoTrapDamageUpgradeController : InteractableUpgrade
{
    ///////////////////////////////////////////
    // Fields

    public static event Action OnNoTrapDamageUpgraded;

    ///////////////////////////////////////////
    // Methods

    protected override void Start()
    {
        NoTrapDamageUpgradeSO upgradeConfig = ConfigManager.GetNoTrapDamageUpgradeConfig();
        price = upgradeConfig.Price;
        SetImage(upgradeConfig.Image);
        base.Start();
    }

    protected override void SendUpgradedEvent()
    {
        OnNoTrapDamageUpgraded?.Invoke();
    }
}