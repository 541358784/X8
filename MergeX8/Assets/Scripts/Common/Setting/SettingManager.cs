/*
 * @file SetModel
 * 设置
 * @author lu
 */

using DragonPlus;
using DragonPlus.Haptics;
using DragonU3DSDK.Storage;
using UnityEngine;

public class SettingManager : Manager<SettingManager>
{
    private const string kMusicClose = "SettingManager_kMusicClose";
    private const string kSoundClose = "SettingManager_kSoundClose";
    private const string kShakeClose = "SettingManager_kShakeClose";

    public void Init()
    {
        AudioManager.Instance.MusicClose = MusicClose;
        AudioManager.Instance.SoundClose = SoundClose;
    }

    private bool GetFunctionEnable(string key, bool defaultValue = false)
    {
        return PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) >= 1;
    }

    private void SetFunctionEnable(string key, bool isEnable)
    {
        PlayerPrefs.SetInt(key, isEnable ? 1 : 0);
        PlayerPrefs.Save();
    }

    public bool MusicClose
    {
        get { return GetFunctionEnable(kMusicClose); }
        set
        {
            SetFunctionEnable(kMusicClose, value);
            AudioManager.Instance.MusicClose = value;
        }
    }

    public bool SoundClose
    {
        get { return GetFunctionEnable(kSoundClose); }
        set
        {
            SetFunctionEnable(kSoundClose, value);
            AudioManager.Instance.SoundClose = value;
        }
    }

    public bool ShakeClose
    {
        get
        {
            return GetFunctionEnable(kShakeClose,
                HapticsManager.IsHapticsSupported() == false || Application.platform == RuntimePlatform.Android); //
        }
        set { SetFunctionEnable(kShakeClose, value); }
    }
}