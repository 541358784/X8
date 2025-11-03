using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using UnityEngine;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class EasterPackModel : Singleton<EasterPackModel>
{
    private const int _shopId = 6001;
    
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

    private StorageEasterPack _storageEasterPack;

    public StorageEasterPack StorageEasterPack
    {
        get
        {
            var storage = StorageManager.Instance.GetStorage<StorageHome>().EasterPack;
            if (_storageEasterPack == null)
            {
                if (!storage.ContainsKey(EasterModel.Instance.StorageKey))
                    storage.Add(EasterModel.Instance.StorageKey, new StorageEasterPack());
                _storageEasterPack = storage[EasterModel.Instance.StorageKey];
            }
           
            return _storageEasterPack;
        }
    }
   

    public bool CanShowInStore()
    {
        if (!EasterModel.Instance.IsOpened())
            return false;
        
        if (StorageEasterPack.IsFinish)
            return false;
        if (!EasterModel.Instance.StorageEaster.IsShowStartView)
            return false;

        return StorageEasterPack.LastPopUpTime > 0;
    }
    
    public string GetActiveTime()
    {
        long diffValue =(long) EasterModel.Instance.EndTime - (long)APIManager.Instance.GetServerTime();
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
            MergeEventType = BiEventCooking.Types.MergeEventType.MergeChangeReasonEasterBuildingBuy,
            itemAId = config.id,
            ItemALevel = config.level,
            isChange = true,
        });
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventEasterPackagePop);

        StorageEasterPack.IsFinish = true;
        StorageEasterPack.LastPopUpTime = (long)APIManager.Instance.GetServerTime();

        List<ResData> listResData = new List<ResData>();
        ResData resData = new ResData(_tableBundle.bundleItemList[0], _tableBundle.bundleItemCountList[0]);
        listResData.Add(resData);
        
        UIEasterShopController  controller = UIManager.Instance.GetOpenedUIByPath(UINameConst.UIEasterShop) as UIEasterShopController;
        if (controller != null)
            controller.ClickUIMask();
        EventDispatcher.Instance.DispatchEvent(EventEnum.PURCHASE_SUCCESS_REWARD, listResData);

        CommonRewardManager.Instance.PopCommonReward(listResData, CurrencyGroupManager.Instance.GetCurrencyUseController(), true, animEndCall:() =>
        {
        });
    }
}
