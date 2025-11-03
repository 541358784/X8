using System.Collections.Generic;
using System.ComponentModel;
using Activity.JumpGrid;
using ActivityLocal.ClimbTower.Model;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;


public partial class SROptions
{
    private const string ClimbTower = "爬塔";
    [Category(ClimbTower)]
    [DisplayName("重置跳爬塔")]
    public void RestClimbTower()
    {
       ClimbTowerModel.Instance.ClimbTower.Clear();
       
    }

    [Category(ClimbTower)]
    [DisplayName("重置刷新时间")]
    public void RestRefreshTime()
    {
        ClimbTowerModel.Instance.ClimbTower.RefreshTime = 0;
       
    }
    
    [Category(ClimbTower)]
    [DisplayName("免费次数")]
    public int ClimbTowerFreeTime
    {
        get
        {
            return ClimbTowerModel.Instance.ClimbTower.FreeTimes;
        }
        set
        {
            ClimbTowerModel.Instance.ClimbTower.FreeTimes = value;
        }
    }
    [Category(ClimbTower)]
    [DisplayName("付费次数")]
    public int ClimbTowerPayTime
    {
        get
        {
            return ClimbTowerModel.Instance.ClimbTower.PayTimes;
        }
        set
        {
            ClimbTowerModel.Instance.ClimbTower.PayTimes = value;
        }
    }
    [Category(ClimbTower)]
    [DisplayName("是否付费过")]
    public bool ClimbTowerIsPay
    {
        get
        {
            return ClimbTowerModel.Instance.ClimbTower.IsPay;
        }
        set
        {
            ClimbTowerModel.Instance.ClimbTower.IsPay = value;
        }
    }
    [Category(ClimbTower)]
    [DisplayName("是否付费关卡")]
    public bool ClimbTowerIsPayLevel
    {
        get
        {
            return ClimbTowerModel.Instance.ClimbTower.IsPayLevel;
        }
        set
        {
            ClimbTowerModel.Instance.ClimbTower.IsPayLevel = value;
        }
    }
}