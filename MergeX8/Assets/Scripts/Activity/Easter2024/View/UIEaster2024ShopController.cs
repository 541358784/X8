using System;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using DragonPlus.UI;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

public partial class UIEaster2024ShopController:UIWindowController
{
    private LocalizeTextMeshProUGUI TimeText;
    private StorageEaster2024 Storage;
    private Button CloseBtn;
    private LocalizeTextMeshProUGUI ScoreText;
    private Transform DefaultLevelGroup;
    private List<StoreItemLevel> LevelList = new List<StoreItemLevel>();
    private LeaderBoardLevel LeaderBoardLevelItem;
    private RectTransform Content;
    private ScrollRect ScrollView;
    public override void PrivateAwake()
    {
        TimeText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        InvokeRepeating("UpdateTime",0f,01f);
        CloseBtn = GetItem<Button>("Root/ButtonClose");
        CloseBtn.onClick.AddListener(OnClickCloseBtn);
        ScoreText = GetItem<LocalizeTextMeshProUGUI>("Root/NumGroup/Text");
        EventDispatcher.Instance.AddEvent<EventEaster2024ScoreChange>(OnScoreChange);
        Content = transform.Find("Root/Scroll View/Viewport/Content").GetComponent<RectTransform>();
        ScrollView = transform.Find("Root/Scroll View").GetComponent<ScrollRect>();
        DefaultLevelGroup = transform.Find("Root/Scroll View/Viewport/Content/TagGroup");
        DefaultLevelGroup.gameObject.SetActive(false);
        LeaderBoardLevelItem = transform.Find("Root/Scroll View/Viewport/Content/RankGroup").gameObject.AddComponent<LeaderBoardLevel>();
        EventDispatcher.Instance.AddEvent<EventEaster2024BuyStoreItem>(OnBuyStoreItem);
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEvent<EventEaster2024ScoreChange>(OnScoreChange);
        EventDispatcher.Instance.RemoveEvent<EventEaster2024BuyStoreItem>(OnBuyStoreItem);
    }

    public void OnBuyStoreItem(EventEaster2024BuyStoreItem evt)
    {
        CheckPerformUnLockStoreLevel();
    }

    public void OnScoreChange(EventEaster2024ScoreChange evt)
    {
        UpdateScoreText();
    }
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        Storage = objs[0] as StorageEaster2024;
        UpdateScoreText();
        var storeLevelConfigList = Easter2024Model.Instance.StoreLevelConfig;
        for (var i = 0; i < storeLevelConfigList.Count; i++)
        {
            var levelItem = Instantiate(DefaultLevelGroup, DefaultLevelGroup.parent).gameObject
                .AddComponent<StoreItemLevel>();
            levelItem.gameObject.SetActive(true);
            levelItem.InitStoreItemLevel(Storage,storeLevelConfigList[i]);
            LevelList.Add(levelItem);
        }
        LeaderBoardLevelItem.InitLeaderBoardLevel(Storage);
        LeaderBoardLevelItem.transform.SetAsLastSibling();
        ScrollView.enabled = false;
        LayoutRebuilder.ForceRebuildLayoutImmediate(DefaultLevelGroup.parent as RectTransform);
        SetContentPosition();
        XUtility.WaitSeconds(0.3f, () =>
        {
            ScrollView.enabled = true;
            CheckPerformUnLockStoreLevel();
        });
    }

    public void CheckPerformUnLockStoreLevel()
    {
        var curStoreLevelConfig = Storage.GetCurStoreLevel();
        if (!Storage.UnLockStoreLevel.Contains(curStoreLevelConfig.Id))
        {
            ScrollView.enabled = false;
            var targetPosY = GetContentAnchorPosY(curStoreLevelConfig.Id);
            targetPosY = Math.Min(MaxContentY, targetPosY);
            targetPosY = Math.Max(MinContentY, targetPosY);
            Content.DOAnchorPosY(targetPosY, 0.5f).OnComplete(() =>
            {
                Storage.UnLockStoreLevel.Add(curStoreLevelConfig.Id);
                LevelList[curStoreLevelConfig.Id - 1].PerformUnlock(() =>
                {
                    ScrollView.enabled = true;
                });
            });   
        }
    }
    private float UnitBaseHeight = 80;
    private float UnitRowHeight = 265;
    private float RankGroupHeight = 415;
    private float MaxContentY
    {
        get
        {
            var height = GetContentAnchorPosY(999);
            height += RankGroupHeight*Content.localScale.y;
            var viewHeight = (Content.parent as RectTransform).rect.height;
            return height - viewHeight;
        }
    }

    private float MinContentY = 0;
    public void SetContentPosition()
    {
        var curStoreLevelConfig = Storage.GetCurStoreLevel();
        var targetPosY = GetContentAnchorPosY(curStoreLevelConfig.Id);
        if (!Storage.UnLockStoreLevel.Contains(curStoreLevelConfig.Id))
        {
            targetPosY = GetContentAnchorPosY(curStoreLevelConfig.Id-1);
        }
        targetPosY = Math.Min(MaxContentY, targetPosY);
        var tempPos = Content.anchoredPosition;
        tempPos.y = targetPosY;
        Content.anchoredPosition = tempPos;
    }

    public float GetContentAnchorPosY(int storeLevelId)
    {
        var storeLevelConfigList = Easter2024Model.Instance.StoreLevelConfig;
        var anchorPosY = 0f;
        for (var i = 0; i < storeLevelConfigList.Count; i++)
        {
            var storeLevelConfig = storeLevelConfigList[i];
            if (storeLevelConfig.Id < storeLevelId)
            {
                var rowCount = (storeLevelConfig.StoreItemList.Count-1) / 3 + 1;
                anchorPosY += UnitBaseHeight + rowCount * UnitRowHeight;
            }
            else
            {
                break;
            }
        }
        anchorPosY *= Content.localScale.y;
        return anchorPosY;
    }

    public void UpdateScoreText()
    {
        ScoreText.SetText(Storage.Score.ToString());
    }

    public void OnClickCloseBtn()
    {
        AnimCloseWindow();
    }

    public void UpdateTime()
    {
        TimeText.SetText(Storage.GetLeftTimeText());
    }
    public static UIEaster2024ShopController Open(StorageEaster2024 storageEaster2024)
    {
        return UIManager.Instance.OpenUI(UINameConst.UIEaster2024Shop, storageEaster2024) as
            UIEaster2024ShopController;
    }
}