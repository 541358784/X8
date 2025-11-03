using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TileMatch.Game
{
    public class PointerHandlerMono : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private Action<PointerEventData> _onPointerEnter;
        private Action<PointerEventData> _onPointerExit;
        
        public void Init(Action<PointerEventData> onPointerEnter, Action<PointerEventData> onPointerExit)
        {
            _onPointerEnter = onPointerEnter;
            _onPointerExit = onPointerExit;
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            _onPointerEnter?.Invoke(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _onPointerExit?.Invoke(eventData);
        }
    }
}