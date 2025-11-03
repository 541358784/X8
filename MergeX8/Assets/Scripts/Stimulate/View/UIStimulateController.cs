using System;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using Spine.Unity;
using Stimulate.Configs;
using Stimulate.Event;
using Stimulate.FSM_Stimulate.States;
using Stimulate.Model;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace Stimulate.View
{
    public class UIStimulateBubble
    {
        private GameObject _icon;
        private GameObject _lock;
        private Button _button;
        private TableStimulateNodes _nodeConfig;
        private Transform _root;
        private StimulateModel.NodeState _state = StimulateModel.NodeState.Lock;

        public StimulateModel.NodeState state
        {
            get { return _state; }
        }

        public TableStimulateNodes nodeConfig
        {
            get { return _nodeConfig; }
        }
        
        public UIStimulateBubble(Transform root, TableStimulateNodes config)
        {
            _root = root;
            _nodeConfig = config;

            _button = _root.transform.Find(_nodeConfig.nodePath).GetComponent<Button>();
            _button.onClick.AddListener(OnClickButton);

            _icon = _button.transform.Find("Icon").gameObject;
            _lock = _button.transform.Find("Lock").gameObject;

            RefreshState();
            
            var topLayer = new List<Transform>();
            topLayer.Add(_button.transform);
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.StimulateChoseBubble, _button.transform as RectTransform, targetParam:config.id.ToString(), topLayer: topLayer);
        }

        private void OnClickButton()
        {
            if(_state < StimulateModel.NodeState.UnLock)
                return;
            
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.StimulateChoseBubble, _nodeConfig.id.ToString());
            StimulateGameLogic.Instance.EnterGame(_nodeConfig);
        }

        public void RefreshState()
        {
            _button.gameObject.SetActive(true);
            _lock.gameObject.SetActive(false);
            _state =  StimulateModel.Instance.GetNodeState(_nodeConfig.levelId, _nodeConfig.id);
            switch (_state)
            {
                case StimulateModel.NodeState.Lock:
                {
                    _lock.gameObject.SetActive(true);
                    break;
                }
                case StimulateModel.NodeState.Owned:
                case StimulateModel.NodeState.Finish:
                {
                    if (!StimulateModel.Instance.IsFinish(StimulateModel.Instance._config))
                        _button.gameObject.SetActive(false);
                    
                    break;
                }
            }
        }

        public void GuideLogic()
        {
            _state =  StimulateModel.Instance.GetNodeState(_nodeConfig.levelId, _nodeConfig.id);
            
            if(_state == StimulateModel.NodeState.UnLock)
                GuideSubSystem.Instance.Trigger(GuideTriggerPosition.StimulateChoseBubble, _nodeConfig.id.ToString(), _nodeConfig.id.ToString());
        }
    }
    
    public class UIStimulateController : MonoBehaviour
    {
        public SkeletonGraphic _skeletonGraphic;
        public Animator _animator;
        public List<UIStimulateBubble> _bubbles = new List<UIStimulateBubble>();
        private Button _enterButton;
        private Canvas _canvas;
        private GameObject _bubblesObj;
        
        private FSM_Stimulate.Fsm _fsm;
        private bool _init = false;
        
        private void Awake()
        {
            _skeletonGraphic = transform.Find("Root/Spine").GetComponent<SkeletonGraphic>();
            _animator = transform.GetComponent<Animator>();
            
            _enterButton = transform.Find("Root/EnterButton").GetComponent<Button>();
            _enterButton.onClick.AddListener(OnEnterButton);
            _enterButton.gameObject.SetActive(false);
            
            var closeBtn = transform.Find("Root/CloseButton").GetComponent<Button>();
            closeBtn.onClick.AddListener(OnCloseButton);
            
            var topLayer = new List<Transform>();
            topLayer.Add(closeBtn.transform);
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.StimulateClose, closeBtn.transform as RectTransform, topLayer: topLayer);

            
            EventDispatcher.Instance.AddEventListener(ConstEvent.Event_Refresh_State, Event_Refresh_State);
            EventDispatcher.Instance.AddEventListener(ConstEvent.Event_Refresh_Guide, Event_Refresh_Guide);
            
            _canvas = gameObject.GetOrCreateComponent<Canvas>();
            _canvas.overrideSorting = true;
            gameObject.GetOrCreateComponent<GraphicRaycaster>();
            
            _canvas.sortingOrder = 2;
            
            var nodes = StimulateConfigManager.Instance.GetNodes(StimulateModel.Instance._config.levelId);
            foreach (var config in nodes)
            {
                _bubbles.Add(new UIStimulateBubble(transform, config));
            }

            _bubblesObj = transform.Find("Root/bubbles").gameObject;
            _bubblesObj.gameObject.SetActive(false);
            
            Event_Refresh_State(null);
        }

        private void Start()
        {
            StartCoroutine(CommonUtils.DelayWork(1.5f, () =>
            {
                _init = true;
                Event_Refresh_Guide(null);
                _bubblesObj.gameObject.SetActive(true);
            }));
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEventListener(ConstEvent.Event_Refresh_State, Event_Refresh_State);
            EventDispatcher.Instance.RemoveEventListener(ConstEvent.Event_Refresh_Guide, Event_Refresh_Guide);
            
        }

        private void OnEnterButton()
        {
            var bubble = _bubbles.Find(a => a.state == StimulateModel.NodeState.UnLock);
            if(bubble == null)
                return;
            
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.StimulateChoseBubble, bubble.nodeConfig.id.ToString());
            StimulateGameLogic.Instance.EnterGame(bubble.nodeConfig);
        }
        private void OnCloseButton()
        {
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.StimulateClose);
            SceneFsm.mInstance.ChangeState(StatusType.ExitStimulate);
        }
        
        private void Event_Refresh_State(BaseEvent e)
        {
            _bubbles.ForEach(a=>a.RefreshState());
            // var bubble = _bubbles.Find(a => a.state == StimulateModel.NodeState.UnLock);
            // _enterButton.gameObject.SetActive(bubble != null);
            
            _fsm = new FSM_Stimulate.Fsm();
            _fsm.ChangeState<FsmInitSkin>(this);
        }

        private void Event_Refresh_Guide(BaseEvent e)
        {
            if(!_init)
                return;
            
            _bubbles.ForEach(a=>a.GuideLogic());
            GuideSubSystem.Instance.Trigger(GuideTriggerPosition.StimulateClose, null);
        }
    }
}