using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ABTest;
using DG.Tweening;
using Dlugin;
using DragonPlus;
using DragonPlus.Config.DogPlay;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using Merge.Order;
using UnityEngine;

public partial class DogPlayModel : Manager<DogPlayModel>
{
    public bool LastOpenState = false;
    private Dictionary<int, List<DogPlayOrderConfig>> ConfigDic = new Dictionary<int, List<DogPlayOrderConfig>>();

    private List<DogPlayPayTypeConfig> PayTypeConfigList => DogPlayConfigManager.Instance.GetConfig<DogPlayPayTypeConfig>();
    public void InitConfig()
    {
        EventDispatcher.Instance.AddEvent<EventCreateMergeOrder>(OnCreateTaskEvent);
        EventDispatcher.Instance.AddEvent<EventCompleteTask>(OnCompleteTaskEvent);
        DogPlayConfigManager.Instance.InitConfig();
        // return;
        ConfigDic.Clear();
        var configs = DogPlayConfigManager.Instance.GetConfig<DogPlayOrderConfig>();
        for (var i = 0; i < configs.Count; i++)
        {
            var config = configs[i];
            if (!ConfigDic.ContainsKey(config.PayType))
            {
                ConfigDic.Add(config.PayType,new List<DogPlayOrderConfig>());
            }
            ConfigDic[config.PayType].Add(config);
        }
        LastOpenState = IsOpen();
        if (!LastOpenState)
        {
            EventDispatcher.Instance.AddEventListener(EventEnum.OnLevelUp,OnLevelUp);
            EventDispatcher.Instance.AddEventListener(EventEnum.OnKeepPetLevelUp,OnLevelUp);
        }
        else
        {
            InitStorage();   
        }
    }

    public void OnLevelUp(BaseEvent evt)
    {
        if (!LastOpenState)
        {
            LastOpenState = IsOpen();
            if (LastOpenState)
            {
                InitStorage();
            }
        }
        if (LastOpenState)
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.OnLevelUp,OnLevelUp);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.OnKeepPetLevelUp,OnLevelUp);
            return;
        }
    }
    // private const string AbKey = "DogPlayABKey";

    public bool IsOpenAB()
    {
        return true;
        // return ABTestManager.Instance.IsOpenDogPlay();
        // if (!StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig.ContainsKey(AbKey))
        // {
        //     StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig[AbKey] =
        //         ABTestManager.Instance.IsOpenDogPlay() ? "1" : "0";
        // }
        //
        // bool isOpen = StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig[AbKey] == "1";
        //
        // if (!StorageManager.Instance.GetStorage<StorageCommon>().Abtests.ContainsKey(AbKey))
        // {
        //     StorageManager.Instance.GetStorage<StorageCommon>().Abtests[AbKey] = isOpen ? "1" : "0";
        // }
        //
        // return isOpen;
    }
    public bool IsUnlock => UnlockManager.IsOpen(UnlockManager.MergeUnlockType.DogPlayUnlockLevel) && UnlockManager.IsOpen(UnlockManager.MergeUnlockType.DogPlayUnlockKeepPetLevel);

    public bool IsOpen()
    {
        // Debug.LogError("DogPlay IsUnlock:"+IsUnlock+" IsAbOpen:"+ABTestManager.Instance.IsOpenDogPlay());
        return IsOpenAB() && IsUnlock;
    }

    public StorageDogPlay Storage => StorageManager.Instance.GetStorage<StorageHome>().DogPlay;

    public DogPlayPayTypeConfig GetCurPayTypeConfig()
    {
        var payValue = StorageManager.Instance.GetStorage<StorageCommon>().RevenueUSDCents/100f;
        DogPlayPayTypeConfig curGroup = null;
        for (var i = 0; i < PayTypeConfigList.Count; i++)
        {
            var payTypeConfig = PayTypeConfigList[i];
            if (payValue >= payTypeConfig.MinPay)
            {
                curGroup = payTypeConfig;
            }
            else
            {
                break;
            }
        }
        return curGroup;
    }
    public void InitStorage()
    {
        if (Storage.CurConfigId == 0)
        {
            Storage.Rounds = 0;
            CreateNewLevelStorage();
        }
    }

    public bool HideTaskItemGroup = false;
    public Task CollectRewards()
    {
        if (Storage.CurCount < Storage.MaxCount)
            return Task.CompletedTask;
        var rewards = CommonUtils.FormatReward(Storage.Rewards);
        var reason = new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.DogPlay);
        UserData.Instance.AddRes(rewards,reason);
        var extraRewards = DogPlayExtraRewardModel.Instance.GetExtraRewards(Storage);
        if (extraRewards.Count > 0)
        {
            DogPlayExtraRewardModel.Instance.OnCollectExtraRewards();
            var extraReason = new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.DogPlay);
            UserData.Instance.AddRes(extraRewards,extraReason);
            rewards.AddRange(extraRewards);
        }
        Storage.Rounds++;
        RefreshTaskCD();
        CreateNewLevelStorage();
        var task = new TaskCompletionSource<bool>();
        CommonRewardManager.Instance.PopCommonReward(rewards, CurrencyGroupManager.Instance.currencyController, false,
            reason,animEndCall:()=>task.SetResult(true));
        return task.Task;
    }

    public void RefreshTaskCD()
    {
        MainOrderManager.Instance.StorageTaskGroup.OrderRefreshTime.Clear();
        // MainOrderManager.Instance.StorageTaskGroup.OrderRefreshTime.Remove(SlotDefinition.Slot2.ToString());
        // MainOrderManager.Instance.StorageTaskGroup.OrderRefreshTime.Remove(SlotDefinition.Slot3.ToString());
        // MainOrderManager.Instance.StorageTaskGroup.OrderRefreshTime.Remove(SlotDefinition.Slot4.ToString());
        // MainOrderManager.Instance.StorageTaskGroup.OrderRefreshTime.Remove(SlotDefinition.Slot5.ToString());
        MainOrderManager.Instance.AutoTryFillOrder();
        // MainOrderManager.Instance.StorageTaskGroup.OrderRefreshTime["MainOrderCreatorRandom1_Point"] = 0;
    }

    public void CreateNewLevelStorage()
    {
        DogPlayPayTypeConfig curGroup = GetCurPayTypeConfig();
        var configs = ConfigDic[curGroup.Id];
        var config = configs[Storage.Rounds<configs.Count?Storage.Rounds:configs.Count-1];
        Storage.CurConfigId = config.Id;
        Storage.OrderCanProductCount.Clear();
        Storage.MaxTaskCountDic.Clear();
        Storage.MaxCount = 0;
        Storage.CurTaskCountDic.Clear();
        Storage.CurCount = 0;
        Storage.OrderActiveState.Clear();
        Storage.Rewards.Clear();
        var rewards = CommonUtils.FormatReward(config.RewardId, config.RewardCount);
        foreach (var reward in rewards)
        {
            Storage.Rewards.Add(reward.id,reward.count);
        }
        var curTaskList = MainOrderManager.Instance.CurTaskList;
        for (var i = 0; i < config.OrderType.Count; i++)
        {
            var orderType = config.OrderType[i];
            var coinCount = config.CoinCount[i];
            Storage.OrderCanProductCount.Add(orderType,coinCount);
            var orderCount = config.OrderCount[i];
            var totalCoin = coinCount * orderCount;
            Storage.MaxTaskCountDic.Add(orderType,orderCount);
            Storage.CurTaskCountDic.Add(orderType,0);
            Storage.MaxCount += totalCoin;
        }

        for (var i = 0; i < curTaskList.Count; i++)
        {
            var curTask = curTaskList[i];
            OnCreateTask(curTask);
        }
        EventDispatcher.Instance.SendEventImmediately(new EventDogPlayNewLevel());
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventKeepPetDogPlayPop,
            curGroup.Id.ToString(),Storage.CurConfigId.ToString(),Storage.Rounds.ToString());
        MergeDogPlay.Instance?.RefreshView();
    }

    public void OnCompleteTask(StorageTaskItem task)
    {
        if (Storage.OrderActiveState.TryGetValue(task.Id,out var propCount))
        {
            Storage.CurCount += propCount;
            Storage.CurTaskCountDic[task.Type]++;
            Storage.OrderActiveState.Remove(task.Id);
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventKeepPetDogCoinGet,
                propCount.ToString(),Storage.CurConfigId.ToString(),Storage.Rounds.ToString());
            if (Storage.CurCount >= Storage.MaxCount)
            {
                DogPlayExtraRewardModel.Instance.OnDogPlayOrderCompleted(Storage.CurConfigId);
            }
        }
    }

    public void OnCreateTaskEvent(EventCreateMergeOrder evt)
    {
        if (!LastOpenState)
            return;
        OnCreateTask(evt.OrderItem);
    }

    public void OnCompleteTaskEvent(EventCompleteTask evt)
    {
        if (!LastOpenState)
            return;
        OnCompleteTask(evt.TaskItem);
    }

    public void OnCreateTask(StorageTaskItem task)
    {
        if (Storage.MaxTaskCountDic.TryGetValue(task.Type,out var maxCount) && maxCount > 0)
        {
            var curCount = GetActiveTaskCount(task.Type) + Storage.CurTaskCountDic[task.Type];
            if (curCount < maxCount)//在任务上挂载道具
            {
                Storage.OrderActiveState.Add(task.Id,Storage.OrderCanProductCount[task.Type]);
            }
        }
    }

    public int GetActiveTaskCount(int orderType)
    {
        var curTaskList = MainOrderManager.Instance.CurTaskList;
        var keys = Storage.OrderActiveState.Keys.ToList();
        var count = 0;
        for (var j = 0; j < keys.Count; j++)
        {
            var task = curTaskList.Find(a => a.Id == keys[j]);
            if (task != null && task.Type == orderType)
            {
                count++;
            }
        }
        return count;
    }

    public int GetPropCount(StorageTaskItem task)
    {
        if (Storage.OrderActiveState.TryGetValue(task.Id, out var count))
            return count;
        return 0;
    }



    public Transform GetDogFrisbeeFlyTarget()
    {
        return GetCommonFlyTarget();
    }

    public Transform GetDogDrumstickFlyTarget()
    {
        return GetCommonFlyTarget();
    }

    public Transform GetDogSteakFlyTarget()
    {
        return GetCommonFlyTarget();
    }

    public Transform GetDogHeadFlyTarget()
    {
        return GetCommonFlyTarget();
    }

    public Transform GetCommonFlyTarget()
    {
        if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
        {
            var entrance = GetMergeEntrance();
            if (entrance)
                return entrance.transform;
            else
                return MergeMainController.Instance.rewardBtnTrans;
        }
        else
        {
            return UIHomeMainController.mainController.MainPlayTransform;
        }
    }
    public MergeDogPlay GetMergeEntrance()
    {
        return MergeDogPlay.Instance;
    }

    public void OpenMainPopup()
    {
        if (DogPlayExtraRewardModel.Instance.IsOpened() && DogPlayExtraRewardModel.Instance.GetExtraRewards(Storage).Count > 0)
        {
            UIPopupDogPlayExtraRewardController.Open();
        }
        else
        {
            UIPopupDogPlayController.Open();   
        }
    }

    public void CheckOrderState()
    {
        if (Storage.OrderActiveState.Count == 0)
        {
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventDogPlayNoItemOnOrder);
        }
    }
}