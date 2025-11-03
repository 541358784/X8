using System;
using System.Collections.Generic;
using DragonPlus.Config.DogHope;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using Newtonsoft.Json;
using UnityEngine;

public partial class DogHopeModel :ActivityEntityBase
{
    public string GetAuxItemAssetPath()
    {
        return "Prefabs/Activity/Dog/Aux_DogHope";
    }
    public string GetTaskItemAssetPath()
    {
        return "Prefabs/Activity/Dog/TaskList_DogHope";
    }
    private static DogHopeModel _instance;
    public static DogHopeModel Instance => _instance ?? (_instance = new DogHopeModel());
    
    public override string Guid => "OPS_EVENT_TYPE_DOG_HOPE";
 
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitAuto()
    {
        Instance.Init();
    }


    public const int _dogCookiesId = 20101;

    public DogHopeModel()
    {
        TMatch.Timer.Register(1, UpdateTime, null, true);
    }
    private bool _lastActivityOpenState;
    private bool LoopCreateStorage;
    public void UpdateTime()
    {
        // if(DogHopeModel.Instance.IsCanClearDogCookie())
        //     RemoveAllDogCookies();
        if (!IsInitFromServer())
            return;
        if (LoopCreateStorage && IsOpened() && CurStorageDogHopeWeek == null)
        {
            if (CreateStorage())
            {
                LoopCreateStorage = false;
            }
        }
        var currentActivityOpenState = DogHopeModel.Instance.IsOpenActivity();
        if (_lastActivityOpenState == currentActivityOpenState)
            return;
        if (_lastActivityOpenState && !currentActivityOpenState)
        {
            DogHopeLeaderBoardModel.CanShowUnCollectRewardsUI();
        }
        else if(!_lastActivityOpenState && currentActivityOpenState)
        {
        }

        _lastActivityOpenState = currentActivityOpenState;
    }
    
    public override void InitFromServerData(string activityId, string activityType, ulong startTime, ulong endTime,
        ulong rewardEndTime, bool manualEnd, string configJson, string activitySubType)
    {
        base.InitFromServerData(activityId, activityType, startTime, endTime, rewardEndTime, manualEnd, configJson,
            activitySubType);
        DogHopeConfigManager.Instance.InitConfig(configJson);
        InitServerDataFinish();
        if (CurStorageDogHopeWeek != null)
        {
            CurStorageDogHopeWeek.StartTime = (long) StartTime;
            CurStorageDogHopeWeek.EndTime = (long) EndTime;
            CurStorageDogHopeWeek.LeaderBoardStorage.JsonRecoverCoinRewardConfig = JsonConvert.SerializeObject(DogHopeModel.Instance.LeaderBoardRewardConfig);
            CurStorageDogHopeWeek.LeaderBoardStorage.EndTime = CurStorageDogHopeWeek.EndTime;
            CurStorageDogHopeWeek.LeaderBoardStorage.StartTime = CurStorageDogHopeWeek.StartTime;
        }
        if (CurStorageDogHopeWeek == null && DogHopeModel.GetFirstWeekCanGetReward() == null)
        {
            if (!CreateStorage())
            {
                LoopCreateStorage = true;
            }
        }
        _lastActivityOpenState = IsOpenActivity();
        DebugUtil.Log($"InitConfig:{Guid}");
        DogHopeLeaderBoardModel.Instance.InitFromServerData();
        var removeKeyList = new List<string>();
        foreach (var pair in StorageDogHope)
        {
            if (pair.Value.ActivityId.IsEmptyString())
            {
                removeKeyList.Add(pair.Key);
            }
        }
        foreach (var key in removeKeyList)
        {
            StorageDogHope.Remove(key);
        }
    }

    public override void UpdateActivityState()
    {
        InitServerDataFinish();
    }


    public bool IsOpenActivity()
    {
        if (!IsOpened() || CurStorageDogHopeWeek == null || CurStorageDogHopeWeek.IsTimeOut())
            return false;
        return true;
        return  !CurStorageDogHopeWeek.IsManualActivity;
    }

    public override bool IsOpened(bool hasLog = false)
    {
        return UnlockManager.IsOpen(UnlockManager.MergeUnlockType.DogHope) //已解锁
               && base.IsOpened(hasLog);
    }

    public bool CanShowStartView()
    {
        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.DogHope))
            return false;
        return IsOpenActivity() && !CurStorageDogHopeWeek.IsShowStartView;
    }

    public void ShowStartView()
    {
        CurStorageDogHopeWeek.IsShowStartView = true;
    }
  
    public bool IsCanClearDogCookie()
    {
        if (IsInitFromServer())
            return !IsOpenActivity();

        return IsActivityEnd();
    }

    public bool IsActivityEnd()
    {
        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.DogHope))
            return false;
        
        List<string> keys = new List<string>(StorageManager.Instance.GetStorage<StorageHome>().DogHopes.Keys);
        for (int i = keys.Count - 1; i >= 0; i--)
        {
            if (StorageManager.Instance.GetStorage<StorageHome>().DogHopes[keys[i]].EndTime > 0)
                return (long)APIManager.Instance.GetServerTime() >= StorageManager.Instance.GetStorage<StorageHome>()
                    .DogHopes[keys[i]].EndTime;
        }

        return true;
    }
    
    public void ShowEndView()
    {
        CurStorageDogHopeWeek.IsShowEndView = true;
    }

    public DogHopeReward AddScore(int score)
    {
        CurStorageDogHopeWeek.TotalScore += score;
        DogHopeLeaderBoardModel.Instance.SetStar(CurStorageDogHopeWeek.TotalScore);
        var config = GetCurIndexData();
        if (config != null)
        {
            if(CurStorageDogHopeWeek.TotalScore >= config.Score && CurStorageDogHopeWeek.CurIndex < DogHopeConfigManager.Instance.GetConfig<DogHopeReward>().Count)
                CurStorageDogHopeWeek.CurIndex++;   
        }

        // if (CurStorageDogHopeWeek.CurIndex == DogHopeConfigManager.Instance.GetConfig<DogHopeReward>().Count - 1)
        //     CurStorageDogHopeWeek.TotalScore = Math.Min(config.Score, CurStorageDogHopeWeek.TotalScore);
        
        return config;
    }

    public void RemoveAllDogCookies()
    {
        MergeManager.Instance.RemoveAllItemByType(MergeItemType.dogCookies,MergeBoardEnum.Main,"DogRemove");
    }
    public bool CanManualActivity()
    {
        // if (!IsOpenActivity())
        //     return false;

        if (CurStorageDogHopeWeek.IsManualActivity)
            return false;

        return IsMax();
    }

    public bool IsMax()
    {
        var config = GetCurIndexData();
        if (CurStorageDogHopeWeek.CurIndex == DogHopeConfigManager.Instance.GetConfig<DogHopeReward>().Count - 1 &&
            CurStorageDogHopeWeek.TotalScore >= config.Score)
            return true;
        return false;
    }
    
    public int GetIndexStageScore(int index = -1)
    {
        index = index < 0 ? CurStorageDogHopeWeek.CurIndex : index;
        var curData = GetCurIndexData(index);
        if (curData == null)
            return 0;
        
        var preData = index == 0 ? null : GetPreIndexData(index);

        int preStateScore = preData == null ? 0 : preData.Score;

        return curData.Score - preStateScore;
    }

    public int GetScore()
    {
        if (!IsOpenActivity())
            return 0;
        return CurStorageDogHopeWeek.TotalScore;
    }

    public int GetCurIndex()
    {
        if (!IsOpenActivity())
            return 0;
        return CurStorageDogHopeWeek.CurIndex;
    }
    
    public DogHopeReward GetCurIndexData(int index = -1)
    {
        index = index < 0 ? CurStorageDogHopeWeek.CurIndex : index;
        
        return GetTableDogHopesByIndex(index);
    }

    public DogHopeReward GetNextIndexData(int index = -1)
    {
        index = index < 0 ? CurStorageDogHopeWeek.CurIndex : index;
        
        return GetTableDogHopesByIndex(index+1);
    }   
    
    public DogHopeReward GetPreIndexData(int index = -1)
    {
        index = index < 0 ? CurStorageDogHopeWeek.CurIndex-1 : index - 1;
        
        return GetTableDogHopesByIndex(index);
    }

    public int GetCurStageScore()
    {
        var preData = GetPreIndexData();
        return GetScore() - (preData == null ? 0 : preData.Score);
    }

    public int GetIndexByScore(int score)
    {
        int count = DogHopeConfigManager.Instance.GetConfig<DogHopeReward>().Count - 1;
        if (score > DogHopeConfigManager.Instance.GetConfig<DogHopeReward>()[count].Score)
            return count;
        
        for (int i = 0; i <= count; i++)
        {
            if (DogHopeConfigManager.Instance.GetConfig<DogHopeReward>()[i].Score >= score)
                return i;
        }

        return 0;
    }
    public DogHopeReward GetMaxIndexData()
    {
        var tableDogHopes=DogHopeConfigManager.Instance.GetConfig<DogHopeReward>();

        if (tableDogHopes == null)
            return null;
        
        return tableDogHopes[tableDogHopes.Count - 1];
    }

    
    public void EndActivity(bool isManual = false)
    {
        CurStorageDogHopeWeek.IsShowEndView = false;
        CurStorageDogHopeWeek.IsManualActivity = isManual;
    }
 
    public bool IsComplete()
    {
        var lastData = GetMaxIndexData();
        if (lastData == null)
            return true;
        
        return GetScore() >= lastData.Score;
    }

    public void CheckActivitySuccess()
    {
        if(!IsComplete())
            return;
        
        if(CurStorageDogHopeWeek.IsShowEndView)
            return;
        
        CurStorageDogHopeWeek.IsShowEndView = true;
         UIManager.Instance.OpenUI(UINameConst.UIDogMain,UIDogMainController.DogMainShowType.Open);
         if(DogHopeModel.Instance.IsCanClearDogCookie())
            RemoveAllDogCookies();
    }
    public DogHopeReward GetTableDogHopesByIndex(int index)
    {
        if (!IsOpened())
            return null;
        
        var tableDogHopes =DogHopeConfigManager.Instance.GetConfig<DogHopeReward>();
        if (index < 0 || index >= tableDogHopes.Count)
            return null;

        return tableDogHopes[index];
    }

    
    public override bool CanDownLoadRes()
    {
        return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.DogHope);
    }
}