using System.Collections.Generic;
using Dlugin;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Audio;
using Framework;
using Gameplay;
using UnityEngine;

namespace TMatch
{
    public enum AudioType
    {
        MUSIC,
        SOUND
    }

    public class AudioManager : Manager<AudioManager>
    {
        private Dictionary<string, AudioClip> _audioCache = new Dictionary<string, AudioClip>();

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

        public void PlayMusic(AudioClip clip, float volume = 1, bool loop = true)
        {
            if (clip != null)
            {
                DragonU3DSDK.Audio.AudioManager.PlayMusic(clip, volume, loop, true);
            }
            else
            {
                DebugUtil.Log($"Play music failed! Asset not exist.");
            }
        }
        public void PlayMusic(string musicName, bool loop = true, float volume = 1)
        {
            PlayMusic(LoadAudio(AudioType.MUSIC, musicName), volume, loop);
        }

        public void PlayHomeMusic(string musicName, bool loop = true)
        {
            var au = ResourcesManager.Instance.LoadResource<AudioClip>(musicName);
            if (au != null)
            {
                DragonU3DSDK.Audio.AudioManager.PlayMusic(au, 1, loop, true);
            }
        }
        
        public void PlaySound(string soundName, float volume = 1)
        {
            var audio = LoadAudio(AudioType.SOUND, soundName);
            if (audio != null)
            {
                DragonU3DSDK.Audio.AudioManager.PlaySound(audio);
            }
            else
            {
                DebugUtil.LogWarning($"Play sound failed! Asset {soundName} not exist.");
            }
        }
        
        public void PauseSound(int soundId)
        {
            var sound = DragonU3DSDK.Audio.AudioManager.GetSoundAudio(soundId);
            if (sound == null || sound.paused)
                return;
            sound.Pause();
        }
        public void ResumeSound(int soundId)
        {
            var sound = DragonU3DSDK.Audio.AudioManager.GetSoundAudio(soundId);
            if (sound == null || !sound.paused)
                return;
            sound.Resume();
        }
        
        public int PlaySound(string soundName, bool loop)
        {
            var audio = LoadAudio(AudioType.SOUND, soundName);
            if (audio != null)
            {
                return DragonU3DSDK.Audio.AudioManager.PlaySound(audio, loop);
            }
            else
            {
                DebugUtil.Log($"Play sound failed! Asset {soundName} not exist.");
            }

            return -1;
        }

        public void PlayBtnTap()
        {
            PlaySound(SfxNameConst.UIBtnTap);
        }

        public void StopSoundById(int soundId)
        {
            DragonU3DSDK.Audio.AudioManager.StopSoundById(soundId);
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

            if (!DragonU3DSDK.Audio.AudioManager.AnyMusicPlaying())
            {
                // switch (MyMain.myGame.Fsm.CurrentState.Type)
                // {
                //     case FsmStateType.Decoration:
                //     case FsmStateType.Hospital:
                //     case FsmStateType.Merge:
                //         MyMain.myGame.Fsm.CurrentState.PlayBgm();
                //         break;
                // }
            }
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

            var audioPath = $"Audios/{prefix}/{audioName}";

            if (!_audioCache.TryGetValue(audioPath, out var result))
            {
                result = ResourcesManager.Instance.LoadResource<AudioClip>(audioPath);
                _audioCache.Add(audioPath, result);
            }

            return result;
        }
    }
}