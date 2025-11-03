using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.CoinRush;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;

public partial class CoinRushModel :ActivityEntityBase
{
    private static CoinRushModel _instance;
    public static CoinRushModel Instance => _instance ?? (_instance = new CoinRushModel());
    
    public override string Guid => "OPS_EVENT_TYPE_COIN_RUSH";
    public static string Guid2 => "OPS_EVENT_TYPE_COIN_RUSH2";

    static bool IsCoinRushStorageEnd(StorageCoinRush storageCoinRush)
    {
        return storageCoinRush.IsCollectFinalReward ||
               (long) APIManager.Instance.GetServerTime() >= storageCoinRush.ActivityEndTime ||
               (long) APIManager.Instance.GetServerTime() < storageCoinRush.StartActivityTime;
    }


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitAuto()
    {
        Instance.Init();
        if (ActivityManager.Instance._activityModules.ContainsKey(Guid2))
        {
            DebugUtil.LogError($"register {Guid2} to config hub repeated.");
            return;
        }
        ActivityManager.Instance._activityModules[Guid2] = Instance;
    }
    public StorageCoinRush StorageCoinRush
    {
        get
        {
            if (!IsInitFromServer())
                return new StorageCoinRush();
            var storage = StorageManager.Instance.GetStorage<StorageHome>().CoinRush;
            if (!storage.ContainsKey(StorageKey))
            {
                var newStorage = new StorageCoinRush();
                newStorage.PayLevelGroup = PayLevelModel.Instance.GetCurPayLevelConfig().CoinRushGroupId;
                storage.Add(StorageKey, newStorage);
                newStorage.SkinName = PreheatConfigList[0].SkinName;
                newStorage.ActivityId = ActivityId;
                // var resMd5List = ActivityManager.Instance.GetActivityMd5List(ActivityId);
                // newStorage.ActivityResList.Clear();
                // foreach (var resMd5 in resMd5List)
                // {
                //     var resPath = ActivityResHotUpdate.GetFilePath(resMd5);
                //     newStorage.ActivityResList.Add(resPath);
                // }
            }
            return storage[StorageKey];
        }
    }

    public long PreheatTime => IsSkipActivityPreheating()?0:PreheatConfigList[0].PreheatTime * 3600 * 1000;
    public List<ResData> FinialRewards
    {
        get
        {
            var lastRewardConfig = LastRewardConfigList[0];
            var result = new List<ResData>();
            for (var i = 0; i < lastRewardConfig.RewardId.Count; i++)
            {
                result.Add(new ResData(lastRewardConfig.RewardId[i],lastRewardConfig.RewardNum[i]));
            }
            return result;
        }
    }
    public List<CoinRushTaskConfig> CoinRushTaskConfig=>CoinRushTaskConfigList;
    private Dictionary<int, CoinRushTaskConfig> CoinRushTaskConfigDic;
    public override void InitFromServerData(string activityId, string activityType, ulong startTime, ulong endTime,
        ulong rewardEndTime, bool manualEnd, string configJson, string activitySubType)
    {
        CoinRushConfigManager.Instance.InitConfig(configJson);
        startTime += (ulong)PreheatTime;
        base.InitFromServerData(activityId, activityType, startTime, endTime, rewardEndTime, manualEnd, configJson,
            activitySubType);
        StorageCoinRush.StartActivityTime = (long)StartTime;
        StorageCoinRush.ActivityEndTime = (long)EndTime;
        CoinRushTaskConfigDic = new Dictionary<int, CoinRushTaskConfig>();
        CollectTypeTaskList.Clear();
        for (var i = 0; i < CoinRushTaskConfig.Count; i++)
        {
            var taskConfig = CoinRushTaskConfig[i];
            CoinRushTaskConfigDic.Add(taskConfig.Id,taskConfig);
            if (AlreadyCollectLevels.Contains(taskConfig.Id))
                continue;
            var collectType = taskConfig.CollectType;
            if (!CollectTypeTaskList.TryGetValue(collectType, out var taskList))
            {
                taskList = new List<int>();
                CollectTypeTaskList.Add(collectType,taskList);
            }
            taskList.Add(taskConfig.Id);
        }
        _lastActivityOpenState = CoinRushModel.Instance.IsOpened();
        InitServerDataFinish();
        DebugUtil.Log($"InitConfig:{Guid}");
        CleanUselessStorage();
        StorageCoinRush.AddSkinUIWindowInfo();
    }
    public void CleanUselessStorage()
    {
        var cleanStorageKeyList = StorageManager.Instance.GetStorage<StorageHome>().CoinRush.Keys.ToList();
        foreach (var key in cleanStorageKeyList)
        {
            var storage = StorageManager.Instance.GetStorage<StorageHome>().CoinRush[key];
            if (storage.ActivityEndTime < (long) APIManager.Instance.GetServerTime() && 
                storage.UnCollectRewards.Count == 0)
            {
                StorageManager.Instance.GetStorage<StorageHome>().CoinRush.Remove(key);
            }
        }
    }

    public Dictionary<int, List<int>> CollectTypeTaskList =
        new Dictionary<int, List<int>>();
    public void OnAddRes(UserData.ResourceId resId,int count)
    {
        if (!IsOpened())
            return;
        if (!StorageCoinRush.TargetCollectState.ContainsKey((int) resId))
        {
            StorageCoinRush.TargetCollectState.Add((int) resId,0);
        }
        StorageCoinRush.TargetCollectState[(int) resId] += count;
        lastAddValue = count;
        CheckTaskComplete((int)resId);
    }

    private int lastAddValue;

    public enum CoinRushTaskTargetType
    {
        Add=0,
        Consume=1,
        Special = 2,
    }
    public enum CoinRushSpecialCollectType
    {
        Bubble = -1,
        FlashSale = -2,
        GetCoin = -3,
    }
    public void OnGetCoin(int count)
    {
        if (!IsOpened())
            return;
        var resId = CoinRushSpecialCollectType.GetCoin;
        if (!StorageCoinRush.SpecialTargetState.ContainsKey((int) resId))
        {
            StorageCoinRush.SpecialTargetState.Add((int) resId,0);
        }
        StorageCoinRush.SpecialTargetState[(int) resId] += count;
        lastAddValue = count;
        CheckTaskComplete((int)resId);
    }
    public void CheckTaskComplete(int resId)
    {
        if (CollectTypeTaskList.TryGetValue(resId, out var taskList))
        {
            for (var i = 0; i < taskList.Count; i++)
            {
                var taskId = taskList[i];
                var taskConfig = CoinRushTaskConfigDic[taskId];
                if (GetTaskCollectCount(taskConfig) >= taskConfig.CollectCount)
                {
                    CompletedCoinRushTask(taskConfig);
                    i--;
                    if (IsMaxLevel && !StorageCoinRush.IsCollectFinalReward && StorageCoinRush.UnCollectRewards.Count == 0)
                    {
                        foreach (var finialReward in FinialRewards)
                        {
                            StorageCoinRush.UnCollectRewards.Add(finialReward.id, finialReward.count);
                        }
                    }
                }
            }
        }
    }

    public int GetTaskCollectCount(CoinRushTaskConfig taskConfig)
    {
        if (!IsOpened())
            return 0;
        var resId = taskConfig.CollectType;
        if ((CoinRushTaskTargetType) taskConfig.TargetType == CoinRushTaskTargetType.Add)
        {
            return StorageCoinRush.TargetCollectState.TryGetValue(resId,out var count)?count:0;
        }
        else if ((CoinRushTaskTargetType) taskConfig.TargetType == CoinRushTaskTargetType.Consume)
        {
            return StorageCoinRush.TargetConsumeState.TryGetValue(resId,out var count)?count:0;
        }
        else if ((CoinRushTaskTargetType) taskConfig.TargetType == CoinRushTaskTargetType.Special)
        {
            return StorageCoinRush.SpecialTargetState.TryGetValue(resId, out var count) ? count : 0;
        }
        return 0;
    }
    public void OnConsumeRes(UserData.ResourceId resId,int count)
    {
        if (!IsOpened())
            return;
        if (!StorageCoinRush.TargetConsumeState.ContainsKey((int) resId))
        {
            StorageCoinRush.TargetConsumeState.Add((int) resId,0);
        }
        StorageCoinRush.TargetConsumeState[(int) resId] += count;
        lastAddValue = count;
        CheckTaskComplete((int)resId);
    }
    public StorageList<int> AlreadyCollectLevels => StorageCoinRush.AlreadyCollectLevels;


    public void CompletedCoinRushTask(CoinRushTaskConfig task)
    {
        CollectTypeTaskList[task.CollectType].Remove(task.Id);
        AlreadyCollectLevels.Add(task.Id);
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventCoinrushMission,task.Id.ToString());
        UICoinRushTaskCompletedController.PushCompletedTask(task,Math.Max(0,task.CollectCount-lastAddValue),task.CollectCount);
    }
    
    public CoinRushModel()
    {
        TMatch.Timer.Register(1, UpdateTime, null, true);
        EventDispatcher.Instance.AddEvent<EventUserDataAddRes>((e) =>
        {
            OnAddRes(e.ResId,e.Count);
        });
        EventDispatcher.Instance.AddEvent<EventUserDataConsumeRes>((e) =>
        {
            OnConsumeRes(e.ResId,e.Count);
        });
        EventDispatcher.Instance.AddEventListener(EventEnum.AddCoin, (e) =>  OnGetCoin((int)e.datas[0]));
        EventDispatcher.Instance.AddEventListener(EventEnum.AddRecoverCoinStar, (e) =>  OnGetCoin((int)e.datas[0]));
    }

    public int Level => StorageCoinRush.AlreadyCollectLevels.Count;
    public int MaxLevel => CoinRushTaskConfig.Count;
    public bool IsMaxLevel => Level == MaxLevel;

    public override void UpdateActivityState()
    {
        // InitServerDataFinish();
    }

    protected override void InitServerDataFinish()
    {
        
    }

    public override bool IsOpened(bool hasLog = false)
    {
        return UnlockManager.IsOpen(UnlockManager.MergeUnlockType.CoinRush) //已解锁
               && base.IsOpened(hasLog) 
               && !StorageCoinRush.IsCollectFinalReward;
    }

    public void CompletedActivity()
    {
        CompletedStorageActivity(StorageCoinRush);
    }

    public void CompletedStorageActivity(StorageCoinRush storage)
    {
        storage.IsCollectFinalReward = true;
        storage.UnCollectRewards.Clear();
    }

    public void EndActivity()
    {
        CoinRushModel.CanShowUnCollectRewardsUI();
    }

    public void StartActivity()
    {
        
    }
    
    private bool _lastActivityOpenState;
    public void UpdateTime()
    {
        if (!IsInitFromServer())
            return;
        var currentActivityOpenState = CoinRushModel.Instance.IsOpened();
        if (_lastActivityOpenState == currentActivityOpenState)
            return;
        if (_lastActivityOpenState && !currentActivityOpenState)
        {
            CoinRushModel.Instance.EndActivity();
        }
        else if(!_lastActivityOpenState && currentActivityOpenState)
        {
            CoinRushModel.Instance.StartActivity();
        }

        _lastActivityOpenState = currentActivityOpenState;
    }
    public List<ResData> GetUnCollectRewards()
    {
        var unCollectRewardsList = new List<ResData>();
        List<string> keys = new List<string>(StorageManager.Instance.GetStorage<StorageHome>().CoinRush.Keys);
        for (int i = keys.Count - 1; i >= 0; i--)
        {
            var storageCoinRush = StorageManager.Instance.GetStorage<StorageHome>().CoinRush[keys[i]];
            if (IsCoinRushStorageEnd(storageCoinRush))
            {
                foreach (var pair in storageCoinRush.UnCollectRewards)
                {
                    if (pair.Value > 0)
                    {
                        unCollectRewardsList.Add(new ResData(pair.Key,pair.Value));
                    }
                }
            }
        }
        return unCollectRewardsList;
    }
    public void CleanUnCollectRewards()
    {
        List<string> keys = new List<string>(StorageManager.Instance.GetStorage<StorageHome>().CoinRush.Keys);
        for (int i = keys.Count - 1; i >= 0; i--)
        {
            var storageCoinRush = StorageManager.Instance.GetStorage<StorageHome>().CoinRush[keys[i]];
            if (IsCoinRushStorageEnd(storageCoinRush))
            {
                CompletedStorageActivity(storageCoinRush);
            }
        }
    }
    
    public override bool CanDownLoadRes()
    {
        return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.CoinRush);
    }
    public static bool CanShowUnCollectRewardsUI()
    {
        if (CoinRushModel.Instance.GetUnCollectRewards().Count > 0)
        {
            var reasonArgs = new GameBIManager.ItemChangeReasonArgs(){reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.CoinrushMission};
            var unCollectRewards = CoinRushModel.Instance.GetUnCollectRewards();
            CommonRewardManager.Instance.PopActivityUnCollectReward(unCollectRewards, reasonArgs, null, () =>
            {
                for (int i = 0; i < unCollectRewards.Count; i++)
                {
                    if (!UserData.Instance.IsResource(unCollectRewards[i].id))
                    {
                        GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                        {
                            MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonCoinrushMission,
                            isChange = false,
                            itemAId = unCollectRewards[i].id
                        });
                    }
                }
                CoinRushModel.Instance.CleanUnCollectRewards();
            });
            // UIManager.Instance.OpenUI(UINameConst.UIClimbTreeUnSelect);
            return true;
        }
        return false;
    }

    private const string coolTimeKey = "CoinRush";
    public static bool CanShowMainPopup()
    {
        if (!Instance.IsOpened())
            return false;
        if (!CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, coolTimeKey))
        {
            CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey, CommonUtils.GetTimeStamp());
            UIManager.Instance.OpenUI(Instance.StorageCoinRush.GetAssetPathWithSkinName(UINameConst.UICoinRushMain));
            return true;
        }
        return false;
    }
    
    public static string[] ShowMainUIList()
    {
        return new[] {Instance.StorageCoinRush.GetAssetPathWithSkinName(UINameConst.UICoinRushMain)};
    }
    public static string[] TaskCompleteUIList()
    {
        return new[] {Instance.StorageCoinRush.GetAssetPathWithSkinName(UINameConst.UICoinRushTaskCompleted)};
    }
    
    
    public override List<string> GetNeedResList(string activityId,List<string> allResList)
    {
        var skinNameList = new List<string>();
        if (IsInitFromServer())
            skinNameList.Add(StorageCoinRush.SkinName.ToLower());
        var resList = new List<string>();
        foreach (var path in allResList)
        {
            foreach (var skinName in skinNameList)
            {
                if (path.Contains(skinName))
                {
                    DebugUtil.Log("CoinRushModel -> 活动资源 : " + path);
                    resList.Add(path);
                    break;
                }
            }
        }
        return resList;
    }

    public string GetMergeItemAssetPath()
    {
        if (StorageCoinRush == null)
            return null;

        return StorageCoinRush.GetTaskItemAssetPath();
    }
}