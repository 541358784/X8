using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public enum PageScrollType
{
    Horizontal,
    Vertical,
}

public class PageScrollView : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    protected ScrollRect rect;
    protected int pageCount; //页面item个数
    private RectTransform content;
    protected List<float> pages = new List<float>(); //存滚动条进度

    public float moveTime = 0.3f;
    private float timer = 0;
    private float startMovePos; //开始位置

    protected int currentPage = 0; //当前页

    //是否在移动
    private bool isMoving = false;

    //是否开启自动滚动
    public bool IsAutoScroll;

    //是否正在拖拽
    private bool isDraging = false;
    public float AutoScrollTime = 3;
    private float AutoScrollTimer = 0;

    public PageScrollType pageScrollType = PageScrollType.Horizontal;

    public Action<int> onPageChange;

    private int autoMoveDir = 1;

    private Vector2 startPosition;

    private void Awake()
    {
        rect = transform.GetComponent<ScrollRect>();
        if (rect == null)
        {
            throw new System.Exception("未查询到scrollRect");
        }

        content = transform.Find("Viewport/Content").GetComponent<RectTransform>();
    }

    private void Start()
    {
        UpdatePageCount();
    }

    // Update is called once per frame
    void Update()
    {
        ListenerMove();
        ListenerAutoScroll();
    }

    public int ChildCount()
    {
        return content.childCount;
    }

    public void UpdatePageCount()
    {
        if (content == null)
            return;

        pageCount = content.childCount;
        for (int i = 0; i < content.childCount; i++)
        {
            Transform child = content.GetChild(i);
            if (child.gameObject.activeSelf)
                continue;

            pageCount = pageCount - 1;
        }

        pages.Clear();
        for (int i = 0; i < pageCount; i++)
        {
            switch (pageScrollType)
            {
                case PageScrollType.Horizontal:
                    pages.Add(i * (1 / (float) (pageCount - 1)));
                    break;
                case PageScrollType.Vertical:
                    pages.Add(1 - i * (1 / (float) (pageCount - 1)));
                    break;
            }
        }
    }

    //监听移动
    public void ListenerMove()
    {
        if (!isMoving)
            return;

        if (pages == null || pages.Count == 0)
            return;

        timer += Time.deltaTime * (1 / moveTime);
        switch (pageScrollType)
        {
            case PageScrollType.Horizontal:
                rect.horizontalNormalizedPosition = Mathf.Lerp(startMovePos, pages[currentPage], timer);
                break;
            case PageScrollType.Vertical:
                rect.verticalNormalizedPosition = Mathf.Lerp(startMovePos, pages[currentPage], timer);
                break;
            default:
                break;
        }

        if (timer >= 1)
            isMoving = false;
    }

    //监听自动滚动
    public void ListenerAutoScroll()
    {
        if (isDraging)
            return;

        if (!IsAutoScroll)
            return;

        AutoScrollTimer += Time.deltaTime;
        if (AutoScrollTimer < AutoScrollTime)
            return;

        AutoScrollTimer = 0;
        currentPage += autoMoveDir;

        currentPage = Math.Min(currentPage, pageCount - 1);
        currentPage = Math.Max(currentPage, 0);
        if (currentPage >= pageCount - 1)
            autoMoveDir = -1;
        else if (currentPage <= 0)
            autoMoveDir = 1;

        ScrollPage(currentPage);
    }

    //计算出离得最近的一页
    public int CaculateMinDistancePage()
    {
        int minPage = 0;
        for (int i = 1; i < pages.Count; i++)
        {
            float curPage = 0;
            switch (pageScrollType)
            {
                case PageScrollType.Horizontal:
                    curPage = rect.horizontalNormalizedPosition;
                    break;
                case PageScrollType.Vertical:
                    curPage = rect.verticalNormalizedPosition;
                    break;
            }

            if (Mathf.Abs(pages[i] - curPage) < Mathf.Abs(pages[minPage] - curPage))
            {
                minPage = i;
            }
        }

        return minPage;
    }

    //计算左右的某一页
    public int CaculateMinDistancePage(bool isLeft)
    {
        int page = currentPage;

        if (isLeft)
            page = page + 1 >= pages.Count ? pages.Count - 1 : page + 1;
        else
            page = page - 1 < 0 ? 0 : page - 1;

        return page;
    }

    //滚到哪一页
    private void ScrollPage(int page)
    {
        isMoving = true;
        this.currentPage = page;
        timer = 0;
        switch (pageScrollType)
        {
            case PageScrollType.Horizontal:
                startMovePos = rect.horizontalNormalizedPosition;
                break;
            case PageScrollType.Vertical:
                startMovePos = rect.verticalNormalizedPosition;
                break;
        }

        onPageChange?.Invoke(this.currentPage);
    }

    public void ScrollPageImmediately(int page)
    {
        if (page < 0 || page >= pages.Count)
        {
            currentPage = 0;
        }
        else
            currentPage = page;

        if (pages.Count <= page)
            return;

        switch (pageScrollType)
        {
            case PageScrollType.Horizontal:
                rect.horizontalNormalizedPosition = pages[currentPage];
                break;
            case PageScrollType.Vertical:
                rect.verticalNormalizedPosition = pages[currentPage];
                break;
            default:
                break;
        }

        ScrollPage(page);
    }

    //结束拖拽滑动
    public void OnEndDrag(PointerEventData eventData)
    {
        this.ScrollPage(CaculateMinDistancePage((eventData.position.x - startPosition.x) < 0));
        isDraging = false;
        AutoScrollTimer = 0
            ;
    }

    //开始拖拽滑动
    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = eventData.position;
        isDraging = true;
    }
}