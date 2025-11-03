// using System;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using Activity.GardenTreasure.Model;
// using Deco.Node;
// using Deco.World;
// using Decoration;
// using DG.Tweening;
// using DragonPlus;
// using DragonPlus.Config.CoinRush;
// using DragonPlus.Config.GardenTreasure;
// using DragonU3DSDK;
// using DragonU3DSDK.Network.API;
// using DragonU3DSDK.Network.API.Protocol;
// using DragonU3DSDK.Storage;
// using Gameplay;
// using GamePool;
// using Newtonsoft.Json;
// using UnityEngine;
// using Random = UnityEngine.Random;
//
// public class GardenTreasureLeaderBoardModel:CommonLeaderBoardModel
// {
//     private static GardenTreasureLeaderBoardModel _instance;
//     public static GardenTreasureLeaderBoardModel Instance => _instance ?? (_instance = new GardenTreasureLeaderBoardModel());
//
//     public async void CreateStorage(StorageGardenTreasure storage)
//     {
//         await XUtility.WaitSeconds(0.1f);
//         var activityId = storage.ActivityId;
//         var jsonRewardConfig = JsonConvert.SerializeObject(GardenTreasureConfigManager.Instance.GetConfig<GardenTreasureLeaderBoardRewardConfig>());
//         var startTime = storage.PreheatEndTime;
//         var endTime = storage.ActivityEndTime;
//         var maxPlayerCount = 30;
//         var resList = new List<string>();
//         var resMd5List = ActivityManager.Instance.GetActivityMd5List(activityId);
//         foreach (var resMd5 in resMd5List)
//         {
//             var resPath = ActivityResHotUpdate.GetFilePath(resMd5);
//             resList.Add(resPath);
//         }
//         var leastStarCount = 1;
//         var auxItemAssetPath = "";
//         var taskEntranceAssetPath = "";
//         var mainPopupAssetPath = UINameConst.UIGardenTreasureLeaderBoardMain;
//         CreateStorage(activityId, jsonRewardConfig, startTime, endTime, maxPlayerCount, resList,resMd5List, leastStarCount,
//             auxItemAssetPath, taskEntranceAssetPath, mainPopupAssetPath,0);
//     }
//     private static readonly Dictionary<StorageCommonLeaderBoard, List<GardenTreasureLeaderBoardRewardConfig>> RewardConfigPool =
//         new Dictionary<StorageCommonLeaderBoard, List<GardenTreasureLeaderBoardRewardConfig>>();
//     public override List<ResData> GetRewardsByRank(StorageCommonLeaderBoard storageWeek, int rank)
//     {
//         var rewardList = new List<ResData>();
//         if (storageWeek.JsonRewardConfig.IsEmptyString())
//             return rewardList;
//         if (!RewardConfigPool.ContainsKey(storageWeek))
//         {
//             var newRewardConfig = JsonConvert.DeserializeObject<List<GardenTreasureLeaderBoardRewardConfig>>(storageWeek.JsonRewardConfig);
//             RewardConfigPool.Add(storageWeek,newRewardConfig);
//         }
//         var rewardConfig = RewardConfigPool[storageWeek];
//         GardenTreasureLeaderBoardRewardConfig targetConfig = null;
//         for (var i = 0; i < rewardConfig.Count; i++)
//         {
//             var cfg = rewardConfig[i];
//             if (cfg.RankMin <= rank && cfg.RankMax >= rank)
//             {
//                 targetConfig = cfg;
//                 break;
//             }
//         }
//         if (targetConfig == null)
//             return rewardList;
//         if (targetConfig.RewardId == null)
//             return rewardList;
//         for (var i = 0; i < targetConfig.RewardId.Count; i++)
//         {
//             rewardList.Add(new ResData(targetConfig.RewardId[i],targetConfig.RewardNum[i]));
//         }
//         return rewardList;
//     }
//
//     public override string LeadBoardKeyWord()
//     {
//         return "GardenTreasureLeaderBoard";
//     }
//
//     public override Dictionary<string, StorageCommonLeaderBoard> GetStorage()
//     {
//         return StorageManager.Instance.GetStorage<StorageHome>().GardenTreasureLeaderBoard;
//     }
//
//     public override void OpenMainPopup(StorageCommonLeaderBoard storageWeek)
//     {
//         UIGardenTreasureLeaderBoardMainController.Open(storageWeek);
//     }
//
//     public override BiEventAdventureIslandMerge.Types.ItemChangeReason GetItemChangeReason()
//     {
//         return BiEventAdventureIslandMerge.Types.ItemChangeReason.GardenTreasureLeaderBoardGet;
//     }
//
//     public override BiEventAdventureIslandMerge.Types.GameEventType LeaderBoardFinishGameEventType()
//     {
//         return BiEventAdventureIslandMerge.Types.GameEventType.GameEventGardenTreasureLeaderBoard;
//     }
//
//     public override bool CanDownloadRes(StorageCommonLeaderBoard storageWeek)
//     {
//         return !GardenTreasureModel.Instance.IsInitFromServer() && base.CanDownloadRes(storageWeek);
//     }
//
//     public void AddScore()
//     {
//         if (!GardenTreasureModel.Instance.IsInitFromServer())
//             return;
//         GetLeaderBoardStorage(GardenTreasureModel.Instance.ActivityId)?.CollectStar(1);
//     }
// }