using System;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupCrocodileStartController : TMatch.UIWindowController
{
    #region open ui

    /// <summary>
    /// 预制体路径
    /// </summary>
    private const string PREFAB_PATH = "Prefabs/Activity/TMatch/Crocodile/UIPopupCrocodileStart";

    /// <summary>
    /// 打开
    /// </summary>
    public static void Open()
    {
       TMatch.UIManager.Instance.OpenWindow<UIPopupCrocodileStartController>(PREFAB_PATH);
    }

    #endregion
    public  Action EmptyCloseAction => OnClickedCloseButton;

    private Transform _beginGroup;
    private Transform _failedGroup;

    public LocalizeTextMeshProUGUI BeginTimerText;
    public LocalizeTextMeshProUGUI FailedTimerText;
    private LocalizeTextMeshProUGUI _beginPrizeText;
    private LocalizeTextMeshProUGUI _failedPrizeText;

    public bool CanStart;
    private bool _haveStarted;
    public bool CanStartInCurrentTurn;
    public override void PrivateAwake()
    {
        transform.Find("Root/CloseButton").GetComponent<Button>().onClick.AddListener(OnClickedCloseButton);
        transform.Find("Root/BeginGroup/ContinueButton").GetComponent<Button>().onClick.AddListener(OnClickedContinueButton);
        
        _beginGroup = transform.Find("Root/BeginGroup");
        _failedGroup = transform.Find("Root/FailedGroup");

        BeginTimerText = GetItem<LocalizeTextMeshProUGUI>("Root/BeginGroup/TimeGroup/TimeText");
        FailedTimerText = GetItem<LocalizeTextMeshProUGUI>("Root/FailedGroup/TimeGroup/TimeText");
        _beginPrizeText = GetItem<LocalizeTextMeshProUGUI>("Root/BeginGroup/PrizeGroup/NumberText");
        _failedPrizeText = GetItem<LocalizeTextMeshProUGUI>("Root/FailedGroup/PrizeGroup/NumberText");
        
        InvokeRepeating("UpdateRepeating", 0, 1);
    }

    protected override void OnOpenWindow(TMatch.UIWindowData data)
    {
        base.OnOpenWindow(data);
        TMatch.EventDispatcher.Instance.AddEventListener(EventEnum.RefreshWinStreak, InitAndRefreshUI);
        InitAndRefreshUI();
    }



    private void InitAndRefreshUI()
    {
        InitUI();
        RefreshUI();
    }

    private void InitAndRefreshUI(TMatch.BaseEvent evt)
    {
        InitAndRefreshUI();
    }

    protected override void OnCloseWindow(bool destroy = false)
    {
        base.OnCloseWindow(destroy);
        TMatch.EventDispatcher.Instance.RemoveEventListener(EventEnum.RefreshWinStreak, InitAndRefreshUI);

    }

    // public override void OnViewDestroy()
    // {
    //     base.OnViewDestroy();
    //     if (!_haveStarted)
    //     {
    //         LobbyTaskSystem.Instance.FinishCurrentTask();
    //     }
    // }

    private void InitUI()
    {
        var baseConfig = CrocodileActivityModel.Instance.GetBaseConfig();
        _beginPrizeText.SetText(baseConfig.RewardCnt[0].ToString());
        _failedPrizeText.SetText(baseConfig.RewardCnt[0].ToString());
        BeginTimerText.SetText("");
        FailedTimerText.SetText("");

    }

    public void RefreshUI()
    {
        CrocodileActivityModel.Instance.GetCurrentActivityLeftTime();
        CanStart = CrocodileActivityModel.Instance.CanStartChallenge();
        _beginGroup.gameObject.SetActive(CanStart);
        _failedGroup.gameObject.SetActive(!CanStart);
        if (CanStart) return;

        CanStartInCurrentTurn = CrocodileActivityModel.Instance.CanStartInCurrentTurn();
    }

    private void UpdateRepeating()
    {
        if (CanStart)
        {
            var leftTime = CrocodileActivityModel.Instance.GetCurrentActivityLeftTime(); // 获取当前轮次的剩余时间
            BeginTimerText.SetText(CommonUtils.FormatLongToTimeStr(leftTime));
        }
        else
        {
            if (CanStartInCurrentTurn)
            {
                var leftTime = CrocodileActivityModel.Instance.GetCanChallengeLeftTime();
                if (leftTime >= 0)
                {
                    FailedTimerText.SetText(CommonUtils.FormatLongToTimeStr(leftTime));
                }
                else
                {
                    RefreshUI();
                }
            }
            else
            {
               FailedTimerText.SetTerm("UI_lava_finished");
            }
        }
    }
    private void OnClickedCloseButton()
    {
        CloseWindowWithinUIMgr();
    }

    private void OnClickedContinueButton()
    {
        CrocodileActivityModel.Instance.StartChallenge();
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventTmLavaquestEnter,CrocodileActivityModel.Instance.Storage.ChallengeTimes.ToString());
       
        CloseWindowWithinUIMgr();
        UICrocodileMatchController.Open();
    }

   
}