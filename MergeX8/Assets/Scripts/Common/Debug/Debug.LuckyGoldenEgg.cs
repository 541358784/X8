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
    private const string LuckyGoldenEgg = "幸运金蛋";
    [Category(LuckyGoldenEgg)]
    [DisplayName("清除数据")]
    public void ClearLuckyGoldenEggStorage()
    {
        StorageManager.Instance.GetStorage<StorageHome>().LuckyGoldenEgg.Clear();
        var guideIdList = new List<int>() {911,912,913};
        CleanGuideList(guideIdList);
        
        CoolingTimeManager.Instance.RemoveCooling(CoolingTimeManager.CDType.OtherDay,"LuckyGoldenEgg");
        CoolingTimeManager.Instance.RemoveCooling(CoolingTimeManager.CDType.OtherDay,"coolTimeKey_Preheating");
        
    }
   
    [Category(LuckyGoldenEgg)]
    [DisplayName("设置蛋数量")]
    public int SetEgg
    {
        get
        {
            return UserData.Instance.GetRes(UserData.ResourceId.GoldenEgg);
        }
        set
        {
            UserData.Instance.AddRes((int)UserData.ResourceId.GoldenEgg,value,
                new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug));
        }
    }
  
}
