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
    [Category(TreasureHunt)]
    [DisplayName("清除数据")]
    public void ClearTreasureHuntStorage()
    {
        StorageManager.Instance.GetStorage<StorageHome>().TreasureHunt.Clear();
        var guideIdList = new List<int>() {901,902,903};
        CleanGuideList(guideIdList);
        CoolingTimeManager.Instance.RemoveCooling(CoolingTimeManager.CDType.OtherDay,"TreasureHunt");

    }
   
    [Category(TreasureHunt)]
    [DisplayName("设置锤子")]
    public int SetHammer
    {
        get
        {
            return UserData.Instance.GetRes(UserData.ResourceId.Hammer);
        }
        set
        {
            UserData.Instance.AddRes((int)UserData.ResourceId.Hammer,value,
                new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug));
        }
    }
  
}
