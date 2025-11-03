using System.Collections.Generic;
using System.ComponentModel;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;

public partial class SROptions
{
    private const string NewDailyPackageExtraReward = "新每日礼包补丁";
    [Category(NewDailyPackageExtraReward)]
    [DisplayName("重制")]
    public void ResetNewDailyPackageExtraReward()
    {
        HideDebugPanel();
        StorageManager.Instance.GetStorage<StorageHome>().NewDailyPackageExtraReward.Clear();
        var guideIdList = new List<int>(){};
        CleanGuideList(guideIdList);
    }
}