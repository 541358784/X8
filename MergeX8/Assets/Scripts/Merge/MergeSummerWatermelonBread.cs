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

public class MergeSummerWatermelonBread : MonoBehaviour
{
    private Transform _timeGroup;
    private LocalizeTextMeshProUGUI _countDownTime;
    private Button _butCoinRush;
    private Transform _redPoint;
    private LocalizeTextMeshProUGUI _redPointText;
    public static MergeSummerWatermelonBread Instance;
    private void Awake()
    {
        if (!Instance)
            Instance = this;
        _timeGroup = transform.Find("Root/TimeGroup");
        _butCoinRush = transform.GetComponent<Button>();
        _butCoinRush.onClick.AddListener(OnClick);
        _redPoint = transform.Find("Root/RedPoint");
        _redPointText = transform.Find("Root/RedPoint/Label").GetComponent<LocalizeTextMeshProUGUI>();
        _countDownTime = transform.Find("Root/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();

        InvokeRepeating("RefreshCountDown", 0, 1f);
    }

    public void Update()
    {
    }
    public void OnClick()
    {
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.SummerWatermelonBreadGameEntrance);
        SummerWatermelonBreadModel.Instance.OpenMainPopup();
    }

    private void OnEnable()
    {
        RefreshView();
        StopAllCoroutines();
    }

    public void RefreshView()
    {
        gameObject.SetActive(SummerWatermelonBreadModel.Instance.IsStart);
        
        if (!SummerWatermelonBreadModel.Instance.IsStart)
            return;
        // _timeGroup.gameObject.SetActive(SummerWatermelonBreadModel.Instance.IsStart);
        _redPoint.gameObject.SetActive(SummerWatermelonBreadModel.Instance.UnSetItemsCount > 0);
        _redPointText.gameObject.SetActive(SummerWatermelonBreadModel.Instance.UnSetItemsCount > 1);
        _redPointText.SetText(SummerWatermelonBreadModel.Instance.UnSetItemsCount.ToString());
    }
    private void RefreshCountDown()
    {
        RefreshView();
        if (SummerWatermelonBreadModel.Instance.IsStart)
            _countDownTime.SetText(SummerWatermelonBreadModel.Instance.GetActivityLeftTimeString());
    }
}