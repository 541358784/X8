using System;
using System.Collections.Generic;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using DragonU3DSDK.Storage.Decoration;
using Framework;
using UnityEngine;

public class LastTimeCountModel: GlobalSystem<LastTimeCountModel>,IInitable
{
    public void Init()
    {
        LastTimeCountManager.Instance.Init();
    }

    public void Release()
    {
        LastTimeCountManager.Instance.Release();
    }
}

public class LastTimeCountManager: Manager<LastTimeCountManager>
{
    public void Init()
    {
        //InvokeRepeating("UpdateLastTime",0,1);
        XUtility.WaitSeconds(2f,
            () =>
            {
                Debug.LogError("当前时间为" + XUtility.GetTimeString((long) APIManager.Instance.GetServerTime()));
                Debug.LogError("当前Date时间为"+XUtility.GetTimeString(CommonUtils.GetTimeStamp())+" "+TimeZone.CurrentTimeZone.StandardName);
            });
    }

    public void Release()
    {
        CancelInvoke("UpdateLastTime");
    }
    public void UpdateLastTime()
    {
        var serverTime = APIManager.Instance.GetServerTime();
        if (serverTime > StorageLastServerTime)
            StorageLastServerTime = serverTime;
        else if (StorageLastServerTime-serverTime>XUtility.Min)
        {
            var illegalTimeRecord = "StorageTime=" + XUtility.GetTimeString((long) StorageLastServerTime) + 
                                    " ResetTime="+XUtility.GetTimeString((long) serverTime);
            PushIllegalTimeRecord(illegalTimeRecord);
            StorageLastServerTime = serverTime;
        }
            
    }
    private ulong StorageLastServerTime
    {
        get => StorageManager.Instance.GetStorage<StorageHome>().LastServerTime;
        set => StorageManager.Instance.GetStorage<StorageHome>().LastServerTime = value;
    }

    private List<string> IllegalTimeRecordList =>
        StorageManager.Instance.GetStorage<StorageHome>().IllegalTimeRecordList;
    public void PushIllegalTimeRecord(string illegalTimeRecord)
    {
        IllegalTimeRecordList.Add(illegalTimeRecord);
        if (IllegalTimeRecordList.Count > 100)
        {
            IllegalTimeRecordList.RemoveAt(0);
        }
    }
}