using UnityEngine;

public class ConfigManager : MonoBehaviour
{
    ///////////////////////////////////////
    // Fields
    private static ConfigManager instance;

    private static WeaponsSO weaponConfig;
    private static SpawnAreaSO spawnAreaConfig;
    private static RoomsAndCorridorsPrefabsSO roomsAndCorridorsPrefabsConfig;
    private static PlayerSO playerConfig;
    private static SpawnersSO spawnersConfig;
    private static RoomsSO roomsConfig;
    private static LastLevelSpawnerSO lastLevelSpawnerConfig;
    private static FinalLevelSpawnerSO finalLevelSpawnerConfig;

    private static string location;

    // const
    private const string CONFIGS = "Configs/";
    private const string OTHER_PATH = CONFIGS + "Other/";

    private const string HEALTH_UPGRADE_CONFIG = CONFIGS + Definitions.PLAYER + "/" + Definitions.HEALTH_UPGRADE_CONFIG;
    private const string SPEED_UPGRADE_CONFIG = CONFIGS + Definitions.PLAYER + "/" + Definitions.SPEED_UPGRADE_CONFIG;
    private const string FIRE_RATE_CONFIG = CONFIGS + Definitions.PLAYER + "/" + Definitions.FIRE_RATE_CONFIG;
    private const string NO_TRAP_DAMAGE_CONFIG = CONFIGS + Definitions.PLAYER + "/" + Definitions.NO_TRAP_DAMAGE_CONFIG;

    ///////////////////////////////////////
    // Methods

    private void Awake()
    {
        if(instance == null)
        {    
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        GameState.OnAllFieldsInitialized += OnAllFieldsInitialized;

        roomsConfig = null;
        spawnersConfig = null;
    }

    private void OnEnable()
    {
        PauseUIController.OnExitToMainMenuPressed += OnExitToMainMenuPressed;

        FinishUIController.OnExitToMainMenuPressed += OnExitToMainMenuPressed;
    }

    private void OnDestroy()
    {
        PauseUIController.OnExitToMainMenuPressed -= OnExitToMainMenuPressed;

        FinishUIController.OnExitToMainMenuPressed -= OnExitToMainMenuPressed;
    }

    private void OnAllFieldsInitialized()
    {
        if (string.IsNullOrEmpty(location) && Utilities.IsFirstLocation()) location = Definitions.FIRST_LOCATION;
        else if (location.Contains(Definitions.FIRST_LOCATION) && Utilities.IsSecondLocation())
        {
            location = Definitions.SECOND_LOCATION;
            lastLevelSpawnerConfig = null;
        }
        else if (location.Contains(Definitions.SECOND_LOCATION) && Utilities.IsFinalLevel())
        {
            location = Definitions.FINAL_LOCATION;
            lastLevelSpawnerConfig = null;
        }
    }
    
    public static HealthUpgradeSO GetMaxHealthUpgradeConfig() => Resources.Load<HealthUpgradeSO>(HEALTH_UPGRADE_CONFIG);
    public static SpeedUpgradeSO GetMaxSpeedUpgradeConfig() => Resources.Load<SpeedUpgradeSO>(SPEED_UPGRADE_CONFIG);
    public static FireRateUpgradeSO GetFireRateUpgradeConfig() => Resources.Load<FireRateUpgradeSO>(FIRE_RATE_CONFIG);
    public static NoTrapDamageUpgradeSO GetNoTrapDamageUpgradeConfig() => Resources.Load<NoTrapDamageUpgradeSO>(NO_TRAP_DAMAGE_CONFIG);
    
    public static WeaponsSO GetWeaponsConfig()
    {
        if (weaponConfig == null) weaponConfig = Resources.Load<WeaponsSO>(OTHER_PATH + Definitions.WEAPONS_CONFIG);
        return weaponConfig;
    }

    public static SpawnAreaSO GetSpawnAreaConfig()
    {
        if (spawnAreaConfig == null) spawnAreaConfig = Resources.Load<SpawnAreaSO>(OTHER_PATH + Definitions.SPAWN_AREA_CONFIG);
        return spawnAreaConfig;
    }

    public static RoomsAndCorridorsPrefabsSO GetRoomsAndCorridorsPrefabsConfig()
    {
        if (roomsAndCorridorsPrefabsConfig == null) roomsAndCorridorsPrefabsConfig = 
                                                    Resources.Load<RoomsAndCorridorsPrefabsSO>(OTHER_PATH + Definitions.ROOMS_CORRIDORS_PREFABS_CONFIG);
        return roomsAndCorridorsPrefabsConfig;
    }
    
    public static LastLevelSpawnerSO GetFinalLevelSpawnerConfig()
    {
        if (lastLevelSpawnerConfig == null) lastLevelSpawnerConfig = Resources.Load<LastLevelSpawnerSO>(CONFIGS + Definitions.FINAL_LOCATION_PATH + Definitions.FINAL_LEVEL_CONFIG);
        return lastLevelSpawnerConfig;
    }
    
    public static FinalLevelSpawnerSO GetFinalLevelConfig()
    {
        if (finalLevelSpawnerConfig == null) finalLevelSpawnerConfig = Resources.Load<FinalLevelSpawnerSO>(CONFIGS + Definitions.FINAL_LOCATION_PATH + Definitions.FINAL_LEVEL_CONFIG);
        return finalLevelSpawnerConfig;
    }

    //////////////////////////////////////////////////////////////
    
    public static PlayerSO GetPlayerConfig(string characterName)
    {
        if (playerConfig == null)
        {
            string configPath = CONFIGS + Definitions.PLAYER + "/";
            if (characterName.Contains(Definitions.SPACE_GUY)) configPath += Definitions.SPACE_GUY_CONFIG;
            else if (characterName.Contains(Definitions.RANGER)) configPath += Definitions.RANGER_CONFIG;
            playerConfig = Resources.Load<PlayerSO>(configPath);
        }

        return playerConfig;
    }
    
    public static SpawnersSO GetSpawnerConfig()
    {
        if (spawnersConfig == null)
        {
            string levelName = GameState.CurrentLevelName;
            string configPath = CONFIGS;
            
            if (Utilities.IsFirstLocation()) configPath += Definitions.FIRST_LOCATION_PATH;
            else if (Utilities.IsSecondLocation()) configPath += Definitions.SECOND_LOCATION_PATH;

            if (levelName.Contains(Definitions.LEVEL_1)) configPath += Definitions.LEVEL_1_CONFIG;
            else if (levelName.Contains(Definitions.LEVEL_2)) configPath += Definitions.LEVEL_2_CONFIG;
            else if (levelName.Contains(Definitions.LEVEL_3)) configPath += Definitions.LEVEL_3_CONFIG;
            
            spawnersConfig = Resources.Load<SpawnersSO>(configPath);
        }

        return spawnersConfig;
    }

    public static RoomsSO GetRoomConfig()
    {
        if (roomsConfig == null)
        {
            string configPath = CONFIGS;
            if (Utilities.IsFirstLocation()) configPath += Definitions.FIRST_LOCATION_PATH;
            else if (Utilities.IsSecondLocation()) configPath += Definitions.SECOND_LOCATION_PATH;
            
            configPath += Definitions.ROOMS_CONFIG;
            roomsConfig = Resources.Load<RoomsSO>(configPath);
        }

        return roomsConfig;
    }

    public static LastLevelSpawnerSO GetLastLevelSpawnerConfig()
    {
        if (lastLevelSpawnerConfig == null)
        {
            string configPath = CONFIGS;
            if (Utilities.IsFirstLocation()) configPath += Definitions.FIRST_LOCATION_PATH;
            else if (Utilities.IsSecondLocation()) configPath += Definitions.SECOND_LOCATION_PATH;
            
            configPath += Definitions.LAST_LEVEL_CONFIG;
            lastLevelSpawnerConfig = Resources.Load<LastLevelSpawnerSO>(configPath);
        }

        return lastLevelSpawnerConfig;
    }

    private void OnExitToMainMenuPressed()
    {
        Destroy(gameObject);
    }
}
