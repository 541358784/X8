using System;
using System.Collections.Generic;
using Decoration;
using DG.Tweening;
using DragonPlus;
using DragonPlus.UI;
using DragonU3DSDK.Storage;
using Farm.Model;
using UnityEngine;
using UnityEngine.UI;

public partial class UIMonopolyShopController:UIWindowController
{
    private LocalizeTextMeshProUGUI TimeText;
    private StorageMonopoly Storage;
    private Button CloseBtn;
    private LocalizeTextMeshProUGUI ScoreText;
    private Transform DefaultLevelGroup;
    private List<StoreItemLevel> LevelList = new List<StoreItemLevel>();
    private LeaderBoardLevel LeaderBoardLevelItem;
    private RectTransform Content;
    private ScrollRect ScrollView;
    private Button PreViewBtn;
    public override void PrivateAwake()
    {
        TimeText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        InvokeRepeating("UpdateTime",0f,01f);
        CloseBtn = GetItem<Button>("Root/ButtonClose");
        CloseBtn.onClick.AddListener(OnClickCloseBtn);
        ScoreText = GetItem<LocalizeTextMeshProUGUI>("Root/NumGroup/Text");
        EventDispatcher.Instance.AddEvent<EventMonopolyScoreChange>(OnScoreChange);
        Content = transform.Find("Root/Scroll View/Viewport/Content").GetComponent<RectTransform>();
        ScrollView = transform.Find("Root/Scroll View").GetComponent<ScrollRect>();
        DefaultLevelGroup = transform.Find("Root/Scroll View/Viewport/Content/TagGroup");
        DefaultLevelGroup.gameObject.SetActive(false);
        LeaderBoardLevelItem = transform.Find("Root/Scroll View/Viewport/Content/RankGroup").gameObject.AddComponent<LeaderBoardLevel>();
        EventDispatcher.Instance.AddEvent<EventMonopolyBuyStoreItem>(OnBuyStoreItem);
        
        
        PreViewBtn = GetItem<Button>("Root/PreviewButton");
        PreViewBtn.onClick.AddListener(() =>
        {
            // GuideSubSystem.Instance.FinishCurrent(GuideTargetType.MonopolyPreview);
            var mainUI = UIManager.Instance.GetOpenedUIByPath<UIMonopolyMainController>(UINameConst.UIMonopolyMain);
            if (mainUI)
                mainUI.AnimCloseWindow();
            AnimCloseWindow(() =>
            {
                var rewards = new List<int>();
                foreach (var pair in MonopolyModel.Instance.StoreItemConfig)
                {
                    var config = pair.Value;
                    var type = (MonopolyStoreItemType)config.Type;
                    if (type == MonopolyStoreItemType.BuildItem)
                    {
                        rewards.Add(config.RewardId[0]);
                    }
                }
                for (var i = 0; i < rewards.Count; i++)
                {
                    if (rewards[i] == MonopolyModel.Instance.GlobalConfig.CenterDecoItem)
                    {
                        rewards.RemoveAt(i);
                        rewards.Insert(0,MonopolyModel.Instance.GlobalConfig.CenterDecoItem);
                        break;
                    }
                }
                Action callback = ()=>
                {
                    UIMonopolyMainController.Open(Storage);
                    UIMonopolyShopController.Open(Storage);
                };
                if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Game|| FarmModel.Instance.IsFarmModel())
                {
                    SceneFsm.mInstance.ChangeState(StatusType.Transition, StatusType.BackHome,
                        DecoOperationType.Preview, rewards,callback);
                }
                else
                {
                    EventDispatcher.Instance.DispatchEventImmediately(EventEnum.NODE_PREVIEW, rewards,callback);
                }
            });
        });
        PreViewBtn.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEvent<EventMonopolyScoreChange>(OnScoreChange);
        EventDispatcher.Instance.RemoveEvent<EventMonopolyBuyStoreItem>(OnBuyStoreItem);
    }

    public void OnBuyStoreItem(EventMonopolyBuyStoreItem evt)
    {
        CheckPerformUnLockStoreLevel();
    }

    public void OnScoreChange(EventMonopolyScoreChange evt)
    {
        UpdateScoreText();
    }
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        Storage = objs[0] as StorageMonopoly;
        UpdateScoreText();
        var storeLevelConfigList = MonopolyModel.Instance.StoreLevelConfig;
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
        var storeLevelConfigList = MonopolyModel.Instance.StoreLevelConfig;
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
    public static UIMonopolyShopController Instance;
    public static UIMonopolyShopController Open(StorageMonopoly storageMonopoly)
    {
        Instance = UIManager.Instance.OpenUI(UINameConst.UIMonopolyShop, storageMonopoly) as
            UIMonopolyShopController;
        return Instance;
    }
}