using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Deco.World;
using Decoration;
using DragonPlus;
using DragonPlus.Config.FlowerField;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;
using Utils = DragonU3DSDK.Utils;

public static class FlowerFieldUtils
{
    public static FlowerFieldModel Model => FlowerFieldModel.Instance;
    public static long GetPreheatLeftTime(this StorageFlowerField storage)
    {
        var heatTime = storage.PreheatCompleteTime - (long) APIManager.Instance.GetServerTime();
        heatTime = Math.Max(heatTime, 0);
        return heatTime;
    }

    public static void SetPreheatLeftTime(this StorageFlowerField storageWeek, long leftTime)
    {
        storageWeek.PreheatCompleteTime = (long) APIManager.Instance.GetServerTime() + leftTime;
    }

    public static string GetPreheatLeftTimeText(this StorageFlowerField storage)
    {
        return CommonUtils.FormatLongToTimeStr(storage.GetPreheatLeftTime());
    }
    public static bool IsTimeOut(this StorageFlowerField storageWeek)
    {
        return storageWeek.GetLeftTime() <= 0;
    }
    
    public static long GetLeftTime(this StorageFlowerField storageWeek)
    {
        return Math.Max(storageWeek.EndTime - (long) APIManager.Instance.GetServerTime(), 0);
    }

    public static void SetLeftTime(this StorageFlowerField storageWeek, long leftTime)
    {
        var endTime = (long) APIManager.Instance.GetServerTime() + leftTime;
        storageWeek.EndTime = endTime;
    }

    public static string GetLeftTimeText(this StorageFlowerField storageWeek)
    {
        return CommonUtils.FormatLongToTimeStr(storageWeek.GetLeftTime());
    }
    
    
}

public class FlowerFieldLevelState
{
    public int Level;
    public int GroupInnerIndex;
    public int CurScore;
    public int MaxScore;
    public int TotalScore;
    public int TotalMaxScore;
    public List<ResData> Rewards;
}