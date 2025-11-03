using System;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.KeepPet;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;

public partial class KeepPetModel
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
    public List<KeepPetDailyTaskConfig> DailyTaskConfig;
    private Dictionary<int, KeepPetDailyTaskConfig> DailyTaskConfigDic;
    public Dictionary<int, List<int>> CollectTypeTaskList =
        new Dictionary<int, List<int>>();
    public void InitDailyTaskConfig()
    {
        DailyTaskConfig = KeepPetModel.Instance.KeepPetDailyTaskConfig();
        DailyTaskConfigDic = new Dictionary<int, KeepPetDailyTaskConfig>();
        foreach (var taskConfig in DailyTaskConfig)
        {
            DailyTaskConfigDic.Add(taskConfig.Id, taskConfig);
        }
        var storage = StorageDailyTask;//用于初始化Storage,避免在刷新任务状态中循环调用
        RefreshTaskState();
    }

    public void RefreshTaskState()
    {
        CollectTypeTaskList.Clear();
        foreach (var taskConfig in DailyTaskConfig)
        {
            if (StorageDailyTask.AlreadyCollectLevels.Contains(taskConfig.Id))
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
    public long GetDailyTaskRefreshTime()
    {
        var curTime = APIManager.Instance.GetServerTime();
        var offset = (ulong)GlobalConfig.DailyTaskRefreshTimeOffset * XUtility.Hour;
        var curDay =(curTime-offset) / XUtility.DayTime;
        var refreshTime = (long)((curDay + 1) * XUtility.DayTime + offset);
        return refreshTime;
    }
    public StorageKeepPetDailyTask StorageDailyTask
    {
        get
        {
            var refreshTime = GetDailyTaskRefreshTime();
            if (!Storage.DailyTaskDictionary.TryGetValue(refreshTime, out var dailyTaskStorage))
            {
                Storage.PayLevelGroup = PayLevelModel.Instance.GetCurPayLevelConfig().KeepPetGroupId;
                
                dailyTaskStorage = new StorageKeepPetDailyTask();
                Storage.DailyTaskDictionary.Add(refreshTime,dailyTaskStorage);
                InitDailyTaskConfig();
            }
            return dailyTaskStorage;
        }
    }

    public bool CanShowUnCollectReward()
    {
        if (GlobalConfig == null)
            return false;
        var rewards = CheckUnCollectDailyTaskRewards();
        if (rewards != null && rewards.Count > 0)
        {
            var reasonArgs = new GameBIManager.ItemChangeReasonArgs(){reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.KeepPetDailyTaskGet};
            CommonRewardManager.Instance.PopCommonReward(rewards, CurrencyGroupManager.Instance.currencyController,
                true, reasonArgs);
            // CommonRewardManager.Instance.PopActivityUnCollectReward(rewards, reasonArgs, null);
            return true;
        }
        else
        {
            return false;
        }
    }
    public List<ResData> CheckUnCollectDailyTaskRewards()//获取所有未及时领取的最终奖励,并清理存档
    {
        var unCollectRewards = new List<ResData>();
        var refreshTime = GetDailyTaskRefreshTime();
        var removeKeyList = new List<long>();
        foreach (var pair in Storage.DailyTaskDictionary)
        {
            if (pair.Key == refreshTime)
                continue;
            removeKeyList.Add(pair.Key);
            var storage = pair.Value;
            if (!storage.IsCollectFinalReward)
            {
                unCollectRewards.AddRange(CommonUtils.FormatReward(storage.UnCollectRewards));
            }
        }
        foreach (var key in removeKeyList)
        {
            Storage.DailyTaskDictionary.Remove(key);
        }
        return unCollectRewards;
    }

    public bool CollectFinalReward()//领取当天的大奖
    {
        if (!StorageDailyTask.IsCollectFinalReward && Level >= GlobalConfig.DailyTaskFinalRewardNeedTaskCount)
        {
            var reward = FinialRewards;
            var reason =
                new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason
                    .KeepPetDailyTaskGet);
            UserData.Instance.AddRes(reward,reason);
            CommonRewardManager.Instance.PopCommonReward(reward, CurrencyGroupManager.Instance.currencyController,
                false, reason);
            ReduceUnCollectRewards(reward);
            StorageDailyTask.IsCollectFinalReward = true;
            EventDispatcher.Instance.SendEventImmediately(new EventKeepPetCollectFinalDailyTaskReward());
            return true;
        }
        return false;
    }

    public void ReduceUnCollectRewards(List<ResData> rewards)
    {
        foreach (var reward in rewards)
        {
            if (StorageDailyTask.UnCollectRewards.ContainsKey(reward.id))
            {
                StorageDailyTask.UnCollectRewards[reward.id] -= reward.count;
                if (StorageDailyTask.UnCollectRewards[reward.id] <= 0)
                {
                    StorageDailyTask.UnCollectRewards.Remove(reward.id);
                }
            }
        }
    }

    public void AddUnCollectRewards(List<ResData> rewards)
    {
        foreach (var reward in rewards)
        {
            StorageDailyTask.UnCollectRewards.TryAdd(reward.id, 0);
            StorageDailyTask.UnCollectRewards[reward.id] += reward.count;
        }
    }

    private int lastAddValue;
    public void OnFlashSale()
    {
        if (!IsOpen())
            return;
        var collectType = (int) DailyTaskSpecialCollectType.FlashSale;
        StorageDailyTask.SpecialTargetState.TryAdd(collectType, 0);
        StorageDailyTask.SpecialTargetState[collectType] += 1;
        lastAddValue = 1;
        CheckTaskComplete(collectType);
    }
    public void OnBreakBubble()
    {
        if (!IsOpen())
            return;
        var collectType = (int) DailyTaskSpecialCollectType.Bubble;
        StorageDailyTask.SpecialTargetState.TryAdd(collectType, 0);
        StorageDailyTask.SpecialTargetState[collectType] += 1;
        lastAddValue = 1;
        CheckTaskComplete(collectType);
    }
    public void OnGetCoin(int count)
    {
        if (!IsOpen())
            return;
        var collectType = (int) DailyTaskSpecialCollectType.GetCoin;
        StorageDailyTask.SpecialTargetState.TryAdd(collectType, 0);
        StorageDailyTask.SpecialTargetState[collectType] += count;
        lastAddValue = count;
        CheckTaskComplete(collectType);
    }
    public void OnAddRes(UserData.ResourceId resId, int count)
    {
        if (!IsOpen())
            return;
        var collectType = (int) resId;
        StorageDailyTask.TargetCollectState.TryAdd(collectType, 0);
        StorageDailyTask.TargetCollectState[collectType] += count;
        lastAddValue = count;
        CheckTaskComplete(collectType);
    }
    public void OnConsumeRes(UserData.ResourceId resId, int count)
    {
        if (!IsOpen())
            return;
        var collectType = (int) resId;
        StorageDailyTask.TargetConsumeState.TryAdd(collectType, 0);
        StorageDailyTask.TargetConsumeState[collectType] += count;
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
                    if (Level == GlobalConfig.DailyTaskFinalRewardNeedTaskCount && !StorageDailyTask.IsCollectFinalReward)
                    {
                        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventKeepPetDailytaskClear);
                        AddUnCollectRewards(FinialRewards);
                    }
                }
            }
        }
    }

    public int GetTaskCollectCount(KeepPetDailyTaskConfig taskConfig)
    {
        if (!IsOpen())
            return 0;
        if (!IsDailyTaskUnlock(taskConfig))
            return 0;
        var collectType = taskConfig.CollectType;
        var collectCount = 0;
        if ((DailyTaskTargetType) taskConfig.TargetType == DailyTaskTargetType.Add)
        {
            collectCount = StorageDailyTask.TargetCollectState.TryGetValue(collectType, out var count) ? count : 0;
        }
        else if ((DailyTaskTargetType) taskConfig.TargetType == DailyTaskTargetType.Consume)
        {
            collectCount = StorageDailyTask.TargetConsumeState.TryGetValue(collectType, out var count) ? count : 0;
        }
        else if ((DailyTaskTargetType) taskConfig.TargetType == DailyTaskTargetType.Special)
        {
            collectCount = StorageDailyTask.SpecialTargetState.TryGetValue(collectType, out var count) ? count : 0;
        }
        if (StorageDailyTask.UnlockReduceValue.TryGetValue(taskConfig.Id, out var reduceValue))
            collectCount -= reduceValue;
        return collectCount;
    }

    public bool IsDailyTaskUnlock(KeepPetDailyTaskConfig taskConfig)
    {
        var curLevel = Storage.Exp.KeepPetGetCurLevelConfig();
        return curLevel.Id >= taskConfig.UnLockLevel;
    }

    public bool IsCollectTask(KeepPetDailyTaskConfig taskConfig)
    {
        if (!IsOpen())
            return false;
        if (StorageDailyTask.AlreadyCollectLevels.Contains(taskConfig.Id))
            return true;
        return false;
    }

    public KeepPetDailyTaskConfig GetNextUnlockNewDailyTaskLevel()
    {
        KeepPetDailyTaskConfig level = null;
        var curLevel = Storage.Exp.KeepPetGetCurLevelConfig();
        foreach (var task in DailyTaskConfig)
        {
            if (task.UnLockLevel > curLevel.Id)
            {
                if (level == null)
                    level = task;
                else
                {
                    if (level.UnLockLevel > task.UnLockLevel)
                        level = task;
                }
            }
        }
        return level;
    }
    public void CollectDailyTask(KeepPetDailyTaskConfig task)
    {
        if (!IsDailyTaskUnlock(task))
            return;
        StorageDailyTask.CanCollectLevels.Remove(task.Id);
        StorageDailyTask.AlreadyCollectLevels.Add(task.Id);
        var rewards = CommonUtils.FormatReward(task.RewardId, task.RewardNum);
        var reason = new GameBIManager.ItemChangeReasonArgs()
        {
            reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.KeepPetDailyTaskGet
        };
        UserData.Instance.AddRes(rewards,reason);
        if (KeepPetTurkeyModel.Instance.IsInitFromServer())
        {
            KeepPetTurkeyModel.Instance.Storage.AddScore(task.GetTurkeyScore(),"KeepPetDailyTask",true);   
        }
        ReduceUnCollectRewards(rewards);
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventKeepPetDailytaskFinish, task.Id.ToString(),Level+"/"+MaxLevel);
        EventDispatcher.Instance.SendEventImmediately(new EventKeepPetCollectDailyTaskReward());
    }
    public void CompletedDailyTask(KeepPetDailyTaskConfig task)
    {
        CollectTypeTaskList[task.CollectType].Remove(task.Id);
        StorageDailyTask.CanCollectLevels.Add(task.Id);
        var curLevelConfig = Storage.Exp.KeepPetGetCurLevelConfig();
        if (curLevelConfig.Id < task.UnLockLevel)
            return;
        var rewards = CommonUtils.FormatReward(task.RewardId, task.RewardNum);
        AddUnCollectRewards(rewards);
        UIPopupKeepPetTaskCompletedController.PushCompletedTask(task,Math.Max(0,task.CollectCount-lastAddValue),task.CollectCount);
        CollectDailyTask(task);
    }

    public void UpdateDailyTaskOnExpChange(int oldValue,int newValue)
    {
        var oldLevel = oldValue.KeepPetGetCurLevelConfig();
        var newLevel = newValue.KeepPetGetCurLevelConfig();
        if (oldLevel.Id != newLevel.Id)
        {
            foreach (var task in DailyTaskConfig)
            {
                if (task.UnLockLevel == newLevel.Id)
                {
                    var alreadyCollectCount = GetTaskCollectCount(task);
                    if (alreadyCollectCount > task.UnLockMaxValue)
                    {
                        StorageDailyTask.UnlockReduceValue.Add(task.Id,alreadyCollectCount - task.UnLockMaxValue);
                    }
                }
            }
        }
    }

    public int CanCollectLevelCount
    {
        get
        {
            var canCollectCount = 0;
            var levelConfig = Storage.Exp.KeepPetGetCurLevelConfig();
            foreach (var level in StorageDailyTask.CanCollectLevels)
            {
                var config = DailyTaskConfigDic[level];
                if (levelConfig.Id >= config.UnLockLevel)
                {
                    canCollectCount++;
                }
            }
            return canCollectCount;
        }
    }
    public int Level => StorageDailyTask.AlreadyCollectLevels.Count+CanCollectLevelCount;
    public int MaxLevel
    {
        get
        {
            var unlockCount = 0;
            var levelConfig = Storage.Exp.KeepPetGetCurLevelConfig();
            foreach (var config in DailyTaskConfig)
            {
                if (levelConfig.Id >= config.UnLockLevel)
                {
                    unlockCount++;
                }
            }
            return unlockCount;
        }
    }

    public List<ResData> FinialRewards =>
        CommonUtils.FormatReward(GlobalConfig.DailyTaskFinalRewardId, GlobalConfig.DailyTaskFinalRewardNum);
    
}
