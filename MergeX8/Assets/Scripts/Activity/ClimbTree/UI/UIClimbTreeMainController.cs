using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.ClimbTree;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;

public partial class UIClimbTreeMainController:UIWindowController
{
    // public enum ClimbTreeMainShowType
    // {
    //     Open,
    //     // Failed,
    // }
    private Button _buttonClose;
    private Button _buttonStart;
    private Slider _progressSlider;
    private Slider _progressSliderDescribe;
    private LocalizeTextMeshProUGUI _progressSliderDescribeText;
    private Transform _timeGroup;
    private LocalizeTextMeshProUGUI _timeGroupText;
    private Dictionary<int,ClimbTreeRewardItem> _rewardItems=null;
    const int UnitHeight = 212;
    const int MaxHeight = 0;
    private int MinHeight = -1700;
    private const float GrowTime = 2f;
    private float GrowSpeed => ClimbTreeModel.Instance.GetLevelStageScore(ClimbTreeModel.Instance.CurLevel)/GrowTime;
    
    private RectTransform _content;
    private ScrollRect _scrollRect;
    
    private SkeletonGraphic _skeletonGraphic;

    public void PerformAddScore(int subNum, float time, System.Action callBack = null)
    {
        int lastUpdateSubNum = 0;
        DOTween.To(() => 0f, curSubNumF =>
        {
            var curSubNum = (int) Mathf.Floor(curSubNumF);
            var addValue = curSubNum - lastUpdateSubNum;
            lastUpdateSubNum = curSubNum;
            if (addValue != 0)
            {
                ClimbTreeModel.Instance.CurScore += addValue;
            }
            OnChangeScore(curSubNumF - curSubNum);
        }, (float)subNum, time).OnComplete(() =>
        {
            var addValue = subNum - lastUpdateSubNum;
            lastUpdateSubNum = addValue;
            if (addValue != 0)
            {
                ClimbTreeModel.Instance.CurScore += addValue;   
            }
            OnChangeScore();
            callBack?.Invoke();
        }).SetEase(Ease.Linear);
    }
    public void OnChangeScore(float extraCurScore = 0f)
    {
        int levelMaxScore = ClimbTreeModel.Instance.GetLevelStageScore(ClimbTreeModel.Instance.CurLevel);
        int curScore = ClimbTreeModel.Instance.GetUpperScore(ClimbTreeModel.Instance.CurScore, ClimbTreeModel.Instance.CurLevel);
        _progressSlider.value = ClimbTreeModel.Instance.CurLevel + (curScore + extraCurScore) / levelMaxScore;
        _progressSliderDescribe.value = _progressSlider.value;
        _progressSliderDescribeText.SetText(curScore + "/" + levelMaxScore);
        _progressSliderDescribe.gameObject.SetActive(!ClimbTreeModel.Instance.IsMaxLevel());
    }
    public async Task PerformGrowProgress(int targetScore)
    {
        var upScore = targetScore - ClimbTreeModel.Instance.CurScore;
        if (upScore <= 0)
            return;
        var unitUpdateTime = 1f / GrowSpeed;
        var usingTime = unitUpdateTime * upScore;
        var growProgressTask = new TaskCompletionSource<bool>();
        PerformAddScore(upScore, usingTime, () => growProgressTask.SetResult(true));
        await growProgressTask.Task;
    }
    public async Task PerformLevelUp()
    {
        var levelUpScore = ClimbTreeModel.Instance.GetLevelBaseScore(ClimbTreeModel.Instance.CurLevel + 1);
        await PerformGrowProgress(levelUpScore);
        await OnLevelUp();
    }
    public async Task OnLevelUp()
    {
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventMonkeyReward,ClimbTreeModel.Instance.CurLevel.ToString());
        MergeTaskTipsController.Instance._mergeClimbTree.PerformLevelUp();
        var rewards = ClimbTreeModel.Instance.GetLevelRewards(ClimbTreeModel.Instance.CurLevel);
        for (int i = 0; i < rewards.Count; i++)
        {
            var reward = rewards[i];
            
            if (!UserData.Instance.IsResource(reward.id))
            {
                GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                {
                    MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonMonkeyReward,
                    itemAId = reward.id,
                    isChange = true,
                });
            }
            UserData.Instance.AddRes(reward.id, reward.count,
                new GameBIManager.ItemChangeReasonArgs() {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.MonkeyReward}, true);
        }
        var eventTask = new TaskCompletionSource<bool>();
        var endTask = new TaskCompletionSource<bool>();
        void HandleAnimationEvent(Spine.TrackEntry trackentry, Spine.Event e)
        {
            if (e.Data.Name == "start")
            {
                _skeletonGraphic.AnimationState.Event -= HandleAnimationEvent;
                eventTask.SetResult(true);
            }
        }
        _skeletonGraphic.AnimationState.Event += HandleAnimationEvent;
        _skeletonGraphic.PlaySkeletonAnimationAsync("up").AddCallBack(() =>
        {
            _skeletonGraphic.PlaySkeletonAnimation("idle",true);
            eventTask.SetResult(true);
            endTask.SetResult(true);
        }).WrapErrors();
        await eventTask.Task;
        UpdateMonkeyPosition();
        var targetContentPosition = GetContentPosition(ClimbTreeModel.Instance.CurLevel);
        DOTween.To(() => _content.anchoredPosition, curPos => _content.anchoredPosition = curPos, targetContentPosition,
            0.5f).SetTarget(_content);
        await XUtility.WaitSeconds(0.5f);
        await _rewardItems[ClimbTreeModel.Instance.CurLevel].PerformCollectReward();
        await endTask.Task;
    }

    public Vector3 GetMonkeyPosition(int level)
    {
        return transform.Find("Root/Scroll View/Viewport/Content/RewardGroup/" + level + "/Point").position;
    }

    public async void UpdateMonkeyPosition()
    {
        // await XUtility.WaitFrames(1);
        _skeletonGraphic.transform.position = GetMonkeyPosition(ClimbTreeModel.Instance.CurLevel);
    }

    private bool _firstPopUp;
    public override void PrivateAwake()
    {
        _rewardItems = new Dictionary<int, ClimbTreeRewardItem>();
        _buttonClose = GetItem<Button>("Root/ButtonClose");
        _buttonClose.onClick.AddListener(OnCloseBtn);
        _buttonStart = GetItem<Button>("Root/Button");
        _buttonStart.onClick.AddListener(OnStartBtn);

        _progressSlider = GetItem<Slider>("Root/Scroll View/Viewport/Content/Slider");
        _progressSliderDescribe = GetItem<Slider>("Root/Scroll View/Viewport/Content/SliderNum");
        _progressSliderDescribeText = GetItem<LocalizeTextMeshProUGUI>("Root/Scroll View/Viewport/Content/SliderNum/Fill Area/Fill/Num/Text");
        
        _timeGroup = GetItem<Transform>("Root/TimeGroup");
        _timeGroupText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        
        for (int i = ClimbTreeModel.Instance.MinLevel+1; i <= ClimbTreeModel.Instance.MaxLevel; i++)
        {
            var rewardItem= transform.Find("Root/Scroll View/Viewport/Content/RewardGroup/"+i).gameObject.AddComponent<ClimbTreeRewardItem>();
            _rewardItems.Add(i,rewardItem);
        }
        
        _skeletonGraphic = transform.Find("Root/Scroll View/Viewport/Content/Monkey").GetComponent<SkeletonGraphic>();

        _content=transform.Find("Root/Scroll View/Viewport/Content") as RectTransform;
        _scrollRect = GetItem<ScrollRect>("Root/Scroll View");
        MinHeight = (int)((_scrollRect.transform as RectTransform).rect.height -
                    (_content.transform as RectTransform).rect.height);
        InvokeRepeating("UpdateTime", 0, 1);
        
        
        List<Transform> topLayer = new List<Transform>();
        topLayer.Add(_buttonStart.transform);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.ClimbTreePreview, transform as RectTransform);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.ClimbTreeDes, transform as RectTransform);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.ClimbTreePlay, _buttonStart.transform as RectTransform, topLayer:topLayer);
    }

    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        UpdateMonkeyPosition();
    }

    public async void ClimbTreeGuide(Action callback)//第一次打开时展示全部奖励的表演
    {
        _buttonStart.gameObject.SetActive(false);
        _buttonClose.gameObject.SetActive(false);
        int moveDuration = 4;
        _content.anchoredPosition = GetContentPosition(ClimbTreeModel.Instance.MaxLevel);
        
        await Task.Delay(1000);

        DOTween.To(() => _content.anchoredPosition, curPos => _content.anchoredPosition = curPos,
            GetContentPosition(ClimbTreeModel.Instance.MinLevel),
            moveDuration).SetTarget(_content).SetEase(Ease.Linear);
        await Task.Delay(moveDuration*1000);
        await Task.Delay(500);
        _buttonStart.gameObject.SetActive(true);
        _buttonClose.gameObject.SetActive(true);
        callback.Invoke();
    }
    
    private Task isPerforming;
    protected override async void OnOpenWindow(params object[] objs)  
    {
        base.OnOpenWindow(objs);
        InitLeaderBoardEntrance();
        RefreshUI();
        _skeletonGraphic.PlaySkeletonAnimation("idle",true);
        _firstPopUp = false;
        if (ClimbTreeModel.Instance.CanShowStartView())//是否第一次打开
        {
            _firstPopUp = true;
            ClimbTreeModel.Instance.ShowStartView();
        }

        var performTaskSource = new TaskCompletionSource<bool>();
        isPerforming = performTaskSource.Task;
        var remindRemoteInterval = StorageManager.Instance.RemoteInterval;
        var remindLocalInterval = StorageManager.Instance.LocalInterval;
        StorageManager.Instance.LocalInterval = 99999;
        StorageManager.Instance.RemoteInterval = 99999;
        if (ClimbTreeModel.Instance.CurLevel < ClimbTreeModel.Instance.TotalLevel)//触发升级时的表演
        {
            _scrollRect.enabled = false;
            _buttonStart.gameObject.SetActive(false);
            _buttonClose.gameObject.SetActive(false);
            await XUtility.WaitSeconds(0.3f);
            while (ClimbTreeModel.Instance.CurLevel < ClimbTreeModel.Instance.TotalLevel)
            {
                await PerformLevelUp();
            }

            // if (!ClimbTreeModel.Instance.IsMaxLevel())
            // {
                _buttonStart.gameObject.SetActive(true);
                _buttonClose.gameObject.SetActive(true);   
            // }
            _scrollRect.enabled = true;
        }
        if (ClimbTreeModel.Instance.IsMaxLevel())//满级时跳过剩余分数的收集
        {
            ClimbTreeModel.Instance.CompletedActivity();
        }
        else if (ClimbTreeModel.Instance.CurScore < ClimbTreeModel.Instance.TotalScore)//将CurScore同步到TotalScore
        {
            _buttonStart.gameObject.SetActive(false);
            _buttonClose.gameObject.SetActive(false);
            await PerformGrowProgress(ClimbTreeModel.Instance.TotalScore);
            _buttonStart.gameObject.SetActive(true);
            _buttonClose.gameObject.SetActive(true);
        }
        performTaskSource.SetResult(true);
        StorageManager.Instance.LocalInterval = remindLocalInterval;
        StorageManager.Instance.RemoteInterval = remindRemoteInterval;
        isPerforming = null;

        if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.ClimbTreePreview, null))
        {
            ClimbTreeGuide(() =>
            {
                GuideSubSystem.Instance.FinishCurrent(GuideTargetType.ClimbTreePreview);
            });
        }
        else
        {
            GuideSubSystem.Instance.Trigger(GuideTriggerPosition.ClimbTreeDes, null);
            GuideSubSystem.Instance.Trigger(GuideTriggerPosition.ClimbTreePlay, null);
        }
    }

    public void RefreshUI()
    {
        for (var i = ClimbTreeModel.Instance.MinLevel + 1; i <= ClimbTreeModel.Instance.MaxLevel; i++)
        {
            var rewards = ClimbTreeModel.Instance.GetLevelRewards(i);
            ClimbTreeRewardItem.ClimbTreeRewardStatus status = ClimbTreeRewardItem.ClimbTreeRewardStatus.None;
            if (ClimbTreeModel.Instance.CurLevel >= i)
            {
                status=ClimbTreeRewardItem.ClimbTreeRewardStatus.Finish;
            }
            _rewardItems[i].Init(rewards,status,i);
        }
        
        OnChangeScore();
        
        _content.anchoredPosition = GetContentPosition(ClimbTreeModel.Instance.CurLevel);
    }

    public Vector2 GetContentPosition(int level)
    {
        var y = Math.Max(MinHeight, MaxHeight - UnitHeight * level);
        return new Vector2(0, y);
    }
    private void OnStartBtn()
    {
        if (_firstPopUp)//第一次打开时点开始按钮要发BI
        {
            _firstPopUp = false;
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventMonkeyOn);
        }
        AnimCloseWindow(() =>
        {
            if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Game)
                return;
            SceneFsm.mInstance.TransitionGame();
            
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.ClimbTreePlay, null);
        });
    }

    private void OnCloseBtn()
    {
        AnimCloseWindow(() =>
        {
        });
    }

    // public void SetStartButtonStatus(bool isShow)
    // {
    //     _buttonStart.gameObject.SetActive(isShow);
    // }
    
    public void UpdateTime()
    {
        if (!ClimbTreeModel.Instance.IsPrivateOpened())
        {
            // _showType = ClimbTreeMainShowType.Failed;
            _timeGroup.gameObject.SetActive(false);
            if (isPerforming != null)
            {
                isPerforming.AddCallBack(() => 
                    AnimCloseWindow(()=>UIPopupClimbTreeEndController.CanShowUI())
                    ).WrapErrors();
            }
            else
            {
                AnimCloseWindow(() => UIPopupClimbTreeEndController.CanShowUI());
            }
            CancelInvoke("UpdateTime");
            return;
        }
        _timeGroupText.SetText(ClimbTreeModel.Instance.GetActivityLeftTimeString());
    }


    public override void ClickUIMask()
    {
        if (!canClickMask)
            return;

        canClickMask = false;
        AnimCloseWindow(() =>
        {
           
        });
    }

    // private static string coolTimeKey = "ClimbTree";
    // public static bool CanShowUI()
    // {
    //     if (!ClimbTreeModel.Instance.IsPrivateOpened())
    //         return false;
    //
    //     // if (ClimbTreeModel.Instance.CanShowStartView())
    //     // {
    //     //     CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey, CommonUtils.GetTimeStamp());
    //     //     UIManager.Instance.OpenUI(UINameConst.UIDogStart);
    //     //     return true;
    //     // }
    //
    //     if (!CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, coolTimeKey))
    //     {
    //         CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey, CommonUtils.GetTimeStamp());
    //         UIManager.Instance.OpenUI(UINameConst.UIClimbTreeMain);
    //         return true;
    //     }
    //     return false;
    // }
}
