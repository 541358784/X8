using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TMatch
{
    public class UIPopupBGClickHandler : MonoBehaviour, IPointerClickHandler
    {
        public Action handle;

        public void OnPointerClick(PointerEventData eventData)
        {
            List<RaycastResult> list = new List<RaycastResult>();
            gameObject.GetComponent<GraphicRaycaster>().Raycast(eventData, list);
            if (list.Count > 0 && list[0].gameObject == gameObject)
            {
                handle?.Invoke();
            }
        }
    }
}