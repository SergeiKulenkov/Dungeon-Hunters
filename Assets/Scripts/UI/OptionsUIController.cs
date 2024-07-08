using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System;
using System.Collections.Generic;

public class OptionsUIController : MonoBehaviour
{
    ///////////////////////////////////////////
    // Fields
    [SerializeField] private AudioMixer audioMixer;

    private List<string> resolutionOptions = new List<string>();

    public static float SoundFXSliderValue { get; private set; }
    public static float MusicSliderValue { get; private set; }
    public static bool IsOptionsChanged { get; private set; }
    public static bool SlidersSet { get; private set; }

    // events
    public event Action OnOptionsOkPressed;

    // const
    private const string OPTIONS_PATH = "Options";
    private const string SOUND_VOLUME = "SoundVolume";
    private const string MUSIC_VOLUME = "MusicVolume";
    private const string FULL_SCREEN_OPTION = "FullScreenOption";
    private const string GRAPHICS_QUALITY = "GraphicsQuality";
    private const string RESOLUTION_SELECTION = "ResolutionSelection";
    
    ///////////////////////////////////////////
    // Methods

    private void Awake()
    {
        string option = "";
        string prevOption = "";
        Resolution[] resolutions = Screen.resolutions;

        for (int i = 0; i < resolutions.Length; i++)
        {
            option = resolutions[i].width + "x" + resolutions[i].height;
            if (i > 0)
            {
                if (option != prevOption) resolutionOptions.Add(option);
            }
            else resolutionOptions.Add(option);
            prevOption = option;
        }
    }

    private void Start()
    {
        SetFullScreenToggle();
        SetResolutionDropdown();
        
        if (!SlidersSet)
        {
            SoundFXSliderValue = 1;
            MusicSliderValue = 1;
            SlidersSet = true;
        }

        IsOptionsChanged = false;
    }

    private void SetFullScreenToggle()
    {
        Toggle fullScreenToggle = transform.Find(OPTIONS_PATH + "/" + FULL_SCREEN_OPTION).GetComponentInChildren<Toggle>();
        if (Screen.fullScreenMode == FullScreenMode.FullScreenWindow) fullScreenToggle.isOn = true;
        else if (Screen.fullScreenMode == FullScreenMode.Windowed) fullScreenToggle.isOn = false;
    }

    private void SetResolutionDropdown()
    {
        TMPro.TMP_Dropdown resolutionDropdown = transform.Find(OPTIONS_PATH + "/" + RESOLUTION_SELECTION).GetComponentInChildren<TMPro.TMP_Dropdown>();
        resolutionDropdown.ClearOptions();

        int currentSelectedResolutionInd = 0;
        for (int i = 0; i < resolutionOptions.Count; i++)
        {
            if (resolutionOptions[i] == (Screen.width.ToString() + "x" + Screen.height.ToString()))
            {
                currentSelectedResolutionInd = i;
                break;
            }
        }

        resolutionDropdown.AddOptions(resolutionOptions);
        resolutionDropdown.value = currentSelectedResolutionInd;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetLoadedOptions()
    {
        foreach (Transform child in transform.Find(OPTIONS_PATH))
        {
            if (child.name.Contains(SOUND_VOLUME)) child.GetComponentInChildren<Slider>().value = SoundFXSliderValue;
            else if (child.name.Contains(MUSIC_VOLUME)) child.GetComponentInChildren<Slider>().value = MusicSliderValue;
            else if (child.name.Contains(GRAPHICS_QUALITY)) child.GetComponentInChildren<TMPro.TMP_Dropdown>().value = QualitySettings.GetQualityLevel();
        }
    }

    public void SetSoundFXVolume(float value)
    {
        SoundFXSliderValue = value;
        audioMixer.SetFloat(Definitions.SOUND_VOLUME, Mathf.Log10(value) * 20);
        if (gameObject.activeSelf) IsOptionsChanged = true;
    }

    public void SetMusicVolume(float value)
    {
        MusicSliderValue = value;
        audioMixer.SetFloat(Definitions.MUSIC_VOLUME, Mathf.Log10(value) * 20);
        if (gameObject.activeSelf) IsOptionsChanged = true;
    }

    public void SetFullScreen(bool isFullScreen)
    {
        if (isFullScreen) Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        else Screen.fullScreenMode = FullScreenMode.Windowed;
        if (gameObject.activeSelf) IsOptionsChanged = true;
    }

    public void SetGraphicsQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
        if (gameObject.activeSelf) IsOptionsChanged = true;
    }

    public void SetResolution(int index)
    {
        string newResolution = resolutionOptions[index];
        int indexOfX = newResolution.IndexOf("x");
        int width = Int32.Parse(newResolution.Remove(indexOfX));
        int height = Int32.Parse(newResolution.Remove(0, indexOfX + 1));

        Screen.SetResolution(width, height, Screen.fullScreen);
        if (gameObject.activeSelf) IsOptionsChanged = true;
    }

    public void OnOkButtonPressed() => OnOptionsOkPressed?.Invoke();
    
    public static void ResetStaticParams()
    {
        SlidersSet = false;
        IsOptionsChanged = false;
    }

    public void SetSlidersValues(float soundFXValue, float musicValue)
    {
        SoundFXSliderValue = soundFXValue;
        MusicSliderValue = musicValue;
        SlidersSet = true;
    }
}
