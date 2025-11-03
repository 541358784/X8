using System.Collections.Generic;
using System.Threading.Tasks;
using DragonPlus;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using TMatch;
using UnityEngine;

public partial class EventEnum
{
    public const string BuyEnergy = "BuyEnergy";
}
public class EventBuyEnergy : BaseEvent
{
    public EventBuyEnergy():base(EventEnum.BuyEnergy) { }
}
public class EndlessEnergyGiftBagModel:Manager<EndlessEnergyGiftBagModel>
{
    public List<TableEndlessEnergyGiftBagReward> RewardConfigs =>
        GlobalConfigManager.Instance.EndlessEnergyGiftBagRewardList;
    public TableEndlessEnergyGiftBagGlobal GlobalConfig => GlobalConfigManager.Instance.EndlessEnergyGiftBagGlobalList[0];
    public List<TableEndlessEnergyGiftBagReward> repeatRewardConfigs;
    public List<TableEndlessEnergyGiftBagReward> RepeatRewardConfigs
    {
        get
        {
            if (repeatRewardConfigs == null)
            {
                repeatRewardConfigs = new List<TableEndlessEnergyGiftBagReward>();
                foreach (var rewardConfig in RewardConfigs)
                {
                    if (rewardConfig.repeat)
                        repeatRewardConfigs.Add(rewardConfig);
                }
            }
            return repeatRewardConfigs;
        }
    }

    public StorageEndlessEnergyGiftBag Storage =>
        StorageManager.Instance.GetStorage<StorageHome>().EndlessEnergyGiftBag;
    public bool IsUnlock => UnlockManager.IsOpen(UnlockManager.MergeUnlockType.EndlessEnergyGiftBag);
    public bool IsOpen => (long)APIManager.Instance.GetServerTime() < Storage.EndTime;

    public void TryOpen()
    {
        if (IsOpen)
            return;
        if (!IsUnlock)
            return;
        if (Storage.DayCount < GlobalConfig.dayCount)
            return;
        var payLevelOpenFlag = PayLevelModel.Instance.GetCurPayLevelConfig().EndlessEnergyGiftBagOpenFlag;
        if (!payLevelOpenFlag)
            return;
        Storage.PayLevelGroup = PayLevelModel.Instance.GetCurPayLevelConfig().Id;
        Storage.StartTime = (long)APIManager.Instance.GetServerTime();
        Storage.EndTime = Storage.StartTime + (long)XUtility.Hour * GlobalConfig.activeTime;
        Storage.Level = 0;
        Storage.DayCount = GlobalConfig.dayCount - 1;//确保今天不会再开启
    }

    public void Init()
    {
        Timer.Register(1, UpdateDayCount, null, true);
        EventDispatcher.Instance.AddEvent<EventBuyEnergy>(OnBuyEnergy);
    }
    public void OnBuyEnergy(EventBuyEnergy evt)
    {
        Storage.BuyEnergy = true;
        Storage.DayCount = 0;
    }
    public void UpdateDayCount()
    {
        var curDay =(int) (APIManager.Instance.GetServerTime() / XUtility.DayTime);
        if (curDay != Storage.DayId)
        {
            if (Storage.BuyEnergy)
            {
                Storage.DayCount = 0;
            }
            else if (Storage.DayId != 0)
            {
                Storage.DayCount++;
            }
            Storage.BuyEnergy = false;
            if (Storage.DayId == 0)
            {
                Storage.DayCount = GlobalConfig.dayCount;//初始化默认有两天未付费天数计数
            }
            Storage.DayId = curDay;
        }
    }

    public TableEndlessEnergyGiftBagReward GetRewardConfig(int level)
    {
        if (level < RewardConfigs.Count)
        {
            return RewardConfigs[level];
        }
        else
        {
            var repeatLevel = level - RewardConfigs.Count;
            repeatLevel %= RepeatRewardConfigs.Count;
            return RepeatRewardConfigs[repeatLevel];
        }
    }
    public TaskCompletionSource<bool> Collect(TableEndlessEnergyGiftBagReward config)
    {
        var taskSource = new TaskCompletionSource<bool>();
        if (!IsOpen)
        {
            taskSource.SetResult(false);
            return taskSource;
        }
        var rewardConfig = GetRewardConfig(Storage.Level);
        if (rewardConfig != config)
        {
            Debug.LogError("领取奖励等级错误 存档="+rewardConfig.id+" 领取="+config.id);
            taskSource.SetResult(false);
            return taskSource;   
        }
        var reason = new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.EndlessEnergyGiftBag);
        if (config.price > 0)
        {
            if (!UserData.Instance.CanAford(UserData.ResourceId.Diamond, config.price))
            {
                BuyResourceManager.Instance.TryShowBuyResource(UserData.ResourceId.Diamond, "",
                    "" ,"EndLessEnergyGiftBag",needCount:config.price);
                taskSource.SetResult(false);
                return taskSource;   
            }
            UserData.Instance.ConsumeRes(UserData.ResourceId.Diamond, config.price,reason);
            EventDispatcher.Instance.SendEventImmediately(new EventBuyEnergy());
        }
        if (UIEndlessEnergyGiftBagController.Instance)
            UIEndlessEnergyGiftBagController.Instance.HasBuy = true;
        Storage.Level++;
        var rewards = CommonUtils.FormatReward(config.rewardId, config.rewardNum);
        UserData.Instance.AddRes(rewards,reason);
        CommonRewardManager.Instance.PopCommonReward(rewards, CurrencyGroupManager.Instance.currencyController, false,
            reason, () =>
            {
                taskSource.SetResult(true);
            });
        return taskSource;

    }

    public long GetLeftTime()
    {
        return Storage.EndTime - (long)APIManager.Instance.GetServerTime();
    }

    public string GetLeftTimeStr()
    {
        return CommonUtils.FormatLongToTimeStr(GetLeftTime());
    }
    
    
    
}