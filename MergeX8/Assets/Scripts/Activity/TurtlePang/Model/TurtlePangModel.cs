using System;
using System.Collections.Generic;
using System.Linq;
using Activity.TurtlePang.View;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.TurtlePang;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Dynamic;
using Gameplay;
using UnityEngine;

public partial class TurtlePangModel : ActivityEntityBase
{
    private static TurtlePangModel _instance;
    public static TurtlePangModel Instance => _instance ?? (_instance = new TurtlePangModel());
    public override string Guid => "OPS_EVENT_TYPE_TURTLE_PANG";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitAuto()
    {
        Instance.Init();
    }

    public TurtlePangModel()
    {
        EventDispatcher.Instance.AddEventListener(EventEnum.BackLogin, InitEntranceAgain);
    }

    public bool IsUnlock => UnlockManager.IsOpen(UnlockManager.MergeUnlockType.TurtlePang);

    public bool IsOpenPrivate()
    {
        return IsUnlock && IsOpened();
    }

    public bool IsStart()
    {
        return IsOpenPrivate() && Storage.GetPreheatTime() == 0;
    }

    public override bool CanDownLoadRes()
    {
        return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.TurtlePang);
    }

    public TurtlePangGlobalConfig GlobalConfig =>
        TurtlePangConfigManager.Instance.GetConfig<TurtlePangGlobalConfig>()[0];

    public List<TurtlePangItemConfig> ItemConfig =>
        TurtlePangConfigManager.Instance.GetConfig<TurtlePangItemConfig>();

    public Dictionary<int, TurtlePangStoreItemConfig>
        StoreItemConfig = new Dictionary<int, TurtlePangStoreItemConfig>();

    public List<TurtlePangStoreLevelConfig> StoreLevelConfig =>
        TurtlePangConfigManager.Instance.GetConfig<TurtlePangStoreLevelConfig>();

    public List<TurtlePangTaskRewardConfig> TaskRewardConfig =>
        TurtlePangConfigManager.Instance.GetConfig<TurtlePangTaskRewardConfig>();

    public List<TurtlePangGiftBagConfig> GiftBagConfig =>
        TurtlePangConfigManager.Instance.GetConfig<TurtlePangGiftBagConfig>();

    public override void InitFromServerData(string activityId, string activityType, ulong startTime, ulong endTime,
        ulong rewardEndTime, bool manualEnd, string configJson, string activitySubType)
    {
        base.InitFromServerData(activityId, activityType, startTime, endTime, rewardEndTime, manualEnd, configJson,
            activitySubType);
        TurtlePangConfigManager.Instance.InitConfig(configJson);
        StoreItemConfig.Clear();
        foreach (var config in TurtlePangConfigManager.Instance.GetConfig<TurtlePangStoreItemConfig>())
        {
            StoreItemConfig.Add(config.Id, config);
        }

        DebugUtil.Log($"InitConfig:{Guid}");
        InitStorage();
    }

    public void InitEntranceAgain(BaseEvent e)
    {
        if (!IsInitFromServer())
            return;
        var turtlePang = 1;
        Debug.Log(turtlePang);
    }

    public long PreheatTimeOffset => IsSkipActivityPreheating() ? 0 : (GlobalConfig.PreheatTime * (long) XUtility.Hour);

    public void InitStorage()
    {
        if (!IsInitFromServer())
            return;
        if (Storage.ActiviryId != ActivityId)
        {
            Storage.Clear();
            Storage.ActiviryId = ActivityId;
        }

        Storage.StartTime = (long) StartTime;
        Storage.EndTime = (long) EndTime;
        Storage.PreheatTime = (long) StartTime + PreheatTimeOffset;
    }

    public StorageTurtlePang Storage => StorageManager.Instance.GetStorage<StorageHome>().TurtlePang;

    private static string CanShowUICoolTimeKey = "TurtlePang_CanShowUI";

    public bool CanShowUI()
    {
        if (!IsOpenPrivate())
            return false;
        if (Storage.GetPreheatTime() > 0)
            return false;
        if (!CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, CanShowUICoolTimeKey))
        {
            CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, CanShowUICoolTimeKey,
                CommonUtils.GetTimeStamp());
            UITurtlePangMainController.Open(Storage);
            return true;
        }

        return false;
    }

    private static string CanShowPreheatUICoolTimeKey = "TurtlePang_CanShowPreheatUI";

    public bool CanShowPreheatUI()
    {
        if (!IsOpenPrivate())
            return false;
        if (Storage.GetPreheatTime() <= 0)
            return false;
        if (!CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, CanShowPreheatUICoolTimeKey))
        {
            CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, CanShowPreheatUICoolTimeKey,
                CommonUtils.GetTimeStamp());
            UIPopupTurtlePangPreviewController.Open(Storage);
            return true;
        }

        return false;
    }

    public int GetPackageCount()
    {
        if (!IsOpenPrivate())
            return 0;
        if (Storage.GetPreheatTime() > 0)
            return 0;
        return Storage.PackageCount;
    }

    public void AddPackage(int count, string reason)
    {
        if (!IsOpenPrivate())
            return;
        if (Storage.GetPreheatTime() > 0)
            return;
        Storage.PackageCount += count;
        EventDispatcher.Instance.SendEventImmediately(new EventTurtlePangPackageCountChange(count));
    }

    public int GetScore()
    {
        if (!IsOpenPrivate())
            return 0;
        if (Storage.GetPreheatTime() > 0)
            return 0;
        return Storage.Score;
    }

    public void AddScore(int count, string reason)
    {
        if (!IsOpenPrivate())
            return;
        if (Storage.GetPreheatTime() > 0)
            return;
        Storage.Score += count;
        EventDispatcher.Instance.SendEventImmediately(new EventTurtlePangScoreChange(count));
        GameBIManager.Instance.SendGameEvent(
            BiEventAdventureIslandMerge.Types.GameEventType.GameEventTurtlePangRadishChange,
            count.ToString(), Storage.Score.ToString(), reason);
    }

    public Transform GetCommonFlyTarget()
    {
        if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
        {
            var entrance = DynamicEntryManager.Instance.GetDynamicEntry<DynamicEntry_Game_TurtlePang>();
            if (entrance)
                return entrance.transform;
            else
                return MergeMainController.Instance.rewardBtnTrans;
        }
        else
        {
            var auxItem = DynamicEntryManager.Instance.GetDynamicEntry<DynamicEntry_Home_TurtlePang>();
            if (auxItem != null && auxItem.gameObject.activeInHierarchy)
                return auxItem.transform;
            else
                return UIHomeMainController.mainController.MainPlayTransform;
        }
    }

    public int GetTaskValue(StorageTaskItem taskItem, bool isMul)
    {
        if (!IsInitFromServer())
            return 0;
        int tempPrice = 0;
        for (var i = 0; i < taskItem.RewardTypes.Count; i++)
        {
            if (taskItem.RewardTypes[i] == (int) UserData.ResourceId.Coin ||
                taskItem.RewardTypes[i] == (int) UserData.ResourceId.RareDecoCoin)
            {
                if (taskItem.RewardNums.Count > i)
                    tempPrice = taskItem.RewardNums[i];

                break;
            }
        }

        if (tempPrice == 0)
        {
            foreach (var itemId in taskItem.ItemIds)
            {
                tempPrice += OrderConfigManager.Instance.GetItemPrice(itemId);
            }
        }

        var value = 0;
        var configs = TaskRewardConfig;
        if (configs != null && configs.Count > 0)
        {
            foreach (var config in configs)
            {
                if (tempPrice <= config.Max_value)
                {
                    value = config.Output;
                    break;
                }
            }
        }

        return value;
    }

    public void AddBagValue(int itemId, int count)
    {
        Storage.Bag.TryAdd(itemId, 0);
        Storage.Bag[itemId] += count;
        EventDispatcher.Instance.SendEventImmediately(new EventTurtlePangBagItemChange());
    }

    public void AddGameBagValue(int itemId, int count)
    {
        Storage.BagGame.TryAdd(itemId, 0);
        Storage.BagGame[itemId] += count;
    }

    public void OnPurchase(TableShop shopConfig)
    {
        var cfg = GiftBagConfig.Find(a => a.ShopId == shopConfig.id);
        if (cfg == null)
            return;
        var rewards = CommonUtils.FormatReward(cfg.RewardId, cfg.RewardNum);
        var reason = new GameBIManager.ItemChangeReasonArgs()
        {
            reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.Iap
        };
        EventDispatcher.Instance.DispatchEvent(EventEnum.PURCHASE_SUCCESS_REWARD, rewards);

        CommonRewardManager.Instance.PopCommonReward(rewards, CurrencyGroupManager.Instance.currencyController, true,
            reason, () =>
            {
                // if (UITurtlePangGiftBagController.Instance)
                //     UITurtlePangGiftBagController.Instance.AnimCloseWindow();
                if (UITurtlePangMainController.Instance)
                    UITurtlePangMainController.Instance.UpdatePackageNumText();
            });
    }


    public void FakePlay(int times, List<int> packageTypeConfig)
    {
        var resultDic = new Dictionary<int, int>();
        var totalTurtleCount = 0;
        var Storage = new StorageTurtlePang();
        for (var timeCount = 0; timeCount < times; timeCount++)
        {
            Storage.Clear();
            Storage.LuckyColor = 1;
            Storage.BasePackageCount = packageTypeConfig[0];
            Storage.ExtraPackageCount = packageTypeConfig[1];
            while (Storage.BasePackageCount > 0 || Storage.ExtraPackageCount > 0)
            {
                //放包
                var gridCount = GlobalConfig.BoardSize * GlobalConfig.BoardSize;
                for (var i = 0; i < gridCount; i++)
                {
                    if (!Storage.BoardState.TryGetValue(i, out var value))
                    {
                        if (Storage.BasePackageCount > 0)
                        {
                            Storage.BoardState.Add(i, 0);
                            Storage.BasePackageCount--;
                        }
                        else if (Storage.ExtraPackageCount > 0)
                        {
                            Storage.BoardState.Add(i, 0);
                            Storage.ExtraPackageCount--;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                var newIndexList = new List<int>();
                //拆包
                for (var i = 0; i < gridCount; i++)
                {
                    if (Storage.BoardState.TryGetValue(i, out var value) && value == 0)
                    {
                        var randomList = new List<int>();
                        foreach (var itemConfig in ItemConfig)
                        {
                            randomList.Add(itemConfig.Weight);
                        }

                        var randomIdx = Utils.RandomByWeight(randomList);
                        var randomColor = ItemConfig[randomIdx];
                        Storage.BoardState[i] = randomColor.Id;
                        newIndexList.Add(i);
                    }
                }

                //幸运色结算
                var luckySendCount = 0;
                var luckyGridList = new List<int>();
                for (var i = 0; i < newIndexList.Count; i++)
                {
                    var index = newIndexList[i];
                    if (Storage.BoardState.TryGetValue(index, out var value) && value == Storage.LuckyColor)
                    {
                        luckyGridList.Add(index);
                        luckySendCount += GlobalConfig.LuckyColorSendCount;
                        Storage.ExtraPackageCount += GlobalConfig.LuckyColorSendCount;
                    }
                }

                //bingo结算
                var triggerList = new List<List<int>>();
                //横线
                for (var i = 0; i < GlobalConfig.BoardSize; i++)
                {
                    int color = 0;
                    var trigger = true;
                    for (var i1 = 0; i1 < GlobalConfig.BoardSize; i1++)
                    {
                        var index = i * GlobalConfig.BoardSize + i1;
                        if (!Storage.BoardState.TryGetValue(index, out var value) || value == 0)
                        {
                            trigger = false;
                            break;
                        }
                        else
                        {
                            if (color == 0)
                                color = value;
                            else if (color != value)
                            {
                                trigger = false;
                                break;
                            }
                        }
                    }

                    if (trigger)
                    {
                        var indexList = new List<int>();
                        for (var i1 = 0; i1 < GlobalConfig.BoardSize; i1++)
                        {
                            var index = i * GlobalConfig.BoardSize + i1;
                            indexList.Add(index);
                            Storage.BoardState.Remove(index);
                        }

                        triggerList.Add(indexList);
                        Storage.BagGame.TryAdd(color, 0);
                        Storage.BagGame[color] += indexList.Count;
                        Storage.ExtraPackageCount += GlobalConfig.DrawLineSendCount;
                    }
                }

                //竖线
                for (var i1 = 0; i1 < GlobalConfig.BoardSize; i1++)
                {
                    int color = 0;
                    var trigger = true;
                    for (var i = 0; i < GlobalConfig.BoardSize; i++)
                    {
                        var index = i * GlobalConfig.BoardSize + i1;
                        if (!Storage.BoardState.TryGetValue(index, out var value) || value == 0)
                        {
                            trigger = false;
                            break;
                        }
                        else
                        {
                            if (color == 0)
                                color = value;
                            else if (color != value)
                            {
                                trigger = false;
                                break;
                            }
                        }
                    }

                    if (trigger)
                    {
                        var indexList = new List<int>();
                        for (var i = 0; i < GlobalConfig.BoardSize; i++)
                        {
                            var index = i * GlobalConfig.BoardSize + i1;
                            indexList.Add(index);
                            Storage.BoardState.Remove(index);
                        }

                        triggerList.Add(indexList);
                        Storage.BagGame.TryAdd(color, 0);
                        Storage.BagGame[color] += indexList.Count;
                        Storage.ExtraPackageCount += GlobalConfig.DrawLineSendCount;
                    }
                }

                //右斜线
                {
                    int color = 0;
                    var trigger = true;
                    for (var i = 0; i < GlobalConfig.BoardSize; i++)
                    {
                        var i1 = i;
                        var index = i * GlobalConfig.BoardSize + i1;
                        if (!Storage.BoardState.TryGetValue(index, out var value) || value == 0)
                        {
                            trigger = false;
                            break;
                        }
                        else
                        {
                            if (color == 0)
                                color = value;
                            else if (color != value)
                            {
                                trigger = false;
                                break;
                            }
                        }
                    }

                    if (trigger)
                    {
                        var indexList = new List<int>();
                        for (var i = 0; i < GlobalConfig.BoardSize; i++)
                        {
                            var i1 = i;
                            var index = i * GlobalConfig.BoardSize + i1;
                            indexList.Add(index);
                            Storage.BoardState.Remove(index);
                        }

                        triggerList.Add(indexList);
                        Storage.BagGame.TryAdd(color, 0);
                        Storage.BagGame[color] += indexList.Count;
                        Storage.ExtraPackageCount += GlobalConfig.DrawLineSendCount;
                    }
                }
                //左斜线
                {
                    int color = 0;
                    var trigger = true;
                    for (var i = 0; i < GlobalConfig.BoardSize; i++)
                    {
                        var i1 = GlobalConfig.BoardSize - 1 - i;
                        var index = i * GlobalConfig.BoardSize + i1;
                        if (!Storage.BoardState.TryGetValue(index, out var value) || value == 0)
                        {
                            trigger = false;
                            break;
                        }
                        else
                        {
                            if (color == 0)
                                color = value;
                            else if (color != value)
                            {
                                trigger = false;
                                break;
                            }
                        }
                    }

                    if (trigger)
                    {
                        var indexList = new List<int>();
                        for (var i = 0; i < GlobalConfig.BoardSize; i++)
                        {
                            var i1 = GlobalConfig.BoardSize - 1 - i;
                            var index = i * GlobalConfig.BoardSize + i1;
                            indexList.Add(index);
                            Storage.BoardState.Remove(index);
                        }

                        triggerList.Add(indexList);
                        Storage.BagGame.TryAdd(color, 0);
                        Storage.BagGame[color] += indexList.Count;
                        Storage.ExtraPackageCount += GlobalConfig.DrawLineSendCount;
                    }
                }
                //对对碰
                var triggerPangList = new List<List<int>>();
                for (var i = 0; i < gridCount; i++)
                {
                    if (!Storage.BoardState.TryGetValue(i, out var value1) || value1 == 0)
                        continue;
                    for (var i1 = i + 1; i1 < gridCount; i1++)
                    {
                        if (!Storage.BoardState.TryGetValue(i1, out var value2) || value2 == 0)
                            continue;
                        if (value1 == value2)
                        {
                            Storage.BoardState.Remove(i);
                            Storage.BoardState.Remove(i1);
                            Storage.BagGame.TryAdd(value1, 0);
                            Storage.BagGame[value1] += 2;
                            Storage.ExtraPackageCount += GlobalConfig.SameColorSendCount;
                            triggerPangList.Add(new List<int>() {i, i1});
                            break;
                        }
                    }
                }

                //清场
                if (Storage.BoardState.Count == 0)
                {
                    Storage.ExtraPackageCount += GlobalConfig.CleanBoardSendCount;
                }

                //十三不靠
                if (Storage.BoardState.Count == gridCount)
                {
                    for (var i = 0; i < gridCount; i++)
                    {
                        var color = Storage.BoardState[i];
                        Storage.BoardState.Remove(i);
                        Storage.BagGame.TryAdd(color, 0);
                        Storage.BagGame[color] += 1;
                    }

                    Storage.ExtraPackageCount += GlobalConfig.FillBoardSendCount;
                }
                //GameOver
                if (Storage.BasePackageCount == 0 && Storage.ExtraPackageCount == 0)
                {
                    var keyList = Storage.BoardState.Keys.ToList();
                    foreach (var key in keyList)
                    {
                        var color = Storage.BoardState[key];
                        Storage.BoardState.Remove(key);
                        Storage.BagGame.TryAdd(color, 0);
                        Storage.BagGame[color] += 1;
                    }
                }
            }
            var turtleCount = 0;
            foreach (var pair in Storage.BagGame)
            {
                turtleCount += pair.Value;
            }
            resultDic.TryAdd(turtleCount, 0);
            resultDic[turtleCount]++;
            totalTurtleCount += turtleCount;
        }
        Debug.LogError("每包平均产出:"+((float)totalTurtleCount/times/packageTypeConfig[0]).ToString());
        var log = "";
        var resultKeyList = resultDic.Keys.ToList();
        resultKeyList.Sort();
        for (var i = 0; i < resultKeyList.Count; i++)
        {
            var key = resultKeyList[i];
            var value = resultDic[key];
            log += "\n产出"+key+",次数"+value+",占比"+((float)value/times).ToString();
        }
        Debug.LogError(log);
    }


    public bool ShowAuxItem()
    {
        if (Storage == null)
            return false;

        return TurtlePangUtils.ShowAuxItem(Storage);
    }

    public bool ShowTaskEntrance()
    {
        if (Storage == null)
            return false;

        return Storage.ShowTaskEntrance();
    }
    
    public bool TurtlePangDebugClear = false;
    public bool TurtlePangDebugFull = false;
}