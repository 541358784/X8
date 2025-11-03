using System;
using System.Collections.Generic;
using Decoration;
using DragonPlus;
using DragonPlus.Config.TMatch;
using DragonPlus.Config.TMatchShop;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using TMatch;
using UnityEngine;
using UnityEngine.UI;
using AudioManager = DragonPlus.AudioManager;

public partial class UIKapiTileMainController:UIWindowController
{
    private StorageKapiTile Storage;
    private LocalizeTextMeshProUGUI TimeText;
    private Button CloseBtn;
    private Button StartBtn;
    private LocalizeTextMeshProUGUI LifeCountText;
    private LocalizeTextMeshProUGUI LifeRecoverTimeText;
    private LocalizeTextMeshProUGUI LevelText;
    // private Button HelpBtn;
    public override void PrivateAwake()
    {
        CommonUtils.NotchAdapte(transform.Find("Root") as RectTransform);
        TimeText = GetItem<LocalizeTextMeshProUGUI>("Root/TopGroup/TimeGroup/TimeText");
        InvokeRepeating("UpdateTime",0f,1f);
        CloseBtn = GetItem<Button>("Root/TopGroup/ButtonClose");
        CloseBtn.onClick.AddListener(OnClickCloseBtn);
        StartBtn = GetItem<Button>("Root/Button");
        StartBtn.onClick.AddListener(async () =>
        {
            
            if (KapiTileModel.Instance.Storage.Life <= 0)
            {
                UIPopupKapiTileGiftBagController.Open();
                return;
            }
            var bigLevel = Storage.BigLevel;
            var smallLevel = Storage.SmallLevel;
            KapiTileModel.Instance.DealStartGame();
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventKapiTileLevelEnter,data1:bigLevel.ToString(),data2:smallLevel.ToString());
            int layoutId = KapiTileModel.Instance.LevelConfig[bigLevel].SmallLevels[smallLevel];
            if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Game)
            {
                SceneFsm.mInstance.ChangeState(StatusType.Transition, StatusType.BackHome);
                await XUtility.WaitSeconds(1f);
            }
            SceneFsm.mInstance.ChangeState(StatusType.TileMatch, layoutId);
            await XUtility.WaitFrames(1);
            UIKapiTileMainController.Hide();
            DecoManager.Instance.CurrentWorld.HideByPosition();
        });
        LifeCountText = transform.Find("Root/TopGroup/Energy/Text").GetComponent<LocalizeTextMeshProUGUI>();
        EventDispatcher.Instance.AddEvent<EventKapiTileLifeChange>(OnLifeChange);
        DestroyActions.Add(() =>
        {
            EventDispatcher.Instance.RemoveEvent<EventKapiTileLifeChange>(OnLifeChange);
        });
        LifeRecoverTimeText = transform.Find("Root/TopGroup/Energy/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        LevelText = transform.Find("Root/TopGroup/TitleGroup/LevelTitleText").GetComponent<LocalizeTextMeshProUGUI>();
    }

    public void OnLifeChange(EventKapiTileLifeChange evt)
    {
        LifeCountText.SetText(Storage.Life.ToString());
        
    }

    public void OnClickCloseBtn()
    {
        AnimCloseWindow();
    }
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        Storage = objs[0] as StorageKapiTile;
        InitGiftBagEntrance();
        InitLevelGroup();
        LifeCountText.SetText(Storage.Life.ToString());
        AudioManager.Instance.PlayMusic("bgm_capybara_tm",true);
        DestroyActions.Add(() =>
        {
            AudioManager.Instance.PlayMusic(1, true);
        });
    }

    public void PerformAfterGame(bool isWin)
    {
        if (!isWin)
        {
            var curLevelConfig = KapiTileModel.Instance.GetLevelConfig(Storage.BigLevel);
            var startPosition = Storage.PlayingSmallLevel;
            PerformJumpFail(curLevelConfig,startPosition);
        }
        else
        {
            var isLevelUp = Storage.SmallLevel == 0;
            var curLevelConfig = KapiTileModel.Instance.GetLevelConfig(isLevelUp?Storage.BigLevel-1:Storage.BigLevel);
            var startPosition = Storage.PlayingSmallLevel;
            PerformJumpWin(curLevelConfig,startPosition);
        }
    }
    
    public void UpdateTime()
    {
        if (Storage == null)
            return;
        TimeText.SetText(Storage.GetLeftTimeText());
        if (Storage.Life >= KapiTileModel.Instance.GlobalConfig.MaxLife)
            LifeRecoverTimeText.SetText("Full");
        else
        {
            var leftAddLifeTime = Storage.LifeUpdateTime +
                                  KapiTileModel.Instance.GlobalConfig.LifeRecoverTime * (long)XUtility.Min -
                                  (long)APIManager.Instance.GetServerTime();
            LifeRecoverTimeText.SetText(CommonUtils.FormatLongToTimeStr(leftAddLifeTime));
        }
    }

    private List<Action> DestroyActions = new List<Action>();
    private void OnDestroy()
    {
        foreach (var action in DestroyActions)
        {
            action();
        }
    }

    public static void Hide()
    {
        Instance?.gameObject.SetActive(false);
    }

    public async static void Show(bool isWin)
    {
        if (!Instance)
            return;
        Instance.gameObject.SetActive(true);
        await XUtility.WaitSeconds(1f);
        Instance.PerformAfterGame(isWin);
    }

    public static UIKapiTileMainController Instance;
    public static UIKapiTileMainController Open(StorageKapiTile storageKapiTile)
    {
        Instance = UIManager.Instance.OpenUI(UINameConst.UIKapiTileMain, storageKapiTile) as
            UIKapiTileMainController;
        return Instance;
    }
    
}