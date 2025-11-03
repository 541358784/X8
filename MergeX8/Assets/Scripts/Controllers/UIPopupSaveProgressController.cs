using System.Collections;
using DragonPlus;
using DragonPlus.Config;
using Gameplay;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class UIPopupSaveProgressController : UIWindowController
{
    private UIItemData _uiFacebookAward;
    private UIItemData _uiAppleAward;

    public override void PrivateAwake()
    {
        BindEvent("Root/AppleSignButton", null, OnClickAppleLogin);
        BindEvent("Root/FBSignButton", null, OnClickFacebookLogin);
        BindEvent("Root/BgPopupBoandBig/ButtonClose", null, o => CloseWindowWithinUIMgr(true));

        _uiFacebookAward = new UIItemData(GetItem("Root/FBSignButton/Reward"), null, null, "AmountText");
        _uiAppleAward = new UIItemData(GetItem("Root/AppleSignButton/Reward"), null, null, "AmountText");

        Text text = GetItem<Text>("Root/FBSignButton/Text");
        text.text = LocalizationManager.Instance.GetLocalizedString("UI_fb_account_button");

        text = GetItem<Text>("Root/AppleSignButton/Text");
        text.text = LocalizationManager.Instance.GetLocalizedString("UI_apple_account_button");
    }


    protected override void OnOpenWindow(params object[] objs)
    {
        var appleRewardCfg =
            GlobalConfigManager.Instance.GetGlobal_Config_Number_Value(GlobalStringConfigKey.bind_apple_reward);
        var appleRewards = ResData.ParseList(appleRewardCfg);

        var facebookRewardCfg =
            GlobalConfigManager.Instance.GetGlobal_Config_Number_Value(GlobalStringConfigKey.link_fb_reward);
        var facebookRewards = ResData.ParseList(facebookRewardCfg);

        if (appleRewards != null && appleRewards.Count > 0)
        {
            _uiAppleAward.SetData(appleRewards[0], UIItemData.ValueTextFormat.PlusNumber, false, false);
            _uiAppleAward.SetActive(true);
            // var layout = _uiAppleAward.GetItem<HorizontalLayoutGroup>("");
            // if (layout != null)
            // {
            //     layout.enabled = true;
            // }
        }
        else
        {
            _uiAppleAward.SetActive(false);
        }

        if (facebookRewards != null && facebookRewards.Count > 0)
        {
            _uiFacebookAward.SetData(facebookRewards[0], UIItemData.ValueTextFormat.PlusNumber, false, false);
            _uiFacebookAward.SetActive(true);
        }
        else
        {
            _uiFacebookAward.SetActive(false);
        }

        // todo : 临时修复背景图宽度不跟随文字长度刷新的bug
        StartCoroutine(_LateUpdateLayout());

        base.OnOpenWindow(objs);
    }

    private IEnumerator _LateUpdateLayout()
    {
        // todo : 临时修复背景图宽度不跟随文字长度刷新的bug
        yield return null;

        var layout = _uiAppleAward.GetItem<HorizontalLayoutGroup>("Root/AppleSignButton");
        if (layout != null)
        {
            layout.enabled = false;
            layout.enabled = true;
        }

        layout = _uiFacebookAward.GetItem<HorizontalLayoutGroup>("Root/FBSignButton");
        if (layout != null)
        {
            layout.enabled = false;
            layout.enabled = true;
        }
    }

    private void OnClickAppleLogin(GameObject go)
    {
        AudioManager.Instance.PlaySound(SfxNameConst.button_s);
        WaitingManager.Instance.OpenWindow();
        // GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventLoginTapApple);
        GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventFtueEnterLoginClick, "3");
#if UNITY_EDITOR
        OnBindAppleResult(true);
#else
        AppIconChangerSystem.Instance.IsFBAppleBinding = true;
        DragonU3DSDK.Account.AccountManager.Instance.BindApple(OnBindAppleResult);
#endif
    }

    private void OnClickFacebookLogin(GameObject go)
    {
        AudioManager.Instance.PlaySound(SfxNameConst.button_s);
        WaitingManager.Instance.OpenWindow();
        GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventBindFacebookLogin);
        GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventFtueEnterLoginClick, "2");
#if UNITY_EDITOR
        OnBindFacebookResult(true);
#else
        AppIconChangerSystem.Instance.IsFBAppleBinding = true;
        DragonU3DSDK.Account.AccountManager.Instance.BindFacebook(OnBindFacebookResult);
#endif
    }

    public static void OnBindFacebookResult(bool success)
    {
        WaitingManager.Instance.CloseWindow();
        if (success)
        {
            //DragonPlus.LocalizationManager.Instance.SetCurrentLocale(LanguageModel.Instance.GetLocale());
            SendLoginEvent();
            //GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventFteEnterGameLoadingStart);

            MyMain.Game.InitManager();
            SceneFsm.mInstance.EnterGame();
        }
        else
        {
            GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventBindFacebookFail,
                "UNKNOWN");
            CommonUtils.OpenCommonConfirmWindow(new NoticeUIData
            {
                DescString = LocalizationManager.Instance.GetLocalizedString("UI_bind_fb_fail_authen_error_text"),
                HasCancelButton = false,
                HasCloseButton = false,
            });
        }
    }

    public static void OnBindAppleResult(bool success)
    {
        WaitingManager.Instance.CloseWindow();
        if (success)
        {
            //DragonPlus.LocalizationManager.Instance.SetCurrentLocale(LanguageModel.Instance.GetLocale());
            SendLoginEvent();
            //GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventFteEnterGameLoadingStart);

            MyMain.Game.InitManager();
            SceneFsm.mInstance.EnterGame();
        }
        else
        {
            // GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventLoginTapAppleFailed, "UNKNOWN");

            CommonUtils.OpenCommonConfirmWindow(new NoticeUIData
            {
                DescString = LocalizationManager.Instance.GetLocalizedString("UI_bind_apple_fail_authen_error_text"),
                HasCancelButton = false,
                HasCloseButton = false,
            });
        }
    }

    public static void SendLoginEvent()
    {
        // send login to adjust
        var dataProvider = Dlugin.SDK.GetInstance().adjustPlugin; //Dlugin.SDK.GetInstance().dataProvider;
        if (dataProvider != null)
        {
            JObject obj = new JObject();
            //obj["hello"] = "world";
            dataProvider.TrackEvent("social_login", 0, obj.ToString());
        }
    }
}