using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Decoration;
using DragonPlus;
using DragonPlus.Config.ThemeDecoration;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Farm.Model;
using Gameplay;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;
using Utils = DragonU3DSDK.Utils;

public enum ThemeDecorationStoreItemType
{
    MergeItem = 1,
    BuildItem = 2,
}

public static class ThemeDecorationUtils
{
    public static bool BuyStoreItem(this StorageThemeDecoration storage, ThemeDecorationStoreItemConfig storeItemConfig)
    {
        if (storage.FinishStoreItemList.Contains(storeItemConfig.Id))
            return false;
        if (storage.Score < storeItemConfig.Price)
            return false;
        ThemeDecorationModel.Instance.ReduceScore(storeItemConfig.Price, "BuyItem");
        GameBIManager.Instance.SendGameEvent(
            BiEventAdventureIslandMerge.Types.GameEventType.GameEventThemeDecorationRadishExchange,
            storeItemConfig.Id.ToString(), storage.GetCurStoreLevel().Id.ToString(), storage.Score.ToString());
        storage.FinishStoreItemList.Add(storeItemConfig.Id);

        for (var i = 0; i < ThemeDecorationModel.Instance.StoreLevelConfig.Count; i++)
        {
            var levelConfig = ThemeDecorationModel.Instance.StoreLevelConfig[i];
            if (!storage.CollectStoreLevel.Contains(levelConfig.Id) &&
                storage.CanCollectLevelCompleteReward(levelConfig))
            {
                storage.CollectStoreLevel.Add(levelConfig.Id);
                var rewardData = CommonUtils.FormatReward(levelConfig.RewardId, levelConfig.RewardNum);
                foreach (var resData in rewardData)
                {
                    if (!storage.UnCollectRewards.ContainsKey(resData.id))
                    {
                        storage.UnCollectRewards.Add(resData.id, 0);
                    }

                    storage.UnCollectRewards[resData.id] += resData.count;
                }
            }
        }

        if ((ThemeDecorationStoreItemType)storeItemConfig.Type == ThemeDecorationStoreItemType.BuildItem)
        {
            foreach (var buildItemId in storeItemConfig.RewardId)
            {
                DecoManager.Instance.UnlockDecoBuilding(buildItemId);
            }

            var hasShopUI = false;
            {
                var shopUI =
                    UIManager.Instance.GetOpenedUIByPath<UIThemeDecorationShopController>(
                        storage.GetAssetPathWithSkinName(UINameConst.UIThemeDecorationShop));
                if (shopUI)
                {
                    hasShopUI = true;
                    shopUI.CloseWindowWithinUIMgr(true);
                }
            }
            Action Callback = () =>
            {
                if (hasShopUI)
                {
                    UIThemeDecorationShopController.Open(storage);
                    XUtility.WaitSeconds(0.3f,
                        () =>
                        {
                            EventDispatcher.Instance.SendEventImmediately(
                                new EventThemeDecorationBuyStoreItem(storeItemConfig));
                        });
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
                reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.ThemeDecorationGet
            };
            CommonRewardManager.Instance.PopCommonReward(rewardData, CurrencyGroupManager.Instance.currencyController,
                true,
                reasonArgs,
                animEndCall: () =>
                {
                    EventDispatcher.Instance.SendEventImmediately(
                        new EventThemeDecorationBuyStoreItem(storeItemConfig));
                });
        }

        // EventDispatcher.Instance.SendEventImmediately(new EventThemeDecorationBuyStoreItem(storeItemConfig));
        return true;
    }

    public static ThemeDecorationStoreLevelConfig GetCurStoreLevel(this StorageThemeDecoration storage)
    {
        if (!ThemeDecorationModel.Instance.IsInitFromServer())
        {
            return null;
        }

        var levelConfig = ThemeDecorationModel.Instance.StoreLevelConfig;
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

    public static long GetPreheatLeftTime(this StorageThemeDecoration storage) =>
        Math.Max(storage.PreheatCompleteTime - (long)APIManager.Instance.GetServerTime(), 0);

    public static void SetPreheatLeftTime(this StorageThemeDecoration storageWeek, long leftTime) =>
        storageWeek.PreheatCompleteTime = (long)APIManager.Instance.GetServerTime() + leftTime;

    public static string GetPreheatLeftTimeText(this StorageThemeDecoration storage) =>
        CommonUtils.FormatLongToTimeStr(storage.GetPreheatLeftTime());

    public static long GetPreEndBuyLeftTime(this StorageThemeDecoration storage) =>
        Math.Max(storage.PreEndBuyTime - (long)APIManager.Instance.GetServerTime(), 0);

    public static void SetPreEndBuyLeftTime(this StorageThemeDecoration storageWeek, long leftTime) =>
        storageWeek.PreEndBuyTime = (long)APIManager.Instance.GetServerTime() + leftTime;

    public static string GetPreEndBuyLeftTimeText(this StorageThemeDecoration storage) =>
        CommonUtils.FormatLongToTimeStr(storage.GetPreEndBuyLeftTime());

    public static long GetPreEndLeftTime(this StorageThemeDecoration storageWeek) =>
        Math.Max(storageWeek.PreEndTime - (long)APIManager.Instance.GetServerTime(), 0);

    public static void SetPreEndLeftTime(this StorageThemeDecoration storageWeek, long leftTime) =>
        storageWeek.PreEndTime = (long)APIManager.Instance.GetServerTime() + leftTime;

    public static string GetPreEndLeftTimeText(this StorageThemeDecoration storageWeek) =>
        CommonUtils.FormatLongToTimeStr(storageWeek.GetPreEndLeftTime());

    public static long GetLeftTime(this StorageThemeDecoration storageWeek) =>
        Math.Max(storageWeek.EndTime - (long)APIManager.Instance.GetServerTime(), 0);

    public static void SetLeftTime(this StorageThemeDecoration storageWeek, long leftTime) =>
        storageWeek.EndTime = (long)APIManager.Instance.GetServerTime() + leftTime;

    public static string GetLeftTimeText(this StorageThemeDecoration storageWeek) =>
        CommonUtils.FormatLongToTimeStr(storageWeek.GetLeftTime());

    public static long GetTotalLeftTime(this StorageThemeDecoration storageWeek) => storageWeek.IsBuyPreEnd
        ? storageWeek.GetLeftTime()
        : storageWeek.GetPreEndLeftTime();

    public static string GetTotalLeftTimeText(this StorageThemeDecoration storageWeek) =>
        CommonUtils.FormatLongToTimeStr(storageWeek.GetTotalLeftTime());

    public static bool IsTimeOut(this StorageThemeDecoration storageWeek) => storageWeek.GetLeftTime() <= 0;
    public static bool IsPreheat(this StorageThemeDecoration storageWeek) => storageWeek.GetPreheatLeftTime() <= 0;
    public static bool IsPreEnd(this StorageThemeDecoration storageWeek) => storageWeek.GetPreEndLeftTime() <= 0;
    public static bool IsTotalTimeOut(this StorageThemeDecoration storageWeek) => storageWeek.GetTotalLeftTime() <= 0;

    public static bool CanBuyEnd(this StorageThemeDecoration storageWeek) =>
        !storageWeek.IsBuyPreEnd && storageWeek.GetPreEndBuyLeftTime() > 0;

    public static void CompletedStorageActivity(this StorageThemeDecoration storage)
    {
        if (storage.TryRelease())
            ThemeDecorationModel.Instance.CreateStorage();
    }

    public static StorageThemeDecorationLeaderBoard GetUnCollectLeaderBoard(this StorageThemeDecoration storage)
    {
        foreach (var leaderBoard in storage.LeaderBoardStorageList)
        {
            if (leaderBoard.CanStorageThemeDecorationLeaderBoardGetReward())
                return leaderBoard;
        }

        return null;
    }

    public static List<ResData> GetUnCollectReward(this StorageThemeDecoration storage)
    {
        if (!storage.IsTimeOut())
            return null;
        if (storage.UnCollectRewards.Count == 0)
            return null;
        var resList = new List<ResData>();
        foreach (var pair in storage.UnCollectRewards)
        {
            resList.Add(new ResData(pair.Key, pair.Value));
        }

        return resList;
    }

    public static bool TryRelease(this StorageThemeDecoration storage)
    {
        if (storage.IsTimeOut() && storage.GetUnCollectReward() == null && storage.GetUnCollectLeaderBoard() == null)
        {
            Debug.LogError("删除ActivityId = " + storage.ActivityId + "排行榜数据");
            ThemeDecorationModel.StorageThemeDecoration.Remove(storage.ActivityId);
            return true;
        }

        return false;
    }

    public static string GetSkinName(this StorageThemeDecoration weekStorage)
    {
        if (weekStorage == null)
            return "Default";
        if (weekStorage.SkinName == "")
            weekStorage.SkinName = "Default";
        return weekStorage.SkinName;
    }

    public const string ConnectKeyWord = "llllll";

    public static string GetAssetPathWithSkinName(this StorageThemeDecoration weekStorage, string assetBasePath)
    {
        // Debug.LogError("周期storage.SkinName="+weekStorage.GetSkinName());
        return assetBasePath.Replace("/ThemeDecoration/",
            "/ThemeDecoration" + ThemeDecorationUtils.ConnectKeyWord + weekStorage.GetSkinName() + "/");
    }

    public static string GetAtlasNameWithSkinName(this StorageThemeDecoration weekStorage)
    {
        return "ThemeDecoration" + ThemeDecorationUtils.ConnectKeyWord + weekStorage.GetSkinName() + "Atlas";
    }

    public static void AddSkinUIWindowInfo(this StorageThemeDecoration weekStorage)
    {
        // UIManager.Instance._WindowMetaPublic(weekStorage.GetAssetPathWithSkinName(UINameConst.UIPopupThemeDecorationEnd), UIWindowLayer.Normal, false);
        UIManager.Instance._WindowMetaPublic(
            weekStorage.GetAssetPathWithSkinName(UINameConst.UIPopupThemeDecorationPreview), UIWindowLayer.Normal,
            false);
        UIManager.Instance._WindowMetaPublic(
            weekStorage.GetAssetPathWithSkinName(UINameConst.UIPopupThemeDecorationStart), UIWindowLayer.Normal, false);
        UIManager.Instance._WindowMetaPublic(weekStorage.GetAssetPathWithSkinName(UINameConst.UIThemeDecorationShop),
            UIWindowLayer.Normal, false);
        // UIManager.Instance._WindowMetaPublic(weekStorage.GetAssetPathWithSkinName(UINameConst.UIThemeDecorationLeaderBoardMain), UIWindowLayer.Normal, false);
        UIManager.Instance._WindowMetaPublic(weekStorage.GetAssetPathWithSkinName(UINameConst.UIThemeDecorationHelp),
            UIWindowLayer.Normal, false);
        UIManager.Instance._WindowMetaPublic(
            weekStorage.GetAssetPathWithSkinName(UINameConst.UIPopupThemeDecorationShopBuy), UIWindowLayer.Normal,
            false);
        UIManager.Instance._WindowMetaPublic(
            weekStorage.GetAssetPathWithSkinName(UINameConst.UIPopupThemeDecorationBuyPreEnd), UIWindowLayer.Normal,
            false);
        UIManager.Instance._WindowMetaPublic(
            weekStorage.GetAssetPathWithSkinName(UINameConst.UIPopupThemeDecorationMultipleScore), UIWindowLayer.Normal,
            false);
        UIManager.Instance._WindowMetaPublic(
            weekStorage.GetAssetPathWithSkinName(UINameConst.UIThemeDecorationMapPreview), UIWindowLayer.Normal, false);
        if (weekStorage != null)
        {
            foreach (var leaderBoard in weekStorage.LeaderBoardStorageList)
            {
                leaderBoard.AddSkinUIWindowInfo();
            }
        }
    }

    public static string GetAuxItemAssetPath(this StorageThemeDecoration storage)
    {
        return storage.GetAssetPathWithSkinName("Prefabs/Activity/ThemeDecoration/Aux_ThemeDecoration");
    }

    public static string GetTaskItemAssetPath(this StorageThemeDecoration storage)
    {
        return storage.GetAssetPathWithSkinName("Prefabs/Activity/ThemeDecoration/TaskList_ThemeDecoration");
    }

    public static bool IsResExist(this StorageThemeDecoration weekStorage)
    {
        return ActivityManager.Instance.CheckResExist(weekStorage.ActivityResList) ||
               ActivityManager.Instance.IsActivityResourcesDownloaded(ThemeDecorationModel.Instance.ActivityId);
    }

    public static bool ShowEntrance(this StorageThemeDecoration storage)
    {
        if (!storage.IsResExist())
            return false;
        if (storage.IsTimeOut())
            return false;
        if (!storage.IsPreheat())
            return true;
        if (!storage.IsTotalTimeOut())
            return true;
        if (storage.CanBuyEnd())
            return true;
        return false;
    }


    public static bool HasCollectLevelCompleteReward(this StorageThemeDecoration storage,
        ThemeDecorationStoreLevelConfig storeLevelConfig)
    {
        return storage.CollectStoreCompleteLevel.Contains(storeLevelConfig.Id);
    }

    public static bool CanCollectLevelCompleteReward(this StorageThemeDecoration storage,
        ThemeDecorationStoreLevelConfig storeLevelConfig)
    {
        if (storage.HasCollectLevelCompleteReward(storeLevelConfig))
            return false;
        foreach (var storeItemId in storeLevelConfig.StoreItemList)
        {
            if (!storage.FinishStoreItemList.Contains(storeItemId))
                return false;
        }

        return true;
    }

    public static void CollectLevelCompleteReward(this StorageThemeDecoration storage,
        ThemeDecorationStoreLevelConfig storeLevelConfig, Action callback = null)
    {
        if (!storage.CanCollectLevelCompleteReward(storeLevelConfig))
            return;
        var rewardData = CommonUtils.FormatReward(storeLevelConfig.RewardId, storeLevelConfig.RewardNum);
        storage.CollectStoreCompleteLevel.Add(storeLevelConfig.Id);
        var reasonArgs = new GameBIManager.ItemChangeReasonArgs()
        {
            reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.ThemeDecorationGet
        };

        foreach (var resData in rewardData)
        {
            if (storage.UnCollectRewards.ContainsKey(resData.id))
            {
                storage.UnCollectRewards[resData.id] -= resData.count;
                if (storage.UnCollectRewards[resData.id] <= 0)
                {
                    storage.UnCollectRewards.Remove(resData.id);
                }
            }
        }

        CommonRewardManager.Instance.PopCommonReward(rewardData, CurrencyGroupManager.Instance.currencyController,
            true,
            reasonArgs, animEndCall: callback);
        EventDispatcher.Instance.SendEventImmediately(
            new EventThemeDecorationCollectStoreCompleteReward(storeLevelConfig));
    }
}