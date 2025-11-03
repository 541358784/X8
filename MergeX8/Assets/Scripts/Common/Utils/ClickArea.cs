using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickArea : MonoBehaviour, IPointerClickHandler,IPointerDownHandler, IPointerUpHandler,IDragHandler
{
    private Action<Vector3> ClickCallback;
    private Action<Vector3> DownCallback;
    private Action<Vector3> UpCallback;
    private Action<Vector3> DragCallback;
    public void OnClick(Action<Vector3> callback)
    {
        ClickCallback = callback;
    }
    public void OnDown(Action<Vector3> callback)
    {
        DownCallback = callback;
    }
    public void OnUp(Action<Vector3> callback)
    {
        UpCallback = callback;
    }
    public void OnDrag(Action<Vector3> callback)
    {
        DragCallback = callback;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        // 获取点击的世界坐标
        Vector3 clickPosition = eventData.pointerCurrentRaycast.worldPosition;
        ClickCallback?.Invoke(clickPosition);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        Vector3 clickPosition = eventData.pointerCurrentRaycast.worldPosition;
        DownCallback?.Invoke(clickPosition);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Vector3 clickPosition = eventData.pointerCurrentRaycast.worldPosition;
        UpCallback?.Invoke(clickPosition);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 clickPosition = eventData.pointerCurrentRaycast.worldPosition;
        DragCallback?.Invoke(clickPosition);
    }
}