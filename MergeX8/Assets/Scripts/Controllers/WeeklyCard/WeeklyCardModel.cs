using System;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Framework;
using Gameplay;
using Gameplay.UI;
using UnityEngine;

public class WeeklyCardModel : Manager<WeeklyCardModel>
{
    public int HourOffset = 0;
    public StorageWeeklyCard GetStorageWeeklyCard(int weekCardId)
    {
        var storage = StorageManager.Instance.GetStorage<StorageHome>().WeeklyCard;
        if (!storage.ContainsKey(weekCardId.ToString()))
            storage.Add(weekCardId.ToString(), new StorageWeeklyCard());
        return storage[weekCardId.ToString()];
    }
    
    public void PurchaseSuccess(TableShop tableShop)
    {
        var weekCardCfg= GlobalConfigManager.Instance.tableWeeklyCards.Find(a => a.shopId == tableShop.id);
        
        var rewards = CommonUtils.FormatReward(weekCardCfg.firstReward, weekCardCfg.firstRewardNum);
        EventDispatcher.Instance.DispatchEvent(EventEnum.PURCHASE_SUCCESS_REWARD, rewards);
        CommonRewardManager.Instance.PopCommonReward(rewards,CurrencyGroupManager.Instance.GetCurrencyUseController(), 
            true, new GameBIManager.ItemChangeReasonArgs()
            {
                reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.Iap,
                data1 = tableShop.id.ToString()
            }
            , () =>
            {
                PayRebateModel.Instance.OnPurchaseAniFinish();
                PayRebateLocalModel.Instance.OnPurchaseAniFinish();
            });
        var storageWeeklyCard = GetStorageWeeklyCard(weekCardCfg.id);
        storageWeeklyCard.BuyState = true;
        storageWeeklyCard.IsNewBuy = true;
        storageWeeklyCard.BuyTime = CommonUtils.GetTimeStamp();
        EventDispatcher.Instance.DispatchEvent(EventEnum.WEEKLYCARD_PURCHASE);

    }

    public bool IsPurchase(int weekCardId)
    {
        var storageWeeklyCard = GetStorageWeeklyCard(weekCardId);
        return storageWeeklyCard.BuyState;
    }

    public bool IsHaveCanClaim()
    {
        foreach (var weeklyCard in GlobalConfigManager.Instance.tableWeeklyCards)
        {
            if (IsCanClaim(weeklyCard))
                return true;
        }
        return false;
    }
    
    public bool IsCanClaim(TableWeeklyCard weeklyCard)
    {
        var storageWeeklyCard = GetStorageWeeklyCard(weeklyCard.id);
        //没有买
        if (!storageWeeklyCard.BuyState)
            return false;
        
        //买了后未领取过
        if (storageWeeklyCard.LastClaimTime == 0)
            return true;
        
        return GetCanClaimDays(weeklyCard)>0;
    }

    public int GetCanClaimDays(TableWeeklyCard weeklyCard)
    {
        var storageWeeklyCard = GetStorageWeeklyCard(weeklyCard.id);
        if (!storageWeeklyCard.BuyState)
            return 0;
        //买了后未领取过
        if (storageWeeklyCard.LastClaimTime == 0)
            return 1;
        //剩余天
        int leftDays= weeklyCard.RewardDays - storageWeeklyCard.TotalClaimCount;
        
        int today = (int) ((CommonUtils.GetTimeStamp() - HourOffset * 3600 * 1000) / (24 * 3600 * 1000));
        int lastClaimDay = (int) (storageWeeklyCard.LastClaimTime / (24 * 3600 * 1000));
        int days = today - lastClaimDay;
        return Mathf.Min(leftDays, days);
    }

    public bool IsOld(TableWeeklyCard weeklyCard)
    {
        return IsPurchase(weeklyCard.id) && !GetStorageWeeklyCard(weeklyCard.id).IsNewBuy;
    }

    public void Claim(TableWeeklyCard weeklyCard)
    {
        var storageWeeklyCard = GetStorageWeeklyCard(weeklyCard.id);

        int days = GetCanClaimDays(weeklyCard);
        List<ResData> rewards = new List<ResData>();

        for (int i = 0; i < days; i++)
        {
            if (WeeklyCardModel.Instance.IsOld(weeklyCard))
            {
                rewards.Add(new ResData(weeklyCard.oldReward,weeklyCard.oldCount));
            }
            else
            {
                rewards.Add(new ResData(weeklyCard.everydayReward[storageWeeklyCard.TotalClaimCount+i],weeklyCard.everydayRewardNum[storageWeeklyCard.TotalClaimCount+i]));
            }
            // rewards.AddRange(CommonUtils.FormatReward(weeklyCard.everydayReward,weeklyCard.everydayRewardNum));
        }
        CommonRewardManager.Instance.PopCommonReward(rewards,CurrencyGroupManager.Instance.GetCurrencyUseController(), 
            true, new GameBIManager.ItemChangeReasonArgs()
            {
                reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.Iap,
                data1 = weeklyCard.shopId.ToString()
            }
            , () =>
            {
                PayRebateModel.Instance.OnPurchaseAniFinish();
                PayRebateLocalModel.Instance.OnPurchaseAniFinish();
            });

        storageWeeklyCard.TotalClaimCount += days;
        storageWeeklyCard.LastClaimTime = CommonUtils.GetTimeStamp();

        if (storageWeeklyCard.TotalClaimCount >= weeklyCard.RewardDays)
        {
            storageWeeklyCard.FinishTime= CommonUtils.GetTimeStamp();
        }
        
        EventDispatcher.Instance.DispatchEvent(EventEnum.WEEKLYCARD_GETREWARD);
    }

    public bool IsFinish(TableWeeklyCard weeklyCard,int day)
    {
        var storageWeeklyCard = GetStorageWeeklyCard(weeklyCard.id);
        if (storageWeeklyCard.TotalClaimCount > day)
            return true;
    
        return false;
    }
    public bool IsCanClaim(TableWeeklyCard weeklyCard,int day)
    {
        var storageWeeklyCard = GetStorageWeeklyCard(weeklyCard.id);
        if (!storageWeeklyCard.BuyState)
            return false;
        if (storageWeeklyCard.TotalClaimCount > day)
            return false;
        int canClaimDay = GetCanClaimDays(weeklyCard);
        if (day < storageWeeklyCard.TotalClaimCount + canClaimDay)
            return true;
        return false;
    }

    public void RefreshWeeklyCard()
    {
        foreach (var weeklyCard in GlobalConfigManager.Instance.tableWeeklyCards)
        {
            var storageWeeklyCard = GetStorageWeeklyCard(weeklyCard.id);
            if (storageWeeklyCard.FinishTime > 0)
            {
                if ((CommonUtils.GetTimeStamp() - HourOffset * 3600 * 1000)/ (24 * 3600 * 1000)!=
                    (storageWeeklyCard.FinishTime- HourOffset * 3600 * 1000)/ (24 * 3600 * 1000))
                {
                    storageWeeklyCard.Clear();
                }
            }
        }
    }
    public bool IsBuy()
    {
        foreach (var weeklyCard in GlobalConfigManager.Instance.tableWeeklyCards)
        {
            var storageWeeklyCard = GetStorageWeeklyCard(weeklyCard.id);
            if (storageWeeklyCard.BuyState)
                return true;
        }

        return false;
    }
    public string GetRestTimeString() 
    {
        long leftTime = Utils.GetTomorrowTimestamp() + HourOffset * 3600 - CommonUtils.GetTimeStamp() / 1000;
        return CommonUtils.FormatLongToTimeStr((int)leftTime*1000);
       
    }
}