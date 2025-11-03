using System;
using System.Collections.Generic;
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

public class SingleCommonResourceLeaderBoardModel:CommonLeaderBoardModel
{
    private StorageCommonResourceLeaderBoard Storage;
    public SingleCommonResourceLeaderBoardModel(StorageCommonResourceLeaderBoard storage):base()
    {
        Storage = storage;
        RegisterModel();
    }
    public StorageCommonLeaderBoard CreateStorage(SingleCommonResourceLeaderBoardConfigStruct configStruct)
    {
        var activityId = configStruct.ActivityId;
        var jsonRewardConfig = JsonConvert.SerializeObject(configStruct.RewardConfig);
        var startTime = configStruct.StartTime;
        var endTime = configStruct.EndTime;
        var maxPlayerCount = configStruct.GlobalConfig.MaxPlayerCount;
        var resList = new List<string>();
        var resMd5List = ActivityManager.Instance.GetActivityMd5List(activityId);
        foreach (var resMd5 in resMd5List)
        {
            var resPath = ActivityResHotUpdate.GetFilePath(resMd5);
            resList.Add(resPath);
        }
        var leastStarCount = configStruct.GlobalConfig.LeastEnterBoardScore;
        var auxItemAssetPath = configStruct.GetAssetPathWithSkinName("Prefabs/Activity/CommonResourceLeaderBoard/Aux_CommonResourceLeaderBoard");
        var taskEntranceAssetPath = configStruct.GetAssetPathWithSkinName("Prefabs/Activity/CommonResourceLeaderBoard/TaskList_CommonResourceLeaderBoard");
        var mainPopupAssetPath = configStruct.GetAssetPathWithSkinName("Prefabs/Activity/CommonResourceLeaderBoard/UICommonResourceLeaderBoardMain");
        var collectItemResourceId = configStruct.GlobalConfig.CollectResourceId;
        CreateStorage(activityId, jsonRewardConfig, startTime, endTime, maxPlayerCount, resList,resMd5List, leastStarCount,
            auxItemAssetPath, taskEntranceAssetPath, mainPopupAssetPath,collectItemResourceId);
        return Storage.LeaderBoardDictionary[activityId];
    }
    
    private static readonly Dictionary<StorageCommonLeaderBoard, List<CommonResourceLeaderBoardRewardConfig>> RewardConfigPool =
        new Dictionary<StorageCommonLeaderBoard, List<CommonResourceLeaderBoardRewardConfig>>();
    public override List<ResData> GetRewardsByRank(StorageCommonLeaderBoard storageWeek, int rank)
    {
        var rewardList = new List<ResData>();
        if (storageWeek.JsonRewardConfig.IsEmptyString())
            return rewardList;
        if (!RewardConfigPool.ContainsKey(storageWeek))
        {
            var newRewardConfig = JsonConvert.DeserializeObject<List<CommonResourceLeaderBoardRewardConfig>>(storageWeek.JsonRewardConfig);
            RewardConfigPool.Add(storageWeek,newRewardConfig);
        }
        var rewardConfig = RewardConfigPool[storageWeek];
        CommonResourceLeaderBoardRewardConfig targetConfig = null;
        for (var i = 0; i < rewardConfig.Count; i++)
        {
            var cfg = rewardConfig[i];
            if (cfg.RankMin <= rank && cfg.RankMax >= rank)
            {
                targetConfig = cfg;
                break;
            }
        }
        if (targetConfig == null)
            return rewardList;
        if (targetConfig.RewardId == null)
            return rewardList;
        for (var i = 0; i < targetConfig.RewardId.Count; i++)
        {
            rewardList.Add(new ResData(targetConfig.RewardId[i],targetConfig.RewardNum[i]));
        }
        return rewardList;
    }

    public override string LeadBoardKeyWord()
    {
        if (Storage == null)
            return string.Empty;
        return Storage.KeyWord;
    }

    public override Dictionary<string, StorageCommonLeaderBoard> GetStorage()
    {
        return Storage.LeaderBoardDictionary;
    }

    public override void OpenMainPopup(StorageCommonLeaderBoard storageWeek)
    {
        UICommonResourceLeaderBoardMainController.Open(storageWeek);
    }

    public override BiEventAdventureIslandMerge.Types.ItemChangeReason GetItemChangeReason()
    {
        return BiEventAdventureIslandMerge.Types.ItemChangeReason.MonopolyLeaderBoardGet;
    }

    public override BiEventAdventureIslandMerge.Types.GameEventType LeaderBoardFinishGameEventType()
    {
        return BiEventAdventureIslandMerge.Types.GameEventType.GameEventMonopolyLeaderBoard;
    }
}