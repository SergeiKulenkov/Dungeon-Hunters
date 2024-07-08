using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

public class MusicManager : MonoBehaviour
{
    ///////////////////////////////////////
    // Fields
    // [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private List<AudioSource> audioSources;
    [SerializeField] private MusicSO musicConfig;
    private static MusicManager instance;

    private int audioToggle;
    private bool isFading;
    private Coroutine verseCoroutine;
    // real time cause there may be a pause during intro playing
    private WaitForSecondsRealtime verseDelay;

    // const
    private const int INTRO_INDEX = 0;
    private const int VERSE_INDEX = 1;

    /////////////////////////////////////////
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
        
        // foreach (AudioSource source in audioSources)
        // {
        //     source.outputAudioMixerGroup = audioMixer.FindMatchingGroups(Definitions.MUSIC)[0];
        // }
    }

    private void OnEnable()
    {
        InputManager.OnPausePressed += OnPausePressed;
        InputManager.OnResumePressed += OnResumePressed;
        InputManager.OnInteractPressed += OnInteractPressed;

        PauseUIController.OnResumePressed += OnResumePressed;
        PauseUIController.OnRestartPressed += OnRestartPressed;
        PauseUIController.OnExitToMainMenuPressed += OnExitToMainMenuPressed;

        FinishUIController.OnExitToMainMenuPressed += OnExitToMainMenuPressed;
        
        LastCorridorController.OnExited += ChangeToRandomTrack;
    }

    private void Start()
    {
        PlayNewRandomTrack();
    }

    private void OnDestroy()
    {
        InputManager.OnPausePressed -= OnPausePressed;
        InputManager.OnResumePressed -= OnResumePressed;
        InputManager.OnInteractPressed -= OnInteractPressed;
        
        PauseUIController.OnResumePressed -= OnResumePressed;
        PauseUIController.OnRestartPressed -= OnRestartPressed;
        PauseUIController.OnExitToMainMenuPressed -= OnExitToMainMenuPressed;

        FinishUIController.OnExitToMainMenuPressed -= OnExitToMainMenuPressed;

        LastCorridorController.OnExited -= ChangeToRandomTrack;
    }

    private void OnPausePressed() => StartCoroutine(FadeMusic(musicConfig.MusicVolumeInPause, musicConfig.MusicFadeTime));
    private void OnResumePressed() => StartCoroutine(FadeMusic(1, musicConfig.MusicFadeTime));
    private void OnExitToMainMenuPressed() => Destroy(gameObject);

    private void OnRestartPressed(bool isLevelRestart)
    {
        if (isLevelRestart)
        {
            ChangeToRandomTrack();
        }
        else
        {
            StartCoroutine(FadeMusic(1, musicConfig.MusicFadeTime));
        }
    }

    private void PlayNewRandomTrack()
    {
        audioToggle = INTRO_INDEX;
        if (audioSources[audioToggle].volume != 0) audioSources[audioToggle].volume = 0;
        int introIndex = Random.Range(0, musicConfig.FirstLocationIntros.Length);
        audioSources[INTRO_INDEX].clip = musicConfig.FirstLocationIntros[introIndex];
        audioSources[VERSE_INDEX].clip = musicConfig.FirstLocationVerses[introIndex];

        audioSources[audioToggle].Play();
        StartCoroutine(FadeMusic(1, musicConfig.InroFadeTime));
        verseCoroutine = StartCoroutine(SwitchToVerse());
    }

    private void ChangeToRandomTrack()
    {
        // don't change volume cause verse needs to be at 1
        audioSources[audioToggle].Stop();
        if (audioToggle != VERSE_INDEX) StopCoroutine(verseCoroutine);
        PlayNewRandomTrack();
    }

    private void OnChestOpened()
    {
        if (audioToggle != INTRO_INDEX)
        {
            audioSources[audioToggle].volume = 0;
            audioToggle = INTRO_INDEX;
        }
        audioSources[audioToggle].volume = 1;
        // StartCoroutine(ChangeTracks(musicConfig.Victory));
        audioSources[audioToggle].clip = musicConfig.Victory;
        audioSources[audioToggle].Play();
    }

    private void OnInteractPressed()
    {
        ChestController chest = GameObject.FindObjectOfType<ChestController>();
        if (chest != null) chest.OnChestOpened += OnChestOpened;
    }

    private IEnumerator ChangeTracks(AudioClip track)
    {
        StartCoroutine(FadeMusic(0, musicConfig.MusicFadeTime));
        while (isFading)
        {
            yield return null;
        }
        audioSources[audioToggle].clip = track;
        audioSources[audioToggle].Play();
        StartCoroutine(FadeMusic(1, musicConfig.MusicFadeTime));
        yield break;
    }

    private IEnumerator FadeMusic(float targetVolume, float fadeTime)
    {
        isFading = true;
        float currentTime = 0;
        float startVol = audioSources[audioToggle].volume;

        while (currentTime < fadeTime)
        {
            currentTime += Time.unscaledDeltaTime;
            audioSources[audioToggle].volume = Mathf.Lerp(startVol, targetVolume, currentTime / fadeTime);
            yield return null;
        }

        isFading = false;
        yield break;
    }

    private IEnumerator SwitchToVerse()
    {
        verseDelay = new WaitForSecondsRealtime((audioSources[INTRO_INDEX].clip.samples / audioSources[INTRO_INDEX].clip.frequency) - 0.3f);
        yield return verseDelay;
        audioSources[audioToggle].volume = 0;
        audioToggle = 1 - audioToggle;

        if (!GameState.IsPaused)
        {
            if (audioSources[audioToggle].volume != 1) StartCoroutine(FadeMusic(1, musicConfig.MusicFadeTime));
        }
        else audioSources[audioToggle].volume = musicConfig.MusicVolumeInPause;
        audioSources[audioToggle].Play();
    }
}
