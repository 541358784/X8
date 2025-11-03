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

public class FishCultureLeaderBoardModel:CommonLeaderBoardModel
{
    private static FishCultureLeaderBoardModel _instance;
    public static FishCultureLeaderBoardModel Instance => _instance ?? (_instance = new FishCultureLeaderBoardModel());

    public void CreateStorage(StorageFishCulture storage)
    {
        var activityId = storage.ActivityId;
        var jsonRewardConfig = JsonConvert.SerializeObject(FishCultureModel.Instance.LeaderBoardRewardConfig);
        var startTime = storage.PreheatCompleteTime;
        var endTime = storage.PreEndTime;
        var maxPlayerCount = FishCultureModel.Instance.GlobalConfig.MaxPlayerCount;
        var resList = storage.ActivityResList;
        var resMd5List = storage.ActivityResMd5List;
        var leastStarCount = FishCultureModel.Instance.GlobalConfig.LeastEnterBoardScore;
        var auxItemAssetPath = "";
        var taskEntranceAssetPath = "";
        var mainPopupAssetPath = UINameConst.UIFishCultureBoardMain;
        CreateStorage(activityId, jsonRewardConfig, startTime, endTime, maxPlayerCount, resList,resMd5List, leastStarCount,
            auxItemAssetPath, taskEntranceAssetPath, mainPopupAssetPath,0);
    }
    private static readonly Dictionary<StorageCommonLeaderBoard, List<FishCultureLeaderBoardRewardConfig>> RewardConfigPool =
        new Dictionary<StorageCommonLeaderBoard, List<FishCultureLeaderBoardRewardConfig>>();
    public override List<ResData> GetRewardsByRank(StorageCommonLeaderBoard storageWeek, int rank)
    {
        var rewardList = new List<ResData>();
        if (storageWeek.JsonRewardConfig.IsEmptyString())
            return rewardList;
        if (!RewardConfigPool.ContainsKey(storageWeek))
        {
            var newRewardConfig = JsonConvert.DeserializeObject<List<FishCultureLeaderBoardRewardConfig>>(storageWeek.JsonRewardConfig);
            RewardConfigPool.Add(storageWeek,newRewardConfig);
        }
        var rewardConfig = RewardConfigPool[storageWeek];
        FishCultureLeaderBoardRewardConfig targetConfig = null;
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
        return "FishCultureLeaderBoard";
    }

    public override Dictionary<string, StorageCommonLeaderBoard> GetStorage()
    {
        return StorageManager.Instance.GetStorage<StorageHome>().FishCultureLeaderBoard;
    }

    public override void OpenMainPopup(StorageCommonLeaderBoard storageWeek)
    {
        UIFishCultureLeaderBoardMainController.Open(storageWeek);
    }

    public override BiEventAdventureIslandMerge.Types.ItemChangeReason GetItemChangeReason()
    {
        return BiEventAdventureIslandMerge.Types.ItemChangeReason.FishCultureLeaderBoardGet;
    }

    public override BiEventAdventureIslandMerge.Types.GameEventType LeaderBoardFinishGameEventType()
    {
        return BiEventAdventureIslandMerge.Types.GameEventType.GameEventFishCultureLeaderBoard;
    }

    public override bool CanDownloadRes(StorageCommonLeaderBoard storageWeek)
    {
        return !FishCultureModel.Instance.IsInitFromServer() && base.CanDownloadRes(storageWeek);
    }
}