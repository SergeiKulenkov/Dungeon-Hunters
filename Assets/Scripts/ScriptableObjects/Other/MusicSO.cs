using UnityEngine;

[CreateAssetMenu(fileName = Definitions.MUSIC_CONFIG,
menuName = Definitions.SCRIPTABLE_OBJECTS_PATH + Definitions.MUSIC_CONFIG)]
public class MusicSO : ScriptableObject
{
    public float MusicFadeTime;
    public float InroFadeTime;
    public float MusicVolumeInPause;
    public AudioClip[] FirstLocationIntros;
    public AudioClip[] FirstLocationVerses;
    public AudioClip Victory;
}
