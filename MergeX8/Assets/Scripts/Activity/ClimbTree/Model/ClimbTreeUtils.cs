using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Decoration;
using DragonPlus;
using DragonPlus.Config.ClimbTree;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

public enum ClimbTreeStoreItemType
{
    MergeItem = 1,
    BuildItem = 2,
}
public static class ClimbTreeUtils
{
    
    public static bool IsTimeOut(this StorageClimbTree storageWeek)
    {
        return storageWeek.GetLeftTime() <= 0;
    }
    public static long GetLeftTime(this StorageClimbTree storageWeek)
    {
        return Math.Max(storageWeek.EndTime - (long) APIManager.Instance.GetServerTime(), 0);
    }
    public static void SetLeftTime(this StorageClimbTree storageWeek,long leftTime)
    {
        var endTime = (long) APIManager.Instance.GetServerTime() + leftTime;
        storageWeek.EndTime = endTime;
        storageWeek.LeaderBoardStorage.EndTime = endTime;
    }
    public static string GetLeftTimeText(this StorageClimbTree storageWeek)
    {
        return CommonUtils.FormatLongToTimeStr(storageWeek.GetLeftTime());
    }

    public static void CompletedStorageActivity(this StorageClimbTreeLeaderBoard leadBoardStorage)
    {
        leadBoardStorage.IsFinish = true;
        foreach (var pair in ClimbTreeModel.StorageClimbTree)
        {
            if (pair.Value.LeaderBoardStorage == leadBoardStorage)
            {
                if (pair.Value.TryRelease())
                    ClimbTreeModel.Instance.CreateStorage();
                return;
            }
        }
    }

    public static bool TryRelease(this StorageClimbTree storage)
    {
        if (storage.IsTimeOut() && storage.UnCollectRewards.Count == 0 && (!storage.LeaderBoardStorage.IsInitFromServer() || storage.LeaderBoardStorage.IsFinish))
        {
            Debug.LogError("删除ActivityId = "+storage.ActivityId+"排行榜数据");
            ClimbTreeModel.StorageClimbTree.Remove(storage.ActivityId);
            ClimbTreeLeaderBoardUtils.StorageWeekInitStateDictionary.Remove(storage.LeaderBoardStorage.ActivityId);
            return true;
        }
        return false;
    }

}