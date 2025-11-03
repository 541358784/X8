using System;
using System.Collections.Generic;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using UnityEngine;

public abstract class ActivityEntityBase
{
    static string BaseTime = "00:00:00";
    public abstract string Guid { get; }
    private bool initedFromServer;
    public string SubActivityType = "";

    public string ActivityId { get; set; }

    //public EActivityType ActivityType { get; set; }
    public ulong StartTime { get; set; }
    public ulong EndTime { get; set; }
    public ulong RewardEndTime { get; set; }
    public bool ManualEnd { get; set; }
    public int TokenCollect { get; protected set; }
    protected bool IsInited = false;

    public virtual bool IsCache { get; protected set; }
    
    public string StorageKey => "Activity_" + ActivityId;


    // 配置要在具体活动中去解析
    public virtual void InitFromServerData(string activityId, string activityType, ulong startTime,
        ulong endTime, ulong rewardEndTime, bool manualEnd, string configJson,
        string activitySubType)
    {
        ActivityId = activityId;
        StartTime = startTime;
        EndTime = endTime;
        RewardEndTime = rewardEndTime;
        ManualEnd = manualEnd;

        if (string.IsNullOrEmpty(activitySubType))
            SubActivityType = "";
        else
            SubActivityType = activitySubType;

        initedFromServer = true;
    }

    public virtual List<string> GetNeedResList(string activityId,List<string> allResList)
    {
        var resList = new List<string>();
        foreach (var path in allResList)
        {
            DebugUtil.Log("ActivityManager -> 活动资源 : " + path);
            resList.Add(path);
        }
        return resList;
    }

    public void UpdateActivityUsingResList(string activityId,bool autoDownload = true)
    {
        var allResList = ActivityManager.Instance.GetActivityAllMd5List(activityId);
        if (allResList == null)
        {
            Debug.LogError("活动全部资源未初始化");
            return;
        }
        var resList = GetNeedResList(activityId,allResList);
        ActivityManager.Instance.UpdateActivityUsingResList(activityId,resList);
        ActivityManager.Instance.CheckSingleActivityResState(activityId);
        if (!ActivityManager.Instance.IsActivityResourcesDownloaded(activityId) && autoDownload)
        {
            ActivityManager.Instance.TryPullSingleActivityRes(activityId);
        }
    }

    public virtual void ExecuteActivityChange()
    {
    }

    public virtual string ActiveActivityGuid()
    {
        if (IsOpened() == false)
            return string.Empty;
        return Guid;
    }

    public virtual string GetBaseTime()
    {
        return BaseTime;
    }

    #region 下面方法不要调用其他方法

    public virtual bool IsOpened(bool hasLog = false)
    {
        if (!initedFromServer)
            return false;

        // 应QA需求，输出各个变量的值
        var resOk = ActivityManager.Instance.IsActivityResourcesDownloaded(ActivityId);
        var startTimeOk = APIManager.Instance.GetServerTime() > StartTime;
        var endTimeOk = APIManager.Instance.GetServerTime() < EndTime;
        if (hasLog)
            DebugUtil.LogError(
                "IsOpened: resOk={0}, !ManualEnd={1}, startTimeOk={2}, endTimeOk={3}, ActivityId={4}", resOk,
                !ManualEnd, startTimeOk, endTimeOk, ActivityId);

        return resOk && !ManualEnd && startTimeOk && endTimeOk;
        
    }
    //活动是否在领奖期间
    public virtual bool IsInReward(bool hasLog = false)
    {
        if (!initedFromServer)
            return false;

        // 应QA需求，输出各个变量的值
        var resOk = ActivityManager.Instance.IsActivityResourcesDownloaded(ActivityId);
        var startTimeOk = APIManager.Instance.GetServerTime() > StartTime;
        var endTimeOk = APIManager.Instance.GetServerTime() < RewardEndTime;
        if (hasLog)
            DebugUtil.LogError(
                "IsOpened: resOk={0}, !ManualEnd={1}, startTimeOk={2}, endTimeOk={3}, ActivityId={4}", resOk,
                !ManualEnd, startTimeOk, endTimeOk, ActivityId);

        return resOk && !ManualEnd && startTimeOk && endTimeOk;
    }

    #endregion

    public bool IsInitFromServer()
    {
        return initedFromServer;
    }

    // 活动剩余时间
    public ulong GetActivityLeftTime()
    {
        var left = (long) EndTime - (long) APIManager.Instance.GetServerTime();
        if (left < 0)
            left = 0;
        return (ulong) left;
    }

    // 活动领奖剩余时间
    public ulong GetActivityRewardLeftTime()
    {
        var left = (long) RewardEndTime - (long) APIManager.Instance.GetServerTime();
        if (left < 0)
            left = 0;
        return (ulong) left;
    }

    public virtual void OnPurchased(int shopId)
    {
    }

    /// <summary>
    /// 更新子活动时间
    /// </summary>
    /// <param name="startTime"></param>
    /// <param name="endTime"></param>
    /// <param name="rewardEndTime"></param>
    public virtual void UpdateSubActivityTime(ulong startTime, ulong endTime, ulong rewardEndTime, ulong duration,
        ulong totalDuration)
    {
    }

    // 活动剩余时间的字符串显示
    public virtual string GetActivityLeftTimeString()
    {
        return CommonUtils.FormatLongToTimeStr((long) GetActivityLeftTime());
    }

    // 领奖剩余时间的字符串显示
    public string GetActivityRewardLeftTimeString()
    {
        return CommonUtils.FormatLongToTimeStr((long) GetActivityRewardLeftTime());
    }

    // 获得当前是活动的第几天
    public int GetDayNum(long timeStamp = 0)
    {
        var nowTime = (ulong) (timeStamp > 0 ? timeStamp : (long) APIManager.Instance.GetServerTime());
        var startTime = StartTime;

        return (int) (nowTime - startTime) / 86400000;
    }

    /// <summary>
    /// 完成活动数据初始化
    /// </summary>
    protected virtual void InitServerDataFinish()
    {
    }

    /// <summary>
    /// 更新活动状态
    /// </summary>
    public virtual void UpdateActivityState()
    {
    }

    /// <summary>
    /// 关卡胜利
    /// </summary>
    public virtual void LevelComplete()
    {
    }

    /// <summary>
    /// 关卡失败
    /// </summary>
    public virtual void LevelFail()
    {
    }

    /// <summary>
    /// 检测其他资源
    /// </summary>
    public virtual void CheckOrDownloadOtherRes()
    {
    }

    /// <summary>
    /// 是否路过活动预热
    /// </summary>
    public virtual bool IsSkipActivityPreheating()
    {
        return StorageManager.Instance.GetStorage<StorageHome>().SkipActivityPreheating;
    }

    #region for init

    public void Init()
    {
        if (!ActivityManager.Instance.IsRegistered(this))
            ActivityManager.Instance.Register(this);
    }
    public void Release()
    {
        if (ActivityManager.Instance.IsRegistered(this))
            ActivityManager.Instance.Unregister(this);
    }

    #endregion
    public abstract bool CanDownLoadRes();
}