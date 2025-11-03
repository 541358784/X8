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

public class MergeCardCollection : MonoBehaviour
{
    private Image Slider;
    private LocalizeTextMeshProUGUI SliderText;
    private LocalizeTextMeshProUGUI _countDownTime;
    private Button _butCoinRush;
    private CardCollectionEntranceThemeRedPoint _redPoint;
    // private Transform _rewardGroup;


    private void Awake()
    {
        Slider = transform.Find("Root/Slider").GetComponent<Image>();
        SliderText = transform.Find("Root/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _butCoinRush = transform.GetComponent<Button>();
        _butCoinRush.onClick.AddListener(OnClick);
        _countDownTime = transform.Find("Root/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        _redPoint = transform.Find("Root/RedPoint").gameObject.AddComponent<CardCollectionEntranceThemeRedPoint>();
        InvokeRepeating("RefreshCountDown", 0, 1f);
    }
    public void OnClick()
    {
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.CardMergeEntrance);
        UICardMainController.Open(Theme);
    }

    public void RefreshView()
    {
        gameObject.SetActive(CardCollectionActivityModel.Instance.IsOpened() && CardCollectionModel.Instance.IsResReady(Theme));
        if (gameObject.activeSelf)
        {
            var openTheme = CardCollectionActivityModel.Instance.GetThemeOnOpened();
            if (openTheme != Theme && openTheme.GetUpGradeTheme() != Theme)//不是本期卡册就隐藏
            {
                gameObject.SetActive(false);
            }
            if (Theme.GetUpGradeTheme() != Theme)//变金卡册也隐藏
            {
                gameObject.SetActive(false);
            }
        }
        if (Theme.CardThemeConfig.NeedUnLock && 
            !CardCollectionModel.Instance.StorageCardCollection.UnlockThemeList.Contains(Theme.CardThemeConfig.Id))
        {
            gameObject.SetActive(false);
        }
        var theme = Theme;
        if (theme != null)
        {
            var maxCardCount = 0;
            var collectCardCount = 0;
            foreach (var bookPair in theme.CardBookStateList)
            {
                collectCardCount += bookPair.Value.CollectCardItemCount;
                maxCardCount += bookPair.Value.MaxCardItemCount;
            }
            SliderText.SetText(collectCardCount+"/"+maxCardCount);
            Slider.fillAmount = collectCardCount / (float) maxCardCount;
        }
    }
    private void RefreshCountDown()
    {
        RefreshView();
        
        _countDownTime.SetText(CardCollectionActivityModel.Instance.GetActivityLeftTimeString());
    }

    private CardCollectionCardThemeState Theme;
    public void SetTheme(CardCollectionCardThemeState theme)
    {
        Theme = theme;
        _redPoint.SetTheme(Theme);
    }
}