using UnityEngine;

[CreateAssetMenu(fileName = Definitions.SOUNDS_CONFIG,
menuName = Definitions.SCRIPTABLE_OBJECTS_PATH + Definitions.SOUNDS_CONFIG)]
public class SoundsSO : ScriptableObject
{
    public SoundAudioClip[] AudioClips;

    [System.Serializable]
    public class SoundAudioClip
    {
        public Definitions.Sounds sound;
        public AudioClip clip;
    }
}
