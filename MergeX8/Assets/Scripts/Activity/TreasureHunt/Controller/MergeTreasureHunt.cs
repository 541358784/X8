using System;
using System.Collections.Generic;
using Activity.TreasureHuntModel;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.ClimbTree;
using Gameplay;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class MergeTreasureHunt : MonoBehaviour
{
    private LocalizeTextMeshProUGUI _countDownTime;
    private Button _button;
    private Transform _redPoint;
    private LocalizeTextMeshProUGUI _redPointText;
    private LocalizeTextMeshProUGUI _rewardText;
    private Image _slider;
    private LocalizeTextMeshProUGUI _sliderText;
    private void Awake()
    {
        _button = transform.GetComponent<Button>();
        _button.onClick.AddListener(OnClick);
        _redPoint = transform.Find("RedPoint");
        _redPointText = transform.Find("RedPoint/Label").GetComponent<LocalizeTextMeshProUGUI>();
        _countDownTime = transform.Find("TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        _rewardText = transform.Find("Reward/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _rewardText.gameObject.SetActive(false);
        _slider = transform.Find("Slider").GetComponent<Image>();
        _sliderText = transform.Find("Slider/Text").GetComponent<LocalizeTextMeshProUGUI>();
        InvokeRepeating("RefreshCountDown", 0, 1f);
    }

    public void Update()
    {
        RefreshView();
    }
    public void OnClick()
    {
        TreasureHuntModel.Instance.OpenMainPopup();
    }

    private void OnEnable()
    {
        RefreshView();
        StopAllCoroutines();
    }

    public void RefreshView()
    {
        if (!TreasureHuntModel.Instance.IsStart())
            return;
        _redPoint.gameObject.SetActive(TreasureHuntModel.Instance.GetHammer() > 0);
        _redPointText.SetText(TreasureHuntModel.Instance.GetHammer().ToString());
        _slider.fillAmount = TreasureHuntModel.Instance.GetCostProgress();
        _sliderText.SetText(TreasureHuntModel.Instance.TreasureHunt.EnergyCost+"/"+TreasureHuntModel.Instance.GetCastEnergy());
        _rewardText.SetText(TreasureHuntModel.Instance.TreasureHuntActivityConfig.HammerCount.ToString());
    }
    private void RefreshCountDown()
    {
        if (TreasureHuntModel.Instance.IsStart())
            _countDownTime.SetText(TreasureHuntModel.Instance.GetActivityLeftTimeString());
    }
}