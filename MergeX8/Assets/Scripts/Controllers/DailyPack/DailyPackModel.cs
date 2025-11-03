using System;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using Gameplay.UI;

public class DailyPackModel : Manager<DailyPackModel>
{
    public StorageDailyPack packData
    {
        get { return StorageManager.Instance.GetStorage<StorageHome>().DailyBundleData; }
    }
    public int GetCurPackageLevel()
    {
        return AdConfigHandle.Instance.GetDailyPackPrice().Groupid[packData.CurrentPrice];
    }
    public bool IsOpen()
    {
        // return false;
        if (!PayLevelModel.Instance.GetCurPayLevelConfig().OpenOldDailyPack)
            return false;
        var common = AdConfigHandle.Instance.GetCommon();
        if (common.DailyPackContain < 0)
            return false;
        
        if (packData.IsFinish)
            return false;
        if (packData.StartTime <= 0)
        {
            return false;
        }

        if (!IsHaveConifg())
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
        if (common.DailyPackContain < 0)
            return false;
        
        if (packData.PackInfo.Count <= 0)
            return false;
        return true;
    }

    public DailyPackPriceRules MatchPriceRules()
    {
        var rules = AdConfigHandle.Instance.GetDailyPackPriceRules();
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
            if (packData.LastPriceChange != rule.Last_price_change)
                continue;
       
            if (isPayLastDay != rule.Is_pay_last_show)
                continue;
            if (rule.Pay_times > 0 && packData.PayTime != rule.Pay_times)
                continue;
            if (packData.UnPayDays != rule.Unpay_show_days)
                continue;

            DebugUtil.Log("------动态礼包-----匹配到 TableDailyPackPriceRules" + rule.Id);
            return rule;
        }

        return null;
    }

    public void RefreshPack()
    {
        if (IsCanRefreshPack())
        {
            // var dailyPackPrice = AdConfigHandle.Instance.GetDailyPackPrice();
            // if (dailyPackPrice == null)
            //     return;
            // var rules = MatchPriceRules();
            //
            // //生成价格和折扣----------------------------
            // if (!packData.IsInit)
            // {
            //     packData.CurrentPrice = dailyPackPrice.Start_price;
            //     packData.IsInit = true;
            // }
            //
            // if (rules != null)
            // {
            //     packData.CurrentPrice = packData.CurrentPrice + rules.Price_change;
            //     if (packData.CurrentPrice < 0)
            //         packData.CurrentPrice = 0;
            //     if (packData.CurrentPrice >= dailyPackPrice.Groupid.Count)
            //         packData.CurrentPrice = dailyPackPrice.Groupid.Count - 1;
            //     packData.UnPayDays = 0;
            //     packData.PayTime = 0;
            //     packData.LastPriceChange = rules.Price_change;
            // }
            //
            // if (packData.CurrentPrice > dailyPackPrice.Groupid.Count - 1)
            //     packData.CurrentPrice = 0;
            //
            // //生成内容--------------------------------------
            // int price = dailyPackPrice.Groupid[packData.CurrentPrice];
            var groupId = PayLevelModel.Instance.GetCurPayLevelConfig().DailyPackGroupId;
            var dailyPack = AdConfigHandle.Instance.GetDailyPack(groupId);
            packData.PackInfo.Clear();
            
            for (int i = 0; i < dailyPack.Contain.Count; i++)
            {
                StorageDailyPackItem item = new StorageDailyPackItem();
                var packInfo = AdConfigHandle.Instance.GetDailyPackInfoById(dailyPack.Contain[i]);
                for (int j = 0; j < packInfo.Contain.Count; j++)
                { 
                    item.Id.Add(packInfo.Contain[j]);
                    item.Count.Add(packInfo.Contain_num[j]);
                }
                item.Shopid = dailyPack.Shopid[i];
                item.PackId = dailyPack.Id;
                item.PackInfoId = packInfo.Id;
                packData.PackInfo.Add(item);
            }
            packData.StartTime = CommonUtils.GetTimeStamp() / 1000;
            packData.GotShopItem.Clear();
            packData.IsFinish = false;
            EventDispatcher.Instance.DispatchEventImmediately(EventEnum.Daily_Pack_Time_REFRESH);

        }
    }

    
    public bool IsCanRefreshPack()
    {
        var common = AdConfigHandle.Instance.GetCommon();
        if (common.DailyPackContain < 0)
            return false;
        
        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.DailyPack))
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

    public void RecordBuyItem(int shopID)
    {
        if (!packData.GotShopItem.Contains(shopID))
        {
            packData.GotShopItem.Add(shopID);
            EventDispatcher.Instance.DispatchEvent(EventEnum.Daily_Pack_REFRESH, shopID);
        }

        if (packData.GotShopItem.Count >= packData.PackInfo.Count)
        {
            packData.IsFinish = true;
            EventDispatcher.Instance.DispatchEvent(EventEnum.Daily_Pack_Finish);
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
        EventDispatcher.Instance.DispatchEvent(EventEnum.Daily_Pack_Begin);

        packData.LastPopUpTime = CommonUtils.GetTimeStamp();
        packData.PopTimes++;
    }

    public void PurchaseSuccess(TableShop cfg,string openSrc)
    {
        List<ResData> listResData = new List<ResData>();
        StorageDailyPackItem dailyPackItem=null;
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
        var reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.DailyDeals;
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
        }
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventDailyDealsSuccess,
            openSrc);

        RecordBuyItem(cfg.id);
        var existExtraView = UIPopupExtraView.CheckExtraViewOpenState<DailyPackExtraView>();
        if (existExtraView)
            GameBIManager.Instance.SendGameEvent(
                BiEventAdventureIslandMerge.Types.GameEventType.GameEventBpbuyLifeUseup,data1:"0");
    }

    public void ClearPack()
    {
        packData.Clear();

        CoolingTimeManager.Instance.RemoveCooling(CoolingTimeManager.CDType.OtherDay, "MaterialPack");
    }

    public void DebugRefresh(int dailyPackID)
    {
        var dailyPack = AdConfigHandle.Instance.GetDailyPack(dailyPackID);
        packData.PackInfo.Clear();
            
        for (int i = 0; i < dailyPack.Contain.Count; i++)
        {
            StorageDailyPackItem item = new StorageDailyPackItem();
            var packInfo = AdConfigHandle.Instance.GetDailyPackInfoById(dailyPack.Contain[i]);
            for (int j = 0; j < packInfo.Contain.Count; j++)
            { 
                item.Id.Add(packInfo.Contain[j]);
                item.Count.Add(packInfo.Contain_num[j]);
            }
            item.Shopid = dailyPack.Shopid[i];
            item.PackId = dailyPack.Id;
            item.PackInfoId = packInfo.Id;
            packData.PackInfo.Add(item);
        }
        packData.StartTime = CommonUtils.GetTimeStamp() / 1000;
        packData.GotShopItem.Clear();
        packData.IsFinish = false;
        EventDispatcher.Instance.DispatchEventImmediately(EventEnum.Daily_Pack_Time_REFRESH);

    }
}