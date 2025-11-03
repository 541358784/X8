using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Deco.World;
using Decoration;
using DragonPlus;
using DragonPlus.Config.PillowWheel;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;
using Utils = DragonU3DSDK.Utils;

public static class PillowWheelUtils
{
    public static PillowWheelModel Model => PillowWheelModel.Instance;
    public static long GetPreheatLeftTime(this StoragePillowWheel storage)
    {
        var heatTime = storage.PreheatCompleteTime - (long) APIManager.Instance.GetServerTime();
        heatTime = Math.Max(heatTime, 0);
        return heatTime;
    }

    public static string GetPreheatLeftTimeText(this StoragePillowWheel storage)
    {
        return CommonUtils.FormatLongToTimeStr(storage.GetPreheatLeftTime());
    }
    public static bool IsTimeOut(this StoragePillowWheel storageWeek)
    {
        return storageWeek.GetLeftTime() <= 0;
    }
    
    public static long GetLeftTime(this StoragePillowWheel storageWeek)
    {
        return Math.Max(storageWeek.EndTime - (long) APIManager.Instance.GetServerTime(), 0);
    }

    public static string GetLeftTimeText(this StoragePillowWheel storageWeek)
    {
        return CommonUtils.FormatLongToTimeStr(storageWeek.GetLeftTime());
    }
    
    
}

public class PillowWheelLevelState
{
    public int Level;
    public int Group;
    public int GroupInnerIndex;
    public int CurScore;
    public int MaxScore;
    public List<ResData> Rewards;
}