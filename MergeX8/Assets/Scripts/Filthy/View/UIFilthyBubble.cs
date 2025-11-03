using System.Collections.Generic;
using DragonPlus.Config.Filthy;
using Filthy.Game;
using Filthy.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Filthy.View
{
    public class UIFilthyBubble
    {
        private GameObject _icon;
        private GameObject _lock;
        private Button _button;
        private FilthyNodes _nodeConfig;
        private Transform _root;
        private FilthyModel.NodeState _state = FilthyModel.NodeState.Lock;

        public FilthyModel.NodeState state
        {
            get { return _state; }
        }

        public FilthyNodes nodeConfig
        {
            get { return _nodeConfig; }
        }
        
        public UIFilthyBubble(Transform root, FilthyNodes config)
        {
            _root = root;
            _nodeConfig = config;

            _button = _root.transform.Find(_nodeConfig.NodePath).GetComponent<Button>();
            _button.onClick.AddListener(OnClickButton);

            _icon = _button.transform.Find("Icon").gameObject;
            _lock = _button.transform.Find("Lock").gameObject;

            RefreshState();
            
            var topLayer = new List<Transform>();
            topLayer.Add(_button.transform);
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.StimulateChoseBubble, _button.transform as RectTransform, targetParam:config.Id.ToString(), topLayer: topLayer);
        }

        private void OnClickButton()
        {
            if(_state < FilthyModel.NodeState.UnLock)
                return;
            
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.StimulateChoseBubble, _nodeConfig.Id.ToString());
            FilthyGameLogic.Instance.EnterGame(_nodeConfig);
        }

        public void RefreshState()
        {
            _button.gameObject.SetActive(true);
            _lock.gameObject.SetActive(false);
            _state =  FilthyModel.Instance.GetNodeState(_nodeConfig.LevelId, _nodeConfig.Id);
            switch (_state)
            {
                case FilthyModel.NodeState.Lock:
                {
                    _lock.gameObject.SetActive(true);
                    break;
                }
                case FilthyModel.NodeState.Owned:
                case FilthyModel.NodeState.Finish:
                {
                    if (!FilthyModel.Instance.IsFinish(FilthyModel.Instance._config.LevelId))
                        _button.gameObject.SetActive(false);
                    
                    break;
                }
            }
        }

        public void GuideLogic()
        {
            _state =  FilthyModel.Instance.GetNodeState(_nodeConfig.LevelId, _nodeConfig.Id);
            
            if(_state == FilthyModel.NodeState.UnLock)
                GuideSubSystem.Instance.Trigger(GuideTriggerPosition.StimulateChoseBubble, _nodeConfig.Id.ToString(), _nodeConfig.Id.ToString());
        }
    }
}