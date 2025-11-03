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

public class MergeSummerWatermelon : MonoBehaviour
{
    private Transform _timeGroup;
    private LocalizeTextMeshProUGUI _countDownTime;
    private Button _butCoinRush;
    private Transform _redPoint;
    private LocalizeTextMeshProUGUI _redPointText;

    private void Awake()
    {
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
        RefreshView();
    }
    public void OnClick()
    {
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.SummerWatermelonGameEntrance);
        SummerWatermelonModel.Instance.OpenMainPopup();
    }

    private void OnEnable()
    {
        RefreshView();
        StopAllCoroutines();
    }

    public void RefreshView()
    {
        if (!SummerWatermelonModel.Instance.IsStart)
            return;
        // _timeGroup.gameObject.SetActive(SummerWatermelonModel.Instance.IsStart);
        _redPoint.gameObject.SetActive(SummerWatermelonModel.Instance.UnSetItemsCount > 0);
        _redPointText.gameObject.SetActive(SummerWatermelonModel.Instance.UnSetItemsCount > 1);
        _redPointText.SetText(SummerWatermelonModel.Instance.UnSetItemsCount.ToString());
    }
    private void RefreshCountDown()
    {
        if (SummerWatermelonModel.Instance.IsStart)
            _countDownTime.SetText(SummerWatermelonModel.Instance.GetActivityLeftTimeString());
    }
}