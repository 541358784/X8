using System.Collections.Generic;
using System.ComponentModel;
using Activity.JumpGrid;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;


public partial class SROptions
{
    private const string EndlessEnergyGiftBag = "无尽体力礼包";
    [Category(EndlessEnergyGiftBag)]
    [DisplayName("重置")]
    public void RestEndlessEnergyGiftBag()
    {
       EndlessEnergyGiftBagModel.Instance.Storage.Clear();
    }
    
    [Category(EndlessEnergyGiftBag)]
    [DisplayName("连续未付费天数")]
    public int EndlessEnergyGiftBagDayCount
    {
        get
        {
            return EndlessEnergyGiftBagModel.Instance.Storage.DayCount;
        }
        set
        {
            EndlessEnergyGiftBagModel.Instance.Storage.DayCount = value;
        }
    }
    
    [Category(EndlessEnergyGiftBag)]
    [DisplayName("DayId")]
    public int EndlessEnergyGiftBagDayId
    {
        get
        {
            return EndlessEnergyGiftBagModel.Instance.Storage.DayId;
        }
        set
        {
            EndlessEnergyGiftBagModel.Instance.Storage.DayId = value;
        }
    }
    
}