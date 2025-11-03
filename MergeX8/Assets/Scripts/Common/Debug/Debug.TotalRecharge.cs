using System.Collections.Generic;
using System.ComponentModel;
using DragonPlus;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using Activity.TreasureMap;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;


public partial class SROptions
{
    private const string TotalRecharge = "新手累计充值";
    [Category(TotalRecharge)]
    [DisplayName("清除数据")]
    public void ClearTotalRechargeNew()
    {
        StorageManager.Instance.GetStorage<StorageHome>().TotalRecharges_New.Clear();
        StorageManager.Instance.GetStorage<StorageHome>().DecoBuildRewards.Remove("10001");
        CoolingTimeManager.Instance.RemoveCooling(CoolingTimeManager.CDType.OtherDay,"totalRecharge_new");
    }
    
    [Category(TotalRecharge)]
    [DisplayName("下一天")]
    public void RechargeNextDay()
    {
        if(!StorageManager.Instance.GetStorage<StorageHome>().DecoBuildRewards.ContainsKey("10001"))
            return;

        StorageManager.Instance.GetStorage<StorageHome>().DecoBuildRewards["10001"].GetTime = 0;
    }
}
