using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DragonPlus.Config.CommonResourceLeaderBoard;
using DragonU3DSDK;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;

public class CommonResourceLeaderBoardModel : ActivityEntityBase
{
    private static CommonResourceLeaderBoardModel _instance;
    public static CommonResourceLeaderBoardModel Instance => _instance ?? (_instance = new CommonResourceLeaderBoardModel());
    public override string Guid => "OPS_EVENT_TYPE_COMMON_RESOURCE_LEADER_BOARD";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitAuto()
    {
        Instance.Init();
    }

    public Dictionary<string, StorageCommonResourceLeaderBoard> Storage =>
        StorageManager.Instance.GetStorage<StorageHome>().CommonResourceLeaderBoard;
    public Dictionary<string, SingleCommonResourceLeaderBoardConfigStruct> ConfigDictionary =
        new Dictionary<string, SingleCommonResourceLeaderBoardConfigStruct>();

    public Dictionary<string, SingleCommonResourceLeaderBoardModel> ModelDictionary =
        new Dictionary<string, SingleCommonResourceLeaderBoardModel>();

    public Dictionary<UserData.ResourceId, List<StorageCommonLeaderBoard>> ResourceCollectStorageDic =
        new Dictionary<UserData.ResourceId, List<StorageCommonLeaderBoard>>();

    public SingleCommonResourceLeaderBoardModel GetModel(string keyWord)
    {
        if (!Storage.TryGetValue(keyWord, out var modelStorage))
        {
            modelStorage = new StorageCommonResourceLeaderBoard();
            modelStorage.KeyWord = keyWord;
            Storage.Add(keyWord,modelStorage);
        }
        if (!ModelDictionary.TryGetValue(keyWord,out var model))
        {
            model = new SingleCommonResourceLeaderBoardModel(modelStorage);
            ModelDictionary.Add(keyWord,model);
        }
        return model;
    }

    private StorageCommonLeaderBoard _storageCommonLeaderBoard;
    
    public override void InitFromServerData(string activityId, string activityType, ulong startTime, ulong endTime,
        ulong rewardEndTime, bool manualEnd, string configJson, string activitySubType)
    {
        base.InitFromServerData(activityId, activityType, startTime, endTime, rewardEndTime, manualEnd, configJson,
            activitySubType);
        CommonResourceLeaderBoardConfigManager.Instance.InitConfig(configJson);
        if (!ConfigDictionary.TryGetValue(activityId,out var configStruct))
        {
            configStruct = new SingleCommonResourceLeaderBoardConfigStruct();
            ConfigDictionary.Add(activityId,configStruct);
        }
        configStruct.GlobalConfig = CommonResourceLeaderBoardConfigManager.Instance
            .GetConfig<CommonResourceLeaderBoardGlobalConfig>()[0];
        configStruct.RewardConfig = CommonResourceLeaderBoardConfigManager.Instance
            .GetConfig<CommonResourceLeaderBoardRewardConfig>();
        configStruct.StartTime = (long)startTime;
        configStruct.EndTime = (long)endTime;
        configStruct.ActivityId = activityId;
        UpdateActivityUsingResList(activityId,false);
        var model = GetModel(configStruct.GlobalConfig.KeyWord);
        _storageCommonLeaderBoard = model.CreateStorage(configStruct);
        var collectType = (UserData.ResourceId) configStruct.GlobalConfig.CollectResourceId;
        if (!ResourceCollectStorageDic.TryGetValue(collectType, out var storageList))
        {
            storageList = new List<StorageCommonLeaderBoard>();
            ResourceCollectStorageDic.Add(collectType,storageList);
        }
        if (!storageList.Contains(_storageCommonLeaderBoard))
            storageList.Add(_storageCommonLeaderBoard);
        CreateTaskEntrance(_storageCommonLeaderBoard);
    }

    public void OnAddRes(UserData.ResourceId resId,int count)
    {
        if (!ResourceCollectStorageDic.TryGetValue(resId, out var storageList))
        {
            return;
        }

        foreach (var storage in storageList)
        {
            if (storage.IsActive())
            {
                storage.CollectStar(count);
            }
        }
    }
    
    public override bool CanDownLoadRes()
    {
        return true;
    }
    public override List<string> GetNeedResList(string activityId,List<string> allResList)
    {
        var config = ConfigDictionary[activityId];
        var skinNameList = new List<string>();
        if (IsInitFromServer())
            skinNameList.Add(config.GlobalConfig.SkinName.ToLower());
        var resList = new List<string>();
        foreach (var path in allResList)
        {
            foreach (var skinName in skinNameList)
            {
                if (path.Contains(skinName))
                {
                    DebugUtil.Log("ActivityManager -> 活动资源 : " + path);
                    resList.Add(path);
                    break;
                }
            }
        }
        return resList;
    }
    
    public MergeCommonResourceLeaderBoard GetTaskEntrance(StorageCommonLeaderBoard storage)
    {
        // if (MergeTaskEntrance_CommonResourceLeaderBoard.CreatorDic.TryGetValue(storage, out var creator))
        // {
        //     if (creator.TaskEntrance)
        //         return creator.TaskEntrance;
        //     var auxItem = MergeTaskTipsController.Instance.TryCreateEntrance(creator);
        //     if (auxItem)
        //         return auxItem as MergeCommonResourceLeaderBoard;
        // }
        return null;
    }
    
    
    public async void CreateTaskEntrance(StorageCommonLeaderBoard storage)
    {
        if (storage == null)
            return;
        while (MergeTaskTipsController.Instance == null)
        {
            // await XUtility.WaitSeconds(1f);
            await Task.Delay(1000);
        }
        // if (!MergeTaskEntrance_CommonResourceLeaderBoard.CreatorDic.ContainsKey(storage))
        // {
        //     if (MergeTaskTipsController.Instance)
        //         MergeTaskTipsController.Instance.PushTaskEntranceCreator(new MergeTaskEntrance_CommonResourceLeaderBoard(storage));
        // }
    }

    public bool CheckUnCollectStorage()
    {
        foreach (var pair in Storage)
        {
            var model = GetModel(pair.Key);
            var collectWeekStorage = model.GetFirstWeekCanGetReward();
            if (collectWeekStorage != null)
            {
                var autoPopup = new AutoPopupManager.AutoPopupManager.AutoPopUI(model.CanShowUnCollectRewardsUI,new[] {UINameConst.UIWaiting,collectWeekStorage.MainPopupAssetPath});
                AutoPopupManager.AutoPopupManager.Instance.PushExtraPopup(autoPopup);
            }
        }
        return false;
    }

    public string AuxItemAssetPath()
    {
        if (_storageCommonLeaderBoard == null)
            return "";

        return _storageCommonLeaderBoard.AuxItemAssetPath;
    }
    
    public bool CanCreate()
    {
        if (_storageCommonLeaderBoard == null)
            return false;
        
        return _storageCommonLeaderBoard.IsResExist() && _storageCommonLeaderBoard.IsActive();
    }
}