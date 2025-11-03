using System;
using System.Collections.Generic;
using ActivityLocal.CardCollection.Home;
using DG.Tweening;
using DragonPlus;
using Gameplay;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public partial class UICardBookController:UIWindowController
{
    public class PageScrollRect : ScrollRect
    {
        public UICardBookController Controller;

        public void Copy(ScrollRect scrollRect)
        {
            content = scrollRect.content;
            horizontal = scrollRect.horizontal;
            vertical = scrollRect.vertical;
            movementType = scrollRect.movementType;
            elasticity = scrollRect.elasticity;
            inertia = scrollRect.inertia;
            decelerationRate = scrollRect.decelerationRate;
            scrollSensitivity = scrollRect.scrollSensitivity;
            viewport = scrollRect.viewport;
            horizontalScrollbar = scrollRect.horizontalScrollbar;
            verticalScrollbar = scrollRect.verticalScrollbar;
        }

        public void BindController(UICardBookController controller)
        {
            Controller = controller;
        }

        private int BeginPageNum;
        public override void OnBeginDrag(PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);
            Controller.ScrollViewContent.DOKill();
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);
            velocity = Vector2.zero;
            var curPageNum = Controller.GetPageNum();
            if (curPageNum == Controller.CurrentCardBookIndex)
            {
                var limitDeltaX = 1f;
                var deltaX = eventData.delta.x;
                var curPageCenterX = curPageNum*PageWidth;
                var curContentX = -Controller.ScrollViewContent.anchoredPosition.x;
                if (curContentX > curPageCenterX && deltaX < -limitDeltaX)
                {
                    curPageNum++;
                }
                else if (curContentX < curPageCenterX && deltaX > limitDeltaX)
                {
                    curPageNum--;
                }
            }
            Controller.UpdateContentPosition(curPageNum,0.1f);
        }
    }
    private int CurrentCardBookIndex;
    private CardCollectionCardThemeState CardThemeState;
    private PageScrollRect ScrollView;
    private RectTransform ScrollViewContent;
    private Transform DefaultCardBookPageItem;
    private List<CardBookPage> CardBookPageList = new List<CardBookPage>();
    private CardItemView CardViewController;
    private LocalizeTextMeshProUGUI PageNumText;
    private Button LeftBtn;
    private Button RightBtn;
    private Button HelpBtn;
    private Button BackBtn;
    private Button CloseButton;
    private const int PageWidth = 700;
    private UICardMainController.WildCardView WildCardViewController;
    private Transform DefaultWildCardItem;
    private Dictionary<int, UICardMainController.WildCardItem> WildCardItemDictionary = new Dictionary<int, UICardMainController.WildCardItem>();
    public override void PrivateAwake()
    {
        LeftBtn = GetItem<Button>("Root/LeftButton");
        LeftBtn.onClick.AddListener(OnClickLeftBtn);
        RightBtn = GetItem<Button>("Root/RightButton");
        RightBtn.onClick.AddListener(OnClickRightBtn);
        HelpBtn = GetItem<Button>("Root/ButtonHelp");
        HelpBtn.onClick.AddListener(OnClickHelpBtn);
        BackBtn = GetItem<Button>("Root/ButtonBack");
        BackBtn.gameObject.SetActive(false);
        // BackBtn.onClick.AddListener(OnClickBackBtn);
        CloseButton = GetItem<Button>("Root/ButtonClose");
        CloseButton.onClick.AddListener(OnClickCloseButton);
        var scrollObj = transform.Find("Root/Scroll View").gameObject;
        var scrollRect = scrollObj.GetComponent<ScrollRect>();
        scrollRect.decelerationRate = 0;
        var tempObj = new GameObject("Temp");
        tempObj.transform.SetParent(scrollObj.transform);
        var tempScrollRect = tempObj.AddComponent<PageScrollRect>();
        tempScrollRect.Copy(scrollRect);
        DestroyImmediate(scrollRect);
        ScrollView = scrollObj.AddComponent<PageScrollRect>();
        ScrollView.Copy(tempScrollRect);
        DestroyImmediate(tempObj);
        ScrollView.BindController(this);
        ScrollView.onValueChanged.AddListener(UpdateViewState);
        DefaultCardBookPageItem = transform.Find("Root/Scroll View/Viewport/Content/Page");
        DefaultCardBookPageItem.gameObject.SetActive(false);
        ScrollViewContent = DefaultCardBookPageItem.parent as RectTransform;
        CardViewController = transform.Find("Root/UICardView").gameObject.AddComponent<CardItemView>();
        CardViewController.gameObject.SetActive(true);
        CardViewController.gameObject.SetActive(false);
        PageNumText = transform.Find("Root/PageNum").GetComponent<LocalizeTextMeshProUGUI>();
        _animator.enabled = false;
        WildCardViewController = transform.Find("Root/UIWildCardView").gameObject.AddComponent<UICardMainController.WildCardView>();
        WildCardViewController.gameObject.SetActive(true);
        WildCardViewController.gameObject.SetActive(false);
        transform.Find("Root/UniversalCardGroup").gameObject.SetActive(true);
        DefaultWildCardItem = transform.Find("Root/UniversalCardGroup/UniversalCard");
        DefaultWildCardItem.gameObject.SetActive(false);
    }
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        if (LeftBtn.gameObject.GetComponent<ShieldButtonOnClick>() != null)
        {
            LeftBtn.gameObject.GetComponent<ShieldButtonOnClick>().isUse = false;   
        }
        if (RightBtn.gameObject.GetComponent<ShieldButtonOnClick>() != null)
        {
            RightBtn.gameObject.GetComponent<ShieldButtonOnClick>().isUse = false;   
        }
        var cardBookState = objs[0] as CardCollectionCardBookState;
        CardThemeState = cardBookState.CardThemeStateList[0];
        var currentCardBookIndex = 0;
        foreach (var pair in CardThemeState.CardBookStateList)
        {
            if (pair.Value != cardBookState)
                currentCardBookIndex++;
            else
                break;
        }
        
        // foreach (var page in CardBookPageList)
        //     Destroy(page.gameObject);
        // CardBookPageList.Clear();
        for (var i = CardBookPageList.Count; i < CardThemeState.CardBookStateList.Count; i++)
        {
            var cardBookPageObj = Instantiate(DefaultCardBookPageItem.gameObject, DefaultCardBookPageItem.parent);
            cardBookPageObj.name = "CardBookPage" + CardBookPageList.Count;
            cardBookPageObj.SetActive(true);
            var cardBookPage = cardBookPageObj.AddComponent<CardBookPage>();
            cardBookPage.BindCardViewController(CardViewController);
            CardBookPageList.Add(cardBookPage);
        }

        for (var i = CardThemeState.CardBookStateList.Count; i < CardBookPageList.Count; i++)
        {
            CardBookPageList[i].gameObject.SetActive(false);
        }

        var pageIndex = 0;
        foreach (var pair in CardThemeState.CardBookStateList)
        {
            var bookState = pair.Value;
            var cardBookPage = CardBookPageList[pageIndex];
            pageIndex++;
            cardBookPage.gameObject.SetActive(true);
            // cardBookPage.BindCardViewController(CardViewController);
            cardBookPage.BindCardBookState(bookState);
        }
        UpdateContentPosition(currentCardBookIndex);
        
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
                var cardBookItem = wildCardItemObj.AddComponent<UICardMainController.WildCardItem>();
                cardBookItem.BindWildCardState(wildCardConfig);
                WildCardItemDictionary.Add(wildCardConfig.Id,cardBookItem);
                cardBookItem.BindCardViewController(WildCardViewController);
            }
        }
        
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.CardCardBookReward))
        {
            {
                List<Transform> topLayer = new List<Transform>();
                var rewardGroup = CardBookPageList[CurrentCardBookIndex].TopGroup.transform;
                topLayer.Add(transform.Find("Root/BG/Image (2)"));
                topLayer.Add(rewardGroup);
                GuideSubSystem.Instance.RegisterTarget(GuideTargetType.CardCardBookReward, rewardGroup as RectTransform,
                    topLayer: topLayer);
            }
            if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.CardCardBookReward, null))
            {
                List<Transform> topLayer = new List<Transform>();
                var backBtn = CloseButton.transform;
                topLayer.Add(backBtn);
                GuideSubSystem.Instance.RegisterTarget(GuideTargetType.CardReturnMain, backBtn as RectTransform,
                    topLayer: topLayer);
            }
        }
        
        _animator.enabled = true;
        
    }

    public int GetPageNum()
    {
        var pageNumF = -ScrollViewContent.anchoredPosition.x / PageWidth;
        var pageNum = Mathf.FloorToInt(pageNumF + 0.5f);
        return pageNum;
    }
    public void UpdateViewState(Vector2 pos)
    {
        PageNumText.SetText((GetPageNum()+1).ToString());
    }

    public void UpdateContentPosition(int targetIndex,float time=0f)
    {
        if (targetIndex < 0)
        {
            targetIndex = 0;
        }
        if (targetIndex >= CardThemeState.CardBookStateList.Count)
        {
            targetIndex = CardThemeState.CardBookStateList.Count - 1;
        }
        ScrollViewContent.DOKill();
        CurrentCardBookIndex = targetIndex;
        // CardBookPageList[CurrentCardBookIndex].OnViewPage();
        LeftBtn.gameObject.SetActive(CurrentCardBookIndex > 0);
        RightBtn.gameObject.SetActive(CurrentCardBookIndex < CardThemeState.CardBookStateList.Count-1);
        if (time == 0f)
        {
            ScrollViewContent.anchoredPosition = new Vector2(-PageWidth * targetIndex, 0);
            UpdateViewState(Vector2.zero);
        }
        else
        {
            // ScrollView.enabled = false;
            ScrollViewContent.DOAnchorPosX(-PageWidth * targetIndex, time).SetEase(Ease.Linear)
                .OnUpdate(()=>ScrollView.velocity = Vector2.zero);
            // .OnComplete(() => ScrollView.enabled = true);
        }
    }
    public void OnClickLeftBtn()
    {
        UpdateContentPosition(CurrentCardBookIndex-1,0.1f);
    }
    public void OnClickRightBtn()
    {
        UpdateContentPosition(CurrentCardBookIndex+1,0.1f);
    }
    public void OnClickHelpBtn()
    {
        UICardHelpController.Open(CardThemeState);
    }
    // public void OnClickBackBtn()
    // {
    //     GuideSubSystem.Instance.FinishCurrent(GuideTargetType.CardReturnMain);
    //     CloseWindowWithinUIMgr(false);
    //     // AnimCloseWindow();
    // }
    public void OnClickCloseButton()
    {
        // AnimCloseWindow(null,false);
        // var mainPopup = UIManager.Instance.GetOpenedUIByPath<UICardMainController>(UINameConst.UICardMain);
        // if (mainPopup)
        //     mainPopup.CloseWindowWithinUIMgr(true);
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.CardReturnMain);
        CloseWindowWithinUIMgr(false);
    }
    public static void Open(CardCollectionCardBookState cardBookState)
    {
        var uiPath = cardBookState.CardThemeStateList[0].GetCardUIName(CardUIName.UIType.UICardBook);
        if (!UIManager.Instance.GetOpenedUIByPath(uiPath))
            UIManager.Instance.OpenUI( uiPath,cardBookState);
    }
}