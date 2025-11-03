using System;
using System.Collections.Generic;
using System.Linq;
using DragonPlus.UI;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollRectRecycleController:MonoBehaviour,IBeginDragHandler,IEndDragHandler
{
    public ScrollRect scrollRect;
    public ScrollRect ScrollRect
    {
        get
        {
            if (scrollRect == null)
            {
                scrollRect = transform.GetComponent<ScrollRect>();
            }
            return scrollRect;
        }
    }

    public Func<object, RectTransform> GetItemFunc;
    public void BindGetItemFunc(Func<object, RectTransform> func)
    {
        GetItemFunc = func;
    }
    public Action<RectTransform> RecycleItemFunc;
    public void BindRecycleItemFunc(Action<RectTransform> func)
    {
        RecycleItemFunc = func;
    }
    public RectTransform Content => ScrollRect?.content;
    public RectTransform ViewPort => ScrollRect?.viewport;

    public float ViewPortOffsetPercent = 2f;

    public List<RectTransform> VisibleItemList = new List<RectTransform>();
    public List<object> DataList = new List<object>();
    public int TopIndex = 0;//闭区间,已加载TopIndex
    public int BottomIndex = 0;//开区间，未加载BottomIndex
    public bool IsInDrag;
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        IsInDrag = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        IsInDrag = false;
    }

    private void Awake()
    {
        ScrollRect.onValueChanged.AddListener((e) =>
        {
            if (IsInDrag)
                return;
            var hasChange = false;
            while (TryLoadTop())//上方补足
            {
                hasChange = true;
                
            }
            while (TryLoadBottom())//下方补足
            {
                hasChange = true;
            }
            while (VisibleItemList.Count > 0 && VisibleItemList[0].GetWorldRect().yMin > ViewPort.GetWorldRect().yMax + ViewPortOffsetPercent * ViewPort.GetWorldRect().height) //上方回收
            {
                RecycleTop();
                hasChange = true;
            }
            while (VisibleItemList.Count > 0 && VisibleItemList.Last().GetWorldRect().yMax < ViewPort.GetWorldRect().yMin - ViewPortOffsetPercent * ViewPort.GetWorldRect().height) //下方回收
            {
                RecycleBottom();
                hasChange = true;
            }
        });
    }
    private void RecycleAll()
    {
        while (VisibleItemList.Count > 0)
        {
            RecycleBottom();
        }
    }

    public void SetData(List<object> data)
    {
        RecycleAll();
        DataList = data.DeepCopy();
        TopIndex = 0;
        BottomIndex = 0;
    }
    public void RebuildWithFocusIndex(int index,float focusY)//focusY传入聚焦元素坐标在显示区域的Y轴占比,0~1
    {
        RecycleAll();
        if (index < 0)
            return;
        if (focusY < 0)
            focusY = 0;
        if (focusY > 1)
            focusY = 1;
        var viewPortY = ViewPort.GetHeight();
        var anchorY = (focusY-1) * viewPortY;
        Content.SetAnchorPositionY(anchorY);
        TopIndex = index;
        BottomIndex = index;
        LoadBottom();
        while (TryLoadBottom()) { }
        while (TryLoadTop()) { }
    }
    public void AddData(object data)
    {
        DataList.Add(data);
        if (VisibleItemList.Count == 0 ||  //没有显示的item
            (VisibleItemList.Last().GetWorldRect().yMin > ViewPort.GetWorldRect().yMin && BottomIndex == DataList.Count-1))//可在下方加载新的item且添加的正好是要加载的item
        {
            var item = LoadBottom();
        }
    }
    public void InsertData(object data,int index)
    {
        DataList.Insert(index,data);
        DealInsert(index);
    }

    private void UpdateSiblingIndex()
    {
        for (var i = 0; i < VisibleItemList.Count; i++)
        {
            VisibleItemList[i].SetSiblingIndex(i);
        }
    }
    
    private RectTransform LoadTop()
    {
        if (TopIndex <= 0)
            return null;
        TopIndex--;
        var data = DataList[TopIndex];
        var item = GetItemFunc(data);
        VisibleItemList.Insert(0,item);
        var heightOld = Content.GetHeight();
        item.SetParent(Content,false);
        UpdateSiblingIndex();
        LayoutRebuilder.ForceRebuildLayoutImmediate(Content);
        var heightNew = Content.GetHeight();
        var distance = heightNew - heightOld;
        Content.SetAnchorPositionY(Content.anchoredPosition.y + distance);
        return item;
    }
    
    private RectTransform LoadBottom()
    {
        if (BottomIndex >= DataList.Count || BottomIndex < 0)
            return null;
        var data = DataList[BottomIndex];
        BottomIndex++;
        var item = GetItemFunc(data);
        VisibleItemList.Add(item);
        // var heightOld = Content.GetHeight();
        item.SetParent(Content,false);
        UpdateSiblingIndex();
        LayoutRebuilder.ForceRebuildLayoutImmediate(Content);
        // var heightNew = Content.GetHeight();
        // var distance = heightNew - heightOld;
        // Content.SetAnchorPositionY(Content.anchoredPosition.y + distance);
        return item;
    }
    private RectTransform TryLoadTop()
    {
        if (VisibleItemList.Count > 0 && VisibleItemList[0].GetWorldRect().yMax <= ViewPort.GetWorldRect().yMax +ViewPortOffsetPercent * ViewPort.GetWorldRect().height)//上方补足
        {
            return LoadTop();
        }
        else
        {
            return null;
        }
    }
    private RectTransform TryLoadBottom()
    {
        if (VisibleItemList.Count > 0 && (VisibleItemList.Last().GetWorldRect().yMin >= ViewPort.GetWorldRect().yMin - ViewPortOffsetPercent * ViewPort.GetWorldRect().height))//下方补足
        {
            return LoadBottom();
        }
        else
        {
            return null;
        }
    }

    private void RecycleTop()
    {
        if (VisibleItemList.Count == 0)
            return;
        TopIndex++;
        var recycleItem = VisibleItemList[0];
        VisibleItemList.RemoveAt(0);
        var heightOld = Content.GetHeight();
        RecycleItemFunc(recycleItem);
        UpdateSiblingIndex();
        LayoutRebuilder.ForceRebuildLayoutImmediate(Content);
        var heightNew = Content.GetHeight();
        var distance = heightOld - heightNew;
        Content.SetAnchorPositionY(Content.anchoredPosition.y - distance);
    }
    private void RecycleBottom()
    {
        if (VisibleItemList.Count == 0)
            return;
        BottomIndex--;
        var recycleItem = VisibleItemList[VisibleItemList.Count-1];
        VisibleItemList.RemoveAt(VisibleItemList.Count-1);
        // var heightOld = Content.GetHeight();
        RecycleItemFunc(recycleItem);
        UpdateSiblingIndex();
        LayoutRebuilder.ForceRebuildLayoutImmediate(Content);
        // var heightNew = Content.GetHeight();
        // var distance = heightOld - heightNew;
        // Content.SetAnchorPositionY(Content.anchoredPosition.y - distance);
    }
    
    private RectTransform DealInsert(int insertIndex)//处理插入，默认固定插入元素上方，对插入元素下方重新排列
    {
        if (TopIndex >= insertIndex)//插入元素在显示范围之上，双下标++
        {
            TopIndex++;
            BottomIndex++;
            return null;
        }
        else if (BottomIndex <= insertIndex) //插入元素在显示范围下方，双下标不用改
        {
            return null;
        }
        else //插入元素在现实范围之间，执行插入逻辑
        {
            for (var i = 0; i < (BottomIndex - insertIndex); i++)//删除插入元素下方的元素
            {
                RecycleBottom();
            }
            RectTransform insertItem = null;
            while (TryLoadBottom())//加载元素至显示区域满
            {
                if (insertIndex == BottomIndex - 1)//生成的是插入元素
                {
                    insertItem = VisibleItemList.Last();
                }
            }
            return insertItem;   
        }
    }
    
    
}