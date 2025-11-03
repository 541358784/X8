using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using UnityEngine;


public partial class SROptions
{
    private const string NewNewIceBreakPack = "牛牛破冰礼包";
    [Category(NewNewIceBreakPack)]
    [DisplayName("重置")]
    public void ResetNewNewIceBreakPack()
    {
        HideDebugPanel();
        var newUser = NewNewIceBreakPackModel.Instance.Storage.IsNewUser;
        NewNewIceBreakPackModel.Instance.Storage.Clear();
        NewNewIceBreakPackModel.Instance.Storage.IsNewUser = newUser;
    }

    [Category(NewNewIceBreakPack)]
    [DisplayName("是否为新用户")]
    public bool NewNewIceBreakPackIsNewUser
    {
        get
        {
            return NewNewIceBreakPackModel.Instance.Storage.IsNewUser;
        }
        set
        {
            NewNewIceBreakPackModel.Instance.Storage.IsNewUser = value;
        }
    }

    [Category(NewNewIceBreakPack)]
    [DisplayName("时间前调一天")]
    public void NewNewIceBreakPackOneDayBefore()
    {
        if (!NewNewIceBreakPackModel.Instance.Storage.IsInit)
            return;
        NewNewIceBreakPackModel.Instance.Storage.StartTime += (long)XUtility.DayTime;
        NewNewIceBreakPackModel.Instance.Storage.EndTime += (long)XUtility.DayTime;
    }
    [Category(NewNewIceBreakPack)]
    [DisplayName("时间后调一天")]
    public void NewNewIceBreakPackOneDayAfter()
    {
        if (!NewNewIceBreakPackModel.Instance.Storage.IsInit)
            return;
        NewNewIceBreakPackModel.Instance.Storage.StartTime -= (long)XUtility.DayTime;
        NewNewIceBreakPackModel.Instance.Storage.EndTime -= (long)XUtility.DayTime;
    }
}