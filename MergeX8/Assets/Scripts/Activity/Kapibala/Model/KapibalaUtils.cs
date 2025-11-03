using System;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;

public enum KapibalaStoreItemType
{
    MergeItem = 1,
    BuildItem = 2,
}

public static class KapibalaUtils
{
    public static KapibalaModel Model => KapibalaModel.Instance;

    public static bool IsTimeOut(this StorageKapibala storageWeek)
    {
        return storageWeek.GetLeftTime() <= 0;
    }

    public static long GetLeftTime(this StorageKapibala storageWeek)
    {
        return Math.Max(storageWeek.EndTime - (long) APIManager.Instance.GetServerTime(), 0);
    }

    public static void SetLeftTime(this StorageKapibala storageWeek, long leftTime)
    {
        var endTime = (long) APIManager.Instance.GetServerTime() + leftTime;
        storageWeek.EndTime = endTime;
    }

    public static string GetLeftTimeText(this StorageKapibala storageWeek)
    {
        return CommonUtils.FormatLongToTimeStr(storageWeek.GetLeftTime());
    }
    
    public static long GetPreheatLeftTime(this StorageKapibala storage)
    {
        var heatTime = storage.PreheatCompleteTime - (long) APIManager.Instance.GetServerTime();
        heatTime = Math.Max(heatTime, 0);
        return heatTime;
    }

    public static void SetPreheatLeftTime(this StorageKapibala storageWeek, long leftTime)
    {
        storageWeek.PreheatCompleteTime = (long) APIManager.Instance.GetServerTime() + leftTime;
    }

    public static string GetPreheatLeftTimeText(this StorageKapibala storage)
    {
        return CommonUtils.FormatLongToTimeStr(storage.GetPreheatLeftTime());
    }








    #region Entrance
    public static bool ShowAuxItem(this StorageKapibala storage)
    {
        if (!KapibalaModel.Instance.IsOpened())
            return false;
        if (storage.IsTimeOut())
            return false;
        if (KapibalaModel.Instance.IsFinished())
            return false;
        return true;
    }
    public static bool ShowTaskEntrance(this StorageKapibala storage)
    {
        if (!KapibalaModel.Instance.IsOpened())
            return false;
        if (KapibalaModel.Instance.Storage.GetPreheatLeftTime() > 0)
            return false;
        if (storage.IsTimeOut())
            return false;
        if (KapibalaModel.Instance.IsFinished())
            return false;
        return true;
    }

    public static string GetAuxItemAssetPath(this StorageKapibala storage)
    {
        return "Prefabs/Activity/Kapibala/Aux_Kapibala";
    }

    public static string GetTaskItemAssetPath(this StorageKapibala storage)
    {
        return "Prefabs/Activity/Kapibala/TaskList_Kapibala";
    }
    #endregion
}