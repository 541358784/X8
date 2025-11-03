using System;
using System.Collections.Generic;
using ActivityLocal.CardCollection.Home;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public partial class UICardMainController : UIWindowController
{
    private Button CloseButton;
    private Button HelpButton;
    private Button TradeButton;
    private LocalizeTextMeshProUGUI CardBookCompletedProgress;
    private Slider CardBookCompletedSlider;
    private CardCollectionCardThemeState CurrentThemeState;
    private Transform DefaultCardBookItem;
    private Dictionary<int, CardBookItem> CardBookItemDictionary = new Dictionary<int, CardBookItem>();
    private Transform DefaultWildCardItem;
    private Dictionary<int, WildCardItem> WildCardItemDictionary = new Dictionary<int, WildCardItem>();
    private LocalizeTextMeshProUGUI TimeText;
    private Transform TimeGroup;
    private WildCardView WildCardViewController;
    private Transform DefaultRewardItem;
    private List<CommonRewardItem> RewardItemList = new List<CommonRewardItem>();
    private Transform CompletedFlag;
    private LocalizeTextMeshProUGUI NameText;
    public override void PrivateAwake()
    {
        CloseButton = transform.Find("Root/ButtonClose").GetComponent<Button>();
        CloseButton.onClick.AddListener(OnClickCloseButton);
        HelpButton = transform.Find("Root/ButtonHelp").GetComponent<Button>();
        HelpButton.onClick.AddListener(OnClickHelpButton);
        TradeButton = transform.Find("Root/ExchangeButton").GetComponent<Button>();
        TradeButton.onClick.AddListener(OnClickTradeButton);
        TradeButton.gameObject.SetActive(false);
        CardBookCompletedProgress = transform.Find("Root/Slider/Text").GetComponent<LocalizeTextMeshProUGUI>();
        CardBookCompletedSlider = transform.Find("Root/Slider").GetComponent<Slider>();
        DefaultCardBookItem = transform.Find("Root/Scroll View/Viewport/Content/CardBook");
        DefaultCardBookItem.gameObject.SetActive(false);
        DefaultWildCardItem = transform.Find("Root/UniversalCardGroup/UniversalCard");
        DefaultWildCardItem.gameObject.SetActive(false);
        TimeGroup = transform.Find("Root/TimeGroup");
        TimeText = transform.Find("Root/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        WildCardViewController = transform.Find("Root/UICardView").gameObject.AddComponent<WildCardView>();
        WildCardViewController.gameObject.SetActive(true);
        WildCardViewController.gameObject.SetActive(false);
        DefaultRewardItem = transform.Find("Root/RewardGroup/Item");
        DefaultRewardItem.gameObject.SetActive(false);
        CompletedFlag = transform.Find("Root/RewardGroup/Completed");
        NameText = transform.Find("Root/BG/TextTitle").GetComponent<LocalizeTextMeshProUGUI>();
        InvokeRepeating("UpdateTime", 0, 1);
        AddAllEvent();
    }

    public void UpdateTime()
    {
        TimeText.SetText(CardCollectionActivityModel.Instance.GetActivityLeftTimeString());
    }
    private void OnDestroy()
    {
        RemoveAllEvent();
    }

    public void InitView()
    {
        foreach (var pair in CardBookItemDictionary)
        {
            Destroy(pair.Value.gameObject);
        }

        CardBookItemDictionary.Clear();
        var inUseCardBookConfig = CurrentThemeState.CardBookStateList;
        foreach (var pair in inUseCardBookConfig)
        {
            var book = pair.Value;
            var bookId = pair.Key;
            var cardBookItemObj = Instantiate(DefaultCardBookItem.gameObject, DefaultCardBookItem.parent);
            cardBookItemObj.name = "CardBookGroup" + bookId;
            cardBookItemObj.SetActive(true);
            var cardBookItem = cardBookItemObj.AddComponent<CardBookItem>();
            cardBookItem.BindCardBookState(book, CurrentThemeState);
            CardBookItemDictionary.Add(bookId, cardBookItem);
        }
        TimeGroup.gameObject.SetActive(CurrentThemeState == CardCollectionModel.Instance.ThemeInUse);
        foreach (var pair in WildCardItemDictionary)
        {
            Destroy(pair.Value.gameObject);
        }
        WildCardItemDictionary.Clear();
        foreach (var pair in CardCollectionModel.Instance.StorageCardCollection.WildCards)
        {
            if (pair.Value > 0)
            {
                var wildCardConfig = CardCollectionModel.Instance.TableCardWildCard[pair.Key];
                var wildCardItemObj = Instantiate(DefaultWildCardItem.gameObject, DefaultWildCardItem.parent);
                wildCardItemObj.name = "WildCard" + wildCardConfig.Id;
                wildCardItemObj.SetActive(true);
                var cardBookItem = wildCardItemObj.AddComponent<WildCardItem>();
                cardBookItem.BindWildCardState(wildCardConfig);
                WildCardItemDictionary.Add(wildCardConfig.Id,cardBookItem);
                cardBookItem.BindCardViewController(WildCardViewController);
            }
        }
        foreach (var rewardItem in RewardItemList)
        {
            Destroy(rewardItem.gameObject);
        }
        RewardItemList.Clear();
        foreach (var reward in CurrentThemeState.CompletedReward)
        {
            var rewardItem = Instantiate(DefaultRewardItem.gameObject, DefaultRewardItem.parent)
                .AddComponent<CommonRewardItem>();
            rewardItem.gameObject.SetActive(true);
            rewardItem.Init(reward);
            RewardItemList.Add(rewardItem);
        }
        NameText.SetTerm(CurrentThemeState.NameKey);
        UpdateCardBookCompletedProgress();
    }

    int CardBookMaxCount => CurrentThemeState.CardBookStateList.Count;
    private int CardBookCompletedCount;

    public void UpdateCardBookCompletedProgress()
    {
        CompletedFlag.gameObject.SetActive(CurrentThemeState.IsCompleted);
        var inUseCardBookConfig = CurrentThemeState.CardBookStateList;
        CardBookCompletedCount = 0;
        foreach (var pair in inUseCardBookConfig)
        {
            if (pair.Value.IsCompleted)
                CardBookCompletedCount++;
        }

        CardBookCompletedProgress.SetText(CardBookCompletedCount + "/" + CardBookMaxCount);
        CardBookCompletedSlider.value = (float) CardBookCompletedCount / CardBookMaxCount;
        CardBookCompletedSlider.gameObject.SetActive(!CurrentThemeState.IsCompleted);
        for (var i = 0; i < RewardItemList.Count; i++)
        {
            RewardItemList[i].gameObject.SetActive(!CurrentThemeState.IsCompleted);
        }
    }

    #region Event

    public void AddAllEvent()
    {
        EventDispatcher.Instance.AddEvent<EventCardBookComplete>(OnCardBookComplete);
        EventDispatcher.Instance.AddEvent<EventCardThemeComplete>(OnCardThemeComplete);
    }

    public void RemoveAllEvent()
    {
        EventDispatcher.Instance.RemoveEvent<EventCardBookComplete>(OnCardBookComplete);
        EventDispatcher.Instance.RemoveEvent<EventCardThemeComplete>(OnCardThemeComplete);
    }

    public void OnCardBookComplete(EventCardBookComplete evt)
    {
        if (!evt.CardBookState.CardThemeStateList.Contains(CurrentThemeState))
            return;
        UpdateCardBookCompletedProgress();
    }

    public void OnCardThemeComplete(EventCardThemeComplete evt)
    {
        if (evt.CardThemeState != CurrentThemeState)
            return;
        UpdateCardBookCompletedProgress();
    }

    #endregion

    #region CloseButton

    public void OnClickCloseButton()
    {
        AnimCloseWindow();
    }

    #endregion

    public void OnClickHelpButton()
    {
        UICardHelpController.Open(CurrentThemeState);
    }

    public void OnClickTradeButton()
    {
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventGalleryStarchestShow);
        UICardGiftController.Open();
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        if (objs.Length > 0)
        {
            CurrentThemeState = objs[0] as CardCollectionCardThemeState;
        }
        else
        {
            CurrentThemeState = CardCollectionModel.Instance.GetCardThemeState(1);
        }
        InitView();
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.CardClickCardBook))
        {
            {
                List<Transform> topLayer = new List<Transform>();
                CardBookItem cardBookItem = null;
                foreach (var bookPair in CurrentThemeState.CardBookStateList)
                {
                    foreach (var itemPair in bookPair.Value.CardItemStateList)
                    {
                        if (itemPair.Value.CollectCount > 0 &&
                            itemPair.Value.CardItemConfig.Level == 2)
                        {
                            cardBookItem = CardBookItemDictionary[bookPair.Key];
                            break;
                        }
                    }
                    if (cardBookItem != null)
                        break;
                }

                if (cardBookItem != null)
                {
                    topLayer.Add(cardBookItem.transform);
                    GuideSubSystem.Instance.RegisterTarget(GuideTargetType.CardClickCardBook,
                        cardBookItem.transform as RectTransform, topLayer: topLayer);   
                }
            }
            GuideSubSystem.Instance.Trigger(GuideTriggerPosition.CardClickCardBook, null);
            {
                List<Transform> topLayer = new List<Transform>();
                var rewardGroup = transform.Find("Root/RewardGroup");
                topLayer.Add(transform.Find("Root/BG/Image (2)"));
                topLayer.Add(CardBookCompletedSlider.transform);
                topLayer.Add(rewardGroup);
                GuideSubSystem.Instance.RegisterTarget(GuideTargetType.CardThemeReward,
                    rewardGroup as RectTransform, topLayer: topLayer);
            }
        }
    }

    public static UICardMainController Instance;
    public static void Open(CardCollectionCardThemeState themeState)
    {
        var uiPath = themeState.GetCardUIName(CardUIName.UIType.UICardMain);
        Instance = UIManager.Instance.OpenUI( uiPath,themeState) as UICardMainController;
    }
}