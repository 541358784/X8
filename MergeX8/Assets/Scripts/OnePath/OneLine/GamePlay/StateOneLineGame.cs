// using System.Threading.Tasks;
// using Decoration.DynamicMap;
// using DragonU3DSDK.Asset;
// using DragonU3DSDK.Audio;
// using DragonU3DSDK.Storage;
// using Framework;
// using OnePath.Model;
// using OnePath.System;
// using UnityEngine;
//
// namespace OneLine
// {
//     public class OneLineGameFsmParam : FsmParam
//     {
//         public int LevelId;
//     }
//     
//     public class StateOneLineGame : IFsmState
//     {
//         private OneLineGame m_Game;
//         private bool        m_IsLastStateMultiTouchEnabled;
//     
//         FsmStateType IFsmState.Type => FsmStateType.OneLine;
//     
//         async Task<bool> IFsmState.PreEnterAsync(FsmParam param)
//         {
//             OneLineGameFsmParam param1 = (OneLineGameFsmParam) param;
//     
//             m_IsLastStateMultiTouchEnabled = Input.multiTouchEnabled;
//             Input.multiTouchEnabled        = false;
//             AudioManager.StopAllMusic();
//             var levelCfg = OneLineConfigManager.Instance.GetLevel(param1.LevelId);
//     
//             OneLineOrder order = new OneLineOrder();
//             order.Graphic               = JsonUtility.FromJson<OneLineGraphic>(ResourcesManager.Instance.LoadResource<TextAsset>($"OneLine/Config/{levelCfg.ResourceId}").text);
//             order.Template              = ResourcesManager.Instance.LoadResource<Texture2D>($"OneLine/Textures/{levelCfg.ResourceId}");
//             order.TemplateColor         = GetColor("#4a2210", 0.3f);
//             order.DrawColor             = GetColor("#334855", 1f);
//             order.SuccessColor          = order.DrawColor;
//             order.FailedColor           = order.DrawColor;
//             order.AdsorbToPointDistance = 10f;
//             order.UICamera              = CameraManager.UICamera;
//     
//             m_Game = new OneLineGame(order);
//             UIViewSystem.Instance.Open<OneLineGameView>(new OneLineGameViewParam(param1.LevelId));
//             await Task.Yield();
//             ResourcesManager.Instance.ReleaseRes($"OneLine/Textures/{levelCfg.ResourceId}");
//             return true;
//     
//             Color GetColor(string htmlString, float a)
//             {
//                 ColorUtility.TryParseHtmlString(htmlString, out Color color);
//                 color.a = a;
//                 return color;
//             }
//         }
//     
//         void IFsmState.EnterFinish()
//         {
//             m_Game.Start(UIViewSystem.Instance.Get<OneLineGameView>());
//             StorageManager.Instance.uploadEnable = false;
//         }
//     
//         void IFsmState.Update(float deltaTime)
//         {
//         }
//     
//         void IFsmState.LateUpdate(float deltaTime)
//         {
//         }
//     
//         void IFsmState.Exit(FsmStateType toStateType)
//         {
//             StorageManager.Instance.uploadEnable = true;
//             Input.multiTouchEnabled              = m_IsLastStateMultiTouchEnabled;
//             UIViewSystem.Instance.Close<OneLineGameView>();
//             m_Game.Dispose();
//         }
//     }
// }