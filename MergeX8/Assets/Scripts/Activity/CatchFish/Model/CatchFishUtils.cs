using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Deco.World;
using Decoration;
using DragonPlus;
using DragonPlus.Config.CatchFish;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;
using Utils = DragonU3DSDK.Utils;

public static class CatchFishUtils
{
    public static CatchFishModel Model => CatchFishModel.Instance;
    public static long GetPreheatLeftTime(this StorageCatchFish storage)
    {
        var heatTime = storage.PreheatCompleteTime - (long) APIManager.Instance.GetServerTime();
        heatTime = Math.Max(heatTime, 0);
        return heatTime;
    }

    public static string GetPreheatLeftTimeText(this StorageCatchFish storage)
    {
        return CommonUtils.FormatLongToTimeStr(storage.GetPreheatLeftTime());
    }
    public static bool IsTimeOut(this StorageCatchFish storageWeek)
    {
        return storageWeek.GetLeftTime() <= 0;
    }
    
    public static long GetLeftTime(this StorageCatchFish storageWeek)
    {
        return Math.Max(storageWeek.EndTime - (long) APIManager.Instance.GetServerTime(), 0);
    }

    public static string GetLeftTimeText(this StorageCatchFish storageWeek)
    {
        return CommonUtils.FormatLongToTimeStr(storageWeek.GetLeftTime());
    }
    
    
}

public class CatchFishLevelState
{
    public int Level;
    public int Group;
    public int GroupInnerIndex;
    public int CurScore;
    public int MaxScore;
    public List<ResData> Rewards;
}