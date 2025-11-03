using System;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;

public static class CardCollectionReopenActivityUtils
{
    public static bool IsTimeOut(this StorageCardCollectionReopenActivity storageWeek)
    {
        return storageWeek.GetLeftTime() <= 0;
    }
    public static long GetLeftTime(this StorageCardCollectionReopenActivity storageWeek)
    {
        return Math.Max(storageWeek.EndTime - (long) APIManager.Instance.GetServerTime(), 0);
    }
    public static void SetLeftTime(this StorageCardCollectionReopenActivity storageWeek,long leftTime)
    {
        var endTime = (long) APIManager.Instance.GetServerTime() + leftTime;
        storageWeek.EndTime = endTime;
    }
    public static string GetLeftTimeText(this StorageCardCollectionReopenActivity storageWeek)
    {
        return CommonUtils.FormatLongToTimeStr(storageWeek.GetLeftTime());
    }
}