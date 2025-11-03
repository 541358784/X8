using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Deco.World;
using Decoration;
using DragonPlus;
using DragonPlus.Config.KeepPetTurkey;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;
using Utils = DragonU3DSDK.Utils;

public enum KeepPetTurkeyStoreItemType
{
    MergeItem = 1,
    BuildItem = 2,
}

public static class KeepPetTurkeyUtils
{
    public static KeepPetTurkeyModel Model => KeepPetTurkeyModel.Instance;
    public static void AddScore(this StorageKeepPetTurkey storage, int addCount, string reason, bool needWait = false)
    {
        storage.Score += addCount;
        EventDispatcher.Instance.SendEventImmediately(new EventKeepPetTurkeyScoreChange(addCount, needWait));
        GameBIManager.Instance.SendGameEvent(
            BiEventAdventureIslandMerge.Types.GameEventType.GameEventKeepPetTurkeyRadishChange,
            addCount.ToString(), storage.Score.ToString(), reason);
    }
    
    public static bool BuyStoreItem(this StorageKeepPetTurkey storage, KeepPetTurkeyStoreItemConfig storeItemConfig)
    {
        if (storage.Score < storeItemConfig.Price)
            return false;
        if (!KeepPetTurkeyModel.Instance.ReduceScore(storeItemConfig.Price, "BuyItem"))
            return false;
        GameBIManager.Instance.SendGameEvent(
            BiEventAdventureIslandMerge.Types.GameEventType.GameEventKeepPetTurkeyRadishExchange,
            storeItemConfig.Id.ToString(), storage.GetCurStoreLevel().Id.ToString(), storage.Score.ToString());
        storage.FinishStoreItemList.Add(storeItemConfig.Id);
        if ((KeepPetTurkeyStoreItemType) storeItemConfig.Type == KeepPetTurkeyStoreItemType.BuildItem)
        {
            KeepPetModel.Instance.GetBuilding(storeItemConfig.RewardId[0]);
            var mainUI =
                UIManager.Instance.GetOpenedUIByPath<UIKeepPetMainController>(UINameConst.UIKeepPetMain);
            var shopUI =
                UIManager.Instance
                    .GetOpenedUIByPath<UIKeepPetTurkeyShopController>(UINameConst.UIKeepPetTurkeyShop);
            if (mainUI && shopUI)
            {
                shopUI.CloseWindowWithinUIMgr(true);
                mainUI.PlayAllBuildingAppearAnimation();
                XUtility.WaitSeconds(1f, () =>
                {
                    UIKeepPetTurkeyShopController.Open(storage);
                });
            }
        }
        else
        {
            var rewardData = CommonUtils.FormatReward(storeItemConfig.RewardId, storeItemConfig.RewardNum);
            var reasonArgs = new GameBIManager.ItemChangeReasonArgs()
            {
                reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.KeepPetTurkeyGet
            };
            CommonRewardManager.Instance.PopCommonReward(rewardData, CurrencyGroupManager.Instance.currencyController,
                true,
                reasonArgs);
            foreach (var reward in rewardData)
            {
                if (!UserData.Instance.IsResource(reward.id))
                {
                    GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                    {
                        MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonKeepPetTurkeyGet,
                        itemAId =reward.id,
                        isChange = true,
                    });
                }
            }
        }

        EventDispatcher.Instance.SendEventImmediately(new EventKeepPetTurkeyBuyStoreItem(storeItemConfig));
        return true;
    }

    public static KeepPetTurkeyStoreLevelConfig GetCurStoreLevel(this StorageKeepPetTurkey storage)
    {
        if (!KeepPetTurkeyModel.Instance.IsInitFromServer())
        {
            return null;
        }

        var levelConfig = KeepPetTurkeyModel.Instance.StoreLevelConfig;
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

    public static bool IsTimeOut(this StorageKeepPetTurkey storageWeek)
    {
        return storageWeek.GetLeftTime() <= 0;
    }

    public static long GetLeftTime(this StorageKeepPetTurkey storageWeek)
    {
        return Math.Max(storageWeek.EndTime - (long) APIManager.Instance.GetServerTime(), 0);
    }

    public static void SetLeftTime(this StorageKeepPetTurkey storageWeek, long leftTime)
    {
        var endTime = (long) APIManager.Instance.GetServerTime() + leftTime;
        storageWeek.EndTime = endTime;
    }

    public static string GetLeftTimeText(this StorageKeepPetTurkey storageWeek)
    {
        return CommonUtils.FormatLongToTimeStr(storageWeek.GetLeftTime());
    }

    public static int GetTurkeyScore(this KeepPetDailyTaskConfig dailyTaskConfig)
    {
        if (!KeepPetTurkeyModel.Instance.IsInitFromServer())
        {
            return 0;
        }
        return KeepPetTurkeyModel.Instance.TaskScoreConfig[dailyTaskConfig.Id].TurkeyScore;
    }
}