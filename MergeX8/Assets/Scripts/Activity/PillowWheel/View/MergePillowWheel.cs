using System;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.ClimbTree;
using Gameplay;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class MergePillowWheel : MonoBehaviour
{
    public static MergePillowWheel Instance=>MergeTaskTipsController.Instance.MergePillowWheel;
    private LocalizeTextMeshProUGUI _countDownTime;
    private Button _butCoinRush;
    // private Transform _redPoint;
    // private LocalizeTextMeshProUGUI _redPointText;
    private PillowWheelEntranceRedPoint _redPoint;
    public LocalizeTextMeshProUGUI CountText;
    public LocalizeTextMeshProUGUI RankText;
    private void Awake()
    {
        RankText = transform.Find("RankText").GetComponent<LocalizeTextMeshProUGUI>();
        _butCoinRush = transform.GetComponent<Button>();
        _butCoinRush.onClick.AddListener(OnClick);
        _redPoint = transform.Find("RedPoint")?.gameObject.AddComponent<PillowWheelEntranceRedPoint>();
        if (_redPoint)
            _redPoint.Init();
        // _redPoint = transform.Find("Root/RedPoint");
        // _redPointText = transform.Find("Root/RedPoint/Label").GetComponent<LocalizeTextMeshProUGUI>();
        _countDownTime = transform.Find("TimeGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();
        InvokeRepeating("RefreshCountDown", 0, 1f);
        CountText = transform.Find("Num/Text").GetComponent<LocalizeTextMeshProUGUI>();
    }
    

    public void UpdateViewState()
    {
        CountText.SetText("x"+PillowWheelModel.Instance.GetItem());
    }
    public void OnClick()
    {
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.PillowWheelStart);
        UIPillowWheelMainController.Open();
    }

    private void OnEnable()
    {
        RefreshView();
        StopAllCoroutines();
    }

    private void RefreshView()
    {
        gameObject.SetActive(PillowWheelModel.Instance.IsStart());
        // _redPoint.gameObject.SetActive(false);
        if (gameObject.activeSelf)
        {
            UpdateViewState();
            var leaderBoard = PillowWheelLeaderBoardModel.Instance.GetLeaderBoardStorage(PillowWheelModel.Instance.Storage.ActivityId);
            RankText.gameObject.SetActive(leaderBoard.IsStorageWeekInitFromServer());
            if (RankText.gameObject.activeSelf)
            {
                RankText.SetText("No."+leaderBoard.SortController().MyRank);
            }
        }
    }

    private bool isLockNum = false;
    public void LockNum()
    {
        isLockNum = true;
    }

    public void UnlockNum()
    {
        isLockNum = false;
        UpdateViewState();
    }
    private void RefreshCountDown()
    {
        RefreshView();
        
        _countDownTime.SetText(PillowWheelModel.Instance.GetActivityLeftTimeString());
    }
}