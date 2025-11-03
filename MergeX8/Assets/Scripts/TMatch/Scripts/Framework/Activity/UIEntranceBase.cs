using DragonPlus;
using DragonU3DSDK;
using UnityEngine;
using UnityEngine.UI;

namespace TMatch
{
    public abstract class UIEntranceBase : MonoBehaviour
    {
        protected const string ButtonRes = "Entrance";
        protected Transform _progressGroup;
        protected Transform _countDownGroup;
        protected LocalizeTextMeshProUGUI _progressTxt;
        protected Slider _progressValue;
        private bool _oldState;
        protected bool _currentResState;//当前资源状态
        protected LocalizeTextMeshProUGUI _countDownTxt;
        protected float _delay = 1F;
        protected float _lastTime = 0F;
        protected Transform _downProgressGroup;
        protected Slider _downProgressValue;
        protected LocalizeTextMeshProUGUI _downProgressTxt;

        protected GameObject reminderObj; 
        protected RedPointController reminder; 
        protected bool _useCountDown = true;
        protected bool _useProgress = false;
        protected bool _useRemind = false;

        protected float _showProgress;
        protected Button _btn;
        protected virtual string redKey { get; set; }
        // protected abstract string ActivityId { get; }

        protected virtual void Awake()
        {           
            _currentResState = IsActivityResReady();
            _oldState = _currentResState;
            _countDownGroup = transform.Find($"Root/Countdown");
            if (_countDownGroup != null)
            {
                _countDownTxt = _countDownGroup.Find("Count").GetComponent<LocalizeTextMeshProUGUI>();
            }
            
            _progressGroup = transform.Find($"Root/Progress");
            if (_progressGroup != null)
            {
                _progressValue = _progressGroup.GetComponent<Slider>();
                _progressTxt = _progressValue.transform.Find($"Count").GetComponent<LocalizeTextMeshProUGUI>();
            }
            
            
            _downProgressGroup = transform.Find($"DownGroup");
            if (_downProgressGroup != null)
            {
                _downProgressValue = _downProgressGroup.Find("Progress").GetComponent<Slider>();
                _downProgressTxt = _downProgressValue.transform.Find($"Count").GetComponent<LocalizeTextMeshProUGUI>();
                if (_downProgressTxt != null) _downProgressTxt.SetText($"0%");
            }

            OnResStateChanged(_oldState, _currentResState);
            reminderObj = gameObject.FindChild("Reminder").gameObject; 
            if (_useRemind)
            {
                reminder = reminderObj.AddComponent<RedPointController>();
                reminder.SetData(redKey);
                SetRedValue();
            }
            else
            {
                reminderObj.gameObject.SetActive(false);
            }

            _btn = GetComponent<Button>();
            if (_btn != null)
            {
                ButtonUtils.Add(_btn, OnClickCall);
            }
            if (_countDownTxt != null) UpdateTime();
            EventDispatcher.Instance.AddEventListener(EventEnum.ACTIVITY_UPDATE, OnActivityUpdate);
            EventDispatcher.Instance.AddEventListener(EventEnum.DOWNLOAD_FINISH, OnActivityUpdate);
        }
        protected virtual void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.ACTIVITY_UPDATE, OnActivityUpdate);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.DOWNLOAD_FINISH, OnActivityUpdate);
        }
        protected virtual void Update()
        {
            _oldState = _currentResState;
            if (_currentResState)
            {
                CountDownTime();
                return;
            }
            if (IsActivityResReady())
            {
                _currentResState = true;
                OnResStateChanged(_oldState, _currentResState);
                return;
            }

            if (_currentResState)
            {
                _currentResState = false;
                OnResStateChanged(_oldState, _currentResState);
            }
            if (_downProgressGroup != null) _downProgressValue.value = GetProgress();
            if (_downProgressTxt != null)
            {
                if (_showProgress == Mathf.Floor(GetProgress() * 100)) return;
                _showProgress = Mathf.Floor(GetProgress() * 100);
                _downProgressTxt.SetText($"{_showProgress}%");
            }
        }


        protected virtual void OnActivityUpdate(BaseEvent obj)
        {
            if (!IsActivityOpened(false) && !IsActivityInReward(false)) return;
            if (gameObject == null) return;
            gameObject.SetActive(true);
            UpdateTime();
        }

        protected virtual void CountDownTime()
        {
            if (_countDownTxt == null) return;
            _lastTime += Time.deltaTime;
            if (_lastTime < _delay) return;
            if (!IsActivityOpened(false) && !IsActivityInReward(false))
            {
                gameObject.SetActive(false);
                return;
            }
            _lastTime = 0;
            UpdateTime();
        }

        protected abstract bool IsActivityResReady();
        protected abstract bool IsActivityOpened(bool isDependRes = true);
        protected abstract bool IsActivityInReward(bool isDependRes = true);
        protected abstract float GetProgress();

        protected virtual void OnResStateChanged(bool oldState, bool newState)
        {
            if (_progressGroup != null) _progressGroup.gameObject.SetActive(newState && _useProgress);
            if (_countDownGroup != null) _countDownGroup.gameObject.SetActive(newState && _useCountDown);
            if (_downProgressGroup != null) _downProgressGroup.gameObject.SetActive(!newState);
        }

        protected virtual void SetRedValue()
        {
            
        }

        protected virtual void UpdateTime()
        {
            
        }
        public virtual void OnClickCall()
        {
            AudioManager.Instance.PlayBtnTap();
            if (!IsActivityOpened(false) && !IsActivityInReward(false))
            {
                DebugUtil.LogError("活动没开始");
                return;
            }
            // Model.Instance.CheckActivityResState(ActivityId);
            if (!IsActivityResReady())
            {
                string trim = "UI_acitvity_download_tips";
                if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                    trim = "UI_acitvity_download_no_net";
                }
                DebugUtil.LogError("活动资源未准备好");
                FrameWorkUINotice.Open(new UINoticeData()
                {
                    DescString= LocalizationManager.Instance.GetLocalizedString(trim),
                });
                _oldState = _currentResState;
                _currentResState = false;
                OnResStateChanged(_oldState, _currentResState);
                // Model.Instance.TryPullActivityRes(ActivityId);
                return;
            }
            OnClick();
        }
        
        protected abstract void OnClick();
    }

    public class UIEntranceBase<T> : UIEntranceBase where T : ActivityEntityBase
    {
        protected virtual T Model { get; set; }
        // protected override string ActivityId { get; }
        protected override bool IsActivityResReady()
        {
            return Model != null && ActivityManager.Instance.IsActivityResourcesDownloaded(Model.ActivityId);
        }

        protected override bool IsActivityOpened(bool isDependRes = true)
        {
            return Model != null && Model.IsOpened(isDependRes);
        }

        protected override bool IsActivityInReward(bool isDependRes = true)
        {
            return Model != null && Model.IsInReward(isDependRes);
        }

        protected override float GetProgress()
        {
            return Model != null ? ActivityManager.Instance.GetActivityResourcesDownloadProgress(Model.ActivityId): 0;
        }

        protected override void OnClick()
        {
            
        }
    }
}