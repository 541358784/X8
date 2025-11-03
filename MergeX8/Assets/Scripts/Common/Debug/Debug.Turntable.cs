using System;
using System.ComponentModel;
using Activity.CrazeOrder.Model;
using Activity.LimitTimeOrder;
using Activity.TimeOrder;
using Activity.Turntable.Model;
using DragonPlus.Config.CrazeOrder;
using DragonPlus.Config.LimitOrderLine;
using DragonPlus.Config.TimeOrder;
using DragonU3DSDK.Storage;
using Merge.Order;
using UnityEngine;


public partial class SROptions
{
    private const string Turntable = "积分轮盘";
    [Category(Turntable)]
    [DisplayName("清理轮盘")]
    public void ClearTurntable()
    {
        HideDebugPanel();
        TurntableModel.Instance.Turntable.Clear();
    }
    
    [Category(Turntable)]
    [DisplayName("轮盘积分")]
    public int turntableCoin
    {
       get
       {
           return TurntableModel.Instance.Turntable.ActivityCoin;
       }
       set
       {
           TurntableModel.Instance.Turntable.ActivityCoin = value;
       }
    }
}