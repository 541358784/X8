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

public partial class UIThemeDecorationShopController:UIWindowController
{
    private LocalizeTextMeshProUGUI TimeText;
    private StorageThemeDecoration Storage;
    private Button CloseBtn;
    private LocalizeTextMeshProUGUI ScoreText;
    private Transform DefaultLevelGroup;
    private List<StoreItemLevel> LevelList = new List<StoreItemLevel>();
    private RectTransform Content;
    private ScrollRect ScrollView;
    private Button ButtonHelp;
    private Button ButtonPlay;
    private Button PreViewBtn;
    public override void PrivateAwake()
    {
        TimeText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        InvokeRepeating("UpdateTime",0f,01f);
        CloseBtn = GetItem<Button>("Root/ButtonClose");
        CloseBtn.onClick.AddListener(OnClickCloseBtn);
        ScoreText = GetItem<LocalizeTextMeshProUGUI>("Root/NumGroup/Text");
        EventDispatcher.Instance.AddEvent<EventThemeDecorationScoreChange>(OnScoreChange);
        Content = transform.Find("Root/Scroll View/Viewport/Content").GetComponent<RectTransform>();
        ScrollView = transform.Find("Root/Scroll View").GetComponent<ScrollRect>();
        DefaultLevelGroup = transform.Find("Root/Scroll View/Viewport/Content/TagGroup");
        DefaultLevelGroup.gameObject.SetActive(false);
        ButtonHelp = GetItem<Button>("Root/ButtonHelp");
        ButtonHelp.onClick.AddListener(() =>
        {
            UIThemeDecorationHelpController.Open(Storage);
        });
        ButtonPlay = GetItem<Button>("Root/PlayButton");
        ButtonPlay.onClick.AddListener(() =>
        {
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.ThemeDecorationPlay);
            AnimCloseWindow(() =>
            {
                if (SceneFsm.mInstance.GetCurrSceneType() != StatusType.Game)
                {
                    SceneFsm.mInstance.TransitionGame();
                }
            });
        });
        PreViewBtn = GetItem<Button>("Root/PreviewButton");
        PreViewBtn.onClick.AddListener(() =>
        {
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.ThemeDecorationPreview);
            AnimCloseWindow(() =>
            {
                var rewards = new List<int>();
                foreach (var pair in ThemeDecorationModel.Instance.StoreItemConfig)
                {
                    var config = pair.Value;
                    var type = (ThemeDecorationStoreItemType)config.Type;
                    if (type == ThemeDecorationStoreItemType.BuildItem)
                    {
                        rewards.Add(config.RewardId[0]);
                    }
                }
                for (var i = 0; i < rewards.Count; i++)
                {
                    if (rewards[i] == ThemeDecorationModel.Instance.GlobalConfig.CenterDecoItem)
                    {
                        rewards.RemoveAt(i);
                        rewards.Insert(0,ThemeDecorationModel.Instance.GlobalConfig.CenterDecoItem);
                        break;
                    }
                }
                if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Game|| FarmModel.Instance.IsFarmModel())
                {
                    SceneFsm.mInstance.ChangeState(StatusType.Transition, StatusType.BackHome,
                        DecoOperationType.Preview, rewards);
                }
                else
                {
                    EventDispatcher.Instance.DispatchEventImmediately(EventEnum.NODE_PREVIEW, rewards);
                }
            });
        });
        EventDispatcher.Instance.AddEvent<EventThemeDecorationBuyStoreItem>(OnBuyStoreItem);
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEvent<EventThemeDecorationScoreChange>(OnScoreChange);
        EventDispatcher.Instance.RemoveEvent<EventThemeDecorationBuyStoreItem>(OnBuyStoreItem);
    }

    public void OnBuyStoreItem(EventThemeDecorationBuyStoreItem evt)
    {
        ThemeDecorationStoreLevelConfig completeLevel = null;
        for (var i = 0; i < ThemeDecorationModel.Instance.StoreLevelConfig.Count; i++)
        {
            var levelConfig = ThemeDecorationModel.Instance.StoreLevelConfig[i];
            if (Storage.CanCollectLevelCompleteReward(levelConfig))
            {
                completeLevel = levelConfig;
                break;
            }
        }
        if (completeLevel == null)
        {
            CheckPerformUnLockStoreLevel();   
        }
        else
        {
            Storage.CollectLevelCompleteReward(completeLevel,CheckPerformUnLockStoreLevel);
        }
    }

    public void OnScoreChange(EventThemeDecorationScoreChange evt)
    {
        UpdateScoreText();
    }
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        Storage = objs[0] as StorageThemeDecoration;
        UpdateScoreText();
        var storeLevelConfigList = ThemeDecorationModel.Instance.StoreLevelConfig;
        for (var i = 0; i < storeLevelConfigList.Count; i++)
        {
            var levelItem = Instantiate(DefaultLevelGroup, DefaultLevelGroup.parent).gameObject
                .AddComponent<StoreItemLevel>();
            levelItem.gameObject.SetActive(true);
            levelItem.InitStoreItemLevel(Storage,storeLevelConfigList[i]);
            LevelList.Add(levelItem);
        }
        ScrollView.enabled = false;
        LayoutRebuilder.ForceRebuildLayoutImmediate(DefaultLevelGroup.parent as RectTransform);
        SetContentPosition();
        XUtility.WaitSeconds(0.3f, () =>
        {
            ScrollView.enabled = true;
            // CheckPerformUnLockStoreLevel();
        });
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.ThemeDecorationInfo1))
        {
            if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.ThemeDecorationInfo1, null))
            {
                List<Transform> topLayer = new List<Transform>();
                topLayer.Add(PreViewBtn.transform);
                GuideSubSystem.Instance.RegisterTarget(GuideTargetType.ThemeDecorationPreview, PreViewBtn.transform as RectTransform,
                    topLayer: topLayer);       
            }
        }
        else if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.ThemeDecorationPlay))
        {
            List<Transform> topLayer = new List<Transform>();
            topLayer.Add(ButtonPlay.transform);
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.ThemeDecorationPlay, ButtonPlay.transform as RectTransform,
                topLayer: topLayer);
            if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.ThemeDecorationPlay, null))
            {
            }
        }
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
    private float UnitBaseHeight = 78;
    private float UnitRowHeight = 243;
    // private float RankGroupHeight = 415;
    private float MaxContentY
    {
        get
        {
            var height = GetContentAnchorPosY(999);
            // height += RankGroupHeight*Content.localScale.y;
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
        var storeLevelConfigList = ThemeDecorationModel.Instance.StoreLevelConfig;
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
        TimeText.SetText(Storage.GetTotalLeftTimeText());
        if (Storage.IsTotalTimeOut())
            CloseWindowWithinUIMgr();
    }
    public static UIThemeDecorationShopController Open(StorageThemeDecoration storageThemeDecoration)
    {
        return UIManager.Instance.OpenUI(storageThemeDecoration.GetAssetPathWithSkinName(UINameConst.UIThemeDecorationShop), storageThemeDecoration) as
            UIThemeDecorationShopController;
    }
}