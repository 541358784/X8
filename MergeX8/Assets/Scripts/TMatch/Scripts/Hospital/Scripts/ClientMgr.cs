
using System;
using System.Collections.Generic;
using DragonPlus;
using Framework;
using Gameplay;
// using Hospital.Logic;

namespace TMatch
{
    public partial class ClientMgr : Manager<ClientMgr>// , TaskSystem.ITask
    {
        private bool isInit;
        protected override void InitImmediately()
        {
            // ResourcesManager.Instance.RegisterTypeSuffix(typeof(Hospital.Logic.MapData), new[] {".asset"});
            PathManager.UIPrefabPath.Add("Hospital/UI/Prefabs");
        }

        // public readonly BuffManager buffManager = new BuffManager();
        //
        // public void Init()
        // {
        //     if (isInit) return;
        //     isInit = true;
        //
        //     initMapConfigs();
        //     InitLevel();
        //     // SyncEnergy();
        //     int mapId = CurMapId;
        //     var res = DynamicDownloadManager.Instance.GetDownResBundleInfo(GroupEnum.Hospital, mapId);
        //     if (res == null || res.state != DownloadState.Downloaded) mapId = Utils.MinLevelId / Utils.MapGrade;
        //     Logic.GameModel.Instance.Init(mapId, new HospitalProxy());
        //     
        //     buffManager.Init();
        // }
        //
        // public void Release()
        // {
        //     if (!isInit) return;
        //     isInit = false;
        //     
        //     buffManager.Release();
        //     
        //     Logic.GameModel.Instance.Release();
        //     
        //     ReleaseLevel();
        // }
        //
        // public void EnterGame(TinyGameFsmParam gameParam)
        // {
        //     Utils.L($"EnterGame - levelId:{gameParam.levelId}, loop:{gameParam.loopCount}");
        //     int mapId = Utils.GetMapId(gameParam.levelId);
        //     var res = DynamicDownloadManager.Instance.GetDownResBundleInfo(GroupEnum.Hospital, mapId);
        //     if (res == null)
        //     {
        //         throw new Exception($"Group = {GroupEnum.Hospital},mapId = {mapId} is not Exist");
        //     }
        //
        //     if (res.state == DownloadState.Downloaded)
        //     {
        //         _EnterGame(gameParam);
        //         return;
        //     }
        //     UIDecoDownload.Open(res, () =>
        //     {
        //         _EnterGame(gameParam);
        //     });
        // }
        //
        // private void _EnterGame(TinyGameFsmParam gameParam)
        // {
        //     int mapId = Utils.GetMapId(gameParam.levelId);
        //     Logic.GameModel.Instance.LoadMap(mapId);
        //     GameBridge.Instance.OpenGameStart();
        //     Main.Game.Fsm.ChangeState(FsmStateType.Hospital, gameParam);
        // }
        //
        // private void Update()
        // {
        //     Hospital.Logic.GameModel.Instance?.Update();
        // }

        // public List<TaskSystem.TaskEntity> FillTasks()
        // {
        //     List<TaskSystem.TaskEntity> tasks = new List<TaskSystem.TaskEntity>();
        //
        //     // tasks.Add(new TaskSystem.TaskEntity()
        //     // {
        //     //     EntityName = "DownloadNextMap",
        //     //     TaskCondition = DownNextMap
        //     // });
        //     
        //     return tasks;
        // }
    }
}