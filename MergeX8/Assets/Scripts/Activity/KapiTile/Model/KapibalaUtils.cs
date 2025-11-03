using System;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;

public enum KapiTileStoreItemType
{
    MergeItem = 1,
    BuildItem = 2,
}

public static class KapiTileUtils
{
    public static KapiTileModel Model => KapiTileModel.Instance;

    public static bool IsTimeOut(this StorageKapiTile storageWeek)
    {
        return storageWeek.GetLeftTime() <= 0;
    }

    public static long GetLeftTime(this StorageKapiTile storageWeek)
    {
        return Math.Max(storageWeek.EndTime - (long) APIManager.Instance.GetServerTime(), 0);
    }

    public static void SetLeftTime(this StorageKapiTile storageWeek, long leftTime)
    {
        var endTime = (long) APIManager.Instance.GetServerTime() + leftTime;
        storageWeek.EndTime = endTime;
    }

    public static string GetLeftTimeText(this StorageKapiTile storageWeek)
    {
        return CommonUtils.FormatLongToTimeStr(storageWeek.GetLeftTime());
    }
    
    public static long GetPreheatLeftTime(this StorageKapiTile storage)
    {
        var heatTime = storage.PreheatCompleteTime - (long) APIManager.Instance.GetServerTime();
        heatTime = Math.Max(heatTime, 0);
        return heatTime;
    }

    public static void SetPreheatLeftTime(this StorageKapiTile storageWeek, long leftTime)
    {
        storageWeek.PreheatCompleteTime = (long) APIManager.Instance.GetServerTime() + leftTime;
    }

    public static string GetPreheatLeftTimeText(this StorageKapiTile storage)
    {
        return CommonUtils.FormatLongToTimeStr(storage.GetPreheatLeftTime());
    }








    #region Entrance
    public static bool ShowAuxItem(this StorageKapiTile storage)
    {
        if (!KapiTileModel.Instance.IsOpened())
            return false;
        if (storage.IsTimeOut())
            return false;
        if (KapiTileModel.Instance.IsFinished())
            return false;
        return true;
    }
    public static bool ShowTaskEntrance(this StorageKapiTile storage)
    {
        if (!KapiTileModel.Instance.IsOpened())
            return false;
        if (KapiTileModel.Instance.Storage.GetPreheatLeftTime() > 0)
            return false;
        if (storage.IsTimeOut())
            return false;
        if (KapiTileModel.Instance.IsFinished())
            return false;
        return true;
    }

    public static string GetAuxItemAssetPath(this StorageKapiTile storage)
    {
        return "Prefabs/Activity/KapiTile/Aux_KapiTile";
    }

    public static string GetTaskItemAssetPath(this StorageKapiTile storage)
    {
        return "Prefabs/Activity/KapiTile/TaskList_KapiTile";
    }
    #endregion
}