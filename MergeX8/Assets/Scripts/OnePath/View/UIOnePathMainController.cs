using System.Collections;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.TMatch;
using DragonPlus.Haptics;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using Newtonsoft.Json;
using OneLine;
using OnePath.Model;
using OnePathSpace;
using Spine;
using Spine.Unity;
using TMatch;

using UnityEngine;
using UnityEngine.UI;
using NotImplementedException = System.NotImplementedException;

namespace OnePath.View
{
    public class UIOnePathMainController : UIWindowController , IOneLineView
    {
        private Button m_BackButton;

        private Slider m_ProgressSlider;

        private RawImage m_DrawOnImage;

        // [ComponentBinder("Root/TopGroup/Slider/Handle Slide Area/Handle/Icon")]
        // private SkeletonGraphic m_BrainGraphic;
        private GameObject _BrainGraphic;
        
        private RectTransform m_PenRectTransform;

        private GameObject m_FailRoot;

        private GameObject m_SuccessRoot;
        
        private RectTransform m_GuideHand;

        private OneLineGame      m_Game;
        private TableOnePathLevel m_Level;
        private OneLineGameGuide m_Guide;

        private float m_TargetProgressValue;

        private const string NormalIdleAnim      = "1";
        private const string NormalGoForwardAnim = "2";
        private const string SwitchToCryAnim     = "3";
        private const string CryGoBackAnim       = "4";
        private const float  ResetDuration       = 0.5f;
        private long lastOnDrawTime;

        private Animator _animator;
        public RawImage DrawOnImage => m_DrawOnImage;

        public override void PrivateAwake()
        {
            m_BackButton = transform.Find("Root/PauseButton").GetComponent<Button>();
            m_BackButton.onClick.AddListener(OnBackButtonClick);

            _animator = transform.Find("Root/Slider/Slide").GetComponent<Animator>();
            
            m_ProgressSlider = transform.Find("Root/Slider").GetComponent<Slider>();

            _BrainGraphic = transform.Find("Root/Slider/Slide").gameObject;
            
            m_PenRectTransform = transform.Find("Root/MainImage/pen").transform as RectTransform;

            m_DrawOnImage = transform.Find("Root/MainImage").GetComponent<RawImage>();

            m_GuideHand = transform.Find("Root/MainImage/Guide").transform as RectTransform;
            m_GuideHand.gameObject.SetActive(false);
            
            m_FailRoot = transform.Find("Root/Fail").gameObject;
            m_SuccessRoot = transform.Find("Root/Finish").gameObject;
            m_FailRoot.gameObject.SetActive(false);
            m_SuccessRoot.gameObject.SetActive(false);
            
            m_SuccessRoot.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (m_Level.id == 1 && !isFinish)
                {
                    UILoadingTransitionController.Show(null);
                    XUtility.WaitSeconds(0.3f, () =>
                    {
                        SceneFsm.mInstance.ChangeState(StatusType.ExitOnePath);
                        XUtility.WaitFrames(1, () =>
                        {
                            SceneFsm.mInstance.ChangeState(StatusType.EnterOnePath,
                                OnePathConfigManager.Instance._configs[1], true);
                        });
                    });
                }
                else
                {
                    SceneFsm.mInstance.ChangeState(StatusType.ExitOnePath);
                    if (isFinish || m_Level.id != 2)
                    {
                        if (AdSubSystem.Instance.CanPlayInterstitial(ADConstDefine.IN_AsmrContinue))
                        {
                            AdSubSystem.Instance.PlayInterstital(ADConstDefine.IN_AsmrContinue, b => { });
                        }
                    }
                }
            });
        }

        private bool isFinish;
        protected override void OnOpenWindow(params object[] objs)
        {
            m_Level = objs[0] as TableOnePathLevel;
            isFinish = (bool)objs[1];
            if (m_Level != null)
            {
                m_SuccessRoot.transform.Find("Bag")?.gameObject.SetActive(m_Level.id == 2&&!isFinish);
                if (m_Level.id == 1 && !isFinish)
                {
                    m_SuccessRoot.transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>().SetTerm("ui_minigame_1Line_Lv1_FirstTime");
                }
                if (m_Level.id == 2 && !isFinish)
                {
                    m_SuccessRoot.transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>().SetTerm("ui_minigame_1Line_Lv2_FirstTime");
                    // m_SuccessRoot.transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>().SetTerm("ui_minigame_End_Dive");   
                }
            }
            HapticsManager.Init();
        }

        public void Update()
        {
            if (m_Guide != null && m_Guide.MoveNext(Time.deltaTime) == false)
            {
                m_Guide = null;
            }

            if (m_Game != null && m_Game.IsDrawing)
            {
                m_ProgressSlider.value = Mathf.MoveTowards(m_ProgressSlider.value, m_TargetProgressValue, Time.deltaTime);
            }
        }

        private void OnBackButtonClick()
        {
            CommonUtils.OpenCommon1ConfirmWindow(new NoticeUIData
            {
                DescString =
                    LocalizationManager.Instance.GetLocalizedString("ui_minigame_11"),
                OKCallback = () =>
                {
                    TMatch.UILoadingEnter.Open(() =>
                    {
                        GameBIManager.Instance.SendGameEvent(
                            BiEventAdventureIslandMerge.Types.GameEventType.GameEventMinigameOnePathLevelend,
                            data1:m_Level.id.ToString(),"1");
                        
                        SceneFsm.mInstance.ChangeState(StatusType.ExitOnePath);
                    });
                },
                HasCloseButton = true,
                HasCancelButton = true,
                IsHighSortingOrder = true,
            });
            
        }

        void IOneLineView.OnStart(OneLineGame game)
        {
            m_Game = game;
            // m_BrainGraphic.AnimationState.SetAnimation(0, NormalIdleAnim, true);
            //
            
            if (m_Level.id == 1 && OnePathEntryControllerModel.Instance.IsFinish(m_Level) == false)
            {
                m_Guide = new OneLineGameGuide(m_Game, m_GuideHand, m_DrawOnImage);
                m_Guide.Start();
            }
        }

        void IOneLineView.OnBeginDraw(OneLineGraphic.Point startPoint)
        {
            PlayDrawSound();
            m_PenRectTransform.gameObject.SetActive(true);
            m_PenRectTransform.anchoredPosition = startPoint.Position;
            //m_BrainGraphic.AnimationState.SetAnimation(0, NormalGoForwardAnim, true);
        }

        void IOneLineView.OnDraw(Pixel drawingPixel, float completeProgress)
        {
            m_TargetProgressValue               = completeProgress;
            m_PenRectTransform.anchoredPosition = drawingPixel;
            
            var now = CommonUtils.GetTimeStamp();
            if (lastOnDrawTime > 0)
            {
                if (now - lastOnDrawTime > 300)
                {
                    Debug.Log("调用OnDraw间隔时间大于0.3s, 播放一次画线声音");
                    PlayDrawSound();
                }
            }
            lastOnDrawTime = now;
        }

        void IOneLineView.OnSuccess()
        {
            DragonPlus.AudioManager.Instance.PlaySound("sfx_fig_win");
            OnePathModel.Instance.Win(m_Level.id);
            m_SuccessRoot.gameObject.SetActive(true);
            m_PenRectTransform.gameObject.SetActive(false);
            m_ProgressSlider.value      = 1f;
            m_DrawOnImage.raycastTarget = false;
            m_BackButton.interactable   = false;
            
            GameBIManager.Instance.SendGameEvent(
                BiEventAdventureIslandMerge.Types.GameEventType.GameEventMinigameOnePathLevelend,
                data1:m_Level.id.ToString(),"0");
        }

        void IOneLineView.OnFailed(bool moveFlag)
        {
            OnePathModel.Instance.Failed(m_Level.id);
            m_DrawOnImage.raycastTarget = false;
            m_PenRectTransform.gameObject.SetActive(false);

            m_FailRoot.gameObject.SetActive(false);
            //m_FailRoot.gameObject.SetActive(true);

            _animator.Play("Fill", 0, 0);
            
            GameBIManager.Instance.SendGameEvent(
                BiEventAdventureIslandMerge.Types.GameEventType.GameEventMinigameOnePathLevelend,
                data1:m_Level.id.ToString(),"1");

            if (moveFlag)
            {
                DragonPlus.AudioManager.Instance.PlaySound("sfx_fig_defeat");
                UIOnePathFailController.Open(() =>
                {
                    m_ProgressSlider.DOValue(0, 0.5f).OnComplete(() =>
                    {
                        m_Game.Reset();
                        m_DrawOnImage.raycastTarget = true;
                        m_FailRoot.gameObject.SetActive(false);
                    });
                });}
            else
            {
                m_Game.Reset();
                m_DrawOnImage.raycastTarget = true;
                m_FailRoot.gameObject.SetActive(false);
            }
            // CommonUtils.OpenCommon1ConfirmWindow(new NoticeUIData
            // {
            //     DescString =
            //         LocalizationManager.Instance.GetLocalizedString("ui_minigame_11"),
            //     OKCallback = () =>
            //     {
            //         TMatch.UILoadingEnter.Open(() =>
            //         {
            //             SceneFsm.mInstance.ChangeState(StatusType.ExitOnePath);
            //         });
            //     },
            //     HasCloseButton = true,
            //     HasCancelButton = true,
            //     IsHighSortingOrder = true,
            // });
        }

        void IOneLineView.OnReset()
        {
            m_TargetProgressValue  = 0f;
            m_ProgressSlider.value = 0f;
            m_PenRectTransform.gameObject.SetActive(false);
            m_Guide?.Start();

            _animator.Play("normal", 0, 0);
        }
        
        private int drawSoundId;
        private void PlayDrawSound()
        {
            // AudioSysManager.Instance.StopSoundById(drawSoundId);
            // drawSoundId = AudioSysManager.Instance.PlaySound("YX_oneline_draw", false);
        }
    }
}