using System.Collections.Generic;
using System.ComponentModel;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using UnityEngine;


public partial class SROptions
{
    [Category(SummerWatermelon)]
    [DisplayName("重置夏日西瓜")]
    public void ResetSummerWatermelon()
    {
        HideDebugPanel();
        StorageManager.Instance.GetStorage<StorageHome>().SummerWatermelon.Clear();
        SummerWatermelonModel.Instance.UnSetItemsCount = 0;
        var guideIdList = new List<int>() {546,547,548,549,550,551,552};
        CleanGuideList(guideIdList);
    }

    [Category(SummerWatermelon)]
    [DisplayName("获取该等级的活动棋子")]
    public void GetSummerWatermelonItem()
    {
        SummerWatermelonModel.Instance.UnSetItems.Add(SummerWatermelonModel.Instance.GetMergeItemConfig(summerWatermelonItemLevel).id);
        SummerWatermelonModel.Instance.UnSetItemsCount++;
    }
    private int summerWatermelonItemLevel;
    [Category(SummerWatermelon)]
    [DisplayName("设置棋子等级")]
    public int SetSummerWatermelonItemLevel
    {
        get
        {
            return summerWatermelonItemLevel;
        }
        set
        {
            summerWatermelonItemLevel = value;
        }
    }
}