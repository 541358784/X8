
using System.Collections.Generic;
using Activity.KapiTile.View;
using DragonPlus;
using DragonPlus.Config.KapiTile;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Dynamic;
using Gameplay;
using TMatch;
using UnityEngine;

public partial class KapiTileModel : ActivityEntityBase
{
    public bool ShowEntrance()
    {
        return IsPrivateOpened();
    }
    private static KapiTileModel _instance;
    public static KapiTileModel Instance => _instance ?? (_instance = new KapiTileModel());

    public override string Guid => "OPS_EVENT_TYPE_KAPI_TILE";


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitAuto()
    {
        Instance.Init();
    }
    
    public StorageKapiTile Storage => StorageManager.Instance.GetStorage<StorageHome>().KapiTile;

    public void InitStorage()
    {
        if (!IsInitFromServer())
            return;
        if (Storage.ActivityId != ActivityId)
        {
            Storage.Clear();
            Storage.ActivityId = ActivityId;
            Storage.IsStart = false;
        }
        Storage.StartTime = (long)StartTime;
        Storage.PreheatCompleteTime = (long) StartTime + GlobalConfig.PreheatTime * (long)XUtility.Hour;
        Storage.EndTime = (long)EndTime;
    }

    private static void InitTable<T>(Dictionary<int, T> config) where T : TableBase
    {
        if (config == null)
            return;

        List<T> tableData = KapiTileConfigManager.Instance.GetConfig<T>();
        if (tableData == null)
            return;

        config.Clear();
        foreach (T kv in tableData)
        {
            config.Add(kv.GetID(), kv);
        }
    }

    public KapiTileGlobalConfig GlobalConfig => KapiTileConfigManager.Instance.GetConfig<KapiTileGlobalConfig>()[0];
    public List<KapiTileLevelConfig> LevelConfig => KapiTileConfigManager.Instance.GetConfig<KapiTileLevelConfig>();
    public List<KapiTileGiftBagConfig> GiftBagConfig => KapiTileConfigManager.Instance.GetConfig<KapiTileGiftBagConfig>();
    public override void InitFromServerData(string activityId, string activityType, ulong startTime, ulong endTime,
        ulong rewardEndTime, bool manualEnd, string configJson, string activitySubType)
    {
        DebugUtil.LogError("1");
        KapiTileConfigManager.Instance.InitConfig(configJson);
        base.InitFromServerData(activityId, activityType, startTime, endTime, rewardEndTime, manualEnd, configJson,
            activitySubType);
        DebugUtil.Log($"InitConfig:{Guid}");
        InitStorage();
    }
    public KapiTileModel()
    {
        TMatch.Timer.Register(1, UpdateLife, null, true);
    }
    public bool IsUnlock => UnlockManager.IsOpen(UnlockManager.MergeUnlockType.KapiTile);

    public override bool IsOpened(bool hasLog = false)
    {
        return base.IsOpened(hasLog) && IsUnlock && PayLevelModel.Instance.GetCurPayLevelConfig().KaipiOpenFlag; //当前当前周的配置;
    }
    public bool IsPrivateOpened()
    {
        return IsOpened() &&!Storage.IsTimeOut() && !IsFinished();
    }

    public bool IsFinished()
    {
        if (!IsInitFromServer())
            return true;
        return Storage.BigLevel == LevelConfig.Count-1 && Storage.SmallLevel >= LevelConfig[Storage.BigLevel].SmallLevels.Count;
    }
    public override bool CanDownLoadRes()
    {
        return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.KapiTile);
    }

    public Transform GetCommonFlyTarget()
    {
        if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
        {
            var entrance = DynamicEntryManager.Instance.GetDynamicEntry<DynamicEntry_Game_KapiTile>();
            if (entrance)
                return entrance.transform;
            else
                return MergeMainController.Instance.rewardBtnTrans;
        }
        else
        {
            var auxItem = DynamicEntryManager.Instance.GetDynamicEntry<DynamicEntry_Home_KapiTile>();
            if (auxItem != null && auxItem.gameObject.activeInHierarchy)
                return auxItem.transform;
            else
                return UIHomeMainController.mainController.MainPlayTransform;
        }
    }

    public int GetLife()
    {
        return Storage.Life;
    }
    public void AddLife(int count,string source)
    {
        var oldValue = Storage.Life;
        Storage.Life += count;
        var newValue = Storage.Life;
        if (oldValue >= GlobalConfig.MaxLife && newValue < GlobalConfig.MaxLife)
        {
            Storage.LifeUpdateTime = (long)APIManager.Instance.GetServerTime();
        }
        EventDispatcher.Instance.SendEventImmediately(new EventKapiTileLifeChange(oldValue,newValue));
        GameBIManager.Instance.SendGameEvent(
            BiEventAdventureIslandMerge.Types.GameEventType.GameEventKapiTileLifeChange,
            count.ToString(), newValue.ToString(), source);
    }
    public void UpdateLife()
    {
        if (!IsInitFromServer())
        {
            return;
        }
        if (Storage.Life >= GlobalConfig.MaxLife)
            return;
        var lifeAddUnitTime = GlobalConfig.LifeRecoverTime * (long)XUtility.Min;
        var passTime = (long)APIManager.Instance.GetServerTime() - Storage.LifeUpdateTime;
        var addLife = passTime / lifeAddUnitTime;
        Storage.LifeUpdateTime += addLife * lifeAddUnitTime;
        if (Storage.Life + addLife >= GlobalConfig.MaxLife)
            addLife = GlobalConfig.MaxLife - Storage.Life;
        if (addLife !=0 )
            AddLife((int)addLife, "recover");
    }
    public int GetRebornCount()
    {
        return Storage.RebornCount;
    }

    public void AddRebornItem(int getCount,string reason)
    {
        var oldValue = Storage.RebornCount;
        Storage.RebornCount += getCount;
        var newValue = Storage.RebornCount;
        EventDispatcher.Instance.SendEventImmediately(new EventKapiTileRebornCountChange(oldValue,newValue));
        
        GameBIManager.Instance.SendGameEvent(
            BiEventAdventureIslandMerge.Types.GameEventType.GameEventKapiTileRebornChange,
            getCount.ToString(), newValue.ToString(), reason);
    }
    
    
    
    public void OnPurchase(TableShop shopConfig)
    {
        if (!IsInitFromServer())
            return;
        var rewards = new List<ResData>();
        if (GlobalConfig.TotalShopId == shopConfig.id)
        {
            foreach (var giftBag in GiftBagConfig)
            {
                rewards.AddRange(CommonUtils.FormatReward(giftBag.Contain, giftBag.ContainCount));
            }
        }
        else
        {
            var product = GiftBagConfig.Find(a => a.ShopId == shopConfig.id);
            if (product == null)
                return;
            rewards.AddRange(CommonUtils.FormatReward(product.Contain, product.ContainCount));   
        }
        var reason = new GameBIManager.ItemChangeReasonArgs()
        {
            reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.Iap
        };
        EventDispatcher.Instance.DispatchEvent(EventEnum.PURCHASE_SUCCESS_REWARD, rewards);

        CommonRewardManager.Instance.PopCommonReward(rewards, CurrencyGroupManager.Instance.GetCurrencyUseController(),
            true,
            reason, animEndCall: () =>
            {
                EventDispatcher.Instance.DispatchEventImmediately(EventEnum.UserDataUpdate);
            });
    }

    public KapiTileLevelConfig GetLevelConfig(int level)
    {
        if (level < 0 || level >= LevelConfig.Count)
            return null;
        return LevelConfig[level];
    }

    public void DealStartGame()
    {
        Storage.PlayingSmallLevel = Storage.SmallLevel;
        Storage.SmallLevel = 0;
        AddLife(-1,"ReduceOnStart");
    }
    public void DealWin()
    {
        GameBIManager.Instance.SendGameEvent(
            BiEventAdventureIslandMerge.Types.GameEventType.GameEventKapiTileLevelEnd,
            Storage.BigLevel.ToString(),
            Storage.PlayingSmallLevel.ToString(),
            "win",
            TileMatchRoot.Instance.PropUseState.ToBIString());
        AddLife(1,"ReturnOnWin");
        Storage.SmallLevel = Storage.PlayingSmallLevel+1;
        var curLevel = GetLevelConfig(Storage.BigLevel);
        if (Storage.SmallLevel >= curLevel.SmallLevels.Count)//通关
        {
            var rewards = CommonUtils.FormatReward(curLevel.RewardId, curLevel.RewardNum);
            var reason = new GameBIManager.ItemChangeReasonArgs()
            {
                reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.KapiTile
            };
            UserData.Instance.AddRes(rewards,reason);
            foreach (var reward in rewards)
            {
                if (!UserData.Instance.IsResource(reward.id) && !TMatchModel.Instance.IsTMatchResId(reward.id))
                {
                    GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                    {
                        MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonKapiTileGet,
                        isChange = false,
                        itemAId = reward.id
                    });
                }
            }
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventKapiTileReward,Storage.BigLevel.ToString());
            if (Storage.BigLevel + 1 < LevelConfig.Count)
            {
                Storage.BigLevel++;
                Storage.SmallLevel = 0;
            }
        }
    }

    public void DealFail()
    {
        GameBIManager.Instance.SendGameEvent(
            BiEventAdventureIslandMerge.Types.GameEventType.GameEventKapiTileLevelEnd,
            Storage.BigLevel.ToString(),
            Storage.PlayingSmallLevel.ToString(),
            "fail",
            TileMatchRoot.Instance.PropUseState.ToBIString());
    }

    public bool ShowAuxItem()
    {
        if (Storage == null)
            return false;

        return Storage.ShowAuxItem();
    }
    
    public bool ShowTaskEntrance()
    {
        if (Storage == null)
            return false;

        return Storage.ShowTaskEntrance();
    }
    
}