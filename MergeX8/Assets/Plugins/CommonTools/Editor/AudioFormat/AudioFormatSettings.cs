using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "AudioFormatSettings", menuName = "ResourceImportSettings/AudioFormatSettings", order = 0)]
public class AudioFormatSettings : ScriptableObject
{
    public List<AudioFormatSetting> settings;
}

[System.Serializable]
public class AudioFormatSetting
{
    public string Name;
    public NameFormatPre PreFix;
    public NameFormatPost PostFix;
    
    

    public bool forceToMono;
    public bool loadInBackground;

    public AudioClipLoadType loadType;

    public AudioCompressionFormat compressionFormat = AudioCompressionFormat.MP3;

    [Range(1,100)]
    public int quality = 100;
    //public bool ambisonic;
    //public bool preloadAudioData;

    public enum NameFormatPre
    {
        none,
        bgm_,
        sfx_
    }

    public enum NameFormatPost
    {
        none,
        _bg,
    }
}

