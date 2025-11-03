using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollTopDetector : MonoBehaviour, IEndDragHandler
{
    [Header("组件引用")] private ScrollRect _scrollRect;
    public ScrollRect scrollRect
    {
        get
        {
            if (_scrollRect == null) _scrollRect = GetComponent<ScrollRect>();
            return _scrollRect;
        }
        set
        {
            _scrollRect = value;
        }
    }

    private Action OverTopCallback;
    private Action OverBottomCallback;

    public void OnOverTop(Action action)
    {
        OverTopCallback = action;
    }
    public void OnOverBottom(Action action)
    {
        OverBottomCallback = action;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (scrollRect.verticalNormalizedPosition >= 1)
        {
            Debug.LogError("下拉刷新触发！");
            OverTopCallback?.Invoke();
        }
        else if (scrollRect.verticalNormalizedPosition <= 0)
        {
            OverBottomCallback?.Invoke();
            Debug.LogError("上拉刷新触发！");
        }
    }
    
}