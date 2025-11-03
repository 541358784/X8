using UnityEngine;
using UnityEngine.UI;
using System;
using DragonPlus;
using DragonU3DSDK.Storage;
using System.Collections;
using DragonU3DSDK.Network.API.Protocol;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class UIPopupRateUsController : UIWindowController
{
    public static string coolTimeKey = "RateUsTime";

    private Button ButtonLaterButton { get; set; }
    private Button ButtonFirstButton { get; set; }
    private Button ButtonCloseButton { get; set; }

    public Action OnClose = null;

    private Animator _animator;

    public override void PrivateAwake()
    {
        _animator = transform.GetComponent<Animator>();
        ButtonLaterButton = transform.Find("Root/LaterButton").GetComponent<Button>();
        ButtonFirstButton = transform.Find("Root/FirstButton").GetComponent<Button>();
        ButtonCloseButton = transform.Find("Root/ButtonClose").GetComponent<Button>();


        ButtonLaterButton.onClick.AddListener(OnButtonLaterButtonClick);
        ButtonFirstButton.onClick.AddListener(OnButtonFirstButtonClick);
        ButtonCloseButton.onClick.AddListener(OnButtonCloseClick);
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey,
            CommonUtils.GetTimeStamp());

        GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventRateUsPop);

        GooglePlayReviewManager.Instance.Prepare();
        AppIconChangerSystem.Instance.IsSkipingAppraise = true;
    }

    private void OnButtonCloseClick()
    {
        StorageHome storage = StorageManager.Instance.GetStorage<StorageHome>();
        storage.RateUsFinish = true;
        GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventRateUsClose);

        AudioManager.Instance.PlaySound(SfxNameConst.button_s);
        StartCoroutine(CommonUtils.PlayAnimation(_animator, UIAnimationConst.DisAppear, null,
            () => { CloseWindowWithinUIMgr(true); }));
    }

    private void OnButtonLaterButtonClick()
    {
        AudioManager.Instance.PlaySound(SfxNameConst.button_s);
        StorageHome storage = StorageManager.Instance.GetStorage<StorageHome>();
        storage.RateUsFinish = true;

        UIManager.Instance.OpenUI(UINameConst.UIContactUs, false);
        StartCoroutine(CommonUtils.PlayAnimation(_animator, UIAnimationConst.DisAppear, null,
            () => { CloseWindowWithinUIMgr(true); }));

        GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventRateUs4);
    }

    private void OnButtonFirstButtonClick()
    {
        AudioManager.Instance.PlaySound(SfxNameConst.button_s);
        StorageHome storage = StorageManager.Instance.GetStorage<StorageHome>();
        storage.RateUsFinish = true;
#if SUB_CHANNEL_AMAZON
         Global.OpenAppStore();
#elif UNITY_ANDROID && !UNITY_EDITOR
            GooglePlayReviewManager.Instance.OpenReview();
#else
        Global.OpenAppStore();
#endif

        StartCoroutine(CommonUtils.PlayAnimation(_animator, UIAnimationConst.DisAppear, null,
            () => { CloseWindowWithinUIMgr(true); }));
        GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventRateUs5);
    }

    protected override void OnCloseWindow(bool destroy = false)
    {
        OnClose?.Invoke();
    }

    public static void PopUpRateUs(Action onClose)
    {
        CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey,
            CommonUtils.GetTimeStamp());
        StorageHome cookStorage = StorageManager.Instance.GetStorage<StorageHome>();
#if UNITY_IOS && !UNITY_EDITOR
        // if (BackHomeControl.IsShowInter)
        // {
        //     onClose?.Invoke();
        //     return;
        // }
       
        if (CommonUtils.GetTimeStamp() - cookStorage.RateUsData.LastPopiOSTime > (long)365*24*3600*1000)
        {
            cookStorage.RateUsData.PopTimesiOS = 0;
        }

        if (cookStorage.RateUsData.PopTimesiOS < 3 && UnityEngine.iOS.Device.RequestStoreReview())
        {
            cookStorage.RateUsData.LastPopiOSTime = CommonUtils.GetTimeStamp();
            cookStorage.RateUsData.PopTimesiOS++;
        }
        else 
        {
            string iosStore =
                GlobalConfigManager.Instance.GetGlobal_Config_Number_Value(GlobalStringConfigKey.appstore_url);
            DragonU3DSDK.DragonNativeBridge.ShowRateUsViewController(iosStore,
            LocalizationManager.Instance.GetLocalizedString("&key.UI_rateus_title_text"),
            LocalizationManager.Instance.GetLocalizedString("&key.UI_rateus_ios_desc_text"),
            LocalizationManager.Instance.GetLocalizedString("&key.UI_rateus_ios_rate_text"),
            LocalizationManager.Instance.GetLocalizedString("&key.UI_rateus_ios_later_text"),
            LocalizationManager.Instance.GetLocalizedString("&key.UI_rateus_ios_no_text"),
            "Main");
            DragonPlus.AdSubSystem.Instance.SkipAPauseInt();
        }
        onClose?.Invoke();
#else
        var dlg = UIManager.Instance.OpenUI(UINameConst.UIPopupRateUs);
        if (dlg is UIPopupRateUsController rateUsController)
        {
            rateUsController.OnClose = onClose;
        }
#endif

        cookStorage.RateUsData.LastPopUpTime = CommonUtils.GetTimeStamp();
    }

    public static bool CanShowUI()
    {
        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.RateUs))
            return false;

        StorageHome storage = StorageManager.Instance.GetStorage<StorageHome>();

        if (storage.RateUsFinish)
            return false;

        if (!CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, coolTimeKey))
        {
            PopUpRateUs(() => { });
            // UIManager.Instance.OpenUI(UINameConst.UIPopupRateUs);
            return true;
        }

        return false;
    }
}