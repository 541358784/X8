using System;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.ChristmasBlindBox;
using DragonU3DSDK;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using SomeWhere;
using UnityEngine;
using Random = UnityEngine.Random;

public partial class ChristmasBlindBoxModel : ActivityEntityBase
{
    private static ChristmasBlindBoxModel _instance;
    public static ChristmasBlindBoxModel Instance => _instance ?? (_instance = new ChristmasBlindBoxModel());

    public override string Guid => "OPS_EVENT_TYPE_CHRISTMAS_BLIND_BOX";


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitAuto()
    {
        Instance.Init();
    }

    public StorageChristmasBlindBox Storage => StorageManager.Instance.GetStorage<StorageHome>().ChristmasBlindBox;

    public void InitStorage()
    {
        if (!IsInitFromServer())
            return;
        if (Storage.ActivityId != ActivityId)
        {
            Storage.Clear();
            Storage.ActivityId = ActivityId;
            Storage.IsStart = false;
            Storage.BuyTimes = 0;
        }
        Storage.StartTime = (long)StartTime;
        Storage.EndTime = (long)EndTime;
    }
    private static void InitTable<T>(Dictionary<int, T> config) where T : TableBase
    {
        if (config == null)
            return;

        List<T> tableData = ChristmasBlindBoxConfigManager.Instance.GetConfig<T>();
        if (tableData == null)
            return;

        config.Clear();
        foreach (T kv in tableData)
        {
            config.Add(kv.GetID(), kv);
        }
    }
    public ChristmasBlindBoxGlobalConfig GlobalConfig => ChristmasBlindBoxConfigManager.Instance.GetConfig<ChristmasBlindBoxGlobalConfig>()[0];
    public override void InitFromServerData(string activityId, string activityType, ulong startTime, ulong endTime,
        ulong rewardEndTime, bool manualEnd, string configJson, string activitySubType)
    {
        DebugUtil.LogError("1");
        ChristmasBlindBoxConfigManager.Instance.InitConfig(configJson);
        base.InitFromServerData(activityId, activityType, startTime, endTime, rewardEndTime, manualEnd, configJson,
            activitySubType);
        DebugUtil.Log($"InitConfig:{Guid}");
        InitStorage();
    }

    public bool IsUnlock => UnlockManager.IsOpen(UnlockManager.MergeUnlockType.BlindBox);

    public override bool IsOpened(bool hasLog = false)
    {
        return base.IsOpened(hasLog) && IsUnlock && BlindBoxModel.Instance.GetStorage(GlobalConfig.ThemeId).IsResReady(); //当前当前周的配置;
    }
    
    public bool IsPrivateOpened()
    {
        return IsOpened() &&!Storage.IsTimeOut() && Storage.BuyTimes < GlobalConfig.BuyLimit;
    }
    public override bool CanDownLoadRes()
    {
        return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.BlindBox);
    }

    public Transform GetCommonFlyTarget()
    {
        return KeepPetModel.Instance.GetCommonFlyTarget();
    }
    public static bool CanShowStart()
    {
        if (Instance.IsOpened() && 
            !Instance.Storage.IsStart && 
            (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Home ||
             SceneFsm.mInstance.GetCurrSceneType() == StatusType.BackHome) &&
            !GuideSubSystem.Instance.IsShowingGuide())
        {
            Instance.Storage.IsStart = true;
            // UIPopupChristmasBlindBoxStartController.Open(Instance.Storage);
            return true;
        }
        return false;
    }

    public void OnPurchase(TableShop cfg)
    {
        if (cfg.id != GlobalConfig.ShopId)
            return;
        Storage.BuyTimes++;
        var themeConfig = BlindBoxModel.Instance.ThemeConfigDic[GlobalConfig.ThemeId];
        var storage = BlindBoxModel.Instance.GetStorage(GlobalConfig.ThemeId);
        var itemList = new List<BlindBoxItemConfig>();
        var specialConfigs = themeConfig.GetSpecialItemConfigs();
        itemList.AddRange(specialConfigs);
        var normalItemConfig = themeConfig.GetNormalItemConfigs();
        while (itemList.Count < GlobalConfig.BoxCount)
        {
            var index = Random.Range(0, normalItemConfig.Count);
            itemList.Add(normalItemConfig[index]);
            normalItemConfig.RemoveAt(index);
        }

        foreach (var item in itemList)
        {
            var isNew = storage.CollectItem(item);
            if (isNew && !specialConfigs.Contains(item))
            {
                storage.CurCollectTimes = 0;
            }
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventBlindBoxOpen,item.Id.ToString());
        }
        EventDispatcher.Instance.SendEventImmediately(new EventChristmasBlindBoxBuy(itemList));
        if (UIPopupChristmasBlindBoxController.Instance)
            UIPopupChristmasBlindBoxController.Instance.AnimCloseWindow();
        UIChristmasBlindBoxOpenController.Open(itemList);
    }
}