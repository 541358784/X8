using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Decoration;
using DragonPlus;
using DragonPlus.Config.DogHope;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

public enum DogHopeStoreItemType
{
    MergeItem = 1,
    BuildItem = 2,
}
public static class DogHopeUtils
{
    
    public static bool IsTimeOut(this StorageDogHope storageWeek)
    {
        return storageWeek.GetLeftTime() <= 0;
    }
    public static long GetLeftTime(this StorageDogHope storageWeek)
    {
        return Math.Max(storageWeek.EndTime - (long) APIManager.Instance.GetServerTime(), 0);
    }
    public static void SetLeftTime(this StorageDogHope storageWeek,long leftTime)
    {
        var endTime = (long) APIManager.Instance.GetServerTime() + leftTime;
        storageWeek.EndTime = endTime;
        storageWeek.LeaderBoardStorage.EndTime = endTime;
    }
    public static string GetLeftTimeText(this StorageDogHope storageWeek)
    {
        return CommonUtils.FormatLongToTimeStr(storageWeek.GetLeftTime());
    }

    public static void CompletedStorageActivity(this StorageDogHopeLeaderBoard leadBoardStorage)
    {
        leadBoardStorage.IsFinish = true;
        foreach (var pair in DogHopeModel.StorageDogHope)
        {
            if (pair.Value.LeaderBoardStorage == leadBoardStorage)
            {
                if (pair.Value.TryRelease())
                    DogHopeModel.Instance.CreateStorage();
                return;
            }
        }
    }

    public static bool TryRelease(this StorageDogHope storage)
    {
        if (storage.IsTimeOut() && (!storage.LeaderBoardStorage.IsInitFromServer() || storage.LeaderBoardStorage.IsFinish))
        {
            Debug.LogError("删除ActivityId = "+storage.ActivityId+"排行榜数据");
            DogHopeModel.StorageDogHope.Remove(storage.ActivityId);
            DogHopeLeaderBoardUtils.StorageWeekInitStateDictionary.Remove(storage.LeaderBoardStorage.ActivityId);
            return true;
        }
        return false;
    }

}