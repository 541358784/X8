using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.ClimbTree;
using Gameplay;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class MergeClimbTree : MonoBehaviour
{
    private Image _slider;
    private LocalizeTextMeshProUGUI _needText;
    private LocalizeTextMeshProUGUI _countDownTime;
    private Button _butClimbTree;
    private SkeletonGraphic _skeletonGraphic;
    private Transform _completeBtn;
    private Transform _completeEffect;
    // private Image _rewardIcon;
    private Transform _rewardGroup;
    private int _currentLevel;
    private int _currentMaxValue;
    private int _currentValue;
    private LocalizeTextMeshProUGUI _rankText;

    private void Awake()
    {
        _rewardGroup = transform.Find("Root/Slider/RewardGroup");
        // _rewardIcon = transform.Find("Root/Slider/rewardIcon").GetComponent<Image>();
        _completeEffect = transform.Find("Root/Finish");
        _completeEffect.gameObject.SetActive(false);
        _completeBtn = transform.Find("Root/Button");
        _completeBtn.gameObject.SetActive(false);
        _slider = transform.Find("Root/Slider").GetComponent<Image>();
        _butClimbTree = transform.GetComponent<Button>();
        _butClimbTree.onClick.AddListener(() =>
        {
            if (PerformAddValueInAsync || BananaFlyState > 0)
                return;
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.ClimbTreeLeaderBoardMergeEntrance);
            UIClimbTreeMainController controller =
                (UIClimbTreeMainController) UIManager.Instance.OpenUI(UINameConst.UIClimbTreeMain);
            // controller.SetStartButtonStatus(false);
        });

        _needText = transform.Find("Root/Slider/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _countDownTime = transform.Find("Root/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();

        _skeletonGraphic = transform.Find("Root/Mask/PortraitSpine").GetComponent<SkeletonGraphic>();

        _rankText = transform.Find("Root/RankText").GetComponent<LocalizeTextMeshProUGUI>();
        _rankText.gameObject.SetActive(false);
        InvokeRepeating("RefreshCountDown", 0, 1f);
    }

    private void OnEnable()
    {
        InitUI();
        StopAllCoroutines();
        PlaySkeletonAnimation("idle");
    }

    private void InitUI()
    {
        if (!ClimbTreeModel.Instance.IsInitFromServer())
            return;
        _currentLevel = ClimbTreeModel.Instance.CurLevel;
        // _rewardIcon.sprite = UserData.GetResourceIcon(ClimbTreeConfigManager.Instance.GetLevelRewardsShowId(_currentLevel + 1));
        UpdateRewards();
        _currentValue = ClimbTreeModel.Instance.GetUpperScore(ClimbTreeModel.Instance.TotalScore,_currentLevel);
        _currentMaxValue = ClimbTreeModel.Instance.GetLevelStageScore(_currentLevel);   
        OnChangeValue();
    }

    private void RefreshCountDown()
    {
        _countDownTime.SetText(ClimbTreeModel.Instance.GetActivityLeftTimeString());

        RefreshState();
    }

    public void PerformLevelUp()
    {
        var rewards = ClimbTreeModel.Instance.GetLevelRewards(_currentLevel+1);
        for (var i = 0; i < rewards.Count; i++)
        {
            var resourceId = rewards[i].id;
            float delayTime = 0.3f;
            if (rewards[i].count >= 5)
                delayTime = 0.1f;
            FlyGameObjectManager.Instance.FlyCurrency(rewards[i].id, rewards[i].count, _rewardGroup.position, 1, false, delayTime, action:
                () =>
                {
                });
        }
        _currentLevel++;
        UpdateRewards();
        _currentValue -= _currentMaxValue;
        _currentMaxValue = ClimbTreeModel.Instance.GetLevelStageScore(_currentLevel);
        OnChangeValue();
    }

    public void RefreshState()
    {
        gameObject.SetActive(ClimbTreeModel.Instance.ShowEntrance());
        if (gameObject.activeSelf)
        {
            UpdateRankText();
        }
    }
    public void UpdateRankText()
    {
        var enterLeaderBoard = ClimbTreeModel.Instance.CurStorageClimbTreeWeek.LeaderBoardStorage.IsInitFromServer();
        _rankText.gameObject.SetActive(true);
        if (enterLeaderBoard)
        {
            _rankText.SetText("No."+ClimbTreeModel.Instance.CurStorageClimbTreeWeek.LeaderBoardStorage.SortController().MyRank);
        }
        else
        {
            _rankText.SetText(ClimbTreeModel.Instance.CurStorageClimbTreeWeek.TotalScore+"/"+ClimbTreeLeaderBoardModel.Instance.LeastEnterBoardScore);
        }
    }
    public void UpdateRewards()
    {
        UpdateRankText();
        var maxRewardsCount = 3;
        if (_currentLevel < ClimbTreeModel.Instance.MaxLevel)
        {
            {
                var rewardIcon = _rewardGroup.Find("Item1").GetComponent<Image>();
                rewardIcon.sprite = UserData.GetResourceIcon(ClimbTreeModel.Instance.GetLevelRewardsShowId(_currentLevel + 1));   
            }
            for (var i = 1; i < maxRewardsCount; i++)
            {
                var rewardIcon = _rewardGroup.Find("Item" + (i + 1)).GetComponent<Image>();
                rewardIcon.gameObject.SetActive(false);
            }   
        }
        else
        {
            for (var i = 0; i < maxRewardsCount; i++)
            {
                var rewardIcon = _rewardGroup.Find("Item" + (i + 1)).GetComponent<Image>();
                rewardIcon.gameObject.SetActive(false);
            }  
        }
        // var rewards = ClimbTreeModel.Instance.GetLevelRewards(_currentLevel+1);
        // for (int i = 0; i < rewards.Count; i++)
        // {
        //     var rewardIcon = _rewardGroup.Find("Item" + (i + 1)).GetComponent<Image>();
        //     rewardIcon.sprite = UserData.GetResourceIcon(rewards[i].id);
        //     rewardIcon.gameObject.SetActive(true);
        // }
        //
        // for (var i = rewards.Count; i < maxRewardsCount; i++)
        // {
        //     var rewardIcon = _rewardGroup.Find("Item" + (i + 1)).GetComponent<Image>();
        //     rewardIcon.gameObject.SetActive(false);
        // }
    }

    private int BananaFlyState = 0;
    public void PreBananaFly()
    {
        BananaFlyState++;
    }
    private List<TaskCompletionSource<bool>> PerformAddValueAsyncLock = new List<TaskCompletionSource<bool>>();
    private bool PerformAddValueInAsync = false;
    public async void PerformAddValue(int subNum, float time, List<TaskCompletionSource<bool>> taskList)
    {
        BananaFlyState--;
        PlayChangeTextAnim();
        if (PerformAddValueInAsync)
        {
            var asyncLock = new TaskCompletionSource<bool>();
            PerformAddValueAsyncLock.Add(asyncLock);
            await asyncLock.Task;
        }
        PerformAddValueInAsync = true;
        while (taskList.Count > 0)
        {
            var task = new TaskCompletionSource<bool>();
            var localSubNum = _currentMaxValue - _currentValue;
            int lastUpdateSubNum = 0;
            subNum -= localSubNum;
            await XUtility.WaitFrames(1);
            DOTween.To(() => 0f, curSubNumF =>
            {
                var curSubNum = (int) Mathf.Floor(curSubNumF);
                var addValue = curSubNum - lastUpdateSubNum;
                lastUpdateSubNum = curSubNum;
                _currentValue += addValue;
                OnChangeValue();
            }, localSubNum, time).OnComplete(() =>
            {
                var addValue = localSubNum - lastUpdateSubNum;
                lastUpdateSubNum = addValue;
                _currentValue += addValue;
                OnChangeValue();
                taskList[0].SetResult(true);
                task.SetResult(true);
            });
            await task.Task;
        }

        if (ClimbTreeModel.Instance.IsMaxLevel())
        {
            // gameObject.SetActive(false);
            OnChangeValue();
        }
        else 
        if (subNum > 0)
        {
            var task = new TaskCompletionSource<bool>();
            int lastUpdateSubNum = 0;
            await XUtility.WaitFrames(1);
            DOTween.To(() => 0f, curSubNumF =>
            {
                var curSubNum = (int) Mathf.Floor(curSubNumF);
                var addValue = curSubNum - lastUpdateSubNum;
                lastUpdateSubNum = curSubNum;
                _currentValue += addValue;
                OnChangeValue();;
            }, subNum, time).OnComplete(() =>
            {
                var addValue = subNum - lastUpdateSubNum;
                lastUpdateSubNum = addValue;
                _currentValue += addValue;
                OnChangeValue();
                task.SetResult(true);
            });
            await task.Task;
        }
        PerformAddValueInAsync = false;
        if (PerformAddValueAsyncLock.Count > 0)
        {
            var asyncLock = PerformAddValueAsyncLock[0];
            PerformAddValueAsyncLock.RemoveAt(0);
            asyncLock.SetResult(true);
        }
    }

    private bool maxState = false;

    public void OnChangeValue()
    {
        if (_currentLevel < ClimbTreeModel.Instance.MaxLevel)
        {
            _needText.gameObject.SetActive(true);
            _slider.gameObject.SetActive(true);
            _needText.SetText(_currentValue + "/" + _currentMaxValue);
            _slider.fillAmount = Mathf.Min(_currentValue / (float) _currentMaxValue, 1f);
        }
        else
        {
            _needText.gameObject.SetActive(true);
            _slider.gameObject.SetActive(true);
            _needText.SetText(ClimbTreeModel.Instance.TotalScore.ToString());
            _slider.fillAmount = 1;
        }
        
        var curMaxState = _currentValue >= _currentMaxValue;
        if (maxState != curMaxState)
        {
            maxState = curMaxState;
            // _completeBtn.gameObject.SetActive(curMaxState);
            // _completeEffect.gameObject.SetActive(curMaxState);
        }
    }

    private void PlayChangeTextAnim()
    {
        StopAllCoroutines();
        float time = PlaySkeletonAnimation("eat");
        StartCoroutine(CommonUtils.DelayWork(time, () => { PlaySkeletonAnimation("idle"); }));
    }

    private float PlaySkeletonAnimation(string animName)
    {
        if (_skeletonGraphic == null)
            return 0;

        TrackEntry trackEntry = _skeletonGraphic.AnimationState.GetCurrent(0);
        if (trackEntry != null && trackEntry.Animation != null && trackEntry.Animation.Name == animName)
            return trackEntry.AnimationEnd;

        _skeletonGraphic.AnimationState?.SetAnimation(0, animName, true);
        _skeletonGraphic.Update(0);

        return trackEntry.AnimationEnd;
    }
}