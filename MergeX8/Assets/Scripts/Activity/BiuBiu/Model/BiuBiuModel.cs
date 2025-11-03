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
using DragonPlus.Config.BiuBiu;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using GamePool;
using Newtonsoft.Json;
using SomeWhere;
using SRF;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public partial class BiuBiuModel : ActivityEntityBase
{
    public bool ShowEntrance()
    {
        return IsPrivateOpened();
    }
    private static BiuBiuModel _instance;
    public static BiuBiuModel Instance => _instance ?? (_instance = new BiuBiuModel());

    public override string Guid => "OPS_EVENT_TYPE_BIU_BIU";
    public const int BOARD_WIDTH = 5;
    public const int BOARD_HEIGHT = 2;
    public string GetAuxItemAssetPath()
    {
        return "Prefabs/Activity/BiuBiu/Aux_BiuBiu";
    }

    public string GetTaskItemAssetPath()
    {
        return "Prefabs/Activity/BiuBiu/TaskList_BiuBiu";
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitAuto()
    {
        Instance.Init();
    }
    public StorageBiuBiu Storage => StorageManager.Instance.GetStorage<StorageHome>().BiuBiu;

    public void InitStorage()
    {
        if (!IsInitFromServer())
            return;
        if (Storage.ActivityId != ActivityId)
        {
            StorageManager.Instance.GetStorage<StorageGame>().MergeBoards.Remove((int) MergeBoardEnum.BiuBiu);
            Storage.Clear();
            Storage.PayLevelGroup = PayLevelModel.Instance.GetCurPayLevelConfig().BiuBiuGroupId;
            Storage.ActivityId = ActivityId;
            Storage.IsStart = false;
            Storage.UnSetItems.Clear();
            foreach (var itemLevel in GlobalConfig.InitItems)
            {
                Storage.UnSetItems.Add(GetMergeItemConfig(itemLevel).id);
            }
            Storage.ShowState.Clear();
            Storage.Fate.Clear();
            Storage.Round = 1;
            LoadFateConfig();
            LoadRound();
        }
        Storage.StartTime = (long)StartTime;
        Storage.EndTime = (long)EndTime;
    }

    public void LoadRound()
    {
        if (!FateConfigDic.TryGetValue(Storage.Round, out var configs))
        {
            configs = FateConfigDic[FateConfigDic.Keys.ToList().Max()];
        }

        var weights = new List<int>();
        foreach (var config in configs)
        {
            weights.Add(config.Weight);
        }
        var randomConfig = configs[weights.RandomIndexByWeight()];
        Debug.LogError("飞镖随机配置"+randomConfig.Id);
        Storage.Fate.Clear();
        foreach (var fate in randomConfig.Fate)
        {
            Storage.Fate.Add(fate);
        }
        Storage.ShowState.Clear();
    }
    public TableMergeLine MergeLineConfig => GameConfigManager.Instance.GetMergeLine(GlobalConfig.LineId);
    private Dictionary<int, TableMergeItem> _mergeItemDictionary;
    public Dictionary<int, TableMergeItem> MergeItemDictionary
    {
        get
        {
            if (_mergeItemDictionary == null)
            {
                _mergeItemDictionary = new Dictionary<int, TableMergeItem>();
                var items = GameConfigManager.Instance.GetMergeInLineItems(GlobalConfig.LineId);
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
    private static void InitTable<T>(Dictionary<int, T> config) where T : TableBase
    {
        if (config == null)
            return;

        List<T> tableData = BiuBiuConfigManager.Instance.GetConfig<T>();
        if (tableData == null)
            return;

        config.Clear();
        foreach (T kv in tableData)
        {
            config.Add(kv.GetID(), kv);
        }
    }
    public BiuBiuGlobalConfig GlobalConfig => BiuBiuGlobalConfigList[0];
    public Dictionary<int, List<BiuBiuFateConfig>> FateConfigDic = new Dictionary<int, List<BiuBiuFateConfig>>();
    public Dictionary<int, BiuBiuUIConfig> UIConfigDic = new Dictionary<int, BiuBiuUIConfig>();
    public List<BiuBiuPackageConfig> PackageConfigList =>
        BiuBiuPackageConfigList;
    public override void InitFromServerData(string activityId, string activityType, ulong startTime, ulong endTime,
        ulong rewardEndTime, bool manualEnd, string configJson, string activitySubType)
    {
        DebugUtil.LogError("1");
        BiuBiuConfigManager.Instance.InitConfig(configJson);
        InitTable(UIConfigDic);
        base.InitFromServerData(activityId, activityType, startTime, endTime, rewardEndTime, manualEnd, configJson,
            activitySubType);
        DebugUtil.Log($"InitConfig:{Guid}");
        LoadFateConfig();
        InitStorage();
    }

    public void LoadFateConfig()
    {
        var fateConfigList = BiuBiuFateConfigList;
        foreach (var fateConfig in fateConfigList)
        {
            if (!FateConfigDic.ContainsKey(fateConfig.Round))
            {
                FateConfigDic.Add(fateConfig.Round,new List<BiuBiuFateConfig>());
            }
            FateConfigDic[fateConfig.Round].Add(fateConfig);
        }
    }

    public bool IsUnlock => UnlockManager.IsOpen(UnlockManager.MergeUnlockType.BiuBiu);

    public override bool IsOpened(bool hasLog = false)
    {
        return base.IsOpened(hasLog) && IsUnlock; //当前当前周的配置;
    }
    public bool IsPrivateOpened()
    {
        return IsOpened() &&!Storage.IsTimeOut()/* && Storage.IsStart*/;
    }
    public override bool CanDownLoadRes()
    {
        return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.BiuBiu);
    }

    public Transform GetCommonFlyTarget()
    {
        var storage = Storage;
        if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
        {
            var entrance = storage.GetTaskEntrance();
            if (entrance)
                return entrance.transform;
            else
                return MergeMainController.Instance.rewardBtnTrans;
        }
        else
        {
            var auxItem = storage.GetAuxItem();
            if (auxItem != null && auxItem.gameObject.activeInHierarchy)
                return auxItem.transform;
            else
                return UIHomeMainController.mainController.MainPlayTransform;
        }
    }
    private static string coolTimeKey = "BiuBiu";
    public static bool CanShowStart()
    {
        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.BiuBiu))
            return false;
        if (!Instance.IsPrivateOpened())
            return false;
        Instance.Storage.IsStart = true;
        if (Instance.IsOpened() &&
            (SceneFsm.mInstance.GetCurrSceneType() == StatusType.BackHome || SceneFsm.mInstance.GetCurrSceneType() == StatusType.Home) && 
            !GuideSubSystem.Instance.IsShowingGuide() && !GuideSubSystem.Instance.isFinished(GuideTriggerPosition.BiuBiuStart))
        {
            GuideSubSystem.Instance.Trigger(GuideTriggerPosition.BiuBiuStart, null);
        }
        else
        {
            if (!CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, coolTimeKey))
            {
                UIBiuBiuMainController.Open();
                CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey, CommonUtils.GetTimeStamp());
                return true;
            }
        }
       
        return false;
    }
    
    public void PurchaseSuccess(TableShop tableShop)
    {
        var config =PackageConfigList.Find(a => a.ShopId == tableShop.id);
        if (config == null)
            return;
        var reasonArgs = new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.Iap);
        for (int i = 0; i < config.RewardId.Count; i++)
        {
            if (MergeLineConfig.output.Contains(config.RewardId[i]))
            {
                for (int j = 0; j < config.RewardCount[i]; j++)
                {
                    Storage.UnSetItems.Add(config.RewardId[i]);
                    GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventBiuBiuGet,config.RewardId[i].ToString());
                }
            }
            else
            {
                UserData.Instance.AddRes(config.RewardId[i],config.RewardCount[i],reasonArgs);
            }
        }
        var res= CommonUtils.FormatReward(config.RewardId, config.RewardCount);
        EventDispatcher.Instance.DispatchEvent(EventEnum.PURCHASE_SUCCESS_REWARD, res);
        CommonRewardManager.Instance.PopCommonReward(res, CurrencyGroupManager.Instance.GetCurrencyUseController(), false, reasonArgs, animEndCall:
            () =>
            {
                UIBiuBiuMainController.Instance?.RefreshBtnView();
            });
    }
    
    
    
    
    public void TryProductActivityItem(int index,int id,MergeBoard board,int doubleEnergyTimes)//尝试生成活动棋子
    {
        if (!IsPrivateOpened())
            return;

        var cfg = GlobalConfig;
        var randomWeight = Random.Range(0, GlobalConfig.MaxWeight);
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
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventBiuBiuGet,newItemConfig.id.ToString());
        Storage.UnSetItems.Add(newItemConfig.id);
        FlyProductItem(newItemConfig.id, board.GetGridPosition(index),
            MergeBiuBiu.Instance.transform, 0.8f, true,
            () =>
            {
                
            });
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
    
    private static GameObject _flyItemObj;
    public static GameObject FlyItemObj
    {
        get
        {
            if (_flyItemObj == null)
            {
                var prefab = ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/Activity/BiuBiu/FlyItem");
                _flyItemObj = GameObject.Instantiate(prefab);
            }
            return _flyItemObj;
        }
    }
    
    public async void OnUseItem(int index,int id)
    {
        DebugUtil.Log("OnUseItem----?"+id);
        if (id != 2206104)
            return;
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.BiuBiuUse,null);
        var result = Storage.Fate[0];
        var hasReward = result > 0;
        var rewardType = result > 0 ? result : -result;
        var uiConfig = UIConfigDic[rewardType];
        var rewards = CommonUtils.FormatReward(uiConfig.RewardId, uiConfig.RewardNum);
        var reason =
            new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.BiuBiuGet);
        if (hasReward)
        {
            UserData.Instance.AddRes(rewards,reason);
        }
        Storage.Fate.RemoveAt(0);
        var positions = new List<int>();
        foreach (var position in uiConfig.ShowIndex)
        {
            if (!Storage.ShowState.Contains(position))
            {
                positions.Add(position);
            }
        }
        if (positions.Count == 0)
        {
            Debug.LogError("未找到目标气球");
            return;
        }
        var targetPosition = positions.RandomPickOne();
        Storage.ShowState.Add(targetPosition);
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventBiuBiuUse,
            hasReward.ToString(),Storage.Round.ToString(), Storage.ShowState.Count.ToString());
        var isFinish = Storage.Fate.Count == 0;
        var mainUI = UIBiuBiuMainController.Instance;
        if (Storage.Fate.Count == 0)
        {
            Storage.Round++;
            LoadRound();
            if (mainUI)
                mainUI.LockBoard();
        }
        if (mainUI)
        {
            await mainUI.PerformBiuBiu(targetPosition, rewards, hasReward);
        }

        if (hasReward)
        {
            var task = new TaskCompletionSource<bool>();
            CommonRewardManager.Instance.PopCommonReward(rewards, CurrencyGroupManager.Instance.currencyController,
                false, reason, () =>
                {
                    task.SetResult(true);
                    if (mainUI)
                        mainUI.HideReward(targetPosition);
                });
            await task.Task;
        }

        if (isFinish)
        {
            var task = new TaskCompletionSource<bool>();
            UIPopupBiuBiuTipController.Open(()=>task.SetResult(true));
            await task.Task;
            if (mainUI)
                mainUI.ReloadShowState();
            if (mainUI)
                mainUI.UnLockBoard();
        }
    }
    public async void OnNewItem(int index,int id,RefreshItemSource source)
    {
        if (MergeLineConfig.output.LastOrDefault() == id)
        {
            var grid = UIBiuBiuMainController.Instance.mergeBoard.Grids[index];
            if (grid.board)
            {
                GuideSubSystem.Instance.RegisterTarget(GuideTargetType.BiuBiuUse, grid.board.image_icon.transform as RectTransform);
                GuideSubSystem.Instance.Trigger(GuideTriggerPosition.BiuBiuUse,null);   
            }
        }
    }
}