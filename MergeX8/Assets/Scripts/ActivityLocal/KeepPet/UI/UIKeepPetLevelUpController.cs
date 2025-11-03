using System;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public partial class UIKeepPetLevelUpController : UIWindowController
{
    public static UIKeepPetLevelUpController Open(StorageKeepPet storage)
    {
        var openWindow = UIManager.Instance.GetOpenedUIByPath<UIKeepPetLevelUpController>(UINameConst.UIKeepPetLevelUp);
        if (openWindow)
            return openWindow;
        return UIManager.Instance.OpenUI(UINameConst.UIKeepPetLevelUp, storage) as UIKeepPetLevelUpController;
    }
    public override void PrivateAwake()
    {
        CommingSoon = transform.Find("Root/MiddleGroup/Scroll View/Viewport/Content/ComingSoon");
        DefaultLevelItem = transform.Find("Root/MiddleGroup/Scroll View/Viewport/Content/Reward");
        DefaultLevelItem.gameObject.SetActive(false);
        LevelSlider = transform.Find("Root/MiddleGroup/Scroll View/Viewport/Content/Slider").GetComponent<Slider>();
        CloseBtn = transform.Find("Root/TitleGroup/ButtonClose").GetComponent<Button>();
        CloseBtn.onClick.AddListener(()=>
        {
            FinishCloseLevelRewardGuide();
            AnimCloseWindow(() =>
            {
                var mainUI =
                    UIManager.Instance.GetOpenedUIByPath<UIKeepPetMainController>(UINameConst.UIKeepPetMain);
                if (mainUI)
                {
                    mainUI.PlayAllBuildingAppearAnimation();
                    mainUI.CheckDailyTaskEntranceGuide();
                    mainUI.CheckClickSearchBtnGuide();
                }
            });
        });
        LevelText = transform.Find("Root/TitleGroup/Slider/Level/Text").GetComponent<LocalizeTextMeshProUGUI>();
        ExpProgressText = transform.Find("Root/TitleGroup/Slider/Text").GetComponent<LocalizeTextMeshProUGUI>();
        ExpProgressSlider = transform.Find("Root/TitleGroup/Slider").GetComponent<Slider>();
        DogSpine = transform.Find("Root/TitleGroup/BG/SkeletonGraphic (MX8_dog)").GetComponent<SkeletonGraphic>();
        EventDispatcher.Instance.AddEvent<EventKeepPetChangeSkin>(OnChangeSkin);
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEvent<EventKeepPetChangeSkin>(OnChangeSkin);
    }

    public SkeletonGraphic DogSpine;
    public StorageKeepPet Storage;
    public KeepPetLevelConfig Level;
    public List<LevelItem> LevelItemList = new List<LevelItem>();
    private Transform DefaultLevelItem;
    private Transform CommingSoon;
    private Slider LevelSlider;
    private Button CloseBtn;
    private LocalizeTextMeshProUGUI LevelText;
    private LocalizeTextMeshProUGUI ExpProgressText;
    private Slider ExpProgressSlider;
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        Storage = objs[0] as StorageKeepPet;
        Level = Storage.Exp.KeepPetGetCurLevelConfig();
        InitViewState();
        var cueLevel = Storage.Exp.KeepPetGetCurLevelConfig();
        for (var i = 1; i <= cueLevel.Id; i++)
        {
            if (!Storage.LevelRewardCollectState.ContainsKey(i))
            {
                ScrollToTargetLevel(i);
                break;
            }
        }
        CheckLevelRewardInfoGuide();
        CheckCollectLevelRewardGuide();
        
        DogSpine.Skeleton.SetSkin(Storage.SkinName);
        DogSpine.Skeleton.SetSlotsToSetupPose();
        DogSpine.AnimationState.Apply(DogSpine.Skeleton);
    }

    public void OnChangeSkin(EventKeepPetChangeSkin evt)
    {
        DogSpine.Skeleton.SetSkin(Storage.SkinName);
        DogSpine.Skeleton.SetSlotsToSetupPose();
        DogSpine.AnimationState.Apply(DogSpine.Skeleton);
    }

    public void ScrollToTargetLevel(int level)
    {
        var scrollView = transform.Find("Root/MiddleGroup/Scroll View").GetComponent<ScrollRect>();
        scrollView.enabled = false;
        var collectLevel = level;
        var levelItem = LevelItemList.Find(a => a.LevelConfig.Id == collectLevel);
        LayoutRebuilder.ForceRebuildLayoutImmediate(levelItem.transform.parent as RectTransform);
        var itemRect = levelItem.transform as RectTransform;
        var content = itemRect.parent as RectTransform;
        var targetY = (-itemRect.anchoredPosition.y - itemRect.rect.height / 2) * content.transform.localScale.y;
        var contentPos = content.anchoredPosition;
        contentPos.y = targetY;
        content.anchoredPosition = contentPos;
        LayoutRebuilder.ForceRebuildLayoutImmediate(levelItem.transform.parent as RectTransform);
        scrollView.transform.DOKill();
        DOVirtual.DelayedCall(0.3f, () =>
        {
            if (scrollView)
                scrollView.enabled = true;
        }).SetTarget(scrollView.transform);
    }

    public void InitViewState()
    {
        foreach (var levelItem in LevelItemList)
        {
            DestroyImmediate(levelItem.gameObject);
        }   
        LevelItemList.Clear();
        for (var i = 0; i < KeepPetModel.Instance.LevelConfig.Count; i++)
        {
            var level = KeepPetModel.Instance.LevelConfig[i];
            if (level.PlaceHolder)
                continue;
            var levelItem = Instantiate(DefaultLevelItem, DefaultLevelItem.parent).gameObject.AddComponent<LevelItem>();
            levelItem.gameObject.SetActive(true);
            levelItem.Init(this,level);
            LevelItemList.Add(levelItem);
        }
        CommingSoon.SetAsLastSibling();
        LevelSlider.maxValue = LevelItemList.Count;
        var nextLevelNeedExp = Level.GetNextLevelNeedExp();
        var curExp = Level.GetCurLevelExp(Storage.Exp);
        var progress = (float) curExp / nextLevelNeedExp;
        LevelSlider.value = Level.Id + progress;
        
        LevelText.SetText(Level.Id.ToString());
        ExpProgressText.SetText(curExp+"/"+nextLevelNeedExp);
        ExpProgressSlider.maxValue = 1;
        ExpProgressSlider.value = progress;
    }

    public void OnClickCollectLevelRewardBtn(KeepPetLevelConfig level)
    {
        FinishCollectLevelRewardGuide(level);
        if (KeepPetModel.Instance.CollectLevelReward(level.Id))
        {
            if (level.RewardId != null && level.RewardId.Count > 0)
            {
                var rewards = CommonUtils.FormatReward(level.RewardId, level.RewardNum);
                var reason = new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason
                    .KeepPetLevelRewardGet);
                CommonRewardManager.Instance.PopCommonReward(rewards, CurrencyGroupManager.Instance.currencyController,
                    false, reason);
            }

            if (level.RewardBuildingItem > 0)
            {
                var rewardItem = LevelItemList.Find(a => a.LevelConfig == level);
                var target = rewardItem.BuildingGroup.NormalBuildingIcon.transform;
                FlyGameObjectManager.Instance.FlyObject(target.gameObject, target.position, CloseBtn.transform.position, true, 0.5f,
                    0f, () =>
                    {
                    });
            }
        }
    }
}