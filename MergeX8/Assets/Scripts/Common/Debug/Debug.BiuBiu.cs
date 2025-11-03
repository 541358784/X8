using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using UnityEngine;


public partial class SROptions
{
    private const string BiuBiu = "幽默飞镖人";
    [Category(BiuBiu)]
    [DisplayName("重置")]
    public void ResetBiuBiu()
    {
        HideDebugPanel();
        StorageManager.Instance.GetStorage<StorageHome>().BiuBiu.Clear();
        StorageManager.Instance.GetStorage<StorageGame>().MergeBoards.Remove((int) MergeBoardEnum.BiuBiu);
        CoolingTimeManager.Instance.RemoveCooling(CoolingTimeManager.CDType.OtherDay, "BiuBiu");
        var guideIdList = new List<int>() {4461,4462,4463,4464};
        CleanGuideList(guideIdList);
    }

    [Category(BiuBiu)]
    [DisplayName("获取该等级的活动棋子")]
    public void GetBiuBiuItem()
    {
        BiuBiuModel.Instance.Storage.UnSetItems.Add(BiuBiuModel.Instance.GetMergeItemConfig(biuBiuItemLevel).id);
    }
    public int biuBiuItemLevel;
    [Category(BiuBiu)]
    [DisplayName("设置棋子等级")]
    public int BiuBiuItemLevel
    {
        get
        {
            return biuBiuItemLevel;
        }
        set
        {
            biuBiuItemLevel = value;
        }
    }

    [Category(BiuBiu)]
    [DisplayName("检查气球配置是否合规")]
    public void CheckBiuBiuConfig()
    {
        if (!BiuBiuModel.Instance.IsInitFromServer())
            return;
        var errCount = 0;
        foreach (var pair1 in BiuBiuModel.Instance.FateConfigDic)
        {
            foreach (var config in pair1.Value)
            {
                var resultDic = new Dictionary<int, int>();
                foreach (var result in config.Fate)
                {
                    var resultKey = result>0?result:-result;
                    resultDic.TryAdd(resultKey, 0);
                    resultDic[resultKey]++;
                }

                var resultKeys = resultDic.Keys.ToList();
                foreach (var resultKey in resultKeys)
                {
                    if (BiuBiuModel.Instance.UIConfigDic[resultKey].ShowIndex.Count < resultDic[resultKey])
                    {
                        Debug.LogError("配置错误 id=" + config.Id + " 气球" + resultKey + "数量为" + resultDic[resultKey]);
                        errCount++;
                    }
                }
            }
        }

        if (errCount == 0)
            Debug.LogError("配置无错误");
    }
    
    
    [Category(BiuBiu)]
    [DisplayName("使用中的顺序")]
    public string BiuBiuCurFateStorage
    {
        get
        {
            return BiuBiuModel.Instance.Storage.Fate.ToLogString();   
        }
    }
}