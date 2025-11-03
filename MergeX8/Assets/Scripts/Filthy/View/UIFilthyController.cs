using System.Collections.Generic;
using DragonPlus.Config.Filthy;
using Filthy.Event;
using Filthy.Game;
using Filthy.Model;
using Filthy.SubFsm;
using Spine.Unity;
using SRF;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

namespace Filthy.View
{
    public class UIFilthyController : MonoBehaviour
    {
        public SkeletonGraphic _skeletonGraphic;
        public Animator _animator;
        public List<UIFilthyBubble> _bubbles = new List<UIFilthyBubble>();
        private Button _enterButton;
        private Canvas _canvas;
        private GameObject _bubblesObj;
        
        private SubFsm.Base.Fsm _fsm;
        private bool _init = false;
        
        private void Awake()
        {
            transform.Find("Root/screwMask").gameObject.SetActive(false);
            transform.Find("Root/screwScale/screwbg").gameObject.SetActive(false);
            transform.Find("Root/screwScale/screwNode").gameObject.SetActive(false);
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
            
            foreach (Transform child in transform.Find("Root/bubbles").transform)
            {
                child.gameObject.SetActive(false);
            }
            
            var nodes = FilthyConfigManager.Instance.FilthyNodesList.FindAll(a=>a.LevelId == FilthyModel.Instance._config.LevelId);
            foreach (var config in nodes)
            {
                _bubbles.Add(new UIFilthyBubble(transform, config));
            }

            _bubblesObj = transform.Find("Root/bubbles").gameObject;
            _bubblesObj.gameObject.SetActive(false);
            
            Event_Refresh_State(null);
        }

        private void Start()
        {
            StartCoroutine(CommonUtils.DelayWork(1f, () =>
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
            var bubble = _bubbles.Find(a => a.state == FilthyModel.NodeState.UnLock);
            if(bubble == null)
                return;
            
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.StimulateChoseBubble, bubble.nodeConfig.Id.ToString());
            FilthyGameLogic.Instance.EnterGame(bubble.nodeConfig);
        }
        private void OnCloseButton()
        {
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.StimulateClose);
            SceneFsm.mInstance.ChangeState(StatusType.ExitStimulate);
        }
        
        private void Event_Refresh_State(BaseEvent e)
        {
            _bubbles.ForEach(a=>a.RefreshState());
            
            _fsm = new SubFsm.Base.Fsm();
            _fsm.ChangeState<SubFsm_InitSkin>(this);
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