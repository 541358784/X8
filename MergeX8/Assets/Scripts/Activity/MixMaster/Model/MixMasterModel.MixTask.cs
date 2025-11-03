using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DragonPlus;
using DragonPlus.Config.MixMaster;
using DragonU3DSDK.Network.API.Protocol;

public partial class MixMasterModel
{
    public List<MixMasterMixTaskConfig> MixTaskConfig =>
        MixMasterConfigManager.Instance.GetConfig<MixMasterMixTaskConfig>();
    public void CheckMixTaskCompleted()
    {
        foreach (var mixTask in MixTaskConfig)
        {
            if (!Storage.AlreadyCollectLevels.Contains(mixTask.Id) &&
                !Storage.CanCollectLevels.Contains(mixTask.Id) &&
                Storage.FirstMixCount >= mixTask.CollectCount)
            {
                Storage.CanCollectLevels.Add(mixTask.Id);
                UIPopupMixMasterTaskCompletedController.PushCompletedTask(mixTask,mixTask.CollectCount-1,mixTask.CollectCount);
            }
        }
    }

    public void CollectTask(MixMasterMixTaskConfig mixTask)
    {
        if (Storage.AlreadyCollectLevels.Contains(mixTask.Id))
            return;
        if (!Storage.CanCollectLevels.Contains(mixTask.Id))
            return;
        if (Storage.FirstMixCount < mixTask.CollectCount)
            return;
        Storage.CanCollectLevels.Remove(mixTask.Id);
        Storage.AlreadyCollectLevels.Add(mixTask.Id);
        var rewards = CommonUtils.FormatReward(mixTask.RewardId, mixTask.RewardNum);
        if (mixTask.FormulaId > 0)
            MixMasterModel.Instance.UnlockFormula(mixTask.FormulaId);
        if (rewards.Count > 0)
        {
            var reason = new GameBIManager.ItemChangeReasonArgs()
            {
                reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.MixMasterGet
            };
            CommonRewardManager.Instance.PopCommonReward(rewards, CurrencyGroupManager.Instance.currencyController,
                true, reason);
        }
        EventDispatcher.Instance.SendEventImmediately(new EventMixMasterUpdateRedPoint());
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventMixMasterReward,
            data1: mixTask.Id.ToString());
    }
}