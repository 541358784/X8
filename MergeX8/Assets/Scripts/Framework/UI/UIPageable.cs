using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIPageable : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    private float _startDragHorizontal;
    private bool _isDrag = false; //是否拖拽结束  
    private bool _moving = false;
    private float _startTime;
    private float _fixedSmooting = 4; //基本滑动速度
    public float _smooting = 4; //滑动速度  
    private float _targethorizontal = 0; //滑动的起始坐标  
    private List<float> _posList; //求出每页的临界角，页索引从0开始  
    public float _dragMaxDeltaOfScreen = 0.3f; //最大滑动距离(超出后按一页)
    private int _currentPageIndex = 0;

    private ScrollRect _scrollRect;
    private bool _invokeDelegatePageChanged = true;

    public System.Action OnBeginDragDelegate;
    public System.Action OnEndDragDelegate;
    public System.Action<int> DelegatePageChanged;

    public bool Moving
    {
        get => _moving;
    }

    public void Init(ScrollRect scrollRect, int cellCount, int initIndex)
    {
        _scrollRect = scrollRect;

        _posList = new List<float>(cellCount);
        for (var i = 0; i < cellCount; i++)
        {
            _posList.Add(i * (1f / (cellCount - 1)));
        }

        GotoPage(initIndex, 100f);
    }

    public void Update()
    {
        if (!_isDrag && _moving)
        {
            _startTime += 0.05f;
            var t = _startTime * _smooting;
            _scrollRect.horizontalNormalizedPosition =
                Mathf.Lerp(_scrollRect.horizontalNormalizedPosition, _targethorizontal, t);
            if (t >= 1)
            {
                _moving = false;
                _smooting = _fixedSmooting;
                if (_invokeDelegatePageChanged) DelegatePageChanged?.Invoke(_currentPageIndex);
            }
        }
    }

    public void GotoPage(int index, float smooting = -1)
    {
        index = Mathf.Clamp(index, 0, _posList.Count - 1);
        _invokeDelegatePageChanged = _currentPageIndex != index;
        _targethorizontal = _posList[index];
        _isDrag = false;
        _startTime = 0;
        _moving = true;
        _currentPageIndex = index;
        _smooting = smooting == -1 ? _fixedSmooting : smooting;
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        _moving = true;
        _isDrag = true;

        _startDragHorizontal = _scrollRect.horizontalNormalizedPosition;
        OnBeginDragDelegate?.Invoke();
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        OnEndDragDelegate?.Invoke();

        var posX = _scrollRect.horizontalNormalizedPosition;

        var delta = _scrollRect.horizontalNormalizedPosition - _startDragHorizontal;
        var sign = Mathf.Sign(delta);
        delta = Mathf.Abs(delta);

        delta *= _posList.Count;

        var targetIndex = _currentPageIndex;
        if (delta >= _dragMaxDeltaOfScreen)
        {
            targetIndex += (int) sign;
        }

        GotoPage(targetIndex, _fixedSmooting);
    }
}