using System;
using System.Collections.Generic;
using System.Linq;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using Gameplay.UI;
using UnityEngine.Networking;

public class NewDailyPackModel : Manager<NewDailyPackModel>
{
    public StorageNewDailyPack packData
    {
        get { return StorageManager.Instance.GetStorage<StorageHome>().NewDailyBundleData; }
    }

    public bool IsOpen()
    {
        if (packData.IsFinish)
            return false;
        if (packData.StartTime <= 0)
            return false;
        if (!IsHaveConifg())
            return false;
        if (GetCurrentPackage() == null)
            return false;
        return true;
    }

    public bool IsFinish()
    {
        if (packData.IsFinish)
            return true;

        return false;
    }

    public bool IsHaveConifg()
    { 
        var common = AdConfigHandle.Instance.GetCommon();
        if (common.NewDailyPackPrice < 0)
            return false;
        
        if (packData.PackInfo.Count <= 0)
            return false;
        return true;
    }

    public TableNewDailyPackLevelChangeConfig MatchPriceRules()
    {
        var rules = GlobalConfigManager.Instance.GetNewDailyPackLevelChangeConfig();
        //昨日是否付费
        int isPayLastDay = 0;
        if (packData.GotShopItem.Count > 0)
        {
            packData.UnPayDays = 0;
            isPayLastDay = 1;
            packData.PayTime++;
        }
        else
        {
            if (packData.IsInit)
                packData.UnPayDays++;
        }

        foreach (var rule in rules)
        {
            if (packData.LastPriceChange != rule.last_price_change)
                continue;
       
            if (isPayLastDay != rule.is_pay_last_show)
                continue;
            if (rule.pay_times > 0 && packData.PayTime != rule.pay_times)
                continue;
            if (packData.UnPayDays != rule.unpay_show_days)
                continue;

            DebugUtil.Log("------动态礼包-----匹配到 TableDailyPackPriceRules" + rule.id);
            return rule;
        }

        return null;
    }

    public int GetCurPackageLevel()
    {
        var groupId = AdConfigHandle.Instance.GetCommon().NewDailyPackPrice;
        var dailyPackPrice = GlobalConfigManager.Instance.GetNewDailyPackGroupConfig(groupId);
        int price = dailyPackPrice.levelList[packData.CurrentPrice];
        return price;
    }
    public void RefreshPack()
    {
        if (IsCanRefreshPack())
        {
            // var groupId = AdConfigHandle.Instance.GetCommon().NewDailyPackPrice;
            // var dailyPackPrice = GlobalConfigManager.Instance.GetNewDailyPackGroupConfig(groupId);
            // if (dailyPackPrice == null)
            //     return;
            // var rules = MatchPriceRules();
            //
            // //生成价格和折扣----------------------------
            // if (!packData.IsInit)
            // {
            //     packData.CurrentPrice = dailyPackPrice.startPrice;
            //     packData.IsInit = true;
            // }
            //
            // if (rules != null && 
            //     !(packData.BuyFlag && packData.CurrentPrice + rules.price_change < dailyPackPrice.endPrice))
            // {
            //     packData.CurrentPrice = packData.CurrentPrice + rules.price_change;
            //     if (packData.CurrentPrice < 0)
            //         packData.CurrentPrice = 0;
            //     if (packData.CurrentPrice >= dailyPackPrice.levelList.Length)
            //         packData.CurrentPrice = dailyPackPrice.levelList.Length - 1;
            //     packData.UnPayDays = 0;
            //     packData.PayTime = 0;
            //     packData.LastPriceChange = rules.price_change;   
            // }
            //
            // if (packData.CurrentPrice > dailyPackPrice.levelList.Length - 1)
            //     packData.CurrentPrice = 0;
            //
            // //生成内容--------------------------------------
            // int price = dailyPackPrice.levelList[packData.CurrentPrice];
            var groupId = PayLevelModel.Instance.GetCurPayLevelConfig().NewDailyPackGroupId;
            var dailyPack = GlobalConfigManager.Instance.GetNewDailyPackLevelConfig(groupId);
            packData.PackInfo.Clear();
            
            for (int i = 0; i < dailyPack.packageList.Length; i++)
            {
                StorageNewDailyPackItem item = new StorageNewDailyPackItem();
                var packInfo = GlobalConfigManager.Instance.GetNewDailyPackPackageConfig(dailyPack.packageList[i]);
                for (int j = 0; j < packInfo.contain.Length; j++)
                { 
                    item.Id.Add(packInfo.contain[j]);
                    item.Count.Add(packInfo.containCount[j]);
                }
                item.Shopid = packInfo.shopId;
                item.PackId = dailyPack.id;
                item.PackInfoId = packInfo.id;
                // item.LabelNum = packInfo.labelNum;
                packData.PackInfo.Add(item);
            }
            packData.StartTime = CommonUtils.GetTimeStamp() / 1000;
            packData.GotShopItem.Clear();
            packData.IsFinish = false;
            EventDispatcher.Instance.DispatchEventImmediately(EventEnum.NewDaily_Pack_Time_REFRESH);

        }
    }

    
    public bool IsCanRefreshPack()
    {
        var common = AdConfigHandle.Instance.GetCommon();
        if (common.NewDailyPackPrice < 0)
            return false;
        
        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.NewDailyPack))
            return false;
        if (packData.StartTime <= 0 || packData.PackInfo.Count <= 0)
            return true;
        if (!Utils.IsSameDay(packData.StartTime, CommonUtils.GetTimeStamp() / 1000))
            return true;
        return false;
    }

    public int GetPackCoolTime()
    {
        if (IsOpen() == false)
            return 0;
        long leftTime = Utils.GetTomorrowTimestamp() - CommonUtils.GetTimeStamp() / 1000;

        return Math.Max(0, (int) leftTime);
    }

    public int GetShowPackCoolTime()
    {
        long leftTime = Utils.GetTomorrowTimestamp() - CommonUtils.GetTimeStamp() / 1000;
        return Math.Max(0, (int) leftTime);
    }

    public bool IsCanBuyItem(int shopID)
    {
        return !packData.GotShopItem.Contains(shopID);
    }

    public List<int> GetCanBuyPackageIdList()
    {
        var packageIdList = new List<int>();
        if (!IsOpen())
            return packageIdList;
        foreach (var package in packData.PackInfo)
        {
            if (IsCanBuyItem(package.Shopid))
                packageIdList.Add(package.PackInfoId);
        }
        return packageIdList;
    }

    public void RecordBuyItem(int shopID)
    {
        if (!packData.GotShopItem.Contains(shopID))
        {
            packData.GotShopItem.Add(shopID);
            EventDispatcher.Instance.DispatchEvent(EventEnum.NewDaily_Pack_REFRESH, shopID);
        }

        if (packData.GotShopItem.Count >= packData.PackInfo.Count)
        {
            packData.IsFinish = true;
            EventDispatcher.Instance.DispatchEvent(EventEnum.NewDaily_Pack_Finish);
        }
    }

    public int GetPopTimes()
    {
        if (!Utils.IsSameDay(packData.LastPopUpTime / 1000, CommonUtils.GetTimeStamp() / 1000))
        {
            packData.PopTimes = 0;
        }

        return packData.PopTimes;
    }

    public void RecordOpenState()
    {
        EventDispatcher.Instance.DispatchEvent(EventEnum.NewDaily_Pack_Begin);

        packData.LastPopUpTime = CommonUtils.GetTimeStamp();
        packData.PopTimes++;
    }

    public void RvSuccess(int packInfoId,string openSrc)
    {
        var popup = UIManager.Instance.GetOpenedUIByPath<UIPopupNewDailyGiftController>(UINameConst.UIPopupNewDailyGift);
        if (popup)
            popup.AnimCloseWindow();
        List<ResData> listResData = new List<ResData>();
        StorageNewDailyPackItem dailyPackItem=null;
        
        foreach (var item in packData.PackInfo)
        {
            if (item.PackInfoId == packInfoId)
            {
                dailyPackItem = item;
                for (int i = 0; i < item.Id.Count; i++)
                {
                    ResData res = new ResData(item.Id[i], item.Count[i]);
                    listResData.Add(res);
                    if (!UserData.Instance.IsResource(res.id))
                    {
                        TableMergeItem mergeItemConfig = GameConfigManager.Instance.GetItemConfig(res.id);
                        if (mergeItemConfig != null)
                        {
                            GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                            {
                                MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonDailyDeals,
                                itemAId = mergeItemConfig.id,
                                ItemALevel = mergeItemConfig.level,
                                isChange = true,
                            });
                        }
                    }
                }

                break;
            }
        }
        
        
        if (listResData == null || listResData.Count == 0)
            return;
        var reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.NewDailyDealsSuccess;
        EventDispatcher.Instance.DispatchEvent(EventEnum.PURCHASE_SUCCESS_REWARD, listResData);
        CommonRewardManager.Instance.PopCommonReward(listResData,
            CurrencyGroupManager.Instance.GetCurrencyUseController(), true, new GameBIManager.ItemChangeReasonArgs()
            {
                reason = reason,
            }
            , () =>
            {
                PayRebateModel.Instance.OnPurchaseAniFinish();
                PayRebateLocalModel.Instance.OnPurchaseAniFinish();
                // var nextPackage = GetCurrentPackage();
                // if (nextPackage != null && openSrc == "pack")
                // {
                //     var autoPopup = new BackHomeControl.AutoPopUI(ContinuePopupUI,new[] {UINameConst.UIPopupDailyGift, UINameConst.UIPopupReward});
                //     BackHomeControl.PushExtraPopup(autoPopup);
                // }
            });
        
        var extras2 = new Dictionary<string, string>();
        extras2.Clear();
        if (dailyPackItem != null)
        {
            extras2.Add("PackID", dailyPackItem.PackId.ToString());
            for (int i = 0; i < dailyPackItem.Id.Count; i++)
            {
                if(!extras2.ContainsKey(dailyPackItem.Id[i].ToString()))
                    extras2.Add(dailyPackItem.Id[i].ToString(),dailyPackItem.Count[i].ToString());
            }
            extras2.Add(dailyPackItem.Id.ToString(),dailyPackItem.Count.ToString());
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventNewDailyDealsSuccess,data1:openSrc,data2:dailyPackItem.PackInfoId.ToString());
        }
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventDailyDealsSuccess,
            openSrc);

        if (dailyPackItem != null)
        {
            NewDailyPackageExtraRewardModel.Instance.PurchasePackage(dailyPackItem.PackInfoId);
            RecordBuyItem(dailyPackItem.Shopid);
        }
        var existExtraView = UIPopupExtraView.CheckExtraViewOpenState<NewDailyPackExtraView>();
        if (existExtraView)
            GameBIManager.Instance.SendGameEvent(
                BiEventAdventureIslandMerge.Types.GameEventType.GameEventBpbuyLifeUseup,data1:"0");
        
    }
    public void PurchaseSuccess(TableShop cfg,string openSrc)
    {
        packData.BuyFlag = true;
        var popup = UIManager.Instance.GetOpenedUIByPath<UIPopupNewDailyGiftController>(UINameConst.UIPopupNewDailyGift);
        if (popup)
            popup.AnimCloseWindow();
        List<ResData> listResData = new List<ResData>();
        StorageNewDailyPackItem dailyPackItem=null;
        foreach (var item in packData.PackInfo)
        {
            if (item.Shopid == cfg.id)
            {
                dailyPackItem = item;
                for (int i = 0; i < item.Id.Count; i++)
                {
                    ResData res = new ResData(item.Id[i], item.Count[i]);
                    listResData.Add(res);
                    if (!UserData.Instance.IsResource(res.id))
                    {
                        TableMergeItem mergeItemConfig = GameConfigManager.Instance.GetItemConfig(res.id);
                        if (mergeItemConfig != null)
                        {
                            GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                            {
                                MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonDailyDeals,
                                itemAId = mergeItemConfig.id,
                                ItemALevel = mergeItemConfig.level,
                                isChange = true,
                            });
                        }
                    }
                }

                break;
            }
        }

        if (listResData == null || listResData.Count == 0)
            return;
        var reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.NewDailyDealsSuccess;
        EventDispatcher.Instance.DispatchEvent(EventEnum.PURCHASE_SUCCESS_REWARD, listResData);
        CommonRewardManager.Instance.PopCommonReward(listResData,
            CurrencyGroupManager.Instance.GetCurrencyUseController(), true, new GameBIManager.ItemChangeReasonArgs()
            {
                reason = reason,
            }
            , () =>
            {
                PayRebateModel.Instance.OnPurchaseAniFinish();
                PayRebateLocalModel.Instance.OnPurchaseAniFinish();
                // var nextPackage = GetCurrentPackage();
                // if (nextPackage != null && openSrc == "pack")
                // {
                //     var autoPopup = new BackHomeControl.AutoPopUI(ContinuePopupUI,new[] {UINameConst.UIPopupDailyGift, UINameConst.UIPopupReward});
                //     BackHomeControl.PushExtraPopup(autoPopup);
                // }
            });
        
        var extras2 = new Dictionary<string, string>();
        extras2.Clear();
        if (dailyPackItem != null)
        {
            extras2.Add("PackID", dailyPackItem.PackId.ToString());
            for (int i = 0; i < dailyPackItem.Id.Count; i++)
            {
                if(!extras2.ContainsKey(dailyPackItem.Id[i].ToString()))
                    extras2.Add(dailyPackItem.Id[i].ToString(),dailyPackItem.Count[i].ToString());
            }
            extras2.Add(dailyPackItem.Id.ToString(),dailyPackItem.Count.ToString());
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventNewDailyDealsSuccess,data1:openSrc,data2:dailyPackItem.PackInfoId.ToString());
        }
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventDailyDealsSuccess,
            openSrc);

        if (dailyPackItem != null)
        {
            NewDailyPackageExtraRewardModel.Instance.PurchasePackage(dailyPackItem.PackInfoId);
        }
        RecordBuyItem(cfg.id);
        var existExtraView = UIPopupExtraView.CheckExtraViewOpenState<NewDailyPackExtraView>();
        if (existExtraView)
            GameBIManager.Instance.SendGameEvent(
                BiEventAdventureIslandMerge.Types.GameEventType.GameEventBpbuyLifeUseup,data1:"0");
    }

    public bool ContinuePopupUI()
    {
        UIManager.Instance.OpenUI(UINameConst.UIPopupNewDailyGift, "pack");
        return true;
    }
    public void ClearPack()
    {
        packData.Clear();

        CoolingTimeManager.Instance.RemoveCooling(CoolingTimeManager.CDType.OtherDay, UIPopupNewDailyGiftController.coolTimeKey);
    }

    public void DebugRefresh(int dailyPackID)
    {
        var dailyPack = GlobalConfigManager.Instance.GetNewDailyPackLevelConfig(dailyPackID);
        packData.PackInfo.Clear();
            
        for (int i = 0; i < dailyPack.packageList.Length; i++)
        {
            StorageNewDailyPackItem item = new StorageNewDailyPackItem();
            var packInfo = GlobalConfigManager.Instance.GetNewDailyPackPackageConfig(dailyPack.packageList[i]);
            for (int j = 0; j < packInfo.contain.Length; j++)
            { 
                item.Id.Add(packInfo.contain[j]);
                item.Count.Add(packInfo.containCount[j]);
            }
            item.Shopid = packInfo.shopId;
            item.PackId = dailyPack.id;
            item.PackInfoId = packInfo.id;
            // item.LabelNum = packInfo.labelNum;
            packData.PackInfo.Add(item);
        }
        packData.StartTime = CommonUtils.GetTimeStamp() / 1000;
        packData.GotShopItem.Clear();
        packData.IsFinish = false;
        EventDispatcher.Instance.DispatchEventImmediately(EventEnum.NewDaily_Pack_Time_REFRESH);

    }

    public StorageNewDailyPackItem GetCurrentPackage()
    {
        for (var i = 0; i < packData.PackInfo.Count; i++)
        {
            if (packData.GotShopItem.Contains(packData.PackInfo[i].Shopid))
                continue;
            return packData.PackInfo[i];
        }
        return null;
    }
}