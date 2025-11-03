using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Framework.UI.TableView
{
    public class UIScrollRectHolder : UIElement
    {
        private MonoHandler _monoHandler;
        public  ScrollRect  ScrollRect => _scrollRect;

        protected ScrollRect _scrollRect;

        public RectTransform content => _scrollRect.content;

        /// <summary>
        /// 返回true拉到最下面，false拉到最上面
        /// </summary>
        public Action<bool> OnPullDelegate = (a) => { };

        public Action OnEndDragSlider = () => { };

        public Action<PointerEventData> OnBeginDragDelegate;


        public float horizontalNormalizedPosition
        {
            get => _scrollRect.horizontalNormalizedPosition;
            set => _scrollRect.horizontalNormalizedPosition = value;
        }


        protected override void OnCreate()
        {
            base.OnCreate();

            _monoHandler = _gameObject.AddComponent<MonoHandler>();
            _monoHandler.OnBeginDragDelegate = OnBeginDrag;
            _monoHandler.OnEndDragDelegate = OnEndDrag;

            _scrollRect = BindItem<ScrollRect>("");

            _scrollRect.onValueChanged.AddListener(OnMove);

            if (_scrollRect != null && _scrollRect.content != null)
            {
                _scrollRect.content.anchorMax = Vector2.one;
                _scrollRect.content.anchorMin = new Vector2(0, 1);

                var contentSizeFitter = _scrollRect.content.GetComponent<ContentSizeFitter>();
                if (contentSizeFitter != null) contentSizeFitter.enabled = false;

                var verticalLayout = _scrollRect.content.GetComponent<VerticalLayoutGroup>();
                if (verticalLayout != null) verticalLayout.enabled = false;

                var horizontalLayout = _scrollRect.content.GetComponent<HorizontalLayoutGroup>();
                if (horizontalLayout != null) horizontalLayout.enabled = false;
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            BeginDrag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            EndDrag(eventData);
        }

        protected virtual void BeginDrag(PointerEventData eventData)
        {
            _scrollRect.OnBeginDrag(eventData);

            OnBeginDragDelegate?.Invoke(eventData);
        }

        protected virtual void EndDrag(PointerEventData eventData)
        {
            _scrollRect.OnEndDrag(eventData);

            OnEndDragSlider?.Invoke();

            if (_scrollRect.verticalNormalizedPosition < 0)
            {
                OnPullDelegate?.Invoke(true);
            }

            if (_scrollRect.verticalNormalizedPosition > 1)
            {
                OnPullDelegate?.Invoke(false);
            }
        }

        protected virtual void OnMove(Vector2 value)
        {
        }


        public void SetVertical(bool value)
        {
            _scrollRect.vertical = value;
        }
    }
}