using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;

public static class BuyDiamondTicketUtils
{
    public static string GetTicketLeftTimeText(this StorageBuyDiamondTicket storage)
    {
        return CommonUtils.FormatLongToTimeStr(storage.GetTicketLeftTime());
    }
    public static long GetTicketLeftTime(this StorageBuyDiamondTicket storage)
    {
        return storage.EndTime - (long)APIManager.Instance.GetServerTime();
    }

    public static TableBuyDiamondTicket GetTicketConfig(this StorageBuyDiamondTicket storage)
    {
        return BuyDiamondTicketModel.Instance.Config[storage.TicketId];
    }

    public static int GetExtraDiamondCount(this TableBuyDiamondTicket config,int shopId)
    {
        var triggerIndex = -1;
        for (var i = 0; i < config.shopId.Length; i++)
        {
            if (config.shopId[i] == shopId)
            {
                triggerIndex = i;
                break;
            }
        }
        if (triggerIndex < 0)
        {
            return 0;
        }
        return config.diamond[triggerIndex];
    }
}