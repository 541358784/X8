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
    private const string DogPlay = "潘虹";
    [Category(DogPlay)]
    [DisplayName("清档")]
    public void DogPlayClearStorage()
    {
        StorageManager.Instance.GetStorage<StorageHome>().DogPlay.Clear();
        if (DogPlayModel.Instance.LastOpenState)
            DogPlayModel.Instance.InitStorage();
        var guideIdList = new List<int>() {4443,4444};
        CleanGuideList(guideIdList);
    }
}
