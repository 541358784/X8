using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Framework.UI
{
    public class MonoHandler : MonoBehaviour, IBeginDragHandler, IEndDragHandler
    {
        public Action<PointerEventData> OnBeginDragDelegate;
        public Action<PointerEventData> OnEndDragDelegate;

        public Action UpdateDelegate;

        public void OnBeginDrag(PointerEventData eventData)
        {
            OnBeginDragDelegate?.Invoke(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            OnEndDragDelegate?.Invoke(eventData);
        }

        private void Update()
        {
            UpdateDelegate?.Invoke();
        }
    }
}