
using System.Collections.Generic;
using Activity.KapiScrew.View;
using DragonPlus;
using DragonPlus.Config.KapiScrew;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Dynamic;
using Gameplay;
using Screw;
using TMatch;
using UnityEngine;

public partial class KapiScrewModel : ActivityEntityBase
{
    public bool ShowEntrance()
    {
        return IsPrivateOpened();
    }
    private static KapiScrewModel _instance;
    public static KapiScrewModel Instance => _instance ?? (_instance = new KapiScrewModel());

    public override string Guid => "OPS_EVENT_TYPE_KAPI_SCREW";


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitAuto()
    {
        Instance.Init();
    }
    
    public StorageKapiScrew Storage => StorageManager.Instance.GetStorage<StorageHome>().KapiScrew;

    public void InitStorage()
    {
        if (!IsInitFromServer())
            return;
        if (Storage.ActivityId != ActivityId)
        {
            Storage.Clear();
            Storage.ActivityId = ActivityId;
            Storage.IsStart = false;
            var giftBagConfig = GetOptionalGiftActivityConfig();
            Storage.GiftBag.SelectItem.Add(0,giftBagConfig.Item1[0]);
            Storage.GiftBag.SelectItem.Add(1,giftBagConfig.Item2[0]);
            Storage.GiftBag.SelectItem.Add(2,giftBagConfig.Item3[0]);
            Storage.ChangeEnemy = true;
        }
        Storage.StartTime = (long)StartTime;
        Storage.PreheatCompleteTime = (long) StartTime + GlobalConfig.PreheatTime * (long)XUtility.Hour;
        Storage.EndTime = (long)EndTime;
    }

    private static void InitTable<T>(Dictionary<int, T> config) where T : TableBase
    {
        if (config == null)
            return;

        List<T> tableData = KapiScrewConfigManager.Instance.GetConfig<T>();
        if (tableData == null)
            return;

        config.Clear();
        foreach (T kv in tableData)
        {
            config.Add(kv.GetID(), kv);
        }
    }

    public KapiScrewGlobalConfig GlobalConfig => KapiScrewConfigManager.Instance.GetConfig<KapiScrewGlobalConfig>()[0];
    public List<KapiScrewLevelConfig> LevelConfig => KapiScrewConfigManager.Instance.GetConfig<KapiScrewLevelConfig>();
    public List<KapiScrewGiftBagConfig> GiftBagConfig => KapiScrewConfigManager.Instance.GetConfig<KapiScrewGiftBagConfig>();
    public override void InitFromServerData(string activityId, string activityType, ulong startTime, ulong endTime,
        ulong rewardEndTime, bool manualEnd, string configJson, string activitySubType)
    {
        DebugUtil.LogError("1");
        KapiScrewConfigManager.Instance.InitConfig(configJson);
        base.InitFromServerData(activityId, activityType, startTime, endTime, rewardEndTime, manualEnd, configJson,
            activitySubType);
        DebugUtil.Log($"InitConfig:{Guid}");
        InitStorage();
    }
    public KapiScrewModel()
    {
        TMatch.Timer.Register(1, UpdateLife, null, true);
    }
    public bool IsUnlock => UnlockManager.IsOpen(UnlockManager.MergeUnlockType.KapiScrew);

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
        return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.KapiScrew);
    }

    public Transform GetCommonFlyTarget()
    {
        if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
        {
            var entrance = DynamicEntryManager.Instance.GetDynamicEntry<DynamicEntry_Game_KapiScrew>();
            if (entrance)
                return entrance.transform;
            else
                return MergeMainController.Instance.rewardBtnTrans;
        }
        else
        {
            var auxItem = DynamicEntryManager.Instance.GetDynamicEntry<DynamicEntry_Home_KapiScrew>();
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
            UIPopupKapiScrewStartController.Open(Instance.Storage);
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
        EventDispatcher.Instance.SendEventImmediately(new EventKapiScrewLifeChange(oldValue,newValue));
        GameBIManager.Instance.SendGameEvent(
            BiEventAdventureIslandMerge.Types.GameEventType.GameEventKapiScrewLifeChange,
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
        EventDispatcher.Instance.SendEventImmediately(new EventKapiScrewRebornCountChange(oldValue,newValue));
        
        GameBIManager.Instance.SendGameEvent(
            BiEventAdventureIslandMerge.Types.GameEventType.GameEventKapiScrewRebornChange,
            getCount.ToString(), newValue.ToString(), reason);
    }

    public KapiScrewLevelConfig GetLevelConfig(int level)
    {
        if (level < 0 || level >= LevelConfig.Count)
            return null;
        return LevelConfig[level];
    }

    public void DealStartGame()
    {
        Storage.PlayingSmallLevel = Storage.SmallLevel;
        Storage.SmallLevel = 0;
        Storage.ChangeEnemy = true;
        AddLife(-1,"ReduceOnStart");
        PropCostState.Clear();
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventKapiScrewLevelEnter,
            data1:Storage.BigLevel.ToString(),
            data2:Storage.PlayingSmallLevel.ToString());
    }
    public Dictionary<int, int> PropCostState = new Dictionary<int, int>();

    public void CostProp(int prop, int count)
    {
        if (!IsInitFromServer())
            return;
        PropCostState.TryAdd(prop, 0);
        PropCostState[prop] += count;
    }

    public Dictionary<string, string> PropCostStateToString()
    {
        var str = new Dictionary<string, string>();
        foreach (var pair in PropCostState)
        {
            str.Add(pair.Key.ToString(),pair.Value.ToString());
        }
        return str;
    }
    public void DealWin()
    {
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventKapiScrewLevelEnd,
            data1:KapiScrewModel.Instance.Storage.BigLevel.ToString(),
            data2:KapiScrewModel.Instance.Storage.PlayingSmallLevel.ToString(),
            data3:"Win",
            extras:PropCostStateToString());
        Storage.ChangeEnemy = false;
        AddLife(1,"ReturnOnWin");
        Storage.SmallLevel = Storage.PlayingSmallLevel+1;
        var curLevel = GetLevelConfig(Storage.BigLevel);
        if (Storage.SmallLevel >= curLevel.SmallLevels.Count)//通关
        {
            var rewards = CommonUtils.FormatReward(curLevel.RewardId, curLevel.RewardNum);
            var reason = new GameBIManager.ItemChangeReasonArgs()
            {
                reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.KapiScrew
            };
            UserData.Instance.AddRes(rewards,reason);
            foreach (var reward in rewards)
            {
                if (!UserData.Instance.IsResource(reward.id) && !ScrewGameModel.Instance.IsScrewResId(reward.id))
                {
                    GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                    {
                        MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonKapiScrewGet,
                        isChange = false,
                        itemAId = reward.id
                    });
                }
            }
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventKapiScrewReward,Storage.BigLevel.ToString());
            if (Storage.BigLevel + 1 < LevelConfig.Count)
            {
                Storage.BigLevel++;
                Storage.SmallLevel = 0;
                Storage.ChangeEnemy = true;
            }
        }
    }

    public void DealFail()
    {
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventKapiScrewLevelEnd,
            data1:KapiScrewModel.Instance.Storage.BigLevel.ToString(),
            data2:KapiScrewModel.Instance.Storage.PlayingSmallLevel.ToString(),
            data3:"Fail",
            extras:PropCostStateToString());
    }

    public string GetRandomName()
    {
        var nameList = KapiScrewConfigManager.Instance.GetConfig<KapiScrewNameConfig>();
        return nameList[Random.Range(0, nameList.Count)].Name;
    }

    public bool ShowAuxItem()
    {
        if (Storage == null)
            return false;

        return KapiScrewUtils.ShowAuxItem(Storage);
    }
    
    public bool ShowTaskEntrance()
    {
        if (Storage == null)
            return false;

        return Storage.ShowTaskEntrance();
    }
    
}