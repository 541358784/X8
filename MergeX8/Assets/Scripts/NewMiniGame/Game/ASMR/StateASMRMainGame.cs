// using System;
// using System.Collections;
// using System.Threading.Tasks;
// 
// using UnityEngine;
// using DragonU3DSDK;
// using DragonU3DSDK.Audio;
// using Framework;
//
// namespace Gameplay
// {
//     public class ASMRMainGameFsmParam : FsmParam
//     {
//         public ASMRMainGameFsmParam(int levelId, int homeId, int taskId)
//         {
//             this.levelId = levelId;
//             this.homeId = homeId;
//             this.taskId = taskId;
//         }
//
//         // 关卡配置
//         public int levelId;
//         public int homeId;
//         public int taskId;
//     }
//
//     public class StateASMRMainGame : IFsmState
//     {
//         FsmStateType IFsmState.Type => FsmStateType.ASMRMain;
//         private ASMRMainGameFsmParam _param;
//
//         public Task<bool> PreEnterAsync(FsmParam param, FsmStateType fromStateType)
//         {
//             _param = param as ASMRMainGameFsmParam;
//             if (_param != null)
//             {
//                 ASMRMain.Model.Instance.Init(_param.homeId);
//                 var levelConfig = ASMRMain.Model.Instance.AsmrLevelConfigs.Find(c => c.Id == _param.levelId);
//
//                 ASMRMain.Model.Instance.LoadLevelbyLevelId(levelConfig, _param.homeId, _param.taskId);
//
//                 ASMRMain.Model.Instance.LeaveAsmr = false;
//
//                 AudioManager.StopAllMusic();
//             }
//
//             Input.multiTouchEnabled = true;
//
//             return Task.FromResult(true);
//         }
//
//         public void Exit(FsmStateType toStateType)
//         {
//             Input.multiTouchEnabled = false;
//             UIViewSystem.Instance.CloseAll((int)UIViewLayer.Notice, (int)UIViewLayer.Loading, (int)UIViewLayer.TopNotice);
//
//             ASMRMain.Model.Instance.ExitGame();
//
//             AudioManager.StopAllMusic();
//
//             _param = null;
//
//             Resources.UnloadUnusedAssets();
//
//             ASMRMain.Model.Instance.Release();
//
//             CoroutineManager.Instance.StartCoroutine(DelayHideLoading(10.0f));
//         }
//
//         private IEnumerator DelayHideLoading(float checkTime)
//         {
//             var time = Time.time;
//             yield return null;
//             while ((Time.time - time) < checkTime && !ASMRMain.Model.Instance.LeaveAsmr)
//             {
//                 yield return null;
//             }
//         }
//
//         public void EnterFinish(FsmStateType fromStateType)
//         {
//             QualityMgr.Instance.SetAsmrFrameRate();
//         }
//
//         public void Update()
//         {
//         }
//
//         public void FixedUpdate(float deltaTime)
//         {
//         }
//     }
// }