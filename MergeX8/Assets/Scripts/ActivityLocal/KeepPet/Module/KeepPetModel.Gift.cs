
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.KeepPet;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;

public partial class KeepPetModel
{
    public string GetGiftRestTimeString() 
    {
        long leftTime = GetGiftRestTime();
        return CommonUtils.FormatLongToTimeStr((int)leftTime*1000);
    }   
        
    public long GetGiftRestTime() 
    {
        long leftTime = Utils.GetTomorrowTimestamp() - CommonUtils.GetTimeStamp() / 1000;
        return leftTime;
    }

    public void UpdateGiftBagTimeState()
    {
        if (!CommonUtils.IsSameDayWithToday((ulong) Storage.StoreRefreshTime))
        {
            Storage.StoreBuyTimes=0;
            Storage.ThreeOneStoreBuyState = false;
            Storage.StoreRefreshTime = (long)APIManager.Instance.GetServerTime();
        }   
    }
    public KeepPetStoreConfig GetCurrentKeepPetStoreConfig()
    {
        UpdateGiftBagTimeState();
        int index = Storage.StoreBuyTimes <= KeepPetConfigManager.Instance.GetConfig<KeepPetStoreConfig>().Count-1
            ? Storage.StoreBuyTimes
            : KeepPetConfigManager.Instance.GetConfig<KeepPetStoreConfig>().Count-1;

        return KeepPetConfigManager.Instance.GetConfig<KeepPetStoreConfig>()[index];
    }
    
    public KeepPetStoreConfig GetKeepPetStoreConfigByShopId(int shopId)
    {
        return KeepPetConfigManager.Instance.GetConfig<KeepPetStoreConfig>().Find(a => a.ShopId == shopId);
    }

    public List<KeepPetThreeOneStoreConfig> ThreeOneStoreConfig =>
        KeepPetConfigManager.Instance.GetConfig<KeepPetThreeOneStoreConfig>();
    public void PurchaseThreeOneSuccess(TableShop cfg)
    {
        Storage.ThreeOneStoreBuyState = true;
        var rewards = new List<ResData>();
        if (GlobalConfig.ThreeOneShopId == cfg.id)
        {
            foreach (var config in ThreeOneStoreConfig)
            {
                var reward = CommonUtils.FormatReward(config.RewardId, config.RewardCount);
                rewards.AddRange(reward);
            }
        }
        else
        {
            var config = ThreeOneStoreConfig.Find(a => a.ShopId == cfg.id);
            var reward = CommonUtils.FormatReward(config.RewardId, config.RewardCount);
            rewards.AddRange(reward);
        }
        PopReward(rewards);
        EventDispatcher.Instance.DispatchEvent(EventEnum.PURCHASE_SUCCESS_REWARD, rewards);
        EventDispatcher.Instance.DispatchEvent(EventEnum.KEEPPET_GIFT_PURCHASE);
    }
    public void PurchaseSuccess(TableShop cfg)
    {
        Storage.StoreBuyTimes++;
        var config=   GetKeepPetStoreConfigByShopId(cfg.id);
        var res= CommonUtils.FormatReward(config.RewardId, config.RewardCount);
        EventDispatcher.Instance.DispatchEvent(EventEnum.PURCHASE_SUCCESS_REWARD, res);
        PopReward(res);
        EventDispatcher.Instance.DispatchEvent(EventEnum.KEEPPET_GIFT_PURCHASE);
    }
    
    public void PopReward(List<ResData> listResData)
    {
        if (listResData == null || listResData.Count <= 0)
            return;
        int count = listResData.Count > 8 ? 8 : listResData.Count;
        var list = listResData.GetRange(0, count);
        listResData.RemoveRange(0, count);
        var reasonArgs = new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.Iap);
        CommonRewardManager.Instance.PopCommonReward(list, CurrencyGroupManager.Instance.GetCurrencyUseController(), true, reasonArgs, animEndCall: () => { PopReward(listResData); });
    }
}