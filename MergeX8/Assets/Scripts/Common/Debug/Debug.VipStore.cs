using System.Collections.Generic;
using System.ComponentModel;
using ConnectLine.Model;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using Filthy.Game;
using Filthy.Model;
using Gameplay.UI.Store.Vip.Model;
using Makeover;
using OnePath.Model;
using UnityEngine;


public partial class SROptions
{
    private const string VipStore = "1Vip Store";
    
    [Category(VipStore)]
    [DisplayName("当前VIP等级")]
    public int CurrentVipLevel
    {
        get
        {
            return VipStoreModel.Instance.vipStore.VipLevel;
        }
        set
        {
            VipStoreModel.Instance.vipStore.VipLevel = value;
        }
    }
    
    [Category(VipStore)]
    [DisplayName("重置VIP")]
    public void RestVipStore()
    {
        VipStoreModel.Instance.vipStore.Clear();
        var guideList = new List<int>
        {
            4590,
            4591,
            4592,
            4593,
            4594,
        };
        CleanGuideList(guideList);
    }
    
    [Category(VipStore)]
    [DisplayName("重置VIP购买记录")]
    public void RestVipStoreRecord()
    {
        VipStoreModel.Instance.vipStore.BuyRecord.Clear();
    } 
    
    [Category(VipStore)]
    [DisplayName("充值金额")]
    public int CurrentPurchase
    {
        get
        {
            return VipStoreModel.Instance.vipStore.PurchasePrice;
        } 
        set
        {
            VipStoreModel.Instance.Purchase(value);
        }
    }  
    
    [Category(VipStore)]
    [DisplayName("执行降档")]
    public void VipStoreCycle()
    {
        VipStoreModel.Instance.vipStore.CycleTime = 0;
    }
    
    [Category(VipStore)]
    [DisplayName("执行刷新")]
    public void VipStoreRefresh()
    {
        VipStoreModel.Instance.vipStore.RefreshTime = 0;
    }
}