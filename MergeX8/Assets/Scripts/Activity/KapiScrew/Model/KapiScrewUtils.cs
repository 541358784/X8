using System;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;

public enum KapiScrewStoreItemType
{
    MergeItem = 1,
    BuildItem = 2,
}

public static class KapiScrewUtils
{
    public static KapiScrewModel Model => KapiScrewModel.Instance;

    public static bool IsTimeOut(this StorageKapiScrew storageWeek)
    {
        return storageWeek.GetLeftTime() <= 0;
    }

    public static long GetLeftTime(this StorageKapiScrew storageWeek)
    {
        return Math.Max(storageWeek.EndTime - (long) APIManager.Instance.GetServerTime(), 0);
    }

    public static void SetLeftTime(this StorageKapiScrew storageWeek, long leftTime)
    {
        var endTime = (long) APIManager.Instance.GetServerTime() + leftTime;
        storageWeek.EndTime = endTime;
    }

    public static string GetLeftTimeText(this StorageKapiScrew storageWeek)
    {
        return CommonUtils.FormatLongToTimeStr(storageWeek.GetLeftTime());
    }
    
    public static long GetPreheatLeftTime(this StorageKapiScrew storage)
    {
        var heatTime = storage.PreheatCompleteTime - (long) APIManager.Instance.GetServerTime();
        heatTime = Math.Max(heatTime, 0);
        return heatTime;
    }

    public static void SetPreheatLeftTime(this StorageKapiScrew storageWeek, long leftTime)
    {
        storageWeek.PreheatCompleteTime = (long) APIManager.Instance.GetServerTime() + leftTime;
    }

    public static string GetPreheatLeftTimeText(this StorageKapiScrew storage)
    {
        return CommonUtils.FormatLongToTimeStr(storage.GetPreheatLeftTime());
    }








    #region Entrance
    public static bool ShowAuxItem(this StorageKapiScrew storage)
    {
        if (!KapiScrewModel.Instance.IsOpened())
            return false;
        if (storage.IsTimeOut())
            return false;
        if (KapiScrewModel.Instance.IsFinished())
            return false;
        return true;
    }
    public static bool ShowTaskEntrance(this StorageKapiScrew storage)
    {
        if (!KapiScrewModel.Instance.IsOpened())
            return false;
        if (KapiScrewModel.Instance.Storage.GetPreheatLeftTime() > 0)
            return false;
        if (storage.IsTimeOut())
            return false;
        if (KapiScrewModel.Instance.IsFinished())
            return false;
        return true;
    }

    public static string GetAuxItemAssetPath(this StorageKapiScrew storage)
    {
        return "Prefabs/Activity/KapiScrew/Aux_KapibalaContest";
    }

    public static string GetTaskItemAssetPath(this StorageKapiScrew storage)
    {
        return "Prefabs/Activity/KapiScrew/TaskList_KapibalaContest";
    }
    #endregion
}