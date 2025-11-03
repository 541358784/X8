using System.Collections.Generic;
using DragonPlus;
// using DragonPlus.ConfigHub.World;
using DragonU3DSDK;
using DragonU3DSDK.Storage;
using Gameplay;
// using Hospital.Game;
// using Hospital.Logic;
// using WorldConfig;
using UnityEngine;
using System.IO;
using System.Text;

namespace TMatch
{
    // public class GameProxyTest : GameProxy
    // {
    //     public override string FactorGuid() { return "Test"; }
    //     public override string FactorLevelName() { return "Peak Challenge"; }
    //     public override UnityEngine.Sprite FactorScoreSprite() { return null; }
    //     public override void OnLevelEnd(int levelId, GameSettlement settlement)
    //     {
    //         base.OnLevelEnd(levelId, settlement);
    //         // 在此加分数
    //         if (settlement.isFactor && settlement.isWin)
    //             Utils.E($"factor on level end, score:{settlement.factorScore}");
    //     }
    // }
    
    public partial class ClientMgr
    {
        public const string DebugTitle = "关卡";

        // public static List<DebugCfg> GetDebugCfg()
        // {
        //     var ui = UIManager.Instance.GetOpenedWindow<DebugUiController>();
        //     var finalCfg = new List<DebugCfg>();
        //     finalCfg.Add(new DebugCfg
        //     {
        //         TitleStr = "通关",
        //         ClickCallBack = (inputText1, inputText2) =>
        //         {
        //             Main.Instance.Debug_End(true, inputText1);
        //             UIManager.Instance.CloseUI<DebugUiController>();
        //         }
        //     });
        //     finalCfg.Add(new DebugCfg
        //     {
        //         TitleStr = "失败",
        //         ClickCallBack = (inputText1, inputText2) =>
        //         {
        //             Main.Instance.Debug_End(false);
        //             UIManager.Instance.CloseUI<DebugUiController>();
        //         }
        //     });
        //     finalCfg.Add(new DebugCfg
        //     {
        //         TitleStr = "添加物品",
        //         ClickCallBack = (inputText1, inputText2) =>
        //         {
        //             Main.Instance.Debug_CreateItem(inputText1);
        //         }
        //     });
        //     finalCfg.Add(new DebugCfg
        //     {
        //         TitleStr = "DebugInfo",
        //         ClickCallBack = (inputText1, inputText2) =>
        //         {
        //             Main.Instance.Debug_DebugInfo();
        //             UIManager.Instance.CloseUI<DebugUiController>();
        //         }
        //     });
        //     finalCfg.Add(new DebugCfg
        //     {
        //         TitleStr = "重置所有关卡",
        //         ClickCallBack = (inputText1, inputText2) =>
        //         {
        //             StorageManager.Instance.GetStorage<StorageHospital>().MapData.Clear();
        //             StorageManager.Instance.GetStorage<StorageHospital>().MaxLevel = 0;
        //             UIManager.Instance.CloseUI<DebugUiController>();
        //             Framework.Main.Game.Fsm.ChangeState(FsmStateType.Login, null);
        //         }
        //     });
        //     finalCfg.Add(new DebugCfg
        //     {
        //         TitleStr = "重置指定map关卡数据",
        //         ClickCallBack = (inputText1, inputText2) =>
        //         {
        //             if (!int.TryParse(inputText1, out int mapId))
        //             {
        //                 mapId = Utils.GetMapId(Instance.MainMaxLevel);
        //             }
        //             StorageHospital hospital = StorageManager.Instance.GetStorage<StorageHospital>();
        //             hospital.MapData.TryGetValue(mapId, out var map);
        //             if (map == null) return;
        //             map.Clear();
        //             Instance.MainMaxLevel = mapId * Utils.MapGrade;
        //             UIManager.Instance.CloseUI<DebugUiController>();
        //             Framework.Main.Game.Fsm.ChangeState(FsmStateType.Login, null);
        //         }
        //     });
        //     finalCfg.Add(new DebugCfg
        //     {
        //         TitleStr = "设置关卡运行倍数",
        //         ClickCallBack = (inputText1, inputText2) =>
        //         {
        //             if (!float.TryParse(inputText1, out float speed)) return;
        //             if (speed < 1) speed = 1;
        //             if (speed > 8) speed = 8;
        //             Main.Instance.TimeScale = speed;
        //             UIManager.Instance.CloseUI<DebugUiController>();
        //         }
        //     });
        //     finalCfg.Add(new DebugCfg
        //     {
        //         TitleStr = "设置最大关卡",
        //         ClickCallBack = (p1, p2) =>
        //         {
        //             if (!int.TryParse(p1, out int levelId)) return;
        //             if (levelId < 0) return;
        //             int mapId = Utils.GetMapId(Instance.CurLevelId);
        //             Maps maps = Model.Instance.GetConfigMap(mapId);
        //             if (levelId >= maps.LevelCount) levelId = maps.LevelCount;
        //             StorageHospital hospital = StorageManager.Instance.GetStorage<StorageHospital>();
        //             hospital.MapData.TryGetValue(mapId, out var map);
        //             if (map == null) return;
        //             map.MainLevelId = levelId;
        //             Instance.MainMaxLevel = mapId * Utils.MapGrade + levelId;
        //             UIManager.Instance.CloseUI<DebugUiController>();
        //             Framework.Main.Game.Fsm.ChangeState(FsmStateType.Login, null);
        //         }
        //     });
        //     finalCfg.Add(new DebugCfg
        //     {
        //         TitleStr = "打印当前关卡",
        //         ClickCallBack = (inputText1, inputText2) =>
        //         {
        //             DebugUtil.LogError($"当前关卡id：{Instance.CurLevelId}");
        //             DragonPlus.UINotice.Open(new DragonPlus.UINoticeData
        //             {
        //                 DescString = $"当前关卡id：{Instance.CurLevelId}",
        //             });
        //         }
        //     });
        //     finalCfg.Add(new DebugCfg
        //     {
        //         TitleStr = "开始关卡",
        //         ClickCallBack = (p1, p2) =>
        //         {
        //             if (!int.TryParse(p1, out int levelId) || levelId <= 0)
        //             {
        //                 UINotice.Open(new UINoticeData()
        //                 {
        //                     DescString = "请输入正确的关卡Id（如：101001）"
        //                 });
        //                 return;
        //             }
        //
        //             var maps = WorldConfigManager.Instance.GetConfig<Maps>();
        //             maps.Sort((a, b) => a.MapId.CompareTo(b.MapId));
        //             int worldId = levelId / Hospital.Utils.WorldGrade;
        //             int mapId = levelId / Hospital.Utils.MapGrade;
        //             int levelCount = (levelId % Hospital.Utils.MapGrade) - 1;
        //             if (!maps.Exists(p => p.WorldId == worldId && p.MapId == mapId))
        //             {
        //                 UINotice.Open(new UINoticeData()
        //                 {
        //                     DescString = "请输入正确的关卡Id（如：101001）"
        //                 });
        //                 return;
        //             }
        //             UIManager.Instance.CloseUI<DebugUiController>();
        //             
        //             GameBridge.Instance.OpenGameStart();
        //             
        //             StorageHospital hospital = StorageManager.Instance.GetStorage<StorageHospital>();
        //             for (int i = maps.Count - 1; i >= 0; i--)
        //             {
        //                 if (maps[i].MapId > mapId)
        //                 {
        //                     if (hospital.MapData.ContainsKey(maps[i].MapId))
        //                     {
        //                         hospital.MapData[maps[i].MapId].Clear();
        //                     }
        //                 }
        //                 else if (maps[i].MapId < mapId)
        //                 {
        //                     if (!hospital.MapData.ContainsKey(maps[i].MapId))
        //                     {
        //                         hospital.MapData.Add(maps[i].MapId, new StorageHospitalMap());
        //                     }
        //
        //                     hospital.MapData[maps[i].MapId].MapId = maps[i].MapId;
        //                     hospital.MapData[maps[i].MapId].MainLevelId = maps[i].LevelCount;
        //                 }
        //                 else
        //                 {
        //                     if (!hospital.MapData.ContainsKey(maps[i].MapId))
        //                     {
        //                         hospital.MapData.Add(maps[i].MapId, new StorageHospitalMap());
        //                     }
        //                     
        //                     hospital.MapData[maps[i].MapId].MapId = maps[i].MapId;
        //                     if (levelCount >= maps[i].LevelCount)
        //                     {
        //                         levelCount = maps[i].LevelCount - 1;
        //                         levelId = mapId * Hospital.Utils.MapGrade + levelCount + 1;
        //                     }
        //                     hospital.MapData[maps[i].MapId].MainLevelId = levelCount;
        //                 }
        //             }
        //             var bundle = DynamicDownloadManager.Instance.GetDownResBundleInfo(GroupEnum.Hospital, mapId);
        //             if (bundle == null) return;
        //
        //             void Call()
        //             {
        //                 GameModel.Instance.LoadMap(mapId);
        //                 Maps maps = Model.Instance.GetConfigMap(mapId);
        //                 if (levelCount + 1 >= maps.LevelCount) levelCount = maps.LevelCount - 1;
        //                 var gameParam = new TinyGameFsmParam();
        //                 gameParam.levelId = levelId;
        //                 // gameParam.proxyCustom = new GameProxyTest();
        //                 Instance.MainMaxLevel = levelId - 1;
        //                 Instance.LevelCountInit(true);
        //                 // gameParam.boosts = new List<BoostTypeEnum>();
        //                 Framework.Main.Game.Fsm.ChangeState(FsmStateType.Hospital, gameParam);
        //             }
        //
        //             if (bundle.state != DownloadState.Downloaded)
        //             {
        //                 DynamicDownloadManager.Instance.Download(bundle);
        //                 UIDecoDownload.Open(bundle,Call);
        //             }
        //             else
        //             {
        //                 Call();
        //             }
        //         }
        //     });
        //     
        //     finalCfg.Add(new DebugCfg
        //     {
        //         TitleStr = "无尽关卡轮数",
        //         ClickCallBack = (inputText1, inputText2) =>
        //         {
        //             if (!int.TryParse(inputText1, out int mapId)) return;
        //             StorageHospitalMap mapData = Instance.GetStorageMap(mapId);
        //             if (mapData == null) return;
        //             string str = $"map：{mapId}  当前无尽轮数：{mapData.LoopLevelId}";
        //             DragonPlus.UINotice.Open(new DragonPlus.UINoticeData
        //             {
        //                 DescString = str,
        //             });
        //             DebugUtil.LogError(str);
        //             UIManager.Instance.CloseUI<DebugUiController>();
        //         }
        //     });
        //     finalCfg.Add(new DebugCfg
        //     {
        //         TitleStr = "重置无尽关卡轮数",
        //         ClickCallBack = (inputText1, inputText2) =>
        //         {
        //             if (!int.TryParse(inputText1, out int mapId)) return;
        //             StorageHospitalMap mapData = Instance.GetStorageMap(mapId);
        //             if (mapData == null) return;
        //             mapData.LoopLevelId = 0;
        //             UIManager.Instance.CloseUI<DebugUiController>();
        //         }
        //     });
        //     finalCfg.Add(new DebugCfg
        //     {
        //         TitleStr = "重置因子",
        //         ClickCallBack = (inputText1, inputText2) =>
        //         {
        //             StorageManager.Instance.GetStorage<StorageHospital>().FactorData.Clear();
        //         }
        //     });
        //     finalCfg.Add(new DebugCfg
        //     {
        //         TitleStr = "设置复活触发系数",
        //         ClickCallBack = (inputText1, inputText2) =>
        //         {
        //             int.TryParse(inputText1, out var count);
        //             if (count < 0) return;
        //             DragonU3DSDK.Storage.StorageManager.Instance.GetStorage<DragonU3DSDK.Storage.StorageHospital>().ReviveFailedCount = count;
        //             UnityEngine.Debug.Log($"复活触发系数：{DragonU3DSDK.Storage.StorageManager.Instance.GetStorage<DragonU3DSDK.Storage.StorageHospital>().ReviveFailedCount}");
        //
        //             UIManager.Instance.CloseUI<DebugUiController>();
        //         }
        //     }); 
        //     finalCfg.Add(new DebugCfg
        //     {
        //         TitleStr = "设置已免费复活次数",
        //         ClickCallBack = (inputText1, inputText2) =>
        //         {
        //             int.TryParse(inputText1, out var count);
        //             if (count < 0) return;
        //             DragonU3DSDK.Storage.StorageManager.Instance.GetStorage<DragonU3DSDK.Storage.StorageHospital>().FreeReviveLevelTimes = count;
        //             UnityEngine.Debug.Log($"当前免费复活使用次数：{DragonU3DSDK.Storage.StorageManager.Instance.GetStorage<DragonU3DSDK.Storage.StorageHospital>().FreeReviveLevelTimes} ，总免费复活次数：{Hospital.Config.MapCommon.ConfigManager.Instance.LevelBaseList[0].FreeReviveLevelTimes}");
        //             UnityEngine.Debug.Log($"复活触发系数：{DragonU3DSDK.Storage.StorageManager.Instance.GetStorage<DragonU3DSDK.Storage.StorageHospital>().ReviveFailedCount}");
        //
        //             UIManager.Instance.CloseUI<DebugUiController>();
        //         }
        //     });
        //     finalCfg.Add(new DebugCfg
        //     {
        //         TitleStr = "添加Buff",
        //         ClickCallBack = (inputText1, inputText2) =>
        //         {
        //             int.TryParse(inputText1, out var id);
        //             if (id < 0) return;
        //             int.TryParse(inputText2, out var seconds);
        //             if (seconds < 0) return;
        //             Instance.buffManager.AddBuff(id, seconds);
        //         }
        //     });
        //     finalCfg.Add(new DebugCfg
        //     {
        //         TitleStr = "查看Buff",
        //         ClickCallBack = (inputText1, inputText2) =>
        //         {
        //             DragonPlus.UINotice.Open(new DragonPlus.UINoticeData
        //             {
        //                 TitleString = "Buff",
        //                 DescString = Instance.buffManager.Dump(),
        //                 Width = 1200,
        //                 Height = 700
        //             });
        //         }
        //     });
        //     finalCfg.Add(new DebugCfg
        //     {
        //         TitleStr = "清空Buff",
        //         ClickCallBack = (inputText1, inputText2) =>
        //         {
        //             Instance.buffManager.ClearAll();
        //         }
        //     });
        //     finalCfg.Add(new DebugCfg
        //     {
        //         TitleStr = "输出Map路径距离信息",
        //         ClickCallBack = (inputText1, inputText2) =>
        //         {
        //             // Hospital.Config.MapData.ConfigManager.Instance.GetAllPathDistances();
        //             
        //             Config.MapData.ConfigManager.Instance.ComputeDistanceTable(out StringBuilder stringBuilder);
        //             string folderPath = Path.Combine(Application.dataPath, "MapPathInfo");
        //             string filePath = Path.Combine(folderPath, $"Map{Main.Instance.MapId}_PathInfoFile.txt");
        //             Directory.CreateDirectory(folderPath);
        //             using (StreamWriter writer = new StreamWriter(filePath, false))
        //             {
        //                 writer.Write(stringBuilder);
        //             }
        //             
        //             UIManager.Instance.CloseUI<DebugUiController>();
        //             UINotice.Open(new UINoticeData
        //             {
        //                 TitleString = "路径信息",
        //                 DescString = $"已输出Map路径距离信息文件:{filePath}",
        //                 Width = 1200,
        //                 Height = 700
        //             });
        //         }
        //     });
        //     finalCfg.Add(new DebugCfg
        //     {
        //         TitleStr = "强制使用Map_B配置",
        //         ClickCallBack = (inputText1, inputText2) =>
        //         {
        //             Main.DebugForceUseMapB = true;
        //             UINotice.Open(new UINoticeData()
        //             {
        //                 DescString = "The ABTest group is ignored and force use Map_B config."
        //             });
        //             GameModel.Instance.LoadMap(GameModel.Instance.MapId, true);
        //         }
        //     });
        //     finalCfg.Add(new DebugCfg
        //     {
        //         TitleStr = "取消强制Map_B配置",
        //         ClickCallBack = (inputText1, inputText2) =>
        //         {
        //             Main.DebugForceUseMapB = false;
        //             UINotice.Open(new UINoticeData()
        //             {
        //                 DescString = "Force use Map_B is cancelled. Use config based on the ABTest group."
        //             });
        //             GameModel.Instance.LoadMap(GameModel.Instance.MapId, true);
        //         }
        //     });
        //     finalCfg.Add(new DebugCfg
        //     {
        //         TitleStr = "是否强制使用Map_B配置",
        //         ClickCallBack = (inputText1, inputText2) =>
        //         {
        //             string result = Main.DebugForceUseMapB ? "Yes" : "No";
        //             UINotice.Open(new UINoticeData()
        //             {
        //                 DescString = $"Is force use Map_B config : {result}"
        //             });
        //         }
        //     });
        //     return finalCfg;
        // }
    }
}