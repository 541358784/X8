using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ABTest;
using ActivityLocal.KeepPet.UI;
using DG.Tweening;
using Dlugin;
using DragonPlus;
using DragonPlus.Config.KeepPet;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Dynamic;
using Gameplay;
using Merge.Order;
using UnityEngine;

public partial class KeepPetModel : Manager<KeepPetModel>
{
    protected override void InitImmediately()
    {
        TMatch.Timer.Register(1, UpdateTime, null, true);
        RegisterDailyTaskEvent();
    }

    public void UpdateTime()
    {
        if (!IsOpen())
            return;
        if (CurState == null)
            return;
        UpdateGiftBagTimeState();
        CheckStateChange();
    }

    private bool LastOpenState = false;

    // public List<KeepPetStoreConfig> PowerPropStoreConfig;
    public KeepPetGlobalConfig GlobalConfig;
    public List<KeepPetSearchTaskConfig> SearchTaskConfig;
    public List<KeepPetLevelConfig> LevelConfig;
    public Dictionary<int, List<KeepPetSearchTaskRewardPoolConfig>> SearchTaskRewardPool;
    public Dictionary<int, KeepPetBuildingHangPointConfig> BuildingHangPointConfig;
    public Dictionary<int, KeepPetBuildingItemConfig> BuildingItemConfig;

    
    private Dictionary<int, List<KeepPetDailyTaskConfig>> _keepPetDailyTaskMap = new Dictionary<int, List<KeepPetDailyTaskConfig>>();
    private Dictionary<int, List<KeepPetOrderConfig>> _keepPetOrderMap = new Dictionary<int, List<KeepPetOrderConfig>>();
    
    public void InitConfig()
    {
        KeepPetConfigManager.Instance.InitConfig();
        // return;

        LastOpenState = IsOpen();
        if (!LastOpenState)
        {
            EventDispatcher.Instance.AddEventListener(EventEnum.OnLevelUp,OnLevelUp);
        }
        else
        {
            InitKeepPet();
        }
    }

    private void InitKeepPet()
    {
        GlobalConfig = KeepPetConfigManager.Instance.GetConfig<KeepPetGlobalConfig>()[0];
        SearchTaskConfig = KeepPetConfigManager.Instance.GetConfig<KeepPetSearchTaskConfig>();
        LevelConfig = KeepPetConfigManager.Instance.GetConfig<KeepPetLevelConfig>();
        SearchTaskRewardPool = new Dictionary<int, List<KeepPetSearchTaskRewardPoolConfig>>();
        var rewardConfigList = KeepPetConfigManager.Instance.GetConfig<KeepPetSearchTaskRewardPoolConfig>();
        foreach (var rewardConfig in rewardConfigList)
        {
            if (!SearchTaskRewardPool.TryGetValue(rewardConfig.Pool, out var pool))
            {
                pool = new List<KeepPetSearchTaskRewardPoolConfig>();
                SearchTaskRewardPool.Add(rewardConfig.Pool, pool);
            }

            pool.Add(rewardConfig);
        }

        BuildingHangPointConfig = new Dictionary<int, KeepPetBuildingHangPointConfig>();
        var buildingHangPointConfigList = KeepPetConfigManager.Instance.GetConfig<KeepPetBuildingHangPointConfig>();
        foreach (var buildingHangPointConfig in buildingHangPointConfigList)
        {
            BuildingHangPointConfig.Add(buildingHangPointConfig.Id,buildingHangPointConfig);
        }
        
        BuildingItemConfig = new Dictionary<int, KeepPetBuildingItemConfig>();
        var buildingItemConfigList = KeepPetConfigManager.Instance.GetConfig<KeepPetBuildingItemConfig>();
        foreach (var buildingItemConfig in buildingItemConfigList)
        {
            BuildingItemConfig.Add(buildingItemConfig.Id,buildingItemConfig);
        }

        _keepPetOrderMap.Clear();
        foreach (var config in KeepPetConfigManager.Instance.KeepPetOrderConfigList)
        {
            if (!_keepPetOrderMap.ContainsKey(config.PayLevelGroup))
                _keepPetOrderMap[config.PayLevelGroup] = new List<KeepPetOrderConfig>();
            
            _keepPetOrderMap[config.PayLevelGroup].Add(config);
        }
        
        _keepPetDailyTaskMap.Clear();
        foreach (var config in KeepPetConfigManager.Instance.KeepPetDailyTaskConfigList)
        {
            if (!_keepPetDailyTaskMap.ContainsKey(config.PayLevelGroup))
                _keepPetDailyTaskMap[config.PayLevelGroup] = new List<KeepPetDailyTaskConfig>();
            
            _keepPetDailyTaskMap[config.PayLevelGroup].Add(config);
        }
        
        InitState();
        InitStorage();
        InitDailyTaskConfig();
    }
    public List<KeepPetOrderConfig> GetKeepPetOrderConfig()
    {
        int group = PayLevelGroup();

        if (_keepPetOrderMap.ContainsKey(group))
            return _keepPetOrderMap[group];

        return _keepPetOrderMap[_keepPetOrderMap.Keys.First()];
    }
    
    public List<KeepPetDailyTaskConfig> KeepPetDailyTaskConfig()
    {
        int group = PayLevelGroup();

        if (_keepPetDailyTaskMap.ContainsKey(group))
            return _keepPetDailyTaskMap[group];

        return _keepPetDailyTaskMap[_keepPetDailyTaskMap.Keys.First()];
    }
    
    public void OnLevelUp(BaseEvent evt)
    {
        if (!LastOpenState)
        {
            LastOpenState = IsOpen(true);
            if (LastOpenState)
            {
                InitKeepPet();
                
                Storage.CurPetState = (int) KeepPetStateEnum.Sleep;
                Storage.Exp = 0;
                Storage.DailyTaskRefreshTime = (long) APIManager.Instance.GetServerTime();
                Storage.Cure = true;
                // Storage.LastWakeUpTime = (long)APIManager.Instance.GetServerTime();
                Storage.LastWakeUpTime = 0;
                var curTime = APIManager.Instance.GetServerTime();
                var offset = (ulong)KeepPetModel.Instance.GlobalConfig.DogHungryTimeOffset * XUtility.Hour;
                var curDay =(int)((curTime-offset) / XUtility.DayTime);
                Storage.HungryDayId = curDay;
                CurState = Storage.GetCurState();
                CurState.Storage = Storage;
                CheckStateChange();
            }
        }
        if (LastOpenState)
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.OnLevelUp,OnLevelUp);
            return;
        }
    }

    private const string PetName = "Dog";
    private const string PetNameNew = "DogNew";
    public bool IsUnlock => UnlockManager.IsOpen(UnlockManager.MergeUnlockType.KeepPetDog);

    public bool IsOpen(bool isLevelUp = false)
    {
        if(isLevelUp)
            return IsUnlock && ABTestManager.Instance.IsOpenKeepPet();
        // Debug.LogError("KeepPet IsUnlock:"+IsUnlock+" IsAbOpen:"+ABTestManager.Instance.IsOpenKeepPet());
        return Storage != null && IsUnlock && ABTestManager.Instance.IsOpenKeepPet();
    }

    public StorageKeepPet Storage =>
        StorageManager.Instance.GetStorage<StorageHome>().KeepPet.TryGetValue(PetNameNew, out var storage)
            ? storage
            : null;

    public Dictionary<KeepPetStateEnum, KeepPetBaseState> StateDictionary =
        new Dictionary<KeepPetStateEnum, KeepPetBaseState>();

    public void InitState()
    {
        var subclassTypes = Assembly.GetAssembly(typeof(KeepPetBaseState)) // 获取包含基类的程序集
            .GetTypes() // 获取所有类型
            .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(KeepPetBaseState))); // 筛选子类

        // 创建每个子类的实例并执行操作
        foreach (var type in subclassTypes)
        {
            var instance = (KeepPetBaseState) Activator.CreateInstance(type); // 创建实例
            StateDictionary.TryAdd(instance.Enum, instance);
        }
    }

    public int PayLevelGroup()
    {
        if (Storage == null)
            return 0;

        return Storage.PayLevelGroup;
    }
    
    public void InitStorage()
    {
        if (Storage == null)
        {
            MainOrderManager.Instance.RemoveOrder(MainOrderType.KeepPet);
            var storage = new StorageKeepPet();
            if (StorageManager.Instance.GetStorage<StorageHome>().KeepPet.TryGetValue(PetName, out var oldStorage))
            {
                storage.SearchPropCount = oldStorage.SearchPropCount;
            }
            storage.CurPetState = (int) KeepPetStateEnum.Sleep;
            storage.Exp = 0;
            storage.DailyTaskRefreshTime = (long) APIManager.Instance.GetServerTime();
            storage.Cure = true;
            storage.PayLevelGroup = PayLevelModel.Instance.GetCurPayLevelConfig().KeepPetGroupId;
            // storage.LastWakeUpTime = (long)APIManager.Instance.GetServerTime();
            storage.LastWakeUpTime = 0;
            
            var curTime = APIManager.Instance.GetServerTime();
            var offset = (ulong)KeepPetModel.Instance.GlobalConfig.DogHungryTimeOffset * XUtility.Hour;
            var curDay =(int)((curTime-offset) / XUtility.DayTime);
            storage.HungryDayId = curDay;
            StorageManager.Instance.GetStorage<StorageHome>().KeepPet.Add(PetNameNew, storage);
        }

        foreach (var hangPoint in BuildingHangPointConfig)
        {
            if (hangPoint.Value.DefaultItem != 0 && !Storage.BuildingCollectState.ContainsKey(hangPoint.Value.DefaultItem))
            {
                Storage.BuildingCollectState.Add(hangPoint.Value.DefaultItem,true);
            }
            if (hangPoint.Value.DefaultItem != 0 && !Storage.BuildingActiveState.ContainsKey(hangPoint.Key))
            {
                Storage.BuildingActiveState.Add(hangPoint.Key,hangPoint.Value.DefaultItem);
                var itemConfig = BuildingItemConfig[hangPoint.Value.DefaultItem];
                if (!itemConfig.SkinName.IsEmptyString())
                {
                    Storage.SkinName = itemConfig.SkinName;
                }
            }
        }

        CurState = Storage.GetCurState();
        CurState.Storage = Storage;
        // CheckStateChange();
    }

    public KeepPetBaseState CurState;

    public void CheckStateChange()
    {
        var nextStateEnum = CurState.CheckStateChange();
        if (nextStateEnum != CurState.Enum)
        {
            var nextState = nextStateEnum.GetState();
            var oldEnum = CurState.Enum;
            var newEnum = nextState.Enum;
            CurState.QuitState();
            nextState.Storage = CurState.Storage;
            CurState = nextState;
            CurState.EnterState();
            EventDispatcher.Instance.SendEventImmediately(new EventKeepPetStateChange(oldEnum, newEnum));
            CheckStateChange();
        }
    }

    public List<ResData> GetSearchTaskRewards(int searchTaskId)
    {
        var rewards = new List<ResData>();
        var taskConfig = SearchTaskConfig.Find(a => a.Id == searchTaskId);
        if (taskConfig == null)
            return rewards;
        var rareAndBuildingCountIndex = Utils.RandomByWeight(taskConfig.RarePropAndBuildingWeight);
        var rareCount = taskConfig.RarePropCount[rareAndBuildingCountIndex];
        if (rareCount > taskConfig.RewardCount)
            rareCount = taskConfig.RewardCount;
        var buildingCount = taskConfig.BuildingCount[rareAndBuildingCountIndex];
        if (buildingCount > taskConfig.RewardCount - rareCount)
            buildingCount = taskConfig.RewardCount - rareCount;
        var itemCount = taskConfig.RewardCount - rareCount - buildingCount;
        var rareRewards = GetMultipleRewardFromPool(taskConfig.RarePropPool, rareCount);
        rewards.AddRange(rareRewards);
        var buildingRewards = GetMultipleRewardFromPool(taskConfig.BuildingPool, buildingCount);
        rewards.AddRange(buildingRewards);
        var itemRewards = GetMultipleRewardFromPool(taskConfig.ItemPool, itemCount);
        rewards.AddRange(itemRewards);
        return rewards;
    }

    public List<ResData> GetMultipleRewardFromPool(int poolId, int rewardCount)
    {
        var rewards = new List<ResData>();
        if (!SearchTaskRewardPool.TryGetValue(poolId, out var pool))
            return rewards;
        var weightList = new List<int>();
        var indexList = new List<KeepPetSearchTaskRewardPoolConfig>();
        for (var i = 0; i < pool.Count; i++)
        {
            weightList.Add(pool[i].Weight);
            indexList.Add(pool[i]);
        }

        if (rewardCount > pool.Count)
            rewardCount = pool.Count;
        for (var i = 0; i < rewardCount; i++)
        {
            var weightIndex = Utils.RandomByWeight(weightList);
            var index = indexList[weightIndex];
            weightList.RemoveAt(weightIndex);
            indexList.RemoveAt(weightIndex);
            var rewardData = new ResData(index.ItemId, 1);
            rewards.Add(rewardData);
            for (var i1 = 0; i1 < indexList.Count; i1++)
            {
                var config = indexList[i1];
                if (config.GroupId == index.GroupId)//groupId过滤
                {
                    indexList.RemoveAt(i1);
                    weightList.RemoveAt(i1);
                    i1--;
                }
            }
            
        }

        return rewards;
    }

    public int GetDogFrisbee()
    {
        return Storage.FrisbeeCount;
    }

    public int GetDogDrumstick()
    {
        return Storage.MedicineCount;
    }

    public int GetDogSteak()
    {
        return Storage.SearchPropCount;
    }

    public int GetDogHead()
    {
        return Storage.DogHeadCount;
    }

    public void AddDogFrisbee(int count, string reason)
    {
        if (count > 0 && Storage.FrisbeeCount >= GlobalConfig.MaxFrisbee)
        {
            Debug.LogError("飞盘到达上限");
            return;   
        }
        var oldValue = Storage.FrisbeeCount;
        Storage.FrisbeeCount += count;
        if (Storage.FrisbeeCount > GlobalConfig.MaxFrisbee)
            Storage.FrisbeeCount = GlobalConfig.MaxFrisbee;
        var newValue = Storage.FrisbeeCount;
        EventDispatcher.Instance.SendEventImmediately(new EventKeepPetFrisbeeCountChange(oldValue, newValue));
        if (count > 0)
            CheckFrisbeeEnoughLevelUpGuide();
    }

    public void AddDogDrumstick(int count, string reason)
    {
        var oldValue = Storage.MedicineCount;
        Storage.MedicineCount += count;
        var newValue = Storage.MedicineCount;
        EventDispatcher.Instance.SendEventImmediately(new EventKeepPetMedicineCountChange(oldValue, newValue));
        CheckFeedGuide();
    }

    public void AddDogSteak(int count, string reason)
    {
        var oldValue = Storage.SearchPropCount;
        Storage.SearchPropCount += count;
        var newValue = Storage.SearchPropCount;
        EventDispatcher.Instance.SendEventImmediately(new EventKeepPetSearchPropCountChange(oldValue, newValue));
    }

    public void AddDogHead(int count, string reason)
    {
        if (count > 0 && Storage.DogHeadCount >= GlobalConfig.MaxDogHead)
        {
            Debug.LogError("狗头到达上限");
            return;   
        }
        var oldValue = Storage.DogHeadCount;
        Storage.DogHeadCount += count;
        if (Storage.DogHeadCount > GlobalConfig.MaxDogHead)
            Storage.DogHeadCount = GlobalConfig.MaxDogHead;
        var newValue = Storage.DogHeadCount;
        EventDispatcher.Instance.SendEventImmediately(new EventKeepPetDogHeadChange(oldValue, newValue));
    }

    public void FeedDrumStick()
    {
        if (CurState.Enum != KeepPetStateEnum.Hunger)
            return;
        if (Storage.Cure)
            return;
        var useCount = 1;
        if (!UserData.Instance.CanAford(UserData.ResourceId.KeepPetDogDrumstick,useCount))
        {
            Debug.LogError("鸡腿不足");
            return;
        }
        AwakeDog();
        UserData.Instance.ConsumeRes(UserData.ResourceId.KeepPetDogDrumstick,useCount,
            new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.KeepPetUse));
        Storage.CheckHungry();
        Storage.Cure = true;
        CheckStateChange();
    }
    public void UseFrisbee()
    {
        if (CurState.Enum != KeepPetStateEnum.Happy)
            return;
        var useCount = 1;
        if (!UserData.Instance.CanAford(UserData.ResourceId.KeepPetDogFrisbee,useCount))
        {
            Debug.LogError("飞盘不足");
            return;
        }
        AwakeDog();
        UserData.Instance.ConsumeRes(UserData.ResourceId.KeepPetDogFrisbee,useCount,
            new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.KeepPetUse));
        var oldLevel = GetLevel();
        var oldValue = Storage.Exp;
        Storage.Exp += useCount * GlobalConfig.FrisbeeExpValue;
        var newValue = Storage.Exp;
        var newLevel = GetLevel();
        if (oldLevel != newLevel)
        {
            EventDispatcher.Instance.DispatchEventImmediately(EventEnum.OnKeepPetLevelUp, newLevel);
        }
        UpdateDailyTaskOnExpChange(oldValue,newValue);
        EventDispatcher.Instance.SendEventImmediately(new EventKeepPetExpChange(oldValue, newValue));
        CheckLevelUpBi(oldValue, newValue);
    }

    public void CheckLevelUpBi(int oldExp,int newExp)
    {
        var oldLevel = oldExp.KeepPetGetCurLevelConfig();
        var newLevel = newExp.KeepPetGetCurLevelConfig();
        if (newLevel.Id > oldLevel.Id)
        {
            GameBIManager.Instance.SendGameEvent(
                BiEventAdventureIslandMerge.Types.GameEventType.GameEventKeepPetLevelUp, newLevel.Id.ToString());
        }
    }
    public void AwakeDog()
    {
        Storage.LastWakeUpTime = (long)APIManager.Instance.GetServerTime();
        CheckStateChange();
    }

    public void PerformSearchTask(int searchTaskId)
    {
        if (CurState.Enum == KeepPetStateEnum.Searching || CurState.Enum == KeepPetStateEnum.SearchFinish)
            return;
        var taskConfig = SearchTaskConfig.Find(a => a.Id == searchTaskId);
        if (taskConfig == null)
            return;
        if (!UserData.Instance.CanAford((UserData.ResourceId) taskConfig.ConsumeType, taskConfig.ConsumeCount))
        {
            return;
        }
        UserData.Instance.ConsumeRes((UserData.ResourceId) taskConfig.ConsumeType,taskConfig.ConsumeCount,
            new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.KeepPetUse));
        Storage.SearchTaskId = taskConfig.Id;
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventKeepPetSearch,taskConfig.Id.ToString());
        CheckStateChange();
    }

    public Transform GetDogFrisbeeFlyTarget()
    {
        return GetCommonFlyTarget();
    }

    public Transform GetDogDrumstickFlyTarget()
    {
        return GetCommonFlyTarget();
    }

    public Transform GetDogSteakFlyTarget()
    {
        return GetCommonFlyTarget();
    }

    public Transform GetDogHeadFlyTarget()
    {
        return GetCommonFlyTarget();
    }

    public Transform GetCommonFlyTarget()
    {
        if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
        {
            var entrance = DynamicEntryManager.Instance.GetDynamicEntry<DynamicEntry_Game_KeepPet>();
            if (entrance)
                return entrance.transform;
            else
                return MergeMainController.Instance.rewardBtnTrans;
        }
        else
        {
            var auxItem = Storage.GetAuxItem();
            if (auxItem != null && auxItem.gameObject.activeInHierarchy)
                return auxItem.transform;
            else
                return UIHomeMainController.mainController.MainPlayTransform;
        }
    }

    public void OpenMainView(string source)
    {
        UIKeepPetMainController.Open(Storage);
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventKeepPetEnter, source);
    }
    public bool CanShowReturnPopup()
    {
        if (!ABTestManager.Instance.IsOpenKeepPet())
            return false;
        if (!IsOpen())
            return false;
        
        if (Storage.IsStart)
            return false;
        if (!GuideSubSystem.Instance.isFinished(3001))
            return false;
        Storage.IsStart = true;
        UIPopupKeepPetReturnController.Open();
        return true;
    }

    public void CheckDrumstickOrder()
    {
        if (!IsOpen())
            return;
        if (CurState.Enum == KeepPetStateEnum.Hunger && Storage.MedicineCount == 0)
            MainOrderCreateKeepPet.TryCreateOrder(SlotDefinition.KeepPet);
    }

    public bool IsUnlockThreeOneGift()
    {
        return Storage.Exp.KeepPetGetCurLevelConfig().Id >= GlobalConfig.ThreeOneUnLockLevel;
    }

    public bool ShowEntrance()
    {
        if (Storage == null)
            return false;

        return Storage.ShowEntrance();
    }
}