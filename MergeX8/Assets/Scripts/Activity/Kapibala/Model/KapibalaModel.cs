
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.Kapibala;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Dynamic;
using Gameplay;
using TMatch;
using UnityEngine;

public partial class KapibalaModel : ActivityEntityBase
{
    public bool ShowEntrance()
    {
        return IsPrivateOpened();
    }
    private static KapibalaModel _instance;
    public static KapibalaModel Instance => _instance ?? (_instance = new KapibalaModel());

    public override string Guid => "OPS_EVENT_TYPE_KAPIBALA";


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitAuto()
    {
        Instance.Init();
    }
    
    public StorageKapibala Storage => StorageManager.Instance.GetStorage<StorageHome>().Kapibala;

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

        List<T> tableData = KapibalaConfigManager.Instance.GetConfig<T>();
        if (tableData == null)
            return;

        config.Clear();
        foreach (T kv in tableData)
        {
            config.Add(kv.GetID(), kv);
        }
    }

    public KapibalaGlobalConfig GlobalConfig => KapibalaConfigManager.Instance.GetConfig<KapibalaGlobalConfig>()[0];
    public List<KapibalaLevelConfig> LevelConfig => KapibalaConfigManager.Instance.GetConfig<KapibalaLevelConfig>();
    public List<KapibalaGiftBagConfig> GiftBagConfig => KapibalaConfigManager.Instance.GetConfig<KapibalaGiftBagConfig>();
    public override void InitFromServerData(string activityId, string activityType, ulong startTime, ulong endTime,
        ulong rewardEndTime, bool manualEnd, string configJson, string activitySubType)
    {
        DebugUtil.LogError("1");
        KapibalaConfigManager.Instance.InitConfig(configJson);
        base.InitFromServerData(activityId, activityType, startTime, endTime, rewardEndTime, manualEnd, configJson,
            activitySubType);
        DebugUtil.Log($"InitConfig:{Guid}");
        InitStorage();
    }
    public KapibalaModel()
    {
        TMatch.Timer.Register(1, UpdateLife, null, true);
    }
    public bool IsUnlock => UnlockManager.IsOpen(UnlockManager.MergeUnlockType.Kapibala);

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
        return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.Kapibala);
    }

    public Transform GetCommonFlyTarget()
    {
        if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
        {
            var entrance = DynamicEntryManager.Instance.GetDynamicEntry<DynamicEntry_Game_Kapibala>();
            if (entrance)
                return entrance.transform;
            else
                return MergeMainController.Instance.rewardBtnTrans;
        }
        else
        {
            var auxItem = DynamicEntryManager.Instance.GetDynamicEntry<DynamicEntry_Home_Kapibala>();
            if (auxItem != null && auxItem.gameObject.activeInHierarchy)
                return auxItem.transform;
            else
                return UIHomeMainController.mainController.MainPlayTransform;
        }
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
            UIPopupKapibalaStartController.Open(Instance.Storage);
            return true;
        }
        return false;
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
        EventDispatcher.Instance.SendEventImmediately(new EventKapibalaLifeChange(oldValue,newValue));
        GameBIManager.Instance.SendGameEvent(
            BiEventAdventureIslandMerge.Types.GameEventType.GameEventKapibalaLifeChange,
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
        EventDispatcher.Instance.SendEventImmediately(new EventKapibalaRebornCountChange(oldValue,newValue));
        
        GameBIManager.Instance.SendGameEvent(
            BiEventAdventureIslandMerge.Types.GameEventType.GameEventKapibalaRebornChange,
            getCount.ToString(), newValue.ToString(), reason);
    }
    
    
    
    public void OnPurchase(TableShop shopConfig)
    {
        if (!IsInitFromServer())
            return;
        var productList = GiftBagConfig;
        var product = productList.Find(a => a.ShopId == shopConfig.id);
        if (product == null)
            return;
        var rewards = CommonUtils.FormatReward(product.RewardId, product.RewardNum);
        var reason = new GameBIManager.ItemChangeReasonArgs()
        {
            reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.Iap
        };
        EventDispatcher.Instance.DispatchEvent(EventEnum.PURCHASE_SUCCESS_REWARD, rewards);

        CommonRewardManager.Instance.PopCommonReward(rewards, CurrencyGroupManager.Instance.GetCurrencyUseController(),
            true,
            reason, animEndCall: () =>
            {
            });
    }

    public KapibalaLevelConfig GetLevelConfig(int level)
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
        AddLife(1,"ReturnOnWin");
        Storage.SmallLevel = Storage.PlayingSmallLevel+1;
        var curLevel = GetLevelConfig(Storage.BigLevel);
        if (Storage.SmallLevel >= curLevel.SmallLevels.Count)//通关
        {
            var rewards = CommonUtils.FormatReward(curLevel.RewardId, curLevel.RewardNum);
            var reason = new GameBIManager.ItemChangeReasonArgs()
            {
                reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.KapibalaGet
            };
            UserData.Instance.AddRes(rewards,reason);
            foreach (var reward in rewards)
            {
                if (!UserData.Instance.IsResource(reward.id) && !TMatchModel.Instance.IsTMatchResId(reward.id))
                {
                    GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                    {
                        MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonKapibalaGet,
                        isChange = false,
                        itemAId = reward.id
                    });
                }
            }
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventKapibalaReward,Storage.BigLevel.ToString());
            if (Storage.BigLevel + 1 < LevelConfig.Count)
            {
                Storage.BigLevel++;
                Storage.SmallLevel = 0;
            }
        }
    }

    public void DealFail()
    {
        
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