/*
 * @file ChooseProgressController
 * 登录界面
 * @author lu
 */

using System.Collections.Generic;
using DragonPlus;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using DragonPlus.Config;
using System;
using System.Linq;
using DragonU3DSDK;
using Gameplay;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class ChooseProgressData
{
    private int _level = 0;

    public int Level
    {
        get { return _level; }
        set { _level = value; }
    }

    public double Time { get; set; }
    public int CoinNum { get; set; }
    public int DiamondNum { get; set; }

    public string json;

    public ChooseProgressData(string json)
    {
        try
        {
            this.json = json;
            JObject obj = JObject.Parse(json);
            Time = double.Parse(obj["StorageCommon"]["updatedAt"].ToString());

            //TODO 这里会报错
            Level = 0;
            // if (obj["StorageM3Game"] != null && obj["StorageM3Game"]["playedLevelCount"] != null)
            //     int.TryParse(obj["StorageM3Game"]["playedLevelCount"].ToString(), out _level);
                
            if (obj["StorageHome"]["level"] != null)
            {
                Level = int.Parse(obj["StorageHome"]["level"].ToString());
            }

            CoinNum = 0;

            string coinKey = UserData.Instance.GetCurrencyKey(UserData.ResourceId.Coin);
            if (obj["StorageHome"]["currency"][coinKey] != null)
            {
                float vc0 = float.Parse(obj["StorageHome"]["currency"][coinKey]["_vc0"].ToString());
                int vc1 = int.Parse(obj["StorageHome"]["currency"][coinKey]["_vc1"].ToString());
                CoinNum = (int) Math.Round(8.0f * vc0 + vc1);
            }
            
            DiamondNum = 0;

            string diamondKey = UserData.Instance.GetCurrencyKey(UserData.ResourceId.Diamond);
            if (obj["StorageHome"]["currency"][diamondKey] != null)
            {
                float vc0 = float.Parse(obj["StorageHome"]["currency"][diamondKey]["_vc0"].ToString());
                int vc1 = int.Parse(obj["StorageHome"]["currency"][diamondKey]["_vc1"].ToString());
                DiamondNum = (int) Math.Round(8.0f * vc0 + vc1);
            }
        }
        catch (Exception e)
        {
            DebugUtil.LogError(e);
        }
    }
}

public class UIPopupChooseProgressController : UIWindow
{
    public static bool IsOpenWindow = false;

    ChooseProgressData ServerData { get; set; }
    ChooseProgressData LocalData { get; set; }
    DragonU3DSDK.Network.API.Protocol.Profile serverProfile;

    Button LocalButton { get; set; }
    LocalizeTextMeshProUGUI LocalDiamondText { get; set; }
    LocalizeTextMeshProUGUI LocalLevelText { get; set; }
    LocalizeTextMeshProUGUI LocalLevelText2 { get; set; }
    LocalizeTextMeshProUGUI LocalTimeText { get; set; }

    Button ServerButton { get; set; }
    LocalizeTextMeshProUGUI ServerDiamondText { get; set; }
    LocalizeTextMeshProUGUI ServerLevelText { get; set; }
    LocalizeTextMeshProUGUI ServerLevelText2 { get; set; }
    LocalizeTextMeshProUGUI ServerTimeText { get; set; }

    // 唤醒 界面时调用(创建的时 候加载一次) 
    public override void PrivateAwake()
    {
        isPlayDefaultAudio = false;

        LocalButton = transform.Find("ChooseProgress/Progress2/Button").GetComponent<Button>();
        LocalButton.onClick.AddListener(OnClickLocalButton);
        LocalDiamondText = transform.Find("ChooseProgress/Progress2/Diamonds/Textcontent/Text")
            .GetComponent<LocalizeTextMeshProUGUI>();
        LocalLevelText = transform.Find("ChooseProgress/Progress2/Level").GetComponent<LocalizeTextMeshProUGUI>();
        LocalLevelText2 = transform.Find("ChooseProgress/Progress2/Lv/Textcontent/Text").GetComponent<LocalizeTextMeshProUGUI>();
        LocalTimeText = transform.Find("ChooseProgress/Progress2/Time").GetComponent<LocalizeTextMeshProUGUI>();

        ServerButton = transform.Find("ChooseProgress/Progress1/Button").GetComponent<Button>();
        ServerButton.onClick.AddListener(OnClickServerButton);
        ServerDiamondText = transform.Find("ChooseProgress/Progress1/Diamonds/Textcontent/Text")
            .GetComponent<LocalizeTextMeshProUGUI>();
        ServerLevelText = transform.Find("ChooseProgress/Progress1/Level").GetComponent<LocalizeTextMeshProUGUI>();
        ServerLevelText2 = transform.Find("ChooseProgress/Progress1/Lv/Textcontent/Text").GetComponent<LocalizeTextMeshProUGUI>();
        ServerTimeText = transform.Find("ChooseProgress/Progress1/Time").GetComponent<LocalizeTextMeshProUGUI>();

        IsOpenWindow = true;
    }

    private void Start()
    {
        //CKBI.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventChooseProfilePop);
    }

    void OnClickLocalButton()
    {
        AudioManager.Instance.PlaySound(SfxNameConst.button_s);
        //GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventChooseProfileSelect, "client");
        //DragonU3DSDK.Storage.StorageManager.Instance.ResolveProfileConfict(serverProfile, false);
        UIManager.Instance.OpenUI(UINameConst.UIPopupConfirmProgress)
            .GetComponent<UIPopupConfirmProgressController>().SetData(serverProfile, false);
        CloseWindowWithinUIMgr(true);
    }

    void OnClickServerButton()
    {
        AudioManager.Instance.PlaySound(SfxNameConst.button_s);
        //GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventChooseProfileSelect, "server");
        //DragonU3DSDK.Storage.StorageManager.Instance.ResolveProfileConfict(serverProfile, true);
        UIManager.Instance.OpenUI(UINameConst.UIPopupConfirmProgress)
            .GetComponent<UIPopupConfirmProgressController>().SetData(serverProfile, true);
        CloseWindowWithinUIMgr(true);
    }

    public void SetData(DragonU3DSDK.Network.API.Protocol.Profile profile)
    {
        try
        {
            serverProfile = profile;
            var localJson = DragonU3DSDK.Storage.StorageManager.Instance.ToJson();
            var serverJson = profile.Json;
            LocalData = new ChooseProgressData(localJson);
            ServerData = new ChooseProgressData(serverJson);
            ReloadUi();
        }
        catch (Exception e)
        {
            DebugUtil.LogError(e);
        }
    }

    void ReloadUi()
    {
        LocalDiamondText.SetText(LocalData.DiamondNum.ToString());
        string levelStr = LocalizationManager.Instance.GetLocalizedString("UI_common_text_LevelLabel");
        LocalLevelText.SetText(levelStr + (LocalData.Level) + "  " +
                               LocalizationManager.Instance.GetLocalizedString("&key.UI_choose_progress_on_device"));
        LocalLevelText2.SetText(LocalData.Level.ToString());
        LocalTimeText.SetText(DragonU3DSDK.Utils.GetTimeStampDateString(LocalData.Time));

        ServerDiamondText.SetText(ServerData.DiamondNum.ToString());
        ServerLevelText.SetText(levelStr + (ServerData.Level) + "  " +
                                LocalizationManager.Instance.GetLocalizedString("&key.UI_choose_progress_on_server"));
        ServerLevelText2.SetText(ServerData.Level.ToString());
        ServerTimeText.SetText(DragonU3DSDK.Utils.GetTimeStampDateString(ServerData.Time));
    }


    // 打开界面 时调用(每次打开都调用) 
    protected override void OnOpenWindow(params object[] objs)
    {
    }

    // 重新加载界面时调用 
    protected override void OnReloadWindow()
    {
    }

    // 关闭界面时调用(onDestroy) 
    protected override void OnCloseWindow(bool destroy = false)
    {
        IsOpenWindow = false;
    }
}