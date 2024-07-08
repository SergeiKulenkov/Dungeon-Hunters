using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    ///////////////////////////////////////
    // Fields
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private SoundsSO soundsConfig;
    private AudioSource oneShotAudio;

    public static SoundManager Instance { get; private set; }

    // const
    private const string FX_GROUP_NAME = "FX";

    /////////////////////////////////////////
    // Methods

    private void Awake()
    {
        if(Instance == null)
        {    
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        oneShotAudio = GetComponent<AudioSource>();
        oneShotAudio.outputAudioMixerGroup = audioMixer.FindMatchingGroups(FX_GROUP_NAME)[0];
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
    
    public void PlaySound(Definitions.Sounds sound)
    {
        oneShotAudio.PlayOneShot(GetAudioClip(sound));
    }

    private AudioClip GetAudioClip(Definitions.Sounds sound)
    {
        AudioClip audioClip = null;
        foreach (SoundsSO.SoundAudioClip soundClip in soundsConfig.AudioClips)
        {
            if (soundClip.sound == sound)
            {
                audioClip = soundClip.clip;
                break;
            }
        }

        return audioClip;
    }

    private void OnExitToMainMenuPressed()
    {
        Destroy(gameObject);
    }
}
