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
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using GamePool;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class CommonLeaderBoardModel
{
    public abstract void OpenMainPopup(StorageCommonLeaderBoard storageWeek);
    public abstract BiEventAdventureIslandMerge.Types.GameEventType LeaderBoardFinishGameEventType();
    public abstract BiEventAdventureIslandMerge.Types.ItemChangeReason GetItemChangeReason();
    public abstract List<ResData> GetRewardsByRank(StorageCommonLeaderBoard storageWeek, int rank);
    public abstract string LeadBoardKeyWord();
    public abstract Dictionary<string, StorageCommonLeaderBoard> GetStorage();
    public Dictionary<string, StorageCommonLeaderBoard> Storage => GetStorage();
    public bool TryRelease(StorageCommonLeaderBoard storage)
    {
        if (storage.IsTimeOut() && (!storage.IsInitFromServer() || storage.IsFinish))
        {
            Debug.LogError("删除ActivityId = " + storage.ActivityId + "排行榜数据");
            Storage.Remove(storage.ActivityId);
            CommonLeaderBoardUtils.StorageWeekInitStateDictionary.Remove(storage.ActivityId);
            return true;
        }

        return false;
    }
    //查找已进入排行榜且处于开启状态的存档
    public StorageCommonLeaderBoard GetActiveLeaderBoard()
    {
        var canGetRewardStorage = GetFirstWeekCanGetReward();
        if (canGetRewardStorage != null)
            return null;
        foreach (var pair in Storage)
        {
            var leaderBoardStorage = pair.Value;
            if (leaderBoardStorage.IsInitFromServer() && !leaderBoardStorage.IsFinish && leaderBoardStorage.IsActive())
            {
                return leaderBoardStorage;
            }
        }
        return null;
    }

    public StorageCommonLeaderBoard GetLeaderBoardStorage(string activityId)
    {
        if (Storage.TryGetValue(activityId,out var leaderBoardStorage))
        {
            return leaderBoardStorage;
        }
        return null;
    }
    
    public virtual void CreateStorage(string activityId,string jsonRewardConfig,long startTime,long endTime,
        int maxPlayerCount,List<string> resList,List<string> resMd5List,int leastStarCount,
        string auxItemAssetPath,string taskEntranceAssetPath,string mainPopupAssetPath,int collectItemResourceId,int initStartCount = 0)
    {
        if (!Storage.TryGetValue(activityId, out var newWeek))
        {
            newWeek = new StorageCommonLeaderBoard();
            newWeek.LeaderBoardKeyWord = LeadBoardKeyWord();
            newWeek.JsonRewardConfig = jsonRewardConfig;
            newWeek.EndTime = endTime;
            newWeek.StartTime = startTime;
            newWeek.StarCount = initStartCount;
            newWeek.IsFinish = false;
            newWeek.IsStart = true;
            newWeek.MaxPlayerCount = maxPlayerCount;
            newWeek.ActivityId = activityId;
            newWeek.StarUpdateTime = APIManager.Instance.GetServerTime();
            newWeek.IsUpdateFinalData = false;
            newWeek.ActivityResList.Clear();
            foreach (var res in resList)
            {
                newWeek.ActivityResList.Add(res);
            }
            newWeek.ActivityResMd5List.Clear();
            foreach (var resMd5 in resMd5List)
            {
                newWeek.ActivityResMd5List.Add(resMd5);
            }
            newWeek.LeastStarCount = leastStarCount;
            newWeek.AuxItemAssetPath = auxItemAssetPath;
            newWeek.TaskEntranceAssetPath = taskEntranceAssetPath;
            newWeek.MainPopupAssetPath = mainPopupAssetPath;
            newWeek.CollectItemResourceId = collectItemResourceId;
            Storage.Add(newWeek.ActivityId,newWeek);
            CommonLeaderBoardUtils.StorageWeekInitStateDictionary.Add(newWeek.ActivityId, false);
        }
        else
        {
            newWeek.JsonRewardConfig = jsonRewardConfig;
            newWeek.EndTime = endTime;
            newWeek.StartTime = startTime;
            newWeek.LeastStarCount = leastStarCount;
            newWeek.CollectItemResourceId = collectItemResourceId;
        }
    }

    public void InitFromServerData()
    {
        CommonLeaderBoardUtils.StorageWeekInitStateDictionary.Clear();
        foreach (var pair in Storage)
        {
            CommonLeaderBoardUtils.StorageWeekInitStateDictionary.Add(pair.Value.ActivityId,false);
        }
        XUtility.WaitFrames(1, () =>
        {
            var activeStorage = GetActiveLeaderBoard();
            if (activeStorage != null)
            {
                activeStorage.ForceUpdateLeaderBoardFromServer().WrapErrors();
            }
        });
        UpdateTime();
    }

    public virtual bool CanDownloadRes(StorageCommonLeaderBoard storageWeek)
    {
        return !ActivityManager.Instance.activityCheckDatas.ContainsKey(storageWeek.ActivityId);
    }
    public bool CanShowUnCollectRewardsUI()
    {
        var weekCanGetReward = GetFirstWeekCanGetReward();
        if (weekCanGetReward == null)
            return false;
        if (!weekCanGetReward.IsResExist())
        {
            if (CanDownloadRes(weekCanGetReward))
            {
                var activityId = weekCanGetReward.ActivityId;
                var resList = weekCanGetReward.ActivityResMd5List;
                ActivityManager.Instance.SetActivityAllResMd5Dict(activityId, resList);
                ActivityManager.Instance.UpdateActivityUsingResList(activityId,resList);
                ActivityManager.Instance.CheckSingleActivityResState(activityId);
                if (!ActivityManager.Instance.IsActivityResourcesDownloaded(activityId))
                {
                    ActivityManager.Instance.TryPullSingleActivityRes(activityId);
                }
            }
            return false;   
        }

        WaitingManager.Instance.OpenWindow(5f);
        weekCanGetReward.ForceUpdateLeaderBoardFromServer().AddBoolCallBack((success) =>
        {
            WaitingManager.Instance.CloseWindow();
            if (!success)
                return;
            weekCanGetReward.IsUpdateFinalData = true;
            OpenMainPopup(weekCanGetReward);
        }).WrapErrors();
        return true;
    }
    public T OpenMainPopup<T>(StorageCommonLeaderBoard storageWeek) where T:UICommonLeaderBoardMainController
    {
        if (!storageWeek.IsUpdateFinalData)
        {
            storageWeek.TryUpdateLeaderBoardFromServer().WrapErrors();
        }
        var mainWindow = UIManager.Instance.OpenUI(storageWeek.MainPopupAssetPath,storageWeek,this) as T;
        return mainWindow;
    }
    public static Dictionary<string, CommonLeaderBoardModel> LeaderBoardModelDic =
        new Dictionary<string, CommonLeaderBoardModel>();
    public static CommonLeaderBoardModel GetInstance(string keyWord)
    {
        if (LeaderBoardModelDic.TryGetValue(keyWord, out var model))
        {
            return model;
        }
        Debug.LogError("未找到关键字"+keyWord+"的排行榜模块");
        return null;
    }
    public CommonLeaderBoardModel()
    {
        TMatch.Timer.Register(1, UpdateTime, null, true);
        RegisterModel();
    }

    public void RegisterModel()
    {
        var keyWord = LeadBoardKeyWord();
        if (!keyWord.IsEmptyString())
        {
            if (LeaderBoardModelDic.ContainsKey(keyWord))
            {
                Debug.LogError("重复注册排行榜模块 keyWord:"+keyWord);
            }
            else
            {
                LeaderBoardModelDic.Add(keyWord,this);
            }
        }
    }

    private StorageCommonLeaderBoard _lastActivityOpenState;
    public void UpdateTime()
    {
        var currentActivityOpenState = GetFirstWeekCanGetReward();
        if (_lastActivityOpenState == currentActivityOpenState)
            return;
        if (currentActivityOpenState != null)
        {
            var autoPopup = new AutoPopupManager.AutoPopupManager.AutoPopUI(CanShowUnCollectRewardsUI,new[] {UINameConst.UIWaiting,currentActivityOpenState.MainPopupAssetPath});
            AutoPopupManager.AutoPopupManager.Instance.PushExtraPopup(autoPopup);
        }
        else
        {
            
        }
        _lastActivityOpenState = currentActivityOpenState;
    }
    
    public StorageCommonLeaderBoard GetFirstWeekCanGetReward()
    {
        foreach (var leaderBoardPair in Storage)
        {
            var leaderBoard = leaderBoardPair.Value;
            if (leaderBoard.CanStorageCommonLeaderBoardGetReward())
                return leaderBoard;   
        }
        return null;
    }
}