using System;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus.UI;
using DragonU3DSDK;
using Framework;
using Framework.Wrapper;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = UnityEngine.Object;

/// <summary>
/// 列表单项的基类
/// </summary>
/// <typeparam name="D"> 与UI列表单项对应的数据类</typeparam>
public abstract class UIListItem<D>
{
    protected UIGameObjectWrapper _go;
    protected object _context;
    protected int _index;

    public void Init(GameObject go, object context)
    {
        _go = new UIGameObjectWrapper(go);
        _go.SetActive(true);
        _context = context;
        InitUI(_go);
    }

    public GameObject GetGameObject()
    {
        return _go.GameObject;
    }

    public void SetScale(float scale)
    {
        _go.SetScale(scale);
    }

    public void SetParent(Transform parent)
    {
        _go.SetParent(parent, false);
    }

    public void SetActive(bool active)
    {
        _go.SetActive(active);
    }

    public void Dispose()
    {
        OnDispose();
    }

    public void ReUse(D data)
    {
        OnBindData(data);
    }

    public void SetSelected(bool selected)
    {
        OnSelected(selected);
    }

    public void SetIndex(int index)
    {
        _index = index;
    }

    public void DistancePercentFromViewCenter(float percent)
    {
        OnDistancePercentFromViewCenter(percent);
    }

    public void Release()
    {
        Dispose();
        if (_go != null)
        {
            GameObjectFactory.Destroy(_go.GameObject);
            _go = null;
        }
    }

    /// <summary>
    /// item 初始化，缓存界面元素等；
    ///
    ///  code example
    ///
    /// _image = go.GetItem<Image>("xxxx/xxxx/xxxx");
    /// _title = go.GetItem("xxxx/xxxx/xxxx");
    ///
    /// 
    /// </summary>
    /// <param name="go"> 列表下的单项 gameobject 对应的包装类</param>
    protected abstract void InitUI(UIGameObjectWrapper go);

    /// <summary>
    /// item释放资源 （离开可视区域或者界面销毁时）
    ///
    ///  code example
    /// 
    /// res.Dispose();
    /// _image.sprite = null;
    /// _title = null;
    /// button.onClick.RemoveAllListeners(); // 注意：由于UIListItem是复用的，button 一定要清理listener
    /// 
    /// </summary>
    protected abstract void OnDispose();

    /// <summary>
    /// item 加载资源，更新表现 （进入可视区域时）
    ///
    ///  code example
    ///
    /// _image.sprite = LoadSprite();
    /// button.onClick.RemoveAllListeners(); // 注意：由于UIListItem是复用的，button 一定要清理listener，再绑定新的数据
    /// button.onClick.AddListener({});
    /// 
    /// </summary>
    /// <param name="data"> 用于更新 列表单项表现 的数据类</param>
    protected abstract void OnBindData(D data);

    /// <summary>
    /// 被选中时的表现
    /// </summary>
    /// <param name="selected"></param>
    protected abstract void OnSelected(bool selected);

    /// <summary>
    /// 距离中心的比例
    /// </summary>
    /// <param name="percent">cell的size与距离中心的比例 (cellsize/delta)</param>
    protected virtual void OnDistancePercentFromViewCenter(float percent)
    {
    }
}

public class UIList<T, D> : UIGameObjectWrapper where T : UIListItem<D>, new()
{
    protected object _context;
    protected GameObject _itemTemplate;
    private GameObject _slotTemplate;
    private ScrollRect _scrollRect;
    private RectTransform _contentRectTransform;
    private RectTransform _viewportRectTransform;
    private RectTransform _itemRect;

    private UIListLayout _layout;
    private float _cellSize;
    private float _spacing;
    private float _padding;
    private int _groupCount;

    private List<UIListSlot<D>> _slots = new List<UIListSlot<D>>();

    private Stack<UIListItem<D>> _itemPool = new Stack<UIListItem<D>>(16);
    private List<D> _datas;
    private bool _needUpdate;
    private float _updateInterval = 0.1f;
    private float _currentTime = 0f;
    private int _currentSelect = -1;
    private float _scrollRectSize;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="scrollView"> list从属的 带有scrollview组件的game object</param>
    /// <param name="itemUnderScrollView"> 列表单项在 scrollview下的transform路径 </param>
    /// <param name="layout"> 布局方式：行，列，格</param>
    /// <param name="context"> 如果需要的话， 可以传入上下文，比如一些界面里记的状态， 在UIListItem单项里使用 </param>
    public UIList(GameObject scrollView, string itemUnderScrollView, UIListLayout layout, object context) :
        base(scrollView)
    {
        try
        {
            _context = context;
            _scrollRect = GetItem<ScrollRect>(scrollView);
            _itemTemplate = GetItem($"{itemUnderScrollView}");
            _slotTemplate = _BuildTemplateSlot(_itemTemplate);
            var contentTransform = _itemTemplate.transform.parent;
            _contentRectTransform = contentTransform.GetComponent<RectTransform>();
            _viewportRectTransform = contentTransform.parent.GetComponent<RectTransform>();
            _itemRect = _itemTemplate.GetComponent<RectTransform>();

            _itemTemplate.SetActive(false);
            _scrollRectSize = _scrollRect.GetComponent<RectTransform>().sizeDelta.x;

            _layout = layout;
            switch (_layout)
            {
                case UIListLayout.Vertical:
                    var verticalLayoutGroup = contentTransform.GetComponent<VerticalLayoutGroup>();
                    _cellSize = _itemRect.rect.height;
                    _spacing = verticalLayoutGroup.spacing;
                    _padding = verticalLayoutGroup.padding.top;
                    _groupCount = 1;
                    break;
                case UIListLayout.Horizontal:
                    var horizontalLayoutGroup = contentTransform.GetComponent<HorizontalLayoutGroup>();
                    _cellSize = _itemRect.rect.width;
                    _spacing = horizontalLayoutGroup.spacing;
                    _padding = horizontalLayoutGroup.padding.left;
                    _groupCount = 1;
                    break;
                case UIListLayout.Grid:
                    var gridLayoutGroup = contentTransform.GetComponent<GridLayoutGroup>();
                    _cellSize = gridLayoutGroup.cellSize.y;
                    _spacing = gridLayoutGroup.spacing.y;
                    _padding = gridLayoutGroup.padding.top;
                    if (gridLayoutGroup.constraint == GridLayoutGroup.Constraint.Flexible)
                    {
                        gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                        gridLayoutGroup.constraintCount = Mathf.FloorToInt(
                            (_viewportRectTransform.rect.width - gridLayoutGroup.padding.left -
                                gridLayoutGroup.padding.right + gridLayoutGroup.spacing.x) /
                            (gridLayoutGroup.spacing.x + gridLayoutGroup.cellSize.x));
                    }

                    _groupCount = gridLayoutGroup.constraintCount;
                    break;
            }
        }
        catch (Exception e)
        {
            DragonU3DSDK.DebugUtil.LogError(e);
        }
    }

    public ScrollRect ScrollRect
    {
        get { return _scrollRect; }
    }

    public List<UIListSlot<D>> Slots
    {
        get { return _slots; }
    }

    public float CellSize
    {
        get { return _cellSize; }
    }

    public float Spacing => _spacing;

    /// <summary>
    /// 必须调用：把数据传给列表
    /// </summary>
    /// <param name="datas"></param>
    public void SetDatas(List<D> datas)
    {
        _datas = datas;
        _BuildAllSlots();
        _UpdateContentSize();
    }

    public void AddDataRange(IEnumerable<D> datas)
    {
        _datas?.AddRange(datas);
        _UpdateContentSize();
    }

    public void ClearDatas()
    {
        SetDatas(null);
    }

    public void ApplyData()
    {
        for (int i = 0; i < _slots.Count; i++)
        {
            var item = _slots[i].GetItem();
            if (item != null && i < _GetDataCount())
            {
                item.ReUse(_datas[i]);
            }
        }
    }

    /// <summary>
    /// 必须调用：动态列表必需update
    /// </summary>
    public void Update()
    {
        _currentTime += Time.deltaTime;
        while (_currentTime >= _updateInterval)
        {
            _currentTime -= _updateInterval;
            _needUpdate = true;
        }

        if (_needUpdate)
        {
            _UpdateContent();
        }
    }

    /// <summary>
    /// 必须调用，列表正确地销毁
    /// </summary>
    public void Dispose()
    {
        if (_slotTemplate)
        {
            Object.Destroy(_slotTemplate);
            _slotTemplate = null;
        }

        var count = _GetDataCount();
        if (_slots != null)
        {
            for (int i = 0; i < _slots.Count; i++)
            {
                _RemoveItem(_slots[i]);
                GameObjectFactory.Destroy(_slots[i].GameObject);
            }

            _slots.Clear();
        }

        var e = _itemPool.GetEnumerator();
        while (e.MoveNext())
        {
            e.Current.Release();
        }

        _itemPool.Clear();
        _datas = null;
    }

    public void ScrollTo(int index, float duration = -1)
    {
        var startPos = GetItemPosition(index);
        var newPos = _contentRectTransform.localPosition;

        if (_layout == UIListLayout.Horizontal)
        {
            newPos.x = startPos;
        }
        else
        {
            newPos.y = startPos;
        }

        if (duration > 0)
        {
            _contentRectTransform.DOLocalMove(newPos, duration);
        }
        else
        {
            _contentRectTransform.localPosition = newPos;
        }
    }

    public float GetItemPosition(int index)
    {
        return _padding + Mathf.FloorToInt(index / _groupCount) * (_cellSize + _spacing);
    }

    public void ScrollTo(float position, float duration = -1)
    {
        var newPos = _contentRectTransform.localPosition;
        if (_layout == UIListLayout.Horizontal)
        {
            newPos.x = -position;
        }
        else
        {
            newPos.y = -position;
        }

        if (duration > 0)
        {
            _contentRectTransform.DOLocalMove(newPos, duration);
        }
        else
        {
            _contentRectTransform.localPosition = newPos;
        }
    }


    public float GetScrollPos()
    {
        if (_layout == UIListLayout.Horizontal)
        {
            return -_contentRectTransform.localPosition.x;
        }
        else
        {
            return -_contentRectTransform.localPosition.y;
        }
    }

    public void Select(int index)
    {
        if (_slots != null)
        {
            if (_currentSelect >= 0 && _currentSelect < _slots.Count)
            {
                var item = _slots[_currentSelect].GetItem();
                item?.SetSelected(false);
            }

            _currentSelect = index;
            if (_currentSelect >= 0 && _currentSelect < _slots.Count)
            {
                var item = _slots[_currentSelect].GetItem();
                item?.SetSelected(true);
            }
        }
    }

    public bool GetSelectedData(out D data)
    {
        if (_currentSelect >= 0 && _currentSelect < _datas.Count)
        {
            data = _datas[_currentSelect];
            return true;
        }

        data = default(D);
        return false;
    }

    public void Bind(UnityAction<Vector2> onScrollPosChanged)
    {
        _scrollRect.onValueChanged.AddListener(onScrollPosChanged);
    }

    public float GetViewportSize()
    {
        if (_layout == UIListLayout.Horizontal)
        {
            return _viewportRectTransform.rect.width;
        }
        else
        {
            return _viewportRectTransform.rect.height;
        }
    }

    public bool TryGetItem<T>(int index, out T item) where T : UIListItem<D>
    {
        if (index >= 0 && index < _slots.Count)
        {
            if (_slots[index].HasItem())
            {
                item = _slots[index].GetItem() as T;
                if (item != null)
                {
                    return true;
                }
            }
        }

        item = null;
        return false;
    }


    private GameObject _BuildTemplateSlot(GameObject template)
    {
        var rectTransform = template.GetComponent<RectTransform>();
        var slot = new GameObject("Slot");
        slot.layer = template.layer;
        slot.transform.SetParent(template.transform.parent, false);
        var slotRect = slot.AddComponent<RectTransform>();
        slotRect.pivot = rectTransform.pivot;
        slotRect.anchorMax = rectTransform.anchorMax;
        slotRect.anchorMin = rectTransform.anchorMin;
        slotRect.offsetMax = rectTransform.offsetMax;
        slotRect.offsetMin = rectTransform.offsetMin;
        slotRect.sizeDelta = rectTransform.sizeDelta;
        slotRect.anchoredPosition = rectTransform.anchoredPosition;
        slotRect.anchoredPosition3D = rectTransform.anchoredPosition3D;


        var layoutElement = template.GetComponent<LayoutElement>();
        if (layoutElement)
        {
            var slotLayoutElement = slot.AddComponent<LayoutElement>();
            slotLayoutElement.minWidth = layoutElement.minWidth;
            slotLayoutElement.minHeight = layoutElement.minWidth;
            slotLayoutElement.ignoreLayout = layoutElement.ignoreLayout;
            slotLayoutElement.flexibleWidth = layoutElement.flexibleWidth;
            slotLayoutElement.flexibleHeight = layoutElement.flexibleHeight;
            slotLayoutElement.layoutPriority = layoutElement.layoutPriority;
            slotLayoutElement.preferredWidth = layoutElement.preferredWidth;
            slotLayoutElement.preferredHeight = layoutElement.preferredHeight;
            Object.Destroy(layoutElement);
        }

        slot.SetActive(false);
        return slot;
    }

    private void _BuildAllSlots()
    {
        var count = _GetDataCount();
        if (count > 0)
        {
            if (_slots == null)
            {
                _slots = new List<UIListSlot<D>>(count);
            }

            int i = 0;
            for (; i < count; i++)
            {
                _OpenSlot(i);
            }

            for (; i < _slots.Count; i++)
            {
                _CloseSlot(i);
            }
        }
        else
        {
            if (_slots != null)
            {
                for (int i = 0; i < _slots.Count; i++)
                {
                    _CloseSlot(i);
                }
            }
        }

        _needUpdate = true;
    }

    private void _UpdateContent()
    {
        var count = _GetDataCount();

        int startHide, endHide, startShow, endShow;
        _GetShowSlotRange(out startShow, out endShow, out startHide, out endHide);
        for (int i = 0; i < count; i++)
        {
            var slot = _slots[i];

            if (slot.HasItem())
            {
                if (i < startHide || i > endHide)
                {
                    _RemoveItem(slot);
                }
            }
            else
            {
                if (i >= startShow && i <= endShow)
                {
                    _AddItem(slot, _datas[i], i);
                }
            }

            if (slot.HasItem())
            {
                var delta = 0f;
                if (_layout == UIListLayout.Horizontal)
                {
                    delta = slot.LocalPosition.x - Mathf.Abs(_contentRectTransform.localPosition.x);
                }
                else
                {
                    delta = slot.LocalPosition.y - _contentRectTransform.localPosition.y;
                }

                delta = Mathf.Abs(delta);
                var percent = 1f - delta / _scrollRectSize / 2f;
                slot.GetItem().DistancePercentFromViewCenter(percent);
            }
        }
    }


    private void _UpdateContentSize()
    {
        var size = _padding + (_slots.Count / _groupCount) * (_cellSize + _spacing);
        if (_layout == UIListLayout.Horizontal)
        {
            _contentRectTransform.SetWidth(size);
        }
        else
        {
            _contentRectTransform.SetHeight(size);
        }
    }

    public float GetContentSize()
    {
        if (_layout == UIListLayout.Horizontal)
        {
            return _contentRectTransform.GetWidth();
        }
        else
        {
            return _contentRectTransform.GetHeight();
        }
    }

    private UIListSlot<D> _CreateSlot()
    {
        var go = GameObjectFactory.Clone(_slotTemplate);
        return new UIListSlot<D>(go);
    }

    private int _GetDataCount()
    {
        return _datas?.Count ?? 0;
    }

    private void _CloseSlot(int index)
    {
        var slot = _slots[index];
        _RemoveItem(slot);
        slot.SetActive(false);
    }

    private void _OpenSlot(int index)
    {
        var more = index - (_slots.Count - 1);
        if (more > 0)
        {
            for (int i = 0; i < more; i++)
            {
                var slot = _CreateSlot();
                slot.SetActive(true);
                _slots.Add(slot);
            }
        }
        else
        {
            _RemoveItem(_slots[index]);
            _slots[index].SetActive(true);
        }
    }

    private void _RemoveItem(UIListSlot<D> slot)
    {
        var c = slot.GetItem();
        if (c != null)
        {
            c.Dispose();
            slot.RemoveItem();
            _itemPool.Push(c);
            c.SetParent(GameObjectFactory.GetDefaultRoot());
        }
    }

    private void _AddItem(UIListSlot<D> slot, D data, int index)
    {
        UIListItem<D> c = null;
        if (_itemPool.Count > 0)
        {
            c = _itemPool.Pop();
        }
        else
        {
            c = new T();
            c.Init(GameObjectFactory.Clone(_itemTemplate), _context);
        }

        if (c != null)
        {
            slot.AddItem(c);
            c.SetIndex(index);
            c.SetSelected(index == _currentSelect);
            c.ReUse(data);
        }
    }

    // pos = _padding + math.floor(index / _groupCount) * (_cellSize + _spacing)
    private void _GetShowSlotRange(out int startShowIndex, out int endShowIndex, out int startHideIndex,
        out int endHideIndex)
    {
        var rectStartPos = 0f;
        var rectEndPos = 0f;
        if (_layout == UIListLayout.Horizontal)
        {
            rectStartPos = Mathf.Abs(_contentRectTransform.localPosition.x);
            rectEndPos = rectStartPos + _viewportRectTransform.rect.width;
        }
        else
        {
            rectStartPos = _contentRectTransform.localPosition.y;
            rectEndPos = rectStartPos + _viewportRectTransform.rect.height;
        }

        var startIndex = Mathf.FloorToInt((rectStartPos - _padding) / (_cellSize + _spacing)) * _groupCount;
        var endIndex = Mathf.FloorToInt((rectEndPos - _padding) / (_cellSize + _spacing)) * _groupCount;

        startShowIndex = startIndex - _groupCount;
        endShowIndex = endIndex + _groupCount;
        startHideIndex = startIndex - 2 * _groupCount;
        endHideIndex = endIndex + 2 * _groupCount;
    }
}

public enum UIListLayout
{
    Vertical,
    Horizontal,
    Grid,
}


public class UIListSlot<D> : UIGameObjectWrapper
{
    public UIListSlot(GameObject go) : base(go)
    {
        _slot = go;
    }

    private GameObject _slot;
    private UIListItem<D> _item;

    public void AddItem(UIListItem<D> go)
    {
        if (_item != null)
        {
            DebugUtil.LogError($"{GetType()}.AddContent, already has a content");
            return;
        }

        go.SetParent(_slot.transform);
        go.SetActive(true);
        _item = go;
    }

    public UIListItem<D> RemoveItem()
    {
        var ret = _item;
        _item = null;
        ret?.SetParent(null);
        ret?.SetActive(false);
        return ret;
    }

    public bool HasItem()
    {
        return _item != null;
    }

    public UIListItem<D> GetItem()
    {
        return _item;
    }

    public Vector3 LocalPosition
    {
        get { return _slot.transform.localPosition; }
    }
}