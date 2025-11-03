using DragonU3DSDK.Asset;
using DragonU3DSDK.Audio;
using UnityEngine;

namespace TMatch
{
    public class AudioSysManager
    {
        private static AudioSysManager _instance;
    
        public static AudioSysManager Instance => _instance ??= new AudioSysManager();

        public void PlaySound(string name)
        {
            var audio = ResourcesManager.Instance.LoadResource<AudioClip>($"TMatch/Prefabs/{name}");
            if (audio == null)
                return;
        
            DragonU3DSDK.Audio.AudioManager.PlaySound(audio);
        }

        public void PlayMusic(string name)
        {
            var audio = ResourcesManager.Instance.LoadResource<AudioClip>($"TMatch/Prefabs/{name}");
            if (audio == null)
                return;
        
            DragonU3DSDK.Audio.AudioManager.PlayMusic(audio, 1, true, false);
        }
    }   
}
