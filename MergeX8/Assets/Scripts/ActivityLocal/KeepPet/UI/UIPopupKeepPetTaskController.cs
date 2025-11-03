using System;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public partial class UIPopupKeepPetTaskController:UIWindowController
{
    public static UIPopupKeepPetTaskController Open()
    {
        return UIManager.Instance.OpenUI(UINameConst.UIPopupKeepPetTask) as UIPopupKeepPetTaskController;
    }
    private Button _buttonClose;
    private Slider _progressSlider;
    private LocalizeTextMeshProUGUI _progressSliderText;
    private Transform _timeGroup;
    private LocalizeTextMeshProUGUI _timeGroupText;
    Transform _taskDefaultItem;
    private RectTransform _content;
    private Dictionary<int, KeepPetTaskItem> _taskItemDictionary;
    private Transform NextLevelUnlockItem;
    private LocalizeTextMeshProUGUI NextLevelUnlockText;
    private Image _lastRewardImage;
    private LocalizeTextMeshProUGUI _lastRewardCountText;
    private LocalizeTextMeshProUGUI FrisbeeCountText;
    private Button StartBtn;
    public override void PrivateAwake()
    {
        _buttonClose = GetItem<Button>("Root/CloseButton");
        _buttonClose.onClick.AddListener(OnCloseBtn);
        _progressSlider = GetItem<Slider>("Root/Slider");
        _progressSliderText = GetItem<LocalizeTextMeshProUGUI>("Root/Slider/Text");
        _timeGroup = GetItem<Transform>("Root/TimeGroup");
        _timeGroupText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        _taskDefaultItem = GetItem<Transform>("Root/Scroll View/Viewport/Content/Task");
        _taskDefaultItem.gameObject.SetActive(false);
        _content = GetItem<RectTransform>("Root/Scroll View/Viewport/Content");
        _taskItemDictionary = new Dictionary<int, KeepPetTaskItem>();
        _lastRewardImage = GetItem<Image>("Root/Slider/Icon");
        _lastRewardCountText = GetItem<LocalizeTextMeshProUGUI>("Root/Slider/Icon/Text");
        InvokeRepeating("UpdateTime", 0, 1);
        FrisbeeCountText = GetItem<LocalizeTextMeshProUGUI>("Root/IconNum/Text");
        EventDispatcher.Instance.AddEvent<EventKeepPetFrisbeeCountChange>(OnFrisbeeCountChange);
        StartBtn = GetItem<Button>("Root/Button");
        StartBtn.onClick.AddListener(() =>
        {
            var mainUI = UIManager.Instance.GetOpenedUIByPath<UIKeepPetMainController>(UINameConst.UIKeepPetMain);
            if (mainUI)
                mainUI.AnimCloseWindow();
            AnimCloseWindow(() =>
            {
                if (SceneFsm.mInstance.GetCurrSceneType() != StatusType.Game)
                {
                    SceneFsm.mInstance.TransitionGame();
                }
            });
        });
        NextLevelUnlockItem = transform.Find("Root/Scroll View/Viewport/Content/Lock");
        NextLevelUnlockText = GetItem<LocalizeTextMeshProUGUI>("Root/Scroll View/Viewport/Content/Lock/LV/Text");
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEvent<EventKeepPetFrisbeeCountChange>(OnFrisbeeCountChange);
    }

    public void OnFrisbeeCountChange(EventKeepPetFrisbeeCountChange e)
    {
        FrisbeeCountText.SetText(UserData.Instance.GetRes(UserData.ResourceId.KeepPetDogFrisbee)+"/"+KeepPetModel.Instance.GlobalConfig.MaxFrisbee);
    }

    // private StorageKeepPetDailyTask StorageDailyTask;
    private long refreshTime;
    protected override async void OnOpenWindow(params object[] objs)  
    {
        base.OnOpenWindow(objs);
        // StorageDailyTask = objs[0] as StorageKeepPetDailyTask;
        refreshTime = KeepPetModel.Instance.GetDailyTaskRefreshTime();
        RefreshUI();
        KeepPetModel.Instance.CollectFinalReward();
        // GuideSubSystem.Instance.ForceFinished(3016);
        FrisbeeCountText.SetText(UserData.Instance.GetRes(UserData.ResourceId.KeepPetDogFrisbee)+"/"+KeepPetModel.Instance.GlobalConfig.MaxFrisbee);
        CheckKeepPetDailyTaskInfo1Guide();
        CheckKeepPetDailyTaskInfo2Guide();
    }

    public void RefreshUI()
    {
        _progressSlider.gameObject.SetActive(KeepPetModel.Instance.MaxLevel >= KeepPetModel.Instance.GlobalConfig.DailyTaskFinalRewardNeedTaskCount);
        _lastRewardImage.sprite =
            UserData.GetResourceIcon(KeepPetModel.Instance.FinialRewards[0].id, UserData.ResourceSubType.Big);
        if (_lastRewardCountText)
            _lastRewardCountText.SetText(KeepPetModel.Instance.FinialRewards[0].count.ToString());
        _progressSlider.value = (float) KeepPetModel.Instance.Level / KeepPetModel.Instance.MaxLevel;
        _progressSliderText.SetText(KeepPetModel.Instance.Level+"/"+KeepPetModel.Instance.MaxLevel);
        var curLevel = KeepPetModel.Instance.Storage.Exp.KeepPetGetCurLevelConfig();
        if (_taskItemDictionary.Keys.Count != KeepPetModel.Instance.MaxLevel)
        {
            foreach (var KeepPetTask in _taskItemDictionary)
            {
                GameObject.Destroy(KeepPetTask.Value.gameObject);
            }   
            _taskItemDictionary.Clear();
            for (var i = 0; i < KeepPetModel.Instance.DailyTaskConfig.Count; i++)
            {
                var config = KeepPetModel.Instance.DailyTaskConfig[i];
                if (curLevel.Id < config.UnLockLevel)
                    continue;
                var KeepPetTaskObj = GameObject.Instantiate(_taskDefaultItem.gameObject, _taskDefaultItem.parent);
                KeepPetTaskObj.SetActive(true);
                var KeepPetTask = KeepPetTaskObj.AddComponent<KeepPetTaskItem>();
                KeepPetTask.Init(config);
                _taskItemDictionary.Add(config.Id,KeepPetTask);
            }
        }

        var unCompletedTaskList = new List<KeepPetTaskItem>();
        var completedTaskList = new List<KeepPetTaskItem>();
        
        for (var i = 0; i < KeepPetModel.Instance.DailyTaskConfig.Count; i++)
        {
            var config = KeepPetModel.Instance.DailyTaskConfig[i];
            if (curLevel.Id < config.UnLockLevel)
                continue;
            var KeepPetTask = _taskItemDictionary[config.Id];
            KeepPetTask.RefreshView();
            if (KeepPetModel.Instance.StorageDailyTask.AlreadyCollectLevels.Contains(config.Id))
            {
                completedTaskList.Add(KeepPetTask);
            }
            else
            {
                unCompletedTaskList.Add(KeepPetTask);
            }
        }

        for (var i = 0; i < unCompletedTaskList.Count; i++)
        {
            unCompletedTaskList[i].transform.SetAsLastSibling();
        }
        for (var i = 0; i < completedTaskList.Count; i++)
        {
            completedTaskList[i].transform.SetAsLastSibling();
        }
        NextLevelUnlockItem.SetAsLastSibling();
        var nextUnlockLevel = KeepPetModel.Instance.GetNextUnlockNewDailyTaskLevel();
        NextLevelUnlockItem.gameObject.SetActive(nextUnlockLevel != null);
        if (nextUnlockLevel!=null)
            NextLevelUnlockText.SetText(nextUnlockLevel.UnLockLevel.ToString());
        _content.anchoredPosition = new Vector2(0, 0);
    }

    private void OnCloseBtn()
    {
        AnimCloseWindow();
    }

    public void UpdateTime()
    {
        var curRefreshTime = KeepPetModel.Instance.GetDailyTaskRefreshTime();
        if (refreshTime != curRefreshTime)
        {
            refreshTime = curRefreshTime;
            RefreshUI();
        }

        var leftTime = refreshTime - (long)APIManager.Instance.GetServerTime();
        _timeGroupText.SetText(CommonUtils.FormatLongToTimeStr(leftTime));
    }


    public override void ClickUIMask()
    {
        if (!canClickMask)
            return;

        canClickMask = false;
        AnimCloseWindow();
    }
}