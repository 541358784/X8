using DG.Tweening;
using DragonPlus;
using Framework;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace TileMatch.Game.PlayMethod
{
    public partial class TimeLimitPlayMethod
    {
        private TileMatchMainController _mainController;

        private GameObject _timeLimitObj;
        private Slider _timeSlider;
        private Image _timeBgImage;
        private LocalizeTextMeshProUGUI _timeText;
        private Animator _animator;
        private GameObject _timeTipObj;
        private LocalizeTextMeshProUGUI _timeTipText;
        private SkeletonGraphic _spine;
        
        private string[] _sliderImageName = new[] { "TimeSlider1", "TimeSlider2" };

        private Button _closeButton;
        private bool _canClose = false;
        private GameObject _moveObj;
        
        private void InitAnim()
        {
            _mainController = UIManager.Instance.GetOpenedUIByPath<TileMatchMainController>(UINameConst.TileMatchMain);

            _animator = _mainController.transform.GetComponent<Animator>();

            _spine = _mainController.transform.Find("Root/Top/TimeLimitTime/spine").GetComponent<SkeletonGraphic>();
            _spine.AnimationState.SetAnimation(0, "Idle", true);
            _spine.AnimationState.Update(0);
            
            _closeButton = _mainController.transform.Find("Root/TimeStart/CloseButton").GetComponent<Button>();
            _closeButton.onClick.AddListener(CloseTips);
            
            _timeTipObj = _mainController.transform.Find("Root/TimeStart").gameObject;
            _timeTipObj.gameObject.SetActive(false);
            
            _moveObj = _mainController.transform.Find("Root/TimeStart/Icon").gameObject;
            _moveObj.transform.localPosition = new Vector3(0, 100, 0);
            
            _timeLimitObj = _mainController.transform.Find("Root/Top/TimeLimitTime").gameObject;
            _timeSlider = _mainController.transform.Find("Root/Top/TimeLimitTime/Mask/Slider").GetComponent<Slider>();
            _timeText = _mainController.transform.Find("Root/Top/TimeLimitTime/Mask/Slider/Text").GetComponent<LocalizeTextMeshProUGUI>();
            _timeBgImage = _mainController.transform.Find("Root/Top/TimeLimitTime/Mask/Slider/Fill Area/Fill").GetComponent<Image>();
            
            _timeTipText = _mainController.transform.Find("Root/TimeStart/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
            _timeTipText.SetText(TimeUtils.GetTimeMinutesString(_orgTimeLimit));
            
            _timeLimitObj.SetActive(false);
        }

        private void PlayAnim()
        {
            _canClose = false;
            _timeTipObj.gameObject.SetActive(true);
            _animator.Play("Open", 0, 0);

            CoroutineManager.Instance.StartCoroutine(CommonUtils.DelayWork(1f, () =>
            {
                _canClose = true;
            }));
        }
        private void CloseTips()
        {
            if(!_canClose)
                return;
            
            _canClose = false;
            _timeLimitObj.SetActive(true);
            _animator.Play("Fly", 0, 0);
            UIRoot.Instance.EnableEventSystem = false;
            CoroutineManager.Instance.StartCoroutine(CommonUtils.DelayWork(0.9f, () =>
            {
                _moveObj.transform.DOMove(_spine.transform.position, 0.6f);
            }));
            
            CoroutineManager.Instance.StartCoroutine(CommonUtils.DelayWork(1.5f, () =>
            {
                _spine.AnimationState.SetAnimation(0, "Keep", true);
                _spine.AnimationState.Update(0);
                
                _currentTime = 0;
                _isStart = true;
                
                _timeTipObj.gameObject.SetActive(false);
                UIRoot.Instance.EnableEventSystem = true;
            }));
        }
    }
}