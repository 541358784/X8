using System;
using System.Collections.Generic;
using Activity.TreasureMap;
using DragonPlus;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupKeepPetPatrolController:UIWindowController
{
    public static UIPopupKeepPetPatrolController Open(StorageKeepPet storage)
    {
        return UIManager.Instance.OpenUI(UINameConst.UIPopupKeepPetPatrol,storage) as UIPopupKeepPetPatrolController;
    }

    private Button HelpBtn;
    private Button CloseBtn;
    private Dictionary<int, SearchTaskItem> TaskItemDic;
    private Slider _slider;
    private LocalizeTextMeshProUGUI _sliderText;
    private LocalizeTextMeshProUGUI DogHeadText;

    public override void PrivateAwake()
    {
        HelpBtn = GetItem<Button>("Root/PreviewButton");
        HelpBtn.onClick.AddListener(() =>
        {
            UIPopupKeepPetPatrolRewardController.Open();
        });
        CloseBtn = GetItem<Button>("Root/CloseButton");
        CloseBtn.onClick.AddListener(() =>
        {
            AnimCloseWindow();
        });
        TaskItemDic = new Dictionary<int, SearchTaskItem>();
        _slider = transform.Find("Root/Slider").GetComponent<Slider>();
        _slider.gameObject.SetActive(false);
        _sliderText = transform.Find("Root/Slider/Text").GetComponent<LocalizeTextMeshProUGUI>();
        DogHeadText = transform.Find("Root/IconNum/Text").GetComponent<LocalizeTextMeshProUGUI>();
        EventDispatcher.Instance.AddEvent<EventKeepPetDogHeadChange>(OnDogHeadChange);
    }

    public void OnDogHeadChange(EventKeepPetDogHeadChange evt)
    {
        DogHeadText.SetText(evt.NewValue+"/"+KeepPetModel.Instance.GlobalConfig.MaxDogHead);
    }
    
    public void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEvent<EventKeepPetDogHeadChange>(OnDogHeadChange);
    }

    private StorageKeepPet Storage;
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        Storage = objs[0] as StorageKeepPet;
        DogHeadText.SetText(Storage.DogHeadCount+"/"+KeepPetModel.Instance.GlobalConfig.MaxDogHead);
        var searchTaskConfig = KeepPetModel.Instance.SearchTaskConfig;
        for (var i = 0; i < searchTaskConfig.Count; i++)
        {
            var taskConfig = searchTaskConfig[i];
            var node = transform.Find("Root/TaskGroup/" + (i + 1));
            var taskItem = node.gameObject.AddComponent<SearchTaskItem>();
            taskItem.Init(this,Storage,taskConfig);
            TaskItemDic.Add(taskConfig.Id,taskItem);
        }

        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.KeepPetSearchTask2))
        {
            List<Transform> topLayer = new List<Transform>();
            topLayer.Add(TaskItemDic[1].SearchBtn.transform);
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.KeepPetSearchTask2, TaskItemDic[1].SearchBtn.transform as RectTransform,
                topLayer: topLayer);
            if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.KeepPetSearchTask2, ""))
            {
                
            }
        }
        if (TreasureMapModel.Instance.IsOpen())
        {
            var mapLimitConfig = TreasureMapModel.Instance.GetTreasureMapLimitConfig();
            if (mapLimitConfig != null)
            {
                    
                _slider.gameObject.SetActive(true);
                _sliderText.SetText(TreasureMapModel.Instance.TreasureMap.FinishTaskCount+"/"+ mapLimitConfig.DisplayMax);
                _slider.maxValue = mapLimitConfig.DisplayMax;
                _slider.value = TreasureMapModel.Instance.TreasureMap.FinishTaskCount;
            }

        }
    }

    public void OnClickSearchBtn(KeepPetSearchTaskConfig taskConfig)
    {
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.KeepPetSearchTask2);
        if (UserData.Instance.CanAford((UserData.ResourceId) taskConfig.ConsumeType, taskConfig.ConsumeCount))
        {
            KeepPetModel.Instance.PerformSearchTask(taskConfig.Id);
            AnimCloseWindow();
        }
        else
        {
            if ((UserData.ResourceId) taskConfig.ConsumeType == UserData.ResourceId.KeepPetDogHead)
            {
                UIPopupKeepPetGiftNoPowerController.Open();
                return;
            }
            else
            {
                if (Storage.ThreeOneStoreBuyState)
                {
                    UIManager.Instance.OpenUI(UINameConst.UIPopupKeepPetGift);   
                }
                else
                {
                    UIManager.Instance.OpenUI(UINameConst.UIPopupKeepPetThreeOneGift);
                }
                return;   
            }
        }
    }

    public class SearchTaskItem : MonoBehaviour
    {
        private UIPopupKeepPetPatrolController Controller;
        private StorageKeepPet Storage;
        private KeepPetSearchTaskConfig Config;
        public Button SearchBtn;
        private LocalizeTextMeshProUGUI TitleText;
        private LocalizeTextMeshProUGUI Text;
        private LocalizeTextMeshProUGUI TimeText;
        private LocalizeTextMeshProUGUI PriceText;
        private Transform Tag;
        private LocalizeTextMeshProUGUI TagText;
        private LocalizeTextMeshProUGUI ExpText;
        private void Awake()
        {
            SearchBtn = transform.Find("Button").GetComponent<Button>();
            SearchBtn.onClick.AddListener(()=>Controller.OnClickSearchBtn(Config));
            TimeText = transform.Find("TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
            PriceText = transform.Find("Button/Root/Text").GetComponent<LocalizeTextMeshProUGUI>();
            TitleText = transform.Find("TitleText").GetComponent<LocalizeTextMeshProUGUI>();
            Text = transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
            Tag = transform.Find("Tag");
            TagText = transform.Find("Tag/Text").GetComponent<LocalizeTextMeshProUGUI>();
            ExpText = transform.Find("LevelUp/Text").GetComponent<LocalizeTextMeshProUGUI>();
        }

        public void Init(UIPopupKeepPetPatrolController controller,StorageKeepPet storage,KeepPetSearchTaskConfig config)
        {
            Controller = controller;
            Storage = storage;
            Config = config;
            Text.SetTermFormats(Config.RewardCount.ToString(),Config.PickCount.ToString());
            PriceText.SetText(Config.ConsumeCount.ToString());
            TimeText.SetText(CommonUtils.FormatLongToTimeStr(Config.Time*(long)XUtility.Min));
            ExpText.SetText(Config.Exp.ToString());
            PriceText.gameObject.SetActive(Config.ConsumeCount > 1);
            if ((UserData.ResourceId) Config.ConsumeType == UserData.ResourceId.KeepPetDogSteak)
            {
                var redPoint = transform.Find("Button/Root/Num");
                var redPointText = transform.Find("Button/Root/Num/Text").GetComponent<LocalizeTextMeshProUGUI>();
                var addPoint = transform.Find("Button/Root/Add");
                var leftCount = UserData.Instance.GetRes((UserData.ResourceId) Config.ConsumeType);
                addPoint.gameObject.SetActive(leftCount <= 0);
                redPoint.gameObject.SetActive(leftCount > 0);
                redPointText.SetText(leftCount.ToString());
                Action<EventKeepPetSearchPropCountChange> eventDealAction = (e) =>
                {
                    leftCount = e.NewValue;
                    addPoint.gameObject.SetActive(leftCount <= 0);
                    redPoint.gameObject.SetActive(leftCount > 0);
                    redPointText.SetText(leftCount.ToString());
                };
                EventDispatcher.Instance.AddEvent<EventKeepPetSearchPropCountChange>(eventDealAction);
                OnDestroyAction += () => EventDispatcher.Instance.RemoveEvent<EventKeepPetSearchPropCountChange>(eventDealAction);
            }

            Tag.gameObject.SetActive(TreasureMapModel.Instance.IsOpen());
            TagText.SetText("+"+config.MapExp);
        }

        private Action OnDestroyAction;
        private void OnDestroy()
        {
            OnDestroyAction?.Invoke();
        }
    }
}