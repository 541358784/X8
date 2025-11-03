using System;
using System.Collections.Generic;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using UnityEngine;

public static class MixMasterUtils
{
    #region 通用时间工具组
    public static bool IsActive(this StorageMixMaster storageWeek)
    {
        return storageWeek.GetStartTime() <= 0 && storageWeek.GetLeftTime() > 0 && storageWeek.GetPreheatTime() <= 0;
    }
    public static bool IsTimeOut(this StorageMixMaster storageWeek)
    {
        return storageWeek.GetLeftTime() <= 0;
    }
    public static long GetLeftTime(this StorageMixMaster storageWeek)
    {
        return Math.Max(storageWeek.EndTime - (long) APIManager.Instance.GetServerTime(), 0);
    }
    public static void SetLeftTime(this StorageMixMaster storageWeek,long leftTime)
    {
        storageWeek.EndTime = (long) APIManager.Instance.GetServerTime() + leftTime;
    }
    public static string GetLeftTimeText(this StorageMixMaster storageWeek)
    {
        return CommonUtils.FormatLongToTimeStr(storageWeek.GetLeftTime());
    }
    public static long GetStartTime(this StorageMixMaster storageWeek)
    {
        return Math.Max(storageWeek.StartTime - (long) APIManager.Instance.GetServerTime(), 0);
    }
    public static void SetStartTime(this StorageMixMaster storageWeek,long leftTime)
    {
        storageWeek.StartTime = (long) APIManager.Instance.GetServerTime() + leftTime;
    }
    public static long GetPreheatTime(this StorageMixMaster storageWeek)
    {
        return Math.Max(storageWeek.PreheatTime - (long) APIManager.Instance.GetServerTime(), 0);
    }
    public static void SetPreheatTime(this StorageMixMaster storageWeek,long leftTime)
    {
        storageWeek.PreheatTime = (long) APIManager.Instance.GetServerTime() + leftTime;
    }
    public static string GetPreheatTimeText(this StorageMixMaster storageWeek)
    {
        return CommonUtils.FormatLongToTimeStr(storageWeek.GetPreheatTime());
    }
    #endregion
    
    public static bool ShowAuxItem(this StorageMixMaster storage)
    {
        if (!MixMasterModel.Instance.IsOpenPrivate())
            return false;
        if (storage.IsTimeOut())
            return false;
        return true;
    }
    
    public static bool ShowTaskEntrance(this StorageMixMaster storage)
    {
        if (!MixMasterModel.Instance.IsOpenPrivate())
            return false;
        if (storage.IsTimeOut())
            return false;
        return true;
    }
    
    public static string GetAuxItemAssetPath(this StorageMixMaster storage)
    {
        return "Prefabs/Activity/MixMaster/Aux_MixMaster";
    }
    public static string GetTaskEntranceAssetPath(this StorageMixMaster storage)
    {
        return "Prefabs/Activity/MixMaster/TaskList_MixMaster";
    }

    public static Sprite GetFormulaIcon(this MixMasterFormulaConfig formulaConfig)
    {
        return ResourcesManager.Instance.GetSpriteVariant("MixMasterAtlas", formulaConfig.Image);
    }
}