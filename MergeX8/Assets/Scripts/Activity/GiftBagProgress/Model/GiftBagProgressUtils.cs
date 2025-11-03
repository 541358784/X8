using System;
using System.Collections.Generic;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;

public static class GiftBagProgressUtils
{
    public static void AddUnCollectRewards(this StorageGiftBagProgress storage,List<ResData> rewards)
    {
        if (storage == null)
            return;
        foreach (var reward in rewards)
        {
            storage.UnCollectRewards.TryAdd(reward.id, 0);
            storage.UnCollectRewards[reward.id] += reward.count;
        }
    }

    public static void ReduceUnCollectRewards(this StorageGiftBagProgress storage, List<ResData> rewards)
    {
        if (storage == null)
            return;
        foreach (var reward in rewards)
        {
            if (storage.UnCollectRewards.ContainsKey(reward.id))
            {
                storage.UnCollectRewards[reward.id] -= reward.count;
                if (storage.UnCollectRewards[reward.id] <= 0)
                {
                    storage.UnCollectRewards.Remove(reward.id);
                }
            }
        }
    }
    public static bool IsFinish(this StorageGiftBagProgress storage)
    {
        if (storage == null)
            return true;
        if (!GiftBagProgressModel.Instance.IsInitFromServer())
        {
            return true;
        }
        var taskConfig = GiftBagProgressModel.Instance.DailyTaskConfig;
        for (var i = 0; i < taskConfig.Count; i++)
        {
            if (!storage.AlreadyCollectLevels.Contains(taskConfig[i].Id))
                return false;
        }
        return true;
    }

    #region 通用时间工具组
    public static bool IsActive(this StorageGiftBagProgress storageWeek)
    {
        return storageWeek.GetStartTime() <= 0 && storageWeek.GetLeftTime() > 0;
    }
    public static bool IsTimeOut(this StorageGiftBagProgress storageWeek)
    {
        return storageWeek.GetLeftTime() <= 0;
    }
    public static long GetLeftTime(this StorageGiftBagProgress storageWeek)
    {
        return Math.Max(storageWeek.EndTime - (long) APIManager.Instance.GetServerTime(), 0);
    }
    public static void SetLeftTime(this StorageGiftBagProgress storageWeek,long leftTime)
    {
        storageWeek.EndTime = (long) APIManager.Instance.GetServerTime() + leftTime;
    }
    public static string GetLeftTimeText(this StorageGiftBagProgress storageWeek)
    {
        return CommonUtils.FormatLongToTimeStr(storageWeek.GetLeftTime());
    }
    public static long GetStartTime(this StorageGiftBagProgress storageWeek)
    {
        return Math.Max(storageWeek.StartTime - (long) APIManager.Instance.GetServerTime(), 0);
    }
    public static void SetStartTime(this StorageGiftBagProgress storageWeek,long leftTime)
    {
        storageWeek.StartTime = (long) APIManager.Instance.GetServerTime() + leftTime;
    }
    #endregion
    
    public static bool ShowAuxItem(this StorageGiftBagProgress storage)
    {
        if (!GiftBagProgressModel.Instance.IsOpenPrivate())
            return false;
        if (storage.IsTimeOut())
            return false;
        return true;
    }
    
    public static bool ShowTaskEntrance(this StorageGiftBagProgress storage)
    {
        if (!GiftBagProgressModel.Instance.IsOpenPrivate())
            return false;
        if (storage.IsTimeOut())
            return false;
        if (storage.BuyState && storage.CanCollectLevels.Count > 0)
            return true;
        if (!storage.BuyState && storage.CanCollectLevels.Count >=
            GiftBagProgressModel.Instance.GlobalConfig.TaskCompleteShowCount)
            return true;
        return false;
    }
    
    public static bool ShowMergeBubble(this StorageGiftBagProgress storage)
    {
        if (!GiftBagProgressModel.Instance.IsOpenPrivate())
            return false;
        if (storage.IsTimeOut())
            return false;
        return true;
    }

    
    public static MergeGiftBagProgressBubble GetMergeBubble(this StorageGiftBagProgress storage)
    {
        if (storage == null)
            return null;
        if (MergeGiftBagProgressBubble_Creator.CreatorDic.TryGetValue(storage, out var creator))
        {
            if (creator.TaskEntrance)
                return creator.TaskEntrance;
            var auxItem = creator.CreateEntrance();
            if (auxItem)
                return auxItem as MergeGiftBagProgressBubble;
        }

        return null;
    }
    
    public static void CreateMergeBubble(this StorageGiftBagProgress storage)
    {
        if (storage == null)
            return;
        if (!MergeGiftBagProgressBubble_Creator.CreatorDic.ContainsKey(storage))
        {
            var creator = new MergeGiftBagProgressBubble_Creator(storage);
        }
    }

    public static string GetAuxItemAssetPath(this StorageGiftBagProgress storage)
    {
        return "Prefabs/Activity/GiftBagProgress/Aux_GiftBagProgress";
    }

    public static string GetTaskItemAssetPath(this StorageGiftBagProgress storage)
    {
        return "Prefabs/Activity/GiftBagProgress/TaskList_GiftBagProgress";
    }
    
    public static string GetMergeBubbleAssetPath(this StorageGiftBagProgress storage)
    {
        return "Prefabs/Activity/GiftBagProgress/GiftBagProogress";
    }
}