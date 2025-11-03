using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Deco.World;
using Decoration;
using DragonPlus;
using DragonPlus.Config.Zuma;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Farm.Model;
using Gameplay;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;
using Utils = DragonU3DSDK.Utils;

public enum ZumaStoreItemType
{
    MergeItem = 1,
    BuildItem = 2,
}

public static class ZumaUtils
{
    public static ZumaModel Model => ZumaModel.Instance;
    public const ulong Offset = 0 * XUtility.Hour;
    public static int CurDay => (int) ((APIManager.Instance.GetServerTime() - Offset) / XUtility.DayTime);

    public static ulong CurDayLeftTime =>
        XUtility.DayTime - ((APIManager.Instance.GetServerTime() - Offset) % XUtility.DayTime);

    public static string CurDayLeftTimeString => CommonUtils.FormatLongToTimeStr((long) CurDayLeftTime);

    public static void AddScore(this StorageZuma storage, int addCount, string reason, bool needWait = false)
    {
        storage.LevelScore += addCount;
        storage.Score += addCount;
        storage.TotalScore += addCount;
        if (Model.GetLevel(storage.LevelId).IsLoopLevel)
        {
            ZumaLeaderBoardModel.Instance.GetLeaderBoardStorage(storage.ActivityId)?.CollectStar(addCount);
        }
        EventDispatcher.Instance.SendEventImmediately(new EventZumaScoreChange(addCount, needWait));

        GameBIManager.Instance.SendGameEvent(
            BiEventAdventureIslandMerge.Types.GameEventType.GameEventZumaRadishChange,
            addCount.ToString(), storage.Score.ToString(), reason);
    }

    

    public static bool BuyStoreItem(this StorageZuma storage, ZumaStoreItemConfig storeItemConfig)
    {
        if (storage.Score < storeItemConfig.Price)
            return false;
        if (!ZumaModel.Instance.ReduceScore(storeItemConfig.Price, "BuyItem"))
            return false;
        GameBIManager.Instance.SendGameEvent(
            BiEventAdventureIslandMerge.Types.GameEventType.GameEventZumaRadishExchange,
            storeItemConfig.Id.ToString(), storage.GetCurStoreLevel().Id.ToString(), storage.Score.ToString());
        storage.FinishStoreItemList.Add(storeItemConfig.Id);
        if ((ZumaStoreItemType) storeItemConfig.Type == ZumaStoreItemType.BuildItem)
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
                            .GetOpenedUIByPath<UIZumaShopController>(UINameConst.UIZumaShop);
                    if (shopUI)
                    {
                        hasShopUI = true;
                        shopUI.CloseWindowWithinUIMgr(true);
                    }

                    var mainUI =
                        UIManager.Instance
                            .GetOpenedUIByPath<UIZumaMainController>(UINameConst.UIZumaMain);
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
                        ZumaModel.CanShowMainPopup();
                    }

                    if (hasShopUI)
                    {
                        UIZumaShopController.Open(storage);
                    }
                };

                if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Game || FarmModel.Instance.IsFarmModel())
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
                reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.ZumaGet
            };
            CommonRewardManager.Instance.PopCommonReward(rewardData, CurrencyGroupManager.Instance.currencyController,
                true,
                reasonArgs);
        }

        EventDispatcher.Instance.SendEventImmediately(new EventZumaBuyStoreItem(storeItemConfig));
        return true;
    }

    public static ZumaStoreLevelConfig GetCurStoreLevel(this StorageZuma storage)
    {
        if (!ZumaModel.Instance.IsInitFromServer())
        {
            return null;
        }

        var levelConfig = ZumaModel.Instance.StoreLevelConfig;
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

    public static long GetPreheatLeftTime(this StorageZuma storage)
    {
        var heatTime = storage.PreheatCompleteTime - (long) APIManager.Instance.GetServerTime();
        heatTime = Math.Max(heatTime, 0);
        return heatTime;
    }

    public static void SetPreheatLeftTime(this StorageZuma storageWeek, long leftTime)
    {
        storageWeek.PreheatCompleteTime = (long) APIManager.Instance.GetServerTime() + leftTime;
    }

    public static string GetPreheatLeftTimeText(this StorageZuma storage)
    {
        return CommonUtils.FormatLongToTimeStr(storage.GetPreheatLeftTime());
    }

    public static bool IsTimeOut(this StorageZuma storageWeek)
    {
        return storageWeek.GetLeftTime() <= 0;
    }

    public static long GetLeftTime(this StorageZuma storageWeek)
    {
        return Math.Max(storageWeek.EndTime - (long) APIManager.Instance.GetServerTime(), 0);
    }

    public static void SetLeftTime(this StorageZuma storageWeek, long leftTime)
    {
        var endTime = (long) APIManager.Instance.GetServerTime() + leftTime;
        storageWeek.EndTime = endTime;
    }

    public static string GetLeftTimeText(this StorageZuma storageWeek)
    {
        return CommonUtils.FormatLongToTimeStr(storageWeek.GetLeftTime());
    }

    public static void CompletedStorageActivity(this StorageZuma storageWeek)
    {
        if (storageWeek.TryRelease())
            ZumaModel.Instance.CreateStorage();
    }

    public static bool TryRelease(this StorageZuma storage)
    {
        if (storage.IsTimeOut())
        {
            Debug.LogError("删除ActivityId = " + storage.ActivityId + "排行榜数据");
            ZumaModel.StorageZuma.Remove(storage.ActivityId);
            return true;
        }

        return false;
    }
    public static bool ShowAuxItem(this StorageZuma storage)
    {
        if (!ZumaModel.Instance.IsOpened())
            return false;
        if (storage.IsTimeOut())
            return false;
        return true;
    }
    public static bool ShowTaskEntrance(this StorageZuma storage)
    {
        if (!ZumaModel.Instance.IsOpened())
            return false;
        if (ZumaModel.Instance.CurStorageZumaWeek.GetPreheatLeftTime() > 0)
            return false;
        if (storage.IsTimeOut())
            return false;
        return true;
    }

    public static string GetAuxItemAssetPath(this StorageZuma storage)
    {
        return "Prefabs/Activity/Zuma/Aux_Zuma";
    }

    public static string GetTaskItemAssetPath(this StorageZuma storage)
    {
        return "Prefabs/Activity/Zuma/TaskList_Zuma";
    }

    public static List<ZumaBallColor> RandomColorList = new List<ZumaBallColor>()
    {
        ZumaBallColor.Red,
        ZumaBallColor.Green,
        ZumaBallColor.Blue,
        ZumaBallColor.Yellow,
        ZumaBallColor.Purple
    };
    public static void StartLevel(this StorageZuma storage, ZumaLevelConfig level)
    {
        storage.LevelId = level.Id;
        storage.LevelScore = 0;
        var showColorList = new List<ZumaBallColor>();
        storage.ColorTransformTable.Clear();
        var colorList = storage.GetLeftColors();
        for (var i = 0; i < colorList.Count; i++)
        {
            showColorList.Add(RandomColorList[i]);
        }
        for (var i = 0; i < colorList.Count; i++)
        {
            var showColor = showColorList[Random.Range(0, showColorList.Count)];
            showColorList.Remove(showColor);
            storage.ColorTransformTable.Add(colorList[i],(int)showColor);
        }
        storage.CurBallColor = colorList[Random.Range(0, colorList.Count)];
        colorList.Remove(storage.CurBallColor);
        storage.NextBallColor = colorList[Random.Range(0, colorList.Count)];
        if (level.IsLoopLevel)
        {
            ZumaLeaderBoardModel.Instance.GetLeaderBoardStorage(storage.ActivityId)?.CollectStar(1);
        }
    }

    public static List<int> GetLeftColors(this StorageZuma storage)
    {
        var level = Model.LevelConfigs[storage.LevelId];
        var colorList = new List<int>();
        for (var i = 0; i < level.ColorWeight.Count; i++)
        {
            colorList.Add(i);
        }
        return colorList;
    }

    public static ZumaBallColor GetColor(this StorageZuma storage, int configColor)
    {
        if (configColor < 0)
            return ZumaBallColor.Grey;
        if (storage.ColorTransformTable.TryGetValue(configColor,out var color))
            return (ZumaBallColor)color;
        Debug.LogError("未找到对应颜色");
        return ZumaBallColor.Red;
    }

    public static int GetConfigColor(this StorageZuma storage, ZumaBallColor color)
    {
        foreach (var pair in storage.ColorTransformTable)
        {
            if (pair.Value == (int)color)
                return pair.Key;
        }
        Debug.LogError("未找到对应反射颜色");
        return 1;
    }

    public static ZumaLevelConfig GetNextLevel(this StorageZuma storage)
    {
        if (storage.CompleteTimes >= Model.NormalLevelConfig.Count)//进随机关卡
        {
            return Model.LoopLevelConfig;
        }
        else
        {
            return Model.NormalLevelConfig[storage.CompleteTimes];
        }
    }
}