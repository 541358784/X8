using System.Collections.Generic;
using System.ComponentModel;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using UnityEngine;


public partial class SROptions
{
    private const string CoinLeaderBoard = "金币排行榜";
    [Category(CoinLeaderBoard)]
    [DisplayName("重置回收金币")]
    public void ResetCoinLeaderBoard()
    {
        HideDebugPanel();
        StorageManager.Instance.GetStorage<StorageHome>().CoinLeaderBoard.StorageByWeek.Clear();
        StorageManager.Instance.GetStorage<StorageHome>().CoinLeaderBoard.LastFinishNodeList.Clear();
        var guideIdList = new List<int>() {532,533,534,535,536};
        CleanGuideList(guideIdList);
    }

    [Category(CoinLeaderBoard)]
    [DisplayName("设置星星数量")]
    public int SetCoinLeaderBoardStarCount
    {
        get
        {
            return CoinLeaderBoardModel.Instance.GetStar();
        }
        set
        {
            CoinLeaderBoardModel.Instance.AddStar(value-CoinLeaderBoardModel.Instance.GetStar());
        }
    }
    
    [Category(CoinLeaderBoard)]
    [DisplayName("设置结束时间")]
    public int SetCoinLeaderBoardCurWorldEndTime
    {
        get
        {
            if (CoinLeaderBoardModel.Instance.CurStorageCoinLeaderBoardWeek == null)
                return 0;
            return (int)CoinLeaderBoardModel.Instance.CurStorageCoinLeaderBoardWeek.GetLeftTime()/1000;
        }
        set
        {
            CoinLeaderBoardModel.Instance.CurStorageCoinLeaderBoardWeek?.SetLeftTime((long)value*1000);
        }
    }
}