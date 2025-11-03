using System.Collections.Generic;
using System.ComponentModel;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;

public partial class SROptions
{
    private const string BuyDiamondTicket = "钻石券";
    [Category(BuyDiamondTicket)]
    [DisplayName("重制")]
    public void ResetBuyDiamondTicket()
    {
        HideDebugPanel();
        StorageManager.Instance.GetStorage<StorageHome>().BuyDiamondTicket.Clear();
        var guideIdList = new List<int>(){};
        CleanGuideList(guideIdList);
    }
    private int _buyDiamondTicketId = 932;
    [Category(BuyDiamondTicket)]
    [DisplayName("券id")]
    public int BuyDiamondTicketId
    {
        get
        {
            return _buyDiamondTicketId;
        }
        set
        {
            if (!BuyDiamondTicketModel.Instance.IsTicket(_buyDiamondTicketId))
                return;
            _buyDiamondTicketId = value;
        }
    }

    [Category(BuyDiamondTicket)]
    [DisplayName("获得券")]
    public void BuyDiamondTicketGet()
    {
        if (!BuyDiamondTicketModel.Instance.IsTicket(_buyDiamondTicketId))
            return;
        var reward = new List<ResData>()
        {
            new ResData(_buyDiamondTicketId, 1)
        };
        UserData.Instance.AddRes(reward,
            new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug));
    }
}