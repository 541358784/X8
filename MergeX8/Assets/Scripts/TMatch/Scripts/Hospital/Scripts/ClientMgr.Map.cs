using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DragonPlus;
using DragonU3DSDK.Storage;
// using DragonPlus.ConfigHub.World;
using DragonU3DSDK;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;
// using Hospital.Config.Map;
// using Hospital.Game;
// using Hospital.Logic;
using Newtonsoft.Json;
// using TaskSystem;
using UnityEngine;

namespace TMatch
{
    // public partial class EventEnum
    // {
    //     public const string OnEnterNextMap = "OnEnterNextMap"; //开始下一个新map
    // }
    // public readonly struct EventOnLevelCountChanged : IEvent
    // {
    // }
    public partial class ClientMgr
    {
        public int MainMaxLevel
        {
            get => 0;
            // get => StorageManager.Instance.GetStorage<StorageHospital>().MaxLevel;
            // set
            // {
            //     StorageManager.Instance.GetStorage<StorageHospital>().MaxLevel = value;
            //     StorageManager.Instance.GetStorage<StorageHospital>().MaxMap = StorageManager.Instance.GetStorage<StorageHospital>().MaxLevel / Utils.MapGrade;
            // }
        }

        // /// <summary>
        // /// 当前消耗的体力数
        // /// </summary>
        // public int CurrentCostEnergy
        // {
        //     get => StorageManager.Instance.GetStorage<StorageHospital>().CostEnergyCount;
        //     set => StorageManager.Instance.GetStorage<StorageHospital>().CostEnergyCount = value;
        // }

        // private readonly Dictionary<int, Maps> mapsById = new Dictionary<int, Maps>();

        // private void initMapConfigs()
        // {
        //     mapsById.Clear();
        //     var maps = WorldConfigManager.Instance.GetConfig<Maps>();
        //     foreach (var map in maps)
        //         mapsById[map.MapId] = map;
        // }

        /// <summary>
        /// 只做结算标识用
        /// </summary>
        // public bool IsCostEnergy { get; set; }
        //
        // private readonly Dictionary<eLevelRuleType, int> LevelCountByRule = new Dictionary<eLevelRuleType, int>();
        // private long _factorAllScore;
        // private void LevelCountInit(bool notify)
        // {
        //     var storageHospital = StorageManager.Instance.GetStorage<StorageHospital>();
        //     LevelCountByRule[eLevelRuleType.Main] = 1;
        //     LevelCountByRule[eLevelRuleType.Loop] = 0;
        //     LevelCountByRule[eLevelRuleType.Factor] = 0;
        //     
        //     foreach (var mapData in storageHospital.MapData)
        //     {
        //         LevelCountByRule[eLevelRuleType.Main] += mapData.Value.MainLevelId;
        //         LevelCountByRule[eLevelRuleType.Loop] += mapData.Value.LoopLevelId;
        //     }
        //     foreach (var kv in storageHospital.FactorScore)
        //     {
        //         LevelCountByRule[eLevelRuleType.Factor] += kv.Value;
        //     }
        //     _factorAllScore += storageHospital.FactorAllScore;
        //
        //     foreach (var kv in LevelCountByRule)
        //     {
        //         Utils.L($"[LevelCount] Init {kv.Key.ToString()} - {kv.Value}");
        //     }
        //     
        //     if (notify)
        //         EventBus.Notify(new EventOnLevelCountChanged());
        // }
        // public int LevelCountGet(eLevelRuleType t)
        // {
        //     return !LevelCountByRule.ContainsKey(t) ? 0 : LevelCountByRule[t];
        // }

        // public long FactorAllScoreGet()
        // {
        //     return _factorAllScore < 1 ? LevelCountGet(eLevelRuleType.Factor) : _factorAllScore;
        // }
        //
        // public int MainLevelCountGet()
        // {
        //     return LevelCountGet(eLevelRuleType.Main) - 1;
        // }

        // public int SumLevelCountGet()
        // {
        //     return MainLevelCountGet() + LevelCountGet(eLevelRuleType.Loop) + LevelCountGet(eLevelRuleType.Factor);
        // }
        //
        // private void LevelCountAdd(eLevelRuleType t, int count)
        // {
        //     if (!LevelCountByRule.ContainsKey(t))
        //         return;
        //     LevelCountByRule[t] += count;
        //     
        //     Utils.L($"[LevelCount] Add {t.ToString()} - {LevelCountByRule[t]}");
        //     EventBus.Notify(new EventOnLevelCountChanged());
        // }
        //
        // private void FactorAllScoreAdd(long count , bool isOverr = false)
        // {
        //     if (isOverr)
        //         _factorAllScore = count;
        //     else
        //         _factorAllScore += count;
        // }
        //
        // public int LevelCountTotal()
        // {
        //     return LevelCountByRule.Sum(kv => kv.Value);
        // }

        // 当前通关数
        // public int CurMaxLevelCount { get; set; }

        // 当前关卡Id
        // public int CurLevelId => MainMaxLevel == 0 ? Utils.MinLevelId + 1 : MainMaxLevel + 1;

        public int CurLevelId => 0;
        // public int CurMapId => CurLevelId / Utils.MapGrade;

        // public int CurMapLoopCount
        // {
        //     get => StorageManager.Instance.GetStorage<StorageHospital>().CurrentLoop;
        //     set => StorageManager.Instance.GetStorage<StorageHospital>().CurrentLoop = value;
        // }
        //
        // public int PassLevelCount { get; set; }
        // public int PassLevelSuccessCount { get; set; }
        // public int LastPassLevelId { get; set; }
        //
        // public List<int> NewGuest { get; set; }

        // private void InitLevel()
        // {
        //     var storageHospital = StorageManager.Instance.GetStorage<StorageHospital>();
        //     if (storageHospital.MaxLevel < Utils.MinLevelId)
        //         storageHospital.MaxLevel = Utils.MinLevelId;
        //     if (storageHospital.MaxMap <= 0)
        //         storageHospital.MaxMap = storageHospital.MaxLevel / Utils.MapGrade;
        //     
        //     LevelCountInit(false);
        //
        //     // 数据兼容
        //     ProcessLevelId();
        //     PassLevelCount = 0;
        //     PassLevelSuccessCount = 0;
        // }

        // private void ReleaseLevel()
        // {
        // }


        // /// <summary>
        // /// map是否解锁
        // /// </summary>
        // /// <param name="mapId">mapId</param>
        // /// <returns></returns>
        // public bool IsUnlockMap(int mapId)
        // {
        //     return Utils.GetMapId(MainMaxLevel) >= mapId;
        // }
        //
        // public bool IsLastMap()
        // {
        //     var maps = WorldConfigManager.Instance.GetConfig<Maps>();
        //     if (maps == null || maps.Count == 0) return false;
        //     return CurMapId == maps[maps.Count - 1].MapId;
        // }

        // public int GetLastMapId()
        // {
        //     var maps = WorldConfigManager.Instance.GetConfig<Maps>();
        //     if (maps == null || maps.Count == 0) return Utils.MinLevelId / Utils.MapGrade;
        //     return maps[maps.Count - 1].MapId;
        // }
        //
        // public bool IsClearLastMap()
        // {
        //     if (!IsLastMap()) return false;
        //     return IsClearLevel(CurMapId);
        // }

        // /// <summary>
        // /// 指定关卡是否是循环关
        // /// </summary>
        // /// <param name="levelId"></param>
        // /// <returns></returns>
        // public eLevelType GetLevelType(int levelId)
        // {
        //     return IsClearLevel(Utils.GetMapId(levelId)) ? eLevelType.Loop : eLevelType.Main;
        // }

        // public int GetLevelCount(int mapId)
        // {
        //     var storageMap = GetStorageMap(mapId);
        //     if (storageMap == null)
        //         return 0;
        //     return storageMap.LevelCount;
        // }

        // public Maps GetMapConfig(int mapId)
        // {
        //     return mapsById.ContainsKey(mapId) ? mapsById[mapId] : null;
        //     // var maps = WorldConfigManager.Instance.GetConfig<Maps>();
        //     // if (maps == null || maps.Count == 0)
        //     //     return null;
        //     // return maps.Find(p => p.MapId == mapId);
        // }

        // public int CurrentMapMaxLevelCount()
        // {
        //     var maps = WorldConfigManager.Instance.GetConfig<Maps>();
        //     if (maps == null || maps.Count == 0)
        //         return 0;
        //     int count = 0;
        //     for (int i = 0; i < maps.Count; i++)
        //     {
        //         if (maps[i].MapId > CurMapId) continue;
        //         count += maps[i].LevelCount;
        //     }
        //
        //     return count;
        // }

        // public Maps GetMapConfigNext(int mapId)
        // {
        //     var mapConfig = GetMapConfig(mapId);
        //     return mapConfig == null ? null : GetMapConfig(mapConfig.OpenMap);
        // }
        //
        // /// <summary>
        // /// 指定map是否通关
        // /// </summary>
        // /// <param name="mapId"></param>
        // /// <returns></returns>
        // public bool IsClearLevel(int mapId)
        // {
        //     var map = GetMapConfig(mapId);
        //     if (map == null)
        //         return false;
        //     var storageMap = GetStorageMap(mapId);
        //     if (storageMap == null)
        //         return false;
        //     return storageMap.MainLevelId >= map.LevelCount;
        // }

        // /// <summary>
        // /// 指定Map是否开启
        // /// </summary>
        // /// <param name="mapId"></param>
        // /// <returns></returns>
        // public bool IsLevelOpening(int mapId)
        // {
        //     var map = GetMapConfig(mapId);
        //     if (map == null)
        //         return false;
        //     var storageMap = GetStorageMap(mapId);
        //     if (storageMap == null)
        //         return false;
        //     return storageMap.MainLevelId < map.LevelCount;
        // }

        // /// <summary>
        // /// 获取指定map存储
        // /// </summary>
        // /// <param name="mapId">指定mapId</param>
        // /// <returns></returns>
        // public StorageHospitalMap GetStorageMap(int mapId)
        // {
        //     StorageManager.Instance.GetStorage<StorageHospital>().MapData.TryGetValue(mapId, out var storage);
        //     if (null == storage)
        //     {
        //         storage = new StorageHospitalMap
        //         {
        //             LoopLevelId = 0,
        //             MapId = mapId
        //         };
        //         StorageManager.Instance.GetStorage<StorageHospital>().MapData.Add(mapId, storage);
        //     }
        //
        //     return storage;
        // }

        /// <summary>
        /// 同步体力
        /// </summary>
        public void SyncEnergy()
        {
            // if (CurrentCostEnergy <= 0) return;
            // var arg = new DragonPlus.GameBIManager.ItemChangeReasonArgs();
            // arg.reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.PlayLevel;
            // arg.data1 = $"none";
            // arg.data2 = $"none";
            // ItemModel.Instance.Cost((int) ResourceId.Energy, CurrentCostEnergy, arg, true);
            // CurrentCostEnergy = 0;
        }

        // private bool ProcessLevelId()
        // {
        //     var mapId = Utils.GetMapId(MainMaxLevel);
        //     bool isClear = IsClearLevel(mapId);
        //     // map 全通了
        //     if (isClear)
        //     {
        //         var nextMap = GetMapConfigNext(mapId);
        //         if (nextMap != null)
        //         {
        //             MainMaxLevel = nextMap.MapId * Utils.MapGrade;
        //             var data = GetStorageMap(nextMap.MapId);
        //             CurMapLoopCount = data.LoopLevelId;
        //             GameModel.Instance.SetDirty();
        //             EventDispatcher.Instance.DispatchEvent(new BaseEvent(EventEnum.OnEnterNextMap));
        //         }
        //     }
        //     
        //     // //处理存档已经提前解锁当前版本不存在的新Map的情况（新版本的存档回滾老版本客户端）
        //     // mapId = Utils.GetMapId(MainMaxLevel);
        //     // var mapCfg = GetMapConfig(mapId);
        //     // if (mapCfg == null)
        //     // {
        //     //     var lastMapId = GetLastMapId();
        //     //     mapCfg = GetMapConfig(lastMapId);
        //     //     if (mapCfg != null)
        //     //     {
        //     //         StorageManager.Instance.GetStorage<StorageHospital>().MapData.Remove(mapId);
        //     //         MainMaxLevel = lastMapId * Utils.MapGrade + GetStorageMap(lastMapId).MainLevelId;
        //     //     }
        //     // }
        //
        //     CurMaxLevelCount = CurrentMapMaxLevelCount();
        //
        //     return isClear;
        // }

        // public void OnLevelStart(int levelId)
        // {
        //     var mapId = Utils.GetMapId(levelId);
        //     var mapData = GetStorageMap(mapId);
        //     mapData.LevelCount++;
        // }
        //
        // public void OnLevelQuit(int levelId)
        // {
        //     var storage = StorageManager.Instance.GetStorage<StorageHospital>();
        //     storage.FailedCount++; //记录连败次数
        //     storage.FailedCountGuide++; //记录连败次数
        //     if (IsEnableFreeReviveLevel())
        //     {
        //         storage.ReviveFailedCount++; //记录免费复活触发系数
        //         UnityEngine.Debug.Log($"复活触发系数：{storage.ReviveFailedCount}");
        //     }
        //
        //     GiftPack.Model.Instance.AddMapFailTime();
        //     Activity.WinStreak2.Model.Instance.OnMapFailed();
        // }
        //
        // public void OnLevelEnd(int levelId, GameSettlement settlement)
        // {
        //     settlement.level = levelId;
        //     
        //     if (settlement.isWin)
        //     {
        //         EventBus.Notify(new EventOnAddData<int>(DataCenter.NameConst.CompleteAnyLevels, DataCenter.eOperateType.Add, 1));
        //         EventBus.Notify(new EventOnAddData<int>(DataCenter.NameConst.SuccessAnyLevels, DataCenter.eOperateType.Add, 1));
        //     }
        //     
        //     if (settlement.isLoop && settlement.isWin)
        //         EventBus.Notify(new EventOnAddData<int>(GeneralGameMission.MissionConst.CompleteLoopLevel, DataCenter.eOperateType.Add, 1));
        //     
        //     var storage = StorageManager.Instance.GetStorage<StorageHospital>();
        //     if (!settlement.isWin)
        //     {
        //         storage.FailedCount++; //记录连败次数
        //         storage.FailedCountGuide++; //记录连败次数
        //         GiftPack.Model.Instance.AddMapFailTime();
        //         if (!settlement.isLoop)
        //         {
        //             //记录当前主线关累计失败次数
        //             storage.CurrentMainLevelFailedCount++;
        //         }
        //     }
        //     else
        //     {
        //         storage.FailedCount = 0; //连败次数清零
        //         storage.FailedCountGuide = 0; //连败次数清零
        //         storage.ReviveFailedCount = 0; //复活触发系数清零
        //         GiftPack.Model.Instance.ClearMapFailTime();
        //         if (!settlement.isLoop)
        //         {
        //             //当前主线关累计失败次数清零
        //             storage.CurrentMainLevelFailedCount = 0;
        //         }
        //     }
        //     
        //     if (settlement.isWin && settlement.isFactor)
        //     {
        //         // 活动关 处理数据
        //         var factorGuid = Main.Instance.GameProxy.FactorGuid();
        //         var factorScore = storage.FactorScore;
        //         if (!factorScore.ContainsKey(factorGuid))
        //             factorScore[factorGuid] = 1;
        //         else
        //             factorScore[factorGuid]++;
        //         LevelCountAdd(eLevelRuleType.Factor, 1);
        //
        //         if (storage.FactorAllScore <= 0)
        //         {
        //             var addScore = settlement.factorScore;
        //             storage.FactorAllScore = (factorScore[factorGuid] - 1) + addScore;
        //             FactorAllScoreAdd(storage.FactorAllScore,true);
        //         }
        //         else
        //         {
        //             var addScore = settlement.factorScore;
        //             storage.FactorAllScore += addScore;
        //             FactorAllScoreAdd(addScore);
        //         }
        //         
        //         return;
        //     }
        //     
        //     // 结算
        //     var arg = new DragonPlus.GameBIManager.ItemChangeReasonArgs {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.PassLevel};
        //     if (settlement.key > 0)
        //     {
        //         ItemModel.Instance.Add((int) ResourceId.BoxKey, settlement.key, arg);
        //
        //         if (settlement.isLoop)
        //             Hospital.EventBus.Notify(new Hospital.Game.EventOnAddData<int>(DataCenter.NameConst.CollectKeys, DataCenter.eOperateType.Add, settlement.key));
        //     }
        //
        //     if (settlement.star > 0)
        //         ItemModel.Instance.Add((int) ResourceId.Star, settlement.star, arg);
        //     if (settlement.coin > 0)
        //     {
        //         ItemModel.Instance.Add((int) ResourceId.Coin, settlement.coin, new DragonPlus.GameBIManager.ItemChangeReasonArgs()
        //         {
        //             reason = settlement.isWin ? BiEventAdventureIslandMerge.Types.ItemChangeReason.PassLevel : BiEventAdventureIslandMerge.Types.ItemChangeReason.FailLevel,
        //             data1 = $"{levelId}",
        //             data2 = $"{(settlement.isLoop ? "loop" : "main")}"
        //         });
        //         
        //         EventBus.Notify(new EventOnAddData<int>(GeneralGameMission.MissionConst.EarnCoinByLevel, DataCenter.eOperateType.Add, settlement.coin));
        //     }
        //
        //     PassLevelCount++;
        //     
        //     if (!settlement.isWin)
        //         return;
        //
        //     PassLevelSuccessCount++;
        //     if (!settlement.isLoop) LastPassLevelId = levelId;
        //
        //     // 通关处理
        //     var mapId = Utils.GetMapId(levelId);
        //     var mapData = GetStorageMap(mapId);
        //     //应策划需求，levelCount含义变更为当前关卡一共Play多少次，发送bi后置零
        //     mapData.LevelCount = 0;
        //     if (IsClearLevel(mapId))
        //     {
        //         mapData.LoopLevelId++;
        //         if (mapId == CurMapId) CurMapLoopCount = mapData.LoopLevelId;
        //         mapData.LoopLevelIndex = -1; //这里重置随机关卡的索引
        //         LevelCountAdd(eLevelRuleType.Loop, 1);
        //     }
        //     else
        //     {
        //         SendPassLevelBI();
        //         mapData.MainLevelId++;
        //
        //         if (MainMaxLevel < levelId)
        //         {
        //             MainMaxLevel = levelId;
        //             if (ProcessLevelId()) //刚刚跨map
        //             {
        //                 storage.IsUnlockNewLoop = true;
        //             }
        //         }
        //
        //         Hospital.EventBus.Notify(new Hospital.Game.EventOnAddData<int>(DataCenter.NameConst.CompleteLevels, DataCenter.eOperateType.Add, 1));
        //         Hospital.EventBus.Notify(new Hospital.Game.EventOnAddData<int>(DataCenter.NameConst.CompleteHospital, DataCenter.eOperateType.Add, 1));
        //
        //         LevelCountAdd(eLevelRuleType.Main, 1);
        //     }
        //
        //     NewGuest = Hospital.Logic.GameModel.Instance.GetGuestsNew(mapData.MapId);
        //     SetAllGuestsNewToOld(mapId);
        //     Guild.Model.Instance.SyncInfo();
        // }

        // private void SendPassLevelBI()
        // {
        //     if (Main.Instance.ConfigLevel == null) return;
        //     if (string.IsNullOrEmpty(Main.Instance.ConfigLevel.Bi)) return;
        //     DragonPlus.GameBIManager.TryParseGameEventType(Main.Instance.ConfigLevel.Bi, out var gameEventType);
        //     DragonPlus.GameBIManager.SendGameEvent(gameEventType);
        // }
        //
        // public Level GetLevelConfigById(int levelId, int loopCount)
        // {
        //     var mapId = Utils.GetMapId(levelId);
        //     if (loopCount < 0)
        //     {
        //         return ConfigManager.Instance.GetConfig<Level>(levelId);
        //     }
        //
        //     var mapData = GetStorageMap(mapId);
        //     var levelLoops = ConfigManager.Instance.GetConfigs<Level>("levelloop");
        //     var currentLoopIndex = UnityEngine.Random.Range(0, levelLoops.Count); //这里走纯随机
        //     //比较当前存储的随机值
        //     if (mapData.LoopLevelIndex >= 0) currentLoopIndex = mapData.LoopLevelIndex;
        //     mapData.LoopLevelIndex = currentLoopIndex; //存储当前的随机索引
        //     var loopConfig = levelLoops[currentLoopIndex];
        //     var config = CloneLoopData(loopConfig);
        //     // config.Id = levelId;
        //     var mapUser = ConfigManager.Instance.Base;
        //     var baseLevel = config.DiseaseIntensity;
        //     baseLevel += (mapData.LoopLevelId / mapUser.DiseaseIntensityAddDur); //重新计算当前的疾病等级
        //     if (baseLevel > mapUser.LoopMaxDiseaseIntensity) baseLevel = mapUser.LoopMaxDiseaseIntensity;
        //     config.DiseaseIntensity = baseLevel;
        //
        //     float factor = 0;
        //     switch ((eWinType) config.WinType)
        //     {
        //         case eWinType.GuestCure:
        //             factor = mapUser.LoopWinPersonAdd;
        //             break;
        //         case eWinType.Coins:
        //             factor = mapUser.LoopWinGoldAdd;
        //             break;
        //         default:
        //             throw new ArgumentOutOfRangeException();
        //     }
        //
        //     for (int i = 0; i < config.WinParam.Count; i++) //这里重新计算通关条件
        //     {
        //         var param = mapData.LoopLevelId;
        //         config.WinParam[i] += Mathf.FloorToInt(param * factor);
        //         // DebugUtil.LogError($"WinParam:{config.WinParam[i]}");
        //     }
        //
        //     // DebugUtil.LogError($"winCount:{mapData.LoopLevelId}");
        //     return config;
        // }

        // private static Level CloneLoopData(Level levelLoop)
        // {
        //     if (levelLoop == null)
        //         return null;
        //     var json = JsonConvert.SerializeObject(levelLoop);
        //     return JsonConvert.DeserializeObject<Level>(json);
        // }

        // /// <summary>
        // /// 返回循环关轮数,如果还在主线关卡，返回-1
        // /// </summary>
        // /// <param name="mapId">指定mapid</param>
        // /// <returns></returns>
        // public int GetLoopCount(int mapId)
        // {
        //     var mapData = GetStorageMap(mapId);
        //     if (!IsClearLevel(mapId)) return -1;
        //     return mapData.LoopLevelId;
        // }
        //
        // private void SetAllGuestsNewToOld(int mapId)
        // {
        //     var mapData = GetStorageMap(mapId);
        //     if (null == mapData) return;
        //     try
        //     {
        //         var guestsNew = Hospital.Logic.GameModel.Instance.GetGuestsNew(mapData.MapId);
        //         if ((null != guestsNew) && (guestsNew.Count > 0))
        //         {
        //             foreach (var roleID in guestsNew)
        //             {
        //                 Hospital.Logic.GameModel.Instance.SetGuestKnown(mapData.MapId, roleID);
        //                 Hospital.EventBus.Notify(new Hospital.Game.EventOnAddData<int>(("CollectAllPatients" + mapData.MapId.ToString()), DataCenter.eOperateType.Add,
        //                     1));
        //             }
        //         }
        //     }
        //     catch (System.Exception e)
        //     {
        //         DebugUtil.LogError(e.Message);
        //     }
        // }

        // /// <summary>
        // /// 获取下一个mapId
        // /// </summary>
        // /// <returns></returns>
        // public int GetNextMapId()
        // {
        //    var mapdata = GetMapConfigNext(CurMapId);
        //    return mapdata == null ? 0 : mapdata.MapId;
        // }
        //
        // public ResBundleInfo GetMapInfo(int mapId)
        // {
        //     if (mapId <= 0) return null;
        //     var bundle = DynamicDownloadManager.Instance.GetDownResBundleInfo(GroupEnum.Hospital, mapId); //下载下一个world的第一个map
        //     return bundle;
        // }

        // /// <summary>
        // /// 下载下一个map
        // /// </summary>
        // /// <returns></returns>
        // public bool DownNextMap()
        // {
        //     var bundle = GetMapInfo(GetNextMapId());
        //     if (bundle != null && (bundle.state == DownloadState.NeedDownload || bundle.state == DownloadState.DownFailed)) DynamicDownloadManager.Instance.Download(bundle);
        //     var currentBundle = GetMapInfo(CurMapId);
        //     if (currentBundle != null && (currentBundle.state == DownloadState.NeedDownload || currentBundle.state == DownloadState.DownFailed)) DynamicDownloadManager.Instance.Download(currentBundle);
        //     return true;
        // }

        // 随机获取非最后一个map id
        // public int RandomGetMapIdNotLast()
        // {
        //     var mapList = new List<Maps>();
        //     foreach (var kv in mapsById)
        //     {
        //         if (kv.Value.OpenMap > 0)
        //             mapList.Add(kv.Value);
        //     }
        //     if (mapList.Count == 0)
        //         return -1;
        //     return mapList[Utils.RandomRange(0, mapList.Count)].MapId;
        // }
        //
        // /// 当前关卡是否满足免费复活
        // public bool IsEnableFreeReviveLevel()
        // {
        //     var commonCfg = Hospital.Config.MapCommon.ConfigManager.Instance.LevelBaseList[0];
        //
        //     var levelId = ClientMgr.Instance.CurLevelId;
        //     return levelId >= commonCfg.FreeReviveLevelRange[0] && levelId <= commonCfg.FreeReviveLevelRange[1];
        // }

        // /// <summary>
        // /// 获取上一个map
        // /// </summary>
        // /// <param name="map"></param>
        // /// <returns></returns>
        // public int GetPreMap(int map)
        // {
        //     var maps = WorldConfigManager.Instance.GetConfig<Maps>();
        //     Maps config = null;
        //     foreach (var t in maps)
        //     {
        //         if (map == t.MapId)
        //             break;
        //         config = t;
        //     }
        //
        //     return config?.MapId ?? -1;
        // }
        //
        // /// <summary>
        // /// 获取关卡总数
        // /// </summary>
        // /// <param name="level">关卡</param>
        // /// <returns></returns>
        // public int GetLevelTotal(int level)
        // {
        //     var total = 0;
        //     var map = level / 1000;
        //     foreach (var element in mapsById)
        //     {
        //         if (element.Key < map)
        //             total += element.Value.LevelCount;
        //         if (element.Key == map)
        //             total += level % 1000;
        //     }
        //
        //     return total;
        // }

        // /// <summary>
        // /// 获取上一个关卡
        // /// </summary>
        // /// <param name="level">关卡</param>
        // /// <returns></returns>
        // public int GetPreLevel(int level)
        // {
        //     var map = level / 1000;
        //     var id = level % 1000;
        //     if (id == 1)
        //     {
        //         map = GetPreMap(map);
        //         if (map == -1)
        //             return -1;
        //         return mapsById[map].MapId * 1000 + mapsById[map].LevelCount;
        //     }
        //     return level - 1;
        // }

        // /// <summary>
        // /// 获取关卡间隔
        // /// </summary>
        // /// <param name="level1">关卡1</param>
        // /// <param name="level2">关卡2</param>
        // /// <returns></returns>
        // public int GetLevelDelta(int level1, int level2)
        // {
        //     return Math.Abs(GetLevelTotal(level1) - GetLevelTotal(level2));
        // }
    }
}