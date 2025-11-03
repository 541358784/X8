using System;
using System.Linq;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using Merge.Order;
using UnityEngine;

public static class KeepPetUtils
{
    public static KeepPetLevelConfig KeepPetGetCurLevelConfig(this int exp)
    {
        var configs = KeepPetModel.Instance.LevelConfig;
        for (var i = configs.Count-1; i >= 0; i--)
        {
            var config = configs[i];
            if (exp >= config.Exp)
                return config;
        }
        return configs[0];
    }

    public static int GetNextLevelNeedExp(this KeepPetLevelConfig levelConfig)
    {
        var configs = KeepPetModel.Instance.LevelConfig;
        if (levelConfig == configs.Last())
        {
            return 999999;
        }
        else
        {
            var nextLevelConfig = configs.Find(a => a.Id == levelConfig.Id + 1);
            return nextLevelConfig.Exp - levelConfig.Exp;
        }
    }

    public static int GetCurLevelExp(this KeepPetLevelConfig levelConfig,int totalExp)
    {
        var curLevelMaxExp = levelConfig.GetNextLevelNeedExp();
        return Math.Min(curLevelMaxExp,totalExp - levelConfig.Exp);
    }
    public static float GetCurLevelExp(this KeepPetLevelConfig levelConfig,float totalExp)
    {
        var curLevelMaxExp = levelConfig.GetNextLevelNeedExp();
        return Math.Min(curLevelMaxExp,totalExp - levelConfig.Exp);
    }
    public static bool IsSleepByTime(this StorageKeepPet storage)
    {
        var sleepTime = KeepPetModel.Instance.GlobalConfig.SleepTime * (long)XUtility.Min;
        var inSleepTime = storage.LastWakeUpTime + sleepTime;
        var curTime = (long) APIManager.Instance.GetServerTime();
        return curTime > inSleepTime;
    }
    public static void CheckHungry(this StorageKeepPet storage)
    {
        var curTime = APIManager.Instance.GetServerTime();
        var offset = (ulong)KeepPetModel.Instance.GlobalConfig.DogHungryTimeOffset * XUtility.Hour;
        var curDay =(int)((curTime-offset) / XUtility.DayTime);
        if (curDay != storage.HungryDayId)
        {
            storage.HungryDayId = curDay;
            storage.Cure = false;
        }
    }
    public static KeepPetBaseState GetCurState(this StorageKeepPet storage)
    {
        return ((KeepPetStateEnum) storage.CurPetState).GetState();
    }
    public static KeepPetBaseState GetState(this KeepPetStateEnum e)
    {
        return KeepPetModel.Instance.StateDictionary.TryGetValue(e,out var state)?state:null;
    }
    
    
    public static bool ShowEntrance(this StorageKeepPet storage)
    {
        if (!KeepPetModel.Instance.IsOpen())
            return false;
        return true;
    }

    public static Aux_KeepPet GetAuxItem(this StorageKeepPet storage)
    {
        return UIHomeMainController.mainController.ButtonKeepPet.GetComponent<Aux_KeepPet>();
    }
    
    public static string GetTaskItemAssetPath(this StorageKeepPet storage)
    {
        return "Prefabs/KeepPet/TaskList_KeepPet";
    }
}