using System.Collections.Generic;
using System.ComponentModel;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using UnityEngine;


public partial class SROptions
{
    [Category(RecoverCoin)]
    [DisplayName("重置回收金币")]
    public void ResetRecoverCoin()
    {
        HideDebugPanel();
        StorageManager.Instance.GetStorage<StorageHome>().RecoverCoin.StorageByWeek.Clear();
        StorageManager.Instance.GetStorage<StorageHome>().RecoverCoin.LastFinishNodeList.Clear();
        var guideIdList = new List<int>() {532,533,534,535,536};
        CleanGuideList(guideIdList);
    }

    [Category(RecoverCoin)]
    [DisplayName("设置星星数量")]
    public int SetStarCount
    {
        get
        {
            return RecoverCoinModel.Instance.GetStar();
        }
        set
        {
            RecoverCoinModel.Instance.AddStar(value-RecoverCoinModel.Instance.GetStar());
        }
    }
    
    [Category(RecoverCoin)]
    [DisplayName("设置结束时间")]
    public int SetCurWorldEndTime
    {
        get
        {
            if (RecoverCoinModel.Instance.CurStorageRecoverCoinWeek == null)
                return 0;
            return (int)RecoverCoinModel.Instance.CurStorageRecoverCoinWeek.GetLeftTime()/1000;
        }
        set
        {
            RecoverCoinModel.Instance.CurStorageRecoverCoinWeek?.SetLeftTime((long)value*1000);
        }
    }
    
    [Category(RecoverCoin)]
    [DisplayName("设置开始时间")]
    public int SetCurWorldStartTime
    {
        get
        {
            if (RecoverCoinModel.Instance.CurStorageRecoverCoinWeek != null)
                return 0;
            return (int)RecoverCoinModel.Instance.GetNextWeekStartTime()/1000;
        }
        set
        {
            RecoverCoinModel.Instance.SetNextWeekStartTime((long)value*1000);
        }
    }
    
    [Category(RecoverCoin)]
    [DisplayName("机器人区间ID")]
    public int RobotIndex
    {
        get
        {
            if (RecoverCoinModel.Instance.CurStorageRecoverCoinWeek == null)
                return -1;
            return (int)RecoverCoinModel.Instance.CurStorageRecoverCoinWeek.RobotIndex;
        }
        set
        {
            
        }
    }

}