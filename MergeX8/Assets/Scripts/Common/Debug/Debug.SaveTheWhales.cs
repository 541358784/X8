using System;
using System.Collections.Generic;
using System.ComponentModel;
using Activity.SaveTheWhales;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;


public partial class SROptions
{
    [Category(SaveTheWhales)]
    [DisplayName("重置")]
    public void ResetSaveTheWhales()
    {
        HideDebugPanel();
        
        StorageManager.Instance.GetStorage<StorageHome>().SaveTheWhales.Clear();
    }

    public int _energyCount;
    [Category(SaveTheWhales)]
    [DisplayName("数量")]
    public int EnergyCount
    {
        get
        {
            return _energyCount;
        }
        set
        {
            _energyCount = value;
        }
    }

    [Category(SaveTheWhales)]
    [DisplayName("消耗")]
    public void EnergyConsume()
    {
        SaveTheWhalesModel.Instance.OnConsumeRes(UserData.ResourceId.Energy,_energyCount);
    }
    
    
    [Category(SaveTheWhales)]
    [DisplayName("收集数量")]
    public int SaveTheWhalesCollectCount
    {
        get
        {
            return StorageManager.Instance.GetStorage<StorageHome>().SaveTheWhales.CollectCount;
        }
    }
    [Category(SaveTheWhales)]
    [DisplayName("当前组")]
    public string SaveTheWhalesGroupId
    {
        get
        {
            return StorageManager.Instance.GetStorage<StorageHome>().SaveTheWhales.GroupId.ToString();
        }
    }
    [Category(SaveTheWhales)]
    [DisplayName("任务开始时间")]
    public string SaveTheWhalesJoinTime
    {
        get
        {
            return StorageManager.Instance.GetStorage<StorageHome>().SaveTheWhales.JoinTime.ToUTCTimeString();
        }
    }
    [Category(SaveTheWhales)]
    [DisplayName("任务结束时间")]
    public string SaveTheWhalesJoinEndTime
    {
        get
        {
            return StorageManager.Instance.GetStorage<StorageHome>().SaveTheWhales.JoinEndTime.ToUTCTimeString();
        }
    }
    [Category(SaveTheWhales)]
    [DisplayName("IsJoin")]
    public string SaveTheWhalesIsJoin
    {
        get
        {
            return StorageManager.Instance.GetStorage<StorageHome>().SaveTheWhales.IsJoin.ToString();
        }
    }
    [Category(SaveTheWhales)]
    [DisplayName("任务开始日期")]
    public string SaveTheWhalesJoinDay
    {
        get
        {
            return StorageManager.Instance.GetStorage<StorageHome>().SaveTheWhales.JoinDay.ToString();
        }
    }
    
    [Category(SaveTheWhales)]
    [DisplayName("失败次数")]
    public string SaveTheWhalesFailCount
    {
        get
        {
            return StorageManager.Instance.GetStorage<StorageHome>().SaveTheWhales.FailCount.ToString();
        }
    }
    [Category(SaveTheWhales)]
    [DisplayName("成功次数")]
    public string SaveTheWhalesSuccessCount
    {
        get
        {
            return StorageManager.Instance.GetStorage<StorageHome>().SaveTheWhales.SuccessCount.ToString();
        }
    }
    
    [Category(SaveTheWhales)]
    [DisplayName("体力刷新时间")]
    public string SaveTheWhalesConsumeEnergyTime
    {
        get
        {
            return StorageManager.Instance.GetStorage<StorageHome>().SaveTheWhales.ConsumeEnergyTime.ToString();
        }
    }
    [Category(SaveTheWhales)]
    [DisplayName("体力消耗数量")]
    public string SaveTheWhalesConsumeEnergy
    {
        get
        {
            return StorageManager.Instance.GetStorage<StorageHome>().SaveTheWhales.ConsumeEnergy.ToString();
        }
    }
    [Category(SaveTheWhales)]
    [DisplayName("本次是否完成")]
    public string SaveTheWhalesIsFinish
    {
        get
        {
            return StorageManager.Instance.GetStorage<StorageHome>().SaveTheWhales.IsFinish.ToString();
        }
    }
}