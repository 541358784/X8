using System;
using DG.Tweening;
using DragonPlus;
using Framework;
using TMatch;
using UnityEngine;
using UnityEngine.UI;

public class CrocodileGateView : UIEntranceBase<CrocodileActivityModel>
{
    public bool CanShow;
    public LocalizeTextMeshProUGUI TimeText;

    private Animator _addItems;
    private Image _fill;
    private ulong remainTime;

    public void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnGateButtonClicked);

        TimeText = transform.Find("TipsBG/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        _addItems = transform.Find("UIActivityAddItems").GetComponent<Animator>();
        _fill = transform.Find("BG/Fill").GetComponent<Image>();

        _addItems.gameObject.SetActive(false);

        InvokeRepeating("UpdateRepeating", 0, 1);
    }

    private void Start()
    {
        Refresh();
        UpdateProgressAsync();

        TMatch.EventDispatcher.Instance.AddEventListener(EventEnum.RefreshWinStreak, OnRefresh);
        TMatch.EventDispatcher.Instance.AddEventListener(EventEnum.RefreshWinStreakProgress, UpdateProgress);
    }

    protected override void OnDestroy()
    {
        TMatch.EventDispatcher.Instance.RemoveEventListener(EventEnum.RefreshWinStreak, OnRefresh);
        TMatch.EventDispatcher.Instance.RemoveEventListener(EventEnum.RefreshWinStreakProgress, UpdateProgress);
    }

    private void OnRefresh(TMatch.BaseEvent evt)
    {
        Refresh();
        UpdateProgressAsync();
    }

    private void UpdateProgress(TMatch.BaseEvent evt)
    {
        UpdateProgressAsync();
    }

    public void Refresh()
    {
        CanShow = CrocodileActivityModel.Instance.CanShowGateView();
        gameObject.SetActive(CanShow);
    }

    private async void UpdateProgressAsync(bool isAni = false)
    {
        if (!CanShow) return;
        var storageLevel = CrocodileActivityModel.Instance.Storage.Level;
        var curProgress = (float)storageLevel / CrocodileActivityModel.Instance.TOTAL_PLAT_COUNT;
        if (!CrocodileActivityModel.Instance.Storage.EnterLevelResult) curProgress = 0f;
        curProgress = curProgress > 1.0f ? 1.0f : curProgress;
        if (isAni)
        {
            DOTween.Kill(_fill);
            _fill.DOFillAmount(Mathf.Lerp(0.1f, 0.9f, curProgress), 0.2f);
        }
        else
        {
            _fill.fillAmount = Mathf.Lerp(0.1f, 0.9f, curProgress);
        }
    }

    /// <summary>
    /// 更新时间
    /// </summary>
    protected override void UpdateTime()
    {
        base.UpdateTime();
        if (!CrocodileActivityModel.Instance.IsActivityOpened())
        {
            return;
        }

        var leftTime = CrocodileActivityModel.Instance.GetCurrentActivityLeftTime(); // 获取当前轮次的剩余时间

        if (CrocodileActivityModel.Instance.IsFinishCurrentTurnReward() ||
            (!CrocodileActivityModel.Instance.IsInChallenge() &&
             !CrocodileActivityModel.Instance.CanStartChallenge() &&
             !CrocodileActivityModel.Instance.CanStartInCurrentTurn()))
        {
            TimeText.SetTerm("UI_lava_finished");
            return;
        }
        TimeText.SetText(CommonUtils.FormatLongToTimeStr(leftTime));

        if (leftTime <= 0)
        {
            TimeText.SetTerm("UI_lava_finished");
            EventDispatcher.Instance.DispatchEvent(EventEnum.RefreshWinStreak);
        }
    }

    private void OnGateButtonClicked()
    {
        CrocodileActivityModel.Instance.CheckChallengeIsOutTime();
        if (CrocodileActivityModel.Instance.IsInChallenge())
        {
            UICrocodileMainController.Open();
        }
        else
        {
            UIPopupCrocodileStartController.Open();
        }
    }

    private void UpdateRepeating()
    {
        UpdateTime();
        if(!CanShow)
            return;
        
        if (CanShow)
        {
            // 尝试主动刷新显示
            Refresh();

            if (CanShow)
            {
                // 尝试主动刷新进度
                TMatch.EventDispatcher.Instance.DispatchEvent(EventEnum.RefreshWinStreakProgress);
            }

            return;
        }
    }
}