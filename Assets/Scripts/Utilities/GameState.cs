using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class GameState : MonoBehaviour
{
    ///////////////////////////////////////////
    // Fields
    [SerializeField] private Transform playerObject;
    [SerializeField] private float sceneTransitionTime;
    [SerializeField] private float gameExitTime;
    [SerializeField] private string firstSceneName;
    [SerializeField] private Vector2 playerPosition;

    private static GameState instance;
    private static PlayerOptions options;
    private WaitForSeconds sceneTransitionDelay;

    private int lastSavedMaxHealth;
    private float lastSavedMaxSpeed;
    private float lastSavedMaxFireRateChange;
    private bool lastSavedNoTrapDamageUpgraded;

    public static bool IsPaused { get; private set; }
    public static bool IsLoaded { get; private set; }

    public static int CurrentHealth { get; private set; }
    public static int Coins { get; private set; }
    public static string CurrentLevelName { get; private set; }
    public static string CurrentScene { get; private set; }

    public static int HealthDropsCount { get; private set; }
    public static int NotSpawnedCoinsCount { get; private set; }
    public static int NotSpawnedHearts { get; private set; }
    public static bool IsSkippingHealthDrops { get; private set; }

    public static PlayerSettings PlayerSettings { get; private set; }
    public static bool MaxSpeedUpgraded { get; private set; }
    public static bool MaxHealthUpgraded { get; private set; }
    public static bool MaxFireRateUpgraded { get; private set; }

    public static Action OnAllFieldsInitialized;

    ///////////////////////////////////////////
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

        // so we can get here when
        // starting new game
        // loading a saved game on the 1st level
        // loading a saved game not on the 1st level
        // going between levels - spawn player, reset isPaused and other stuff
        
        if (PlayerSettings == null)
        {
            PlayerSettings = new PlayerSettings();
            GetValuesFromStartMenu();
            PlayerSettings loadedSettings = SaveLoad.LoadSettings();
            if (loadedSettings != null)
            {
                PlayerSettings = loadedSettings;
                CurrentLevelName = PlayerSettings.LastSavedLevelName;
                Coins = PlayerSettings.LastSavedCoins;
                IsLoaded = true;
                ResetDropsValues();
            }
        }
        else
        {
            OptionsUIController optionsScript = GameObject.FindObjectOfType<OptionsUIController>(true);
            optionsScript.SetSlidersValues(options.SoundFXSliderValue, options.MusicSliderValue);
            optionsScript.SetLoadedOptions();
        }

        CurrentScene = SceneManager.GetActiveScene().name;
        if (string.IsNullOrEmpty(CurrentLevelName) ||
            (!CurrentScene.Contains(Definitions.LAST_LEVEL) &&
            !CurrentScene.Contains(Definitions.FINAL_LEVEL) &&
            CurrentLevelName.Contains(Definitions.LAST_LEVEL_INDEX.ToString())))
        {
            CurrentLevelName = Definitions.LEVEL_1;
            ResetDropsValues();
        }

        if (sceneTransitionDelay == null) sceneTransitionDelay = new WaitForSeconds(sceneTransitionTime);

        // TO REMOVE WHEN ADDING 2ND CHARACTER 
        // PlayerSettings.CharacterName = Definitions.SPACE_GUY;
        
        InitializeUpgradeFlags();
        InitializePlayer();

        GameObject.FindObjectOfType<OptionsUIController>(true).OnOptionsOkPressed += OnOptionsOkPressed;

        IsPaused = false;
        OnAllFieldsInitialized?.Invoke();
    }

    private void Start()
    {
        InputManager.OnPausePressed += () => IsPaused = true;
        InputManager.OnResumePressed += () => IsPaused = false;

        PauseUIController.OnResumePressed += () => IsPaused = false;
        PauseUIController.OnRestartPressed += OnRestartPressed;
        PauseUIController.OnExitToMainMenuPressed += OnExitToMainMenuPressed;
        PauseUIController.OnQuitPressed += OnQuitPressed;

        FinishUIController.OnRestartPressed += OnRestartPressed;
        FinishUIController.OnExitToMainMenuPressed += OnExitToMainMenuPressed;
        FinishUIController.OnQuitPressed += OnQuitPressed;

        LastCorridorController.OnExited += OnExitedLevel;
        BonfireController.OnSaved += OnSaved;
        Player.OnWeaponPickedUp += OnWeaponPickedUp;

        HealthUpgradeController.OnHealthUpgraded += OnHealthUpgraded;
        SpeedUpgradeController.OnSpeedUpgraded += OnSpeedUpgraded;
        FireRateUpgradeController.OnFireRateUpgraded += OnFireRateUpgraded;
        NoTrapDamageUpgradeController.OnNoTrapDamageUpgraded += OnNoTrapDamageUpgraded;
        InteractableUpgrade.OnUpgraded += OnUpgraded;
        
    }

    private void OnDestroy()
    {
        InputManager.OnPausePressed -= () => IsPaused = true;
        InputManager.OnResumePressed -= () => IsPaused = false;

        PauseUIController.OnResumePressed -= () => IsPaused = false;
        PauseUIController.OnRestartPressed -= OnRestartPressed;
        PauseUIController.OnExitToMainMenuPressed -= OnExitToMainMenuPressed;
        PauseUIController.OnQuitPressed -= OnQuitPressed;
        
        FinishUIController.OnRestartPressed -= OnRestartPressed;
        FinishUIController.OnExitToMainMenuPressed -= OnExitToMainMenuPressed;
        FinishUIController.OnQuitPressed -= OnQuitPressed;

        LastCorridorController.OnExited -= OnExitedLevel;
        BonfireController.OnSaved -= OnSaved;
        Player.OnWeaponPickedUp -= OnWeaponPickedUp;

        HealthUpgradeController.OnHealthUpgraded -= OnHealthUpgraded;
        SpeedUpgradeController.OnSpeedUpgraded -= OnSpeedUpgraded;
        FireRateUpgradeController.OnFireRateUpgraded -= OnFireRateUpgraded;
        NoTrapDamageUpgradeController.OnNoTrapDamageUpgraded -= OnNoTrapDamageUpgraded;
        InteractableUpgrade.OnUpgraded -= OnUpgraded;
    }

    private void InitializePlayer()
    {
        Transform player = Instantiate(playerObject, playerPosition, Quaternion.identity);
        switch (PlayerSettings.CharacterName)
        {
            case Definitions.SPACE_GUY: player.gameObject.AddComponent<SpaceGuyController>();
                break;
            case Definitions.RANGER: player.gameObject.AddComponent<RangerController>();
                break;
        }
    }

    private void GetValuesFromStartMenu()
    {
        StartMenuGameState startMenuGameState = GameObject.FindObjectOfType<StartMenuGameState>();
        if (startMenuGameState != null)
        {
            SaveLoad.InitializeSaveFileName(startMenuGameState.SaveFileName);
            if (!startMenuGameState.IsLoadPressed) SaveLoad.DeletePastSaveWithSameName();

            if (!string.IsNullOrEmpty(startMenuGameState.SelectedCharacterName) && (PlayerSettings != null))
            {
                PlayerSettings.CharacterName = startMenuGameState.SelectedCharacterName;
            }

            PlayerOptions optionsFromStart = startMenuGameState.GetOptions();
            if (optionsFromStart != null)
            {
                options = new PlayerOptions();
                options.MusicSliderValue = optionsFromStart.MusicSliderValue;
                options.SoundFXSliderValue = optionsFromStart.SoundFXSliderValue;

                OptionsUIController optionsScript = GameObject.FindObjectOfType<OptionsUIController>(true);
                optionsScript.SetSlidersValues(optionsFromStart.SoundFXSliderValue, optionsFromStart.MusicSliderValue);
                optionsScript.SetLoadedOptions();
            }

            Destroy(startMenuGameState.gameObject);
        }
    }

    private void OnExitedLevel()
    {
        IsLoaded = false;
        CurrentHealth = Player.Health;

        if (!CurrentLevelName.Contains(Definitions.LAST_LEVEL_INDEX.ToString()))
        {
            HealthDropsCount = MapManager.HealthDropsCount;
            NotSpawnedCoinsCount = MapManager.NotSpawnedCoinsCount;
            NotSpawnedHearts = MapManager.NotSpawnedHearts;
            IsSkippingHealthDrops = MapManager.IsSkippingHealthDrops;

            int newLevelIndex = (int)System.Char.GetNumericValue(CurrentLevelName[CurrentLevelName.Length - 1]) + 1;
            CurrentLevelName = CurrentLevelName.Remove(CurrentLevelName.Length - 1) + newLevelIndex.ToString();
        }

        SoundManager.Instance.PlaySound(Definitions.Sounds.Enter);
        StartCoroutine(LoadNextLevel());
    }

    private IEnumerator LoadNextLevel()
    {
        GameObject.FindObjectOfType<UIManager>().MakeSceneTransition();
        yield return sceneTransitionDelay;
        if (CurrentLevelName.Contains(Definitions.LAST_LEVEL_INDEX.ToString()))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void OnRestartPressed(bool isLevelRestart)
    {
        if (isLevelRestart) StartCoroutine(RestartFromSave());
        else StartCoroutine(RestartGame());
    }

    private IEnumerator RestartFromSave()
    {
        IsLoaded = SaveLoad.IsLevelSaved();
        CurrentHealth = 0;
        Coins = PlayerSettings.LastSavedCoins;
        CurrentLevelName = PlayerSettings.LastSavedLevelName;

        if (CurrentLevelName.Contains(Definitions.LEVEL_1) && PlayerSettings.LastSavedScene.Contains(Definitions.FIRST_LOCATION))
        {
            PlayerSettings.Weapon = "";
            Coins = 0;
        }
        else if (((PlayerSettings.LastSavedScene != CurrentScene) ||
                ((PlayerSettings.LastSavedScene.Contains(Definitions.LAST_LEVEL)) && CurrentScene.Contains(Definitions.LAST_LEVEL))) &&
                (MaxFireRateUpgraded || MaxHealthUpgraded || MaxSpeedUpgraded || PlayerSettings.NoTrapDamageUpgraded))
        {
            ChangeUpgradesToLastSaved();
        }

        Time.timeScale = 1f;
        GameObject.FindObjectOfType<UIManager>().MakeSceneTransition();
        yield return sceneTransitionDelay;
        SceneManager.LoadScene(PlayerSettings.LastSavedScene);
    }

    private IEnumerator RestartGame()
    {
        string characterName = PlayerSettings.CharacterName;
        PlayerSettings = new PlayerSettings();
        PlayerSettings.CharacterName = characterName;

        IsLoaded = false;
        CurrentHealth = 0;
        Coins = 0;
        CurrentLevelName = "";
        CurrentScene = "";
        
        Time.timeScale = 1f;
        GameObject.FindObjectOfType<UIManager>().MakeSceneTransition();
        yield return sceneTransitionDelay;
        SceneManager.LoadScene(firstSceneName);
    }

    private void OnSaved(bool isSaved)
    {
        if (isSaved)
        {
            PlayerSettings.LastSavedHealth = Player.Health;
            PlayerSettings.LastSavedCoins = Coins;
            PlayerSettings.LastSavedLevelName = CurrentLevelName;
            PlayerSettings.LastSavedScene = CurrentScene;

            lastSavedMaxFireRateChange = PlayerSettings.MaxFireRateChange;
            lastSavedMaxHealth = PlayerSettings.MaxHealth;
            lastSavedMaxSpeed = PlayerSettings.MaxSpeed;
            lastSavedNoTrapDamageUpgraded = PlayerSettings.NoTrapDamageUpgraded;

            SaveLoad.SaveSettings(PlayerSettings);
        }
    }

    private void OnWeaponPickedUp()
    {
        PlayerSettings.Weapon = GameObject.FindObjectOfType<Player>().GetWeaponName();
    }

    private void OnHealthUpgraded(int upgradeAmount)
    {
        PlayerSettings.MaxHealth = Player.MaxHealth + upgradeAmount;
        SaveLoad.SaveSettings(PlayerSettings);
        MaxHealthUpgraded = true;
    }

    private void OnSpeedUpgraded(float upgradeAmount)
    {
        MovementController movement = GameObject.FindObjectOfType<Player>().transform.GetComponent<MovementController>();
        movement.ChangeSpeed(upgradeAmount);
        PlayerSettings.MaxSpeed = movement.Speed;
        SaveLoad.SaveSettings(PlayerSettings);
        MaxSpeedUpgraded = true;
    }

    private void OnFireRateUpgraded(float upgradeAmount)
    {
        Transform player = GameObject.FindObjectOfType<Player>().transform;
        player.GetComponentInChildren<RangedWeaponPlayer>().ChangeFireRate(upgradeAmount);
        PlayerSettings.MaxFireRateChange = upgradeAmount;
        SaveLoad.SaveSettings(PlayerSettings);
        MaxFireRateUpgraded = true;
    }

    private void OnNoTrapDamageUpgraded()
    {
        PlayerSettings.NoTrapDamageUpgraded = true;
        SaveLoad.SaveSettings(PlayerSettings);
    }

    private void OnUpgraded(int price)
    {
        Coins -= price;
        GameObject.FindObjectOfType<HUDController>().UpdateCoinsText(Coins);
    }

    private void InitializeUpgradeFlags()
    {
        MaxHealthUpgraded = PlayerSettings.MaxHealth != 0;
        MaxSpeedUpgraded = PlayerSettings.MaxSpeed != 0;
        MaxFireRateUpgraded = PlayerSettings.MaxFireRateChange != 0;
    }

    private void ChangeUpgradesToLastSaved()
    {
        PlayerSettings.MaxFireRateChange = lastSavedMaxFireRateChange;
        PlayerSettings.MaxHealth = lastSavedMaxHealth;
        PlayerSettings.MaxSpeed = lastSavedMaxSpeed;
        PlayerSettings.NoTrapDamageUpgraded = lastSavedNoTrapDamageUpgraded;
        InitializeUpgradeFlags();
        SaveLoad.SaveSettings(PlayerSettings);
    }

    public static void AddCoin()
    {
        Coins++;
        GameObject.FindObjectOfType<HUDController>().UpdateCoinsText(Coins);
    }

    private void ResetDropsValues()
    {
        HealthDropsCount = 0;
        NotSpawnedCoinsCount = 0;
        NotSpawnedHearts = 0;
        IsSkippingHealthDrops = false;
    }

    private void OnOptionsOkPressed()
    {
        if (OptionsUIController.IsOptionsChanged)
        {
            if (options == null) options = new PlayerOptions();
            options.SoundFXSliderValue = OptionsUIController.SoundFXSliderValue;
            options.MusicSliderValue = OptionsUIController.MusicSliderValue;
            options.IsFullScreen = Screen.fullScreen;
            options.QualityIndex = QualitySettings.GetQualityLevel();
            options.SelectedResolutionWidth = Screen.width;
            options.SelectedResolutionHeight = Screen.height;
            SaveLoad.SaveOptions(options);
        }
    }

    private void OnQuitPressed()
    {
        if (SaveLoad.IsLevelSaved()) SaveLoad.SaveSettings(PlayerSettings);
        else SaveLoad.DeleteSavedData();
        StartCoroutine(DelayQuit());
    }

    private IEnumerator DelayQuit()
    {
        // use real time cause time scale = 0
        yield return new WaitForSecondsRealtime(gameExitTime);
        Application.Quit();
    }

    private void OnExitToMainMenuPressed()
    {
        if (SaveLoad.IsLevelSaved()) SaveLoad.SaveSettings(PlayerSettings);
        else SaveLoad.DeleteSavedData();
        
        Time.timeScale = 1f;
        OptionsUIController.ResetStaticParams();
        StartCoroutine(DelayExitToMainMenu());
    }

    private IEnumerator DelayExitToMainMenu()
    {
        yield return new WaitForSeconds(gameExitTime);
        SceneManager.LoadScene(0);
        PlayerSettings = null;
        options = null;
        instance = null;
        CurrentLevelName = "";
        IsLoaded = false;
        Destroy(gameObject);
    }
}
