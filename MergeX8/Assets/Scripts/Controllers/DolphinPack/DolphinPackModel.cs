using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using Merge.Order;
using UnityEngine;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class DolphinPackModel : Singleton<DolphinPackModel>
{
    private const int _shopId = 5011;
    
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

    public StorageDolphinPack storageDolphinPack
    {
        get
        {
            return StorageManager.Instance.GetStorage<StorageHome>().DolphinPack;
        }
    }

    public bool CanShowInStore()
    {
        if (MainOrderManager.Instance.IsFinishDolphinTask())
            return false;
        
        if (!MainOrderManager.Instance.IsDolphinActiveTask())
            return false;

        if (IsActiveTimeEnd())
            return false;
        
        if (storageDolphinPack.IsFinish)
            return false;
        return storageDolphinPack.LastPopUpTime > 0;
    }

    public bool IsActiveTimeEnd()
    {
        if (storageDolphinPack.FinishTime <= 0)
            return false;

        return (long)APIManager.Instance.GetServerTime() > storageDolphinPack.FinishTime;
    }
    
    public string GetActiveTime()
    {
        long diffValue = storageDolphinPack.FinishTime - (long)APIManager.Instance.GetServerTime();
        if (diffValue < 0)
            return "00:00";
        
        return CommonUtils.FormatLongToTimeStr(diffValue);
    }
    
    public void PurchaseSuccess()
    {
        List<ResData> listResData = new List<ResData>();

        for (int i = 0; i < _tableBundle.bundleItemList.Length; i++)
        {
            if (!UserData.Instance.IsResource(_tableBundle.bundleItemList[i]))
            {
                var config = GameConfigManager.Instance.GetItemConfig(_tableBundle.bundleItemList[i]);
                GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                {
                    MergeEventType = BiEventCooking.Types.MergeEventType.MergeChangeReasonDolphinPackage,
                    itemAId = config.id,
                    ItemALevel = config.level,
                    isChange = true,
                }); 
            }
          
            ResData resData = new ResData(_tableBundle.bundleItemList[i], _tableBundle.bundleItemCountList[i]);
            listResData.Add(resData);

        }
     
        storageDolphinPack.IsFinish = true;
        storageDolphinPack.LastPopUpTime = (long)APIManager.Instance.GetServerTime();
    
        UIDolphinPackController  controller = UIManager.Instance.GetOpenedUIByPath(UINameConst.UIDolphinPack) as UIDolphinPackController;
        if (controller != null)
            controller.ClickUIMask();
        var reasonArgs = new GameBIManager.ItemChangeReasonArgs(BiEventCooking.Types.ItemChangeReason.DolphinPackage);
        EventDispatcher.Instance.DispatchEvent(EventEnum.PURCHASE_SUCCESS_REWARD, listResData);

        CommonRewardManager.Instance.PopCommonReward(listResData, CurrencyGroupManager.Instance.GetCurrencyUseController(), true, reasonArgs, animEndCall:() =>
        {
        });
    }
}
