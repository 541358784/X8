using System.Collections.Generic;
using System.ComponentModel;
using Decoration;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using DragonU3DSDK.Storage.Decoration;
using Gameplay;
using UnityEngine;


public partial class SROptions
{
    private const string ShopExtraReward = "商店额外奖励";
    [Category(ShopExtraReward)]
    [DisplayName("清空商店额外奖励")]
    public void CleanShopExtraRewardStorage()
    {
        ShopExtraRewardModel.Instance.StorageDic.Clear();
    }
}