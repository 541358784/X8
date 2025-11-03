using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Deco.World;
using Decoration;
using DragonPlus;
using DragonPlus.Config.BiuBiu;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;
using Utils = DragonU3DSDK.Utils;

public static class BiuBiuUtils
{
    public static BiuBiuModel Model => BiuBiuModel.Instance;


   

    public static bool IsTimeOut(this StorageBiuBiu storageWeek)
    {
        return storageWeek.GetLeftTime() <= 0;
    }
    
    public static long GetLeftTime(this StorageBiuBiu storageWeek)
    {
        return Math.Max(storageWeek.EndTime - (long) APIManager.Instance.GetServerTime(), 0);
    }

    public static void SetLeftTime(this StorageBiuBiu storageWeek, long leftTime)
    {
        var endTime = (long) APIManager.Instance.GetServerTime() + leftTime;
        storageWeek.EndTime = endTime;
    }

    public static string GetLeftTimeText(this StorageBiuBiu storageWeek)
    {
        return CommonUtils.FormatLongToTimeStr(storageWeek.GetLeftTime());
    }

    public static Aux_BiuBiu GetAuxItem(this StorageBiuBiu storage)
    {
        return Aux_BiuBiu.Instance;
    }

    public static MergeBiuBiu GetTaskEntrance(this StorageBiuBiu storage)
    {
        return MergeBiuBiu.Instance;
    }
    
}