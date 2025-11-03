
using System.Collections.Generic;
using DragonPlus.Config.PillowWheel;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Newtonsoft.Json;

public class PillowWheelLeaderBoardModel:CommonLeaderBoardModel
{
    private static PillowWheelLeaderBoardModel _instance;
    public static PillowWheelLeaderBoardModel Instance => _instance ?? (_instance = new PillowWheelLeaderBoardModel());

    public void CreateStorage(StoragePillowWheel storage)
    {
        var activityId = storage.ActivityId;
        var jsonRewardConfig = JsonConvert.SerializeObject(PillowWheelModel.Instance.LeaderBoardRewardConfigList);
        var startTime = storage.PreheatCompleteTime;
        var endTime = storage.EndTime;
        var maxPlayerCount = 30;
        
        var resMd5List = ActivityManager.Instance.GetActivityMd5List(storage.ActivityId);
        var resList = new List<string>();
        foreach (var resMd5 in resMd5List)
        {
            var resPath = ActivityResHotUpdate.GetFilePath(resMd5);
            resList.Add(resPath);
        }
        var leastStarCount = PillowWheelModel.Instance.GlobalConfig.LeastEnterBoardScore;
        var auxItemAssetPath = "";
        var taskEntranceAssetPath = "";
        var mainPopupAssetPath = UINameConst.UIPillowWheelLeaderBoardMain;
        CreateStorage(activityId, jsonRewardConfig, startTime, endTime, maxPlayerCount, resList,resMd5List, leastStarCount,
            auxItemAssetPath, taskEntranceAssetPath, mainPopupAssetPath,0,0);
    }
    private static readonly Dictionary<StorageCommonLeaderBoard, List<PillowWheelLeaderBoardRewardConfig>> RewardConfigPool =
        new Dictionary<StorageCommonLeaderBoard, List<PillowWheelLeaderBoardRewardConfig>>();
    public override List<ResData> GetRewardsByRank(StorageCommonLeaderBoard storageWeek, int rank)
    {
        var rewardList = new List<ResData>();
        if (storageWeek.JsonRewardConfig.IsEmptyString())
            return rewardList;
        if (!RewardConfigPool.ContainsKey(storageWeek))
        {
            var newRewardConfig = JsonConvert.DeserializeObject<List<PillowWheelLeaderBoardRewardConfig>>(storageWeek.JsonRewardConfig);
            RewardConfigPool.Add(storageWeek,newRewardConfig);
        }
        var rewardConfig = RewardConfigPool[storageWeek];
        PillowWheelLeaderBoardRewardConfig targetConfig = null;
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
        return "PillowWheelLeaderBoard";
    }

    public override Dictionary<string, StorageCommonLeaderBoard> GetStorage()
    {
        return StorageManager.Instance.GetStorage<StorageHome>().PillowWheelLeaderBoard;
    }

    public override void OpenMainPopup(StorageCommonLeaderBoard storageWeek)
    {
        UIPillowWheelLeaderBoardMainController.Open(storageWeek);
    }

    public override BiEventAdventureIslandMerge.Types.ItemChangeReason GetItemChangeReason()
    {
        return BiEventAdventureIslandMerge.Types.ItemChangeReason.PillowWheelLeaderBoardGet;
    }

    public override BiEventAdventureIslandMerge.Types.GameEventType LeaderBoardFinishGameEventType()
    {
        return BiEventAdventureIslandMerge.Types.GameEventType.GameEventPillowWheelLeaderBoard;
    }

    public override bool CanDownloadRes(StorageCommonLeaderBoard storageWeek)
    {
        return !PillowWheelModel.Instance.IsInitFromServer() && base.CanDownloadRes(storageWeek);
    }
}