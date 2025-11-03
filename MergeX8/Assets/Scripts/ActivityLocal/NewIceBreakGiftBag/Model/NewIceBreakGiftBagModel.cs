using System.Collections.Generic;
using ABTest;
using DragonPlus;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;

public partial class NewIceBreakGiftBagModel : Manager<NewIceBreakGiftBagModel>
{
    private const string AbKey = "NewIceBreakGiftBagABKey";

    public bool IsOpenAB()
    {
        if (!StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig.ContainsKey(AbKey))
        {
            StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig[AbKey] =
                ABTestManager.Instance.IsOpenNewIceBreak() ? "1" : "0";
        }

        bool isOpen = StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig[AbKey] == "1";

        if (!StorageManager.Instance.GetStorage<StorageCommon>().Abtests.ContainsKey(AbKey))
        {
            StorageManager.Instance.GetStorage<StorageCommon>().Abtests[AbKey] = isOpen ? "1" : "0";
        }

        return isOpen;
    }
    public bool IsOpen => false;//IsOpenAB() && !Storage.IsFinish;
    
    
    
    public string GetAuxItemAssetPath()
    {
        return "Prefabs/Home/Aux_NewIceBreakGiftBag";
    }
    public bool ShowAuxItem()
    {
        return IsOpen && (long)APIManager.Instance.GetServerTime() <= Storage.EndTime;
    }

    public Aux_BuyDiamondTicket GetAuxItem()
    {
        return Aux_BuyDiamondTicket.Instance;
    }
    public StorageNewIceBreakGiftBag Storage => StorageManager.Instance.GetStorage<StorageHome>().NewIceBreakGiftBag;

    public List<TableNewIceBreakGiftBag> ContentConfig => GlobalConfigManager.Instance.TableNewIceBreakGiftBagList;
    public TableNewIceBreakGiftBag GetGiftBagConfig(int id)
    {
        return ContentConfig.Find(a => a.id == id);
    }
    protected override void InitImmediately()
    {
        TMatch.Timer.Register(1, UpdateTime, null, true);
    }
    
    public void UpdateTime()
    {
        UpdateStorage();
    }
    
    public void UpdateStorage()
    {
        if (!IsOpen)
            return;
        var nextPackage = GetGiftBagConfig(Storage.GiftBagId + 1);
        if (StorageManager.Instance.GetStorage<StorageHome>().PayMaxAmount > 0)
            nextPackage = null;
        if (nextPackage != null && ExperenceModel.Instance.GetLevel() >= nextPackage.unlockLevel)
        {
            Storage.GiftBagId = nextPackage.id;
            Storage.EndTime = (long)APIManager.Instance.GetServerTime() + nextPackage.packageTime * (long)XUtility.Min;
            UpdateStorage();
            return;
        }
        if (nextPackage == null && (long)APIManager.Instance.GetServerTime() > Storage.EndTime)
        {
            Storage.IsFinish = true;
            return;
        }
    }
    public string GetLeftTimeText()
    {
        return CommonUtils.FormatLongToTimeStr(GetLeftTime());
    }
    public long GetLeftTime()
    {
        return Storage.EndTime - (long)APIManager.Instance.GetServerTime();
    }
    
    
    
    
    
    public static bool CanShowNewIceBreakGiftBagOnLevelUp()
    {
        if (!Instance.IsOpen)
            return false;
        Instance.UpdateStorage();
        var config = Instance.GetGiftBagConfig(Instance.Storage.GiftBagId);
        if (config == null)
            return false;
        if (Instance.GetLeftTime() <= 0)
            return false;
        if (config.unlockLevel != ExperenceModel.Instance.GetLevel())
            return false;
        return UIPopupNewbiePackController.Open();
    }

    private const string coolTimeKey = "NreIceBreakGiftBagCDKey";
    public static bool CanShowNewIceBreakGiftBagOnBackHome()
    {
        if (!Instance.IsOpen)
            return false;
        Instance.UpdateStorage();
        var config = Instance.GetGiftBagConfig(Instance.Storage.GiftBagId);
        if (config == null)
            return false;
        if (Instance.GetLeftTime() <= 0)
            return false;
        var cdkey = coolTimeKey+"BackHome" + config.id;
        if (CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, cdkey))
            return false;
        if (UIPopupNewbiePackController.Open())
        {
            CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, cdkey,
                CommonUtils.GetTimeStamp());
            return true;
        }

        return false;
    }
    public static bool CanShowNewIceBreakGiftBagOnEnterGame()
    {
        if (!Instance.IsOpen)
            return false;
        Instance.UpdateStorage();
        var config = Instance.GetGiftBagConfig(Instance.Storage.GiftBagId);
        if (config == null)
            return false;
        if (Instance.GetLeftTime() <= 0)
            return false;
        var cdkey = coolTimeKey+"EnterGame" + config.id;
        if (CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, cdkey))
            return false;
        if (UIPopupNewbiePackController.Open())
        {
            CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, cdkey,
                CommonUtils.GetTimeStamp());
            return true;
        }

        return false;
    }

    public void PurchaseSuccess(TableShop shopConfig)
    {
        var config = ContentConfig.Find(a => a.shopId == shopConfig.id);
        if (config == null)
        {
            Debug.LogError("这是个报错");
        }
        Storage.IsFinish = true;
        List<ResData> rewards = null;
        rewards = CommonUtils.FormatReward(config.itemId, config.ItemNum);
        if (rewards != null)
        {
            foreach (var reward in rewards)
            {
                if (!UserData.Instance.IsResource(reward.id))
                {
                    GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                    {
                        MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonNewIceBreakGet,
                        itemAId = reward.id,
                        data1 = reward.count.ToString(),
                        isChange = true,
                    }); 
                }
            }
            var popup = UIPopupNewbiePackController.Instance;
            if (popup)
                popup.AnimCloseWindow();
            EventDispatcher.Instance.DispatchEvent(EventEnum.PURCHASE_SUCCESS_REWARD, rewards);
            CommonRewardManager.Instance.PopCommonReward(rewards,CurrencyGroupManager.Instance.GetCurrencyUseController(), 
                true, new GameBIManager.ItemChangeReasonArgs()
                {
                    reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.NewIceBreak,
                }
                , () =>
                {
                    PayRebateModel.Instance.OnPurchaseAniFinish();
                    PayRebateLocalModel.Instance.OnPurchaseAniFinish();
                });
        }
    }
    
}