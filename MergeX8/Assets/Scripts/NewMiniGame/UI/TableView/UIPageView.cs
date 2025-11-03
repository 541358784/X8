using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;
using Framework.UI;

public class UIPageView : UIElement
{
    private ScrollRect        _scrollRect;           //滑动组件  
    private float             _targethorizontal = 0; //滑动的起始坐标  
    private bool              _isDrag;               //是否拖拽结束  
    private List<float>       _posList;              //求出每页的临界角，页索引从0开始  
    private int               _currentPageIndex = -1;
    private Action<int, bool> _onPageChange;
    private bool              _stopMove = true;
    private float             _smooting = 4; //滑动速度  

    private float _startTime;
    private float _dragMaxDeltaOfScreen = 0.1f; //最大滑动距离(超出后按一页)

    private float _startDragHorizontal;

    protected override void OnCreate()
    {
        base.OnCreate();

        _scrollRect = Transform.GetComponent<ScrollRect>();
        var mono = GameObject.AddComponent<MonoHandler>();

        mono.OnBeginDragDelegate += OnBeginDrag;
        mono.OnEndDragDelegate += OnEndDrag;
        mono.UpdateDelegate += OnUpdate;
    }

    public void Init(Action<int, bool> onPageChange)
    {
        _onPageChange = onPageChange;

        _posList = new List<float>();
        var temp = _scrollRect.content.transform.childCount - 1;
        for (int i = 0; i < _scrollRect.content.transform.childCount; i++)
        {
            //posList.Add(_rectWidth.rect.width * i / horizontalLength);
            _posList.Add(i * (1f / temp));
        }
    }

    private void OnUpdate()
    {
        if (!_isDrag && !_stopMove)
        {
            _startTime += Time.deltaTime;
            float t = _startTime * _smooting;
            _scrollRect.horizontalNormalizedPosition = Mathf.Lerp(_scrollRect.horizontalNormalizedPosition, _targethorizontal, t);
            if (t >= 1)
                _stopMove = true;
        }
        //Debug.Log(rect.horizontalNormalizedPosition);
    }

    public void PageTo(int index)
    {
        if (index < 0) return;
        if (index >= _posList.Count) return;

        SetPageIndex(index, false);
        _targethorizontal = _posList[index]; //设置当前坐标，更新函数进行插值  
        _isDrag = false;
        _startTime = 0;
        _stopMove = false;
    }

    // public void PageTo(int index)
    // {
    //     if (index >= 0 && index < _posList.Count)
    //     {
    //         _scrollRect.horizontalNormalizedPosition = _posList[index];
    //         SetPageIndex(index, false);
    //     }
    // }

    private void SetPageIndex(int index, bool fromDrag)
    {
        if (_currentPageIndex != index)
        {
            _currentPageIndex = index;
            if (_onPageChange != null)
                _onPageChange(index, fromDrag);
        }
    }

    private void OnBeginDrag(PointerEventData eventData)
    {
        _isDrag = true;
        //开始拖动
        _startDragHorizontal = _scrollRect.horizontalNormalizedPosition;
    }

    private void OnEndDrag(PointerEventData eventData)
    {
        float posX = _scrollRect.horizontalNormalizedPosition;

        var delta = _scrollRect.horizontalNormalizedPosition - _startDragHorizontal;
        var sign = Mathf.Sign(delta);
        delta = Mathf.Abs(delta);

        delta *= _posList.Count;
        if (delta >= _dragMaxDeltaOfScreen)
        {
            posX += sign * 1f / _posList.Count;
        }

        // posX += ((posX - startDragHorizontal) * sensitivity);
        // posX = posX < 1 ? posX : 1;
        // posX = posX > 0 ? posX : 0;
        int index = 0;

        float offset = Mathf.Abs(_posList[index] - posX);
        //Debug.Log("offset " + offset);


        for (int i = 1; i < _posList.Count; i++)
        {
            float temp = Mathf.Abs(_posList[i] - posX);
            //Debug.Log("temp " + temp);
            //Debug.Log("i" + i);
            if (temp < offset)
            {
                index = i;
                offset = temp;
            }
            //Debug.Log("index " + index);
        }

        //Debug.Log(index);
        SetPageIndex(index, true);
        _targethorizontal = _posList[index]; //设置当前坐标，更新函数进行插值  
        _isDrag = false;
        _startTime = 0;
        _stopMove = false;
    }


    public void SetScrollAble(bool scrollEnable)
    {
        _scrollRect.enabled = scrollEnable;
    }
}