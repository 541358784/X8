using System;
using DragonPlus.ConfigHub.Ad;
using TileMatch.Event;
using UnityEngine;
using UnityEngine.UI;

public partial class TileMatchMainController
{
    private Animator _settingAnimator;

    private Button _settingButton;
    private Button _closeButton;
    private Button _soundButton;
    private Button _musicButton;
    private Button _backHomeButton;

    private bool _isOpenSetting = false;

    private GameObject _sound_On;
    private GameObject _sound_Off;
    private GameObject _music_On;
    private GameObject _music_Off;
    private void AwakeSetting()
    {
        _settingAnimator = transform.Find("Root/Top/SetGroup").GetComponent<Animator>();

        _settingButton = GetItem<Button>("Root/Top/SetGroup/SetButton");
        _settingButton.onClick.AddListener(OnButtonClick_Setting);
        
        _closeButton = GetItem<Button>("Root/Top/SetGroup/CloseButton");
        _closeButton.onClick.AddListener(OnButtonClick_Close);
        
        _soundButton = GetItem<Button>("Root/Top/SetGroup/SoundButton");
        _soundButton.onClick.AddListener(OnButtonClick_Sound);
        _sound_On = GetItem("Root/Top/SetGroup/SoundButton/OpenIcon");
        _sound_Off = GetItem("Root/Top/SetGroup/SoundButton/CloseIcon");
        
        _musicButton = GetItem<Button>("Root/Top/SetGroup/MusicButton");
        _musicButton.onClick.AddListener(OnButtonClick_Music);
        _music_On = GetItem("Root/Top/SetGroup/MusicButton/OpenIcon");
        _music_Off = GetItem("Root/Top/SetGroup/MusicButton/CloseIcon");
        
        _backHomeButton = GetItem<Button>("Root/Top/SetGroup/HomeButton");
        _backHomeButton.onClick.AddListener(OnButtonClick_Back);
    }

    private void OnButtonClick_Setting()
    {
        _isOpenSetting = !_isOpenSetting;

        PlaySettingAnim(_isOpenSetting ? "appear" : "disappear");
    }
    
    private void OnButtonClick_Close()
    {
        if(!_isOpenSetting)
            return;
        
        _isOpenSetting = false;
        PlaySettingAnim(_isOpenSetting ? "appear" : "disappear");
    }
    
    private void OnButtonClick_Sound()
    {
        SettingManager.Instance.SoundClose = !SettingManager.Instance.SoundClose;
        InitSetting();
    }
    private void OnButtonClick_Music()
    {
        SettingManager.Instance.MusicClose = !SettingManager.Instance.MusicClose;
        InitSetting();
    }
    private void OnButtonClick_Back()
    {
        OnButtonClick_Close();
        TileMatchEventManager.Instance.SendEvent(GameEventConst.GameEvent_Fail,FailTypeEnum.EnterBack);
    }
    
    private void PlaySettingAnim(string anim, Action action = null)
    {
        _settingAnimator.Play(anim);

        if (_isOpenSetting)
        {
            InitSetting();
        }
        if(action == null)
            return;
        
        StartCoroutine(CommonUtils.DelayWork(CommonUtils.GetAnimTime(_settingAnimator, anim), () =>
        {
            action?.Invoke();
        }));
    }

    private void InitSetting()
    {
        _sound_On.gameObject.SetActive(!SettingManager.Instance.SoundClose);
        _sound_Off.gameObject.SetActive(SettingManager.Instance.SoundClose);
        
        _music_On.gameObject.SetActive(!SettingManager.Instance.MusicClose);
        _music_Off.gameObject.SetActive(SettingManager.Instance.MusicClose);
    }
}