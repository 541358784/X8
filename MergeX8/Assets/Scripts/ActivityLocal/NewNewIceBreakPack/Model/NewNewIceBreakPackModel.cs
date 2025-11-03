using System.Collections.Generic;
using System.Threading.Tasks;
using DragonPlus;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;

public partial class NewNewIceBreakPackModel : Manager<NewNewIceBreakPackModel>
{
    public StorageNewNewIceBreakPack Storage => StorageManager.Instance.GetStorage<StorageHome>().NewNewIceBreakPack;
    public TableNewNewIceBreakPackGlobal GlobalConfig => NewNewIceBreakPackGlobalList[0];
    
    public void PurchaseSuccess(TableShop shopConfig)
    {
        // if (!IsOpen())
        //     return;
        if (GlobalConfig.shopId != shopConfig.id)
            return;
        Storage.BuyState = true;
        if (UIPopupNewNewIceBreakPackController.Instance)
            UIPopupNewNewIceBreakPackController.Instance?.OnBuy();
        if (UIPopupNewNewIceBreakPackFinishController.Instance)
            UIPopupNewNewIceBreakPackFinishController.Instance?.OnBuy();
    }

    public bool IsOpen()
    {
        return Storage.IsNewUser && Storage.IsInit && (Storage.EndTime > (long)APIManager.Instance.GetServerTime() || !Storage.ShowEndView);
    }

    public bool IsUnlock => UnlockManager.IsOpen(UnlockManager.MergeUnlockType.NewNewIceBreakPack);
    public void InitStorage()
    {
        if (!Storage.IsNewUser)
            return;
        if (!IsUnlock)
            return;
        if (Storage.IsInit)
            return;
        Storage.IsInit = true;
        Storage.PayLevelGroup = PayLevelModel.Instance.GetCurPayLevelConfig().NewNewIceBreakPackGroupId;
        var curTime = (long)APIManager.Instance.GetServerTime();
        Storage.StartTime = curTime;
        Storage.EndTime = curTime + GlobalConfig.time * (long)XUtility.Min;
    }

    public bool CanCollectReward(TableNewNewIceBreakPackReward reward)
    {
        if (Storage.CollectState.Contains(reward.id))
            return false;
        var curTime = (long)APIManager.Instance.GetServerTime();
        if (Storage.StartTime + (long)XUtility.Min * reward.unLockTime > curTime)
            return false;
        if (!reward.isFree && !Storage.BuyState)
            return false;
        return true;
    }
    public Task CollectReward(TableNewNewIceBreakPackReward reward)
    {
        if (!CanCollectReward(reward))
        {
            return Task.CompletedTask;
        }
        Storage.CollectState.Add(reward.id);
        var rewards = CommonUtils.FormatReward(reward.itemId, reward.ItemNum);
        var task = new TaskCompletionSource<bool>();
        CommonRewardManager.Instance.PopCommonReward(rewards, CurrencyGroupManager.Instance.currencyController, true,
            new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.NewIceBreak),animEndCall:
            () =>
            {
                task.SetResult(true);
            });
        return task.Task;
    }
    public Task CollectRewards(List<TableNewNewIceBreakPackReward> rewardList)
    {
        var rewards = new List<ResData>();
        foreach (var reward in rewardList)
        {
            if (!CanCollectReward(reward))
            {
                continue;
            }
            Storage.CollectState.Add(reward.id);
            rewards.AddRange(CommonUtils.FormatReward(reward.itemId, reward.ItemNum));
        }

        if (rewards.Count == 0)
            return Task.CompletedTask;
        var task = new TaskCompletionSource<bool>();
        CommonRewardManager.Instance.PopCommonReward(rewards, CurrencyGroupManager.Instance.currencyController, true,
            new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.NewIceBreak),animEndCall:
            () =>
            {
                task.SetResult(true);
            });
        return task.Task;
    }

    public bool CanShowUI()
    {
        InitStorage();
        if (!IsOpen())
            return false;
        if (Storage.EndTime > (long)APIManager.Instance.GetServerTime())
        {
            if ((SceneFsm.mInstance.GetCurrSceneType() == StatusType.Game))
            {
                return false;
            }
            var cdkey = "NewNewIceBreakPackCDKey";
            if (CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, cdkey))
                return false;
            UIPopupNewNewIceBreakPackController.Open();
            CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, cdkey, CommonUtils.GetTimeStamp());
            return true;
        }
        else if(!Storage.ShowEndView)
        {
            Storage.ShowEndView = true;
            UIPopupNewNewIceBreakPackController.Open();
            return true;
        }
        return false;
    }
}