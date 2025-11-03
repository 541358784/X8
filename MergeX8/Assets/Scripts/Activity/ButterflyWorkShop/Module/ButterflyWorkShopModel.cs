using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DragonPlus;
using DragonPlus.Config.ButterflyWorkShop;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using SomeWhere;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public partial class ButterflyWorkShopModel:ActivityEntityBase
{
    public static bool InGuideChain = false;
    public const int BOARD_WIDTH = 5;
    public const int BOARD_HEIGHT = 2;

    private static ButterflyWorkShopModel _instance;
    public static ButterflyWorkShopModel Instance => _instance ?? (_instance = new ButterflyWorkShopModel());
    
    public override string Guid => "OPS_EVENT_TYPE_BUTTERFLY_WORKSHOP";
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitAuto()
    {
        Instance.Init();
    }
    public StorageButterflyWorkShop StorageButterflyWorkShop
    {
        get
        {
            var storage = StorageManager.Instance.GetStorage<StorageHome>().ButterflyWorkShop;
            if (!storage.ContainsKey(StorageKey))
            {
                var newStorage = new StorageButterflyWorkShop();
                newStorage.PayLevelGroup = PayLevelModel.Instance.GetCurPayLevelConfig().ButterFlyGroupId;
                storage.Add(StorageKey,newStorage);
                foreach (var itemLevel in ButterflyWorkShopConfig.InitItems)
                {
                    newStorage.UnSetItems.Add(GetMergeItemConfig(itemLevel).id);
                }
                UnSetItemsCount = newStorage.UnSetItems.Count;
                if (IsInitFromServer())
                {
                    newStorage.StartActivityTime = (long)StartTime;
                    newStorage.ActivityEndTime = (long)EndTime;
                }
            }
            return storage[StorageKey];
        }
    }
    

  
    public TableMergeLine MergeLineConfig => GameConfigManager.Instance.GetMergeLine(ButterflyWorkShopConfig.LineId);
    private Dictionary<int, TableMergeItem> _mergeItemDictionary;
    public Dictionary<int, TableMergeItem> MergeItemDictionary
    {
        get
        {
            if (_mergeItemDictionary == null)
            {
                _mergeItemDictionary = new Dictionary<int, TableMergeItem>();
                var items = GameConfigManager.Instance.GetMergeInLineItems(ButterflyWorkShopConfig.LineId);
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

    public StorageList<int> UnSetItems => StorageButterflyWorkShop.UnSetItems;
    
    public int UnSetItemsCount = 0;//用于红点的显示
    public bool IsStart => IsOpened() && StorageButterflyWorkShop.IsStart;
    public override bool IsOpened(bool hasLog = false)
    {
        return UnlockManager.IsOpen(UnlockManager.MergeUnlockType.ButterflyWorkShop) //已解锁
               && base.IsOpened(hasLog);
    }
    public const ulong Offset = 0 * XUtility.Hour;
    public int CurDay => (int)((CurTime-Offset) / XUtility.DayTime);
    public ulong CurTime => APIManager.Instance.GetServerTime();

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
    
    public void TryProductActivityItem(int index,int id,MergeBoard board,int doubleEnergyTimes)//尝试生成活动棋子
    {
        if (!IsStart)
            return;
    
        if (StorageButterflyWorkShop.DayId != CurDay)
        {
            StorageButterflyWorkShop.DayProductCount = 0;
            StorageButterflyWorkShop.DayId = CurDay;
        }
        var totalWeightMulti = GetProductAttenuationConfig(StorageButterflyWorkShop.DayProductCount);
        DebugUtil.Log("蝴蝶产出计数:"+StorageButterflyWorkShop.DayProductCount+" 权重系数:"+totalWeightMulti);

        var cfg = ButterflyWorkShopConfig;
        var randomWeight = Random.Range(0, ButterflyWorkShopConfig.MaxWeight);
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
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventButterflyGet);
        UnSetItems.Add(newItemConfig.id);
        FlyProductItem(newItemConfig.id, board.GetGridPosition(index),
            MergeTaskTipsController.Instance._MergeButterflyWorkShop.transform, 0.8f, true,
            () =>
            {
                UnSetItemsCount++;
            });
    }

    private static GameObject _flyItemObj;
    public static GameObject FlyItemObj
    {
        get
        {
            if (_flyItemObj == null)
            {
                var prefab = ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/Activity/ButterflyWorkShop/FlyItem");
                _flyItemObj = GameObject.Instantiate(prefab);
            }
            return _flyItemObj;
        }
    }
    public static void FlyProductItem(int itemId, Vector2 srcPos,Transform starTransform, float time, bool showEffect, Action action = null)
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
    private static GameObject _butterFly;
    public static GameObject ButterFly
    {
        get
        {
            if (_butterFly == null)
            {
                var prefab = ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/Activity/ButterflyWorkShop/ButterFly");
                _butterFly = GameObject.Instantiate(prefab,UIRoot.Instance.transform);
            }
            return _butterFly;
        }
    }
    public static void FlyButterFly(int index, Vector2 srcPos,Vector3 targetPos, float time, bool showEffect, Action action = null)
    {
        var sg=ButterFly.transform.Find("Spine").GetComponent<SkeletonGraphic>();
        var skeleton = sg.Skeleton;
        var skin = skeleton.Data.FindSkin(Instance.GetButterFlyType(index).ToString());
        if (skin != null)
        {
            sg.initialSkinName = Instance.GetButterFlyType(index).ToString();
            skeleton.SetSkin(skin);
            skeleton.SetSlotsToSetupPose();
            sg.Initialize(true);
            sg.Update(0); // 确保更新SkeletonGraphic
            sg.AnimationState.Apply(skeleton);
            // 强制刷新SkeletonGraphic
            sg.canvasRenderer.SetMaterial(sg.material, null);
            sg.canvasRenderer.SetMesh(sg.GetLastMesh());

        }
        
        float delayTime = 0f;
        Vector3 position = targetPos;
        ButterFly.SetActive(true);
        FlyGameObjectManager.Instance.FlyObjectUpStraight(ButterFly, srcPos, position, showEffect, time, 0f,delayTime, () =>
        {
            FlyGameObjectManager.Instance.PlayHintStarsEffect(position);
            ShakeManager.Instance.ShakeLight();
            action?.Invoke();
        },controlY:0.5f);
        ButterFly.SetActive(false);
    }
    
    public override void InitFromServerData(string activityId, string activityType, ulong startTime, ulong endTime,
        ulong rewardEndTime, bool manualEnd, string configJson, string activitySubType)
    {
        
        ButterflyWorkShopConfigManager.Instance.InitConfig(configJson);
        base.InitFromServerData(activityId, activityType, startTime, endTime, rewardEndTime, manualEnd, configJson,
            activitySubType);
        InitServerDataFinish();
        UnSetItemsCount =UnSetItems.Count;
        DebugUtil.Log($"InitConfig:{Guid}");
        RemoveUselessStorage();
    }

    protected override void InitServerDataFinish()
    {
        base.InitServerDataFinish();
        StorageButterflyWorkShop.StartActivityTime = (long)StartTime;
        StorageButterflyWorkShop.ActivityEndTime = (long)EndTime;
    }

    public bool IsClaimed(int index)
    {
        return StorageButterflyWorkShop.ClaimedItem.Contains(index);
    }
    
    public void OpenMainPopup()
    {
        var mainPopup = UIManager.Instance.OpenUI(MainUIPath) as UIButterflyWorkShopMainController;
    }

    public void PurchaseSuccess(TableShop tableShop)
    {
        var config =ButterflyWorkShopPackageConfigList.Find(a => a.ShopId == tableShop.id);
        // MergeLineConfig.output.Contains()
        for (int i = 0; i < config.RewardId.Count; i++)
        {
            if (MergeLineConfig.output.Contains(config.RewardId[i]))
            {
                for (int j = 0; j < config.RewardCount[i]; j++)
                {
                    UnSetItems.Add(config.RewardId[i]);
                    UnSetItemsCount++;
                }
            }
            else
            {
                UserData.Instance.AddRes(config.RewardId[i],config.RewardCount[i],new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.Iap));
            }
        }
        var res= CommonUtils.FormatReward(config.RewardId, config.RewardCount);
        EventDispatcher.Instance.DispatchEvent(EventEnum.PURCHASE_SUCCESS_REWARD, res);
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventTreasurehuntHammerGet,
            config.RewardCount[0].ToString(),"purchase");
        PopReward(res);
    }
    public void PopReward(List<ResData> listResData)
    {
        if (listResData == null || listResData.Count <= 0)
            return;
        int count = listResData.Count > 8 ? 8 : listResData.Count;
        var list = listResData.GetRange(0, count);
        listResData.RemoveRange(0, count);
        var reasonArgs = new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.Iap);
        CommonRewardManager.Instance.PopCommonReward(list, CurrencyGroupManager.Instance.GetCurrencyUseController(), false, reasonArgs, animEndCall:
            () =>
            {
                PopReward(listResData);
                EventDispatcher.Instance.DispatchEvent(EventEnum.BUTTERFLY_WORKSHOP_PURCHASE);
            });
    }
    public static UIButterflyWorkShopMainController MainView =>
        UIManager.Instance.GetOpenedUIByPath<UIButterflyWorkShopMainController>(Instance.MainUIPath);

    public virtual string MainUIPath => UINameConst.UIButterflyWorkShopMain;
    
    private static string coolTimeKey = "ButterflyWorkShop";
    public static bool CanShowUI()
    {
        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.ButterflyWorkShop))
            return false;

        if (!Instance.IsOpened())
            return false;
        if(!Instance.IsStart)
            StorageManager.Instance.GetStorage<StorageGame>().MergeBoards.Remove((int) MergeBoardEnum.ButterflyWorkShop);

        Instance.StorageButterflyWorkShop.IsStart = true;
        if (Instance.IsOpened() && Instance.IsStart &&
            (SceneFsm.mInstance.GetCurrSceneType() == StatusType.BackHome || SceneFsm.mInstance.GetCurrSceneType() == StatusType.Home) && 
            !GuideSubSystem.Instance.IsShowingGuide() && !GuideSubSystem.Instance.isFinished(GuideTriggerPosition.ButterFlyWorkShopStart))
        {
            GuideSubSystem.Instance.Trigger(GuideTriggerPosition.ButterFlyWorkShopStart, null);
        }
        else
        {
            if (!CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, coolTimeKey))
            {
                UIManager.Instance.OpenUI(UINameConst.UIButterflyWorkShopMain);
                CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey, CommonUtils.GetTimeStamp());
                return true;
            }
        }
       
        return false;
    }
    
    static bool IsActivityStorageEnd(StorageButterflyWorkShop storage)
    {
        return (long) APIManager.Instance.GetServerTime() >= storage.ActivityEndTime ||
               (long) APIManager.Instance.GetServerTime() < storage.StartActivityTime;
    }
   
    public static void RemoveUselessStorage()
    {
        List<string> keys = new List<string>(StorageManager.Instance.GetStorage<StorageHome>().ButterflyWorkShop.Keys);
        for (int i = keys.Count - 1; i >= 0; i--)
        {
            var storage = StorageManager.Instance.GetStorage<StorageHome>().ButterflyWorkShop[keys[i]];
            if (IsActivityStorageEnd(storage) )
            {
                StorageManager.Instance.GetStorage<StorageHome>().ButterflyWorkShop.Remove(keys[i]);
            }
        }

    }

    public StageRewardConfig GetStageRewardConfig()
    {
        var stageRewardConfigList = StageRewardConfigList;
        if (StorageButterflyWorkShop.Stage >= stageRewardConfigList.Count)
        {
            var list = stageRewardConfigList.FindAll(a => a.Loop == true);
            var cout=StorageButterflyWorkShop.Stage - stageRewardConfigList.Count;
            var index = cout % list.Count;
            return list[index];
        }

        return stageRewardConfigList[StorageButterflyWorkShop.Stage];
    }

    public int GetButterFlyType(int index)
    {
        var config = ButterflyWorkShopRewardConfigList
            .Find(a => a.Reward.Contains(index));
        if (config == null)
            return 1;
        return config.Id;
    }
  
    public int GetRandomIndex()
    {
        var butterflyRandomConfigList = ButterflyRandomConfigList;
        var butterflyWorkShopRewardConfigList = ButterflyWorkShopRewardConfigList;
        if (StorageButterflyWorkShop.RandomLine == 0)
        {
            StorageButterflyWorkShop.RandomLine=butterflyRandomConfigList.RandomPickOne().Id;
        }

        var config = butterflyRandomConfigList.Find(a =>
            a.Id == StorageButterflyWorkShop.RandomLine);

        var type = config.Order[StorageButterflyWorkShop.MaxUnlockLevel];
        var rewardConfig = butterflyWorkShopRewardConfigList.Find(a => a.Id == type);
        List<int> temp = new List<int>();
        for (int i = 0; i < rewardConfig.Reward.Count; i++)
        {
            if (!IsClaimed(rewardConfig.Reward[i]))
            {
                temp.Add(rewardConfig.Reward[i]);
            }
        }

        if (temp.Count <= 0)
        {
            temp.AddRange(rewardConfig.Reward);
        }
        
        return  temp.RandomPickOne();
    }

    public bool IsFinish()
    {
        if (ButterflyWorkShopConfig.RewardId.Count <= StorageButterflyWorkShop.ClaimedItem.Count)
            return true;
        return false;
    }

    public bool IsInUseItem=false;
    public async void OnUseItem(int index,int id)
    {
        DebugUtil.Log("OnUseItem----?"+id);
        if (id != 2205105)
            return;
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.ButterFlyUse,null);
        MainView?.LockBoard();
        IsInUseItem = true;
        StorageButterflyWorkShop.StageStore ++;

        var rewardsList = new List<ResData>();
        var targetIndex = GetRandomIndex();
        var reasonArgs = new GameBIManager.ItemChangeReasonArgs(){reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.ButterFlyGet};
        await MainView?.GrowProgressView(id,index,targetIndex);
        if (!StorageButterflyWorkShop.ClaimedItem.Contains(targetIndex))
        {
            rewardsList.Add(new ResData(ButterflyWorkShopConfig.RewardId[targetIndex-1],ButterflyWorkShopConfig.RewardNum[targetIndex-1]));
            StorageButterflyWorkShop.ClaimedItem.Add(targetIndex);
        }
        UserData.Instance.AddRes(rewardsList,reasonArgs);
        var stageRewardConfig = GetStageRewardConfig();
        if (StorageButterflyWorkShop.StageStore >= stageRewardConfig.Score)
        {
            UnSetItems.Add(stageRewardConfig.RewardId[0]);
            UnSetItemsCount++;
            StorageButterflyWorkShop.StageStore = 0;
            StorageButterflyWorkShop.Stage++;
            MainView?.FlyOtherReward(stageRewardConfig.RewardId[0]);
        
        }
        
        MainView?.RefreshProgress();
        MainView?.UnLockBoard();
       
        StorageButterflyWorkShop.MaxUnlockLevel ++;
     
         if (rewardsList.Count > 0)
         {
             Dictionary<string, string> dictionary = new Dictionary<string, string>();
             dictionary.Add("round",(ButterflyWorkShopModel.Instance.StorageButterflyWorkShop.Level+1).ToString());
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventButterflyUse,
                "1",StorageButterflyWorkShop.Stage.ToString(),stageRewardConfig.RewardId[0].ToString(),extras:dictionary);
            MainView?.LockBoard();
            MergeManager.Instance.Refresh((int)MergeBoardEnum.Main);
            var taskRewardCollect = new TaskCompletionSource<bool>();
            CommonRewardManager.Instance.PopCommonReward(rewardsList, CurrencyGroupManager.Instance.currencyController,
                false, reasonArgs, () =>
                {
                    taskRewardCollect.SetResult(true);
                    MainView?.RefreshProgress();
                    CheckFinish();
                });
            await taskRewardCollect.Task;
            MergeManager.Instance.Refresh(MergeBoardEnum.ButterflyWorkShop);
            MainView?.UnLockBoard();
            IsInUseItem = false;
         }else
         {
             IsInUseItem = false;
             Dictionary<string, string> dictionary = new Dictionary<string, string>();
             dictionary.Add("round",(ButterflyWorkShopModel.Instance.StorageButterflyWorkShop.Level+1).ToString());
             GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventButterflyUse,"0",extras:dictionary);
         }
    }

    public void CheckFinish()
    {
        if (IsFinish())
        {
            UIManager.Instance.OpenUI(UINameConst.UIPopupButterflyWorkTip);
            StorageButterflyWorkShop.Level++;
            StorageButterflyWorkShop.ClaimedItem.Clear();
            StorageButterflyWorkShop.RandomLine = 0;
            StorageButterflyWorkShop.MaxUnlockLevel = 0;
            MainView?.RefreshProgress();
        }
    }
    
    public async void OnNewItem(int index,int id,RefreshItemSource source)
    {
        if (MergeLineConfig.output.LastOrDefault() == id)
        {
            var grid = MainView.mergeBoard.Grids[index];
            if (grid.board)
            {
                GuideSubSystem.Instance.RegisterTarget(GuideTargetType.ButterFlyUse, grid.board.image_icon.transform as RectTransform);
                GuideSubSystem.Instance.Trigger(GuideTriggerPosition.ButterFlyUse,null);   
            }
        }
    }
    public Transform GetFlyTarget()
    {
        return MainView?._newItemBtn.transform;
    }
    public override bool CanDownLoadRes()
    {
        return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.ButterflyWorkShop);
    }
}