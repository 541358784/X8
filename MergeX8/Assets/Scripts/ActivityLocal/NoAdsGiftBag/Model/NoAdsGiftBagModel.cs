using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;

public partial class NoAdsGiftBagModel:Manager<NoAdsGiftBagModel>
{
    public StorageNoAdsGiftBag Storage => StorageManager.Instance.GetStorage<StorageHome>().NoAdsGiftBag;
    long CurTime => (long)APIManager.Instance.GetServerTime();
    private List<TableNoAdsGiftBag> ConfigList => GlobalConfigManager.Instance.NoAdsGiftBagList;

    public bool TryCreate()
    {
        if (Storage.ShowTime > 0)
            return false;
        Storage.ShowTime++;
        Storage.PayLevelGroup = PayLevelModel.Instance.GetCurPayLevelConfig().NoAdsGiftBag;
        var config = GetConfig();
        Storage.StartTime = CurTime;
        Storage.EndTime = Storage.StartTime + config.activeTime * (long)XUtility.Min;
        return true;
    }

    public TableNoAdsGiftBag GetConfig()
    {
        var config =
            ConfigList.Find(a => a.payLevelGroup == PayLevelModel.Instance.GetCurPayLevelConfig().NoAdsGiftBag);
        if (config == null)
            config = ConfigList.Find(a => a.payLevelGroup == 0);
        return config;
    }

    public bool IsActive()
    {
        if (Storage.ShowTime == 0)
            return false;
        if (CurTime > Storage.EndTime)
            return false;
        if (Storage.BuyState)
            return false;
        return true;
    }
    public bool TryShow()
    {
        if (!IsActive())
            return false;
        if (Storage.HasShow)
            return false;
        Storage.HasShow = true;
        UIPopupNoADSController.Open();
        return true;
    }

    public void PurchaseSuccess(TableShop shopConfig)
    {
        var config = GetConfig();
        if (config == null)
            return;
        if (config.shopId != shopConfig.id)
            return;
        StorageManager.Instance.GetStorage<StorageHome>().ShopData.GotNoAds = true;
        Storage.BuyState = true;
        var rewards = CommonUtils.FormatReward(config.itemId, config.ItemNum);
        var reason = new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.Iap);
        UserData.Instance.AddRes(rewards,reason);
        CommonRewardManager.Instance.PopCommonReward(rewards, CurrencyGroupManager.Instance.currencyController, false,
            reason);
        // if (UIPopupNoADSController.Instance)
        //     UIPopupNoADSController.Instance.AnimCloseWindow();
    }

    public string LeftTimeString()
    {
        return CommonUtils.FormatLongToTimeStr(Storage.EndTime - CurTime);
    }
}