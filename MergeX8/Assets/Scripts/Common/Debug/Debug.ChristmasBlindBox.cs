using System.Collections.Generic;
using System.ComponentModel;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;

public partial class SROptions
{
    private const string ChristmasBlindBox = "圣诞盲盒礼包";
    [Category(ChristmasBlindBox)]
    [DisplayName("重制")]
    public void ResetChristmasBlindBox()
    {
        HideDebugPanel();
        StorageManager.Instance.GetStorage<StorageHome>().ChristmasBlindBox.Clear();
        if (ChristmasBlindBoxModel.Instance.IsInitFromServer())
            ChristmasBlindBoxModel.Instance.InitStorage();
        var guideIdList = new List<int>() {};
        CleanGuideList(guideIdList);
    }
}