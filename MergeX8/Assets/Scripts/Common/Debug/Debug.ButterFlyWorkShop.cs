using System.Collections.Generic;
using System.ComponentModel;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using UnityEngine;


public partial class SROptions
{
    [Category(ButterflyWorkShop)]
    [DisplayName("重置胡蝶工坊")]
    public void ResetButterflyWorkShop()
    {
        HideDebugPanel();
        StorageManager.Instance.GetStorage<StorageHome>().ButterflyWorkShop.Clear();
        ButterflyWorkShopModel.Instance.UnSetItemsCount = 0;
        StorageManager.Instance.GetStorage<StorageGame>().MergeBoards.Remove((int) MergeBoardEnum.ButterflyWorkShop);
        CoolingTimeManager.Instance.RemoveCooling(CoolingTimeManager.CDType.OtherDay, "ButterflyWorkShop");
        var guideIdList = new List<int>() {4102,4103,4104,4105,4106};
        CleanGuideList(guideIdList);
    }

    [Category(ButterflyWorkShop)]
    [DisplayName("获取该等级的活动棋子")]
    public void GetButterflyItem()
    {
        ButterflyWorkShopModel.Instance.UnSetItems.Add(ButterflyWorkShopModel.Instance.GetMergeItemConfig(butterflyItemLevel).id);
        ButterflyWorkShopModel.Instance.UnSetItemsCount++;
    }
    private int butterflyItemLevel;
    [Category(ButterflyWorkShop)]
    [DisplayName("设置棋子等级")]
    public int ButterflyItemLevel
    {
        get
        {
            return butterflyItemLevel;
        }
        set
        {
            butterflyItemLevel = value;
        }
    }
  
}