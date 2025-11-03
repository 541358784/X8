using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ScrollRectTable : MonoBehaviour
{
    const int RADIUS = 10;
    const float EPSILON = 0.001f;
    static int[] INCREMENTS = new int[] {1, -1};
    static Vector3 FARAWAY = Vector3.one * 100000;

    public enum Axis
    {
        Horizontal = 0,
        Vertical = 1,
    }

    public enum Corner
    {
        LeftTop = 0,
        LeftBottom = 1,
        RightTop = 2,
        RightBottom = 3,
    }

    public GameObject m_Item;
    public int m_Unit = 1;
    public float m_Elastic = 0.2f;
    public Vector2 m_Spacing;
    public Axis m_Axis;
    public Corner m_Corner;
    public bool m_Center;
    [Range(0, 1)] public float m_Anchor;
    [Range(0, 1)] public float m_Align;
    public bool m_Fixed;
    public Scrollbar scrollbar;

    [HideInInspector] public int maxRenderNumber = 10000;

    public float baseIndex { get; private set; }
    public float startIndex { get; private set; }
    public float endIndex { get; private set; }
    public int index { get; private set; }
    public float scrollbarValue { get; private set; }
    public bool dragging { get; private set; }
    public bool moving { get; private set; }
    public System.Action<int, RectTransform, object> onRender = (a, b, c) => { };
    public System.Action<int, RectTransform, object> onRelease = (a, b, c) => { };
    public System.Action onDragFront = () => { };
    public System.Action onDragBack = () => { };
    public System.Action onEndDragFront = () => { };
    public System.Action onEndDragBack = () => { };

    ScrollRect scrollRect;
    Rect viewRect;
    RectTransform moveTransform;
    Vector3 lastMovePos = Vector3.zero;
    Vector3 beginPos = Vector3.zero;
    Vector3 endPos = Vector3.zero;
    Vector3 basePos = Vector3.zero;
    Vector3 baseOffset = Vector3.zero;
    Vector2 anchor = Vector2.zero;
    Vector2 baseAnchor = Vector2.zero;
    Vector2 focusAnchor = Vector2.zero;
    Vector2 layout = Vector2.zero;
    Vector2 shift = Vector2.zero;
    int maxIndex;
    bool atFront;
    bool atBack;
    bool atBoth;
    bool bFited;
    bool bDragFront;
    bool bDragBack;
    bool bEndDragFront;
    bool bEndDragBack;
    bool bChanged;
    bool bMovable;
    object timer;
    bool renderFlag;

    SimpleEnumerable<object> data = new SimpleEnumerable<object>();
    Dictionary<int, RectTransform> renders = new Dictionary<int, RectTransform>();
    Dictionary<int, int> indexDict = new Dictionary<int, int>();
    Stack<RectTransform> pool = new Stack<RectTransform>();

    int aliveCount = 0;

    void Push(RectTransform render)
    {
        aliveCount++;
        render.localPosition = FARAWAY;
        pool.Push(render);
    }

    RectTransform Pop()
    {
        RectTransform render;
        if (pool.Count > 0)
        {
            render = pool.Pop();
            if (aliveCount > 0)
            {
                aliveCount--;
            }
            else
            {
                render.gameObject.SetActive(true);
            }
        }
        else
        {
            render = (RectTransform) Instantiate(m_Item).transform;
            render.SetParent(moveTransform);
            render.anchorMin = Vector2.up;
            render.anchorMax = Vector2.up;
            render.localPosition = Vector3.zero;
            render.localScale = Vector3.one;
            render.localRotation = Quaternion.identity;
            render.gameObject.SetActive(true);
        }

        return render;
    }

    void Awake()
    {
        moveTransform = (RectTransform) transform;
        moveTransform.anchorMin = Vector2.up;
        moveTransform.anchorMax = Vector2.up;
        scrollRect = transform.GetComponentInParent<ScrollRect>();
        CommonUtils.AddEventTrigger(scrollRect.gameObject, EventTriggerType.BeginDrag, OnBeginDrag);
        CommonUtils.AddEventTrigger(scrollRect.gameObject, EventTriggerType.Drag, OnDrag);
        CommonUtils.AddEventTrigger(scrollRect.gameObject, EventTriggerType.EndDrag, OnEndDrag);
        bChanged = true;

        _Refresh();
    }

    void Start()
    {
        if (bChanged)
        {
            _Refresh();
        }
    }

    void OnTransformParentChanged()
    {
        bChanged = true;
    }

    void OnScreenChanged()
    {
        bChanged = true;
        _Refresh();
        Resize();
    }

    public void SetData(List<object> list)
    {
        Clear();
        data.SetData(list);
        _Refresh();
        Clamp();
    }

    public void SetData(int size)
    {
        Clear();
        data.SetData(size);
        _Refresh();
        Clamp();
    }

    void Shift(int index, float offset)
    {
        RectTransform render;
        if (indexDict.TryGetValue(index, out index) && renders.TryGetValue(index, out render))
        {
            Vector3 renderPos = render.localPosition;
            Rect rect = render.rect;

            Vector3 newPos = Vector3.zero;
            newPos.x = (rect.width + m_Spacing.x) * (2 * offset - 1) * anchor.x - renderPos.x +
                       viewRect.width * (focusAnchor.x + 0.5f);
            newPos.x = lastMovePos.x + (newPos.x - lastMovePos.x) * shift.x * shift.x;
            newPos.y = (rect.height + m_Spacing.y) * (2 * offset - 1) * anchor.y - renderPos.y +
                       viewRect.height * (focusAnchor.y - 0.5f);
            newPos.y = lastMovePos.y + (newPos.y - lastMovePos.y) * shift.y * shift.y;
            moveTransform.localPosition = newPos;
            Render();
        }
    }

    void SetValue(float value)
    {
        if (moving)
        {
            return;
        }

        for (int i = 0; i < 5; i++)
        {
            if (Mathf.Approximately(scrollbarValue, value))
            {
                break;
            }

            float size = endIndex - startIndex;
            float range = maxIndex + m_Unit - EPSILON;
            if (data.Count > 0 && size < range)
            {
                float newStartIndex = (range - size) * ((value - 0.5f) * (shift.x + shift.y) + 0.5f);
                focusAnchor.x = anchor.x;
                focusAnchor.y = anchor.y;
                NearIndex(newStartIndex, startIndex);
                focusAnchor.x = baseAnchor.x;
                focusAnchor.y = baseAnchor.y;
                Render();
                Clamp();
            }
        }
    }

    bool Clamp()
    {
        if (dragging && !moving && !bFited)
        {
            return false;
        }

        bool flag = false;

        if (startIndex == 0 || bFited)
        {
            focusAnchor.x = anchor.x;
            focusAnchor.y = anchor.y;
            NearIndex(0, startIndex);
            flag = true;
        }
        else if (Mathf.Approximately(endIndex, maxIndex + m_Unit - EPSILON))
        {
            focusAnchor.x = -anchor.x;
            focusAnchor.y = -anchor.y;
            NearIndex(maxIndex + m_Unit - EPSILON, endIndex);
            flag = true;
        }

        if (flag)
        {
            focusAnchor.x = baseAnchor.x;
            focusAnchor.y = baseAnchor.y;
            Render();
        }

        return flag;
    }

    public void SetIndex(float index)
    {
        index = Mathf.Clamp(index, 0, maxIndex + m_Unit - EPSILON);
        this.index = (int) (index / m_Unit) * m_Unit;
        float offset = (index - this.index) / m_Unit;
        baseOffset.x = this.index == 0 && m_Anchor == 0 ? 0.1f : -1;
        baseOffset.y = this.index == 0 && m_Anchor == 0 ? 0.1f : -1;

        if (moveTransform == null)
        {
            return;
        }

        Clear();
        Render();
        scrollRect.StopMovement();
        if (!offset.Equals(0))
        {
            Shift(this.index, offset);
        }

        Clamp();
    }

    public void NearIndex(float index)
    {
        NearIndex(index, baseIndex);
        Clamp();
    }

    void NearIndex(float index, float lastIndex, float delta = 0)
    {
        index = Mathf.Clamp(index, 0, maxIndex + m_Unit - EPSILON);
        int curIndex = (int) (index / m_Unit) * m_Unit;
        float offset = (index - curIndex) / m_Unit;

        if (!renders.ContainsKey(curIndex))
        {
            if (Mathf.Abs(this.index - curIndex) > renders.Count + 2 * m_Unit)
            {
                SetIndex(index + delta);
                return;
            }
            else
            {
                if (index > lastIndex)
                {
                    for (int i = this.index; i < curIndex; i += m_Unit)
                    {
                        if (!renders.ContainsKey(i + m_Unit))
                        {
                            Shift(i, 1 + EPSILON);
                        }
                    }
                }
                else
                {
                    for (int i = this.index; i > curIndex; i -= m_Unit)
                    {
                        if (!renders.ContainsKey(i - m_Unit))
                        {
                            Shift(i, -EPSILON);
                        }
                    }
                }
            }
        }

        Shift(curIndex, offset + delta);
    }

    public void Move(Vector2 diatance, float time)
    {
    }

    public void MoveTo(float index, float time)
    {
    }

    public void Stop(bool force = true)
    {
        bMovable = !force;
        scrollRect.StopMovement();
    }

    protected void _Refresh()
    {
        int vertical = (m_Axis == Axis.Vertical) ? 1 : 0;
        int hWeight = (m_Corner == Corner.RightTop || m_Corner == Corner.RightBottom) ? 1 : -1;
        int vWeight = (m_Corner == Corner.LeftTop || m_Corner == Corner.RightTop) ? 1 : -1;

        anchor.x = hWeight * 0.5f;
        anchor.y = vWeight * 0.5f;
        layout.x = vertical * -hWeight;
        layout.y = (1 - vertical) * -vWeight;
        shift.x = (1 - vertical) * -hWeight;
        shift.y = vertical * -vWeight;
        baseAnchor.x = anchor.x + m_Anchor * shift.x;
        baseAnchor.y = anchor.y + m_Anchor * shift.y;
        focusAnchor.x = baseAnchor.x;
        focusAnchor.y = baseAnchor.y;
        m_Unit = Mathf.Max(m_Unit, 1);
        index = index / m_Unit * m_Unit;
        maxIndex = (data.Count - 1) / m_Unit * m_Unit;
        index = Mathf.Max(Mathf.Min(index, maxIndex), 0);
        m_Elastic = Mathf.Clamp(m_Elastic, 0, 0.5f);
        m_Align = Mathf.Clamp(m_Align, 0, 1);
        bFited = false;

        if (moveTransform == null)
        {
            return;
        }

        if (bChanged)
        {
            viewRect = ((RectTransform) transform.parent).rect;
            moveTransform.sizeDelta =
                new Vector2(viewRect.width * (RADIUS * 2 + 1), viewRect.height * (RADIUS * 2 + 1));
            bChanged = false;
        }

        Clear();
        Render();
        scrollRect.StopMovement();

        if (scrollbar)
        {
            scrollbar.onValueChanged.RemoveListener(SetValue);
            scrollbar.onValueChanged.AddListener(SetValue);
        }
    }

    void Render()
    {
        Vector3 movePos = moveTransform.localPosition;
        int orientation = (movePos.x - lastMovePos.x) * shift.x > 0 || (movePos.y - lastMovePos.y) * shift.y > 0
            ? 1
            : -1;

        lastMovePos = movePos;
        Rect area = viewRect;
        area.x = -lastMovePos.x;
        area.y = -lastMovePos.y - area.height;
        Vector2 areaCenter = area.center;

        Vector2 focusPos;
        Vector2 initPos;
        Vector2 areaStart;
        Vector2 areaEnd;
        Vector2 viewStart;
        Vector2 viewEnd;
        Vector2 addition;
        Vector2 startPos;
        Vector2 startOffest;
        Vector2 curPos;
        RectTransform render = null;
        Rect rect;
        Vector3 renderPos = Vector3.zero;
        Vector2 renderArea;
        Vector2 rectStart;
        Vector2 rectEnd;
        Vector3 layoutOffset = Vector3.zero;
        Vector3 layoutSize = Vector3.zero;
        Vector2 maxArea;
        Vector2 offset;
        int lastIndex = index;
        startIndex = maxIndex + 1;
        endIndex = -1;

        focusPos.x = areaCenter.x + area.width * focusAnchor.x;
        focusPos.y = areaCenter.y + area.height * focusAnchor.y;
        if (renders.ContainsKey(index))
        {
            initPos.x = basePos.x;
            initPos.y = basePos.y;
        }
        else
        {
            initPos.x = focusPos.x + baseOffset.x * shift.x;
            initPos.y = focusPos.y + baseOffset.y * shift.y;
        }

        viewStart.x = areaCenter.x + area.width * anchor.x;
        viewStart.y = areaCenter.y + area.height * anchor.y;
        viewEnd.x = areaCenter.x - area.width * anchor.x;
        viewEnd.y = areaCenter.y - area.height * anchor.y;
        for (int k = 0; k < INCREMENTS.Length; k++)
        {
            int increment = INCREMENTS[k] * orientation;
            int step = increment * m_Unit;
            float align = increment > 0 ? m_Align : 1 - m_Align;
            int offsetWeight = increment > 0 ? 1 : 0;
            areaStart.x = areaCenter.x + area.width * anchor.x * increment;
            areaStart.y = areaCenter.y + area.height * anchor.y * increment;
            areaEnd.x = areaCenter.x - area.width * anchor.x * increment;
            areaEnd.y = areaCenter.y - area.height * anchor.y * increment;
            addition.x = shift.x != 0 ? increment : 1;
            addition.y = shift.y != 0 ? increment : 1;
            startPos.x = initPos.x;
            startPos.y = initPos.y;

            for (int i = increment > 0 ? lastIndex : lastIndex + step; i >= 0 && i < data.Count; i += step)
            {
                curPos.x = startPos.x;
                curPos.y = startPos.y;
                layoutSize.x = 0;
                layoutSize.y = 0;
                maxArea.x = 0;
                maxArea.y = 0;
                bool created = false;
                bool finished = true;
                bool removeFront = false;
                bool removeBack = false;

                for (int j = i; j < i + m_Unit && j < data.Count; j++)
                {
                    if (!renders.TryGetValue(j, out render))
                    {
                        if (curPos.x * shift.x * increment > areaEnd.x * shift.x * increment
                            || curPos.y * shift.y * increment > areaEnd.y * shift.y * increment
                            || renders.Count >= maxRenderNumber)
                        {
                            break;
                        }

                        render = Pop();
                        onRender(j, render, data[j]);
                        rect = render.rect;
                        renderArea.x = rect.width + m_Spacing.x;
                        renderArea.y = rect.height + m_Spacing.y;
                        layoutOffset.x = renderArea.x * layout.x;
                        layoutOffset.y = renderArea.y * layout.y;
                        renderPos.x = curPos.x + renderArea.x * -anchor.x * addition.x;
                        renderPos.y = curPos.y + renderArea.y * -anchor.y * addition.y;
                        render.localPosition = renderPos;
                        curPos.x += layoutOffset.x;
                        curPos.y += layoutOffset.y;
                        renders.Add(j, render);
                        created = true;
                    }
                    else
                    {
                        rect = render.rect;
                        renderArea.x = rect.width + m_Spacing.x;
                        renderArea.y = rect.height + m_Spacing.y;
                        layoutOffset.x = renderArea.x * layout.x;
                        layoutOffset.y = renderArea.y * layout.y;
                        renderPos = render.localPosition;
                        finished = false;
                    }

                    layoutSize.x += layoutOffset.x;
                    layoutSize.y += layoutOffset.y;
                    maxArea.x = Mathf.Max(maxArea.x, renderArea.x);
                    maxArea.y = Mathf.Max(maxArea.y, renderArea.y);
                }

                if (render == null)
                {
                    break;
                }

                startOffest.x = maxArea.x * shift.x * increment;
                startOffest.y = maxArea.y * shift.y * increment;
                rectStart.x = startPos.x;
                rectStart.y = startPos.y;
                rectEnd.x = rectStart.x + layoutSize.x + startOffest.x;
                rectEnd.y = rectStart.y + +layoutSize.y + startOffest.y;

                if (rectEnd.x * shift.x * increment < focusPos.x * shift.x * increment
                    || rectEnd.y * shift.y * increment < focusPos.y * shift.y * increment)
                {
                    index = Mathf.Clamp(i + step, 0, maxIndex);
                }
                else if (rectStart.x * shift.x * increment > focusPos.x * shift.x * increment
                         || rectStart.y * shift.y * increment > focusPos.y * shift.y * increment)
                {
                }
                else
                {
                    index = Mathf.Min(index, i);
                }

                if (i == index)
                {
                    basePos.x = startPos.x + (1 - offsetWeight) * startOffest.x;
                    basePos.y = startPos.y + (1 - offsetWeight) * startOffest.y;
                    baseOffset.x = (basePos.x - focusPos.x) * shift.x;
                    baseOffset.y = (basePos.y - focusPos.y) * shift.y;

                    offset.x = -baseOffset.x / maxArea.x;
                    offset.y = -baseOffset.y / maxArea.y;
                    baseIndex = i + Mathf.Clamp(Mathf.Max(offset.x, offset.y) * m_Unit, 0, m_Unit - EPSILON);
                }

                if (rectEnd.x * shift.x * increment < areaStart.x * shift.x * increment
                    || rectEnd.y * shift.y * increment < areaStart.y * shift.y * increment)
                {
                    removeFront = true;
                    finished = false;
                }
                else if (rectStart.x * shift.x * increment > areaEnd.x * shift.x * increment
                         || rectStart.y * shift.y * increment > areaEnd.y * shift.y * increment)
                {
                    removeBack = true;
                }
                else
                {
                    if (rectEnd.x * shift.x * increment < areaEnd.x * shift.x * increment
                        || rectEnd.y * shift.y * increment < areaEnd.y * shift.y * increment)
                    {
                        finished = false;
                    }

                    if (i <= startIndex)
                    {
                        beginPos.x = startPos.x + (1 - offsetWeight) * startOffest.x;
                        beginPos.y = startPos.y + (1 - offsetWeight) * startOffest.y;

                        offset.x = (viewStart.x - beginPos.x) * shift.x / maxArea.x;
                        offset.y = (viewStart.y - beginPos.y) * shift.y / maxArea.y;
                        startIndex = i + Mathf.Clamp(Mathf.Max(offset.x, offset.y) * m_Unit, 0, m_Unit - EPSILON);
                    }

                    if (i >= endIndex)
                    {
                        endPos.x = startPos.x + offsetWeight * startOffest.x;
                        endPos.y = startPos.y + offsetWeight * startOffest.y;

                        offset.x = (endPos.x - viewEnd.x) * shift.x / maxArea.x;
                        offset.y = (endPos.y - viewEnd.y) * shift.y / maxArea.y;
                        endIndex = i + Mathf.Clamp((1 - Mathf.Max(offset.x, offset.y)) * m_Unit, 0, m_Unit - EPSILON);
                    }
                }

                startPos.x += startOffest.x;
                startPos.y += startOffest.y;

                for (int j = i; j < i + m_Unit && j < data.Count; j++)
                {
                    if (renders.TryGetValue(j, out render))
                    {
                        if (removeFront)
                        {
                            indexDict.Remove(i);
                            renders.Remove(j);
                            onRelease(j, render, data[j]);
                            Push(render);
                        }
                        else if (removeBack)
                        {
                            indexDict.Remove(i);
                            renders.Remove(j);
                            onRelease(j, render, data[j]);
                            Push(render);
                        }
                        else if (created)
                        {
                            layoutOffset.x = 0;
                            layoutOffset.y = 0;

                            if (m_Center)
                            {
                                layoutOffset.x += (area.width * layout.x - layoutSize.x) * 0.5f;
                                layoutOffset.y += (area.height * layout.y - layoutSize.y) * 0.5f;
                            }

                            rect = render.rect;
                            renderArea.x = rect.width + m_Spacing.x;
                            renderArea.y = rect.height + m_Spacing.y;
                            layoutOffset.x += (maxArea.x - renderArea.x) * shift.x * increment * align;
                            layoutOffset.y += (maxArea.y - renderArea.y) * shift.y * increment * align;

                            if (layoutOffset.x != 0 || layoutOffset.y != 0)
                            {
                                render.localPosition += layoutOffset;
                            }

                            if ((maxArea.x - renderArea.x) * shift.x == 0 && (maxArea.y - renderArea.y) * shift.y == 0)
                            {
                                indexDict[i] = j;
                            }
                        }
                    }
                }

                if (finished)
                {
                    break;
                }
            }
        }

        atFront = startIndex < m_Unit || renders.ContainsKey(0);
        atBack = startIndex >= maxIndex || renders.ContainsKey(maxIndex);
        atBoth = atFront && atBack;

        if (atBoth)
        {
            float frontOffsetX = (beginPos.x - viewStart.x) * shift.x;
            float frontOffsetY = (beginPos.y - viewStart.y) * shift.y;
            float backOffsetX = (viewEnd.x - endPos.x) * shift.x;
            float backOffsetY = (viewEnd.y - endPos.y) * shift.y;

            if (frontOffsetX + backOffsetX + frontOffsetY + backOffsetY >= 0)
            {
                endPos.x += (frontOffsetX + backOffsetX) * shift.x;
                endPos.y += (frontOffsetY + backOffsetY) * shift.y;
                bFited = true;

                int firstIndex;
                RectTransform firstRender;
                if (indexDict.TryGetValue(0, out firstIndex) && renders.TryGetValue(firstIndex, out firstRender))
                {
                    rect = firstRender.rect;
                    endPos.x -= Mathf.Max(viewRect.width * m_Elastic - rect.width - m_Spacing.x, 0) * shift.x;
                    endPos.y -= Mathf.Max(viewRect.height * m_Elastic - rect.height - m_Spacing.y, 0) * shift.y;
                }
            }

            if (frontOffsetX < 0 || backOffsetX < 0 || frontOffsetY < 0 || backOffsetY < 0)
            {
                atBoth = false;
            }

            if ((frontOffsetX < backOffsetX && -frontOffsetX > backOffsetX) ||
                (frontOffsetY < backOffsetY && -frontOffsetY > backOffsetY))
            {
                atFront = false;
            }
        }

        if (!dragging || moving)
        {
            Resize();
        }

        if (scrollbar)
        {
            float size = endIndex - startIndex;
            float range = maxIndex + m_Unit - EPSILON;
            if (data.Count > 0 && size < range)
            {
                scrollbar.size = size / range;
                scrollbarValue = (startIndex / (range - size) - 0.5f) * (shift.x + shift.y) + 0.5f;
            }
            else
            {
                scrollbar.size = 1;
                scrollbarValue = 1;
            }

            scrollbar.value = scrollbarValue;
        }
    }

    void Resize()
    {
        Vector3 offset = Vector3.zero;
        Rect moveRect = moveTransform.rect;

        if (atBoth || atFront)
        {
            offset.x = beginPos.x + (-0.5f - anchor.x) * moveRect.width;
            offset.y = beginPos.y + (0.5f - anchor.y) * moveRect.height;
        }
        else if (atBack)
        {
            Vector2 addition = new Vector2(shift.x != 0 ? -1 : 1, shift.y != 0 ? -1 : 1);
            offset.x = endPos.x + (-0.5f - anchor.x * addition.x) * moveRect.width;
            offset.y = endPos.y + (0.5f - anchor.y * addition.y) * moveRect.height;
        }
        else
        {
            Vector2 shiftAbs = new Vector2(Mathf.Abs(shift.x), Mathf.Abs(shift.y));
            if (lastMovePos.x * shiftAbs.x < -1.5f * RADIUS * viewRect.width * shiftAbs.x
                || lastMovePos.y * shiftAbs.y > 1.5f * RADIUS * viewRect.height * shiftAbs.y)
            {
                offset.x = (-RADIUS * viewRect.width - lastMovePos.x) * shiftAbs.x;
                offset.y = (RADIUS * viewRect.height - lastMovePos.y) * shiftAbs.y;
            }
            else if (lastMovePos.x * shiftAbs.x > -0.5f * RADIUS * viewRect.width * shiftAbs.x
                     || lastMovePos.y * shiftAbs.y < 0.5f * RADIUS * viewRect.height * shiftAbs.y)
            {
                offset.x = (-RADIUS * viewRect.width - lastMovePos.x) * shiftAbs.x;
                offset.y = (RADIUS * viewRect.height - lastMovePos.y) * shiftAbs.y;
            }
        }

        if (m_Axis == Axis.Horizontal)
        {
            offset.y = (0.5f - anchor.y) * (moveRect.height - viewRect.height) - lastMovePos.y;
        }
        else
        {
            offset.x = (-0.5f - anchor.x) * (moveRect.width - viewRect.width) - lastMovePos.x;
        }

        if (offset.x != 0 || offset.y != 0)
        {
            moveTransform.localPosition += offset;
            lastMovePos.x += offset.x;
            lastMovePos.y += offset.y;
            beginPos.x -= offset.x;
            beginPos.y -= offset.y;
            endPos.x -= offset.x;
            endPos.y -= offset.y;
            basePos.x -= offset.x;
            basePos.y -= offset.y;
            foreach (RectTransform child in renders.Values)
            {
                child.localPosition -= offset;
            }
        }
    }

    public void Clear()
    {
        foreach (int key in renders.Keys)
        {
            RectTransform child = renders[key];
            onRelease(key, child, data[key]);
            Push(child);
        }

        renders.Clear();
        indexDict.Clear();
    }

    void OnBeginDrag(BaseEventData eventData)
    {
        dragging = true;
        bDragFront = false;
        bDragBack = false;
        bEndDragFront = false;
        bEndDragBack = false;
    }

    void OnDrag(BaseEventData eventData)
    {
        dragging = true;
    }

    void OnScroll()
    {
        bEndDragFront = false;
        bEndDragBack = false;

        if (atFront || atBack)
        {
            Vector3 movePos = moveTransform.localPosition;
            Rect area = viewRect;
            area.x = -movePos.x;
            area.y = -movePos.y - area.height;
            Vector2 areaCenter = area.center;

            Vector2 areaStart;
            Vector2 areaEnd;
            Vector2 offset = Vector2.zero;
            areaStart.x = areaCenter.x + area.width * anchor.x;
            areaStart.y = areaCenter.y + area.height * anchor.y;
            areaEnd.x = areaCenter.x + area.width * -anchor.x;
            areaEnd.y = areaCenter.y + area.height * -anchor.y;

            do
            {
                if (atFront)
                {
                    offset.x = (beginPos.x - areaStart.x) * shift.x - viewRect.width * m_Elastic;
                    offset.y = (beginPos.y - areaStart.y) * shift.y - viewRect.height * m_Elastic;
                    if (offset.x > 0 || offset.y > 0)
                    {
                        movePos.x -= offset.x * shift.x;
                        movePos.y -= offset.y * shift.y;
                        if (dragging)
                        {
                            if (!bDragFront)
                            {
                                onDragFront();
                                bDragFront = true;
                            }

                            bEndDragFront = true;
                        }

                        break;
                    }
                }

                if (atBack)
                {
                    offset.x = (areaEnd.x - endPos.x) * shift.x - viewRect.width * m_Elastic;
                    offset.y = (areaEnd.y - endPos.y) * shift.y - viewRect.height * m_Elastic;
                    if (offset.x > 0 || offset.y > 0)
                    {
                        movePos.x += offset.x * shift.x;
                        movePos.y += offset.y * shift.y;
                        if (dragging)
                        {
                            if (!bDragBack)
                            {
                                onDragBack();
                                bDragBack = true;
                            }

                            bEndDragBack = true;
                        }

                        break;
                    }
                }
            } while (false);

            if (renders.Count == 0)
            {
                baseOffset.x = 0;
                baseOffset.y = 0;
            }
            else if (atBoth && m_Fixed)
            {
                moveTransform.localPosition = lastMovePos;
            }
            else if (offset.x > 0 || offset.y > 0)
            {
                moveTransform.localPosition = movePos;
            }
        }
    }

    void OnEndDrag(BaseEventData eventData)
    {
        dragging = false;
        renderFlag = true;

        if (bEndDragFront)
        {
            onEndDragFront();
            bEndDragFront = false;
        }

        if (bEndDragBack)
        {
            onEndDragBack();
            bEndDragBack = false;
        }

        if (bFited)
        {
            scrollRect.StopMovement();
            if (startIndex > 0)
            {
                Clamp();
            }
        }

        Resize();
    }

    void LateUpdate()
    {
        Vector3 delta = moveTransform.localPosition - lastMovePos;
        if (Mathf.Abs(delta.x) > 1 || Mathf.Abs(delta.y) > 1 || renderFlag)
        {
            OnScroll();
            Render();
            renderFlag = false;
        }
        else if (aliveCount > 0)
        {
            foreach (RectTransform render in pool)
            {
                render.gameObject.SetActive(false);
                if (--aliveCount == 0)
                {
                    break;
                }
            }
        }
    }

    void OnDestroy()
    {
        if (scrollbar)
        {
            scrollbar.onValueChanged.RemoveListener(SetValue);
        }
    }
}