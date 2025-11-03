using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Deco.World;
using Decoration;
using DragonPlus;
using DragonPlus.Config.Monopoly;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Farm.Model;
using Gameplay;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;
using Utils = DragonU3DSDK.Utils;

public enum MonopolyStoreItemType
{
    MergeItem = 1,
    BuildItem = 2,
}

public static class MonopolyUtils
{
    public const ulong Offset = 0 * XUtility.Hour;
    public static int CurDay => (int) ((APIManager.Instance.GetServerTime() - Offset) / XUtility.DayTime);

    public static ulong CurDayLeftTime =>
        XUtility.DayTime - ((APIManager.Instance.GetServerTime() - Offset) % XUtility.DayTime);

    public static string CurDayLeftTimeString => CommonUtils.FormatLongToTimeStr((long) CurDayLeftTime);

    public static int GetCurDatBuyTimes(this StorageMonopoly storage)
    {
        if (!storage.DiceBuyState.TryGetValue(CurDay, out var buyTimes))
        {
            buyTimes = 0;
            storage.DiceBuyState.Add(CurDay, buyTimes);
        }

        return buyTimes;
    }

    public static void AddCurDatBuyTimes(this StorageMonopoly storage, int addValue)
    {
        if (!storage.DiceBuyState.TryGetValue(CurDay, out var buyTimes))
        {
            buyTimes = 0;
            storage.DiceBuyState.Add(CurDay, buyTimes);
        }

        storage.DiceBuyState[CurDay] += addValue;
    }

    public static void ThrowDice(this StorageMonopoly storage, int diceCount, string biStrIn, out string biStrOut,int betValue)
    {
        biStrOut = biStrIn;
        if (!MonopolyModel.Instance.IsInitFromServer())
            return;
        var stepCount = 0;
        var diceConfigList = new List<MonopolyDiceConfig>();
        for (var j = 0; j < diceCount; j++)
        {
            var cardPool = new List<MonopolyDiceConfig>();
            for (var i = 0; i < storage.DiceRandomPool.Count; i++)
            {
                var tempCardConfig = MonopolyModel.Instance.DiceConfig[storage.DiceRandomPool[i]];
                cardPool.Add(tempCardConfig);
            }

            if (cardPool.Count == 0)
            {
                throw new Exception("异常情况,卡池空了");
            }

            var weightPool = new List<int>();
            for (var i = 0; i < cardPool.Count; i++)
            {
                weightPool.Add(cardPool[i].Weight);
            }

            var index = Utils.RandomByWeight(weightPool);
            var cardConfig = cardPool[index];
            storage.DiceRandomPool.Remove(cardConfig.Id);
            if (storage.DiceRandomPool.Count == 0)
            {
                // if (cardConfig.Id != 3 && !GuideSubSystem.Instance.isFinished(GuideTriggerPosition.MonopolyGetWildCard))
                // {
                //     storage.DiceRandomPool.Clear();
                //     storage.DiceRandomPool.Add(3);
                // }
                // else
                // {
                for (var i = 0; i < MonopolyModel.Instance.DicePool.Count; i++)
                {
                    storage.DiceRandomPool.Add(MonopolyModel.Instance.DicePool[i]);
                }
                // }
            }

            biStrOut += " Step" + cardConfig.Step;
            stepCount += cardConfig.Step;
            diceConfigList.Add(cardConfig);
        }

        EventDispatcher.Instance.SendEventImmediately(new EventMonopolyUIThrowMultipleDice(diceConfigList));
        storage.AddStep(stepCount, biStrOut, out biStrOut,betValue);
    }

    public static void PlayMiniGame(this StorageMonopoly storage, string biStrIn, out string biStrOut,int betValue)
    {
        biStrOut = biStrIn;
        biStrOut += " MiniGame";
        var miniGameConfigIdPool = MonopolyModel.Instance.GlobalConfig.MiniGamePool;
        var miniGameConfigPool = new List<MonopolyMiniGameConfig>();
        var weightPool = new List<int>();
        for (var i = 0; i < miniGameConfigIdPool.Count; i++)
        {
            var config = MonopolyModel.Instance.MiniGameConfig[miniGameConfigIdPool[i]];
            miniGameConfigPool.Add(config);
            weightPool.Add(config.Weight);
        }

        var index = Utils.RandomByWeight(weightPool);
        var miniGameConfig = miniGameConfigPool[index];
        var result = miniGameConfig.ResultList.Last();
        var rewardId = miniGameConfig.RewardId[result];
        var rewardNum = miniGameConfig.RewardNum[result] * storage.GetCurBlockUpgradeRewardMultiple();
        rewardNum *= betValue;
        if (rewardId < 0)
        {
            storage.AddScore(rewardNum, "MiniGame", true);
            biStrOut += " AddScore" + rewardNum;
        }
        else
        {
            UserData.Instance.AddRes(rewardId, rewardNum, new GameBIManager.ItemChangeReasonArgs()
            {
                reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.MonopolyGet
            });
            biStrOut += " Reward" + rewardId + "*" + rewardNum;
        }

        storage.UnFinishedMiniGameConfigId = miniGameConfig.Id;
        EventDispatcher.Instance.SendEventImmediately(new EventMonopolyUIPlayMiniGame(miniGameConfig,betValue));
    }

    public static void GetRandomCard(this StorageMonopoly storage, string biStrIn, out string biStrOut,int betValue)
    {
        biStrOut = biStrIn;
        var stepCount = 0;

        var cardPool = new List<MonopolyCardConfig>();
        for (var i = 0; i < storage.CardRandomPool.Count; i++)
        {
            var tempCardConfig = MonopolyModel.Instance.CardConfig[storage.CardRandomPool[i]];
            cardPool.Add(tempCardConfig);
        }

        if (cardPool.Count == 0)
        {
            throw new Exception("异常情况,卡池空了");
        }

        var weightPool = new List<int>();
        for (var i = 0; i < cardPool.Count; i++)
        {
            weightPool.Add(cardPool[i].Weight);
        }

        var index = Utils.RandomByWeight(weightPool);
        var cardConfig = cardPool[index];
        storage.CardRandomPool.Remove(cardConfig.Id);
        if (storage.CardRandomPool.Count == 0)
        {
            // if (cardConfig.Id != 3 && !GuideSubSystem.Instance.isFinished(GuideTriggerPosition.MonopolyGetWildCard))
            // {
            //     storage.DiceRandomPool.Clear();
            //     storage.DiceRandomPool.Add(3);
            // }
            // else
            // {
            for (var i = 0; i < MonopolyModel.Instance.CardPool.Count; i++)
            {
                storage.CardRandomPool.Add(MonopolyModel.Instance.CardPool[i]);
            }
            // }
        }
        EventDispatcher.Instance.SendEventImmediately(new EventMonopolyUIGetCard(cardConfig,betValue));
        var cardType = (MonopolyCardType) cardConfig.CardType;
        if (cardType == MonopolyCardType.Score)
        {
            var score = cardConfig.Score * storage.GetCurBlockUpgradeRewardMultiple();
            score *= betValue;
            biStrOut += " ScoreCard AddScore" + score;
            storage.AddScore(score, "ScoreCard", true);
        }
        else if (cardType == MonopolyCardType.MultiScore)
        {
            biStrOut += " MultiScoreCard MultiValue" + cardConfig.ScoreMultiValue;
            storage.AddMultiScoreCard(cardConfig.ScoreMultiValue, true);
        }
        else if (cardType == MonopolyCardType.MultiStep)
        {
            biStrOut += " MultiStepCard MultiValue" + cardConfig.StepMultiValue;
            storage.AddMultiStepCard(cardConfig.StepMultiValue, true);
        }
        else if (cardType == MonopolyCardType.Wild)
        {
            biStrOut += " WildCard";
            for (var i = 0; i < betValue; i++)
            {
                storage.AddWildCard();   
            }
        }
    }

    public static void AddScore(this StorageMonopoly storage, int addCount, string reason, bool needWait = false)
    {
        storage.Score += addCount;
        storage.TotalScore += addCount;
        MonopolyLeaderBoardModel.Instance.GetLeaderBoardStorage(storage.ActivityId)?.CollectStar(addCount);
        EventDispatcher.Instance.SendEventImmediately(new EventMonopolyScoreChange(addCount, needWait));

        GameBIManager.Instance.SendGameEvent(
            BiEventAdventureIslandMerge.Types.GameEventType.GameEventMonopolyRadishChange,
            addCount.ToString(), storage.Score.ToString(), reason);
    }

    public static void AddStep(this StorageMonopoly storage, int stepCount, string biStrIn, out string biStrOut,int betValue = 1)
    {
        biStrOut = biStrIn;
        if (!MonopolyModel.Instance.IsInitFromServer())
            return;
        var BlockConfigList = MonopolyModel.Instance.BlockConfigList;
        var targetBlockIndex = storage.CurBlockIndex + stepCount;
        while (targetBlockIndex > BlockConfigList.Count - 1)
        {
            //通关
            var block = BlockConfigList[0];
            if ((MonopolyBlockType) block.BlockType != MonopolyBlockType.Start)
            {
                Debug.LogError("初始地块类型错误");
            }

            EventDispatcher.Instance.SendEventImmediately(new EventMonopolyUIMoveStep(storage.CurBlockIndex,
                BlockConfigList.Count - storage.CurBlockIndex));
            storage.CompleteTimes++;
            targetBlockIndex -= BlockConfigList.Count;
            storage.CurBlockIndex = 0;
            storage.CurBlockBuyState = false;
            biStrOut += " PassLevel";
            var multiple = storage.GetCurBlockUpgradeRewardMultiple();
            if (block.RewardId != null && block.RewardId.Count > 0)
            {
                var reward = CommonUtils.FormatReward(block.RewardId, block.RewardNum);
                foreach (var res in reward)
                {
                    res.count *= multiple;
                    biStrOut += " Reward" + res.id + "*" + res.count;
                }

                var reason =
                    new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason
                        .MonopolyGet);
                EventDispatcher.Instance.SendEventImmediately(new EventMonopolyUIGetBlockReward(block, reward));
                UserData.Instance.AddRes(reward, reason);
            }

            if (block.Score > 0)
            {
                var addScore = block.Score * multiple;
                if (targetBlockIndex == 0)
                    addScore *= betValue;
                storage.AddScore(addScore, "StartPointBlock", true);
                EventDispatcher.Instance.SendEventImmediately(new EventMonopolyUIGetBlockScore(block, addScore, 0));
                biStrOut += " AddScore" + addScore;
            }

            EventDispatcher.Instance.SendEventImmediately(new EventMonopolyLevelUp());
        }

        {
            EventDispatcher.Instance.SendEventImmediately(new EventMonopolyUIMoveStep(storage.CurBlockIndex,
                targetBlockIndex - storage.CurBlockIndex));
            storage.CurBlockIndex = targetBlockIndex;
            var blockIndex = storage.CurBlockIndex;
            var block = BlockConfigList[blockIndex];
            var blockType = (MonopolyBlockType) block.BlockType;
            storage.CurBlockBuyState = blockType == MonopolyBlockType.Score;
            if (blockType == MonopolyBlockType.Score)
            {
                var addScore = storage.GetCurBlockScore(block);
                if (storage.ScoreMultiValue() > 1)
                {
                    addScore *= storage.ScoreMultiValue();
                    storage.ReduceMultiScoreCard();
                }
                addScore *= betValue;
                storage.AddScore(addScore, "Block", true);
                EventDispatcher.Instance.SendEventImmediately(
                    new EventMonopolyUIGetBlockScore(block, addScore, blockIndex));
                biStrOut += " AddScore" + addScore;
                if (storage.CanBuyBlock(block))
                    EventDispatcher.Instance.SendEventImmediately(new EventMonopolyUIPopBuyBlock(block));
            }
            else if (blockType == MonopolyBlockType.Reward)
            {
                var reward = CommonUtils.FormatReward(block.RewardId, block.RewardNum);
                if (storage.ScoreMultiValue() > 1)
                {
                    foreach (var res in reward)
                    {
                        res.count *= storage.ScoreMultiValue();
                    }

                    storage.ReduceMultiScoreCard();
                }

                foreach (var res in reward)
                {
                    res.count *=betValue;
                }

                var reason =
                    new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason
                        .MonopolyGet);
                UserData.Instance.AddRes(reward, reason);
                EventDispatcher.Instance.SendEventImmediately(new EventMonopolyUIGetBlockReward(block, reward));
                foreach (var res in reward)
                {
                    biStrOut += " Reward" + res.id + "*" + res.count;
                }
            }
            else if (blockType == MonopolyBlockType.MiniGame)
            {
                storage.PlayMiniGame(biStrOut, out biStrOut,betValue);
            }
            else if (blockType == MonopolyBlockType.Card)
            {
                storage.GetRandomCard(biStrOut, out biStrOut,betValue);
            }
        }
    }

    public static int GetCurBlockScore(this StorageMonopoly storage, MonopolyBlockConfig blockConfig)
    {
        var groupFull = storage.IsBlockGroupFull(blockConfig);
        var buyTimes = storage.GetBlockBuyTimes(blockConfig.Id);
        return blockConfig.GetBlockScore(buyTimes, groupFull);
    }

    public static int GetBlockScore(this MonopolyBlockConfig blockConfig, int buyTimes, bool groupFull)
    {
        if ((MonopolyBlockType) blockConfig.BlockType != MonopolyBlockType.Score)
            return 0;
        var totalPrice = buyTimes > 0 ? blockConfig.UpgradeScore[buyTimes - 1] : blockConfig.Score;
        if (groupFull)
        {
            totalPrice = (int) (totalPrice * blockConfig.GroupMultiValue);
        }

        return totalPrice;
    }

    public static int GetBlockBuyTimes(this StorageMonopoly storage, int blockId)
    {
        if (storage.BlockBuyState.TryGetValue(blockId, out var buyTimes))
            return buyTimes;
        return 0;
    }

    public static bool IsBlockGroupFull(this StorageMonopoly storage, MonopolyBlockConfig blockConfig,
        bool ignoreSelf = false)
    {
        if ((MonopolyBlockType) blockConfig.BlockType != MonopolyBlockType.Score)
            return false;
        var groupFull = true;
        foreach (var blockId in blockConfig.GroupMember)
        {
            if (ignoreSelf && blockId == blockConfig.Id)
                continue;
            var groupBuyTimes = storage.GetBlockBuyTimes(blockId);
            if (groupBuyTimes == 0)
            {
                groupFull = false;
                break;
            }
        }

        return groupFull;
    }

    public static bool CanBuyBlock(this StorageMonopoly storage, MonopolyBlockConfig blockConfig)
    {
        if ((MonopolyBlockType) blockConfig.BlockType != MonopolyBlockType.Score)
            return false;
        var curBlock = MonopolyModel.Instance.BlockConfigList[storage.CurBlockIndex];
        if (curBlock != blockConfig)
        {
            Debug.LogError("购买节点非当前所在节点 当前:" + curBlock.Id + " 购买:" + blockConfig.Id);
            return false;
        }

        if (!storage.CurBlockBuyState)
        {
            return false;
        }

        storage.BlockBuyState.TryGetValue(blockConfig.Id, out var buyTimes);
        if (buyTimes >= blockConfig.UpgradeScore.Count) //买到头了
            return false;
        return true;
    }

    public static int GetBuyBlockPrice(this StorageMonopoly storage, MonopolyBlockConfig blockConfig)
    {
        if (!storage.CanBuyBlock(blockConfig))
            return 0;
        storage.BlockBuyState.TryGetValue(blockConfig.Id, out var buyTimes);
        return blockConfig.UpgradePrice[buyTimes];
    }

    public static void BuyBlock(this StorageMonopoly storage, MonopolyBlockConfig blockConfig)
    {
        if (!storage.CanBuyBlock(blockConfig))
            return;
        storage.CurBlockBuyState = false;
        if (!storage.BlockBuyState.ContainsKey(blockConfig.Id))
        {
            storage.BlockBuyState.Add(blockConfig.Id, 0);
        }

        storage.BlockBuyState[blockConfig.Id]++;
        storage.BlockBuyTimes++;
        EventDispatcher.Instance.SendEventImmediately(new EventMonopolyUIBuyBlock(blockConfig,
            storage.BlockBuyState[blockConfig.Id]));
    }

    public static int GetCurBlockUpgradeRewardMultiple(this StorageMonopoly storage)
    {
        var multipleList = MonopolyModel.Instance.GlobalConfig.BlockUpgradeRewardMultiple;
        var upgradeTimeList = MonopolyModel.Instance.GlobalConfig.BlockUpgradeTimes;
        for (var i = 0; i < upgradeTimeList.Count; i++)
        {
            if (upgradeTimeList[i] > storage.BlockBuyTimes)
            {
                return i == 0 ? 1 : multipleList[i - 1];
            }
        }

        return multipleList.Last();
    }

    public static void AddWildCard(this StorageMonopoly storage, bool autoSendEvent = true)
    {
        storage.WildCardCount++;
        if (autoSendEvent)
        {
            var cardState = new MonopolyCardState(MonopolyCardType.Wild, 0);
            EventDispatcher.Instance.SendEventImmediately(
                new EventMonopolyCardCountChange(cardState, 1, storage.WildCardCount));
        }
    }

    public static bool ReduceWildCard(this StorageMonopoly storage)
    {
        if (storage.WildCardCount == 0)
            return false;
        storage.WildCardCount--;
        var cardState = new MonopolyCardState(MonopolyCardType.Wild, 0);
        EventDispatcher.Instance.SendEventImmediately(
            new EventMonopolyCardCountChange(cardState, -1, storage.WildCardCount));
        return true;
    }

    public static void AddMultiStepCard(this StorageMonopoly storage, int multiValue, bool autoSendEvent = true)
    {
        var oldValue = storage.StepMultiValue();
        storage.StepMultiList.Add(multiValue);
        var newValue = storage.StepMultiValue();
        if (autoSendEvent)
        {
            EventDispatcher.Instance.SendEventImmediately(new EventMonopolyStepMultiChange(oldValue, newValue));
        }
    }

    public static void ReduceMultiStepCard(this StorageMonopoly storage)
    {
        var oldValue = storage.StepMultiValue();
        storage.StepMultiList.RemoveAt(0);
        var newValue = storage.StepMultiValue();
        EventDispatcher.Instance.SendEventImmediately(new EventMonopolyStepMultiChange(oldValue, newValue));
    }

    public static void AddMultiScoreCard(this StorageMonopoly storage, int multiValue, bool autoSendEvent = true)
    {
        var oldValue = storage.ScoreMultiValue();
        storage.ScoreMultiList.Add(multiValue);
        var newValue = storage.ScoreMultiValue();
        if (autoSendEvent)
        {
            EventDispatcher.Instance.SendEventImmediately(new EventMonopolyScoreMultiChange(oldValue, newValue));
        }
    }

    public static void ReduceMultiScoreCard(this StorageMonopoly storage)
    {
        var oldValue = storage.ScoreMultiValue();
        storage.ScoreMultiList.RemoveAt(0);
        var newValue = storage.ScoreMultiValue();
        EventDispatcher.Instance.SendEventImmediately(new EventMonopolyScoreMultiChange(oldValue, newValue));
    }

    public static bool BuyStoreItem(this StorageMonopoly storage, MonopolyStoreItemConfig storeItemConfig)
    {
        if (storage.Score < storeItemConfig.Price)
            return false;
        if (!MonopolyModel.Instance.ReduceScore(storeItemConfig.Price, "BuyItem"))
            return false;
        GameBIManager.Instance.SendGameEvent(
            BiEventAdventureIslandMerge.Types.GameEventType.GameEventMonopolyRadishExchange,
            storeItemConfig.Id.ToString(), storage.GetCurStoreLevel().Id.ToString(), storage.Score.ToString());
        storage.FinishStoreItemList.Add(storeItemConfig.Id);
        if ((MonopolyStoreItemType) storeItemConfig.Type == MonopolyStoreItemType.BuildItem)
        {
            var unOwnNode = false;
            var showItemList = new List<int>();
            foreach (var buildItemId in storeItemConfig.RewardId)
            {
                var decoItem = DecoWorld.ItemLib[buildItemId];
                DecoManager.Instance.UnlockDecoBuilding(buildItemId,decoItem.Node.Stage.Area.Config.hideAreaInDeco);
                if (decoItem.Node.Stage.Area.Config.hideAreaInDeco || decoItem._node.IsOwned)
                {
                    showItemList.Add(buildItemId);
                }
                else
                {
                    unOwnNode = true;
                }
            }

            if (showItemList.Count > 0)
            {
                var hasShopUI = false;
                var hasMainUI = false;
                {
                    var shopUI =
                        UIManager.Instance
                            .GetOpenedUIByPath<UIMonopolyShopController>(UINameConst.UIMonopolyShop);
                    if (shopUI)
                    {
                        hasShopUI = true;
                        shopUI.CloseWindowWithinUIMgr(true);
                    }

                    var mainUI =
                        UIManager.Instance
                            .GetOpenedUIByPath<UIMonopolyMainController>(UINameConst.UIMonopolyMain);
                    if (mainUI)
                    {
                        hasMainUI = true;
                        mainUI.CloseWindowWithinUIMgr(true);
                    }
                }
                Action Callback = () =>
                {
                    if (unOwnNode)
                        CommonUtils.OpenCommonConfirmWindow(new NoticeUIData
                        {
                            DescString = LocalizationManager.Instance.GetLocalizedString("ui_easter_node_lock_tips"),
                        });
                    if (hasMainUI)
                    {
                        MonopolyModel.CanShowMainPopup();
                    }

                    if (hasShopUI)
                    {
                        UIMonopolyShopController.Open(storage);
                    }
                };

                if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Game|| FarmModel.Instance.IsFarmModel())
                {
                    SceneFsm.mInstance.ChangeState(StatusType.Transition, StatusType.BackHome,
                        DecoOperationType.Install,
                        showItemList, Callback);
                }
                else
                {
                    DecoManager.Instance.InstallItem(showItemList, Callback);
                }
            }
            else
            {
                if (unOwnNode)
                    CommonUtils.OpenCommonConfirmWindow(new NoticeUIData
                    {
                        DescString = LocalizationManager.Instance.GetLocalizedString("ui_easter_node_lock_tips"),
                    });
            }
        }
        else
        {
            var rewardData = CommonUtils.FormatReward(storeItemConfig.RewardId, storeItemConfig.RewardNum);
            var reasonArgs = new GameBIManager.ItemChangeReasonArgs()
            {
                reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.MonopolyGet
            };
            CommonRewardManager.Instance.PopCommonReward(rewardData, CurrencyGroupManager.Instance.currencyController,
                true,
                reasonArgs);
        }

        EventDispatcher.Instance.SendEventImmediately(new EventMonopolyBuyStoreItem(storeItemConfig));
        return true;
    }

    public static int GetUnUsedCardCount(this StorageMonopoly storage, MonopolyCardState cardState)
    {
        if (cardState.CardType == MonopolyCardType.Wild)
        {
            return storage.WildCardCount;
        }
        else
        {
            return 0;
        }
    }

    public static MonopolyStoreLevelConfig GetCurStoreLevel(this StorageMonopoly storage)
    {
        if (!MonopolyModel.Instance.IsInitFromServer())
        {
            return null;
        }

        var levelConfig = MonopolyModel.Instance.StoreLevelConfig;
        for (var i = 0; i < levelConfig.Count; i++)
        {
            var storeItemList = levelConfig[i].StoreItemList;
            for (var i1 = 0; i1 < storeItemList.Count; i1++)
            {
                if (!storage.FinishStoreItemList.Contains(storeItemList[i1]))
                {
                    return levelConfig[i];
                }
            }
        }

        return levelConfig.Last();
    }

    public static MonopolyRewardBoxConfig GetCurRewardBoxConfig(this StorageMonopoly storage)
    {
        var rewardBoxConfig = MonopolyModel.Instance.RewardBoxConfig;
        if (storage.RewardBoxCompleteTimes < rewardBoxConfig.Count)
        {
            return rewardBoxConfig[storage.RewardBoxCompleteTimes];
        }
        else
        {
            var repeatList = MonopolyModel.Instance.GlobalConfig.LoopRewardBoxList;
            var loopTimes = storage.RewardBoxCompleteTimes - rewardBoxConfig.Count;
            loopTimes %= repeatList.Count;
            return rewardBoxConfig.Find(a => a.Id == repeatList[loopTimes]);
        }
    }

    public static void AddRewardBoxProgress(this StorageMonopoly storage, int addCount)
    {
        var oldValue = storage.RewardBoxCollectNum;
        var newValue = oldValue + addCount;
        var box = storage.GetCurRewardBoxConfig();
        newValue = Math.Min(box.CollectNum, newValue);
        if (newValue != oldValue)
        {
            storage.RewardBoxCollectNum = newValue;
            EventDispatcher.Instance.SendEventImmediately(
                new EventMonopolyUIAddRewardBoxScore(box, oldValue, newValue));
        }
    }

    public static bool TryCollectRewardBox(this StorageMonopoly storage)
    {
        var box = storage.GetCurRewardBoxConfig();
        if (storage.RewardBoxCollectNum < box.CollectNum)
        {
            return false;
        }

        storage.RewardBoxCompleteTimes++;
        storage.RewardBoxCollectNum = 0;
        var rewards = CommonUtils.FormatReward(box.RewardId, box.RewardNum);
        UserData.Instance.AddRes(rewards, new GameBIManager.ItemChangeReasonArgs()
        {
            reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.MonopolyGet
        });
        EventDispatcher.Instance.SendEventImmediately(new EventMonopolyUICollectRewardBox(box));
        return true;
    }

    public static int ScoreMultiValue(this StorageMonopoly storage)
    {
        if (storage.ScoreMultiList.Count > 0)
            return storage.ScoreMultiList[0];
        return 1;
    }

    public static int StepMultiValue(this StorageMonopoly storage)
    {
        if (storage.StepMultiList.Count > 0)
            return storage.StepMultiList[0];
        return 1;
    }

    public static long GetPreheatLeftTime(this StorageMonopoly storage)
    {
        var heatTime = storage.PreheatCompleteTime - (long) APIManager.Instance.GetServerTime();
        heatTime = Math.Max(heatTime, 0);
        return heatTime;
    }

    public static void SetPreheatLeftTime(this StorageMonopoly storageWeek, long leftTime)
    {
        storageWeek.PreheatCompleteTime = (long) APIManager.Instance.GetServerTime() + leftTime;
    }

    public static string GetPreheatLeftTimeText(this StorageMonopoly storage)
    {
        return CommonUtils.FormatLongToTimeStr(storage.GetPreheatLeftTime());
    }

    public static bool IsTimeOut(this StorageMonopoly storageWeek)
    {
        return storageWeek.GetLeftTime() <= 0;
    }

    public static long GetLeftTime(this StorageMonopoly storageWeek)
    {
        return Math.Max(storageWeek.EndTime - (long) APIManager.Instance.GetServerTime(), 0);
    }

    public static void SetLeftTime(this StorageMonopoly storageWeek, long leftTime)
    {
        var endTime = (long) APIManager.Instance.GetServerTime() + leftTime;
        storageWeek.EndTime = endTime;
    }

    public static string GetLeftTimeText(this StorageMonopoly storageWeek)
    {
        return CommonUtils.FormatLongToTimeStr(storageWeek.GetLeftTime());
    }

    public static void CompletedStorageActivity(this StorageMonopoly storageWeek)
    {
        if (storageWeek.TryRelease())
            MonopolyModel.Instance.CreateStorage();
    }

    public static bool TryRelease(this StorageMonopoly storage)
    {
        if (storage.IsTimeOut())
        {
            Debug.LogError("删除ActivityId = " + storage.ActivityId + "排行榜数据");
            MonopolyModel.StorageMonopoly.Remove(storage.ActivityId);
            return true;
        }

        return false;
    }


    public static bool ShowEntrance(this StorageMonopoly storage)
    {
        if (!MonopolyModel.Instance.IsOpened())
            return false;
        if (storage.IsTimeOut())
            return false;
        return true;
    }

    public static string GetAuxItemAssetPath(this StorageMonopoly storage)
    {
        return "Prefabs/Activity/Monopoly/Aux_Monopoly";
    }

    public static string GetTaskItemAssetPath(this StorageMonopoly storage)
    {
        return "Prefabs/Activity/Monopoly/TaskList_Monopoly";
    }
}