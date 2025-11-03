using System;
using System.Collections.Generic;
using System.Linq;
using DragonPlus;
using DragonU3DSDK;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;
public static class SlotMachineUtils
{
    public static void Spin(this StorageSlotMachine storage)
    {
        if (storage.HasUnCollectResult)
            return;
        if (storage.SpinCount <= 0)
            return;
        storage.SpinCount--;
        EventDispatcher.Instance.SendEventImmediately(new EventSlotMachineScoreChange(-1));
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventSlotMachineRadishChange,
            "-1",storage.SpinCount.ToString(),"Spin");
        storage.HasUnCollectResult = true;
        storage.ReSpinTimes = 0;
        storage.ResultConfigList.Clear();
        storage.ElementIndexList.Clear();
        storage.InitElementIndexList.Clear();
        var resultConfigList = SlotMachineModel.Instance.GlobalConfig.ResultConfigList;
        var repeatResultFlag = false;
        var repeatResult = -1;
        var resultList = new List<int>();
        for (var i = 0; i < resultConfigList.Count; i++)
        {
            var resultConfig = SlotMachineModel.Instance.ResultConfigDic[resultConfigList[i]];
            var result = -1;
            if (i == 0)
            {
                var index = Utils.RandomByWeight(resultConfig.WeightList);
                result = resultConfig.ResultList[index];
                repeatResult = result;
            }
            else
            {
                var repeatChance = SlotMachineModel.Instance.GlobalConfig.RepeatResultChance[i - 1];
                if (!repeatResultFlag && Random.Range(0, 100) < repeatChance)
                {
                    repeatResultFlag = true;
                    result = repeatResult;
                }
                else
                {
                    var index = Utils.RandomByWeight(resultConfig.WeightList);
                    result = resultConfig.ResultList[index];
                }
            }
            resultList.Add(result);
        }

        if (SlotMachineModel.Instance.GlobalConfig.MixReel)
        {
            var mixTab = new List<int>();
            for (var i = 0; i < resultConfigList.Count; i++)
            {
                mixTab.Add(resultConfigList[i]);
            }
            for (var i = 0; i < resultConfigList.Count; i++)
            {
                var randomIndex = Random.Range(0, mixTab.Count);
                var resultConfigId = mixTab[randomIndex];
                var result = resultList[randomIndex];
                mixTab.RemoveAt(randomIndex);
                resultList.RemoveAt(randomIndex);
                storage.ResultConfigList.Add(resultConfigId);
                storage.ElementIndexList.Add(result);
                storage.InitElementIndexList.Add(result);
            }   
        }
        else
        {
            for (var i = 0; i < resultConfigList.Count; i++)
            {
                storage.ResultConfigList.Add(resultConfigList[i]);
                storage.ElementIndexList.Add(resultList[i]);
                storage.InitElementIndexList.Add(resultList[i]);
            }
        }
        EventDispatcher.Instance.SendEventImmediately(new EventSlotMachinePerformSpinReel(storage));
    }

    public static void SpinGuide(this StorageSlotMachine storage)
    {
        storage.SpinCount--;
        EventDispatcher.Instance.SendEventImmediately(new EventSlotMachineScoreChange(-1));
        storage.HasUnCollectResult = true;
        storage.ReSpinTimes = 0;
        storage.ResultConfigList.Clear();
        storage.ElementIndexList.Clear();
        storage.InitElementIndexList.Clear();
        var resultList = SlotMachineModel.Instance.GlobalConfig.GuideSpinResult;
        foreach (var result in resultList)
        {
            storage.ElementIndexList.Add(result);
            storage.InitElementIndexList.Add(result);
        }
        foreach (var configId in SlotMachineModel.Instance.GlobalConfig.ResultConfigList)
        {
            storage.ResultConfigList.Add(configId);
        }
        EventDispatcher.Instance.SendEventImmediately(new EventSlotMachinePerformSpinReel(storage));
    }

    public static void SpinSingleReel(this StorageSlotMachine storage, int reelIndex)
    {
        if (!storage.HasUnCollectResult)
            return;
        var price = storage.GetReSpinPrice();
        if (!UserData.Instance.CanAford(UserData.ResourceId.Diamond, price))
            return;
        UserData.Instance.ConsumeRes(UserData.ResourceId.Diamond, price,new GameBIManager.ItemChangeReasonArgs(
            reason:BiEventAdventureIslandMerge.Types.ItemChangeReason.SlotMachineUse));
        var resultConfig = SlotMachineModel.Instance.ResultConfigDic[storage.ResultConfigList[reelIndex]];
        var lastTriggerReward = GetTriggerReward(storage.ElementIndexList);
        var tempWeightList = new List<int>(resultConfig.WeightList);
        var lastElement = storage.ElementIndexList[reelIndex];
        var fakeResult = new List<int>(storage.ElementIndexList);
        var forceResult = -1;
        for (var i = 0; i < SlotMachineModel.Instance.GlobalConfig.ElementList.Count; i++)
        {
            var elementIndex = SlotMachineModel.Instance.GlobalConfig.ElementList[i];
            if (lastElement == elementIndex)
                continue;
            fakeResult[reelIndex] = elementIndex;
            var newTriggerReward = GetTriggerReward(fakeResult);
            if (newTriggerReward != null && newTriggerReward != lastTriggerReward &&
                storage.ReSpinTimes == newTriggerReward.MaxReSpinTimes)
            {
                forceResult = elementIndex;
                break;
            }
            if (newTriggerReward != null && newTriggerReward != lastTriggerReward &&
                storage.ReSpinTimes < newTriggerReward.LeastReSpinTimes)
            {
                var disableIndex = resultConfig.ResultList.FindIndex(a => a == elementIndex);
                tempWeightList[disableIndex] = 0;
            }
        }

        var result = -1;
        if (forceResult > 0)
        {
            result = forceResult;
        }
        else
        {
            var index = Utils.RandomByWeight(tempWeightList);
            result = resultConfig.ResultList[index];
        }
        storage.ElementIndexList[reelIndex] = result;
        storage.ReSpinTimes++;
        Debug.LogError("reel:"+reelIndex+" result:"+result);
        EventDispatcher.Instance.SendEventImmediately(new EventSlotMachinePerformSingleSpinReel(storage,reelIndex));
    }

    public static void SpinSingleReelGuide(this StorageSlotMachine storage, int reelIndex)
    {
        storage.ElementIndexList[reelIndex] = SlotMachineModel.Instance.GlobalConfig.GuideReSpinResult;
        Debug.LogError("引导 reel:"+reelIndex+" result:"+SlotMachineModel.Instance.GlobalConfig.GuideReSpinResult);
        EventDispatcher.Instance.SendEventImmediately(new EventSlotMachinePerformSingleSpinReel(storage,reelIndex));
    }

    public static void CollectReward(this StorageSlotMachine storage)
    {
        if (!storage.HasUnCollectResult)
            return;
        storage.HasUnCollectResult = false;
        var rewardConfig = GetTriggerReward(storage.ElementIndexList);
        if (rewardConfig != null)
        {
            var rewards = CommonUtils.FormatReward(rewardConfig.RewardId, rewardConfig.RewardNum);
            var reasonArgs = new GameBIManager.ItemChangeReasonArgs()
            {
                reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.SlotMachineGet
            };
            CommonRewardManager.Instance.PopCommonReward(rewards, CurrencyGroupManager.Instance.currencyController,
                true,
                reasonArgs);   
        }

        var initResultStr = "";
        for (var i = 0; i < storage.InitElementIndexList.Count; i++)
        {
            initResultStr += storage.InitElementIndexList[i] + ",";
        }
        var totalResultStr = "";
        for (var i = 0; i < storage.ElementIndexList.Count; i++)
        {
            totalResultStr += storage.ElementIndexList[i] + ",";
        }
        
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventSlotMachineResult,
            initResultStr,totalResultStr,storage.ReSpinTimes.ToString());
        EventDispatcher.Instance.SendEventImmediately(new EventSlotMachineCollectReward(storage));
    }
    public static SlotMachineRewardConfig GetTriggerReward(List<int> elementList)
    {
        var resultDic = new Dictionary<int, int>();
        for (var i = 0; i < elementList.Count; i++)
        {
            var element = elementList[i];
            if (!resultDic.ContainsKey(element))
            {
                resultDic.Add(element,0);
            }
            resultDic[element]++;
        }

        var rewardConfigList = SlotMachineModel.Instance.RewardConfigList;
        SlotMachineRewardConfig bestReward = null;
        for (var i = 0; i < rewardConfigList.Count; i++)
        {
            var config = rewardConfigList[i];
            var trigger = true;
            var tempResultDic = new Dictionary<int, int>(resultDic);
            if (config.ResultList != null)
            {
                foreach (var result in config.ResultList)
                {
                    if (tempResultDic.TryGetValue(result, out var count) && count > 0)
                    {
                        tempResultDic[result]--;
                    }
                    else
                    {
                        trigger = false;
                        break;
                    }
                }   
            }
            if (trigger)
            {
                if (bestReward == null || bestReward.Id < config.Id)
                {
                    bestReward = config;   
                }
            }
        }
        return bestReward;
    }

    public static int GetReSpinPrice(this StorageSlotMachine storage)
    {
        if (!storage.HasUnCollectResult)
            return 0;
        var priceConfigList = SlotMachineModel.Instance.ReSpinConfigList;
        for (var i = priceConfigList.Count - 1; i >= 0; i--)
        {
            var config = priceConfigList[i];
            if (storage.ReSpinTimes >= config.BuyTimes)
            {
                return config.Price;
            }
        }
        return 0;
    }
    
    public static string GetAuxItemAssetPath(this StorageSlotMachine storage)
    {
        return "Prefabs/Activity/SlotMachine/Aux_SlotMachine";
    }
    public static string GetTaskItemAssetPath(this StorageSlotMachine storage)
    {
        return "Prefabs/Activity/SlotMachine/TaskList_SlotMachine";
    }
    public static bool ShowEntrance(this StorageSlotMachine storage)
    {
        return SlotMachineModel.Instance.IsOpened();
    }
}