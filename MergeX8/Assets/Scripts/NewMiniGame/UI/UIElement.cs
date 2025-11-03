using System;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Asset;
using Framework.UI.fsm;
using MiniGame;
using NewMiniGame.Fsm;
using Spine.Unity;
using Spine.Unity.Playables;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Framework.UI
{
    public class UIElement : UIWindow
    {
        public GameObject GameObject => _gameObject;
        public Transform Transform => _transform;

        protected GameObject _gameObject;
        protected Transform  _transform;
        public    UIElement  Parent;

        private static AudioClip        _commonBtnClickSound;
        protected      Animator         _viewAnimator;
        protected      PlayableDirector _director;
        protected      AudioSource[]    _audioSource;

        private List<UIElement> _subViews;
        private List<UIElement> _pendingToRemoveSubview;

        private Dictionary<Type, Delegate> _eventActions;

        protected StateMachine<UIStateBase> _stateMachine;

        public UIStateBase CurrentState => _stateMachine.CurrentState;

        protected string OpenAnimName;
        protected string CloseAnimName;

        private const string sound_common_click = "Audio/Common/Sound_common_click";

        private RectTransform _rectTransform;

        public RectTransform RectTrans
        {
            get
            {
                if (_rectTransform == null) _rectTransform = _transform as RectTransform;

                return _rectTransform;
            }
        }

        public bool HasDirector => _director != null;
        public PlayableDirector Director => _director;
        public virtual float OpenTimelineTime => (float)_director.duration;
        protected virtual bool IgnoreCloseAni => false;
        private float _normalAniSpeed;

        public void Create(GameObject obj, UIData data)
        {
            _gameObject = obj;
            _transform = _gameObject.transform;
            _subViews = new();
            _pendingToRemoveSubview = new();
            _viewAnimator = _gameObject.GetComponent<Animator>();
            _director = _gameObject.GetComponent<PlayableDirector>();
            _audioSource = _gameObject.GetComponentsInChildren<AudioSource>(true);

            OpenAnimName = "Appear";
            CloseAnimName = "Disappear";
            _normalAniSpeed = _viewAnimator ? _viewAnimator.speed : 1;

            OnCreate();

            OnOpen<UIStateNormal>(data);
        }


        protected virtual void OnCreate()
        {
            _stateMachine = new();

            SetAudioSource(SettingManager.Instance.SoundClose);
        }

        protected void SetAudioSource(bool mute)
        {
            if (_audioSource != null)
            {
                foreach (var audioSource in _audioSource)
                {
                    audioSource.mute = mute;
                }
            }
        }

        protected internal virtual void OnOpen<M, T>(UIData data)
            where T : UIStateNormal, new() where M : UIStateOpen<T>, new()
        {
            _stateMachine.SetState<M>(new UIStateData(this));
        }

        protected internal virtual void OnOpen<T>(UIData data) where T : UIStateNormal, new()
        {
            OnOpen<UIStateOpen<T>, T>(data);
        }


        protected virtual void OnClose()
        {
            foreach (var uiSubView in _subViews) uiSubView.OnClose();

            _stateMachine.SetState<UIStateClose>(new UIStateData(this));
            UnRegisterAll();
        }

        protected internal virtual void OnRemove()
        {
            _subViews.Clear();

            Object.Destroy(_gameObject);
        }

        protected Button BindButtonEvent(string path, Action call, bool playCommonSound = true, bool vibrate = true)
        {
            Button btn = null;
            if (string.IsNullOrEmpty(path))
            {
                btn = _gameObject.GetComponent<Button>();
            }
            else
            {
                btn = BindItem<Button>(path);
            }


            if (btn)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() =>
                                        {
                                            if (playCommonSound) PlayCommonBtnSound();
                                            if (vibrate) PlayVibrate();

                                            if (!IsNormalState()) return;

                                            call.Invoke();
                                        });
            }

            return btn;
        }

        protected void ButtonBindEvent(Button btn, Action call, bool playCommonSound = true, bool vibrate = true)
        {
            if (btn)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() =>
                                        {
                                            if (playCommonSound) PlayCommonBtnSound();
                                            if (vibrate) PlayVibrate();

                                            if (!IsNormalState()) return;

                                            call.Invoke();
                                        });
            }
        }

        protected EventTrigger BindTriggerEvent(string path, UnityAction<BaseEventData> call,
                                                EventTriggerType triggerType)
        {
            var trigger = BindItem<EventTrigger>(path);

            if (trigger)
            {
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = triggerType;
                entry.callback.RemoveAllListeners();
                entry.callback = new EventTrigger.TriggerEvent();
                entry.callback.AddListener((value) =>
                                           {
                                               PlayCommonBtnSound();
                                               PlayVibrate();

                                               if (!IsNormalState()) return;

                                               call.Invoke(value);
                                           });
                trigger.triggers.Add(entry);
            }

            return trigger;
        }

        protected bool IsNormalState()
        {
            if (!(_stateMachine.CurrentState is UIStateNormal)) return false;
            if (Parent != null && !Parent.IsNormalState()) return false;

            return true;
        }

        protected Button BindButtonEvent(Action call)
        {
            return BindButtonEvent("", call);
        }

        protected Toggle BindToggleEvent(string path, Action<bool> call)
        {
            var toggle = BindItem<Toggle>(path);

            if (toggle)
            {
                toggle.onValueChanged.RemoveAllListeners();
                toggle.onValueChanged.AddListener(on =>
                                                  {
                                                      if (on) PlayCommonBtnSound();
                                                      PlayVibrate();

                                                      if (!IsNormalState()) return;

                                                      call.Invoke(on);
                                                  });
            }

            return toggle;
        }

        protected void PlayCommonBtnSound()
        {
            if (_commonBtnClickSound == null)
                _commonBtnClickSound = ResourcesManager.Instance.LoadResource<AudioClip>(sound_common_click);
            AudioManager.Instance.PlaySound(_commonBtnClickSound);
        }

        protected void PlayVibrate()
        {
            if (SettingManager.Instance.ShakeClose) return;

            ShakeManager.Instance.ShakeSelection();
        }

        protected T BindItem<T>(string path)
        {
            return BindItem<T>(path, _transform);
        }

        protected T[] BindItemsInChildren<T>(string path)
        {
            return BindItemsInChildren<T>(path, _transform);
        }

        protected T[] BindItemsInChildren<T>(string path, Transform root)
        {
            if (string.IsNullOrEmpty(path))
                return root.GetComponentsInChildren<T>();

            return root.Find(path).GetComponentsInChildren<T>();
        }

        protected T BindItem<T>(string path, Transform root)
        {
            try
            {
                if (string.IsNullOrEmpty(path))
                    return root.GetComponent<T>();

                return root.Find(path).GetComponent<T>();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            return default;
        }

        protected Transform BindItem(string path)
        {
            if (string.IsNullOrEmpty(path)) return _transform;

            return _transform.Find(path);
        }


        public T BindElement<T>(string path, UIData data = null) where T : UIElement, new()
        {
            var binditem = BindItem(path);
            if (binditem == null)
            {
                Debug.LogError($"BindElement error path:{path}");
                return null;
            }

            return BindElement<T>(binditem.gameObject, data);
        }

        public T AddElement<T>(GameObject obj, Transform parent = null, UIData data = null) where T : UIElement, new()
        {
            if (parent)
            {
                obj.transform.SetParent(parent, false);
            }

            return BindElement<T>(obj, data);
        }


        protected void RemoveSubview(UIElement view)
        {
            _pendingToRemoveSubview.Add(view);
        }

        public void RemoveFromParent()
        {
            Parent.RemoveSubview(this);
        }

        public T BindElement<T>(GameObject obj, UIData data = null) where T : UIElement, new()
        {
            var subView = new T();
            subView.Parent = this;
            subView.Create(obj, data);

            _subViews.Add(subView);

            return subView;
        }


        internal float PlayOpenAnim()
        {
            return PlayAnim(OpenAnimName);
        }

        internal float PlayCloseAnim()
        {
            if (IgnoreCloseAni)
                return 0;

            var time = PlayAnim(CloseAnimName);

            var parentTime = Parent?.GetCloseTime() ?? 0;

            return Mathf.Max(time, parentTime);
        }

        internal float GetCloseTime()
        {
            if (IgnoreCloseAni)
                return 0;

            if (_viewAnimator == null)
                return 0;
            
            if (CommonUtils.HasAnim(_viewAnimator, CloseAnimName))
            {
                return _viewAnimator
                    ? CommonUtils.GetAnimTime(_viewAnimator, CloseAnimName)
                    : (Parent == null ? 0 : Parent.GetCloseTime());
            }

            return (Parent == null ? 0 : Parent.GetCloseTime());
        }

        public float PlayAnim(string animName)
        {
            if (!_viewAnimator) return 0f;
            _viewAnimator.Play(animName);

            return CommonUtils.GetAnimTime(_viewAnimator, animName);
        }

        public void PauseAnim()
        {
            if (!_viewAnimator)
                return;

            _viewAnimator.speed = 0;
        }

        public void ResumeAnim()
        {
            if (!_viewAnimator)
                return;

            _viewAnimator.speed = _normalAniSpeed;
        }

        public float GetAnimTime(string animName)
        {
            if (!_viewAnimator) return 0f;

            return CommonUtils.GetAnimTime(_viewAnimator, animName);
        }

        public float DirectorPlay(string fileName)
        {
            if (!_director) return 0f;

            var playableAsset = ResourcesManager.Instance.LoadResource<PlayableAsset>(fileName);
            _director.playableAsset = playableAsset;

            MiniGameModel.Instance.SetGenericBindingByAnimator(_director);
            
            _director.Play();

            return (float)playableAsset.duration;
        }

        protected internal void FixedUpdate()
        {
            var count =_subViews == null ? 0 : _subViews.Count;
            for (var index = 0; index < count; index++)
            {
                if (index < _subViews.Count)
                {
                    var uiElement = _subViews[index];
                    uiElement.FixedUpdate();
                }
            }

            ClearSubViews();

            _stateMachine.FixedUpdate();
        }

        public virtual void OnOpenFinish()
        {
        }

        private void ClearSubViews()
        {
            foreach (var sub in _pendingToRemoveSubview)
            {
                sub.OnClose();
                sub.OnRemove();
                _subViews.Remove(sub);
            }

            _pendingToRemoveSubview.Clear();
        }

        public void ChangeState<T>(StateData param = null) where T : UIStateBase, new()
        {
            if (param == null) param = new UIStateData(this);

            _stateMachine.SetState<T>(param);
        }

        #region 事件上抛和检测

        protected void PopEvent(string evtName, params object[] args)
        {
            if (Parent == null)
            {
                return;
            }

            if (Parent.CheckNeedPopEvent(evtName, args))
            {
                Parent.PopEvent(evtName, args);
            }
        }

        protected virtual bool CheckNeedPopEvent(string evtName, params object[] args)
        {
            return true;
        }

        #endregion


        #region 全局事件的注册和关闭自动移除取消

        //替换EventBus.Register,UI关闭时可以自动事件的事件
        protected void Register<T>(Action<T> act)
        {
            //Framework.Utils.EventBus.Register<T>(act);
            _eventActions = _eventActions != null ? _eventActions : new Dictionary<Type, Delegate>();
            Action<T> handle = null;
            if (_eventActions.TryGetValue(typeof(T), out var del))
            {
                handle = del as Action<T>;
                handle -= act;
            }

            handle += act;
            _eventActions[typeof(T)] = handle;
        }

        protected void UnRegister<T>(Action<T> act)
        {
            //Framework.Utils.EventBus.UnRegister<T>(act);
            var typeKey = typeof(T);
            if (_eventActions != null && _eventActions.TryGetValue(typeKey, out var _delegate))
            {
                var _delegateNew = Delegate.Remove(_delegate, act);
                if (_delegateNew != null)
                {
                    _eventActions[typeKey] = _delegateNew;
                }
                else
                {
                    _eventActions.Remove(typeKey);
                }
            }
        }

        private void UnRegisterAll()
        {
            if (_eventActions == null)
            {
                return;
            }

            foreach (var kvp in _eventActions)
            {
                if (kvp.Value != null)
                {
                    foreach (var act in kvp.Value.GetInvocationList())
                    {
                        Framework.Utils.EventBus.UnRegister(kvp.Key, act);
                    }
                }
            }

            _eventActions.Clear();
            _eventActions = null;
        }

        #endregion

        public override void PrivateAwake()
        {
        }
    }
}