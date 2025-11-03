using System;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using Gameplay;
using Gameplay.UI.Store.Vip.Model;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupGiftBagProgressTaskController:UIWindowController
{
    public static UIPopupGiftBagProgressTaskController Open(StorageGiftBagProgress Storage)
    {
        return UIManager.Instance.OpenUI(UINameConst.UIPopupGiftBagProgressTask,Storage) as UIPopupGiftBagProgressTaskController;
    }
    private Button _buttonClose;
    private Transform _timeGroup;
    private LocalizeTextMeshProUGUI _timeGroupText;
    Transform _taskDefaultItem;
    private RectTransform _content;
    private Dictionary<int, GiftBagProgressTaskItem> _taskItemDictionary;
    private StorageGiftBagProgress Storage;
    private GiftBagProgressGlobalConfig GlobalConfig => GiftBagProgressModel.Instance.GlobalConfig;

    // private Transform BoxOpenState;
    // private Transform BoxCloseState;
    // private Button TipsBtn;
    private Transform TipsGroup;
    private Transform DefaultTipItem;
    private List<CommonRewardItem> ItemList = new List<CommonRewardItem>();
    private LocalizeTextMeshProUGUI TagText;
    private Button HelpBtn;
    private Transform BuyGroup;
    // private Transform FinishGroup;
    private Button BuyBtn;
    private Text PriceText;
    private Transform LockTipNode;
    private LocalizeTextMeshProUGUI TotalDiamondCountText;
    // private LocalizeTextMeshProUGUI TotalTaskDiamondCountText;
    public override void PrivateAwake()
    {
        _buttonClose = GetItem<Button>("Root/CloseButton");
        _buttonClose.onClick.AddListener(OnCloseBtn);
        _timeGroup = GetItem<Transform>("Root/TimeGroup");
        _timeGroupText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        _taskDefaultItem = GetItem<Transform>("Root/Scroll View/Viewport/Content/Task");
        _taskDefaultItem.gameObject.SetActive(false);
        _content = GetItem<RectTransform>("Root/Scroll View/Viewport/Content");
        _taskItemDictionary = new Dictionary<int, GiftBagProgressTaskItem>();
        InvokeRepeating("UpdateTime", 0, 1);

        // BoxOpenState = GetItem<Transform>("Root/Box/Open");
        // BoxCloseState = GetItem<Transform>("Root/Box/Close");
        // TipsBtn = GetItem<Button>("Root/Box/TipsBtn");
        // TipsBtn.onClick.AddListener(() =>
        // {
        //     TipsGroup.gameObject.SetActive(!TipsGroup.gameObject.activeSelf);
        // });
        TipsGroup = GetItem<Transform>("Root/Box/Tips");
        TipsGroup.gameObject.SetActive(false);
        DefaultTipItem = GetItem<Transform>("Root/Box/Tips/Item");
        DefaultTipItem.gameObject.SetActive(false);
        TagText = GetItem<LocalizeTextMeshProUGUI>("Root/TagGroup/Tag/Text1");
        TagText.SetText(GlobalConfig.TagText);
        HelpBtn = GetItem<Button>("Root/HelpButton");
        HelpBtn.onClick.AddListener(() => UIGiftBagProgressHelpController.Open(Storage));
        BuyGroup = GetItem<Transform>("Root/BuyGroup");
        // FinishGroup = GetItem<Transform>("Root/BuyGroup/Finish");
        BuyBtn = GetItem<Button>("Root/BuyGroup/Buy/Button");
        BuyBtn.onClick.AddListener(() =>
        {
            if (Storage.BuyState)
                return;
            StoreModel.Instance.Purchase(GlobalConfig.ShopId);
        });
        PriceText = GetItem<Text>("Root/BuyGroup/Buy/Button/Text");
        PriceText.text = (StoreModel.Instance.GetPrice(GlobalConfig.ShopId));
        
        transform.Find("Root/BuyGroup/Buy/Button/Vip/Text").GetComponent<LocalizeTextMeshProUGUI>().SetText(VipStoreModel.Instance.GetVipScoreString(GlobalConfig.ShopId));

        EventDispatcher.Instance.AddEvent<EventGiftBagProgressBuyStateChange>(OnBuy);
        LockTipNode = GetItem<Transform>("Root/Lock");
        EventDispatcher.Instance.AddEvent<EventGiftBagProgressCollectTask>(OnCollectTask);
        EventDispatcher.Instance.AddEvent<EventGiftBagProgressCompleteTask>(OnCompleteTask);
        TotalDiamondCountText = GetItem<LocalizeTextMeshProUGUI>("Root/BuyGroup/Buy/Text");
        // TotalTaskDiamondCountText = GetItem<LocalizeTextMeshProUGUI>("Root/BuyGroup/Finish/Text");
    }

    public async void OnBuy(EventGiftBagProgressBuyStateChange evt)
    {
        // BoxOpenState.gameObject.SetActive(Storage.BuyState);
        // BoxCloseState.gameObject.SetActive(!Storage.BuyState);
        // TipsBtn.gameObject.SetActive(!Storage.BuyState);
        BuyGroup.gameObject.SetActive(!Storage.BuyState);
        // FinishGroup.gameObject.SetActive(Storage.BuyState);
        TipsGroup.gameObject.SetActive(!Storage.BuyState);
        var sortList = GetSortList();
        foreach (var item in sortList)
        {
            item.PrepareUnlock();
        }
        foreach (var item in sortList)
        {
            item.PerformUnlock();
            await XUtility.WaitSeconds(0.3f);
        }
    }

    public void OnCollectTask(EventGiftBagProgressCollectTask evt)
    {
        _taskItemDictionary[evt.TaskConfig.Id].Init(evt.TaskConfig,evt.Storage);
        var sortList = GetSortList();
        foreach (var item in sortList)
        {
            item.transform.SetAsLastSibling();
        }
    }
    public void OnCompleteTask(EventGiftBagProgressCompleteTask evt)
    {
        _taskItemDictionary[evt.TaskConfig.Id].Init(evt.TaskConfig,evt.Storage);
        var sortList = GetSortList();
        foreach (var item in sortList)
        {
            item.transform.SetAsLastSibling();
        }
    }
    private void OnDestroy()
    {
        LockTipNode.DOKill(false);
        EventDispatcher.Instance.RemoveEvent<EventGiftBagProgressBuyStateChange>(OnBuy);
        EventDispatcher.Instance.RemoveEvent<EventGiftBagProgressCollectTask>(OnCollectTask);
        EventDispatcher.Instance.RemoveEvent<EventGiftBagProgressCompleteTask>(OnCompleteTask);
    }
    protected override void OnOpenWindow(params object[] objs)  
    {
        base.OnOpenWindow(objs);
        Storage = objs[0] as StorageGiftBagProgress;
        RefreshUI();
    }

    public void RefreshUI()
    {
        // BoxOpenState.gameObject.SetActive(Storage.BuyState);
        // BoxCloseState.gameObject.SetActive(!Storage.BuyState);
        // TipsBtn.gameObject.SetActive(!Storage.BuyState);
        TipsGroup.gameObject.SetActive(true);
        var buyRewards = CommonUtils.FormatReward(GlobalConfig.RewardId, GlobalConfig.RewardNum);
        foreach (var rewardItem in ItemList)
        {
            Destroy(rewardItem.gameObject);
        }
        ItemList.Clear();
        foreach (var reward in buyRewards)
        {
            var rewardItem = Instantiate(DefaultTipItem.gameObject,DefaultTipItem.parent).gameObject.AddComponent<CommonRewardItem>();
            rewardItem.gameObject.SetActive(true);
            rewardItem.Init(reward);
            ItemList.Add(rewardItem);
        }
        TipsGroup.gameObject.SetActive(!Storage.BuyState);
        BuyGroup.gameObject.SetActive(!Storage.BuyState);
        // FinishGroup.gameObject.SetActive(Storage.BuyState);
        
        if (_taskItemDictionary.Keys.Count != GiftBagProgressModel.Instance.DailyTaskConfig.Count)
        {
            foreach (var GiftBagProgressTask in _taskItemDictionary)
            {
                GameObject.Destroy(GiftBagProgressTask.Value.gameObject);
            }   
            _taskItemDictionary.Clear();
            for (var i = 0; i < GiftBagProgressModel.Instance.DailyTaskConfig.Count; i++)
            {
                var config = GiftBagProgressModel.Instance.DailyTaskConfig[i];
                var GiftBagProgressTaskObj = GameObject.Instantiate(_taskDefaultItem.gameObject, _taskDefaultItem.parent);
                GiftBagProgressTaskObj.SetActive(true);
                var GiftBagProgressTask = GiftBagProgressTaskObj.AddComponent<GiftBagProgressTaskItem>();
                GiftBagProgressTask.Init(config,Storage);
                _taskItemDictionary.Add(config.Id,GiftBagProgressTask);
            }
        }

        var sortList = GetSortList();
        foreach (var item in sortList)
        {
            item.transform.SetAsLastSibling();
        }

        _content.anchoredPosition = new Vector2(0, 0);

        var taskDiamondTotalCount = 0;
        foreach (var config in GiftBagProgressModel.Instance.DailyTaskConfig)
        {
            for (var i = 0; i < config.RewardId.Count; i++)
            {
                if (config.RewardId[i] == (int) UserData.ResourceId.Diamond)
                {
                    taskDiamondTotalCount += config.RewardNum[i];
                }
            }
        }
        var bagDiamondCount = 0;
        for (var i = 0; i < GlobalConfig.RewardId.Count; i++)
        {
            if (GlobalConfig.RewardId[i] == (int) UserData.ResourceId.Diamond)
            {
                bagDiamondCount += GlobalConfig.RewardNum[i];
            } 
        }
        var totalDiamondCount = taskDiamondTotalCount + bagDiamondCount;
        TotalDiamondCountText.SetTermFormats(totalDiamondCount.ToString());
        // TotalTaskDiamondCountText.SetTermFormats(taskDiamondTotalCount.ToString());
    }

    public List<GiftBagProgressTaskItem> GetSortList()
    {
        var completedTaskList = new List<GiftBagProgressTaskItem>();
        var unCompletedTaskList = new List<GiftBagProgressTaskItem>();
        var finishTaskList = new List<GiftBagProgressTaskItem>();
        
        for (var i = 0; i < GiftBagProgressModel.Instance.DailyTaskConfig.Count; i++)
        {
            var config = GiftBagProgressModel.Instance.DailyTaskConfig[i];
            var GiftBagProgressTask = _taskItemDictionary[config.Id];
            GiftBagProgressTask.RefreshView();
            if (Storage.AlreadyCollectLevels.Contains(config.Id))
            {
                finishTaskList.Add(GiftBagProgressTask);
            }
            else if (Storage.CanCollectLevels.Contains(config.Id))
            {
                completedTaskList.Add(GiftBagProgressTask);
            }
            else
            {
                unCompletedTaskList.Add(GiftBagProgressTask);
            }
        }
        var totalSortList = new List<GiftBagProgressTaskItem>(); 
        totalSortList.AddRange(completedTaskList);
        totalSortList.AddRange(unCompletedTaskList);
        totalSortList.AddRange(finishTaskList);
        return totalSortList;
    }

    private void OnCloseBtn()
    {
        AnimCloseWindow();
    }

    public void UpdateTime()
    {
        _timeGroupText.SetText(Storage.GetLeftTimeText());
    }


    public override void ClickUIMask()
    {
        if (!canClickMask)
            return;

        canClickMask = false;
        AnimCloseWindow();
    }

    public void ShowLockTip(GiftBagProgressTaskItem item)
    {
        LockTipNode.DOKill(false);
        LockTipNode.gameObject.SetActive(false);
        LockTipNode.gameObject.SetActive(true);
        // LockTipNode.position = item.transform.position;
        DOVirtual.DelayedCall(2f, () => LockTipNode.gameObject.SetActive(false)).SetTarget(LockTipNode);
    }
}