using System.Collections.Generic;
using System.ComponentModel;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using UnityEngine;


public partial class SROptions
{
    private const string PayLevel = "0付费分层";
    [Category(PayLevel)]
    [DisplayName("重置付费分层")]
    public void ResetPayLevel()
    {
        HideDebugPanel();
        PayLevelModel.Instance.Storage.Clear();
        PayLevelModel.Instance.UpdateState();
    }


    [Category(PayLevel)]
    [DisplayName("当前分层")]
    public int CurPayLevel
    {
        get
        {
            return PayLevelModel.Instance.GetCurPayLevelConfig().Id;
        }
    }
    
    [Category(PayLevel)]
    [DisplayName("日期")]
    public string CurPayLevelDay
    {
        get
        {
            return PayLevelModel.Instance.Storage.DayId.ToString();
        }
        set
        {
            PayLevelModel.Instance.Storage.DayId = value.ToInt();
        }
    }
    
    [Category(PayLevel)]
    [DisplayName("当日付费金额")]
    public string CurPayLevelDayPayValue
    {
        get
        {
            return PayLevelModel.Instance.Storage.CurDayPayValue.ToString();
        }
        set
        {
            PayLevelModel.Instance.Storage.CurDayPayValue = value.ToFloat();
        }
    }
    
    [Category(PayLevel)]
    [DisplayName("付费天数")]
    public string CurPayLevelContinuePayDay
    {
        get
        {
            return PayLevelModel.Instance.Storage.ContinuePayDays.ToString();
        }
        set
        {
            PayLevelModel.Instance.Storage.ContinuePayDays = value.ToInt();
        }
    }
    
    [Category(PayLevel)]
    [DisplayName("未付费天数")]
    public string CurPayLevelContinueUnPayDay
    {
        get
        {
            return PayLevelModel.Instance.Storage.ContinueUnPayDays.ToString();
        }
        set
        {
            PayLevelModel.Instance.Storage.ContinueUnPayDays = value.ToInt();
        }
    }
    
    [Category(PayLevel)]
    [DisplayName("更新")]
    public void UpdatePayLevel()
    {
        HideDebugPanel();
        PayLevelModel.Instance.UpdateState();
    }
    
    [Category(PayLevel)]
    [DisplayName("强制指定分层")]
    public int DebugPayLevel
    {
        get
        {
            return PayLevelModel.Instance.DebugPayLevel;
        }
        set
        {
            PayLevelModel.Instance.DebugPayLevel = value;
        }
    }
    
    [Category(PayLevel)]
    [DisplayName("使用强制指定分层")]
    public bool DebugPayLevelOpen
    {
        get
        {
            return PayLevelModel.Instance.DebugPayLevelOpen;
        }
        set
        {
            PayLevelModel.Instance.DebugPayLevelOpen = value;
        }
    }
}