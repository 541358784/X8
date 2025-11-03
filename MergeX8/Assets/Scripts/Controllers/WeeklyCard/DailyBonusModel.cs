using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using DragonU3DSDK.Storage;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK;
using Framework;
using Gameplay;
using Gameplay.UI.Store.Vip.Model;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class DailyBonusModel : Manager<DailyBonusModel>
{
    public const int DailyDays = 7;

    public enum BonusState
    {
        NotCanClaim,
        CanClaim,
        NextDayCanClaim,
        Claimed
    };

    public bool IsDailyBonusPopingUp()
    {
        return UIManager.Instance.GetOpenedUIByPath(UINameConst.UIDailyBouns) != null;
    }

    public bool CheckIsHaveCanClaim()
    {
        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.DailyBonus))
            return false;
        for (int i = 1; i <= DailyBonusModel.DailyDays; ++i)
        {
            var state = DailyBonusModel.Instance.GetDailyBonusState(i);
            if (state == DailyBonusModel.BonusState.CanClaim)
            {
                return true;
            }
        }

        return false;
    }

    public bool IsOpen()
    {
        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.DailyBonus))
            return false;

        return true;
    }

    public int HourOffset = 0;
    public int CheckConsecutiveLoginDays()
    {
        var storageHome = StorageManager.Instance.GetStorage<StorageHome>();
        int today = (int) ((CommonUtils.GetTimeStamp() - HourOffset * 3600 * 1000) / (24 * 3600 * 1000));
        int days = today - storageHome.DailyBonus.LastLoginDay;
        int consecutiveLoginDays = storageHome.DailyBonus.ConsecutiveLoginDays;
        if (days >= 1)
        {
            consecutiveLoginDays++;
        }
        // else if (days > 1)
        // {
        //     consecutiveLoginDays = 1;
        // }

        storageHome.DailyBonus.LastLoginDay = today;
        storageHome.DailyBonus.ConsecutiveLoginDays = consecutiveLoginDays;
        return consecutiveLoginDays;
    }
    public string GetRestTimeString() 
    {
        long leftTime = Utils.GetTomorrowTimestamp() + HourOffset*3600 - CommonUtils.GetTimeStamp() / 1000;

        return TimeUtils.GetTimeString((int)leftTime);
       
    }
    public BonusState GetDailyBonusState(int day)
    {
        int lastClaimDay = StorageManager.Instance.GetStorage<StorageHome>().DailyBonus.LastClaimDailyBonusDay;
        int consecutiveLoginDays = CheckConsecutiveLoginDays();
        int currentDay = (int) ((CommonUtils.GetTimeStamp() - HourOffset * 3600 * 1000) / (24 * 3600 * 1000));
        bool currentDayClaimed = lastClaimDay >= currentDay;
        consecutiveLoginDays = consecutiveLoginDays % DailyDays;
        if (consecutiveLoginDays == 0)
        {
            consecutiveLoginDays = DailyDays;
        }

        if (day == 1 && consecutiveLoginDays == DailyDays && currentDayClaimed)
        {
            return BonusState.NextDayCanClaim;
        }

        if (day < consecutiveLoginDays)
        {
            return BonusState.NotCanClaim;
        }

        if (day == consecutiveLoginDays)
        {
            return currentDayClaimed ? BonusState.NotCanClaim : BonusState.CanClaim;
        }

        if (day == consecutiveLoginDays + 1)
        {
            return currentDayClaimed ? BonusState.NextDayCanClaim : BonusState.NotCanClaim;
        }

        return BonusState.NotCanClaim;
    }

    // 领取奖励 ratio:倍数
    public bool ClaimBonus(int day, bool disEvent = true, int ratio = 1,Action<bool> callback=null)
    {
        if (day <= 0 || day > DailyDays || GetDailyBonusState(day) != BonusState.CanClaim)
        {
            callback?.Invoke(false);
            return false;
        }

        if (GetDailyBonusState(day) != BonusState.CanClaim)
        {
            callback?.Invoke(false);
            return false;
        }
        var reasonArgs = new GameBIManager.ItemChangeReasonArgs(BiEventCooking.Types.ItemChangeReason.SignInGet);

        var config = AdConfigHandle.Instance.GetDailyBonus(day);
        List<ResData> rewards = new List<ResData>();
        for (int i = 0; i < config.ItemId.Count; i++)
        { 
            rewards.Add(new ResData(config.ItemId[i], config.ItemNum[i]*ratio));
            if (!UserData.Instance.IsResource(config.ItemId[i]))
            {
                GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                {
                    MergeEventType = BiEventCooking.Types.MergeEventType.MergeChangeReasonSignIn,
                    itemAId = config.ItemId[i],
                    isChange = true,
                   
                });
            }
        }

        if (VipStoreModel.Instance.VipLevel() >= 2)
        {
            int value = GlobalConfigManager.Instance.GetNumValue("vip_daily_claim_diamond");   
            rewards.Add(new ResData(UserData.ResourceId.Diamond, value));
        }
        
        EventDispatcher.Instance.DispatchEvent(EventEnum.UpdateDailyBonus);
        int currentDay = (int) ((CommonUtils.GetTimeStamp() - HourOffset * 3600 * 1000)/ (24 * 3600 * 1000));
        var storageHome=StorageManager.Instance.GetStorage<StorageHome>();
        storageHome.DailyBonus.LastClaimDailyBonusDay = currentDay;
        storageHome.DailyBonus.TotalClaimDay++;
        CheckIsHaveCanClaim();
        CommonRewardManager.Instance.PopCommonReward(rewards, CurrencyGroupManager.Instance.currencyController, true,
            reasonArgs, () =>
            {
                callback?.Invoke(GetDailyBonusChest(storageHome.DailyBonus.TotalClaimDay)!=null);
            });
        return true;
    }
    //------bOX---
    public DailyBonusChest GetDailyBoxConfig(int totalDay)
    {
        var configs = AdConfigHandle.Instance.GetDailyBonusChest();
        if (configs == null || configs.Count <= 0)
            return null;
        var lastConfig = configs.Last();
        int dayOfMonth = totalDay % lastConfig.Day;
        for (int j = 0; j < configs.Count; j++) //查找排除最后一个
        {
            if (configs[j].Day >= dayOfMonth)
                return configs[j];
        }
       
        return lastConfig;
    }

    public DailyBonusChest GetDailyBonusChest(int totalDay)
    {
        var configs = AdConfigHandle.Instance.GetDailyBonusChest();
        if (configs == null || configs.Count <= 0)
            return null;
        var lastConfig = configs.Last();
        int dayOfMonth = (totalDay-1) % lastConfig.Day+1;
        for (int j = 0; j < configs.Count ; j++)
        {
            if (dayOfMonth == configs[j].Day)
                return configs[j];
        }

        return null;
    }

    public void ClaimBonusChest(int day, Action<bool> callback = null)
    {
        var config = GetDailyBonusChest(day);
        if (config == null)
        {
            callback?.Invoke(false);
        }
        List<ResData> rewards = new List<ResData>();
        for (int i = 0; i < config.ItemId.Count; i++)
        { 
            rewards.Add(new ResData(config.ItemId[i], config.ItemNum[i]));
            if (!UserData.Instance.IsResource(config.ItemId[i]))
            {
                GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                {
                    MergeEventType = BiEventCooking.Types.MergeEventType.MergeChangeReasonSignIn,
                    itemAId = config.ItemId[i],
                    isChange = true,
                   
                });
            }
        }
        var reasonArgs = new GameBIManager.ItemChangeReasonArgs(BiEventCooking.Types.ItemChangeReason.SignInChestGet);

        CommonRewardManager.Instance.PopCommonReward(rewards, CurrencyGroupManager.Instance.currencyController, true, reasonArgs, () =>
        {
            callback?.Invoke(true);
        });
    }

}