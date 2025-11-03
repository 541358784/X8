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
using Gameplay.UI.BindEmail;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;


public class UIPopupSetOtherController : UIWindowController
{
    private Button _buttonClose;
    private Button _buttonFacebook;
    private Button _buttonApple;
    private Button _buttonLanguage;
    private Button _buttonRestore;
    private Button _buttonEmail;
    private Button _buttonDeleteArchive;

    private StorageCommon _storageCommon;
    private StorageAvatar _storageAvatar;
    
    public override void PrivateAwake()
    {
        Button privacyBtn = transform.Find("Root/BottomGroup/BtnPrivacyPolicy").GetComponent<Button>();
        privacyBtn.onClick.AddListener(OnBtnPrivacy);      
        Button serviceBtn = transform.Find("Root/BottomGroup/BtnTermsOfService").GetComponent<Button>();
        serviceBtn.onClick.AddListener(OnBtnService);

        _buttonClose = GetItem<Button>("Root/ButtonClose");
        _buttonClose.onClick.AddListener(OnCloseClick);

        _buttonEmail = GetItem<Button>("Root/MiddleButtonGroup/Mail");
        _buttonEmail.onClick.AddListener(OnEmailClick);
        
        _buttonLanguage = GetItem<Button>("Root/MiddleButtonGroup/LanguageButton");
        _buttonLanguage.onClick.AddListener(OnLanguageClick);

        _buttonApple = GetItem<Button>("Root/MiddleButtonGroup/AppleButton");
        _buttonApple.onClick.AddListener(OnAppleClick);
        
        _buttonDeleteArchive = GetItem<Button>("Root/MiddleButtonGroup/DeleteArchive");
        _buttonDeleteArchive.onClick.AddListener(DeleteArchive);

        _buttonFacebook = GetItem<Button>("Root/MiddleButtonGroup/FacebookButton");
        _buttonFacebook.onClick.AddListener(OnFacebookClick);

        _storageCommon = StorageManager.Instance.GetStorage<StorageCommon>();
        _storageAvatar = StorageManager.Instance.GetStorage<StorageHome>().AvatarData;
        
        UpdateUI();
    }

    private void OnDestroy()
    {
    }

    protected override void OnOpenWindow(params object[] objs)
    {
    }

    private void DeleteArchive()
    {
        AudioManager.Instance.PlaySound(SfxNameConst.button_s);
        StartCoroutine(CommonUtils.PlayAnimation(_animator, UIAnimationConst.DisAppear, null, () =>
        {
            CloseWindowWithinUIMgr(true);
            UIManager.Instance.OpenUI(UINameConst.UIPopupDeleteArchive1);
        }));
    }

    private void UpdateUI()
    {
#if UNITY_IOS
        _buttonApple.gameObject.SetActive(!AccountManager.Instance.HasBindApple() && Gameplay.AppleAccountSubSystem.Instance.SupportAppleLogin());
#else
        _buttonApple.gameObject.SetActive(false);
#endif
        _buttonFacebook.gameObject.SetActive(!AccountManager.Instance.HasBindFacebook());
        
        _buttonEmail.gameObject.SetActive(UIPopupBindEmailController.CanShowBindEmailButton());
    }

    private void OnCloseClick()
    {
        AudioManager.Instance.PlaySound(SfxNameConst.button_s);
        _buttonClose.interactable = false;
        StartCoroutine(CommonUtils.PlayAnimation(_animator, UIAnimationConst.DisAppear, null,
            () =>
            {
                CloseWindowWithinUIMgr(true); 
                UIManager.Instance.OpenUI(UINameConst.UIPopupSet1);
            }));
    }

    private void OnLanguageClick()
    {
        AudioManager.Instance.PlaySound(SfxNameConst.button_s);
        StartCoroutine(CommonUtils.PlayAnimation(_animator, UIAnimationConst.DisAppear, null, () =>
        {
            CloseWindowWithinUIMgr(true);
            UIManager.Instance.OpenUI(UINameConst.UIPopupLanguage);
        }));
    }

    private void OnEmailClick()
    {
        AudioManager.Instance.PlaySound(SfxNameConst.button_s);
        StartCoroutine(CommonUtils.PlayAnimation(_animator, UIAnimationConst.DisAppear, null, () =>
        {
            CloseWindowWithinUIMgr(true);
            UIManager.Instance.OpenWindow(UINameConst.UIPopupBindEmail, true);
        }));
    }
    
    private void OnRestoreClick()
    {
        StoreModel.Instance.RestorePurchase();
    }

    private void OnFacebookClick()
    {
        AudioManager.Instance.PlaySound(SfxNameConst.button_s);

        if (AccountManager.Instance.HasBindFacebook())
            return;
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventBindFacebookLogin);
        WaitingManager.Instance.OpenWindow();
        AudioManager.Instance.PlaySound(SfxNameConst.button_s);
#if UNITY_EDITOR
        OnBindFacebookResult(false);
#else
        AppIconChangerSystem.Instance.IsFBAppleBinding = true;
        DragonU3DSDK.Account.AccountManager.Instance.BindFacebook((success) => { OnBindFacebookResult(success); });
#endif
    }

    private void OnBindFacebookResult(bool ret)
    {
        WaitingManager.Instance.CloseWindow();
        if (ret)
        {
            SendLoginEvent();
            CloseWindowWithinUIMgr(true);
        }
        else
        {
            CommonUtils.OpenCommonConfirmWindow(new NoticeUIData
            {
                //TitleText = "UI_panel_small_title_notice_text",
                DescString = LocalizationManager.Instance.GetLocalizedString("&key.UI_fb_accesstoken_ru_pwchange_text")
            });
        }
        EventDispatcher.Instance.DispatchEvent(EventEnum.LOGIN_SUCCESS);
    }

    private void OnAppleClick()
    {
        AudioManager.Instance.PlaySound(SfxNameConst.button_s);
        if (AccountManager.Instance.HasBindApple())
            return;

        AudioManager.Instance.PlaySound(SfxNameConst.button_s);
        WaitingManager.Instance.OpenWindow();

#if UNITY_EDITOR
        OnBindAppleResult(true);
#else
        AppIconChangerSystem.Instance.IsFBAppleBinding = true;
        DragonU3DSDK.Account.AccountManager.Instance.BindApple(OnBindAppleResult);
#endif
    }

    private void OnBindAppleResult(bool bindSuccess)
    {
        WaitingManager.Instance.CloseWindow();
        if (bindSuccess)
        {
            SendLoginEvent();
            CloseWindowWithinUIMgr(true);
        }
        else
        {
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventBindFacebookFail,
                "UNKNOWN");
            CommonUtils.OpenCommonConfirmWindow(new NoticeUIData
            {
                //TitleText = "UI_panel_small_title_notice_text",
                DescString =
                    LocalizationManager.Instance.GetLocalizedString("&key.UI_Apple_accesstoken_ru_pwchange_text")
            });
        }
        EventDispatcher.Instance.DispatchEvent(EventEnum.LOGIN_SUCCESS);
    }

    public void SendLoginEvent()
    {
        // send login to adjust
        var dataProvider = SDK.GetInstance().adjustPlugin; //Dlugin.SDK.GetInstance().dataProvider;
        if (dataProvider != null)
        {
            var obj = new JObject();
            //obj["hello"] = "world";
            dataProvider.TrackEvent("social_login", 0, obj.ToString());
        }
    }

    
    public override void ClickUIMask()
    {
        if (!canClickMask)
            return;

        canClickMask = false;
        OnCloseClick();
    }
    
    public void OnBtnPrivacy()
    {
        AudioManager.Instance.PlaySound(SfxNameConst.button_s);

        Application.OpenURL(ConfigurationController.Instance.PrivacyPolicyURL);
        AppIconChangerSystem.Instance.IsShowingPrivacyPollcy = true;
    }    
    
    public void OnBtnService()
    {
        AudioManager.Instance.PlaySound(SfxNameConst.button_s);

        Application.OpenURL(ConfigurationController.Instance.TermsOfServiceURL);
        AppIconChangerSystem.Instance.IsShowingService = true;
    }
}