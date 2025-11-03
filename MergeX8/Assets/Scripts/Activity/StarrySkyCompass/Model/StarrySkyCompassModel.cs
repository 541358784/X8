using System;
using System.Collections.Generic;
using System.Linq;
using Activity.StarrySkyCompass.View;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.StarrySkyCompass;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Dynamic;
using Gameplay;
using UnityEngine;

public partial class StarrySkyCompassModel: ActivityEntityBase
{
    private static StarrySkyCompassModel _instance;
    public static StarrySkyCompassModel Instance => _instance ?? (_instance = new StarrySkyCompassModel());
    public override string Guid => "OPS_EVENT_TYPE_STARRY_SKY_COMPASS";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitAuto()
    {
        Instance.Init();
    }

    public StarrySkyCompassModel()
    {
        EventDispatcher.Instance.AddEventListener(EventEnum.BackLogin,InitEntranceAgain);
    }

    public bool IsUnlock => UnlockManager.IsOpen(UnlockManager.MergeUnlockType.StarrySkyCompass);

    public bool IsOpenPrivate()
    {
        return IsUnlock && IsOpened();
    }
    public bool IsStart()
    {
        return IsOpenPrivate() && Storage.GetPreheatTime() == 0;
    }
    public override bool CanDownLoadRes()
    {
        return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.StarrySkyCompass);
    }

    public StarrySkyCompassGlobalConfig GlobalConfig =>
        StarrySkyCompassConfigManager.Instance.GetConfig<StarrySkyCompassGlobalConfig>()[0];

    public Dictionary<int, StarrySkyCompassResultConfig> ResultDicConfig =
        new Dictionary<int, StarrySkyCompassResultConfig>();
    public List<StarrySkyCompassResultConfig> ResultConfig =>
        StarrySkyCompassConfigManager.Instance.GetConfig<StarrySkyCompassResultConfig>();
    public List<StarrySkyCompassShopConfig> ShopConfig =>
        StarrySkyCompassConfigManager.Instance.GetConfig<StarrySkyCompassShopConfig>();
    public List<StarrySkyCompassTaskRewardConfig> TaskRewardConfig =>
        StarrySkyCompassConfigManager.Instance.GetConfig<StarrySkyCompassTaskRewardConfig>();
    public List<StarrySkyCompassTurntableConfig> TurntableConfig =>
        StarrySkyCompassConfigManager.Instance.GetConfig<StarrySkyCompassTurntableConfig>();
    
    public override void InitFromServerData(string activityId, string activityType, ulong startTime, ulong endTime,
        ulong rewardEndTime, bool manualEnd, string configJson, string activitySubType)
    {
        base.InitFromServerData(activityId, activityType, startTime, endTime, rewardEndTime, manualEnd, configJson,
            activitySubType);
        StarrySkyCompassConfigManager.Instance.InitConfig(configJson);
        ResultDicConfig.Clear();
        foreach (var config in ResultConfig)
        {
            ResultDicConfig.Add(config.Id,config);
        }
        DebugUtil.Log($"InitConfig:{Guid}");
        InitStorage();
    }

    public void InitEntranceAgain(BaseEvent e)
    {
        if (!IsInitFromServer())
            return;
        var StarrySkyCompass = 1;
        Debug.Log(StarrySkyCompass);
    }

    public long PreheatTimeOffset =>  IsSkipActivityPreheating()?0:(GlobalConfig.PreheatTime * (long)XUtility.Hour);
    public void InitStorage()
    {
        if (!IsInitFromServer())
            return;
        if (Storage.ActiviryId != ActivityId)
        {
            Storage.ActiviryId = ActivityId;
            Storage.BuyState.Clear();
            Storage.RocketCount = 0;
            Storage.Score = 0;
            Storage.HappyValue = 0;
            Storage.HappyEndTime = 0;
            Storage.HappySpinCount = 0;
            Storage.HappySpinHistory.Clear();
            Storage.RocketCount += GlobalConfig.InitRocketCount;
        }
        Storage.StartTime = (long) StartTime;
        Storage.EndTime = (long) EndTime;
        Storage.PreheatTime = (long) StartTime + PreheatTimeOffset;
    }

    public StorageStarrySkyCompass Storage => StorageManager.Instance.GetStorage<StorageHome>().StarrySkyCompass;
    
    private static string CanShowUICoolTimeKey = "StarrySkyCompass_CanShowUI";
    public bool CanShowUI()
    {
        if (!IsOpenPrivate())
            return false;
        if (Storage.GetPreheatTime() > 0)
            return false;
        if (!CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, CanShowUICoolTimeKey))
        {
            CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, CanShowUICoolTimeKey, CommonUtils.GetTimeStamp());
            UIStarrySkyCompassMainController.Open(Storage);
            return true;
        }
        return false;
    }
    private static string CanShowPreheatUICoolTimeKey = "StarrySkyCompass_CanShowPreheatUI";
    public bool CanShowPreheatUI()
    {
        if (!IsOpenPrivate())
            return false;
        if (Storage.GetPreheatTime() <= 0)
            return false;
        if (!CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, CanShowPreheatUICoolTimeKey))
        {
            CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, CanShowPreheatUICoolTimeKey, CommonUtils.GetTimeStamp());
            UIPopupStarrySkyCompassPreviewController.Open(Storage);
            return true;
        }
        return false;
    }

    public int GetRocketCount()
    {
        if (!IsOpenPrivate())
            return 0;
        if (Storage.GetPreheatTime() > 0)
            return 0;
        return Storage.RocketCount;
    }
    public void AddRocket(int count,string reason)
    {
        if (!IsOpenPrivate())
            return;
        if (Storage.GetPreheatTime() > 0)
            return;
        Storage.RocketCount += count;
        EventDispatcher.Instance.SendEventImmediately(new EventStarrySkyCompassRocketCountChange(count));
        EventDispatcher.Instance.SendEventImmediately(new EventStarrySkyCompassUpdateRedPoint());
    }

    public int GetScore()
    {
        if (!IsOpenPrivate())
            return 0;
        if (Storage.GetPreheatTime() > 0)
            return 0;
        return Storage.Score;
    }

    public void AddScore(int count, string reason, bool needWait = false)
    {
        if (!IsOpenPrivate())
            return;
        if (Storage.GetPreheatTime() > 0)
            return;
        Storage.Score += count;
        EventDispatcher.Instance.SendEventImmediately(new EventStarrySkyCompassScoreChange(count,needWait));
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventStarrySkyCompassRadishChange,
            count.ToString(),Storage.Score.ToString(),reason);
        EventDispatcher.Instance.SendEventImmediately(new EventStarrySkyCompassUpdateRedPoint());
    }
    
    public Transform GetCommonFlyTarget()
    {
        if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
        {
            var entrance = DynamicEntryManager.Instance.GetDynamicEntry<DynamicEntry_Game_StarrySkyCompass>();
            if (entrance)
                return entrance.transform;
            else
                return MergeMainController.Instance.rewardBtnTrans;
        }
        else
        {
            var auxItem = DynamicEntryManager.Instance.GetDynamicEntry<DynamicEntry_Home_StarrySkyCompass>();
            if (auxItem != null && auxItem.gameObject.activeInHierarchy)
                return auxItem.transform;
            else
                return UIHomeMainController.mainController.MainPlayTransform;
        }
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

    private long CurTime => (long) APIManager.Instance.GetServerTime();

    public StarrySkyCompassResultConfig GuideSpin()
    {
        var result = ResultConfig.Find(a => a.Id == GlobalConfig.GuideResult);
        AddScore(result.Score,"Guide",true);
        if (result.HappyValue > 0)
        {
            Storage.HappyValue += result.HappyValue;
            if (Storage.HappyValue >= GlobalConfig.HappyMaxCount)
            {
                Storage.HappyValue = 0;
                Storage.HappyEndTime = CurTime + GlobalConfig.HappyTime * (long) XUtility.Min;
                Storage.HappySpinCount = 0;
                Storage.HappySpinHistory.Clear();
            }
            EventDispatcher.Instance.SendEventImmediately(new EventStarrySkyCompassHappyValueChange(result.HappyValue));   
        }
        return result;
    }
    public StarrySkyCompassResultConfig Spin()
    {
        if (CurTime < Storage.HappyEndTime)
        {
            return HappySpin();
        }
        var pool = ResultConfig;
        var weightList = new List<int>();
        foreach (var item in pool)
        {
            weightList.Add(item.Weight);
        }
        var randomIdx = Utils.RandomByWeight(weightList);
        var result = pool[randomIdx];
        AddScore(result.Score,"Spin",true);
        if (result.HappyValue > 0)
        {
            Storage.HappyValue += result.HappyValue;
            if (Storage.HappyValue >= GlobalConfig.HappyMaxCount)
            {
                Storage.HappyValue = 0;
                Storage.HappyEndTime = CurTime + GlobalConfig.HappyTime * (long) XUtility.Min;
                Storage.HappySpinCount = 0;
                Storage.HappySpinHistory.Clear();
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventStarrySkyHappyHourStart);
            }
            EventDispatcher.Instance.SendEventImmediately(new EventStarrySkyCompassHappyValueChange(result.HappyValue));   
        }
        return result;
    }

    public StarrySkyCompassResultConfig HappySpin()
    {
        var pool = new List<StarrySkyCompassResultConfig>();
        foreach (var item in ResultConfig)
        {
            if (Storage.HappySpinHistory.Contains(item.Id))
                continue;
            if (item.Level == 4 && Storage.HappySpinCount < GlobalConfig.HappyFinalLeastTimes)
                continue;
            pool.Add(item);
        }
        var weightList = new List<int>();
        foreach (var item in pool)
        {
            weightList.Add(item.HappyWeight);
        }
        var randomIdx = Utils.RandomByWeight(weightList);
        var result = pool[randomIdx];
        AddScore(result.HappyScore,"HappySpin",true);
        Storage.HappySpinCount++;
        Storage.HappySpinHistory.Add(result.Id);
        if (result.Level == 4)
        {
            Storage.HappyValue = 0;
            Storage.HappyEndTime = 0;
            Storage.HappySpinCount = 0;
            Storage.HappySpinHistory.Clear();
        }
        return result;
    }
    
    public bool CanShowGuide()
    {
        return CanShowCommonEntranceGuide(GuideTriggerPosition.StarrySkyCompassEntrance, GuideTargetType.StarrySkyCompassEntrance);
    }
    
    public bool CanShowCommonEntranceGuide(GuideTriggerPosition position,GuideTargetType targetType)
    {
        if (IsStart() &&
            !GuideSubSystem.Instance.IsShowingGuide() &&
            !GuideSubSystem.Instance.isFinished(position))
        {
            if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.BackHome ||
                SceneFsm.mInstance.GetCurrSceneType() == StatusType.Home)
            {
                var auxItem = DynamicEntryManager.Instance.GetDynamicEntry<DynamicEntry_Home_StarrySkyCompass>();
                List<Transform> topLayer = new List<Transform>();
                topLayer.Add(auxItem.transform);
                GuideSubSystem.Instance.RegisterTarget(targetType, auxItem.transform as RectTransform,
                    topLayer: topLayer);
                if (GuideSubSystem.Instance.Trigger(position, null))
                {
                    return true;
                }  
            }
            else if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Game)
            {
                var auxItem = DynamicEntryManager.Instance.GetDynamicEntry<DynamicEntry_Game_StarrySkyCompass>();
                List<Transform> topLayer = new List<Transform>();
                topLayer.Add(auxItem.transform);
                GuideSubSystem.Instance.RegisterTarget(targetType, auxItem.transform as RectTransform,
                    topLayer: topLayer);
                if (GuideSubSystem.Instance.Trigger(position, null))
                {
                    if (MergeTaskTipsController.Instance != null && MergeTaskTipsController.Instance.contentRect != null)
                    {
                        MergeTaskTipsController.Instance.contentRect.DOAnchorPosX(-auxItem.transform.localPosition.x+220, 0);
                    }
                    return true;
                }
            }
        }
        return false;
    }

    public bool ShowAuxItem()
    {
        if (Storage == null)
            return false;

        return StarrySkyCompassUtils.ShowAuxItem(Storage);
    }
    public bool ShowTaskEntrance()
    {
        if (Storage == null)
            return false;

        return Storage.ShowTaskEntrance();
    }
    
}