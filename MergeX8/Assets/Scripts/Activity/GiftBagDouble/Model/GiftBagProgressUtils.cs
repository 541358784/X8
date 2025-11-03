using System;
using System.Collections.Generic;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;

public static class GiftBagDoubleUtils
{
    public static List<GiftBagDoubleProductConfig> GetProductList(this StorageGiftBagDouble storage)
    {
        var list = new List<GiftBagDoubleProductConfig>();
        if (!GiftBagDoubleModel.Instance.IsInitFromServer())
            return list;
        var group = GiftBagDoubleModel.Instance.GroupConfig.Find(a => a.Id == storage.GroupId);
        foreach (var productId in group.ProductList)
        {
            var productConfig = GiftBagDoubleModel.Instance.ProductConfig.Find(a => a.Id == productId);
            list.Add(productConfig);
        }
        return list;
    }

    public static GiftBagDoubleGroupConfig GetGroupConfig(this StorageGiftBagDouble storage)
    {
        if (!GiftBagDoubleModel.Instance.IsInitFromServer())
            return null;
        var group = GiftBagDoubleModel.Instance.GroupConfig.Find(a => a.Id == storage.GroupId);
        return group;
    }

    public static bool IsFinish(this StorageGiftBagDouble storage)
    {
        if (storage == null)
            return true;
        if (storage.IsTimeOut())
            return true;
        if (!GiftBagDoubleModel.Instance.IsInitFromServer())
            return true;
        var productList = storage.GetProductList();
        foreach (var productConfig in productList)
        {
            if (!storage.BuyState.Contains(productConfig.Id))
                return false;
        }
        return true;
    }
    #region 通用时间工具组
    public static bool IsActive(this StorageGiftBagDouble storageWeek)
    {
        return storageWeek.GetStartTime() <= 0 && storageWeek.GetLeftTime() > 0;
    }
    public static bool IsTimeOut(this StorageGiftBagDouble storageWeek)
    {
        return storageWeek.GetLeftTime() <= 0;
    }
    public static long GetLeftTime(this StorageGiftBagDouble storageWeek)
    {
        return Math.Max(storageWeek.EndTime - (long) APIManager.Instance.GetServerTime(), 0);
    }
    public static void SetLeftTime(this StorageGiftBagDouble storageWeek,long leftTime)
    {
        storageWeek.EndTime = (long) APIManager.Instance.GetServerTime() + leftTime;
    }
    public static string GetLeftTimeText(this StorageGiftBagDouble storageWeek)
    {
        return CommonUtils.FormatLongToTimeStr(storageWeek.GetLeftTime());
    }
    public static long GetStartTime(this StorageGiftBagDouble storageWeek)
    {
        return Math.Max(storageWeek.StartTime - (long) APIManager.Instance.GetServerTime(), 0);
    }
    public static void SetStartTime(this StorageGiftBagDouble storageWeek,long leftTime)
    {
        storageWeek.StartTime = (long) APIManager.Instance.GetServerTime() + leftTime;
    }
    #endregion
    
    public static bool ShowAuxItem(this StorageGiftBagDouble storage)
    {
        if (!GiftBagDoubleModel.Instance.IsOpenPrivate())
            return false;
        if (storage.IsTimeOut())
            return false;
        return true;
    }
    
    public static string GetAuxItemAssetPath(this StorageGiftBagDouble storage)
    {
        return "Prefabs/Activity/GiftBagDouble/Aux_GiftBagDouble";
    }
}