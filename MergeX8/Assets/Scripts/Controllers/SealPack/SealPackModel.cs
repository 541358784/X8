using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Merge.Order;
using UnityEngine;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class SealPackModel : Singleton<SealPackModel>
{
    private const int _shopId = 5001;
    
    private TableBundle _tableBundle;

    public TableBundle BundleData
    {
        get
        {
            if(_tableBundle == null)
                _tableBundle = GlobalConfigManager.Instance.GetTableBundleByShopID(_shopId);

            return _tableBundle;
        }
    }

    public StorageSealPack storageSealPack
    {
        get
        {
            return StorageManager.Instance.GetStorage<StorageHome>().SealPack;
        }
    }

    public bool CanShowInStore()
    {
        if (MainOrderManager.Instance.IsFinishSealTask())
            return false;
        
        if (!MainOrderManager.Instance.IsSealActiveTask())
            return false;

        if (IsActiveTimeEnd())
            return false;
        
        if (storageSealPack.IsFinish)
            return false;
        return storageSealPack.LastPopUpTime > 0;
    }

    public bool IsActiveTimeEnd()
    {
        if (storageSealPack.FinishTime <= 0)
            return false;

        return (long)APIManager.Instance.GetServerTime() > storageSealPack.FinishTime;
    }
    
    public string GetActiveTime()
    {
        long diffValue = storageSealPack.FinishTime - (long)APIManager.Instance.GetServerTime();
        if (diffValue < 0)
            return "00:00";
        
        return CommonUtils.FormatLongToTimeStr(diffValue);
    }
    
    public void PurchaseSuccess()
    {
        var config = GameConfigManager.Instance.GetItemConfig(_tableBundle.bundleItemList[0]);
        if(config == null)
            return;
        
        GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
        {
            MergeEventType = BiEventCooking.Types.MergeEventType.MergeChangeReasonSealBuildingBuy,
            itemAId = config.id,
            ItemALevel = config.level,
            isChange = true,
        });

        storageSealPack.IsFinish = true;
        storageSealPack.LastPopUpTime = (long)APIManager.Instance.GetServerTime();

        List<ResData> listResData = new List<ResData>();
        ResData resData = new ResData(_tableBundle.bundleItemList[0], _tableBundle.bundleItemCountList[0]);
        listResData.Add(resData);
        
        UISealPackController  controller = UIManager.Instance.GetOpenedUIByPath(UINameConst.UISealPack) as UISealPackController;
        if (controller != null)
            controller.ClickUIMask();
        EventDispatcher.Instance.DispatchEvent(EventEnum.PURCHASE_SUCCESS_REWARD, listResData);
        CommonRewardManager.Instance.PopCommonReward(listResData, CurrencyGroupManager.Instance.GetCurrencyUseController(), true, animEndCall:() =>
        {
        });
    }
}
