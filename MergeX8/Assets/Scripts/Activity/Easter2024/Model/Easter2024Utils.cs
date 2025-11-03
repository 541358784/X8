using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Decoration;
using DragonPlus;
using DragonPlus.Config.Easter2024;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Farm.Model;
using Gameplay;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

public enum Easter2024StoreItemType
{
    MergeItem = 1,
    BuildItem = 2,
}
public static class Easter2024Utils
{
    public static bool BuyStoreItem(this StorageEaster2024 storage,Easter2024StoreItemConfig storeItemConfig)
    {
        if (storage.Score < storeItemConfig.Price)
            return false;
        if (!Easter2024Model.Instance.ReduceScore(storeItemConfig.Price, "BuyItem"))
            return false;
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventEasterRadishExchange,
            storeItemConfig.Id.ToString(),storage.GetCurStoreLevel().Id.ToString(),storage.Score.ToString());
        storage.FinishStoreItemList.Add(storeItemConfig.Id);
        if ((Easter2024StoreItemType) storeItemConfig.Type == Easter2024StoreItemType.BuildItem)
        {
            foreach (var buildItemId in storeItemConfig.RewardId)
            {
                DecoManager.Instance.UnlockDecoBuilding(buildItemId);
            }

            var hasShopUI = false;
            var hasMainUI = false;
            {
                var shopUI = UIManager.Instance.GetOpenedUIByPath<UIEaster2024ShopController>(UINameConst.UIEaster2024Shop);
                if (shopUI)
                {
                    hasShopUI = true;
                    shopUI.CloseWindowWithinUIMgr(true);
                }
                var mainUI = UIManager.Instance.GetOpenedUIByPath<UIEaster2024MainController>(UINameConst.UIEaster2024Main);
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
                    Easter2024Model.CanShowMainPopup();
                }
                if (hasShopUI)
                {
                    UIEaster2024ShopController.Open(storage);
                }
            };
            
            if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Game || FarmModel.Instance.IsFarmModel())
            {
                SceneFsm.mInstance.ChangeState(StatusType.Transition, StatusType.BackHome,DecoOperationType.Install, storeItemConfig.RewardId,Callback);
            }
            else
            {
                DecoManager.Instance.InstallItem(storeItemConfig.RewardId,Callback);
            }
        }
        else
        {
            var rewardData = CommonUtils.FormatReward(storeItemConfig.RewardId, storeItemConfig.RewardNum);
            var reasonArgs = new GameBIManager.ItemChangeReasonArgs()
            {
                reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.Easter2024Get
            };
            CommonRewardManager.Instance.PopCommonReward(rewardData,CurrencyGroupManager.Instance.currencyController, true,
                reasonArgs);
        }
        EventDispatcher.Instance.SendEvent(new EventEaster2024BuyStoreItem(storeItemConfig));
        return true;
    }
    public static int GetUnUsedCardCount(this StorageEaster2024 storage, Easter2024CardState cardState)
    {
        if (cardState.CardType == Easter2024CardType.ExtraBall)
        {
            var count = 0;
            for (var i = 0; i<storage.ExtraBallList.Count;i++)
            {
                if (storage.ExtraBallList[i] == cardState.BallCount)
                {
                    count++;
                }
            }
            return count;
        }
        else if (cardState.CardType == Easter2024CardType.MultiScore)
        {
            var count = 0;
            for (var i = 0; i<storage.MultiBallList.Count;i++)
            {
                if (storage.MultiBallList[i] == cardState.MultiValue)
                {
                    count++;
                }
            }
            return count;
        }
        else
        {
            return 0;
        }
    }
    public static Easter2024StoreLevelConfig GetCurStoreLevel(this StorageEaster2024 storage)
    {
        if (!Easter2024Model.Instance.IsInitFromServer())
        {
            return null;
        }
        var levelConfig = Easter2024Model.Instance.StoreLevelConfig;
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
    public static Easter2024LevelConfig GetCurLevel(this StorageEaster2024 storage)
    {
        if (!Easter2024Model.Instance.IsInitFromServer())
        {
            return null;
        }
        var levelConfig = Easter2024Model.Instance.LevelConfig;
        Easter2024LevelConfig curLevel = null;
        for (var i = 0; i < levelConfig.Count; i++)
        {
            if (storage.TotalScore >= levelConfig[i].ScoreLimit)
                curLevel = levelConfig[i];
            else
                return curLevel;
        }
        return levelConfig.Last();
    }

    public static int GetLevelMaxScore(this Easter2024LevelConfig cfg)
    {
        var levelConfig = Easter2024Model.Instance.LevelConfig;
        Easter2024LevelConfig curLevel = null;
        for (var i = 0; i < levelConfig.Count; i++)
        {
            if (cfg.ScoreLimit < levelConfig[i].ScoreLimit)
                return levelConfig[i].ScoreLimit - cfg.ScoreLimit;
        }
        return -1;
    }
    public static Easter2024LevelConfig GetNextLevel(this Easter2024LevelConfig cfg)
    {
        var levelConfig = Easter2024Model.Instance.LevelConfig;
        Easter2024LevelConfig curLevel = null;
        for (var i = 0; i < levelConfig.Count; i++)
        {
            if (cfg == levelConfig[i])
            {
                if ((i + 1) < levelConfig.Count)
                    return levelConfig[i + 1];
                else
                    return null;
            }
        }
        return null;
    }
    public static long GetPreheatLeftTime(this StorageEaster2024 storage)
    {
        var heatTime = (Easter2024Model.Instance.IsSkipActivityPreheating()?storage.StartTime:storage.PreheatCompleteTime) - (long)APIManager.Instance.GetServerTime();
        heatTime = Math.Max(heatTime, 0);
        return heatTime;
    }
    public static void SetPreheatLeftTime(this StorageEaster2024 storageWeek,long leftTime)
    {
        storageWeek.PreheatCompleteTime = (long) APIManager.Instance.GetServerTime() + leftTime;
    }

    public static string GetPreheatLeftTimeText(this StorageEaster2024 storage)
    {
        return CommonUtils.FormatLongToTimeStr(storage.GetPreheatLeftTime());
    }
    public static bool IsTimeOut(this StorageEaster2024 storageWeek)
    {
        return storageWeek.GetLeftTime() <= 0;
    }
    public static long GetLeftTime(this StorageEaster2024 storageWeek)
    {
        return Math.Max(storageWeek.EndTime - (long) APIManager.Instance.GetServerTime(), 0);
    }
    public static void SetLeftTime(this StorageEaster2024 storageWeek,long leftTime)
    {
        var endTime = (long) APIManager.Instance.GetServerTime() + leftTime;
        storageWeek.EndTime = endTime;
        storageWeek.LeaderBoardStorage.EndTime = endTime;
    }
    public static string GetLeftTimeText(this StorageEaster2024 storageWeek)
    {
        return CommonUtils.FormatLongToTimeStr(storageWeek.GetLeftTime());
    }
    public static void CheckCompletedAllNode(this StorageEaster2024 storage)
    {
        storage.IsFinish = true;
    }

    public static void CompletedStorageActivity(this StorageEaster2024LeaderBoard leadBoardStorage)
    {
        leadBoardStorage.IsFinish = true;
        foreach (var pair in Easter2024Model.StorageEaster2024)
        {
            if (pair.Value.LeaderBoardStorage == leadBoardStorage)
            {
                if (pair.Value.TryRelease())
                    Easter2024Model.Instance.CreateStorage();
                return;
            }
        }
    }

    public static bool TryRelease(this StorageEaster2024 storage)
    {
        if (storage.IsTimeOut() && (!storage.LeaderBoardStorage.IsInitFromServer() || storage.LeaderBoardStorage.IsFinish))
        {
            Debug.LogError("删除ActivityId = "+storage.ActivityId+"排行榜数据");
            Easter2024Model.StorageEaster2024.Remove(storage.ActivityId);
            Easter2024LeaderBoardUtils.StorageWeekInitStateDictionary.Remove(storage.LeaderBoardStorage.ActivityId);
            return true;
        }
        return false;
    }
    
}