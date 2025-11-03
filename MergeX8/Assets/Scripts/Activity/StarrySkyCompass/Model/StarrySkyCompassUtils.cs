using System;
using System.Collections.Generic;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using UnityEngine;

public static partial class StarrySkyCompassUtils
{
    #region 通用时间工具组
    public static bool IsActive(this StorageStarrySkyCompass storageWeek)
    {
        return storageWeek.GetStartTime() <= 0 && storageWeek.GetLeftTime() > 0 && storageWeek.GetPreheatTime() <= 0;
    }
    public static bool IsTimeOut(this StorageStarrySkyCompass storageWeek)
    {
        return storageWeek.GetLeftTime() <= 0;
    }
    public static long GetLeftTime(this StorageStarrySkyCompass storageWeek)
    {
        return Math.Max(storageWeek.EndTime - (long) APIManager.Instance.GetServerTime(), 0);
    }
    public static void SetLeftTime(this StorageStarrySkyCompass storageWeek,long leftTime)
    {
        storageWeek.EndTime = (long) APIManager.Instance.GetServerTime() + leftTime;
    }
    public static string GetLeftTimeText(this StorageStarrySkyCompass storageWeek)
    {
        return CommonUtils.FormatLongToTimeStr(storageWeek.GetLeftTime());
    }
    public static long GetStartTime(this StorageStarrySkyCompass storageWeek)
    {
        return Math.Max(storageWeek.StartTime - (long) APIManager.Instance.GetServerTime(), 0);
    }
    public static void SetStartTime(this StorageStarrySkyCompass storageWeek,long leftTime)
    {
        storageWeek.StartTime = (long) APIManager.Instance.GetServerTime() + leftTime;
    }
    public static long GetPreheatTime(this StorageStarrySkyCompass storageWeek)
    {
        return Math.Max(storageWeek.PreheatTime - (long) APIManager.Instance.GetServerTime(), 0);
    }
    public static void SetPreheatTime(this StorageStarrySkyCompass storageWeek,long leftTime)
    {
        storageWeek.PreheatTime = (long) APIManager.Instance.GetServerTime() + leftTime;
    }
    public static string GetPreheatTimeText(this StorageStarrySkyCompass storageWeek)
    {
        return CommonUtils.FormatLongToTimeStr(storageWeek.GetPreheatTime());
    }
    #endregion
    
    public static bool ShowAuxItem(this StorageStarrySkyCompass storage)
    {
        if (!StarrySkyCompassModel.Instance.IsOpenPrivate())
            return false;
        if (storage.IsTimeOut())
            return false;
        return true;
    }
    
    public static bool ShowTaskEntrance(this StorageStarrySkyCompass storage)
    {
        if (!StarrySkyCompassModel.Instance.IsStart())
            return false;
        if (storage.IsTimeOut())
            return false;
        return true;
    }
    
    public static string GetAuxItemAssetPath(this StorageStarrySkyCompass storage)
    {
        return "Prefabs/Activity/StarrySkyCompass/Aux_StarrySkyCompass";
    }
    public static string GetTaskEntranceAssetPath(this StorageStarrySkyCompass storage)
    {
        return "Prefabs/Activity/StarrySkyCompass/TaskList_StarrySkyCompass";
    }

    public static bool IsInHappyTime(this StorageStarrySkyCompass storage)
    {
        return storage.GetHappyTime() > 0;
    }
    public static long GetHappyTime(this StorageStarrySkyCompass storageWeek)
    {
        return Math.Max(storageWeek.HappyEndTime - (long) APIManager.Instance.GetServerTime(), 0);
    }
    public static void SetHappyTime(this StorageStarrySkyCompass storageWeek,long leftTime)
    {
        storageWeek.HappyEndTime = (long) APIManager.Instance.GetServerTime() + leftTime;
    }
    public static string GetHappyTimeText(this StorageStarrySkyCompass storageWeek)
    {
        return CommonUtils.FormatLongToTimeStr(storageWeek.GetHappyTime());
    }

    public static void SetHappyTime(this StorageStarrySkyCompass storage,int seconds)
    {
        storage.HappyEndTime = (long)APIManager.Instance.GetServerTime() + seconds * (long)XUtility.Second;
    }
}