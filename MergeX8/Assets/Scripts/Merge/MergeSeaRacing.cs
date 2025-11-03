using System;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.CoinRush;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class MergeSeaRacing : MonoBehaviour
{
    private Image Slider;
    private LocalizeTextMeshProUGUI SliderText;
    private LocalizeTextMeshProUGUI _countDownTime;
    private Transform RewrdNode;
    private Button _butCoinRush;
    private Button CollectBtn;
    // private Transform _rewardGroup;


    private void Awake()
    {
        Slider = transform.Find("Root/Slider").GetComponent<Image>();
        SliderText = transform.Find("Root/Text").GetComponent<LocalizeTextMeshProUGUI>();
        RewrdNode = transform.Find("Root/Reward");
        _butCoinRush = transform.GetComponent<Button>();
        _butCoinRush.onClick.AddListener(OnClick);
        
        _countDownTime = transform.Find("Root/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        transform.Find("Root/RedPoint").gameObject.SetActive(false);
        CollectBtn = transform.Find("Root/Button").GetComponent<Button>();
        CollectBtn.onClick.AddListener(OnClick);
        InvokeRepeating("RefreshCountDown", 0, 1f);
    }
    public void OnClick()
    {
        // GuideSubSystem.Instance.FinishCurrent(GuideTargetType.SeaRacingInfo);
        if (!SeaRacingModel.Instance.IsStart() ||
            SeaRacingModel.Instance.CurStorageSeaRacingWeek.CurRound() == null || 
            !SeaRacingModel.Instance.CurStorageSeaRacingWeek.CurRound().IsStart)
        {
            return;
        }

        if (SeaRacingModel.Instance.CurStorageSeaRacingWeek.CurRound().IsStart)
        {
            SeaRacingModel.CanShowMainPopup();   
        }
    }

    private bool Finish = false;
    public void RefreshView()
    {
        if (!SeaRacingModel.Instance.IsStart() ||
            SeaRacingModel.Instance.CurStorageSeaRacingWeek.IsFinish ||
            SeaRacingModel.Instance.CurStorageSeaRacingWeek.CurRound() == null ||
            !SeaRacingModel.Instance.CurStorageSeaRacingWeek.CurRound().IsStart)
        {
            gameObject.SetActive(false);
            return;   
        }
        gameObject.SetActive(true);
        var round = SeaRacingModel.Instance.CurStorageSeaRacingWeek.CurRound();
        for (var i = 1; i <= 3; i++)
        {
            RewrdNode.Find(i.ToString()).gameObject.SetActive(round.SortController().MyRank == i);
            // transform.Find("Root/Icon"+i).gameObject.SetActive(round.SortController().MyRank == i);
        }
        SliderText.SetText(round.Score+"/"+round.MaxScore);
        Slider.fillAmount = round.Score / (float) round.MaxScore;
        CollectBtn.gameObject.SetActive(round.Score==round.MaxScore);
        var curFinish = round.Score == round.MaxScore;
        // CollectBtn.gameObject.SetActive(false);
        if (curFinish && !Finish)
        {
            if (MergeTaskTipsController.Instance != null && MergeTaskTipsController.Instance.contentRect != null)
            {
                MergeTaskTipsController.Instance.contentRect.DOAnchorPosX(-transform.localPosition.x+220, 0);
            }   
        }
        Finish = curFinish;
    }
    private void RefreshCountDown()
    {
        RefreshView();
        
        if (!SeaRacingModel.Instance.IsStart() ||
            SeaRacingModel.Instance.CurStorageSeaRacingWeek.IsFinish ||
            SeaRacingModel.Instance.CurStorageSeaRacingWeek.CurRound() == null ||
            !SeaRacingModel.Instance.CurStorageSeaRacingWeek.CurRound().IsStart)
        {
            return;   
        }
        _countDownTime.SetText(SeaRacingModel.Instance.CurStorageSeaRacingWeek.GetLeftTimeText());
    }
}