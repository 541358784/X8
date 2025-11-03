using System.Collections.Generic;
using Dlugin;
using DragonPlus;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;

public class EnergyPackageModel : Manager<EnergyPackageModel>
{
    private StorageEnergyPackage _storageEnergyPackage;

    public StorageEnergyPackage StorageEnergyPackage
    {
        get
        {
            if (_storageEnergyPackage == null)
            {
                _storageEnergyPackage= StorageManager.Instance.GetStorage<StorageHome>().EnergyPackage;
            }
           
            return _storageEnergyPackage;
        }
    }

    private TableEnergyPackage _energyPackage;
    public TableEnergyPackage EnergyPackageConfig
    {
        get
        {
            if (_energyPackage == null)
            {
                _energyPackage= GlobalConfigManager.Instance.GetEnergyPackage();
            }
           
            return _energyPackage;
        }
    }
    public ulong GetPackageLeftTime()
    {
        var left = (long) EnergyPackageConfig.packageCD*60*1000 -((long)APIManager.Instance.GetServerTime()-StorageEnergyPackage.LastInCdTime);
        if (left < 0)
            left = 0;
        return (ulong) left;
    }

    public virtual string GetGetPackageLeftTimeString()
    {
        return CommonUtils.FormatLongToTimeStr((long) GetPackageLeftTime());
    }

    public void PurchaseSuccess(TableShop shop)
    {
        StorageEnergyPackage.IsPurchase = true;
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventEnergyPackageSuccess,shop.id.ToString(),StorageEnergyPackage.PopTimes.ToString());
        var config = GlobalConfigManager.Instance.GetEnergyPackageByID(shop.id);
        if (config == null)
            return;
        var ret = new List<ResData>();
        for (int i = 0; i < config.itemId.Length; i++)
        {
            ret.Add(new ResData(config.itemId[i],config.ItemNum[i]));
        }
        var reasonArgs =
            new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.EnergyPackageBuy);
        UIPopupEnergyBuyController mainController = UIManager.Instance.GetOpenedUIByPath(UINameConst.UIPopupEnergyBuy) as UIPopupEnergyBuyController;
        if (mainController != null)
        {
            mainController.AnimCloseWindow();
        }
        EventDispatcher.Instance.DispatchEvent(EventEnum.PURCHASE_SUCCESS_REWARD, ret);

        CommonRewardManager.Instance.PopCommonReward(ret, CurrencyGroupManager.Instance.currencyController, true,
            reasonArgs, () =>
            {
               
            });
    }

    public bool IsUnlock => UnlockManager.IsOpen(UnlockManager.MergeUnlockType.EnergyPack);
    public bool CanShow()
    {
        // var storageCommon= StorageManager.Instance.GetStorage<StorageCommon>();
        // if (storageCommon.LastRevenueTime <= 0 )
        //     return false;
        if (!IsUnlock)
            return false;
        //走长 CD
        if (StorageEnergyPackage.IsPurchase || StorageEnergyPackage.PopTimes >= EnergyPackageConfig.dayPop)
        {
            long cd = EnergyPackageConfig.longCd * 60 * 1000;
            if(StorageEnergyPackage.IsPurchase )
                cd=EnergyPackageConfig.payCd * 60 * 1000;
            var left =  cd -((long)APIManager.Instance.GetServerTime()-StorageEnergyPackage.LastInCdTime);
            if (left <=0)
            {
                StorageEnergyPackage.LastInCdTime = (long)APIManager.Instance.GetServerTime();
                StorageEnergyPackage.GroupId.Clear();
                foreach (var id in PayLevelModel.Instance.GetCurPayLevelConfig().EnergyPackageGroupId)
                {
                    StorageEnergyPackage.GroupId.Add(id);
                }
                StorageEnergyPackage.PopTimes = 1;
                StorageEnergyPackage.IsPurchase = false;
                UIManager.Instance.OpenUI(UINameConst.UIPopupEnergyBuy);
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventEnergyPackageFirstPopRound,StorageEnergyPackage.PopTimes.ToString());
                StorageEnergyPackage.OnPackPopTimes = 1;
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventEnergyPackageFirstPopTimes,StorageEnergyPackage.OnPackPopTimes.ToString());

                return true;
            }
        }
        else
        {
            if (!CommonUtils.IsSameDayWithToday((ulong) StorageEnergyPackage.LastInCdTime))
                StorageEnergyPackage.PopTimes = 0;
            var left = GetPackageLeftTime();
            if (left > 0)
            {
                StorageEnergyPackage.OnPackPopTimes ++;
                UIManager.Instance.OpenUI(UINameConst.UIPopupEnergyBuy);
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventEnergyPackageFirstPopTimes,StorageEnergyPackage.OnPackPopTimes.ToString());
                return true;
            }
            else
            {
                var left2 = (long) EnergyPackageConfig.shortCd*60*1000 -((long)APIManager.Instance.GetServerTime()-StorageEnergyPackage.LastInCdTime);
                if (StorageEnergyPackage.LastInCdTime == 0 || left2<=0)
                {
                    StorageEnergyPackage.LastInCdTime = (long)APIManager.Instance.GetServerTime();
                    StorageEnergyPackage.GroupId.Clear();
                    foreach (var id in PayLevelModel.Instance.GetCurPayLevelConfig().EnergyPackageGroupId)
                    {
                        StorageEnergyPackage.GroupId.Add(id);
                    }
                    StorageEnergyPackage.PopTimes ++;
                    UIManager.Instance.OpenUI(UINameConst.UIPopupEnergyBuy);
                    StorageEnergyPackage.OnPackPopTimes = 1;
                    GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventEnergyPackageFirstPopTimes,StorageEnergyPackage.OnPackPopTimes.ToString());
                    GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventEnergyPackageFirstPopRound,StorageEnergyPackage.PopTimes.ToString());

                    return true;
                }
            }
        }
        
        return false;
    }

    public bool IsCanShow()
    {
        if (!IsUnlock)
            return false;
        if (StorageEnergyPackage.IsPurchase)
            return false;

        var left = GetPackageLeftTime();
        if (left > 0)
        {
              return true;
        }
         
        return false;
    }
}
