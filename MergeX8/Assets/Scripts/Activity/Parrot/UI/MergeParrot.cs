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

public class MergeParrot : MonoBehaviour
{
    public static MergeParrot Instance=>MergeTaskTipsController.Instance.MergeParrot;
    private LocalizeTextMeshProUGUI _countDownTime;
    private Button _butCoinRush;
    // private Transform _redPoint;
    // private LocalizeTextMeshProUGUI _redPointText;
    public Image RewardIcon;
    public Image Slider;
    public LocalizeTextMeshProUGUI SliderText;
    public LocalizeTextMeshProUGUI RankText;
    private void Awake()
    {
        RankText = transform.Find("RankText").GetComponent<LocalizeTextMeshProUGUI>();
        _butCoinRush = transform.GetComponent<Button>();
        _butCoinRush.onClick.AddListener(OnClick);
        // _redPoint = transform.Find("Root/RedPoint");
        // _redPointText = transform.Find("Root/RedPoint/Label").GetComponent<LocalizeTextMeshProUGUI>();
        _countDownTime = transform.Find("TimeGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();

        InvokeRepeating("RefreshCountDown", 0, 1f);
        RewardIcon = transform.Find("RewardIcon").GetComponent<Image>();
        Slider = transform.Find("Slider").GetComponent<Image>();
        SliderText = transform.Find("Slider/Text").GetComponent<LocalizeTextMeshProUGUI>();

        InitScore();
    }

    public void InitScore()
    {
        Score = ParrotModel.Instance.Storage.TotalScore;
        ShowScore = Score;
        
        var newState = ParrotModel.Instance.GetLevelState(ShowScore);
        Slider.fillAmount = (float)newState.CurScore / newState.MaxScore;
        if (newState.Rewards.Count > 0)
        {
            SliderText.SetText(newState.CurScore + "/" + newState.MaxScore);
            RewardIcon.gameObject.SetActive(true);
            RewardIcon.sprite = UserData.GetResourceIcon(newState.Rewards[0].id, UserData.ResourceSubType.Big);   
        }
        else
        {
            SliderText.SetText(newState.CurScore.ToString());
            RewardIcon.gameObject.SetActive(false);
        }
    }
    
    public void OnClick()
    {
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.ParrotStart);
        if (!ParrotModel.CanShowStartPopup())
            UIParrotMainController.Open();
    }

    private void OnEnable()
    {
        RefreshView();
        StopAllCoroutines();
    }

    private void RefreshView()
    {
        gameObject.SetActive(ParrotModel.Instance.IsStart());
        // _redPoint.gameObject.SetActive(false);
        if (gameObject.activeSelf)
        {
            var leaderBoard = ParrotLeaderBoardModel.Instance.GetLeaderBoardStorage(ParrotModel.Instance.Storage.ActivityId);
            RankText.gameObject.SetActive(leaderBoard.IsStorageWeekInitFromServer());
            if (RankText.gameObject.activeSelf)
            {
                RankText.SetText("No."+leaderBoard.SortController().MyRank);
            }
        }
    }
    private void RefreshCountDown()
    {
        RefreshView();
        
        _countDownTime.SetText(ParrotModel.Instance.GetActivityLeftTimeString());
    }

    public int Score;
    public int ShowScore;
    public void PerformAddValue(int value)
    {
        Score += value;
        
    }
    public void Update()
    {
        if (ShowScore < Score)
        {
            var oldState = ParrotModel.Instance.GetLevelState(ShowScore);
            var distance = Score - ShowScore;
            if (distance <= 10)
            {
                ShowScore++;   
            }
            else
            {
                ShowScore += distance / 10;
            }
            var newState = ParrotModel.Instance.GetLevelState(ShowScore);
            Slider.fillAmount = (float)newState.CurScore / newState.MaxScore;
            SliderText.SetText(newState.CurScore + "/" + newState.MaxScore);
            if (newState.Rewards.Count == 0)
            {
                SliderText.SetText(newState.CurScore.ToString());
            }
            if (newState.Level != oldState.Level)
            {
                if (newState.Rewards.Count > 0)
                {
                    RewardIcon.gameObject.SetActive(true);
                    RewardIcon.sprite = UserData.GetResourceIcon(newState.Rewards[0].id, UserData.ResourceSubType.Big);   
                }
                else
                {
                    RewardIcon.gameObject.SetActive(false);
                }
            }
        }
    }
}