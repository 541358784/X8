// using System.Collections;
// using DragonPlus.Config.OneLine;
// using DragonPlus.Haptics;
// using DragonU3DSDK.Network.API.Protocol;
// using DragonU3DSDK.Storage;
// using Newtonsoft.Json;
// using Spine;
// using Spine.Unity;
// using UnityEngine;
// using UnityEngine.UI;
//
// namespace OneLine
// {
//     public class OneLineGameViewParam : UIViewParam
//     {
//         public OneLineGameViewParam(int id)
//         {
//             Level = OneLineConfigManager.Instance.GetLevel(id);
//         }
//
//         public Level Level;
//     }
//
//     [AssetAddress("UIOneLine/UILevel")]
//     public class OneLineGameView : UIView, IOneLineView
//     {
//         [ComponentBinder("Root/TopGroup/CloseButton")]
//         private Button m_BackButton;
//
//         [ComponentBinder("Root/TopGroup/Slider")]
//         private Slider m_ProgressSlider;
//
//         [ComponentBinder("Root/MainImage")]
//         private RawImage m_DrawOnImage;
//
//         [ComponentBinder("Root/TopGroup/Slider/Handle Slide Area/Handle/Icon")]
//         private SkeletonGraphic m_BrainGraphic;
//
//         [ComponentBinder("Root/MainImage/Pen")]
//         private RectTransform m_PenRectTransform;
//
//         [ComponentBinder("Root/Faild")]
//         private RectTransform m_FailRoot;
//
//         [ComponentBinder("Root/vfx_fireworks")]
//         private RectTransform m_SuccessRoot;
//
//         [ComponentBinder("Root/MainImage/GuideHand")]
//         private RectTransform m_GuideHand;
//
//         private OneLineGame      m_Game;
//         private Level            m_Level;
//         private OneLineGameGuide m_Guide;
//
//         private float m_TargetProgressValue;
//
//         private const string NormalIdleAnim      = "1";
//         private const string NormalGoForwardAnim = "2";
//         private const string SwitchToCryAnim     = "3";
//         private const string CryGoBackAnim       = "4";
//         private const float  ResetDuration       = 0.5f;
//         private long lastOnDrawTime;
//
//         public RawImage DrawOnImage => m_DrawOnImage;
//
//         public override void OnViewOpen(UIViewParam param)
//         {
//             base.OnViewOpen(param);
//             m_BackButton.onClick.AddListener(OnBackButtonClick);
//
//             m_FailRoot.gameObject.SetActive(false);
//             m_SuccessRoot.gameObject.SetActive(false);
//             m_GuideHand.gameObject.SetActive(false);
//
//             m_Level = ((OneLineGameViewParam) param).Level;
//             HapticsManager.Init();
//         }
//
//         public override void OnViewUpdate(float deltaTime)
//         {
//             base.OnViewUpdate(deltaTime);
//             if (m_Guide != null && m_Guide.MoveNext(deltaTime) == false)
//             {
//                 m_Guide = null;
//             }
//
//             if (m_Game != null && m_Game.IsDrawing)
//             {
//                 m_ProgressSlider.value = Mathf.MoveTowards(m_ProgressSlider.value, m_TargetProgressValue, deltaTime);
//             }
//         }
//
//         private void OnBackButtonClick()
//         {
//             MyMain.myGame.Fsm.ChangeState(FsmStateType.Decoration, new FsmParamDecoration(FsmStateType.OneLine));
//             Model.Instance.SendResultBi("quit");
//         }
//
//         void IOneLineView.OnStart(OneLineGame game)
//         {
//             m_Game = game;
//             m_BrainGraphic.AnimationState.SetAnimation(0, NormalIdleAnim, true);
//
//             if ((m_Level.Id == 1 || m_Level.Id == 2) && Model.Instance.IsPassed(m_Level.Id) == false)
//             {
//                 m_Guide = new OneLineGameGuide(m_Game, m_GuideHand, m_DrawOnImage);
//                 m_Guide.Start();
//             }
//         }
//
//         void IOneLineView.OnBeginDraw(OneLineGraphic.Point startPoint)
//         {
//             PlayDrawSound();
//             m_PenRectTransform.gameObject.SetActive(true);
//             m_PenRectTransform.anchoredPosition = startPoint.Position;
//             m_BrainGraphic.AnimationState.SetAnimation(0, NormalGoForwardAnim, true);
//         }
//
//         void IOneLineView.OnDraw(Pixel drawingPixel, float completeProgress)
//         {
//             m_TargetProgressValue               = completeProgress;
//             m_PenRectTransform.anchoredPosition = drawingPixel;
//             
//             var now = CommonUtils.GetTimeStamp();
//             if (lastOnDrawTime > 0)
//             {
//                 if (now - lastOnDrawTime > 300)
//                 {
//                     Debug.Log("调用OnDraw间隔时间大于0.3s, 播放一次画线声音");
//                     PlayDrawSound();
//                 }
//             }
//             lastOnDrawTime = now;
//         }
//
//         void IOneLineView.OnSuccess()
//         {
//             AudioSysManager.Instance.PlaySound("YX_oneline_success");
//             DoShake();
//             Model.Instance.Win(m_Level.Id);
//             m_SuccessRoot.gameObject.SetActive(true);
//             m_PenRectTransform.gameObject.SetActive(false);
//             m_ProgressSlider.value      = 1f;
//             m_DrawOnImage.raycastTarget = false;
//             m_BackButton.interactable   = false;
//             m_BrainGraphic.AnimationState.SetAnimation(0, NormalIdleAnim, true);
//             Model.Instance.SendResultBi("pass");
//             
//             var rewardDatas = Model.Instance.GetWinRewards();
//             var _gameSuccessParam = new OneLineGameSuccessViewParams();
//             _gameSuccessParam.rewardDatas = rewardDatas;
//             if (rewardDatas.Count > 0)
//             {
//                 var BI = new BiUtil.ItemChangeReasonArgs()
//                 {
//                     reason = BiEventMatchFrenzy.Types.ItemChangeReason.LineStreak,
//                     data1 = m_Level.Id.ToString(),
//                 };
//
//                 // copy from asmr
//                 //做本地存储，那么即使杀进程再次回到大厅也可以将资源加上
//                 JsonSerializerSettings setting = new JsonSerializerSettings();
//                 setting.NullValueHandling = NullValueHandling.Ignore;
//                 string jsonData = JsonConvert.SerializeObject(_gameSuccessParam.rewardDatas, setting);
//                 PlayerPrefs.SetString("ASMRResultData", jsonData);
//                 var biJsonData = JsonConvert.SerializeObject(BI, setting);
//                 PlayerPrefs.SetString("ASMRItemChangeReason", biJsonData);
//                 PlayerPrefs.SetInt("ASMRResultExecuteLast", 0);
//             }
//             
//             StartCoroutine(Animation());
//             IEnumerator Animation()
//             {
//                 yield return new WaitForSeconds(1f);
//                 UIViewSystem.Instance.Open<OneLineGameSuccessView>(_gameSuccessParam);
//             }
//         }
//
//         void IOneLineView.OnFailed()
//         {
//             AudioSysManager.Instance.PlaySound("YX_oneline_fail");
//             DoShake();
//             Model.Instance.Lose(m_Level.Id);
//             m_DrawOnImage.raycastTarget = false;
//             m_PenRectTransform.gameObject.SetActive(false);
//
//             m_FailRoot.gameObject.SetActive(false);
//             m_FailRoot.gameObject.SetActive(true);
//
//             StartCoroutine(Animation());
//
//             IEnumerator Animation()
//             {
//                 TrackEntry track = m_BrainGraphic.AnimationState.SetAnimation(0, SwitchToCryAnim, false);
//                 while (track.IsComplete == false)
//                 {
//                     yield return null;
//                 }
//
//                 float startValue = m_ProgressSlider.value;
//                 m_BrainGraphic.AnimationState.SetAnimation(0, CryGoBackAnim, true);
//                 float resetDuration = 0;
//                 do
//                 {
//                     m_ProgressSlider.value = Mathf.Lerp(startValue, 0f, resetDuration / ResetDuration);
//                     yield return null;
//                 } while ((resetDuration += Time.deltaTime) < ResetDuration);
//
//                 m_ProgressSlider.value = 0f;
//
//                 m_BrainGraphic.AnimationState.SetAnimation(0, NormalIdleAnim, true);
//
//                 m_DrawOnImage.raycastTarget = true;
//                 if (Model.Instance.CanEnterFree(m_Level.Id) || Model.Instance.IsPassed(m_Level.Id) == false)
//                 {
//                     Model.Instance.TryAddFreePlayCount(m_Level.Id);
//                     m_Game.Reset();
//                 }
//                 else
//                 {
//                     UIViewSystem.Instance.Open<OneLineGameFailedView>(new OneLineGameFailedParam()
//                     {
//                         Game  = m_Game,
//                         Level = m_Level,
//                     });
//                 }
//             }
//         }
//
//         void IOneLineView.OnReset()
//         {
//             m_TargetProgressValue  = 0f;
//             m_ProgressSlider.value = 0f;
//             m_PenRectTransform.gameObject.SetActive(false);
//             m_Guide?.Start();
//         }
//         
//         private int drawSoundId;
//         private void PlayDrawSound()
//         {
//             AudioSysManager.Instance.StopSoundById(drawSoundId);
//             drawSoundId = AudioSysManager.Instance.PlaySound("YX_oneline_draw", false);
//         }
//
//         private void DoShake()
//         {
//             bool canShake;
// #if UNITY_IOS
//             canShake = !CommonSetModel.Instance.ShakeClose;
// #else
//             var data = StorageManager.Instance.GetStorage<StorageASMR>();
//             canShake = !data.CloseShort;
// #endif
//             if (!canShake) return;
//             
// #if UNITY_ANDROID
//             HapticsManager.Vibrate(10);
// #else
//             HapticsManager.Haptics(HapticTypes.Light);
// #endif
//         }
//     }
// }