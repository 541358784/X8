using System.Collections;
using System.Collections.Generic;
using Deco.Node;
using DragonPlus;
using Merge.Order;
using UnityEngine;
using UnityEngine.UI;
namespace Decoration
{
    public abstract class FollowTargetBase : MonoBehaviour
    {
        private Transform _selfTransform;
        public Transform _targetTransform;
        protected float _baseMinScale = 0.7f;
        protected float _baseMaxScale = 0.8f;
        private float _orgScale = 0.5f;
        private float _diffScale;
        
        protected Vector3 _diff = Vector3.zero;

        protected GameObject _msgGroup;
        protected LocalizeTextMeshProUGUI _nodeIdText;
        public DecoNode _node = null;

        protected void Init()
        {
            if (Debug.isDebugBuild)
            {
                _msgGroup = transform.Find("Root/MessageGroup").gameObject;
                _nodeIdText = _msgGroup.transform.Find("MessageText").GetComponent<LocalizeTextMeshProUGUI>();
            }
        }

        protected void Bind()
        {
            if (Debug.isDebugBuild && MainOrderManager.Instance.OpenDebugModule)
            {
                if (_node != null)
                {
                    if (_msgGroup == null)
                    {
                        Init();
                    }
                    
                    if (_msgGroup == null)
                        return;
                    
                    _msgGroup.gameObject.SetActive(true);
                    _nodeIdText.SetText(_node.Id.ToString());
                }
            }
        }
        // 跟随目标
        public virtual void FollowTarget(Transform target)
        {
            _selfTransform = transform;
            _targetTransform = target;
            //
            // _diffScale = (_baseMaxScale - _baseMinScale) / 2;
            // _orgScale = _baseMinScale + _diffScale;
        }

        // LateUpdate里，同步两者的世界坐标
        private void LateUpdate()
        {
            OnLateUpdate();
        }

        protected virtual void OnLateUpdate()
        {
            if (!DecoManager.Instance.EnableUpdate)
                return;
            
            if (_targetTransform == null)
                return;
            
            var targetTransfrom = _targetTransform as RectTransform;
            if (targetTransfrom == null)
            {
                var position = DecoSceneRoot.Instance.mSceneCamera.WorldToScreenPoint(_targetTransform.transform.position + _diff);
                var screenPos = UIRoot.Instance.mUICamera.ScreenToWorldPoint(position);
                screenPos.z = 0;
                _selfTransform.position = screenPos;

                // var scale = DecoManager.Instance.CurrentWorld.PinchMap.GetRelativeCameraScale();
                // // if (scale >= 0)
                // //     scale = -1*(1-_baseMinScale) * scale + 1;
                // // else//放
                // //     scale = -1*(_baseMaxScale - 1) * scale + 1;
                //
                // if (scale >= 0)
                // {
                //     scale = _diffScale * scale + _orgScale;
                // }
                // else
                // {
                //     scale =  _orgScale + _diffScale * scale;
                // }
                
                this.transform.localScale = new Vector2(_orgScale, _orgScale);
            }
            else
            {
                _selfTransform.position = _targetTransform.transform.position;
            }
        }

        public virtual void BindNode(DecoNode node)
        {
            _node = node;
        }
        
        public virtual DecoNode GetNode()
        {
            return _node;
        }
    }
}