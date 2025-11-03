using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using Merge.Order;
using Scripts.UI;
using UnityEngine;


public partial class SROptions
{
    private const string Team = "0公会";
    [Category(Team)]
    [DisplayName("公会筹")]
    public int TeamCoin
    {
        get
        {
            return TeamManager.Instance.GetCoin();
        }
        set
        {
            TeamManager.Instance.AddCoin(value - TeamManager.Instance.GetCoin(),"Debug");
        }
    }
    
    [Category(Team)]
    [DisplayName("体力")]
    public int TeamLife
    {
        get
        {
            return TeamManager.Instance.GetLife();
        }
        set
        {
            TeamManager.Instance.AddLife(value - TeamManager.Instance.GetLife(),"Debug");
        }
    }
    
    [Category(Team)]
    [DisplayName("重置引导")]
    public void ClearTeamGuide()
    {
        var guideList = new List<int>()
        {
            4580,
            4581,
            4582,
            4583,
            4584,
            4585,
            4586,
            4587,
            4588,
        };
        CleanGuideList(guideList);
        ResetTeamOrder();
    }
    [Category(Team)]
    [DisplayName("清理卡牌领取记录")]
    public void ClearTeamCardCollectState()
    {
        TeamManager.Instance.Storage.ClaimCardState.Clear();
        TeamManager.Instance.GetBattlePassGiftList(null);
    }
    [Category(Team)]
    [DisplayName("清空公会商店购买记录")]
    public void ClearTeamShopBuyState()
    {
        TeamManager.Instance.Storage.BuyState.Clear();
    }

    [Category(Team)]
    [DisplayName("重置公会任务")]
    public void ResetTeamOrder()
    {
        TeamManager.Instance.Storage.RefreshOrderTime = 0;
        MainOrderManager.Instance.RemoveOrder(MainOrderType.Team);
    }
    
    [Category(Team)]
    [DisplayName("卡牌领取记录")]
    public string TeamCardCollectState
    {
        get
        {
            return TeamManager.Instance.Storage.ClaimCardState.ToLogString();
        }
    }
    
    [Category(Team)]
    [DisplayName("公会礼包列表")]
    public string TeamCardGiftList
    {
        get
        {
            var giftList = new List<long>();
            foreach (var info in TeamManager.Instance.PassGiftInfoList)
            {
                giftList.Add(info.GiftId);
            }
            return giftList.ToLogString();
        }
    }
    
    [Category(Team)]
    [DisplayName("发送垃圾信息50条")]
    public void TeamSendGarbageMessage()
    {
        if (TeamManager.Instance.HasTeam())
        {
            for (var i = 0; i < 50; i++)
            {
                var tempI = i;
                XUtility.WaitSeconds(tempI * 0.5f, () =>
                {
                    TeamManager.Instance.SendTeamChat("垃圾信息" + tempI, null);
                });
            }
        }
    }
}