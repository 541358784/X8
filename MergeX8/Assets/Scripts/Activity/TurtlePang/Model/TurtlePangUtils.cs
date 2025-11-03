using System;
using System.Collections.Generic;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using UnityEngine;

public static partial class TurtlePangUtils
{
    #region 通用时间工具组
    public static bool IsActive(this StorageTurtlePang storageWeek)
    {
        return storageWeek.GetStartTime() <= 0 && storageWeek.GetLeftTime() > 0 && storageWeek.GetPreheatTime() <= 0;
    }
    public static bool IsTimeOut(this StorageTurtlePang storageWeek)
    {
        return storageWeek.GetLeftTime() <= 0;
    }
    public static long GetLeftTime(this StorageTurtlePang storageWeek)
    {
        return Math.Max(storageWeek.EndTime - (long) APIManager.Instance.GetServerTime(), 0);
    }
    public static void SetLeftTime(this StorageTurtlePang storageWeek,long leftTime)
    {
        storageWeek.EndTime = (long) APIManager.Instance.GetServerTime() + leftTime;
    }
    public static string GetLeftTimeText(this StorageTurtlePang storageWeek)
    {
        return CommonUtils.FormatLongToTimeStr(storageWeek.GetLeftTime());
    }
    public static long GetStartTime(this StorageTurtlePang storageWeek)
    {
        return Math.Max(storageWeek.StartTime - (long) APIManager.Instance.GetServerTime(), 0);
    }
    public static void SetStartTime(this StorageTurtlePang storageWeek,long leftTime)
    {
        storageWeek.StartTime = (long) APIManager.Instance.GetServerTime() + leftTime;
    }
    public static long GetPreheatTime(this StorageTurtlePang storageWeek)
    {
        return Math.Max(storageWeek.PreheatTime - (long) APIManager.Instance.GetServerTime(), 0);
    }
    public static void SetPreheatTime(this StorageTurtlePang storageWeek,long leftTime)
    {
        storageWeek.PreheatTime = (long) APIManager.Instance.GetServerTime() + leftTime;
    }
    public static string GetPreheatTimeText(this StorageTurtlePang storageWeek)
    {
        return CommonUtils.FormatLongToTimeStr(storageWeek.GetPreheatTime());
    }
    #endregion
    
    public static bool ShowAuxItem(this StorageTurtlePang storage)
    {
        if (!TurtlePangModel.Instance.IsOpenPrivate())
            return false;
        if (storage.IsTimeOut())
            return false;
        return true;
    }
    
    public static bool ShowTaskEntrance(this StorageTurtlePang storage)
    {
        if (!TurtlePangModel.Instance.IsOpenPrivate())
            return false;
        if (storage.GetPreheatTime() > 0)
            return false;
        if (storage.IsTimeOut())
            return false;
        return true;
    }
    
    public static string GetAuxItemAssetPath(this StorageTurtlePang storage)
    {
        return "Prefabs/Activity/TurtlePang/Aux_TurtlePang";
    }
    public static string GetTaskEntranceAssetPath(this StorageTurtlePang storage)
    {
        return "Prefabs/Activity/TurtlePang/TaskList_TurtlePang";
    }

    public static Sprite GetTurtleIcon(this TurtlePangItemConfig itemConfig)
    {
        return ResourcesManager.Instance.GetSpriteVariant("TurtlePangAtlas", itemConfig.Image);
    }
}