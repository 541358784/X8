using System.Collections.Generic;
using System.ComponentModel;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;

public partial class SROptions
{
    private const string BlindBox = "盲盒";
    [Category(BlindBox)]
    [DisplayName("重制")]
    public void ResetBlindBox()
    {
        HideDebugPanel();
        StorageManager.Instance.GetStorage<StorageHome>().BlindBox.Clear();
        var guideIdList = new List<int>();
        CleanGuideList(guideIdList);
    }

    [Category(BlindBox)]
    [DisplayName("盲盒id")]
    public int BlindBoxResourceId
    {
        get;
        set;
    } = 751;

    [Category(BlindBox)]
    [DisplayName("获得盲盒")]
    public void GetBlindBox()
    {
        if (BlindBoxModel.Instance.BoxConfigDic.ContainsKey(BlindBoxResourceId))
        {
            var reason = new GameBIManager.ItemChangeReasonArgs()
            {
                reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug
            };
            UserData.Instance.AddRes(BlindBoxResourceId,1,reason);
        }
        else
        {
            Debug.LogError("盲盒id错误");
        }
    }
    
    [Category(BlindBox)]
    [DisplayName("奖品id")]
    public int BlindBoxItemId
    {
        get;
        set;
    } = 1001;
    
    [Category(BlindBox)]
    [DisplayName("获得奖品")]
    public void GetBlindBoxItem()
    {
        if (BlindBoxModel.Instance.ItemConfigDic.ContainsKey(BlindBoxItemId))
        {
            var item = BlindBoxModel.Instance.ItemConfigDic[BlindBoxItemId];
            var storage = BlindBoxModel.Instance.GetStorage(item.ThemeId);
            var themeConfig = BlindBoxModel.Instance.ThemeConfigDic[storage.ThemeId];
            var specialConfig = themeConfig.GetSpecialItemConfigs();
            storage.TotalCollectTimes++;
            storage.CurCollectTimes++;
            var isNew = storage.CollectItem(item);
            if (isNew && !specialConfig.Contains(item))
            {
                storage.CurCollectTimes = 0;
            }
        }
        else
        {
            Debug.LogError("奖品id错误");
        }
    }
}