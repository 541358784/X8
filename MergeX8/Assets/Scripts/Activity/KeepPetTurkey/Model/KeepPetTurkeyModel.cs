using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Deco.Node;
using Deco.World;
using Decoration;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.CoinRush;
using DragonPlus.Config.KeepPetTurkey;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using GamePool;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

public partial class KeepPetTurkeyModel : ActivityEntityBase
{
    public bool ShowEntrance()
    {
        return IsPrivateOpened();
    }
    private static KeepPetTurkeyModel _instance;
    public static KeepPetTurkeyModel Instance => _instance ?? (_instance = new KeepPetTurkeyModel());

    public override string Guid => "OPS_EVENT_TYPE_KEEP_PET_TURKEY";


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitAuto()
    {
        Instance.Init();
    }

    public StorageKeepPetTurkey Storage => StorageManager.Instance.GetStorage<StorageHome>().KeepPetTurkey;

    public void InitStorage()
    {
        if (!IsInitFromServer())
            return;
        if (Storage.ActivityId != ActivityId)
        {
            Storage.Clear();
            Storage.ActivityId = ActivityId;
            Storage.FinishStoreItemList.Clear();
            Storage.Score = 0;
            Storage.IsStart = false;
            Storage.UnLockStoreLevel.Clear();
            Storage.UnLockStoreLevel.Add(1);
        }
        Storage.StartTime = (long)StartTime;
        Storage.EndTime = (long)EndTime;
    }
    private static void InitTable<T>(Dictionary<int, T> config) where T : TableBase
    {
        if (config == null)
            return;

        List<T> tableData = KeepPetTurkeyConfigManager.Instance.GetConfig<T>();
        if (tableData == null)
            return;

        config.Clear();
        foreach (T kv in tableData)
        {
            config.Add(kv.GetID(), kv);
        }
    }
    public List<KeepPetTurkeyStoreLevelConfig> StoreLevelConfig => KeepPetTurkeyConfigManager.Instance.GetConfig<KeepPetTurkeyStoreLevelConfig>();
    public Dictionary<int, KeepPetTurkeyStoreItemConfig> StoreItemConfig = new Dictionary<int, KeepPetTurkeyStoreItemConfig>();
    public Dictionary<int, KeepPetTurkeyTaskScoreConfig> TaskScoreConfig = new Dictionary<int, KeepPetTurkeyTaskScoreConfig>();
    public override void InitFromServerData(string activityId, string activityType, ulong startTime, ulong endTime,
        ulong rewardEndTime, bool manualEnd, string configJson, string activitySubType)
    {
        DebugUtil.LogError("1");
        KeepPetTurkeyConfigManager.Instance.InitConfig(configJson);
        InitTable(StoreItemConfig);
        InitTable(TaskScoreConfig);
        base.InitFromServerData(activityId, activityType, startTime, endTime, rewardEndTime, manualEnd, configJson,
            activitySubType);
        DebugUtil.Log($"InitConfig:{Guid}");
        InitStorage();
    }

    public bool IsUnlock => UnlockManager.IsOpen(UnlockManager.MergeUnlockType.KeepPetDog);

    public override bool IsOpened(bool hasLog = false)
    {
        return base.IsOpened(hasLog) && IsUnlock; //当前当前周的配置;
    }
    public bool IsPrivateOpened()
    {
        return IsOpened() &&!Storage.IsTimeOut();
    }
    public int GetScore()
    {
        return Storage.Score;
    }
    public bool ReduceScore(int reduceCount,string reason)
    {
        if (Storage.Score < reduceCount)
            return false;
        Storage.Score -= reduceCount;
        EventDispatcher.Instance.SendEventImmediately(new EventKeepPetTurkeyScoreChange(-reduceCount));
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventKeepPetTurkeyRadishChange,
            (-reduceCount).ToString(),Storage.Score.ToString(),reason);
        return true;
    }
    public override bool CanDownLoadRes()
    {
        return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.KeepPetDog);
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
            UIPopupKeepPetTurkeyStartController.Open(Instance.Storage);
            return true;
        }
        return false;
    }
}