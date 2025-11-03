using System;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;

public static class CardCollectionActivityUtils
{
    public static bool IsTimeOut(this StorageCardCollectionActivity storageWeek)
    {
        return storageWeek.GetLeftTime() <= 0;
    }
    public static long GetLeftTime(this StorageCardCollectionActivity storageWeek)
    {
        return Math.Max(storageWeek.EndTime - (long) APIManager.Instance.GetServerTime(), 0);
    }
    public static void SetLeftTime(this StorageCardCollectionActivity storageWeek,long leftTime)
    {
        var endTime = (long) APIManager.Instance.GetServerTime() + leftTime;
        storageWeek.EndTime = endTime;
    }
    public static string GetLeftTimeText(this StorageCardCollectionActivity storageWeek)
    {
        return CommonUtils.FormatLongToTimeStr(storageWeek.GetLeftTime());
    }
}