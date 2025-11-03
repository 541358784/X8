using System;
using System.Collections.Generic;
using Difference;
using Dlugin;
using DragonPlus;
using DragonU3DSDK.Account;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Game;
using Gameplay.UI.Setting;
using Gameplay.UI.Store.Vip.Model;
using Newtonsoft.Json.Linq;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;


public partial class UIPopupSetController : UIWindowController
{
    private Button _buttonClose;
    private Button _buttonContact;
    private Button _musiceToggle;
    private Button _soundToggle;
    private Button _shakeToggle;
    private Button _settingToggle;

    private LocalizeTextMeshProUGUI _playerIdText;
    private LocalizeTextMeshProUGUI _versionText;
    private StorageCommon _storageCommon;
    private StorageAvatar _storageAvatar;
    private Animator _animator;
    private bool IsInGame = false;

    private Button _buttonEditName;
    private Button _buttonCopyId;
    private InputField _userName;
    private Button _buttonHeadEdit;
    // private Image _headImage;
    // private Image _headFrameImage;
    private RectTransform _headIconRoot;
    private HeadIconNode HeadIcon;
    
    private Slider _levelSlider;
    private LocalizeTextMeshProUGUI _levelText, _levelProgress;
    private Button _levelReward;

    private Button _vipButton;
    private LocalizeTextMeshProUGUI _vipText;

    private LocalizeTextMeshProUGUI _cardNormalText;
    private LocalizeTextMeshProUGUI _cardSuperText;
    private LocalizeTextMeshProUGUI _blindBoxText;
    
    private const int MaxInputLength = 20;

    private List<Transform> _vipIcons = new List<Transform>();
    public override void PrivateAwake()
    {
        _cardNormalText = GetItem<LocalizeTextMeshProUGUI>("Root/Activity2/Content/Card/NormalText");
        _cardSuperText = GetItem<LocalizeTextMeshProUGUI>("Root/Activity2/Content/Card/SuperText");
        _blindBoxText = GetItem<LocalizeTextMeshProUGUI>("Root/Activity2/Content/BlindBox/Text");
        
        GetItem<Button>("Root/Activity2/Content/Card").onClick.AddListener(() =>
        {
            AnimCloseWindow(() =>
            {
                UIManager.Instance.OpenWindow(UINameConst.UIMainCard);
            });
        });
        
        GetItem<Button>("Root/Activity2/Content/BlindBox").onClick.AddListener(() =>
        {
            AnimCloseWindow(() =>
            {
                UIManager.Instance.OpenWindow(UINameConst.UIBlindBoxMain);
            });
        });

        for (int i = 0; i <= 5; i++)
        {
            _vipIcons.Add(transform.Find("Root/PlayerGroup/VIP/Icon/"+i));
        }
        _playerIdText = GetItem<LocalizeTextMeshProUGUI>("Root/BottomGroup/ID/UserIDText");
        _versionText = GetItem<LocalizeTextMeshProUGUI>("Root/BottomGroup/VersionsText");

        _vipButton = GetItem<Button>("Root/PlayerGroup/VIP");
        _vipButton.onClick.AddListener(OnVipClick);
        
        _vipText = GetItem<LocalizeTextMeshProUGUI>("Root/PlayerGroup/VIP/Text");
        
        _buttonClose = GetItem<Button>("Root/ButtonClose");
        _buttonClose.onClick.AddListener(OnCloseClick);

        _buttonContact = GetItem<Button>("Root/BottomGroup/ContactUs");
        _buttonContact.onClick.AddListener(OnContactClick);

        _levelSlider = GetItem<Slider>("Root/PlayerGroup/Slider");
        _levelText = GetItem<LocalizeTextMeshProUGUI>("Root/PlayerGroup/Slider/Star/Text");
        _levelProgress = GetItem<LocalizeTextMeshProUGUI>("Root/PlayerGroup/Slider/ExperienceText");
        _levelReward= GetItem<Button>("Root/PlayerGroup/Slider/Reward/TipButton");
        _levelReward.onClick.AddListener(OnRewardClick);

        _musiceToggle = GetItem<Button>("Root/ButtonGroup/MusicButton");
        _musiceToggle.onClick.AddListener(OnMusicClick);

        _soundToggle = GetItem<Button>("Root/ButtonGroup/SoundButton");
        _soundToggle.onClick.AddListener(OnSoundClick);

        _shakeToggle = GetItem<Button>("Root/ButtonGroup/ShakeButton");
        _shakeToggle.onClick.AddListener(OnShakeClick);
        
        _settingToggle = GetItem<Button>("Root/ButtonGroup/SetButton");
        _settingToggle.onClick.AddListener(OnSetClick);

        _storageCommon = StorageManager.Instance.GetStorage<StorageCommon>();
        _storageAvatar = StorageManager.Instance.GetStorage<StorageHome>().AvatarData;
        
        _animator = transform.GetComponent<Animator>();

        _buttonEditName = GetItem<Button>("Root/PlayerGroup/Name/Button");
        _buttonEditName.onClick.AddListener(() =>
        {
            _userName.ActivateInputField();
        });
        
        _buttonCopyId = GetItem<Button>("Root/BottomGroup/ID/Button");
        _buttonCopyId.onClick.AddListener(() =>
        {
            GUIUtility.systemCopyBuffer = DragonU3DSDK.Utils.PlayerIdToString(_storageCommon.PlayerId);
            CommonUtils.OpenCommonConfirmWindow(new NoticeUIData
            {
                DescString = LocalizationManager.Instance.GetLocalizedString("&key.ui_id_copied")
            });
        });
        
        _userName = GetItem<InputField>("Root/PlayerGroup/Name");
        string name = _storageAvatar.UserName;
        if (name.IsEmptyString())
        {
            name = "Player " + DragonU3DSDK.Utils.PlayerIdToString(_storageCommon.PlayerId);
            _storageAvatar.UserName = name;
        }
        _userName.SetTextWithoutNotify(_storageAvatar.UserName);
        _userName.characterLimit = MaxInputLength;
        
        _buttonHeadEdit = GetItem<Button>("Root/PlayerGroup/HeadGroup");
        _buttonHeadEdit.onClick.AddListener(() =>
        {
            StartCoroutine(CommonUtils.PlayAnimation(_animator, UIAnimationConst.DisAppear, null, () =>
            {
                CloseWindowWithinUIMgr(true);
                UIManager.Instance.OpenUI(UINameConst.UIPopupSetHead);
            }));
            
        });
        transform.Find("Root/PlayerGroup/HeadGroup/BG").gameObject.SetActive(false);
        _headIconRoot = transform.Find("Root/PlayerGroup/HeadGroup/Head") as RectTransform;
        var headRedPoint = transform.Find("Root/PlayerGroup/HeadGroup/RedPoint").gameObject.AddComponent<UIPopupSetHeadRedPoint>();
        headRedPoint.Init();
        UpdateUI();
        UpdateRedPoint(null);


        Awake_Activity();
        EventDispatcher.Instance.AddEventListener(EventEnum.UpdateRedPoint, UpdateRedPoint);
        EventDispatcher.Instance.AddEventListener(EventEnum.UPDATE_HEAD, UpdateHeadIcon);
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.UpdateRedPoint, UpdateRedPoint);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.UPDATE_HEAD, UpdateHeadIcon);

        var oldName = _storageAvatar.UserName;
        _storageAvatar.UserName = !_userName.text.IsEmptyString() ? _userName.text : _storageAvatar.UserName;
        EventDispatcher.Instance.DispatchEventImmediately(EventEnum.UPDATE_NAME);
        if (oldName != _storageAvatar.UserName)
        {
            TeamManager.Instance.UploadMyInfo();   
        }
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        IsInGame = false;
        if (objs != null && objs.Length > 0)
        {
            IsInGame = (bool) objs[0];
        }

        _animator?.Play(UIAnimationConst.Appear, 0);
        //GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventSettingsOpen,"lobby");

        AdjustShield(_musiceToggle);
        AdjustShield(_soundToggle);
        AdjustShield(_shakeToggle);
        UpdateHeadIcon(null);
        XUtility.WaitFrames(1, CheckVipStoreGuide);
    }

    public void CheckVipStoreGuide()
    {
        if (!GuideSubSystem.Instance.IsShowingGuide() &&
            !GuideSubSystem.Instance.isFinished(GuideTriggerPosition.VipStoreDetailClick))
        {
            List<Transform> topLayer = new List<Transform>();
            topLayer.Add(_vipButton.transform);
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.VipStoreDetailClick, _vipButton.transform as RectTransform,
                topLayer: topLayer);
            _vipButton.onClick.AddListener(() =>
            {
                GuideSubSystem.Instance.FinishCurrent(GuideTargetType.VipStoreDetailClick);
            });
            GuideSubSystem.Instance.Trigger(GuideTriggerPosition.VipStoreDetailClick, null);
        }
    }
    private void AdjustShield(Button btn)
    {
        ShieldButtonOnClick shield = btn.gameObject.GetComponent<ShieldButtonOnClick>();
        if (shield == null)
        {
            return;
        }

        shield.shieldButTime = 0.0f;
    }

    protected override void OnCloseWindow(bool destroy = false)
    {
        // if(IsInGame)
        //     LevelDataModel.Instance.GamePause(false);
    }

    private void UpdateRedPoint(BaseEvent obj)
    {
    }

    private void UpdateUI()
    {
        string playerIdStr = DragonU3DSDK.Utils.PlayerIdToString(_storageCommon.PlayerId);
        _playerIdText.SetText(LocalizationManager.Instance.GetLocalizedStringWithFormat("UI_setting_user_id", playerIdStr));

        string resVersion = VersionManager.Instance.GetResDesplayVersion();
        _versionText.SetText(
            LocalizationManager.Instance.GetLocalizedStringWithFormat("UI_setting_version", resVersion));

        SetFuncEnable(_soundToggle, SettingManager.Instance.SoundClose);
        SetFuncEnable(_musiceToggle, SettingManager.Instance.MusicClose);
        SetFuncEnable(_shakeToggle, SettingManager.Instance.ShakeClose);

        _levelSlider.value = ExperenceModel.Instance.GetPercentExp();
        _levelText.SetText(ExperenceModel.Instance.GetLevel().ToString());
        _levelProgress.SetText(string.Format("{0}/{1}", ExperenceModel.Instance.GetExp(), ExperenceModel.Instance.GetCurrentLevelTotalExp()));
        if (ExperenceModel.Instance.IsMaxLevel())
        {
            _levelSlider.value = 1;
            _levelProgress.SetText(string.Format("{0}/{1}", ExperenceModel.Instance.GetExp(), "Max"));
        }
        
        _vipText.SetText(LocalizationManager.Instance.GetLocalizedString("ui_battlepass_vip")+VipStoreModel.Instance.VipLevel());

        _cardNormalText.SetText(CardCollectionModel.Instance.NormalCardThemeCollectStateStr());
        _cardSuperText.SetText(CardCollectionModel.Instance.GoldenCardThemeCollectStateStr());
        _blindBoxText.SetText(BlindBoxModel.Instance.BlindBoxCollectStateStr());
        
        for (var i = 0; i < _vipIcons.Count; i++)
        {
            _vipIcons[i].gameObject.SetActive(i == VipStoreModel.Instance.VipLevel());
        }
    }

    private void OnCloseClick()
    {
        AudioManager.Instance.PlaySound(SfxNameConst.button_s);
        _buttonClose.interactable = false;
        StartCoroutine(CommonUtils.PlayAnimation(_animator, UIAnimationConst.DisAppear, null,
            () => { CloseWindowWithinUIMgr(true); }));
    }

    private void OnVipClick()
    {
        AudioManager.Instance.PlaySound(SfxNameConst.button_s);
        _buttonClose.interactable = false;
        StartCoroutine(CommonUtils.PlayAnimation(_animator, UIAnimationConst.DisAppear, null,
            () =>
            {
                CloseWindowWithinUIMgr(true);
                UIPopupSetVipController.Open(() =>
                {
                    UIManager.Instance.OpenWindow(UINameConst.UIPopupSet1);
                });
            }));
    }
    
    private void OnContactClick()
    {
        AudioManager.Instance.PlaySound(SfxNameConst.button_s);
        StartCoroutine(CommonUtils.PlayAnimation(_animator, UIAnimationConst.DisAppear, null, () =>
        {
            CloseWindowWithinUIMgr(true);
            UIManager.Instance.OpenUI(UINameConst.UIContactUs);
        }));
    }

    private void OnRewardClick()
    {
        var config = ExperenceModel.Instance.GetCurrentLevelConfig();
        if (config == null)
            return;

        int[] reward = config.reward;
        if (DifferenceManager.Instance.IsDiffPlan_New())
            reward = config.planb_reward == null ? config.reward : config.planb_reward;

        List<ResData> listReward = new List<ResData>();
        for (int i = 0; i < reward.Length; i++)
        {
            listReward.Add(new ResData(reward[i], config.amount[i]));
        }

        var pos = _levelReward.transform.position + new Vector3(-0.05f, 0.1f, 0);
        CommonRewardManager.Instance.ShowNormalBoxReward(pos, listReward);
    }
    
    private void OnSoundClick()
    {
        AudioManager.Instance.PlaySound(SfxNameConst.button_s);
        SettingManager.Instance.SoundClose = !SettingManager.Instance.SoundClose;
        SetFuncEnable(_soundToggle, SettingManager.Instance.SoundClose);
        
        StorageManager.Instance.UploadProfile();
    }

    private void OnMusicClick()
    {
        AudioManager.Instance.PlaySound(SfxNameConst.button_s);
        SettingManager.Instance.MusicClose = !SettingManager.Instance.MusicClose;
        SetFuncEnable(_musiceToggle, SettingManager.Instance.MusicClose);
        
        StorageManager.Instance.UploadProfile();
    }

    private void OnShakeClick()
    {
        AudioManager.Instance.PlaySound(SfxNameConst.button_s);
        SettingManager.Instance.ShakeClose = !SettingManager.Instance.ShakeClose;
        SetFuncEnable(_shakeToggle, SettingManager.Instance.ShakeClose);
        ShakeManager.Instance.ShakeMedium();
    }

    private void OnSetClick()
    {
        AudioManager.Instance.PlaySound(SfxNameConst.button_s);
        
        StartCoroutine(CommonUtils.PlayAnimation(_animator, UIAnimationConst.DisAppear, null, () =>
        {
            CloseWindowWithinUIMgr(true);
            UIManager.Instance.OpenUI(UINameConst.UIPopupSet2);
        }));
    }
    private void SetFuncEnable(Button btn, bool isDisable)
    {
        var openIconTrans = btn.transform.Find("OpenIcon");
        var closeIconTrans = btn.transform.Find("CloseIcon");
        btn.targetGraphic = isDisable ? closeIconTrans.GetComponent<Image>() : openIconTrans.GetComponent<Image>();
        openIconTrans.gameObject.SetActive(!isDisable);
        closeIconTrans.gameObject.SetActive(isDisable);
    }

    private void UpdateHeadIcon(BaseEvent e)
    {
        if (_headIconRoot)
        {
            if (HeadIcon)
            {
                HeadIcon.SetAvatarViewState(HeadIconUtils.GetMyViewState());
            }
            else
            {
                HeadIcon = HeadIconNode.BuildHeadIconNode(_headIconRoot,HeadIconUtils.GetMyViewState());
            }
        }
    }  
    
    public override void ClickUIMask()
    {
        if (!canClickMask)
            return;

        canClickMask = false;
        OnCloseClick();
    }
}