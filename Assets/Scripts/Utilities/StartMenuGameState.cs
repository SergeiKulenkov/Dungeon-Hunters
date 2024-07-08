using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuGameState : MonoBehaviour
{
    ///////////////////////////////////////////
    // Fields
    private ChooseFileUIController chooseFileUIScript;
    private PlayerOptions options;

    public string SaveFileName { get; private set; }
    public string SelectedCharacterName { get; private set; }
    public bool IsLoadPressed { get; private set; }
    public static bool IsOptionsLoaded { get; private set; }

    ///////////////////////////////////////////
    // Methods

    private void Start()
    {
        ApplySavedOptions();

        chooseFileUIScript = GameObject.FindObjectOfType<ChooseFileUIController>(true);
        chooseFileUIScript.OnChooseFileConfirmPressed += OnChooseFileConfirmPressed;
        
        OptionsUIController optionsUIScript = GameObject.FindObjectOfType<OptionsUIController>(true);
        optionsUIScript.OnOptionsOkPressed += OnOptionsOkPressed;

        GameObject.FindObjectOfType<StartUIController>(true).OnLoadPressed += OnLoadPressed;

        GameObject.FindObjectOfType<ChooseCharacterUIController>(true).OnChooseCharacterConfirmPressed += () => IsLoadPressed = false;
    }

    public PlayerOptions GetOptions() => options;

    private void OnChooseFileConfirmPressed()
    {
        DontDestroyOnLoad(gameObject);
        if (!IsLoadPressed) SelectedCharacterName = ChooseCharacterUIController.SelectedCharacter;
        SaveFileName = chooseFileUIScript.SaveFileName;
        OptionsUIController.ResetStaticParams();

        if (IsLoadPressed) SceneManager.LoadScene(chooseFileUIScript.LastSavedScene);
        else SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        IsOptionsLoaded = false;
    }

    private void OnOptionsOkPressed()
    {
        if (OptionsUIController.IsOptionsChanged)
        {
            if (options == null) options = new PlayerOptions();
            options.SoundFXSliderValue = OptionsUIController.SoundFXSliderValue;
            options.MusicSliderValue = OptionsUIController.MusicSliderValue;
            if (Screen.fullScreenMode == FullScreenMode.FullScreenWindow) options.IsFullScreen = true;
            else if (Screen.fullScreenMode == FullScreenMode.Windowed) options.IsFullScreen = false;
            options.QualityIndex = QualitySettings.GetQualityLevel();
            options.SelectedResolutionWidth = Screen.width;
            options.SelectedResolutionHeight = Screen.height;
            SaveLoad.SaveOptions(options);
        }
    }

    private void ApplySavedOptions()
    {
        options = SaveLoad.LoadOptions();
        if (options != null)
        {
            if (options.IsFullScreen) Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            else Screen.fullScreenMode = FullScreenMode.Windowed;
            QualitySettings.SetQualityLevel(options.QualityIndex);
            Screen.SetResolution(options.SelectedResolutionWidth, options.SelectedResolutionHeight, Screen.fullScreen);
            OptionsUIController optionsScript = GameObject.FindObjectOfType<OptionsUIController>(true);
            optionsScript.SetSlidersValues(options.SoundFXSliderValue, options.MusicSliderValue);
            optionsScript.SetLoadedOptions();
            IsOptionsLoaded = true;
        }
    }

    private void OnLoadPressed()
    {
        if (!IsLoadPressed) IsLoadPressed = true;
    }
}
