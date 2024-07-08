using UnityEngine;

[System.Serializable]
public class PlayerSettings
{
    public string LastSavedLevelName;
    public string LastSavedScene;
    public int LastSavedHealth;
    public int LastSavedCoins;
    
    public string CharacterName;
    public string Weapon;
    public int MaxHealth;
    public float MaxSpeed;
    public float MaxFireRateChange;
    public bool NoTrapDamageUpgraded;

    public PlayerSettings ()
    {
        LastSavedLevelName = "";
        LastSavedScene = "";
        LastSavedHealth = 0;
        LastSavedCoins = 0;
        CharacterName = "";
        Weapon = "";
        MaxHealth = 0;
        MaxSpeed = 0;
        MaxFireRateChange = 0;
        NoTrapDamageUpgraded = false;
    }

    public PlayerSettings (PlayerSettings newSettings)
    {
        LastSavedLevelName = newSettings.LastSavedLevelName;
        LastSavedScene = newSettings.LastSavedScene;
        LastSavedHealth = newSettings.LastSavedHealth;
        LastSavedCoins = newSettings.LastSavedCoins;
        CharacterName = newSettings.CharacterName;
        Weapon = newSettings.Weapon;
        MaxHealth = newSettings.MaxHealth;
        MaxSpeed = newSettings.MaxSpeed;
        MaxFireRateChange = newSettings.MaxFireRateChange;
        NoTrapDamageUpgraded = newSettings.NoTrapDamageUpgraded;
    }

    public void RemoveUpgrades()
    {
        NoTrapDamageUpgraded = false;
        MaxHealth = 0;
        MaxSpeed = 0;
        MaxFireRateChange = 0;
    }
}

[System.Serializable]
public class PlayerOptions
{
    public float SoundFXSliderValue;
    public float MusicSliderValue;
    public bool IsFullScreen;
    public int QualityIndex;
    public int SelectedResolutionWidth;
    public int SelectedResolutionHeight;
}