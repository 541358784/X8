using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Deco.World;
using Decoration;
using DragonPlus;
using DragonPlus.Config.FishCulture;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Farm.Model;
using Gameplay;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;
using Utils = DragonU3DSDK.Utils;

public enum FishCultureStoreItemType
{
    MergeItem = 1,
    BuildItem = 2,
}

public static class FishCultureUtils
{
    public static FishCultureModel Model => FishCultureModel.Instance;
    public const ulong Offset = 0 * XUtility.Hour;
    public static int CurDay => (int) ((APIManager.Instance.GetServerTime() - Offset) / XUtility.DayTime);

    public static ulong CurDayLeftTime =>
        XUtility.DayTime - ((APIManager.Instance.GetServerTime() - Offset) % XUtility.DayTime);

    public static string CurDayLeftTimeString => CommonUtils.FormatLongToTimeStr((long) CurDayLeftTime);

    public static long GetPreheatLeftTime(this StorageFishCulture storage)
    {
        var heatTime = storage.PreheatCompleteTime - (long) APIManager.Instance.GetServerTime();
        heatTime = Math.Max(heatTime, 0);
        return heatTime;
    }

    public static void SetPreheatLeftTime(this StorageFishCulture storageWeek, long leftTime)
    {
        storageWeek.PreheatCompleteTime = (long) APIManager.Instance.GetServerTime() + leftTime;
    }

    public static string GetPreheatLeftTimeText(this StorageFishCulture storage)
    {
        return CommonUtils.FormatLongToTimeStr(storage.GetPreheatLeftTime());
    }

    public static bool IsTimeOut(this StorageFishCulture storageWeek)
    {
        return storageWeek.GetLeftTime() <= 0;
    }

    public static long GetLeftPreEndTime(this StorageFishCulture storageWeek)
    {
        return Math.Max(storageWeek.PreEndTime - (long) APIManager.Instance.GetServerTime(), 0);
    }
    public static string GetLeftPreEndTimeText(this StorageFishCulture storageWeek)
    {
        return CommonUtils.FormatLongToTimeStr(storageWeek.GetLeftPreEndTime());
    }
    public static void SetLeftPreEndTime(this StorageFishCulture storageWeek, long leftTime)
    {
        var endTime = (long) APIManager.Instance.GetServerTime() + leftTime;
        storageWeek.PreEndTime = endTime;
    }
    
    public static long GetLeftTime(this StorageFishCulture storageWeek)
    {
        return Math.Max(storageWeek.EndTime - (long) APIManager.Instance.GetServerTime(), 0);
    }

    public static void SetLeftTime(this StorageFishCulture storageWeek, long leftTime)
    {
        var endTime = (long) APIManager.Instance.GetServerTime() + leftTime;
        storageWeek.EndTime = endTime;
    }

    public static string GetLeftTimeText(this StorageFishCulture storageWeek)
    {
        return CommonUtils.FormatLongToTimeStr(storageWeek.GetLeftTime());
    }

    
    public static bool ShowAuxItem(this StorageFishCulture storage)
    {
        if (!FishCultureModel.Instance.IsOpened())
            return false;
        if (storage.IsTimeOut())
            return false;
        if (storage.GetLeftPreEndTime() <= 0)
            return false;
        return true;
    }
    public static bool ShowTaskEntrance(this StorageFishCulture storage)
    {
        if (!FishCultureModel.Instance.IsOpened())
            return false;
        if (FishCultureModel.Instance.CurStorageFishCultureWeek.GetPreheatLeftTime() > 0)
            return false;
        if (storage.IsTimeOut())
            return false;
        if (storage.GetLeftPreEndTime() <= 0)
            return false;
        return true;
    }

    public static string GetAuxItemAssetPath(this StorageFishCulture storage)
    {
        return "Prefabs/Activity/FishCulture/Aux_FishCulture";
    }

    public static string GetTaskItemAssetPath(this StorageFishCulture storage)
    {
        return "Prefabs/Activity/FishCulture/TaskList_FishCulture";
    }

    public static string GetFishAssetPath(this FishCultureRewardConfig fishConfig)
    {
        return "Prefabs/Activity/FishCulture/enemy" + fishConfig.Fish;
    }
}