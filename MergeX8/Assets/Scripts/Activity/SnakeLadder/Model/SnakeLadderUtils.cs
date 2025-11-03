using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Decoration;
using DragonPlus;
using DragonPlus.Config.SnakeLadder;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Farm.Model;
using Gameplay;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;
using Utils = DragonU3DSDK.Utils;

public enum SnakeLadderStoreItemType
{
    MergeItem = 1,
    BuildItem = 2,
}

public static class SnakeLadderUtils
{
    public const ulong Offset = 0 * XUtility.Hour;
    public static int CurDay => (int)((APIManager.Instance.GetServerTime()-Offset) / XUtility.DayTime);
    public static ulong CurDayLeftTime => XUtility.DayTime - ((APIManager.Instance.GetServerTime() - Offset) % XUtility.DayTime);
    public static string CurDayLeftTimeString => CommonUtils.FormatLongToTimeStr((long)CurDayLeftTime);
    public static int GetCurDatBuyTimes(this StorageSnakeLadder storage)
    {
        if (!storage.TurntableBuyState.TryGetValue(CurDay,out var buyTimes))
        {
            buyTimes = 0;
            storage.TurntableBuyState.Add(CurDay,buyTimes);
        }
        return buyTimes;
    }

    public static void AddCurDatBuyTimes(this StorageSnakeLadder storage,int addValue)
    {
        if (!storage.TurntableBuyState.TryGetValue(CurDay,out var buyTimes))
        {
            buyTimes = 0;
            storage.TurntableBuyState.Add(CurDay,buyTimes);
        }
        storage.TurntableBuyState[CurDay] += addValue;
    }

    public static Dictionary<SnakeLadderLevelConfig, List<int>> CardPoolDictionary = new Dictionary<SnakeLadderLevelConfig, List<int>>();
    public static List<int> CardPool(this SnakeLadderLevelConfig levelConfig)
    {
        if (!CardPoolDictionary.TryGetValue(levelConfig, out var cardPool))
        {
            cardPool = new List<int>();
            for (var i = 0; i < levelConfig.CardPoolId.Count; i++)
            {
                var cardId = levelConfig.CardPoolId[i];
                var cardNum = levelConfig.CardPoolNum[i];
                for (var j = 0; j < cardNum; j++)
                {
                    cardPool.Add(cardId);
                }
            }
            CardPoolDictionary.Add(levelConfig,cardPool);
        }
        return cardPool;
    }
    public static void SpinTurntable(this StorageSnakeLadder storage)
    {
        var curLevel = storage.GetCurLevel();
        var cardPool = new List<SnakeLadderCardConfig>();
        for (var i = 0; i < storage.TurntableRandomPool.Count; i++)
        {
            var tempCardConfig = SnakeLadderModel.Instance.CardConfig[storage.TurntableRandomPool[i]];
            cardPool.Add(tempCardConfig);
        }

        if (cardPool.Count == 0)
        {
            throw new Exception("异常情况,卡池空了");
            // for (var i = 0; i < curLevel.CardPool.Count; i++)
            // {
            //     var cardConfig = SnakeLadderModel.Instance.CardConfig[curLevel.CardPool[i]];
            //     if ((SnakeLadderCardType) cardConfig.CardType == SnakeLadderCardType.Score &&
            //         cardConfig.Id % 100 == 1)
            //     {
            //         cardPool.Add(cardConfig);
            //         break;
            //     }
            // }
        }

        var weightPool = new List<int>();
        for (var i = 0; i < cardPool.Count; i++)
        {
            weightPool.Add(cardPool[i].Weight);
        }

        var index = Utils.RandomByWeight(weightPool);
        var cardConfig = cardPool[index];
        storage.TurntableRandomPool.Remove(cardConfig.Id);
        if (storage.TurntableRandomPool.Count == 0)
        {
            if (cardConfig.Id != 7 && !GuideSubSystem.Instance.isFinished(GuideTriggerPosition.SnakeLadderGetWildCard))
            {
                storage.TurntableRandomPool.Clear();
                storage.TurntableRandomPool.Add(7);
            }
            else
            {
                for (var i = 0; i < curLevel.CardPool().Count; i++)
                {
                    storage.TurntableRandomPool.Add(curLevel.CardPool()[i]);
                }   
            }
        }

        var spinResultText = "Spin ";
        EventDispatcher.Instance.SendEventImmediately(new EventSnakeLadderUIPlayTurntable(cardConfig));
        var cardState = new SnakeLadderCardState(cardConfig);
        if (cardState.CardType == SnakeLadderCardType.Step)
        {
            SnakeLadderModel.Instance.AddStep(cardState.Step,true,true);
            spinResultText += "Step" + cardState.Step;
        }
        else
        {
            EventDispatcher.Instance.SendEventImmediately(new EventSnakeLadderUIGetCard(cardConfig));
            if (cardState.CardType == SnakeLadderCardType.Score)
            {
                SnakeLadderModel.Instance.AddScore(cardState.Score,"Card",true);
                spinResultText += "Score" + cardState.Score;
            }
            else if (cardState.CardType == SnakeLadderCardType.MultiScore)
            {
                SnakeLadderModel.Instance.AddMultiScoreCard(cardState.MultiScore,true);
                spinResultText += "MultiScore";
            }
            else if (cardState.CardType == SnakeLadderCardType.MultiStep)
            {
                SnakeLadderModel.Instance.AddMultiStepCard(cardState.MultiStep,true);
                spinResultText += "MultiStep";
            }
            else if (cardState.CardType == SnakeLadderCardType.Wild)
            {
                SnakeLadderModel.Instance.AddWildCard(true);
                spinResultText += "Wild";
            }
            else if (cardState.CardType == SnakeLadderCardType.Defense)
            {
                SnakeLadderModel.Instance.AddDefenseCard(true);
                spinResultText += "Defense";
            }
        }
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventSnakeLadderTurntableChange,
            "-1",storage.TurntableCount.ToString(),spinResultText);
    }

    public static bool BuyStoreItem(this StorageSnakeLadder storage, SnakeLadderStoreItemConfig storeItemConfig)
    {
        if (storage.Score < storeItemConfig.Price)
            return false;
        if (!SnakeLadderModel.Instance.ReduceScore(storeItemConfig.Price, "BuyItem"))
            return false;
        GameBIManager.Instance.SendGameEvent(
            BiEventAdventureIslandMerge.Types.GameEventType.GameEventSnakeLadderRadishExchange,
            storeItemConfig.Id.ToString(), storage.GetCurStoreLevel().Id.ToString(), storage.Score.ToString());
        storage.FinishStoreItemList.Add(storeItemConfig.Id);
        if ((SnakeLadderStoreItemType) storeItemConfig.Type == SnakeLadderStoreItemType.BuildItem)
        {
            
            
            foreach (var buildItemId in storeItemConfig.RewardId)
            {
                DecoManager.Instance.UnlockDecoBuilding(buildItemId);
            }
            
            var hasShopUI = false;
            var hasMainUI = false;
            {
                var shopUI =
                    UIManager.Instance
                        .GetOpenedUIByPath<UISnakeLadderShopController>(UINameConst.UISnakeLadderShop);
                if (shopUI)
                {
                    hasShopUI = true;
                    shopUI.CloseWindowWithinUIMgr(true);
                }
            
                var mainUI =
                    UIManager.Instance
                        .GetOpenedUIByPath<UISnakeLadderMainController>(UINameConst.UISnakeLadderMain);
                if (mainUI)
                {
                    hasMainUI = true;
                    mainUI.CloseWindowWithinUIMgr(true);
                }
            }
            Action Callback = () =>
            {
                if (hasMainUI)
                {
                    SnakeLadderModel.CanShowMainPopup();
                }
            
                if (hasShopUI)
                {
                    UISnakeLadderShopController.Open(storage);
                }
            };
            
            if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Game || FarmModel.Instance.IsFarmModel())
            {
                SceneFsm.mInstance.ChangeState(StatusType.Transition, StatusType.BackHome, DecoOperationType.Install,
                    storeItemConfig.RewardId, Callback);
            }
            else
            {
                DecoManager.Instance.InstallItem(storeItemConfig.RewardId, Callback);
            }
        }
        else
        {
            var rewardData = CommonUtils.FormatReward(storeItemConfig.RewardId, storeItemConfig.RewardNum);
            var reasonArgs = new GameBIManager.ItemChangeReasonArgs()
            {
                reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.SnakeLadderGet
            };
            CommonRewardManager.Instance.PopCommonReward(rewardData, CurrencyGroupManager.Instance.currencyController,
                true,
                reasonArgs);
        }

        EventDispatcher.Instance.SendEventImmediately(new EventSnakeLadderBuyStoreItem(storeItemConfig));
        return true;
    }

    public static int GetUnUsedCardCount(this StorageSnakeLadder storage, SnakeLadderCardState cardState)
    {
        if (cardState.CardType == SnakeLadderCardType.Wild)
        {
            return storage.WildCardCount;
        }
        else if (cardState.CardType == SnakeLadderCardType.Defense)
        {
            return storage.DefenseCardCount;
        }
        else
        {
            return 0;
        }
    }

    public static SnakeLadderStoreLevelConfig GetCurStoreLevel(this StorageSnakeLadder storage)
    {
        if (!SnakeLadderModel.Instance.IsInitFromServer())
        {
            return null;
        }

        var levelConfig = SnakeLadderModel.Instance.StoreLevelConfig;
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

    public static SnakeLadderLevelConfig GetCurLevel(this StorageSnakeLadder storage)
    {
        if (!SnakeLadderModel.Instance.IsInitFromServer())
        {
            return null;
        }

        var levelConfig = SnakeLadderModel.Instance.LevelConfig;
        for (var i = 0; i < levelConfig.Count; i++)
        {
            if (storage.CompleteTimes < levelConfig[i].PlayTimes)
                return levelConfig[i];
        }
        var loopLevelList = SnakeLadderModel.Instance.GlobalConfig.LoopLevelList;
        var loopLevelListIndex = storage.CompleteTimes - levelConfig.Last().PlayTimes;
        loopLevelListIndex %= loopLevelList.Count;
        var loopLevelId = loopLevelList[loopLevelListIndex];
        var curLevelConfig = levelConfig.Find((a) => a.Id == loopLevelId);
        return curLevelConfig;
    }

    public static Dictionary<SnakeLadderLevelConfig, List<SnakeLadderBlockConfig>> LevelBlckConfigListDic =
        new Dictionary<SnakeLadderLevelConfig, List<SnakeLadderBlockConfig>>();

    public static List<SnakeLadderBlockConfig> GetBlockConfigList(this SnakeLadderLevelConfig levelConfig)
    {
        if (!LevelBlckConfigListDic.TryGetValue(levelConfig, out var blockConfigList))
        {
            blockConfigList = new List<SnakeLadderBlockConfig>();
            for (var i = 0; i < levelConfig.BlockList.Count; i++)
            {
                var blockConfig = SnakeLadderModel.Instance.BlockConfig[levelConfig.BlockList[i]];
                blockConfigList.Add(blockConfig);
            }

            LevelBlckConfigListDic.Add(levelConfig, blockConfigList);
        }

        return blockConfigList;
    }

    public static long GetPreheatLeftTime(this StorageSnakeLadder storage)
    {
        var heatTime = (SnakeLadderModel.Instance.IsSkipActivityPreheating()?storage.StartTime:storage.PreheatCompleteTime) - (long) APIManager.Instance.GetServerTime();
        heatTime = Math.Max(heatTime, 0);
        return heatTime;
    }

    public static void SetPreheatLeftTime(this StorageSnakeLadder storageWeek, long leftTime)
    {
        storageWeek.PreheatCompleteTime = (long) APIManager.Instance.GetServerTime() + leftTime;
    }

    public static string GetPreheatLeftTimeText(this StorageSnakeLadder storage)
    {
        return CommonUtils.FormatLongToTimeStr(storage.GetPreheatLeftTime());
    }

    public static bool IsTimeOut(this StorageSnakeLadder storageWeek)
    {
        return storageWeek.GetLeftTime() <= 0;
    }

    public static long GetLeftTime(this StorageSnakeLadder storageWeek)
    {
        return Math.Max(storageWeek.EndTime - (long) APIManager.Instance.GetServerTime(), 0);
    }

    public static void SetLeftTime(this StorageSnakeLadder storageWeek, long leftTime)
    {
        var endTime = (long) APIManager.Instance.GetServerTime() + leftTime;
        storageWeek.EndTime = endTime;
        storageWeek.LeaderBoardStorage.EndTime = endTime;
    }

    public static string GetLeftTimeText(this StorageSnakeLadder storageWeek)
    {
        return CommonUtils.FormatLongToTimeStr(storageWeek.GetLeftTime());
    }

    public static void CompletedStorageActivity(this StorageSnakeLadderLeaderBoard leadBoardStorage)
    {
        leadBoardStorage.IsFinish = true;
        foreach (var pair in SnakeLadderModel.StorageSnakeLadder)
        {
            if (pair.Value.LeaderBoardStorage == leadBoardStorage)
            {
                if (pair.Value.TryRelease())
                    SnakeLadderModel.Instance.CreateStorage();
                return;
            }
        }
    }

    public static bool TryRelease(this StorageSnakeLadder storage)
    {
        if (storage.IsTimeOut() &&
            (!storage.LeaderBoardStorage.IsInitFromServer() || storage.LeaderBoardStorage.IsFinish))
        {
            Debug.LogError("删除ActivityId = " + storage.ActivityId + "排行榜数据");
            SnakeLadderModel.StorageSnakeLadder.Remove(storage.ActivityId);
            SnakeLadderLeaderBoardUtils.StorageWeekInitStateDictionary.Remove(storage.LeaderBoardStorage.ActivityId);
            return true;
        }

        return false;
    }
}