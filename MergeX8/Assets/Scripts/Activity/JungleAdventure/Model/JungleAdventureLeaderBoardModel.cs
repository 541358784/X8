using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Deco.Node;
using Deco.World;
using Decoration;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.CoinRush;
using DragonPlus.Config.JungleAdventure;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using GamePool;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

public class JungleAdventureLeaderBoardModel:CommonLeaderBoardModel
{
    private static JungleAdventureLeaderBoardModel _instance;
    public static JungleAdventureLeaderBoardModel Instance => _instance ?? (_instance = new JungleAdventureLeaderBoardModel());

    public void CreateStorage(StorageJungleAdventure storage)
    {
        var activityId = storage.ActivityId;
        var jsonRewardConfig = JsonConvert.SerializeObject(JungleAdventureConfigManager.Instance.GetConfig<TableJungleAdventureRankRewardConfig>());
        var startTime = storage.PreActivityEndTime;
        var endTime = storage.JoinEndTime;
        var maxPlayerCount = JungleAdventureConfigManager.Instance.TableJungleAdventureSettingList[0].RankPeople;
        
        var resMd5List = ActivityManager.Instance.GetActivityMd5List(storage.ActivityId);
        var resList = new List<string>();
        foreach (var resMd5 in resMd5List)
        {
            var resPath = ActivityResHotUpdate.GetFilePath(resMd5);
            resList.Add(resPath);
        }
        var leastStarCount = JungleAdventureConfigManager.Instance.TableJungleAdventureSettingList[0].EnterRankScore;
        var auxItemAssetPath = "";
        var taskEntranceAssetPath = "";
        var mainPopupAssetPath = UINameConst.UIJungleAdventureBoardMain;
        CreateStorage(activityId, jsonRewardConfig, startTime, endTime, maxPlayerCount, resList,resMd5List, leastStarCount,
            auxItemAssetPath, taskEntranceAssetPath, mainPopupAssetPath,0);
    }
    private static readonly Dictionary<StorageCommonLeaderBoard, List<TableJungleAdventureRankRewardConfig>> RewardConfigPool =
        new Dictionary<StorageCommonLeaderBoard, List<TableJungleAdventureRankRewardConfig>>();
    public override List<ResData> GetRewardsByRank(StorageCommonLeaderBoard storageWeek, int rank)
    {
        var rewardList = new List<ResData>();
        if (storageWeek.JsonRewardConfig.IsEmptyString())
            return rewardList;
        if (!RewardConfigPool.ContainsKey(storageWeek))
        {
            var newRewardConfig = JsonConvert.DeserializeObject<List<TableJungleAdventureRankRewardConfig>>(storageWeek.JsonRewardConfig);
            RewardConfigPool.Add(storageWeek,newRewardConfig);
        }
        var rewardConfig = RewardConfigPool[storageWeek];
        TableJungleAdventureRankRewardConfig targetConfig = null;
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
        return "JungleAdventureLeaderBoard";
    }

    public override Dictionary<string, StorageCommonLeaderBoard> GetStorage()
    {
        return StorageManager.Instance.GetStorage<StorageHome>().JungleAdventureLeaderBoard;
    }

    public override void OpenMainPopup(StorageCommonLeaderBoard storageWeek)
    {
        UIJungleAdventureLeaderBoardMainController.Open(storageWeek);
    }

    public override BiEventAdventureIslandMerge.Types.ItemChangeReason GetItemChangeReason()
    {
        return BiEventAdventureIslandMerge.Types.ItemChangeReason.JungleAdventureLeaderBoardGet;
    }

    public override BiEventAdventureIslandMerge.Types.GameEventType LeaderBoardFinishGameEventType()
    {
        return BiEventAdventureIslandMerge.Types.GameEventType.GameEventJungleAdventureLeaderBoard;
    }

    public override bool CanDownloadRes(StorageCommonLeaderBoard storageWeek)
    {
        return !JungleAdventureModel.Instance.IsInitFromServer() && base.CanDownloadRes(storageWeek);
    }
}