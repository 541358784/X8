using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DragonPlus;
using DragonPlus.Config.SummerWatermelonBread;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public partial class SummerWatermelonBreadModel:ActivityEntityBase
{
    public static bool InGuideChain = false;
    public const int BOARD_WIDTH = 5;
    public const int BOARD_HEIGHT = 5;
    
    private static SummerWatermelonBreadModel _instance;
    public static SummerWatermelonBreadModel Instance => _instance ?? (_instance = new SummerWatermelonBreadModel());
    
    public override string Guid => "OPS_EVENT_TYPE_SUMMER_WATERMELON_BREAD";
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitAuto()
    {
        Instance.Init();
    }
    public StorageSummerWatermelonBread StorageSummerWatermelonBread
    {
        get
        {
            var storage = StorageManager.Instance.GetStorage<StorageHome>().SummerWatermelonBread;
            if (!storage.ContainsKey(StorageKey))
            {
                var newStorage = new StorageSummerWatermelonBread();
                foreach (var itemLevel in SummerWatermelonBreadConfig.InitItems)
                {
                    newStorage.UnSetItems.Add(GetMergeItemConfig(itemLevel).id);
                    UnSetItemsCount++;
                }

                if (IsInitFromServer())
                {
                    newStorage.StartActivityTime = (long)StartTime;
                    newStorage.ActivityEndTime = (long)EndTime;
                }
                storage.Add(StorageKey,newStorage);
            }
            return storage[StorageKey];
        }
    }

    public SummerWatermelonBreadConfig SummerWatermelonBreadConfig => SummerWatermelonBreadConfigManager.Instance.GetConfig<SummerWatermelonBreadConfig>()[0];
    public List<SummerWatermelonBreadPackageConfig> PackageConfig => SummerWatermelonBreadConfigManager.Instance.GetConfig<SummerWatermelonBreadPackageConfig>();
    public List<SummerWatermelonBreadProductAttenuationConfig> ProductAttenuationConfig => SummerWatermelonBreadConfigManager.Instance.GetConfig<SummerWatermelonBreadProductAttenuationConfig>();
    private Dictionary<int, SummerWatermelonBreadProductConfig> _SummerWatermelonBreadProductConfig;
    public Dictionary<int,SummerWatermelonBreadProductConfig> SummerWatermelonBreadProductConfig
    {
        get
        {
            if (_SummerWatermelonBreadProductConfig == null)
            {
                _SummerWatermelonBreadProductConfig = new Dictionary<int, SummerWatermelonBreadProductConfig>();
                var serverCfg = SummerWatermelonBreadConfigManager.Instance.GetConfig<SummerWatermelonBreadProductConfig>();
                foreach (var cfg in serverCfg)
                {
                    _SummerWatermelonBreadProductConfig.Add(cfg.ItemId,cfg);
                }
            }
            return _SummerWatermelonBreadProductConfig;
        }
    }

    private Dictionary<int, SummerWatermelonBreadFirstTimeRewardConfig> _SummerWatermelonBreadFirstTimeRewardConfig;
    public Dictionary<int,SummerWatermelonBreadFirstTimeRewardConfig> SummerWatermelonBreadFirstTimeRewardConfig
    {
        get
        {
            if (_SummerWatermelonBreadFirstTimeRewardConfig == null)
            {
                _SummerWatermelonBreadFirstTimeRewardConfig = new Dictionary<int, SummerWatermelonBreadFirstTimeRewardConfig>();
                var serverCfg = SummerWatermelonBreadConfigManager.Instance.GetConfig<SummerWatermelonBreadFirstTimeRewardConfig>();
                foreach (var cfg in serverCfg)
                {
                    _SummerWatermelonBreadFirstTimeRewardConfig.Add(cfg.Level,cfg);
                }
            }
            return _SummerWatermelonBreadFirstTimeRewardConfig;
        }
    }
    
    
    private Dictionary<int, SummerWatermelonBreadMergeRewardConfig> _SummerWatermelonBreadMergeRewardConfig;
    public Dictionary<int,SummerWatermelonBreadMergeRewardConfig> SummerWatermelonBreadMergeRewardConfig
    {
        get
        {
            if (_SummerWatermelonBreadMergeRewardConfig == null)
            {
                _SummerWatermelonBreadMergeRewardConfig = new Dictionary<int, SummerWatermelonBreadMergeRewardConfig>();
                var serverCfg = SummerWatermelonBreadConfigManager.Instance.GetConfig<SummerWatermelonBreadMergeRewardConfig>();
                foreach (var cfg in serverCfg)
                {
                    _SummerWatermelonBreadMergeRewardConfig.Add(cfg.Level,cfg);
                }
            }
            return _SummerWatermelonBreadMergeRewardConfig;
        }
    }

    public TableMergeLine MergeLineConfig => GameConfigManager.Instance.GetMergeLine(SummerWatermelonBreadConfig.LineId);
    private Dictionary<int, TableMergeItem> _mergeItemDictionary;
    public Dictionary<int, TableMergeItem> MergeItemDictionary
    {
        get
        {
            if (_mergeItemDictionary == null)
            {
                _mergeItemDictionary = new Dictionary<int, TableMergeItem>();
                var items = GameConfigManager.Instance.GetMergeInLineItems(SummerWatermelonBreadConfig.LineId);
                foreach (var item in items)
                {
                    _mergeItemDictionary.Add(item.level,item);
                }
            }
            return _mergeItemDictionary;
        }
    }
    public TableMergeItem GetMergeItemConfig(int level)
    {
        return MergeItemDictionary[level];
    }

    public StorageList<int> UnSetItems => StorageSummerWatermelonBread.UnSetItems;
    public StorageList<int> UnSetRewards => StorageSummerWatermelonBread.UnSetRewards;
    public int UnSetItemsCount = 0;//用于红点的显示
    public StorageDictionary<int, int> UnCollectRewards => StorageSummerWatermelonBread.UnCollectRewards;
    public bool IsStart => IsOpened() && StorageSummerWatermelonBread.IsStart;
    public override bool IsOpened(bool hasLog = false)
    {
        return UnlockManager.IsOpen(UnlockManager.MergeUnlockType.SummerWatermelonBread) //已解锁
               && base.IsOpened(hasLog);
    }

    public float GetProductAttenuationConfig(int dayProductCount)
    {
        if (ProductAttenuationConfig == null)
            return 1;
        for (var i = ProductAttenuationConfig.Count-1; i >= 0; i--)
        {
            var config = ProductAttenuationConfig[i];
            if (config.ProductCount <= dayProductCount)
            {
                return config.TotalWeightMulti;
            }
        }
        return 1;
    }
    public const ulong Offset = 0 * XUtility.Hour;
    public int CurDay => (int)((CurTime-Offset) / XUtility.DayTime);
    public ulong CurTime => APIManager.Instance.GetServerTime();
    public void TryProductWaterMellon(int index,int id,MergeBoard board,int doubleEnergyTimes)//尝试生成活动棋子
    {
        if (!IsStart)
            return;
        //缺判断能否生成棋子的条件
        if (!SummerWatermelonBreadProductConfig.ContainsKey(id))
            return;
        if (StorageSummerWatermelonBread.DayId != CurDay)
        {
            StorageSummerWatermelonBread.DayProductCount = 0;
            StorageSummerWatermelonBread.DayId = CurDay;
        }
        var totalWeightMulti = GetProductAttenuationConfig(StorageSummerWatermelonBread.DayProductCount);
        Debug.LogError("西瓜产出计数:"+StorageSummerWatermelonBread.DayProductCount+" 权重系数:"+totalWeightMulti);
        var cfg = SummerWatermelonBreadProductConfig[id];
        var randomWeight = Random.Range(0, cfg.MaxWeight * totalWeightMulti);
        var tempWeight = 0;
        var newItemId = -1;
        for (var i = 0; i < cfg.Weight.Count; i++)
        {
            tempWeight += cfg.Weight[i];
            if (tempWeight > randomWeight)
            {
                newItemId = cfg.OutPut[i];
                break;
            }
        }
        if (newItemId < 0)
            return;
        //缺判断能否生成棋子的条件
        var newItemConfig = GameConfigManager.Instance.GetItemConfig(newItemId);
        if (doubleEnergyTimes > 0)
        {
            for (var i = 0; i < doubleEnergyTimes; i++)
            {
                if (newItemConfig.next_level > 0)
                {
                    newItemId = newItemConfig.next_level;
                    newItemConfig = GameConfigManager.Instance.GetItemConfig(newItemId);
                }   
            }
        }
        StorageSummerWatermelonBread.DayProductCount+=(int)Math.Pow(2, newItemConfig.level-1);
        GameBIManager.Instance.SendGameEvent(
            BiEventAdventureIslandMerge.Types.GameEventType.GameEventSummerGetmelon,
            data1: newItemConfig.level.ToString());
        UnSetItems.Add(newItemConfig.id);
        if (MergeTaskTipsController.Instance._mergeSummerWatermelonBread)
        {
            FlyWatermelon(newItemConfig.id, board.GetGridPosition(index),
                MergeTaskTipsController.Instance._mergeSummerWatermelonBread.transform, 0.8f, true,
                () =>
                {
                    UnSetItemsCount++;
                });   
        }
        else
        {
            UnSetItemsCount++;
        }
    }

    private static GameObject _flyItemObj;
    public static GameObject FlyItemObj
    {
        get
        {
            if (_flyItemObj == null)
            {
                var prefab = ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/Activity/SummerWatermelonBread/FlyItem");
                _flyItemObj = GameObject.Instantiate(prefab);
            }
            return _flyItemObj;
        }
    }
    public static void FlyWatermelon(int itemId, Vector2 srcPos,Transform starTransform, float time, bool showEffect, Action action = null)
    {
        FlyItemObj.transform.Find("Icon").GetComponent<Image>().sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(GameConfigManager.Instance.GetItemConfig(itemId).image);
        Transform target = starTransform;
        float delayTime = 0f;
        Vector3 position = target.position;
        FlyItemObj.SetActive(true);
        FlyGameObjectManager.Instance.FlyObjectUpStraight(FlyItemObj, srcPos, position, showEffect, time, 0.5f,delayTime, () =>
        {
            FlyGameObjectManager.Instance.PlayHintStarsEffect(position);
            ShakeManager.Instance.ShakeLight();
            action?.Invoke();
        },controlY:0.5f);
        FlyItemObj.SetActive(false);
    }
    
    public override void InitFromServerData(string activityId, string activityType, ulong startTime, ulong endTime,
        ulong rewardEndTime, bool manualEnd, string configJson, string activitySubType)
    {
        SummerWatermelonBreadConfigManager.Instance.InitConfig(configJson);
        base.InitFromServerData(activityId, activityType, startTime, endTime, rewardEndTime, manualEnd, configJson,
            activitySubType);
        InitServerDataFinish();
        UnSetItemsCount = UnSetRewards.Count + UnSetItems.Count;
        _lastActivityOpenState = IsOpened();
        DebugUtil.Log($"InitConfig:{Guid}");
        RemoveUselessStorage();
    }

    protected override void InitServerDataFinish()
    {
        base.InitServerDataFinish();
        StorageSummerWatermelonBread.StartActivityTime = (long)StartTime;
        StorageSummerWatermelonBread.ActivityEndTime = (long)EndTime;
    }

    public void OpenMainPopup()
    {
        var mainPopup = UIManager.Instance.OpenUI(MainUIPath) as UIPopupSummerWatermelonBreadMainController;
    }

    public static UIPopupSummerWatermelonBreadMainController MainView =>
        UIManager.Instance.GetOpenedUIByPath<UIPopupSummerWatermelonBreadMainController>(Instance.MainUIPath);

    public virtual string MainUIPath => UINameConst.UIPopupSummerWatermelonBreadMain;
    public virtual string StartUIPath => UINameConst.UIPopupSummerWatermelonBreadStart;
    public virtual string NewLevelUIPath => UINameConst.UISummerWatermelonBreadReward;
    
    public static bool CanShowStartView()
    {
        
        if (Instance.IsOpened() && !Instance.IsStart && 
            (SceneFsm.mInstance.GetCurrSceneType() == StatusType.BackHome || SceneFsm.mInstance.GetCurrSceneType() == StatusType.Home) && 
            !GuideSubSystem.Instance.IsShowingGuide())
        {
            Instance.StorageSummerWatermelonBread.IsStart = true;
            StorageManager.Instance.GetStorage<StorageGame>().MergeBoards.Remove((int) MergeBoardEnum.SummerWatermelonBread);
            var startPopup = UIManager.Instance.OpenUI(Instance.StartUIPath) as UIPopupSummerWatermelonBreadStartController;
            return true;
        }

        if (Instance.IsOpened() && Instance.IsStart &&
            (SceneFsm.mInstance.GetCurrSceneType() == StatusType.BackHome || SceneFsm.mInstance.GetCurrSceneType() == StatusType.Home) && 
            !GuideSubSystem.Instance.IsShowingGuide() && !GuideSubSystem.Instance.isFinished(GuideTriggerPosition.SummerWatermelonBreadStart) &&
            GuideSubSystem.Instance.GetTarget(GuideTargetType.SummerWatermelonBreadStart,"") != null)
        {
            GuideSubSystem.Instance.Trigger(GuideTriggerPosition.SummerWatermelonBreadStart, null);
        }
        return false;
    }
    public void OpenNewLevelPopup(TableMergeItem itemConfig,TaskCompletionSource<bool> callbackTask)
    {
        var newLevelPopup = UIManager.Instance.OpenUI(NewLevelUIPath,itemConfig,callbackTask) as UISummerWatermelonBreadRewardController;
    }
    
    private bool _lastActivityOpenState;
    public SummerWatermelonBreadModel()
    {
        TMatch.Timer.Register(1, UpdateTime, null, true);
    }
    public void UpdateTime()
    {
        if (!IsInitFromServer())
            return;
        var currentActivityOpenState = IsOpened();
        if (_lastActivityOpenState == currentActivityOpenState)
            return;
        if (_lastActivityOpenState && !currentActivityOpenState)
        {
            EndActivity();
        }
        else if(!_lastActivityOpenState && currentActivityOpenState)
        {
            StartActivity();
        }
        _lastActivityOpenState = currentActivityOpenState;
    }
    
    public void EndActivity()
    {
        SummerWatermelonBreadModel.CanShowUnCollectRewardsUI();
    }

    public void StartActivity()
    {
        
    }
    static bool IsActivityStorageEnd(StorageSummerWatermelonBread storage)
    {
        return (long) APIManager.Instance.GetServerTime() >= storage.ActivityEndTime ||
               (long) APIManager.Instance.GetServerTime() < storage.StartActivityTime;
    }
    static void CompletedStorageActivity(StorageSummerWatermelonBread storage)
    {
        storage.UnCollectRewards.Clear();
    }
    public static void CleanUnCollectRewards()
    {
        List<string> keys = new List<string>(StorageManager.Instance.GetStorage<StorageHome>().SummerWatermelonBread.Keys);
        for (int i = keys.Count - 1; i >= 0; i--)
        {
            var storage = StorageManager.Instance.GetStorage<StorageHome>().SummerWatermelonBread[keys[i]];
            if (IsActivityStorageEnd(storage))
            {
                CompletedStorageActivity(storage);
            }
        }
    }

    public static void RemoveUselessStorage()
    {
        List<string> keys = new List<string>(StorageManager.Instance.GetStorage<StorageHome>().SummerWatermelonBread.Keys);
        for (int i = keys.Count - 1; i >= 0; i--)
        {
            var storage = StorageManager.Instance.GetStorage<StorageHome>().SummerWatermelonBread[keys[i]];
            if (IsActivityStorageEnd(storage) && storage.UnCollectRewards.Count==0 && storage.UnSetRewards.Count==0)
            {
                StorageManager.Instance.GetStorage<StorageHome>().SummerWatermelonBread.Remove(keys[i]);
            }
        }
    }
    public static List<ResData> GetAllUnCollectRewards()
    {
        var unCollectRewardsList = new List<ResData>();
        List<string> keys = new List<string>(StorageManager.Instance.GetStorage<StorageHome>().SummerWatermelonBread.Keys);
        for (int i = keys.Count - 1; i >= 0; i--)
        {
            var storage = StorageManager.Instance.GetStorage<StorageHome>().SummerWatermelonBread[keys[i]];
            
            if (IsActivityStorageEnd(storage))
            {
                foreach (var id in storage.UnSetRewards)
                {
                    if (!storage.UnCollectRewards.ContainsKey(id))
                    {
                        storage.UnCollectRewards.Add(id,0);
                    }
                    storage.UnCollectRewards[id]++;
                }
                storage.UnSetRewards.Clear();
                foreach (var pair in storage.UnCollectRewards)
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
    public static bool CanShowUnCollectRewardsUI()
    {
        if (!APIManager.Instance.HasNetwork)
            return false;
        var allUnCollectRewards = GetAllUnCollectRewards();
        if (allUnCollectRewards.Count > 0)
        {
            var reasonArgs = new GameBIManager.ItemChangeReasonArgs(){reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.SummerGet};
            var unCollectRewards = allUnCollectRewards;
            CommonRewardManager.Instance.PopActivityUnCollectReward(unCollectRewards, reasonArgs, null, () =>
            {
                foreach (var reward in unCollectRewards)
                {
                    if (!UserData.Instance.IsResource(reward.id))
                    {
                        GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs()
                        {
                            MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonSummerGet,
                            itemAId = reward.id,
                            isChange = true,
                        });
                    }
                }
                CleanUnCollectRewards();
                RemoveUselessStorage();
            });
            return true;
        }
        return false;
    }
    
    public List<int> GetRewardsOnMerge(TableMergeItem mergeNewConfig)
    {
        if (!IsOpened())
            return null;
        if (!IsSummerWatermelonBreadItemId(mergeNewConfig.id))
            return null;
        GameBIManager.Instance.SendGameEvent(
            BiEventAdventureIslandMerge.Types.GameEventType.GameEventSummerMergemelon,
            data1: mergeNewConfig.level.ToString());
        var rewards = new List<int>();
        if (SummerWatermelonBreadMergeRewardConfig.TryGetValue(mergeNewConfig.level, out var rewardConfig))
        {
            if (rewardConfig.RewardId != null)
            {
                for (var i = 0; i < rewardConfig.RewardId.Count; i++)
                {
                    var rewardId = rewardConfig.RewardId[i];
                    var rewardCount = rewardConfig.RewardNum[i];
                    for (var j = 0; j < rewardCount; j++)
                    {
                        rewards.Add(rewardId);
                    }
                }   
            }   
        }
        return rewards;
    }

    public void AddUnCollectRewards(int rewardId)
    {
        if (!UnCollectRewards.ContainsKey(rewardId))
        {
            UnCollectRewards.Add(rewardId,0);
        }
        UnCollectRewards[rewardId]++;
    }

    public void RemoveUnCollectRewards(int rewardId)
    {
        if (!UnCollectRewards.ContainsKey(rewardId))
        {
            Debug.LogError("未找到id="+rewardId+"的未领取奖励");
            return;
        }
        UnCollectRewards[rewardId]--;
        if (UnCollectRewards[rewardId] <= 0)
        {
            UnCollectRewards.Remove(rewardId);
        }
    }
    public void OnUseItem(int id)
    {
        if (!IsSummerWatermelonBreadItemId(id))
        {
            RemoveUnCollectRewards(id);   
        }
    }
    public async void OnNewItem(int id,RefreshItemSource source)
    {
        var itemConfig = GameConfigManager.Instance.GetItemConfig(id);
        if (itemConfig == null)
            return;
        if (!IsSummerWatermelonBreadItemId(itemConfig.id))
        {
            AddUnCollectRewards(id);
            if (source == RefreshItemSource.mergeOk)
            {
                if (itemConfig.pre_level > 0)
                {
                    RemoveUnCollectRewards(itemConfig.pre_level);
                    RemoveUnCollectRewards(itemConfig.pre_level);
                }
            }
            return;   
        }
        if (itemConfig.level <= StorageSummerWatermelonBread.MaxUnlockLevel)
            return;
        var rewardsList = new List<ResData>();
        for (var i = StorageSummerWatermelonBread.MaxUnlockLevel + 1; i <= itemConfig.level; i++)
        {
            if (SummerWatermelonBreadFirstTimeRewardConfig.TryGetValue(i, out var rewardConfig))
            {
                if (rewardConfig.RewardId != null)
                {
                    for (var j = 0; j < rewardConfig.RewardId.Count; j++)
                    {
                        rewardsList.Add(new ResData(rewardConfig.RewardId[j],rewardConfig.RewardNum[j]));   
                    }
                }   
            }
        }
        StorageSummerWatermelonBread.MaxUnlockLevel = itemConfig.level;
        if (itemConfig.next_level < 0)
        {
            GameBIManager.Instance.SendGameEvent(
                BiEventAdventureIslandMerge.Types.GameEventType.GameEventSummerPrize);   
        }

        MainView?.LockBoard();
        await MainView?.GrowProgressView();
        MainView?.UnLockBoard();
        
        if (StorageSummerWatermelonBread.MaxUnlockLevel > 3)
        {
            var task = new TaskCompletionSource<bool>();
            OpenNewLevelPopup(itemConfig,task);
            await task.Task;
        }
        
        if (rewardsList.Count > 0)
        {
            MainView?.LockBoard();
            MergeManager.Instance.Refresh((int)MergeBoardEnum.Main);
            var taskRewardCollect = new TaskCompletionSource<bool>();
            for (int i = 0; i < rewardsList.Count; i++)
            {
                var reward = rewardsList[i];
                GameBIManager.Instance.SendGameEvent(
                    BiEventAdventureIslandMerge.Types.GameEventType.GameEventSummerGetitem,data1:reward.id.ToString(),data2:reward.count.ToString());
            }
            var reasonArgs = new GameBIManager.ItemChangeReasonArgs(){reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.SummerGet};
            CommonRewardManager.Instance.PopCommonReward(rewardsList, CurrencyGroupManager.Instance.currencyController,
                true, reasonArgs, () =>
                {
                    for (int i = 0; i < rewardsList.Count; i++)
                    {
                        if (!UserData.Instance.IsResource(rewardsList[i].id))
                        {
                            for (var j = 0; j < rewardsList[i].count; j++)
                            {
                                GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                                {
                                    MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonSummerGet,
                                    isChange = false,
                                    itemAId = rewardsList[i].id
                                });   
                            }
                        }
                    }
                    taskRewardCollect.SetResult(true);
                });
            await taskRewardCollect.Task;
            MergeManager.Instance.Refresh(MergeBoardEnum.SummerWatermelonBread);
            MainView?.UnLockBoard();
        }
    }
    public override bool CanDownLoadRes()
    {
        return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.SummerWatermelonBread);
    }
    public string GetAuxItemAssetPath()
    {
        return "Prefabs/Activity/SummerWatermelonBread/Aux_SummerWatermelonBread";
    }
    public string GetTaskItemAssetPath()
    {
        return "Prefabs/Activity/SummerWatermelonBread/TaskList_SummerWatermelonBread";
    }
    
}