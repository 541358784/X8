using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Deco.World;
using Decoration;
using DragonPlus;
using DragonPlus.Config.ChristmasBlindBox;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;
using Utils = DragonU3DSDK.Utils;
public static class ChristmasBlindBoxUtils
{
    public static ChristmasBlindBoxModel Model => ChristmasBlindBoxModel.Instance;

    public static bool IsTimeOut(this StorageChristmasBlindBox storageWeek)
    {
        return storageWeek.GetLeftTime() <= 0;
    }

    public static long GetLeftTime(this StorageChristmasBlindBox storageWeek)
    {
        return Math.Max(storageWeek.EndTime - (long) APIManager.Instance.GetServerTime(), 0);
    }

    public static void SetLeftTime(this StorageChristmasBlindBox storageWeek, long leftTime)
    {
        var endTime = (long) APIManager.Instance.GetServerTime() + leftTime;
        storageWeek.EndTime = endTime;
    }

    public static string GetLeftTimeText(this StorageChristmasBlindBox storageWeek)
    {
        return CommonUtils.FormatLongToTimeStr(storageWeek.GetLeftTime());
    }
    
    
    
    
    
    #region Entrance
    public static bool ShowAuxItem(this StorageChristmasBlindBox storage)
    {
        if (!ChristmasBlindBoxModel.Instance.IsPrivateOpened())
            return false;
        return true;
    }
    
    public static string GetAuxItemAssetPath(this StorageChristmasBlindBox storage)
    {
        return "Prefabs/Activity/ChristmasBlindBox/Aux_ChristmasBlindBox";
    }
    #endregion
}