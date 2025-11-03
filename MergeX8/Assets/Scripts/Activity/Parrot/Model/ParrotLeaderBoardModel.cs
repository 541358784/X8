
using System.Collections.Generic;
using DragonPlus.Config.Parrot;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Newtonsoft.Json;

public class ParrotLeaderBoardModel:CommonLeaderBoardModel
{
    private static ParrotLeaderBoardModel _instance;
    public static ParrotLeaderBoardModel Instance => _instance ?? (_instance = new ParrotLeaderBoardModel());

    public void CreateStorage(StorageParrot storage)
    {
        var activityId = storage.ActivityId;
        var jsonRewardConfig = JsonConvert.SerializeObject(ParrotModel.Instance.ParrotLeaderBoardRewardConfigList);
        var startTime = storage.PreheatCompleteTime;
        var endTime = storage.EndTime;
        var maxPlayerCount = ParrotModel.Instance.GlobalConfig.MaxPlayerCount;
        
        var resMd5List = ActivityManager.Instance.GetActivityMd5List(storage.ActivityId);
        var resList = new List<string>();
        foreach (var resMd5 in resMd5List)
        {
            var resPath = ActivityResHotUpdate.GetFilePath(resMd5);
            resList.Add(resPath);
        }
        var leastStarCount = ParrotModel.Instance.GlobalConfig.LeastEnterBoardScore;
        var auxItemAssetPath = "";
        var taskEntranceAssetPath = "";
        var mainPopupAssetPath = UINameConst.UIParrotLeaderBoardMain;
        CreateStorage(activityId, jsonRewardConfig, startTime, endTime, maxPlayerCount, resList,resMd5List, leastStarCount,
            auxItemAssetPath, taskEntranceAssetPath, mainPopupAssetPath,0,0);
    }
    private static readonly Dictionary<StorageCommonLeaderBoard, List<ParrotLeaderBoardRewardConfig>> RewardConfigPool =
        new Dictionary<StorageCommonLeaderBoard, List<ParrotLeaderBoardRewardConfig>>();
    public override List<ResData> GetRewardsByRank(StorageCommonLeaderBoard storageWeek, int rank)
    {
        var rewardList = new List<ResData>();
        if (storageWeek.JsonRewardConfig.IsEmptyString())
            return rewardList;
        if (!RewardConfigPool.ContainsKey(storageWeek))
        {
            var newRewardConfig = JsonConvert.DeserializeObject<List<ParrotLeaderBoardRewardConfig>>(storageWeek.JsonRewardConfig);
            RewardConfigPool.Add(storageWeek,newRewardConfig);
        }
        var rewardConfig = RewardConfigPool[storageWeek];
        ParrotLeaderBoardRewardConfig targetConfig = null;
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
        return "ParrotLeaderBoard";
    }

    public override Dictionary<string, StorageCommonLeaderBoard> GetStorage()
    {
        return StorageManager.Instance.GetStorage<StorageHome>().ParrotLeaderBoard;
    }

    public override void OpenMainPopup(StorageCommonLeaderBoard storageWeek)
    {
        UIParrotLeaderBoardMainController.Open(storageWeek);
    }

    public override BiEventAdventureIslandMerge.Types.ItemChangeReason GetItemChangeReason()
    {
        return BiEventAdventureIslandMerge.Types.ItemChangeReason.ParrotLeaderBoardGet;
    }

    public override BiEventAdventureIslandMerge.Types.GameEventType LeaderBoardFinishGameEventType()
    {
        return BiEventAdventureIslandMerge.Types.GameEventType.GameEventParrotLeaderBoard;
    }

    public override bool CanDownloadRes(StorageCommonLeaderBoard storageWeek)
    {
        return !ParrotModel.Instance.IsInitFromServer() && base.CanDownloadRes(storageWeek);
    }
}