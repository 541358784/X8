using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CKScrollView : ScrollRect, IPointerDownHandler, IPointerUpHandler
{
    public bool IsTouched = false;

    private Vector2 LastTouchPoint = Vector2.zero;

    private DateTime LastDownTime;

    private const int MinTapTime = 50;

    public delegate void OnTapEventDelegate();
    public OnTapEventDelegate OnTapScreen;

    public void OnPointerClick(PointerEventData eventData)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        GameObject current = eventData.pointerCurrentRaycast.gameObject;
        for (int i = 0; i < results.Count; i++)
        {
            CKEmptyButton ckeb = results[i].gameObject.GetComponent<CKEmptyButton>();
            if (ckeb != null)
            {
                ckeb.OnClick();
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        IsTouched = true;
        LastTouchPoint = eventData.position;
        LastDownTime = DateTime.Now;

        OnTapScreen?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsTouched = false;
        int deltaTime = (DateTime.Now - LastDownTime).Milliseconds;

        if (deltaTime >= MinTapTime && Vector2.Distance(LastTouchPoint, eventData.position) < 120f)
        {
            OnPointerClick(eventData);
        }
    }
}
