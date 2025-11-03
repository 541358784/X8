using System.Collections.Generic;
using System.ComponentModel;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using UnityEngine;


public partial class SROptions
{
    private const string SeaRacing = "海上竞速";
    [Category(SeaRacing)]
    [DisplayName("重置")]
    public void ResetSeaRacing()
    {
        HideDebugPanel();
        StorageManager.Instance.GetStorage<StorageHome>().SeaRacing.Clear();
        var guideIdList = new List<int>() {670,671,672,673,674,675};
        CleanGuideList(guideIdList);
        SeaRacingModel.Instance.CreateStorage();
    }

    [Category(SeaRacing)]
    [DisplayName("设置星星数量")]
    public int SetSeaRacingStarCount
    {
        get
        {
            return SeaRacingModel.Instance.GetStar();
        }
        set
        {
            SeaRacingModel.Instance.AddStar(value-SeaRacingModel.Instance.GetStar());
        }
    }
    
    [Category(SeaRacing)]
    [DisplayName("设置结束时间")]
    public int SetSeaRacingCurWorldEndTime
    {
        get
        {
            if (SeaRacingModel.Instance.CurStorageSeaRacingWeek == null)
                return 0;
            return (int)SeaRacingModel.Instance.CurStorageSeaRacingWeek.GetLeftTime()/1000;
        }
        set
        {
            SeaRacingModel.Instance.CurStorageSeaRacingWeek?.SetLeftTime((long)value*1000);
        }
    }
    
    
    [Category(SeaRacing)]
    [DisplayName("设置开始时间")]
    public int SetSeaRacingCurWorldPreheatTime
    {
        get
        {
            if (SeaRacingModel.Instance.CurStorageSeaRacingWeek == null)
                return 0;
            return (int)SeaRacingModel.Instance.CurStorageSeaRacingWeek.GetPreheatLeftTime()/1000;
        }
        set
        {
            SeaRacingModel.Instance.CurStorageSeaRacingWeek?.SetPreheatLeftTime((long)value*1000);
        }
    }
}