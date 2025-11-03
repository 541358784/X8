using System;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.TMatchShop;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using TMatch;
using UnityEngine;
using UnityEngine.UI;
using AudioManager = DragonPlus.AudioManager;

public partial class UIKapiScrewMainController:UIWindowController
{
    private StorageKapiScrew Storage;
    private LocalizeTextMeshProUGUI TimeText;
    private Button CloseBtn;
    private Button StartBtn;
    private Button MatchBtn;
    private LocalizeTextMeshProUGUI LifeCountText;
    private LocalizeTextMeshProUGUI LifeRecoverTimeText;
    private LocalizeTextMeshProUGUI MaxSmallLevelText;
    private LocalizeTextMeshProUGUI BigLevelText;
    // private Button HelpBtn;
    public override void PrivateAwake()
    {
        CommonUtils.NotchAdapte(transform.Find("Root") as RectTransform);
        TimeText = GetItem<LocalizeTextMeshProUGUI>("Root/TopGroup/TimeGroup/TimeText");
        InvokeRepeating("UpdateTime",0f,1f);
        CloseBtn = GetItem<Button>("Root/TopGroup/ButtonClose");
        CloseBtn.onClick.AddListener(OnClickCloseBtn);
        StartBtn = GetItem<Button>("Root/StartButton");
        StartBtn.onClick.AddListener(OnClickStartBtn);
        MatchBtn = GetItem<Button>("Root/MatchButton");
        MatchBtn.onClick.AddListener(() =>
        {
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.KapiScrewMatchBtn);
            if (!Storage.ChangeEnemy)
                return;
            var curAnimName = Spine.AnimationState.GetCurrent(0)?.Animation.Name; // Assuming you are using Track 0
            if (curAnimName != "idle")
                Spine.PlaySkeletonAnimation("idle", true);
            PerformChangeEnemy().WrapErrors();
        });
        LifeCountText = transform.Find("Root/TopGroup/Energy/Text").GetComponent<LocalizeTextMeshProUGUI>();
        LifeCountText.gameObject.SetActive(true);
        EventDispatcher.Instance.AddEvent<EventKapiScrewLifeChange>(OnLifeChange);
        DestroyActions.Add(() =>
        {
            EventDispatcher.Instance.RemoveEvent<EventKapiScrewLifeChange>(OnLifeChange);
        });
        LifeRecoverTimeText = transform.Find("Root/TopGroup/Energy/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        MaxSmallLevelText = transform.Find("Root/MiddleGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        BigLevelText = transform.Find("Root/MiddleGroup/ContestItem/FractionGroup/Text2").GetComponent<LocalizeTextMeshProUGUI>();
    }

    public void OnLifeChange(EventKapiScrewLifeChange evt)
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
        Storage = objs[0] as StorageKapiScrew;
        InitGiftBagEntrance();
        InitSpine();
        InitHeadIcon();
        InitScoreBoard();
        InitRewardGroup();
        LifeCountText.SetText(Storage.Life.ToString());
        AudioManager.Instance.PlayMusic("bgm_capybara_tm",true);
        DestroyActions.Add(() =>
        {
            AudioManager.Instance.PlayMusic(1, true);
        });
        MatchBtn.gameObject.SetActive(Storage.ChangeEnemy);
        StartBtn.gameObject.SetActive(!Storage.ChangeEnemy);
        // var levelConfig = KapiScrewModel.Instance.GetLevelConfig(Storage.BigLevel);
        // MaxSmallLevelText.SetTermFormats(levelConfig.SmallLevels.Count.ToString());
        // BigLevelText.SetText(LocalizationManager.Instance.GetLocalizedString(BigLevelText.GetTerm()) +  (levelConfig.Id+1));
        
        ShieldButtonOnClick[] shieldButtons = gameObject.GetComponentsInChildren<ShieldButtonOnClick>(true);
        foreach (var shieldBtn in shieldButtons)
        {
            shieldBtn.isUse = false;
        }
        CheckGuideMatch();
    }

    public void CheckGuideMatch()
    {
        if (GuideSubSystem.Instance.isFinished(GuideTriggerPosition.KapiScrewMatchBtn))
            return;
        if (!MatchBtn.gameObject.activeInHierarchy)
            return;
        List<Transform> topLayer = new List<Transform>();
        topLayer.Add(MatchBtn.transform);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.KapiScrewMatchBtn, MatchBtn.transform as RectTransform,
            topLayer: topLayer);
        if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.KapiScrewMatchBtn, null))
        {
        }
    }

    public void PerformAfterGame(bool isWin)
    {
        if (!isWin)
        {
            var curLevelConfig = KapiScrewModel.Instance.GetLevelConfig(Storage.BigLevel);
            var startPosition = Storage.PlayingSmallLevel;
        }
        else
        {
            var isLevelUp = Storage.SmallLevel == 0;
            var curLevelConfig = KapiScrewModel.Instance.GetLevelConfig(isLevelUp?Storage.BigLevel-1:Storage.BigLevel);
            var startPosition = Storage.PlayingSmallLevel;
        }
    }
    
    public void UpdateTime()
    {
        if (Storage == null)
            return;
        TimeText.SetText(Storage.GetLeftTimeText());
        if (Storage.Life >= KapiScrewModel.Instance.GlobalConfig.MaxLife)
            LifeRecoverTimeText.SetText("Full");
        else
        {
            var leftAddLifeTime = Storage.LifeUpdateTime +
                                  KapiScrewModel.Instance.GlobalConfig.LifeRecoverTime * (long)XUtility.Min -
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

    public static UIKapiScrewMainController Instance;
    public static UIKapiScrewMainController Open(StorageKapiScrew storageKapiScrew)
    {
        if (!Instance)
        {
            Instance = UIManager.Instance.OpenUI(UINameConst.UIKapiScrewMain, storageKapiScrew) as
                UIKapiScrewMainController;   
        }
        return Instance;
    }
    
}