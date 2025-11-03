// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2023/10/10/16:14
// Ver : 1.0.0
// Description : PointerScaleAnimation.cs
// ChangeLog :
// **********************************************

using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Screw
{
    [RequireComponent(typeof(Button))]
    public class PointerScaleAnimation : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
    {
        [SerializeField]
        private Transform _transform;

        [SerializeField]
        private float _downRatio = 0.97f;

        [SerializeField]
        private float _overshootRatio = 1.02f;
        
        [SerializeField]
        private float _animationDuration = 0.25f;
 
        private Vector3 _initialScale;
 
        private Tweener _tweener;

        private Button _button;

        protected virtual void Awake()
        {
            _initialScale = _transform.localScale;
            _button = GetComponent<Button>();
        }

      
        private void OnDisable()
        {
            Reset();
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (_button && !_button.interactable)
                return;
            _tweener = _transform.DOScale(_initialScale * _downRatio, _animationDuration);
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            if (_button && !_button.interactable)
                return;
            _tweener = _transform.DOScale(_initialScale * _overshootRatio, _animationDuration);
            _tweener.onComplete += OnPointerUpScaleCompleted;
        }

        private void OnPointerUpScaleCompleted()
        {
            Reset();
        }

        private void Reset()
        {
            if (_tweener != null)
            {
                _tweener.Kill();
                _transform.localScale = _initialScale;
            }
        }

        public void ResetAnimation()
        {
            Reset();
        }
    }
}