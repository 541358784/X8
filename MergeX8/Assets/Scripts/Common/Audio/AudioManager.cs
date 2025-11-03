using System;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using Gameplay;
using UnityEngine;

namespace DragonPlus
{
    public enum AudioType
    {
        MUSIC,
        SOUND
    }

    public class AudioManager : Manager<AudioManager>
    {
        public bool MusicClose
        {
            get { return DragonU3DSDK.Audio.AudioManager.MusicClose; }
            set { DragonU3DSDK.Audio.AudioManager.MusicClose = value; }
        }

        public bool SoundClose
        {
            get { return DragonU3DSDK.Audio.AudioManager.SoundClose; }
            set { DragonU3DSDK.Audio.AudioManager.SoundClose = value; }
        }

        public float PlayMusic(AudioClip audioClip, bool loop = true)
        {
            if (audioClip == null)
                return -1;
            
            DragonU3DSDK.Audio.AudioManager.PlayMusic(audioClip, 1, loop, true);
            return audioClip.length;
        }
        
        public float PlayMusic(string musicName, bool loop = true)
        {
            AudioClip au = LoadAudio(AudioType.MUSIC, musicName);

            if (au != null)
            {
                DragonU3DSDK.Audio.AudioManager.PlayMusic(au, 1, loop, true);
                return au.length;
            }

            return -1;
        }

        public float PlayMusic(int soundId, bool loop = false)
        {
            string soundName = GlobalConfigManager.Instance.GetSoundName(soundId);

            if (soundName == null || soundName == "")
                return -1;

            return PlayMusic(soundName, loop);
        }

        public float PlaySound(AudioClip audioClip, bool loop = true)
        {
            if (audioClip == null)
                return -1;
            
            if (DragonU3DSDK.Audio.AudioManager.PlaySound(audioClip, loop) > 0)
                return audioClip.length;
            
            return -1;
        }
        
        public float PlaySound(int soundId, bool loop = false)
        {
            string soundName = GlobalConfigManager.Instance.GetSoundName(soundId);

            if (soundName == null || soundName == "")
                return -1;

            return PlaySound(soundName, loop);
        }

        public int PlaySound(string soundName, float volume = 1)
        {
            try
            {
                AudioClip audio = LoadAudio(AudioType.SOUND, soundName);
                if (audio != null)
                {
                    return DragonU3DSDK.Audio.AudioManager.PlaySound(audio);
                }
                // #endif
            }
            catch (Exception e)
            {
                DebugUtil.LogError($"{GetType()}.PlaySound:{soundName} error:{e.Message}");
//                DebugUtil.LogError(e);
            }

            return -1;
        }
        public int PlaySoundById(int soundId, bool loop = false)
        {
            try
            {
                string soundName = GlobalConfigManager.Instance.GetSoundName(soundId);
                if (soundName == null || soundName == "")
                    return -1;
                AudioClip audio = LoadAudio(AudioType.SOUND, soundName);
                if (audio != null)
                {
                    return DragonU3DSDK.Audio.AudioManager.PlaySound(audio,loop);
                }
                // #endif
            }
            catch (Exception e)
            {
                DebugUtil.LogError($"{GetType()}.PlaySound:{soundId} error:{e.Message}");
            }
            return -1;
        }


        public float PlaySound(string soundName, bool loop)
        {
            try
            {
                AudioClip audio = LoadAudio(AudioType.SOUND, soundName);
                if (audio != null)
                {
                    if (DragonU3DSDK.Audio.AudioManager.PlaySound(audio, loop) > 0)
                        return audio.length;
                }

                return -1;
            }
            catch (Exception e)
            {
                DebugUtil.LogError($"{GetType()}.PlaySound:{soundName} error:{e.Message}");
//                DebugUtil.LogError(e);
            }

            return -1;
        }

        public void StopSoundById(int soundId)
        {
// #if UNITY_ANDROID && !UNITY_EDITOR
//             DragonU3DSDK.DragonNativeBridge.StopSound(soundId);
// #else
            DragonU3DSDK.Audio.AudioManager.StopSoundById(soundId);
// #endif
        }

        public void SetMusicEnable(bool isEnable)
        {
            DragonU3DSDK.Audio.AudioManager.globalMusicVolume = isEnable ? 1 : 0;
        }

        public void SetSoundEnable(bool isEnable)
        {
            DragonU3DSDK.Audio.AudioManager.globalSoundsVolume = isEnable ? 1 : 0;
        }

        public void StopAllMusic()
        {
            DragonU3DSDK.Audio.AudioManager.StopAllMusic();
        }

        public void StopAllSound()
        {
            DragonU3DSDK.Audio.AudioManager.StopAllSounds();
        }

        public void PauseAllMusic()
        {
            DragonU3DSDK.Audio.AudioManager.PauseAllMusic();
        }

        public void ResumeAllMusic()
        {
            if (MusicClose)
                return;

            DragonU3DSDK.Audio.AudioManager.ResumeAllMusic();
        }

        private AudioClip LoadAudio(AudioType type, string audioName)
        {
            string prefix = "BGM";
            switch (type)
            {
                case AudioType.MUSIC:
                    prefix = "BGM";
                    break;
                case AudioType.SOUND:
                    prefix = "Sounds";
                    break;
            }

//            DragonU3DSDK.DebugUtil.LogP("audio file:Export/" + "Audios/" + prefix + "/" + audioName, GetType().Name);
            AudioClip _audio =
                ResourcesManager.Instance.LoadResource<AudioClip>(PathManager.AudioPath(prefix, audioName));
            return _audio;
        }
        
        public float PlaySoundByPath(string path, bool loop)
        {
            try
            {
                AudioClip audio = ResourcesManager.Instance.LoadResource<AudioClip>(path);
                if (audio != null)
                {
                    if (DragonU3DSDK.Audio.AudioManager.PlaySound(audio, loop) > 0)
                        return audio.length;
                }
                return -1;
            }
            catch (Exception e)
            {
                DebugUtil.LogError($"{GetType()}.PlaySound:{path} error:{e.Message}");
            }

            return -1;
        }
        
        public void ResumeAllSounds()
        {
            if (MusicClose)
                return;

            DragonU3DSDK.Audio.AudioManager.ResumeAllSounds();
        }
        
        public void PauseAllSounds()
        {
            DragonU3DSDK.Audio.AudioManager.PauseAllSounds();
        }

    }
}