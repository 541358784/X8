using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK;
using DragonU3DSDK.Account;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Framework;
using Gameplay;
using Gameplay.UI;
using UnityEngine;
using static Gameplay.UserData;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class FaceBookSubSystem : GlobalSystem<FaceBookSubSystem>
{
    public const string PositionPopUp = "pop";
    public const string PositionLogin = "login";
    public const string PositionSetting = "settings";

    private string _position;

    public delegate void OnLogin(bool success);

    public delegate void OnLogout(bool success);

    private OnLogin _onLogin;
    private OnLogout _onLogout;
    private bool _bindingMark;

    public bool BindingMark
    {
        get
        {
            var binding = _bindingMark;
            _bindingMark = false;
            return binding;
        }
    }


    public void Login(string position)
    {
        _position = position;
        _Login();
    }

    public void Logout()
    {
        if (!APIManager.Instance.HasNetwork)
        {
            Global.ShowHint(
                LocalizationManager.Instance.GetLocalizedString(UIPopupMsgBoxController.UI_NTWORKXNET_Content));
            _onLogout?.Invoke(false);
        }

        var popUpInfo = new UIPopupMsgBoxController.PopupInfo()
        {
            title = LocalizationManager.Instance.GetLocalizedString(UIPopupMsgBoxController.UI_POPUP_EXIT_Title),
            content = LocalizationManager.Instance.GetLocalizedString(UIPopupMsgBoxController.UI_POPUP_EXIT_Content),
            txtOk = LocalizationManager.Instance.GetLocalizedString(UIPopupMsgBoxController.UI_POPUP_EXIT_BUTTON_Exit),
            onButtonOk = delegate
            {
                Debug.Log("发送facebook解绑");
                Global.ShowUIWaiting(10f);
                AccountManager.Instance.LogoutWithFacebook(delegate(bool success)
                {
                    Global.HideUIWaiting();
                    if (success)
                    {
                    }
                    else
                    {
                        var popUpInfo2 = new UIPopupMsgBoxController.PopupInfo()
                        {
                            title = LocalizationManager.Instance.GetLocalizedString(UIPopupMsgBoxController
                                .COMMON_TITLE_FAILED),
                            content = LocalizationManager.Instance.GetLocalizedString(UIPopupMsgBoxController
                                .UI_LOGOUT_POPUP_FAILED_Content),
                            txtOk = LocalizationManager.Instance.GetLocalizedString(UIPopupMsgBoxController
                                .COMMON_BUTTON_CONTINUE),
                        };
                        UIInterface.Instance.ShowMsgBox(popUpInfo2);
                    }

                    _onLogout.Invoke(success);

                    SceneFsm.mInstance.BackToLogin();
                });
            }
        };
        UIInterface.Instance.ShowMsgBox(popUpInfo);
    }

    private void _Login()
    {
        //BIGameEvent.SetData(BiEventHome.Types.GameEventType.GameEventBindFacebookBind,BiEventHome.Types.GamePosition.FacebookPopup,"success","","","",0,_auto);
        if (!APIManager.Instance.HasNetwork)
        {
//            Global.ShowHint(LocaleConfigManager.Instance.GetLocalizedString(UIMsgBoxController.UI_NTWORKXNET_Content));
            _onLogin?.Invoke(false);
        }

        GamePauseManager.Instance.RegisterPauseReason(GamePauseManager.PauseReasonMask.BindFacebook);
        Global.ShowUIWaiting(10f);
        _bindingMark = true;

        //		var cfg = DataVisiter.Instance.GetTableMixConfig();
        //		var rewards = StringUtils.StringToAwards(cfg.LinkFBReward);
        //		var reason = new UserData.ChangeReason() { reason = BiEventHomeMemory.Types.ItemChangeReason.BindFacebook, };
        //		var flyRewards = new List<FlyAnimationData>();
        //		foreach (var item in rewards)
        //		{
        //			flyRewards.Add(new FlyAnimationData(item.id, item.count));
        //		}
        //		var rewardData = new UIPublicRewardController.Data() { viewType = RewardViewType.Better, rewards = flyRewards, hideCurrencyFinish = !UICurrencyGroupController.IsShow() };
        //
        //		//增加资源
        //		for (int index = 0; index < rewards.Count; index++)
        //		{
        //			Gameplay.UserData.Instance.AddRes(rewards[index].id, rewards[index].count, reason);
        //		}
        //
        //		UIPublicRewardController.Show(rewardData, () =>
        //		{
        //		});
        AccountManager.Instance.BindFacebook((System.Action<bool>) ((success) =>
        {
            Debug.Log($"bind facebook result : {success}");
            Global.HideUIWaiting();
            if (success)
            {
            }
            else
            {
                var popUpInfo = new UIPopupMsgBoxController.PopupInfo()
                {
                    title =
                        LocalizationManager.Instance.GetLocalizedString(UIPopupMsgBoxController.COMMON_TITLE_FAILED),
                    content = LocalizationManager.Instance.GetLocalizedString(UIPopupMsgBoxController
                        .UI_POPUP_FB_FAILED_Content),
                    txtOk =
                        LocalizationManager.Instance.GetLocalizedString(UIPopupMsgBoxController.COMMON_BUTTON_RETRY),
                    onButtonOk = _RetryLoginFb,
                };
                UIInterface.Instance.ShowMsgBox(popUpInfo);
            }

            if (_onLogin != null)
            {
                _onLogin(success);
            }
        }));
    }

    public void RegisterLogin(OnLogin callback)
    {
        if (_onLogin == null)
        {
            _onLogin = callback;
        }
        else
        {
            _onLogin += callback;
        }
    }

    public void UnRegisterLogin(OnLogin callback)
    {
        if (_onLogin != null)
        {
            _onLogin -= callback;
        }
    }

    public void RegisterLogout(OnLogout callback)
    {
        if (_onLogout == null)
        {
            _onLogout = callback;
        }
        else
        {
            _onLogout += callback;
        }
    }

    public void UnRegisterLogout(OnLogout callback)
    {
        if (_onLogout != null)
        {
            _onLogout -= callback;
        }
    }


    private void _RetryLoginFb()
    {
        _Login();
    }
}