// using DragonPlus;
// using DragonPlus.Config.OneLine;
// using DragonU3DSDK.Network.API.Protocol;
// using UnityEngine.UI;
//
// namespace OneLine
// {
//     public class OneLineGameFailedParam : UIViewParam
//     {
//         public OneLineGame Game;
//         public Level       Level;
//     }
//
//     [AssetAddress("UIOneLine/UIGameFailed")]
//     public class OneLineGameFailedView : UIPopup
//     {
//         private OneLineGame m_Game;
//         private Level       m_Level;
//
//         [ComponentBinder("Root/BtnExit")]
//         private Button m_ExitButton;
//
//         [ComponentBinder("Root/CloseButton")]
//         private Button m_CloseButton;
//
//         [ComponentBinder("Root/BtnRetry")]
//         private Button m_RetryButton;
//         
//         public override void OnViewOpen(UIViewParam param)
//         {
//             base.OnViewOpen(param);
//             
//             m_ExitButton.onClick.AddListener(OnExitButtonClick);
//             m_CloseButton.onClick.AddListener(OnExitButtonClick);
//             m_RetryButton.onClick.AddListener(OnRetryButtonClick);
//             
//             OneLineGameFailedParam param1 = (OneLineGameFailedParam) param;
//             m_Game  = param1.Game;
//             m_Level = param1.Level;
//         }
//
//         private void OnExitButtonClick()
//         {
//             UIViewSystem.Instance.Close<OneLineGameFailedView>();
//             MyMain.myGame.Fsm.ChangeState(FsmStateType.Decoration, new FsmParamDecoration(FsmStateType.OneLine));
//             Model.Instance.SendResultBi("quit");
//         }
//         
//         private void OnRetryButtonClick()
//         {
//             if (!AdLogicManager.Instance.ShouldShowRV(eAdReward.OneLine))
//             {
//                 UIViewSystem.Instance.Open<NoticeController>(new NoticeUIData() {DescString = LocalizationManager.Instance.GetLocalizedString("UI_button_loading_ADS")});
//                 return;
//             }
//
//             AdLogicManager.Instance.TryShowRewardedVideo(eAdReward.OneLine, (b, s) =>
//             {
//                 if (b)
//                 {
//                     Model.Instance.OneLineShowRV();
//                     UIViewSystem.Instance.Close<OneLineGameFailedView>();
//                     m_Game.Reset();
//                 }
//             });
//         }
//     }
// }