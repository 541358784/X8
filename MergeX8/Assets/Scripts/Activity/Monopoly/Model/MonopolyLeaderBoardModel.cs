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

public class MonopolyLeaderBoardModel:CommonLeaderBoardModel
{
    private static MonopolyLeaderBoardModel _instance;
    public static MonopolyLeaderBoardModel Instance => _instance ?? (_instance = new MonopolyLeaderBoardModel());

    public void CreateStorage(StorageMonopoly storage)
    {
        var activityId = storage.ActivityId;
        var jsonRewardConfig = JsonConvert.SerializeObject(MonopolyModel.Instance.LeaderBoardRewardConfig);
        var startTime = storage.PreheatCompleteTime;
        var endTime = storage.EndTime;
        var maxPlayerCount = MonopolyModel.Instance.GlobalConfig.MaxPlayerCount;
        var resList = storage.ActivityResList;
        var resMd5List = storage.ActivityResMd5List;
        var leastStarCount = MonopolyModel.Instance.GlobalConfig.LeastEnterBoardScore;
        var auxItemAssetPath = "";
        var taskEntranceAssetPath = "";
        var mainPopupAssetPath = UINameConst.UIMonopolyLeaderBoardMain;
        CreateStorage(activityId, jsonRewardConfig, startTime, endTime, maxPlayerCount, resList,resMd5List, leastStarCount,
            auxItemAssetPath, taskEntranceAssetPath, mainPopupAssetPath,0);
    }
    private static readonly Dictionary<StorageCommonLeaderBoard, List<MonopolyLeaderBoardRewardConfig>> RewardConfigPool =
        new Dictionary<StorageCommonLeaderBoard, List<MonopolyLeaderBoardRewardConfig>>();
    public override List<ResData> GetRewardsByRank(StorageCommonLeaderBoard storageWeek, int rank)
    {
        var rewardList = new List<ResData>();
        if (storageWeek.JsonRewardConfig.IsEmptyString())
            return rewardList;
        if (!RewardConfigPool.ContainsKey(storageWeek))
        {
            var newRewardConfig = JsonConvert.DeserializeObject<List<MonopolyLeaderBoardRewardConfig>>(storageWeek.JsonRewardConfig);
            RewardConfigPool.Add(storageWeek,newRewardConfig);
        }
        var rewardConfig = RewardConfigPool[storageWeek];
        MonopolyLeaderBoardRewardConfig targetConfig = null;
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
        return "MonopolyLeaderBoard";
    }

    public override Dictionary<string, StorageCommonLeaderBoard> GetStorage()
    {
        return StorageManager.Instance.GetStorage<StorageHome>().MonopolyLeaderBoard;
    }

    public override void OpenMainPopup(StorageCommonLeaderBoard storageWeek)
    {
        UIMonopolyLeaderBoardMainController.Open(storageWeek);
    }

    public override BiEventAdventureIslandMerge.Types.ItemChangeReason GetItemChangeReason()
    {
        return BiEventAdventureIslandMerge.Types.ItemChangeReason.MonopolyLeaderBoardGet;
    }

    public override BiEventAdventureIslandMerge.Types.GameEventType LeaderBoardFinishGameEventType()
    {
        return BiEventAdventureIslandMerge.Types.GameEventType.GameEventMonopolyLeaderBoard;
    }

    public override bool CanDownloadRes(StorageCommonLeaderBoard storageWeek)
    {
        return !MonopolyModel.Instance.IsInitFromServer() && base.CanDownloadRes(storageWeek);
    }
}