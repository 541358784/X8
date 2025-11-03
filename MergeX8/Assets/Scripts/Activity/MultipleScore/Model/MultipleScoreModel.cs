using System;
using System.Collections.Generic;
using System.Linq;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using Framework;
using UnityEngine;
using BiEventCooking=DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class MultipleScoreModel : ActivityEntityBase
{
    private static MultipleScoreModel _instance;
    public static MultipleScoreModel Instance => _instance ?? (_instance = new MultipleScoreModel());

    public Dictionary<string, StorageMultipleScore> Storage =>
        StorageManager.Instance.GetStorage<StorageHome>().MultipleScore;

    public StorageMultipleScore CurStorage => Storage.ContainsKey(ActivityId) ? Storage[ActivityId] : null;

    public void CreateStorage()
    {
        if (IsInitFromServer() && CurStorage == null)
        {
            var newStorage = new StorageMultipleScore();
            var configList = MultipleScoreConfigManager.Instance.GetConfig();
            var curTime = APIManager.Instance.GetServerTime();
            ulong longestActiveTime = 0;
            newStorage.StartTime = curTime;
            for (var i = 0; i < configList.Count; i++)
            {
                var config = configList[i];
                var activeTime = (ulong)config.activeTime * XUtility.Hour;
                var endTime = curTime + activeTime;
                endTime = Math.Min(endTime, EndTime);
                longestActiveTime = Math.Max(longestActiveTime, activeTime);
                var multiType = config.influenceFunction;
                var multiValue = config.multiple;
                var rule = new StorageMultipleScoreSingleRule()
                {
                    StartTime = curTime,
                    EndTime = endTime,
                    MultiValue = multiValue,
                };
                newStorage.MultiValueList.Add(multiType,rule);
            }
            newStorage.EndTime = curTime + longestActiveTime;
            Storage.Add(ActivityId,newStorage);
        }
    }

    public override string Guid => "OPS_EVENT_TYPE_MULTIPLE_SCORE";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitAuto()
    {
        Instance.Init();
    }

    public enum InfluenceFuncType
    {
        None=0,
        Mermaid,
        ThemeDecoration,
    }

    public override void InitFromServerData(string activityId, string activityType, ulong startTime, ulong endTime,
        ulong rewardEndTime, bool manualEnd, string configJson, string activitySubType)
    {
        base.InitFromServerData(activityId, activityType, startTime, endTime, rewardEndTime, manualEnd, configJson,
            activitySubType);
        MultipleScoreConfigManager.Instance.InitFromServerData(configJson);
        InitServerDataFinish();
        CreateStorage();
    }


    public bool IsOpenActivity()
    {
        var curTime = APIManager.Instance.GetServerTime();
        return IsOpened() && CurStorage != null && curTime >= CurStorage.StartTime && curTime < CurStorage.EndTime;
    }
    
    public string GetActiveTime(InfluenceFuncType type)
    {
        long diffValue;
        if (!IsOpenActivity())
        {
            return "00:00";
        }
        if (!CurStorage.MultiValueList.TryGetValue((int) type, out var rule))
        {
            return "00:00";
        }
        var curTime = APIManager.Instance.GetServerTime();
        if (curTime < rule.StartTime || curTime > rule.EndTime)
            return "00:00";
        
        diffValue = (long)rule.EndTime - (long)APIManager.Instance.GetServerTime();
        return CommonUtils.FormatLongToTimeStr(diffValue);
    }

    public float GetMultiple(InfluenceFuncType type)
    {
        if (!IsOpenActivity())
            return 1;
        if (!CurStorage.MultiValueList.TryGetValue((int) type, out var rule))
        {
            return 1;
        }
        var curTime = APIManager.Instance.GetServerTime();
        if (curTime < rule.StartTime || curTime > rule.EndTime)
            return 1;
        return rule.MultiValue;
    }
    public override bool CanDownLoadRes()
    {
        return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.Mermaid);
    }
}