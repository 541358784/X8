using System.Collections.Generic;
using System.ComponentModel;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using UnityEngine;


public partial class SROptions
{
    [Category(SummerWatermelonBread)]
    [DisplayName("重置夏日西瓜")]
    public void ResetSummerWatermelonBread()
    {
        HideDebugPanel();
        StorageManager.Instance.GetStorage<StorageHome>().SummerWatermelonBread.Clear();
        SummerWatermelonBreadModel.Instance.UnSetItemsCount = 0;
        var guideIdList = new List<int>() {646,647,648,649,650,651,652};
        CleanGuideList(guideIdList);
        CoolingTimeManager.Instance.RemoveCooling(CoolingTimeManager.CDType.OtherDay,SummerWatermelonBreadModel.packageCoolTimeKey);
    }

    [Category(SummerWatermelonBread)]
    [DisplayName("获取该等级的活动棋子")]
    public void GetSummerWatermelonBreadItem()
    {
        SummerWatermelonBreadModel.Instance.UnSetItems.Add(SummerWatermelonBreadModel.Instance.GetMergeItemConfig(summerWatermelonBreadItemLevel).id);
        SummerWatermelonBreadModel.Instance.UnSetItemsCount++;
    }
    private int summerWatermelonBreadItemLevel;
    [Category(SummerWatermelonBread)]
    [DisplayName("设置棋子等级")]
    public int SetSummerWatermelonBreadItemLevel
    {
        get
        {
            return summerWatermelonBreadItemLevel;
        }
        set
        {
            summerWatermelonBreadItemLevel = value;
        }
    }
}