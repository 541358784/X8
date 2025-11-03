using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Activity.Base;
using Activity.FishCulture.View;
using Deco.Node;
using Deco.World;
using Decoration;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.CoinRush;
using DragonPlus.Config.FishCulture;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Dynamic;
using Gameplay;
using GamePool;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

public partial class FishCultureModel : ActivityEntityBase, I_ActivityStatus
{
    public bool ShowEntrance()
    {
        return IsStart();
    }
    private static FishCultureModel _instance;
    public static FishCultureModel Instance => _instance ?? (_instance = new FishCultureModel());

    public override string Guid => "OPS_EVENT_TYPE_FISH_CULTURE";


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitAuto()
    {
        Instance.Init();
    }

    public StorageFishCulture CurStorageFishCultureWeek =>
        StorageManager.Instance.GetStorage<StorageHome>().FishCulture;

    public void InitStorage()
    {
        if (ActivityId != CurStorageFishCultureWeek.ActivityId)
        {
            CurStorageFishCultureWeek.IsFinish = false;
            CurStorageFishCultureWeek.PayLevelGroup = PayLevelModel.Instance.GetCurPayLevelConfig().FishCultureGroupId;
            CurStorageFishCultureWeek.ActivityId = ActivityId;
            CurStorageFishCultureWeek.CurScore = 0;
            CurStorageFishCultureWeek.IsStart = false;
            CurStorageFishCultureWeek.IsEnd = false;
            CurStorageFishCultureWeek.TotalScore = 0;
            CurStorageFishCultureWeek.CollectList.Clear();
            
            
            var resMd5List = ActivityManager.Instance.GetActivityMd5List(ActivityId);
            CurStorageFishCultureWeek.ActivityResList.Clear();
            CurStorageFishCultureWeek.ActivityResMd5List.Clear();
            foreach (var resMd5 in resMd5List)
            {
                CurStorageFishCultureWeek.ActivityResMd5List.Add(resMd5);
                var resPath = ActivityResHotUpdate.GetFilePath(resMd5);
                CurStorageFishCultureWeek.ActivityResList.Add(resPath);
            }
        }
        CurStorageFishCultureWeek.StartTime = (long)StartTime;
        CurStorageFishCultureWeek.PreheatCompleteTime = (long)StartTime + PreheatTime;
        CurStorageFishCultureWeek.EndTime = (long)EndTime;
        CurStorageFishCultureWeek.PreEndTime = (long)EndTime - PreEndTime;
        FishCultureLeaderBoardModel.Instance.CreateStorage(CurStorageFishCultureWeek);
    }
    
    
    public FishCultureGlobalConfig GlobalConfig => FishCultureConfigManager.Instance.GetConfig<FishCultureGlobalConfig>()[0];
    public long PreheatTime=> IsSkipActivityPreheating()?0:(long)((ulong)GlobalConfig.PreheatTime * XUtility.Hour);
    public long PreEndTime=> (long)((ulong)GlobalConfig.PreEndTime * XUtility.Hour);
    private static void InitTable<T>(Dictionary<int, T> config,List<T> tableData = null) where T : TableBase
    {
        if (config == null)
            return;
        if (tableData == null)
            tableData = FishCultureConfigManager.Instance.GetConfig<T>();
        if (tableData == null)
            return;

        config.Clear();
        foreach (T kv in tableData)
        {
            config.Add(kv.GetID(), kv);
        }
    }

    public List<FishCultureLeaderBoardRewardConfig> LeaderBoardRewardConfig => FishCultureLeaderBoardRewardConfigList;
    public List<FishCultureTaskRewardConfig> TaskRewardConfig => FishCultureTaskRewardConfigList;
    public List<FishCultureRewardConfig> LevelConfig => FishCultureRewardConfigList;
    
    
    public override async void InitFromServerData(string activityId, string activityType, ulong startTime, ulong endTime,
        ulong rewardEndTime, bool manualEnd, string configJson, string activitySubType)
    {
        FishCultureConfigManager.Instance.InitConfig(configJson);
        base.InitFromServerData(activityId, activityType, startTime, endTime, rewardEndTime, manualEnd, configJson,
            activitySubType);
        InitServerDataFinish();
        DebugUtil.Log($"InitConfig:{Guid}");
        await XUtility.WaitFrames(1);
        InitStorage();
        _lastActivityOpenState = IsStart();
        FishCultureLeaderBoardModel.Instance.InitFromServerData();
    }

    public bool IsUnlock => UnlockManager.IsOpen(UnlockManager.MergeUnlockType.FishCulture);

    public override bool IsOpened(bool hasLog = false)
    {
        return base.IsOpened(hasLog) && IsUnlock; //当前当前周的配置;
    }
    public bool IsPrivateOpened()
    {
        return IsOpened() && !CurStorageFishCultureWeek.IsTimeOut();
    }

    public bool IsStart()
    {
        return IsPrivateOpened() && APIManager.Instance.GetServerTime() > (ulong)CurStorageFishCultureWeek.PreheatCompleteTime && CurStorageFishCultureWeek.GetLeftPreEndTime() > 0;
    }

    public void AddScore(int addCount,string reason)
    {
        var oldValue = CurStorageFishCultureWeek.CurScore;
        CurStorageFishCultureWeek.CurScore += addCount;
        var newValue = CurStorageFishCultureWeek.CurScore;
        CurStorageFishCultureWeek.TotalScore += addCount;
        if (addCount > 0)
        {
            FishCultureLeaderBoardModel.Instance.GetLeaderBoardStorage(CurStorageFishCultureWeek.ActivityId)?.CollectStar(addCount);   
        }
        EventDispatcher.Instance.SendEventImmediately(new EventFishCultureScoreChange(oldValue,newValue));
        GameBIManager.Instance.SendGameEvent(
            BiEventAdventureIslandMerge.Types.GameEventType.GameEventFishCultureRadishChange,
            addCount.ToString(), newValue.ToString(), reason);
    }

    public int GetScore()
    {
        return CurStorageFishCultureWeek.CurScore;
    }
    

    public int GetTaskValue(StorageTaskItem taskItem, bool isMul)
    {
        if (!IsInitFromServer())
            return 0;
        int tempPrice = 0;
        for (var i = 0; i < taskItem.RewardTypes.Count; i++)
        {
            if (taskItem.RewardTypes[i] == (int)UserData.ResourceId.Coin || taskItem.RewardTypes[i] == (int)UserData.ResourceId.RareDecoCoin)
            {
                if(taskItem.RewardNums.Count > i)
                    tempPrice = taskItem.RewardNums[i];
                
                break;
            }
        }

        if (tempPrice == 0)
        {
            foreach (var itemId in taskItem.ItemIds)
            {
                tempPrice += OrderConfigManager.Instance.GetItemPrice(itemId);
            }
        }

        var value = 0;
        var configs = TaskRewardConfig;
        if (configs != null && configs.Count > 0)
        {
            foreach (var config in configs)
            {
                if (tempPrice <= config.Max_value)
                {
                    value = config.Output;
                    break;
                }
            }
        }
        return value;
    }
    
    private bool _lastActivityOpenState;//记录上一帧的活动开启状态，在轮询中判断是否触发开启活动或者关闭活动

    public FishCultureModel()
    {
        TMatch.Timer.Register(1, UpdateTime, null, true);
    }
    public void UpdateTime()
    {
        if (!IsInitFromServer())
            return;
        var currentActivityOpenState = IsStart();
        if (_lastActivityOpenState == currentActivityOpenState)
            return;
        if (!currentActivityOpenState)
        {
            if (UIFishCultureMainController.Instance)
                UIFishCultureMainController.Instance.AnimCloseWindow();
        }
        else
        {
            var preheatUI = UIManager.Instance.GetOpenedUIByPath<UIPopupFishCulturePreviewController>(UINameConst.UIPopupFishCulturePreview);
            if (preheatUI)
                preheatUI.AnimCloseWindow();
        }
        _lastActivityOpenState = currentActivityOpenState;
    }
    public static bool CanShowMainPopup()
    {
        if (Instance.IsStart())
        {
            UIFishCultureMainController.Open();
            return true;
        }
        return false;
    }

    public static bool CanShowPreheatPopup()
    {
        if (Instance.IsPrivateOpened() && Instance.CurStorageFishCultureWeek.GetPreheatLeftTime() > 0)
        {
            UIPopupFishCulturePreviewController.Open(Instance.CurStorageFishCultureWeek);
            return true;
        }
        return false;
    }

    public const string preheatCoolTimeKey = "FishCulturePreheat";
    public static bool CanShowPreheatPopupEachDay()
    {
        if (CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, preheatCoolTimeKey))
            return false;
        if (CanShowPreheatPopup())
        {
            CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, preheatCoolTimeKey,CommonUtils.GetTimeStamp());
            return true;
        }
        return false;
    }
    
    public static bool CanShowStartPopup()
    {
        if (Instance.IsStart() && !Instance.CurStorageFishCultureWeek.IsStart)
        {
            Instance.CurStorageFishCultureWeek.IsStart = true;
            // UIPopupFishCultureStartController.Open(Instance.CurStorageFishCultureWeek);
            UIFishCultureMainController.Open();
            return true;
        }
        return false;
    }
    
    public static bool CanShowFinishPopup()
    {
        if (Instance.IsPrivateOpened() && 
            Instance.CurStorageFishCultureWeek.GetLeftPreEndTime() <= 0 && 
            !Instance.CurStorageFishCultureWeek.IsEnd && 
            Instance.CurStorageFishCultureWeek.IsStart)
        {
            Instance.CurStorageFishCultureWeek.IsEnd = true;
            UIFishCultureMainController.Open();
            return true;
        }
        return false;
    }
    
    public override bool CanDownLoadRes()
    {
        return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.FishCulture);
    }
    

    public Transform GetCommonFlyTarget()
    {
        var storage = CurStorageFishCultureWeek;
        if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
        {
            var entrance = DynamicEntryManager.Instance.GetDynamicEntry<DynamicEntry_Game_FishCulture>();
            if (entrance)
                return entrance.transform;
            else
                return MergeMainController.Instance.rewardBtnTrans;
        }
        else
        {
            var auxItem = DynamicEntryManager.Instance.GetDynamicEntry<DynamicEntry_Home_FishCulture>();
            if (auxItem != null && auxItem.gameObject.activeInHierarchy)
                return auxItem.transform;
            else
                return UIHomeMainController.mainController.MainPlayTransform;
        }
    }

    public bool BuyFish(FishCultureRewardConfig fishConfig)
    {
        if (CurStorageFishCultureWeek.CurScore < fishConfig.Price)
            return false;
        AddScore(-fishConfig.Price,"BuyFish"+fishConfig.Fish);
        GameBIManager.Instance.SendGameEvent(
            BiEventAdventureIslandMerge.Types.GameEventType.GameEventFishCultureRadishExchange,
            fishConfig.Id.ToString(), fishConfig.Id.ToString(), CurStorageFishCultureWeek.CurScore.ToString());
        var reason = new GameBIManager.ItemChangeReasonArgs()
        {
            reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.FishCultureGet
        };
        var rewards = CommonUtils.FormatReward(fishConfig.RewardId, fishConfig.RewardNum);
        UserData.Instance.AddRes(rewards,reason);
        CurStorageFishCultureWeek.CollectList.Add(fishConfig.Id);
        if (GetNextFish() == null)
            CurStorageFishCultureWeek.IsFinish = true;//买完最后一条鱼视为活动完成
        EventDispatcher.Instance.SendEventImmediately(new EventFishCultureGetNewFish(fishConfig));
        return true;
    }

    public FishCultureRewardConfig GetNextFish()
    {
        var fishCount = CurStorageFishCultureWeek.CollectList.Count;
        if (fishCount >= LevelConfig.Count)
        {
            return null;
        }
        return LevelConfig[fishCount];
    }

    public bool ShowAuxItem()
    {
        if (CurStorageFishCultureWeek == null)
            return false;

        return FishCultureUtils.ShowAuxItem(CurStorageFishCultureWeek);
    }
    
    public bool ShowTaskEntrance()
    {
        if (CurStorageFishCultureWeek == null)
            return false;

        return CurStorageFishCultureWeek.ShowTaskEntrance();
    }

    public I_ActivityStatus.ActivityStatus GetActivityStatus()
    {
        if (CurStorageFishCultureWeek.IsFinish)
        {
            return I_ActivityStatus.ActivityStatus.Completed;
        }
        else if (CurStorageFishCultureWeek.TotalScore > 0)
        {
            return I_ActivityStatus.ActivityStatus.Incomplete;
        }
        else if (!CurStorageFishCultureWeek.ActivityId.IsEmptyString())
        {
            return I_ActivityStatus.ActivityStatus.NotParticipated;
        }
        return I_ActivityStatus.ActivityStatus.None;
    }
}