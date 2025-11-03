using Cysharp.Threading.Tasks;
using DG.Tweening;
using DragonPlus;
using Screw.GameLogic;
using Screw.Module;
using Screw.UIBinder;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Screw
{
    public class ScrewHeaderView: Entity, IConfirmWindowHandler
    {
        [UIBinder("BackButton")] private Button _backBtn;
        [UIBinder("SettingButton")] private Button _settingBtn;

        [UIBinder("Timer")] private Transform _timer;
        
        [UIBinder("TimerText")] private TMP_Text _timerText;
        [UIBinder("TimerProgress")] private Slider _timerProgress;
       
        [UIBinder("LevelText")] private LocalizeTextMeshProUGUI _levelText;
        [UIBinder("HardTextGroup")] private Transform _levelDifficult;
        [UIBinder("HardTextGroup")] private HorizontalLayoutGroup _levelDifficultGroup;
        [UIBinder("LevelHardLeft")] private LocalizeTextMeshProUGUI _hardLevelTextLeft;
        [UIBinder("LevelHardRight")] private LocalizeTextMeshProUGUI _hardLevelTextRight;

        [UIBinder("AlarmClock")] private Animator _clockAni;

        //迁移报错注释
        // private EventBus.Listener _listener;
        // private EventBus.Listener _listener1;
        private bool isPlayCountDown = false;
        public ScrewHeaderView(Transform headerRoot, ScrewGameContext context)
        {
            Bind(headerRoot, context);
            
            _backBtn.onClick.AddListener(OnBackButtonClicked);
            _settingBtn.onClick.AddListener(OnSettingButtonClicked);
          
            AdaptHeader();

            var isDifficult = ScrewGameLogic.Instance.LevelIsHardDifficulty(context.levelIndex) || context is ScrewGameSpecialContext;
            _levelDifficult.gameObject.SetActive(isDifficult);
            _levelText.gameObject.SetActive(!isDifficult);

            UpdateLevelText();
            RegisterEvent();
            
            if(context is ScrewMiniGameContext)
                headerRoot.gameObject.SetActive(false);
        }

        private void RegisterEvent()
        {
            //迁移报错注释
            // _listener = EventBus.Subscribe<EventLanguageChange>((evt) =>
            // {
            //     if(context.gameTimer != null)
            //         context.gameTimer.EnableTimer(true);
            //     UpdateLevelText();
            // });
            // _listener1 = EventBus.Subscribe<EventUISettingClose>((evt) =>
            // {
            //     if(context.gameTimer != null)
            //         context.gameTimer.EnableTimer(true);
            // });
        }

        public void ClearListener()
        {
            //迁移报错注释
            // if (_listener != null)
            //     EventBus.UnSubscribe(_listener);
            // _listener = null;
            // if (_listener1 != null)
            //     EventBus.UnSubscribe(_listener1);
            // _listener1 = null;
        }

        private async void UpdateLevelText()
        {
            if (_levelText == null)
                return;
            
            if (context is ScrewGameKapiScrewContext)
                _levelText.SetText(LocalizationManager.Instance.GetLocalizedString("ui_screw_level")+" "+(KapiScrewModel.Instance.Storage.BigLevel+1)+"-"+(KapiScrewModel.Instance.Storage.PlayingSmallLevel+1));
            else
            {
                _levelText.SetText(LocalizationManager.Instance.GetLocalizedString("ui_screw_level")+ScrewGameLogic.Instance.GetMainLevelIndex());
            }
            if (!_levelDifficult.gameObject.activeSelf)
                return;
            _hardLevelTextLeft.SetTerm("&key.UI_hard_level_1");
            if (context is ScrewGameSpecialContext)
                _hardLevelTextRight.SetTerm("&key.UI_TreasureClimb_oneShotLevel");
            else
                _hardLevelTextRight.SetTerm("&key.UI_hard_level_CAP");

            await ScrewUtility.WaitNFrame(1);
            _levelDifficult.gameObject.SetActive(false);
            _levelDifficult.gameObject.SetActive(true);
        }

        private void AdaptHeader()
        {
            if (Screen.height / (float)Screen.width > UIModule.Instance.DesignSize.x / UIModule.Instance.DesignSize.y)
            {
                var anchoredPos = ((RectTransform) root).anchoredPosition;
                anchoredPos.y =  UIModule.Instance.GetSafeAreaOffset();
                
                ((RectTransform) root).anchoredPosition = anchoredPos;
            }
            
            _timer.gameObject.SetActive(false);
        }

        public async void PlayTimerAppear(Transform clockStartTransform)
        {
            _levelText.gameObject.SetActive(false);
            _timer.gameObject.SetActive(true);
            _timerText.SetText("");
            ScrewUtility.PlayAnimation(_clockAni, "AlarmClock");
            
            isPlayCountDown = false;
            
             var screenPoint = UIModule.Instance.WorldCamera.WorldToScreenPoint(clockStartTransform.position);
             screenPoint.z = 0;
             var worldPoint = UIModule.Instance.WorldCamera.ScreenToWorldPoint(screenPoint);
             worldPoint.z = 0;
             var closeTransform = _clockAni.transform;
             closeTransform.position = worldPoint;
             var middlePos = closeTransform.localPosition;
             middlePos.y += 80;
             closeTransform.localScale = Vector3.one;
             closeTransform.DOScale(Vector3.one * 1.38f, 0.4f);
             await _clockAni.transform.DOLocalMove(middlePos, 0.4f);
             closeTransform.DOScale(Vector3.one * 0.2f, 0.3f).SetEase(Ease.InQuad);
             await _clockAni.transform.DOLocalMove(Vector3.zero, 0.3f).SetEase(Ease.InQuad);
             context.gameTimer.OnLevelStart();
             ScrewUtility.PlayAnimation(_clockAni, "Idle");
        }

        private async void OnBackButtonClicked()
        {
            if (context.IsMovingTask())
                return;
            if (context.gameState == ScrewGameState.Win || context.gameState == ScrewGameState.Fail)
                return;

            if(context.gameTimer != null)
                context.gameTimer.EnableTimer(false);
            
            if (context.exitLevelHandlers != null
                && context.exitLevelHandlers.Count > 0)
            {
                for (var i = 0; i < context.exitLevelHandlers.Count; i++)
                {
                    var result = await context.exitLevelHandlers[i].Invoke();
                    if (result == false)
                    {
                        return;
                    }
                }
            }
            
            ShowUserExitLevelPopup();
        }

        private void ShowUserExitLevelPopup()
        {
            if (context is ScrewGameKapiScrewContext)
                UIModule.Instance.ShowUI(typeof(UIKapiScrewQuitPopup), context, true);
            else
                UIModule.Instance.ShowUI(typeof(UIConfirmLeaveLevelPopup), this, context);
        }

        public void OnConfirm()
        {
            context.gameState = ScrewGameState.Fail;
            context.failReason = LevelFailReason.Exit;

            context.SendLevelQuitBi();

            SceneFsm.mInstance.ChangeState(StatusType.ScrewHome);
        }

        public void OnQuit()
        {
            if(context.gameTimer != null)
                context.gameTimer.EnableTimer(true);
            //Log.Debug("User Exit");
        }
        private void OnSettingButtonClicked()
        {
            if (context.IsMovingTask())
                return;
            if (context.gameState == ScrewGameState.Win || context.gameState == ScrewGameState.Fail)
                return;
            if(context.gameTimer != null)
                context.gameTimer.EnableTimer(false);
            //迁移报错注释
            //UIModule.Instance.ShowUI(typeof(UISettingPopup));
        }

        private float _timeOffset = 1f;
        public void UpdateTimerText()
        {
            var leftTime = context.gameTimer.GetLeftTime();
            if (leftTime > 0)
            {
                _timerText.text = ScrewUtility.GetTimeText(leftTime);
               
                if (leftTime > 10)
                {
                    _timerText.color = new Color(1, 1, 1, 1);
                    
                    if (isPlayCountDown)
                    {
                        ScrewUtility.PlayAnimation(_clockAni, "Idle");
                        isPlayCountDown = false;
                    }
                }
                else
                {
                    _timerText.color = Color.red;
                   
                    if (!isPlayCountDown)
                    {
                        ScrewUtility.PlayAnimation(_clockAni, "Win");
                        isPlayCountDown = true;
                    }

                    _timeOffset += Time.deltaTime;

                    if (_timeOffset >= 1)
                    {
                        _timeOffset = 0;
                        SoundModule.PlaySfx("sfx_count_down");
                    }
                }
                
                _timerProgress.value = context.gameTimer.GetProgress();
            }
            else
            {
                _timerText.text = "00:00";
                _timerProgress.value = 0;
            }
        }
    }
}