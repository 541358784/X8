using System;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.CoinRush;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public partial class MergeKeepPet : MonoBehaviour
{
    private Button _btn;
    private StorageKeepPet Storage;
    private Transform RedPoint;
    private Transform Idle;
    private Transform Hungry;
    private Transform Sleep;
    private Transform Mission;
    private Transform MissionFinish;
    private LocalizeTextMeshProUGUI StateText;
    private Transform SearchTimeGroup;
    private LocalizeTextMeshProUGUI SearchTimeText;
    private Button FinalDailyTaskRewardBtn;
    private Button FeedBtn;
    private Button SearchTaskFinishBtn;
    public List<SkeletonGraphic> DogSpineList = new List<SkeletonGraphic>();

    private void SetStorage(StorageKeepPet storage)
    {
        Storage = storage;
        RefreshView();
        InitBubble();
        InitDailyTaskSlider();
        UpdateButtonState();
    }

    public void OnCollectFinalDailyTaskReward(EventKeepPetCollectFinalDailyTaskReward evt)
    {
        UpdateButtonState();
        UpdateDailyTaskSliderViewState();
    }

    public void OnCollectDailyTaskReward(EventKeepPetCollectDailyTaskReward evt)
    {
        UpdateButtonState();
        UpdateDailyTaskSliderViewState();
    }

    public void OnExpChange(EventKeepPetExpChange evt)
    {
        var oldLevel = evt.OldValue.KeepPetGetCurLevelConfig();
        var newLevel = evt.NewValue.KeepPetGetCurLevelConfig();
        if (newLevel.Id > oldLevel.Id)
        {
            UpdateDailyTaskSliderViewState();
        }
    }

    public void UpdateButtonState()
    {
        var showFinalDailyTaskRewardBtn = !KeepPetModel.Instance.StorageDailyTask.IsCollectFinalReward &&
                                          KeepPetModel.Instance.Level >= KeepPetModel.Instance.GlobalConfig
                                              .DailyTaskFinalRewardNeedTaskCount;
        var showFeedBtn = CurState.Enum == KeepPetStateEnum.Hunger && Storage.MedicineCount > 0;
        var showSearchTaskFinishBtn = CurState.Enum == KeepPetStateEnum.SearchFinish;
        FinalDailyTaskRewardBtn.gameObject.SetActive(!showFeedBtn && !showSearchTaskFinishBtn &&
                                                     showFinalDailyTaskRewardBtn);
        FeedBtn.gameObject.SetActive(showFeedBtn);
        SearchTaskFinishBtn.gameObject.SetActive(showSearchTaskFinishBtn);
    }

    private void Awake()
    {
        _btn = transform.GetComponent<Button>();
        _btn.onClick.AddListener(OnClick);
        Idle = transform.Find("Root/Idle");
        Hungry = transform.Find("Root/Hungry");
        Sleep = transform.Find("Root/Sleep");
        Mission = transform.Find("Root/Mission");
        MissionFinish = transform.Find("Root/MissionFinish");
        EventDispatcher.Instance.AddEvent<EventKeepPetStateChange>(OnStateChange);
        RedPoint = transform.Find("Root/RedPoint");
        EventDispatcher.Instance.AddEvent<EventKeepPetMedicineCountChange>(OnDrumStickCountChange);
        EventDispatcher.Instance.AddEvent<EventKeepPetAwakeStateChange>(OnSleepStateChange);
        InvokeRepeating("UpdateRedPointVisible", 0f, 1f);
        StateText = transform.Find("Root/Text").GetComponent<LocalizeTextMeshProUGUI>();
        SearchTimeGroup = transform.Find("Root/TimeGroup");
        SearchTimeText = transform.Find("Root/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        InvokeRepeating("UpdateSearchTimeText", 0, 1);
        FinalDailyTaskRewardBtn = transform.Find("Root/ButtonGroup/SteakButton").GetComponent<Button>();
        FinalDailyTaskRewardBtn.onClick.AddListener(() => KeepPetModel.Instance.CollectFinalReward());
        FeedBtn = transform.Find("Root/ButtonGroup/FeedButton").GetComponent<Button>();
        FeedBtn.onClick.AddListener(OnClick);
        SearchTaskFinishBtn = transform.Find("Root/ButtonGroup/ClaimButton").GetComponent<Button>();
        SearchTaskFinishBtn.onClick.AddListener(OnClick);
        EventDispatcher.Instance.AddEvent<EventKeepPetCollectFinalDailyTaskReward>(OnCollectFinalDailyTaskReward);
        EventDispatcher.Instance.AddEvent<EventKeepPetCollectDailyTaskReward>(OnCollectDailyTaskReward);
        EventDispatcher.Instance.AddEvent<EventKeepPetExpChange>(OnExpChange);
        DogSpineList.Add(Idle.Find("Spine").GetComponent<SkeletonGraphic>());
        DogSpineList.Add(Hungry.Find("Spine").GetComponent<SkeletonGraphic>());
        DogSpineList.Add(Sleep.Find("Spine").GetComponent<SkeletonGraphic>());
        DogSpineList.Add(Mission.Find("Spine").GetComponent<SkeletonGraphic>());
        DogSpineList.Add(MissionFinish.Find("Spine").GetComponent<SkeletonGraphic>());
        EventDispatcher.Instance.AddEvent<EventKeepPetChangeSkin>(OnChangeSkin);
        
        SetStorage(KeepPetModel.Instance.Storage);
    }

    private void Start()
    {
        Idle.gameObject.SetActive(true);
        Hungry.gameObject.SetActive(true);
        Sleep.gameObject.SetActive(true);
        Mission.gameObject.SetActive(true);
        MissionFinish.gameObject.SetActive(true);
        RefreshView();
        UpdateSkin();
    }

    public void UpdateSkin()
    {
        foreach (var spine in DogSpineList)
        {
            if (spine.Skeleton != null)
            {
                spine.Skeleton.SetSkin(Storage.SkinName);
                spine.Skeleton.SetSlotsToSetupPose();
                spine.AnimationState.Apply(spine.Skeleton);
            }
        }
    }

    public void OnDrumStickCountChange(EventKeepPetMedicineCountChange evt)
    {
        UpdateRedPointVisible();
        UpdateButtonState();
    }

    public void UpdateSearchTimeText()
    {
        RefreshView();
        
        if (Storage.GetCurState().Enum == KeepPetStateEnum.Searching)
        {
            var curTime = (long) APIManager.Instance.GetServerTime();
            var leftTime = KeepPetModel.Instance.Storage.SearchEndTime - curTime;
            SearchTimeText.SetText(CommonUtils.FormatLongToTimeStr(leftTime));
        }
    }

    public void UpdateRedPointVisible()
    {
        RedPoint.gameObject.SetActive(
            (Storage.GetCurState().Enum == KeepPetStateEnum.Hunger && Storage.MedicineCount > 0) ||
            (Storage.GetCurState().Enum == KeepPetStateEnum.SearchFinish) ||
            KeepPetModel.Instance.StorageDailyTask.UnCollectRewards.Count > 0);
    }

    // public void Update()
    // {
    //     RefreshView();
    // }
    public void OnClick()
    {
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.KeepPetEntrance1);
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.KeepPetFeed4);
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.KeepPetEntrance2);
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.KeepPetEntrance3);
        KeepPetModel.Instance.OpenMainView("MergeEntrance");
    }

    public void RefreshView()
    {
        gameObject.SetActive(Storage != null);
        if (Storage == null)
            return;
        
        var curState = Storage.GetCurState().Enum;
        var animationName = "";
        switch (curState)
        {
            case KeepPetStateEnum.Happy:
                ShowSpine(Idle);
                StateText.SetTerm("ui_dog_icon_desc3");
                break;
            case KeepPetStateEnum.Hunger:
                ShowSpine(Hungry);
                StateText.SetTerm("ui_dog_icon_desc2");
                break;
            case KeepPetStateEnum.Searching:
                ShowSpine(Mission);
                StateText.SetTerm("ui_dog_icon_desc1");
                break;
            case KeepPetStateEnum.SearchFinish:
                ShowSpine(MissionFinish);
                StateText.SetTerm("ui_dog_icon_desc1");
                break;
            case KeepPetStateEnum.Sleep:
                ShowSpine(Sleep);
                StateText.SetTerm("ui_dog_icon_desc3");
                break;
        }

        SearchTimeGroup.gameObject.SetActive(curState == KeepPetStateEnum.Searching);
        UpdateRedPointVisible();
    }

    public void ShowSpine(Transform showNode)
    {
        var nodeList = new List<Transform>() {Idle, Hungry, Sleep, Mission, MissionFinish};
        for (var i = 0; i < nodeList.Count; i++)
        {
            nodeList[i].gameObject.SetActive(showNode == nodeList[i]);
        }
    }

    public void OnSleepStateChange(EventKeepPetAwakeStateChange evt)
    {
        RefreshView();
    }

    public void OnStateChange(EventKeepPetStateChange evt)
    {
        RefreshView();
        UpdateButtonState();
    }
    public void OnChangeSkin(EventKeepPetChangeSkin evt)
    {
        UpdateSkin();
    }
    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEvent<EventKeepPetStateChange>(OnStateChange);
        EventDispatcher.Instance.RemoveEvent<EventKeepPetMedicineCountChange>(OnDrumStickCountChange);
        EventDispatcher.Instance.RemoveEvent<EventKeepPetAwakeStateChange>(OnSleepStateChange);
        EventDispatcher.Instance.RemoveEvent<EventKeepPetCollectFinalDailyTaskReward>(OnCollectFinalDailyTaskReward);
        EventDispatcher.Instance.RemoveEvent<EventKeepPetCollectDailyTaskReward>(OnCollectDailyTaskReward);
        EventDispatcher.Instance.RemoveEvent<EventKeepPetExpChange>(OnExpChange);
        EventDispatcher.Instance.RemoveEvent<EventKeepPetChangeSkin>(OnChangeSkin);
    }
}