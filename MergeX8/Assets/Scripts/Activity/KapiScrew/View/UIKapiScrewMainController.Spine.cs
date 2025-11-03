using System.Text.RegularExpressions;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using Spine.Unity;
using UnityEngine;

public partial class UIKapiScrewMainController
{
    private SkeletonGraphic Spine;
    private Animator WinText;
    private Animator FailText;
    public void InitSpine()
    {
        Spine = transform.Find("Root/SkeletonGraphic (volleyball)").GetComponent<SkeletonGraphic>();
        if (Storage.SmallLevel == 0)
            Spine.PlaySkeletonAnimation("idle", true);
        else
        {
            StartLoopPlayBall();
        }

        WinText = transform.Find("Root/SuccessText").GetComponent<Animator>();
        WinText.gameObject.SetActive(false);
        FailText = transform.Find("Root/FailedText").GetComponent<Animator>();
        FailText.gameObject.SetActive(false);
    }

    private bool LoopPlayBall;
    private bool IsGameWin;
    private int SpineIndex = 0;
    public async void StartLoopPlayBall()
    {
        if (LoopPlayBall)
            return;
        LoopPlayBall = true;
        SpineIndex++;
        var spineIndex = SpineIndex;
        while (LoopPlayBall)
        {
            await Spine.PlaySkeletonAnimationAsync("play", false);
            if (spineIndex != SpineIndex)
                return;
            if (!this)
                return;
        }

        if (IsGameWin)
        {
            await Spine.PlaySkeletonAnimationAsync("play_1", false);
            if (!this)
                return;
            UpdateScore(IsGameWin);
            XUtility.WaitSeconds(1f, () =>
            {
                if (!this)
                    return;
                WinText.gameObject.SetActive(true);
                WinText.PlayAnimation("change", () =>
                {
                    WinText.gameObject.SetActive(false);
                });
            });
            await Spine.PlaySkeletonAnimationAsync("win_1", false);
        }
        else
        {
            await Spine.PlaySkeletonAnimationAsync("play_2", false);
            if (!this)
                return;
            UpdateScore(IsGameWin);
            XUtility.WaitSeconds(1f, () =>
            {
                if (!this)
                    return;
                FailText.gameObject.SetActive(true);
                FailText.PlayAnimation("change", () =>
                {
                    FailText.gameObject.SetActive(false);
                });
            });
            await Spine.PlaySkeletonAnimationAsync("win_2", false);
        }
        if (!this)
            return;
        if (IsGameWin && (KapiScrewModel.Instance.IsFinished() || Storage.SmallLevel == 0))
        {
            var levelConfig = KapiScrewModel.Instance.GetLevelConfig(Storage.SmallLevel == 0?Storage.BigLevel-1:Storage.BigLevel);
            var rewards = CommonUtils.FormatReward(levelConfig.RewardId, levelConfig.RewardNum);
            var reason = new GameBIManager.ItemChangeReasonArgs()
            {
                reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.KapiScrew
            };
            CommonRewardManager.Instance.PopCommonReward(rewards, CurrencyGroupManager.Instance.currencyController,
                false, reason,animEndCall: () =>
                {
                    if (KapiScrewModel.Instance.IsFinished())
                    {
                        AnimCloseWindow();
                    }
                    else
                    {  
                        MyScoreBoard.SetValue(Storage.SmallLevel);
                        EnemyScoreBoard.SetValue(Storage.SmallLevel);
                        MatchBtn.gameObject.SetActive(true);
                        UpdateLevelRewards();
                    }
                });
        }
        else if (!IsGameWin)
        {
            MyScoreBoard.SetValue(Storage.SmallLevel);
            EnemyScoreBoard.SetValue(Storage.SmallLevel);
            MatchBtn.gameObject.SetActive(true);   
        }
        else
        {
            MyScoreBoard.SetValue(Storage.SmallLevel);
            EnemyScoreBoard.SetValue(Storage.SmallLevel);
            StartBtn.gameObject.SetActive(true);   
        }
        if (IsGameWin)
        {
            Spine.PlaySkeletonAnimation("win_1", true);
        }
        else
        {
            Spine.PlaySkeletonAnimation("win_2", true);
        }
    }

    public void PerformOnGameFail()
    {
        LoopPlayBall = false;
        StartLoopPlayBall();
        LoopPlayBall = false;
        IsGameWin = false;
        StartBtn.gameObject.SetActive(false);
    }
    public async void PerformOnGameWin()
    {
        if (KapiScrewModel.Instance.IsFinished() || Storage.SmallLevel == 0)
        {
            LoopPlayBall = false;
            StartLoopPlayBall();
            LoopPlayBall = false;
            IsGameWin = true;
            StartBtn.gameObject.SetActive(false);   
        }
        else
        {
            LoopPlayBall = false;
            StartLoopPlayBall();
            await XUtility.WaitSeconds(0.5f);
            if (!this)
                return;
            UpdateScore(true);
            XUtility.WaitSeconds(1f, () =>
            {
                if (!this)
                    return;
                WinText.gameObject.SetActive(true);
                WinText.PlayAnimation("change", () =>
                {
                    WinText.gameObject.SetActive(false);
                });
            });
        }
    }
}