/*
 * @file LoginController
 * 登录界面
 * @author lu
 */

using DragonPlus;
using DragonU3DSDK;
using UnityEngine;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;
using System;
using System.Linq;
using Difference;
using Dlugin;
using DragonU3DSDK.Account;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using Framework;
using Game.Config;
using Gameplay;
using Merge.Order;
using Screw.GameLogic;

public class UILoginController : UIWindowController
{
    private GameObject facebookLoginGameObject;
    DateTime loginDataTime = DateTime.MinValue;
    private GameObject otherLoginGameObject;


    // 唤醒界面时调用(创建的时候加载一次)
    public override void PrivateAwake()
    {
        isPlayDefaultAudio = false;

//        LocalizationManager.Instance.SetCurrentLocale(LanguageModel.Instance.GetLocale());
        BindEvent("PlayButton", null, OnClickPlayButton);
        facebookLoginGameObject = BindEvent("FacebookButton", null, OnClickFacebookButton);
        otherLoginGameObject = BindEvent("SaveProgressButton", null, OnClickOtherLoginButton);
        DragonU3DSDK.Device.Instance.AddBackButtonCallback(BackHomeControl.BackButtonQuickApp);
        
        if (transform.Find("Loading_2/1") && transform.Find("Loading_2/2"))
        {
            var oldBG = transform.Find("Loading_2/1");
            var newBG = transform.Find("Loading_2/2");
            var oldLogo = transform.Find("logoGroup/1");
            var newLogo = transform.Find("logoGroup/2");
            var curTime = APIManager.Instance.GetServerTime();
            ulong changeTime = 1761235200000;
            oldBG.gameObject.SetActive(curTime < changeTime);
            newBG.gameObject.SetActive(curTime >= changeTime);
            oldLogo.gameObject.SetActive(curTime < changeTime);
            newLogo.gameObject.SetActive(curTime >= changeTime);
        }
    }

    void OnClickPlayButton(GameObject go)
    {
        GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventFtueEnterLoginClick, "1");
        RestGameData();
        if (loginDataTime != DateTime.MinValue && (DateTime.Now - loginDataTime).Seconds <= 2)
        {
            return;
        }

        loginDataTime = DateTime.Now;
        WaitingManager.Instance.OpenWindow(10.0f, 0.5f);
        AudioManager.Instance.PlaySound(SfxNameConst.button_s);
        AccountManager.Instance.Login(OnLoginResult);
    }

    void OnClickFacebookButton(GameObject go)
    {
        if (loginDataTime != DateTime.MinValue && (DateTime.Now - loginDataTime).Seconds <= 2)
        {
            return;
        }

        loginDataTime = DateTime.Now;

        RestGameData();
        AudioManager.Instance.PlaySound(SfxNameConst.button_s);
        WaitingManager.Instance.OpenWindow();
        GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventBindFacebookLogin);
        GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventFtueEnterLoginClick, "2");
#if UNITY_EDITOR
        UIPopupSaveProgressController.OnBindFacebookResult(true);
#else
        DragonU3DSDK.Account.AccountManager.Instance.BindFacebook((success) => { UIPopupSaveProgressController.OnBindFacebookResult(success); });
#endif
    }

    private void OnClickOtherLoginButton(GameObject go)
    {
        RestGameData();
        UIManager.Instance.OpenUI(UINameConst.UISaveProgress);
    }

    // 打开界面时调用(每次打开都调用)
    protected override void OnOpenWindow(params object[] objs)
    {
        GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventFtueEnterLoginScreen);
        RunOnce();
        //Gameplay.PreloadSubSystem.Instance.PreloadAsset();
        UpdateButtons();
        //关闭资源栏
        UIManager.Instance.CloseUI(UINameConst.UIMainGroup, true);

        IDFAManager.Instance.RequestIDFA();
    }

    public static void RunOnce()
    {
        IDFAManager.Instance.RequestIDFA();
        
        var isNewPlayer = DragonU3DSDK.Storage.StorageManager.Instance.RunOnce(() =>
        {
            StorageManager.Instance.GetStorage<StorageHome>().LocalFirstRunTimeStamp =
                CommonUtils.ConvertDateTimeToTimeStamp(DateTime.UtcNow);

            var coin = GlobalConfigManager.Instance.GetNumValue(GlobalNumberConfigKey.new_user_coin);
            var deco = GlobalConfigManager.Instance.GetNumValue(GlobalNumberConfigKey.new_user_key);
            var diamond = GlobalConfigManager.Instance.GetNumValue(GlobalNumberConfigKey.new_user_diamond);
            var energy = GlobalConfigManager.Instance.GetNumValue("InitEnergy") == 0
                ? 50
                : GlobalConfigManager.Instance.GetNumValue("InitEnergy");
            UserData.Instance.AddRes((int) UserData.ResourceId.Coin, coin,
                new GameBIManager.ItemChangeReasonArgs
                {
                    reason = BiEventCooking.Types.ItemChangeReason.CreateProfile,
                    data1 = UserData.ResourceId.Coin.ToString()
                });
            UserData.Instance.AddRes((int) UserData.ResourceId.RareDecoCoin, deco,
                new GameBIManager.ItemChangeReasonArgs
                {
                    reason = BiEventCooking.Types.ItemChangeReason.CreateProfile,
                    data1 = UserData.ResourceId.RareDecoCoin.ToString()
                });
            UserData.Instance.AddRes((int) UserData.ResourceId.Diamond, diamond,
                new GameBIManager.ItemChangeReasonArgs
                {
                    reason = BiEventCooking.Types.ItemChangeReason.CreateProfile,
                    data1 = UserData.ResourceId.Diamond.ToString()
                });
            // 读配置表InitEnergy
            EnergyModel.Instance.AddEnergy(energy,
                new GameBIManager.ItemChangeReasonArgs
                {
                    reason = BiEventCooking.Types.ItemChangeReason.CreateProfile,
                    data1 = UserData.ResourceId.Energy.ToString()
                });
            StorageManager.Instance.SaveToLocal();
        });
        
        ScrewGameLogic.Instance.OnRunOnce();
    }

    private void UpdateButtons()
    {
        var hasBindFacebook = DragonU3DSDK.Account.AccountManager.Instance.HasBindFacebook();
        var hasBindApple = AccountManager.Instance.HasBindApple();
        var supportBindApple = AppleAccountSubSystem.Instance.SupportAppleLogin();

        facebookLoginGameObject.SetActive(!hasBindFacebook && !supportBindApple);
        otherLoginGameObject.SetActive(!hasBindFacebook && !hasBindApple && supportBindApple);
    }

    private void OnLoginResult(bool loginResult)
    {
        //DragonPlus.LocalizationManager.Instance.SetCurrentLocale(LanguageModel.Instance.GetLocale());
        if (loginResult)
        {
            UIPopupSaveProgressController.SendLoginEvent();
            //GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventFteEnterGameLoadingStart);
            //GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventFteConnectSeverSuccess);
        }
        else
        {
            //GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventConnectSeverFail);
        }


        //ClientMgr.Instance.BeforeEnterWorld(new EnterWorldParm());

        CoroutineManager.Instance.StartCoroutine(CommonUtils.DelayWork(1f, () =>
        {
            MyMain.Game.InitManager();
            SceneFsm.mInstance.EnterGame();
            WaitingManager.Instance.CloseWindow();
            MergeManager.Instance.SendMergeBoardBI(MergeBoardEnum.Main);
        }));
    }

    private void RestGameData()
    {
        DifferenceManager.Instance.Reset();
        
        // ClientMgr.Instance.InitWroldModels();
        // RedPointController.InitRedPointController();
    }
}