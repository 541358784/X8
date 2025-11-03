using System;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public partial class UIKeepPetMainController:UIWindowController
{
    private Button FeedBtn;
    private Transform FeedBtnRedPoint;
    private LocalizeTextMeshProUGUI FeedBtnRedPointNumText;
    private Transform FeedBtnAddPoint;
    private Button FrisbeeBtn;
    private Transform FrisbeeBtnRedPoint;
    private LocalizeTextMeshProUGUI FrisbeeBtnRedPointNumText;
    private Transform FrisbeeBtnAddPoint;
    private Button SearchTaskBtn;
    private LocalizeTextMeshProUGUI SearchTaskBtnText;
    private Button DailyTaskBtn;
    private Transform DailyTaskBtnRedPoint;
    private SkeletonGraphic DogSpine;
    private Button HelpBtn;
    
    private Button CloseBtn;
    private Button DogButton;
    private Transform SearchingProgressGroup;
    private Slider SearchingProgressSlider;
    private LocalizeTextMeshProUGUI SearchingLeftTimeText;
    private Button QuickFinishSearchBtn;
    private LocalizeTextMeshProUGUI QuickFinishSearchPriceText;
    private Aux_AuntClue ClueBtn;
    private Button SearchFinishGetRewardBtn;
    private Transform SearchingTipBoard;
    private SkeletonGraphic SearchingTipBoardSpine;
    private Button ReceiveBtn;
    private Transform SearchTaskBtnEffect;
    private List<string> IdleSpineStateList = new List<string>() { "idle","idle","idle","idle2","idle","idle","idle2"};
    private int IdleSpineStateIndex = 0;
    private Button AwakeBtn;
    private LocalizeTextMeshProUGUI DrumStickCountText;
    public KeepPetBaseState CurState=>Storage.GetCurState();
    public override void PrivateAwake()
    {
        CommonUtils.NotchAdapte(transform.Find("Root/BG") as RectTransform);
        CommonUtils.NotchAdapte(transform.Find("Root/ButtonClose") as RectTransform);
        CommonUtils.NotchAdapte(transform.Find("Root/ButtonHelp") as RectTransform);
        CommonUtils.NotchAdapte(transform.Find("Root/ButtonLeft") as RectTransform);
        CommonUtils.NotchAdapte(transform.Find("Root/ButtonRight") as RectTransform);
        CommonUtils.NotchAdapte(transform.Find("Root/IconNum") as RectTransform);
        FeedBtn = GetItem<Button>("Root/ButtonBottom/FeedButton");
        FeedBtn.onClick.AddListener(OnClickFeedBtn);
        FeedBtnRedPoint = FeedBtn.transform.Find("Root/Num");
        FeedBtnRedPointNumText = FeedBtnRedPoint.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
        FeedBtnAddPoint = FeedBtn.transform.Find("Root/Add");
        FrisbeeBtn = GetItem<Button>("Root/ButtonBottom/FrisbeeButton");
        FrisbeeBtn.onClick.AddListener(OnClickFrisbeeBtn);
        FrisbeeBtnRedPoint = FrisbeeBtn.transform.Find("Root/Num");
        FrisbeeBtnRedPointNumText = FrisbeeBtnRedPoint.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
        FrisbeeBtnAddPoint = FrisbeeBtn.transform.Find("Root/Add");
        SearchTaskBtn = GetItem<Button>("Root/ButtonBottom/PatrolButton");
        SearchTaskBtn.onClick.AddListener(OnClickSearchTaskBtn);
        SearchTaskBtnText = GetItem<LocalizeTextMeshProUGUI>("Root/ButtonBottom/PatrolButton/Root/Text");
        SearchTaskBtnEffect = transform.Find("Root/ButtonBottom/PatrolButton/FX_star");
        if (SearchTaskBtnEffect)
            SearchTaskBtnEffect.gameObject.SetActive(false);
        DailyTaskBtn = GetItem<Button>("Root/ButtonLeft/TaskButton");
        DailyTaskBtn.onClick.AddListener(OnClickDailyTaskBtn);
        DailyTaskBtnRedPoint = GetItem<Transform>("Root/ButtonLeft/TaskButton/RedPoint");
        InvokeRepeating("UpdateDailyTaskBtnRedPoint", 0f, 1f);
        DogSpine = GetItem<SkeletonGraphic>("Root/DogSpine");
        HelpBtn = GetItem<Button>("Root/ButtonHelp");
        if (HelpBtn)
        {
            HelpBtn.onClick.AddListener(()=>UIKeepPetHelpController.Open());
        }
        
        CloseBtn = GetItem<Button>("Root/ButtonClose");
        CloseBtn.onClick.AddListener(()=>AnimCloseWindow());
        DogButton = GetItem<Button>("Root/DogButton");
        DogButton.onClick.AddListener(OnClickDogBtn);
        SearchingProgressGroup = GetItem<Transform>("Root/Patrol");
        SearchingProgressSlider = GetItem<Slider>("Root/Patrol/Slider");
        SearchingLeftTimeText = GetItem<LocalizeTextMeshProUGUI>("Root/Patrol/Slider/Text");
        InvokeRepeating("UpdateSearchingProgress",0f,1f);
        QuickFinishSearchBtn = GetItem<Button>("Root/Patrol/Button");
        QuickFinishSearchBtn.onClick.AddListener(OnClickQuickFinishSearchBtn);
        QuickFinishSearchPriceText = GetItem<LocalizeTextMeshProUGUI>("Root/Patrol/Button/NumText");
        ClueBtn = transform.Find("Root/ButtonLeft/Clue").gameObject.AddComponent<Aux_AuntClue>();
        SearchFinishGetRewardBtn = GetItem<Button>("Root/ButtonBottom/ReceiveButton");
        if (SearchFinishGetRewardBtn)
            SearchFinishGetRewardBtn.onClick.AddListener(OnClickSearchFinishGetRewardBtn);
        SearchingTipBoard = GetItem<Transform>("Root/PatrolSpine");
        if (SearchingTipBoard)
            SearchingTipBoard.gameObject.SetActive(false);
        SearchingTipBoardSpine = GetItem<SkeletonGraphic>("Root/PatrolSpine/Spine");
        AwakeBtn = GetItem<Button>("Root/ButtonBottom/WakeButton");
        if (AwakeBtn)
            AwakeBtn.onClick.AddListener(OnClickAwakeBtn);
        var searchBtnRedPoint = transform.Find("Root/ButtonBottom/PatrolButton/Root/RedPoint").gameObject.AddComponent<SearchTaskBtnRedPoint>();
        searchBtnRedPoint.Init();
        DrumStickCountText = transform.Find("Root/IconNum/Text").GetComponent<LocalizeTextMeshProUGUI>();
        DrumStickCountText.SetText(KeepPetModel.Instance.GetDogDrumstick().ToString());
        EventDispatcher.Instance.AddEvent<EventKeepPetDogHeadChange>(OnDogHeadChange);
        EventDispatcher.Instance.AddEvent<EventKeepPetStateChange>(OnStateChange);
        EventDispatcher.Instance.AddEvent<EventKeepPetFrisbeeCountChange>(OnPowerPropCountChange);
        EventDispatcher.Instance.AddEvent<EventKeepPetMedicineCountChange>(OnMedicineCountChange);
        EventDispatcher.Instance.AddEvent<EventKeepPetExpChange>(OnExpChange);
        EventDispatcher.Instance.AddEvent<EventKeepPetAwakeStateChange>(OnSleepStateChange);
        EventDispatcher.Instance.AddEvent<EventKeepPetBuildingActiveChange>(OnChangeBuilding);
    }
    
    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEvent<EventKeepPetDogHeadChange>(OnDogHeadChange);
        EventDispatcher.Instance.RemoveEvent<EventKeepPetStateChange>(OnStateChange);
        EventDispatcher.Instance.RemoveEvent<EventKeepPetFrisbeeCountChange>(OnPowerPropCountChange);
        EventDispatcher.Instance.RemoveEvent<EventKeepPetMedicineCountChange>(OnMedicineCountChange);
        EventDispatcher.Instance.RemoveEvent<EventKeepPetExpChange>(OnExpChange);
        EventDispatcher.Instance.RemoveEvent<EventKeepPetAwakeStateChange>(OnSleepStateChange);
        EventDispatcher.Instance.RemoveEvent<EventKeepPetBuildingActiveChange>(OnChangeBuilding);
        StopDogBGM();
        StopLoopHungrySound();
        StopLoopSleepSound();
        DogPlayModel.Instance.CanShowStartGuide();
    }

    public void OnDogHeadChange(EventKeepPetDogHeadChange evt)
    {
        SearchTaskBtnText.SetTermFormats(Math.Min(evt.NewValue,5).ToString(),"5");
    }
    public void UpdateDailyTaskBtnRedPoint()
    {
        DailyTaskBtnRedPoint.gameObject.SetActive(KeepPetModel.Instance.StorageDailyTask.UnCollectRewards.Count > 0);
    }
    public void OnClickAwakeBtn()
    {
        FinishWakeUpGuide();
        KeepPetModel.Instance.AwakeDog();
    }

    public void OnSleepStateChange(EventKeepPetAwakeStateChange evt)
    {
        HideBubble();
        if (evt.NewValue)
        {
            StartLoopSleepSound();
        }
        else
        {
            StopLoopSleepSound();
            if (CurState.Enum == KeepPetStateEnum.Hunger)
            {
                StartLoopHungrySound();
            }
        }
        UpdateViewState(KeepPetModel.Instance.CurState.Enum);
    }

    public void PlayIdleSpine()
    {
        IdleSpineStateIndex = 0;
        PlayNextIdleSpine();
    }

    public void PlayNextIdleSpine()
    {
        var curSpineName = IdleSpineStateList[IdleSpineStateIndex];
        IdleSpineStateIndex++;
        if (IdleSpineStateIndex >= IdleSpineStateList.Count)
        {
            IdleSpineStateIndex = 0;
        }

        if (curSpineName == "idle2")
        {
            PlayIdle2Sound();
        }
        DogSpine.AnimationState.SetAnimation(0, curSpineName, false).Complete += (t) =>
        {
            PlayNextIdleSpine();
        };
    }

    private bool IsPerformEat;
    public async void OnStateChange(EventKeepPetStateChange evt)//狗状态变化
    {
        StopSearchFinishBarkSound();
        if (evt.NewState == KeepPetStateEnum.Hunger)
        {
            StartLoopHungrySound();
        }
        else
        {
            StopLoopHungrySound();
        }
        if (evt.OldState == KeepPetStateEnum.Hunger && evt.NewState == KeepPetStateEnum.Happy)
        {
            IsPerformEat = true;
            HideBubble();
            DogSpine.AnimationState.SetAnimation(0, "hungry2", false).Complete += (t) =>
            {
                PlayEatSound();
                DogSpine.AnimationState.SetAnimation(0, "eat", false).Complete += (t) =>
                {
                    PlayHappySound();
                    DogSpine.AnimationState.SetAnimation(0, "happy", false).Complete += (t1) =>
                    {
                        IsPerformEat = false;
                        UpdateViewState(evt.NewState);
                    };
                };
            };
        }
        else if (evt.NewState == KeepPetStateEnum.Searching)
        {
            UpdateViewState(evt.NewState,false);
            SetHideActive(SearchingProgressGroup.gameObject, false);
            // SearchingProgressGroup.gameObject.SetActive(false);
            
            DogSpine.gameObject.SetActive(true);
            if (SearchingTipBoard)
            {
                SearchingTipBoard.gameObject.SetActive(false);
            }
            HideBubble();
            PlaySearchingSound();
            DogSpine.AnimationState.SetAnimation(0, "mission", false).Complete += (t) =>
            {
                DogSpine.gameObject.SetActive(false);
                if (SearchingTipBoard)
                {
                    SearchingTipBoard.gameObject.SetActive(true);
                    SearchingTipBoardSpine.AnimationState.SetAnimation(0, "in", false).Complete += (t1) =>
                    {
                        SearchingTipBoardSpine.AnimationState.SetAnimation(0, "idle", true);
                    };
                }
                SearchingProgressGroup.gameObject.SetActive(true);
                CheckQuickFinishSearchingGuide();
            };
        }
        else if (evt.NewState == KeepPetStateEnum.SearchFinish)
        {
            UpdateViewState(evt.NewState,false);
            SearchFinishGetRewardBtn.gameObject.SetActive(false);
            if (SearchingTipBoard)
            {
                SearchingTipBoard.gameObject.SetActive(true);
                var boardTextAnimator = GetItem<Animator>("Root/PatrolSpine/Spine/group");
                if (boardTextAnimator)
                    boardTextAnimator.PlayAnimation("disappear");
                SearchingTipBoardSpine.AnimationState.SetAnimation(0, "out", false).Complete += (t1) =>
                {
                    SearchingTipBoard.gameObject.SetActive(false);
                };
            }
            DogSpine.gameObject.SetActive(false);
            XUtility.WaitSeconds(0.2f, () => DogSpine.gameObject.SetActive(true));
            HideBubble();
            IsPerformSearchFinish = true;
            PlaySearchFinishSound();
            DogSpine.AnimationState.SetAnimation(0, "mission2", false).Complete += (t) =>
            {
                IsPerformSearchFinish = false;
                DogSpine.AnimationState.SetAnimation(0, "mission3", true);
                StartSearchFinishBarkSound();
                UpdateViewState(evt.NewState);
            };
        }
        else
            UpdateViewState(evt.NewState);
    }

    private bool IsPerformSearchFinish;
    private bool IsPerformFrisbee;
    public void OnPowerPropCountChange(EventKeepPetFrisbeeCountChange evt)//飞盘数量变化
    {
        var count = evt.NewValue;
        FrisbeeBtnRedPoint.gameObject.SetActive(count>0);
        FrisbeeBtnRedPointNumText.SetText(count < KeepPetModel.Instance.GlobalConfig.MaxFrisbee?count.ToString():"Max");
        FrisbeeBtnAddPoint.gameObject.SetActive(count == 0);
        if (evt.NewValue < evt.OldValue)
        {
            if (!IsPerformFrisbee)
            {
                HideBubble();
                IsPerformFrisbee = true;
                PlayFrisbeeSound();
                DogSpine.AnimationState.SetAnimation(0, "catch", false).Complete += (t) =>
                {
                    // DogSpine.AnimationState.SetAnimation(0, "idle", true);
                    // DogSpine.AnimationState.Update(0);
                    PlayIdleSpine();
                    IsPerformFrisbee = false;
                    GuideFrisbee = false;
                    CheckClickExpBarGuide();
                };
            }
        }
    }
    public void OnMedicineCountChange(EventKeepPetMedicineCountChange evt)//鸡腿数量变化
    {
        var count = evt.NewValue;
        FeedBtnRedPoint.gameObject.SetActive(count>0);
        FeedBtnRedPointNumText.SetText(count.ToString());
        FeedBtnAddPoint.gameObject.SetActive(count == 0);
        DrumStickCountText.SetText(count.ToString());
    }
    

    private StorageKeepPet Storage;
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        PlayDogBGM();
        Storage = objs[0] as StorageKeepPet;
        InitBubble();
        InitLevelProgress();
        InitTreasureMap();
        InitThreeOneGiftBtn();
        InitKeepPetTurkey();
        {
            var count = Storage.FrisbeeCount;
            FrisbeeBtnRedPoint.gameObject.SetActive(count>0);
            FrisbeeBtnRedPointNumText.SetText(count < KeepPetModel.Instance.GlobalConfig.MaxFrisbee?count.ToString():"Max");
            FrisbeeBtnAddPoint.gameObject.SetActive(count == 0);
        }
        {
            var count = Storage.MedicineCount;
            FeedBtnRedPoint.gameObject.SetActive(count>0);
            FeedBtnRedPointNumText.SetText(count.ToString());
            FeedBtnAddPoint.gameObject.SetActive(count == 0);
        }
        InitBuilding();
        UpdateViewState(CurState.Enum);
        if (CurState.Enum == KeepPetStateEnum.Sleep)
        {
            StartLoopSleepSound();
        }
        else if (CurState.Enum == KeepPetStateEnum.Hunger)
        {
            StartLoopHungrySound();
        }
        if (CurState.Enum == KeepPetStateEnum.SearchFinish)
            StartSearchFinishBarkSound();
        ShieldButtonOnClick[] shieldButtons = gameObject.GetComponentsInChildren<ShieldButtonOnClick>(true);
        foreach (var shieldBtn in shieldButtons)
        {
            shieldBtn.isUse = false;
        }
        SearchTaskBtnText.SetTermFormats( Math.Min(Storage.DogHeadCount,5).ToString(),"5");
        // CheckInfoGuide();
        // CheckSteakGuide();
    }

    public void UpdateViewState(KeepPetStateEnum state,bool triggerGuide = true)
    {
        var animationName = "";
        switch (state)
        {
            case KeepPetStateEnum.Happy:
                animationName = "idle";
                break;
            case KeepPetStateEnum.Hunger:
                animationName = "hungry";
                break;
            case KeepPetStateEnum.Searching:
                animationName = "mission";
                break;
            case KeepPetStateEnum.SearchFinish:
                animationName = "mission3";
                break;
            case KeepPetStateEnum.Sleep:
                animationName = "sleep";
                break;
        }
        if (animationName == "idle")
        {
            PlayIdleSpine();
        }
        else
        {
            DogSpine.AnimationState.SetAnimation(0, animationName, true);
            DogSpine.AnimationState.Update(0);   
        }
        DogSpine.gameObject.SetActive(state != KeepPetStateEnum.Searching);
        FeedBtn.gameObject.SetActive(state == KeepPetStateEnum.Hunger);
        FrisbeeBtn.gameObject.SetActive(state == KeepPetStateEnum.Happy);
        var level = Storage.Exp.KeepPetGetCurLevelConfig();
        SearchTaskBtn.gameObject.SetActive(state == KeepPetStateEnum.Happy && level.Id >= KeepPetModel.Instance.GlobalConfig.SearchUnLockLevel);
        SetHideActive(PowerSlider.gameObject,
            state == KeepPetStateEnum.Hunger || state == KeepPetStateEnum.Happy || state == KeepPetStateEnum.Sleep);
        if (SearchingProgressGroup)
            SetHideActive(SearchingProgressGroup.gameObject, state == KeepPetStateEnum.Searching);
        UpdateSearchingProgress();
        LastShowBubbleTime = 0;
        if (SearchFinishGetRewardBtn)
            SearchFinishGetRewardBtn.gameObject.SetActive(state == KeepPetStateEnum.SearchFinish);
        if (SearchingTipBoard)
        {
            SearchingTipBoard.gameObject.SetActive(state == KeepPetStateEnum.Searching);
            if (SearchingTipBoard.gameObject.activeSelf)
                SearchingTipBoardSpine.AnimationState.SetAnimation(0, "idle", true);
        }
        if (AwakeBtn)
            AwakeBtn.gameObject.SetActive(state == KeepPetStateEnum.Sleep);
        if (!triggerGuide)
            return;
        if (state == KeepPetStateEnum.Sleep)
            CheckWakeUpGuide();
        if (state == KeepPetStateEnum.Searching)
            CheckQuickFinishSearchingGuide();
        if (state == KeepPetStateEnum.SearchFinish)
            CheckSearchFinishGetRewardGuide();
        if (state == KeepPetStateEnum.Hunger && Storage.MedicineCount <= 0)
            CheckFeedDrumstick1Guide();
        if (state == KeepPetStateEnum.Hunger && Storage.MedicineCount > 0)
            CheckFeedDrumstick2Guide();
        if (state == KeepPetStateEnum.Happy)
            CheckUseFrisbeeGuide();
    }
    private bool GuideQuickFinishSearchBtn;
    public void OnClickFeedBtn()
    {
        FinishFeedDrumstick1Guide();
        FinishFeedDrumstick2Guide();
        if (CurState.Enum != KeepPetStateEnum.Hunger)
            return;
        if (Storage.Cure)
            return;
        var useCount = 1;
        if (!UserData.Instance.CanAford(UserData.ResourceId.KeepPetDogDrumstick,useCount))
        {
            UIPopupKeepPetGiftNoDrumsticksController.Open();
            return;
        }
        KeepPetModel.Instance.FeedDrumStick();
    }

    private bool GuideFrisbee = false;
    public void OnClickFrisbeeBtn()
    {
        if (GuideFrisbee)
            return;
        if (GuideSubSystem.Instance.CurrentConfig != null && GuideSubSystem.Instance.CurrentConfig.targetType ==
            (int) GuideTargetType.KeepPetFrisbee1)
        {
            GuideFrisbee = true;
        }
        FinishUseFrisbeeGuide();
        if (CurState.Enum != KeepPetStateEnum.Happy)
            return;
        var useCount = 1;
        if (!UserData.Instance.CanAford(UserData.ResourceId.KeepPetDogFrisbee,useCount))
        {
            UIPopupKeepPetTaskController.Open();
            return;
        }
        KeepPetModel.Instance.UseFrisbee();
    }
    public void OnClickSearchTaskBtn()
    {
        FinishClickSearchBtnGuide();
        // FinishSteakGuide();
        if (CurState.Enum == KeepPetStateEnum.Searching || CurState.Enum == KeepPetStateEnum.SearchFinish)
            return;
        UIPopupKeepPetPatrolController.Open(Storage);
    }
    public void OnClickDailyTaskBtn()
    {
        FinishDailyTaskEntranceGuide();
        FinishDailyTaskEntrance2Guide();
        UIPopupKeepPetTaskController.Open();
    }

    public void OnClickQuickFinishSearchBtn()
    {
        FinishQuickFinishSearchingGuide();
        var curTime = (long) APIManager.Instance.GetServerTime();
        var leftTime = Storage.SearchEndTime - curTime;
        if (leftTime < 0)
            leftTime = 0;
        var leftHour = (float)leftTime / XUtility.Hour;
        var price = (int)(leftHour * KeepPetModel.Instance.GlobalConfig.SearchTaskQuickFinishPrice);
        if (price < 1)
            price = 1;
        if (GuideQuickFinishSearchBtn)
        {
            price = 0;
            GuideQuickFinishSearchBtn = false;
        }
        if (UserData.Instance.CanAford(UserData.ResourceId.Diamond, price))
        {
            UserData.Instance.ConsumeRes(UserData.ResourceId.Diamond,price,
                new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.KeepPetUse));
            Storage.SearchEndTime = curTime;
            KeepPetModel.Instance.CheckStateChange();
        }
        else
        {
            BuyResourceManager.Instance.TryShowBuyResource(UserData.ResourceId.Diamond, "KeepPetQuickFinishSearchTask",
                "", "KeepPetQuickFinishSearchTask",true,price);
        }
    }

    public void UpdateSearchingProgress()
    {
        if (CurState.Enum != KeepPetStateEnum.Searching)
            return;
        var curTime = (long) APIManager.Instance.GetServerTime();
        var allTime = Storage.SearchEndTime - Storage.SearchStartTime;
        var leftTime = Storage.SearchEndTime - curTime;
        if (leftTime < 0)
            leftTime = 0;
        var progress = (allTime - leftTime) / (float) allTime;
        if (SearchingProgressSlider)
        {
            SearchingProgressSlider.value = progress;
        }
        if (SearchingLeftTimeText)
            SearchingLeftTimeText.SetText(CommonUtils.FormatLongToTimeStr(leftTime));
        var leftHour = (float)leftTime / (long)XUtility.Hour;
        var price = (int)(leftHour * KeepPetModel.Instance.GlobalConfig.SearchTaskQuickFinishPrice);
        if (price < 1)
            price = 1;
        if (GuideQuickFinishSearchBtn)
        {
            price = 0;
        }
        QuickFinishSearchPriceText.SetText(price.ToString());
    }

    private bool PlayingHappySpine = false;
    public void OnClickDogBtn()
    {
        if (CurState.Enum == KeepPetStateEnum.Sleep)
            KeepPetModel.Instance.AwakeDog();
        if (CurState.Enum == KeepPetStateEnum.Happy)
        {
            if (PlayingHappySpine || IsPerformFrisbee || IsPerformEat)
                return;
            KeepPetModel.Instance.AwakeDog();
            PlayingHappySpine = true;
            PlayHappySound();
            DogSpine.AnimationState.SetAnimation(0, "happy", false).Complete += (t) =>
            {
                // DogSpine.AnimationState.SetAnimation(0, "idle", true);
                // DogSpine.AnimationState.Update(0);
                PlayIdleSpine();
                PlayingHappySpine = false;
            };
            DogSpine.AnimationState.Update(0);
            return;
        }

        if (CurState.Enum == KeepPetStateEnum.Hunger)
        {
            return;
        }

        if (CurState.Enum == KeepPetStateEnum.SearchFinish && !IsPerformSearchFinish)
        {
            FinishSearchFinishGetRewardGuide();
            UIPopupKeepPetBagController.Open(Storage);
            return;
        }
    }

    public void OnClickSearchFinishGetRewardBtn()
    {
        FinishSearchFinishGetRewardGuide();
        if (CurState.Enum == KeepPetStateEnum.SearchFinish)
        {
            UIPopupKeepPetBagController.Open(Storage);
            return;
        }
    }

    public void SetDogSkin(string skinName)
    {
        Storage.SkinName = skinName;
        DogSpine.Skeleton.SetSkin(skinName);
        DogSpine.Skeleton.SetSlotsToSetupPose();
        DogSpine.AnimationState.Apply(DogSpine.Skeleton);
    }
    public static UIKeepPetMainController Open(StorageKeepPet storage)
    {
        return UIManager.Instance.OpenUI(UINameConst.UIKeepPetMain, storage) as UIKeepPetMainController;
    }
}