using UnityEngine;

public enum AnchorType
{
    BOTTOM = 1,
    RIGHT_TOP = 2,
    LEFT = 3,
    LEFT_TOP = 4,
    MIDDLE = 5,
    RIGHT_BOTTOM = 6,
    RIGHT = 7,
    LEFT_BOTTOM = 8,
    TOP = 9,
}

static public class RectTransformExtension
{
    static public void CopyPositionFrom(this RectTransform to, RectTransform from)
    {
        to.pivot = from.pivot;
        to.anchorMax = from.anchorMax;
        to.anchorMin = from.anchorMin;
        to.offsetMax = from.offsetMax;
        to.offsetMin = from.offsetMin;
        to.sizeDelta = from.sizeDelta;
        to.anchoredPosition = from.anchoredPosition;
        to.anchoredPosition3D = from.anchoredPosition3D;
    }

    /// <summary>
    /// 移到中心原点
    /// </summary>
    /// <param name="rectT"></param>
    static public void MoveOrigin(this RectTransform rectT)
    {
        rectT.SetRectTransformAnchor(AnchorType.MIDDLE);
        rectT.localPosition = Vector2.zero;
        rectT.localScale = Vector2.one;
        rectT.pivot = Vector2.one * 0.5f;
    }

    /// <summary>
    /// 拉伸到占满父空间
    /// </summary>
    /// <param name="t"></param>
    public static void SetFullRect(this RectTransform t)
    {
        t.anchorMin = Vector2.zero;
        t.anchorMax = Vector2.one;
        t.anchoredPosition = Vector2.zero;
        t.sizeDelta = Vector2.zero;
        t.pivot = Vector2.one * 0.5f;
        t.SetLocalPositionZ(0);
        t.localScale = Vector3.one;
    }

    /// <summary>
    /// 设置边距
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="type"></param>
    /// <param name="margin"></param>
    public static void SetMargin(this RectTransform trans, AnchorType type, Vector2 margin)
    {
        if (trans.parent == null)
        {
            return;
        }

        float offsetX = 0;
        float offsetY = 0;
        //left
        if (type == AnchorType.LEFT_BOTTOM || type == AnchorType.LEFT || type == AnchorType.LEFT_TOP)
        {
            offsetX = (trans.rect.width - ((RectTransform) trans.parent).rect.width) / 2 + margin.x;
        }

        //top
        if (type == AnchorType.LEFT_TOP || type == AnchorType.TOP || type == AnchorType.RIGHT_TOP)
        {
            offsetY = (-trans.rect.height + ((RectTransform) trans.parent).rect.height) / 2 - margin.y;
        }

        //right
        if (type == AnchorType.RIGHT_BOTTOM || type == AnchorType.RIGHT || type == AnchorType.RIGHT_TOP)
        {
            offsetX = (-trans.rect.width + ((RectTransform) trans.parent).rect.width) / 2 - margin.x;
        }

        //top
        if (type == AnchorType.LEFT_BOTTOM || type == AnchorType.BOTTOM || type == AnchorType.RIGHT_BOTTOM)
        {
            offsetY = (trans.rect.height - ((RectTransform) trans.parent).rect.height) / 2 + margin.y;
        }

        trans.localPosition = new Vector2(offsetX, offsetY);
    }

    /// <summary>
    /// 设置锚点和大小
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="type"></param>
    /// <param name="margin"></param>
    /// <param name="size"></param>
    public static void SetAnchorWithMarginAndSize(this RectTransform rect, AnchorType type, Vector2 margin,
        Vector2 size)
    {
        switch (type)
        {
            case AnchorType.LEFT_TOP:
                rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, margin.x, size.x);
                rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, margin.y, size.y);
                break;
            case AnchorType.TOP:
                rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, margin.y, size.y);
                break;
            case AnchorType.RIGHT_TOP:
                rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, margin.y, size.y);
                rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, margin.x, size.x);
                break;
            case AnchorType.LEFT:
                rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, margin.x, size.x);
                break;
            case AnchorType.MIDDLE:
                // rect.SetInsetAndSizeFromParentEdge (RectTransform.Edge.Left, margin.x, size.x);
                // rect.SetInsetAndSizeFromParentEdge (RectTransform.Edge.Top, margin.y, size.y);
                // rect.SetInsetAndSizeFromParentEdge (RectTransform.Edge.Right, margin.x, size.x);
                // rect.SetInsetAndSizeFromParentEdge (RectTransform.Edge.Bottom, margin.y, size.y);
                break;
            case AnchorType.RIGHT:
                rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, margin.x, size.x);
                break;
            case AnchorType.LEFT_BOTTOM:
                rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, margin.x, size.x);
                rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, margin.y, size.y);
                break;
            case AnchorType.BOTTOM:
                rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, margin.y, size.y);
                break;
            case AnchorType.RIGHT_BOTTOM:
                rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, margin.x, size.x);
                rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, margin.y, size.y);
                break;
        }
    }

    /// <summary>
    /// 设置锚地，边距，大小
    /// </summary>
    /// <param name="rectT"></param>
    /// <param name="type"></param>
    /// <param name="margin"></param>
    public static void SetAnchorWithMargin(this RectTransform rectT, AnchorType type, Vector2 margin)
    {
        float sizeX = rectT.rect.width;
        float sizeY = rectT.rect.height;
        rectT.SetAnchorWithMarginAndSize(type, margin, new Vector2(sizeX, sizeY));
    }

    /// <summary>
    /// 调整大小
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="size"></param>
    public static void ReSetSize(this RectTransform trans, Vector2 size)
    {
        trans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
        trans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
    }

    /// <summary>
    /// 设置z 轴
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="z"></param>
    public static void SetLocalPositionZ(this Transform trans, float z)
    {
        trans.localPosition = new Vector3(trans.localPosition.x, trans.localPosition.y, z);
    }

    /// <summary>
    /// 设置锚地
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="anchorType"></param>
    public static void SetRectTransformAnchor(this RectTransform rect, AnchorType anchorType)
    {
        switch (anchorType)
        {
            case AnchorType.LEFT_TOP:
                rect.anchorMin = new Vector2(0, 1);
                rect.anchorMax = new Vector2(0, 1);
                break;
            case AnchorType.TOP:
                rect.anchorMin = new Vector2(0.5f, 1);
                rect.anchorMax = new Vector2(0.5f, 1);
                break;
            case AnchorType.RIGHT_TOP:
                rect.anchorMin = new Vector2(1, 1);
                rect.anchorMax = new Vector2(1, 1);
                break;
            case AnchorType.LEFT:
                rect.anchorMin = new Vector2(0, 0.5f);
                rect.anchorMax = new Vector2(0, 0.5f);
                break;
            case AnchorType.MIDDLE:
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                break;
            case AnchorType.RIGHT:
                rect.anchorMin = new Vector2(1, 0.5f);
                rect.anchorMax = new Vector2(1, 0.5f);
                break;
            case AnchorType.LEFT_BOTTOM:
                rect.anchorMin = new Vector2(0, 0);
                rect.anchorMax = new Vector2(0, 0);
                break;
            case AnchorType.BOTTOM:
                rect.anchorMin = new Vector2(0.5f, 0);
                rect.anchorMax = new Vector2(0.5f, 0);
                break;
            case AnchorType.RIGHT_BOTTOM:
                rect.anchorMin = new Vector2(1, 0);
                rect.anchorMax = new Vector2(1, 0);
                break;
        }
    }
}