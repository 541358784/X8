using System;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;
using BiEventCooking=DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public partial class Aux_KeepPet : MonoBehaviour
{
    private Transform Idle;
    private Transform Hungry;
    private Transform Sleep;
    private Transform Mission;
    private Transform MissionFinish;
    private Transform RedPoint;
    private Slider Slider;
    private LocalizeTextMeshProUGUI TimeText;
    public List<SkeletonGraphic> DogSpineList = new List<SkeletonGraphic>();
    public void Awake()
    {
        InvokeRepeating("UpdateActiveState",0f,1f);
        var btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(OnButtonClick);
        Idle = transform.Find("Root/Idle");
        Hungry = transform.Find("Root/Hungry");
        Sleep = transform.Find("Root/Sleep");
        Mission = transform.Find("Root/Mission");
        MissionFinish = transform.Find("Root/MissionFinish");
        EventDispatcher.Instance.AddEvent<EventKeepPetStateChange>(OnStateChange);
        RedPoint = transform.Find("Root/RedPoint");
        Slider = transform.Find("Slider").GetComponent<Slider>();
        TimeText = transform.Find("Slider/Text").GetComponent<LocalizeTextMeshProUGUI>();
        InvokeRepeating("UpdateTime",0f,1f);
        EventDispatcher.Instance.AddEvent<EventKeepPetMedicineCountChange>(OnDrumStickCountChange);
        EventDispatcher.Instance.AddEvent<EventKeepPetAwakeStateChange>(OnSleepStateChange);
        EventDispatcher.Instance.AddEvent<EventKeepPetChangeSkin>(OnChangeSkin);
        UpdateUI();
        InvokeRepeating("UpdateRedPointVisible",0f,1f);
        InitBubble();
        DogSpineList.Add(Idle.Find("Spine").GetComponent<SkeletonGraphic>());
        DogSpineList.Add(Hungry.Find("Spine").GetComponent<SkeletonGraphic>());
        DogSpineList.Add(Sleep.Find("Spine").GetComponent<SkeletonGraphic>());
        DogSpineList.Add(Mission.Find("Spine").GetComponent<SkeletonGraphic>());
        DogSpineList.Add(MissionFinish.Find("Spine").GetComponent<SkeletonGraphic>());
    }

    private void OnEnable()
    {
        Idle.gameObject.SetActive(true);
        Hungry.gameObject.SetActive(true);
        Sleep.gameObject.SetActive(true);
        Mission.gameObject.SetActive(true);
        MissionFinish.gameObject.SetActive(true);
        UpdateUI();
        UpdateSkin();
    }

    public void UpdateSkin()
    {
        if (!KeepPetModel.Instance.IsOpen())
            return;
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
    

    public void UpdateTime()
    {
        if (!KeepPetModel.Instance.IsOpen())
            return;
        if (KeepPetModel.Instance.CurState.Enum != KeepPetStateEnum.Searching)
            return;
        var curTime = (long) APIManager.Instance.GetServerTime();
        var leftTime = KeepPetModel.Instance.Storage.SearchEndTime - curTime;
        var progress = (float)(curTime - KeepPetModel.Instance.Storage.SearchStartTime)
                       / (KeepPetModel.Instance.Storage.SearchEndTime - KeepPetModel.Instance.Storage.SearchStartTime);
        Slider.value = progress;
        TimeText.SetText(CommonUtils.FormatLongToTimeStr(leftTime));
    }
    public void OnDrumStickCountChange(EventKeepPetMedicineCountChange evt)
    {
        UpdateRedPointVisible();
    }
    public void UpdateRedPointVisible()
    {
        if (!KeepPetModel.Instance.IsOpen())
            return;
        RedPoint.gameObject.SetActive(
            (KeepPetModel.Instance.CurState.Enum == KeepPetStateEnum.Hunger && KeepPetModel.Instance.Storage.MedicineCount > 0) ||
            (KeepPetModel.Instance.CurState.Enum == KeepPetStateEnum.SearchFinish) || 
            KeepPetModel.Instance.StorageDailyTask.UnCollectRewards.Count > 0);
    }

    public void UpdateActiveState()
    {
        gameObject.SetActive(KeepPetModel.Instance.IsOpen());
    }
    public void OnStateChange(EventKeepPetStateChange evt)
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (!KeepPetModel.Instance.IsOpen())
            return;
        var curState = KeepPetModel.Instance.CurState.Enum;
        var animationName = "";
        switch (curState)
        {
            case KeepPetStateEnum.Happy:
                ShowSpine(Idle);
                break;
            case KeepPetStateEnum.Hunger:
                ShowSpine(Hungry);
                break;
            case KeepPetStateEnum.Searching:
                ShowSpine(Mission);
                break;
            case KeepPetStateEnum.SearchFinish:
                ShowSpine(MissionFinish);
                break;
            case KeepPetStateEnum.Sleep:
                ShowSpine(Sleep);
                break;
        }
        UpdateRedPointVisible();
        Slider.gameObject.SetActive(curState == KeepPetStateEnum.Searching);
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
        UpdateUI();
    }

    public void OnChangeSkin(EventKeepPetChangeSkin evt)
    {
        UpdateSkin();
    }
    public void OnButtonClick()
    {
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.KeepPetEntrance1);
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.KeepPetFeed4);
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.KeepPetEntrance2);
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.KeepPetEntrance3);
        KeepPetModel.Instance.OpenMainView("HomeEntrance");
    }
    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEvent<EventKeepPetStateChange>(OnStateChange);
        EventDispatcher.Instance.RemoveEvent<EventKeepPetMedicineCountChange>(OnDrumStickCountChange);
        EventDispatcher.Instance.RemoveEvent<EventKeepPetAwakeStateChange>(OnSleepStateChange);
        EventDispatcher.Instance.RemoveEvent<EventKeepPetChangeSkin>(OnChangeSkin);
    }
}
