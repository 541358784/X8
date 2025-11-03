using System;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.GiftBagProgress;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;

public partial class GiftBagProgressModel
{
    public void RegisterDailyTaskEvent()
    {
        EventDispatcher.Instance.AddEvent<EventBuyFlashSale>((e) => { OnFlashSale(); });
        EventDispatcher.Instance.AddEvent<EventBreakBubble>((e) => { OnBreakBubble(); });
        EventDispatcher.Instance.AddEventListener(EventEnum.AddCoin, (e) =>  OnGetCoin((int)e.datas[0]));
        EventDispatcher.Instance.AddEventListener(EventEnum.AddRecoverCoinStar, (e) =>  OnGetCoin((int)e.datas[0]));
        EventDispatcher.Instance.AddEvent<EventUserDataAddRes>((e) => { OnAddRes(e.ResId, e.Count); });
        EventDispatcher.Instance.AddEvent<EventUserDataConsumeRes>((e) => { OnConsumeRes(e.ResId, e.Count); });
    }
    public List<GiftBagProgressTaskConfig> DailyTaskConfig;
    private Dictionary<int, GiftBagProgressTaskConfig> DailyTaskConfigDic;
    public Dictionary<int, List<int>> CollectTypeTaskList =
        new Dictionary<int, List<int>>();
    public void InitDailyTaskConfig()
    {
        var allConfig = GiftBagProgressConfigManager.Instance.GetConfig<GiftBagProgressTaskConfig>();
        DailyTaskConfig = new List<GiftBagProgressTaskConfig>();
        DailyTaskConfigDic = new Dictionary<int, GiftBagProgressTaskConfig>();
        foreach (var config in allConfig)
        {
            if (GlobalConfig.Content.Contains(config.Id))
            {
                DailyTaskConfig.Add(config);
                DailyTaskConfigDic.Add(config.Id, config);
            }
        }
        RefreshTaskState();
    }

    public void RefreshTaskState()
    {
        CollectTypeTaskList.Clear();
        foreach (var taskConfig in DailyTaskConfig)
        {
            if (Storage.AlreadyCollectLevels.Contains(taskConfig.Id) || Storage.CanCollectLevels.Contains(taskConfig.Id))
                continue;
            var collectType = taskConfig.CollectType;
            if (!CollectTypeTaskList.TryGetValue(collectType, out var taskList))
            {
                taskList = new List<int>();
                CollectTypeTaskList.Add(collectType,taskList);
            }
            taskList.Add(taskConfig.Id);
        }
    }

    private int lastAddValue;
    public void OnFlashSale()
    {
        if (!IsOpenPrivate())
            return;
        var collectType = (int) DailyTaskSpecialCollectType.FlashSale;
        Storage.SpecialTargetState.TryAdd(collectType, 0);
        Storage.SpecialTargetState[collectType] += 1;
        lastAddValue = 1;
        CheckTaskComplete(collectType);
    }
    public void OnBreakBubble()
    {
        if (!IsOpenPrivate())
            return;
        var collectType = (int) DailyTaskSpecialCollectType.Bubble;
        Storage.SpecialTargetState.TryAdd(collectType, 0);
        Storage.SpecialTargetState[collectType] += 1;
        lastAddValue = 1;
        CheckTaskComplete(collectType);
    }
    public void OnGetCoin(int count)
    {
        if (!IsOpenPrivate())
            return;
        var collectType = (int) DailyTaskSpecialCollectType.GetCoin;
        Storage.SpecialTargetState.TryAdd(collectType, 0);
        Storage.SpecialTargetState[collectType] += count;
        lastAddValue = count;
        CheckTaskComplete(collectType);
    }
    public void OnAddRes(UserData.ResourceId resId, int count)
    {
        if (!IsOpenPrivate())
            return;
        var collectType = (int) resId;
        Storage.TargetCollectState.TryAdd(collectType, 0);
        Storage.TargetCollectState[collectType] += count;
        lastAddValue = count;
        CheckTaskComplete(collectType);
    }
    public void OnConsumeRes(UserData.ResourceId resId, int count)
    {
        if (!IsOpenPrivate())
            return;
        var collectType = (int) resId;
        Storage.TargetConsumeState.TryAdd(collectType, 0);
        Storage.TargetConsumeState[collectType] += count;
        lastAddValue = count;
        CheckTaskComplete(collectType);
    }
    public enum DailyTaskTargetType
    {
        Add = 0,
        Consume = 1,
        Special = 2,
    }
    public enum DailyTaskSpecialCollectType
    {
        Bubble = -1,
        FlashSale = -2,
        GetCoin = -3,
    }

    public void CheckTaskComplete(int resId)
    {
        if (CollectTypeTaskList.TryGetValue(resId, out var taskList))
        {
            for (var i = 0; i < taskList.Count; i++)
            {
                var taskId = taskList[i];
                var taskConfig = DailyTaskConfigDic[taskId];
                if (GetTaskCollectCount(taskConfig) >= taskConfig.CollectCount)
                {
                    CompletedDailyTask(taskConfig);
                    i--;
                }
            }
        }
    }

    public int GetTaskCollectCount(GiftBagProgressTaskConfig taskConfig)
    {
        if (!IsOpenPrivate())
            return 0;
        var collectType = taskConfig.CollectType;
        if ((DailyTaskTargetType) taskConfig.TargetType == DailyTaskTargetType.Add)
        {
            return Storage.TargetCollectState.TryGetValue(collectType, out var count) ? count : 0;
        }
        else if ((DailyTaskTargetType) taskConfig.TargetType == DailyTaskTargetType.Consume)
        {
            return Storage.TargetConsumeState.TryGetValue(collectType, out var count) ? count : 0;
        }
        else if ((DailyTaskTargetType) taskConfig.TargetType == DailyTaskTargetType.Special)
        {
            return Storage.SpecialTargetState.TryGetValue(collectType, out var count) ? count : 0;
        }
        return 0;
    }

    public void CollectTaskReward(GiftBagProgressTaskConfig task)
    {
        if (!Storage.BuyState)
            return;
        Storage.CanCollectLevels.Remove(task.Id);
        Storage.AlreadyCollectLevels.Add(task.Id);
        var rewards = CommonUtils.FormatReward(task.RewardId, task.RewardNum);
        var reason = new GameBIManager.ItemChangeReasonArgs()
        {
            reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.GiftBagProgressGet
        };
        Storage.ReduceUnCollectRewards(rewards);
        UserData.Instance.AddRes(rewards,reason);
        EventDispatcher.Instance.SendEventImmediately(new EventGiftBagProgressCollectTask(Storage, task));
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventGrogresspackReward, task.Id.ToString());
    }
    public void CompletedDailyTask(GiftBagProgressTaskConfig task)
    {
        CollectTypeTaskList[task.CollectType].Remove(task.Id);
        Storage.CanCollectLevels.Add(task.Id);
        var rewards = CommonUtils.FormatReward(task.RewardId, task.RewardNum);
        Storage.AddUnCollectRewards(rewards);
        UIPopupGiftBagProgressTaskCompletedController.PushCompletedTask(Storage,task,Math.Max(0,task.CollectCount-lastAddValue),task.CollectCount);
        EventDispatcher.Instance.SendEventImmediately(new EventGiftBagProgressCompleteTask(Storage, task));
    }
}