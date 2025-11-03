/****************************************************
    文件：UITweenHelper.cs
    作者：Cys
    邮箱: yongsheng.chen@dragonplus.com
    日期： #CreateTime#
    功能：....
*****************************************************/

using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;

public static class UITweenHelper
{
    #region Style1

    public static void PopupOpen(Transform bg, Action onComplete = null)
    {
        PopupOpen(bg, null, 0.6f, onComplete);
    }

    public static void PopupOpen(Transform bg, Graphic bgMask, float bgMaskAlpha, Action onComplete = null)
    {
        Sequence seq = DOTween.Sequence();

        bg.localScale = Vector3.one * 0.7f;
        seq.Insert(0f, bg.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack));

        CanvasGroup canvasGroup = bg.parent.GetComponentDefault<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            seq.Insert(0f, DOTween.To(() => 0f, x => { canvasGroup.alpha = x; }, 1f, 0.21f).SetEase(Ease.OutCubic));
        }

        // if (bgMask != null)
        // {
        //     bgMask.SetAlpha(0f);
        //     seq.Insert(0f, DOTween.To(() => 0f, x => { bgMask.SetAlpha(x); }, bgMaskAlpha, 0.21f).SetEase(Ease.OutCubic));
        // }

        seq.SetUpdate(true);
        seq.OnComplete(() =>
        {
            if (onComplete != null)
            {
                onComplete();
            }
        });
    }

    public static void PopupClose(Transform bg, Action onComplete = null)
    {
        PopupClose(bg, null, onComplete);
    }

    public static void PopupClose(Transform bg, Graphic bgMask, Action onComplete = null)
    {
        Sequence seq = DOTween.Sequence();
        seq.Insert(0f, bg.DOScale(Vector3.one * 0.5f, 0.25f).SetEase(Ease.InBack));

        CanvasGroup canvasGroup = bg.parent.GetComponentDefault<CanvasGroup>();
        if (canvasGroup != null)
        {
            float startAlpha = canvasGroup.alpha;
            seq.Insert(0.08f,
                DOTween.To(() => startAlpha, x => { canvasGroup.alpha = x; }, 0f, 0.17f).SetEase(Ease.InCubic));
        }

        // if (bgMask != null)
        // {
        //     float startAlpha = bgMask.color.a;
        //     seq.Insert(0.08f, DOTween.To(() => startAlpha, x => { bgMask.SetAlpha(x); }, 0f, 0.17f).SetEase(Ease.InCubic));
        // }

        seq.SetUpdate(true);
        seq.OnComplete(() =>
        {
            bg.localScale = Vector3.one;
            if (onComplete != null)
            {
                onComplete();
            }
        });
    }

    #endregion


    //放大进入，缩小退出

    #region Style2

    public static void PopupOpen2(Transform bg, Action onComplete = null)
    {
        PopupOpen2(bg, null, onComplete);
    }

    public static void PopupOpen2(Transform bg, Graphic bgMask, Action onComplete = null)
    {
        bg.gameObject.SetActive(true);
        Vector3 targetScale = bg.localScale;
        Vector3 __targetScale = targetScale;
        __targetScale.x += 0.1f;
        __targetScale.y += 0.1f;

        float stageTime1 = 0.25f;
        float stageTime2 = 0.05f;

        Sequence seq = DOTween.Sequence();

        seq.Insert(0f, DOTween.To(() => Vector3.forward, v3 => { bg.localScale = v3; }, __targetScale, stageTime1));

        seq.Insert(stageTime1, DOTween.To(() => __targetScale, v3 => { bg.localScale = v3; }, targetScale, stageTime2));

        // if (bgMask != null)
        // {
        //     bgMask.(0f);
        //     seq.Insert(0f, DOTween.To(() => 0f, x => { bgMask.SetAlpha(x); }, 0.6f, stageTime1 + stageTime2));
        // }

        seq.SetUpdate(true);
        seq.OnComplete(() =>
        {
            if (onComplete != null)
            {
                onComplete();
            }
        });
    }

    public static void PopupClose2(Transform bg, Action onComplete = null)
    {
        PopupClose2(bg, null, onComplete);
    }

    public static void PopupClose2(Transform bg, Graphic bgMask, Action onComplete = null)
    {
        Vector3 originScale = bg.localScale;
        Vector3 __originScale = originScale;
        __originScale.x += 0.1f;
        __originScale.y += 0.1f;

        float stageTime1 = 0.05f;
        float stageTime2 = 0.25f;

        Sequence seq = DOTween.Sequence();

        seq.Insert(0f, DOTween.To(() => originScale, v3 => { bg.localScale = v3; }, __originScale, stageTime1));

        seq.Insert(stageTime1,
            DOTween.To(() => __originScale, v3 => { bg.localScale = v3; }, Vector3.forward, stageTime2));

        // if (bgMask != null)
        // {
        //     float startAlpha = bgMask.color.a;
        //     seq.Insert(0.08f, DOTween.To(() => startAlpha, x => { bgMask.SetAlpha(x); }, 0f, stageTime1 + stageTime2 - 0.08f));
        // }

        seq.SetUpdate(true);
        seq.OnComplete(() =>
        {
            bg.gameObject.SetActive(false);
            bg.localScale = originScale;
            if (onComplete != null)
            {
                onComplete();
            }
        });
    }

    #endregion

    //屏幕右侧以外移动到屏幕中央
    public static void ScreenRightToCenter(Transform transform, Action onFinish)
    {
        transform.SetLocalPositionX(Screen.width);
        transform.DOLocalMoveX(0, 0.3f).OnComplete(() =>
        {
            if (onFinish != null)
            {
                onFinish();
            }
        });
    }

    //屏幕中央移动到屏幕右侧以外
    public static void ScreenCenterToRight(Transform transform, Action onFinish)
    {
        transform.SetLocalPositionX(0);
        transform.DOLocalMoveX(Screen.width, 0.3f).OnComplete(() =>
        {
            if (onFinish != null)
            {
                onFinish();
            }
        });
    }

    ////获取y = kx2 + b;
    //private static Vector2 GetEquationPara(Vector2 para1, Vector2 para2)
    //{
    //    Vector2 v2 = Vector2.one;
    //    float dis = para1.x - para2.x;
    //    if(dis > -0.0000001 && dis < 0.0000001 )
    //    {
    //        return v2;
    //    }
    //    v2.x = (para2.y - para1.y) / (para2.x * para2.x - para1.x * para1.x);
    //    v2.y = para1.y - v2.x * para1.x * para1.x;
    //    return v2;
    //}

    public static void DoLocalPathToTargetPos(Transform transform, Vector3 targetPos, Ease ease, Action onFinish = null)
    {
        Vector3 originPos = transform.localPosition;
        //float x = (transform.localPosition.x + targetPos.x) / 2;
        //float y = 0 - Mathf.Sqrt(Mathf.Abs(x));

        Vector3[] path = new Vector3[5];
        path[0] = transform.localPosition;
        path[path.Length - 1] = targetPos;

        Vector3 v3 = transform.localPosition;

        for (int i = 1; i < path.Length - 1; ++i)
        {
            v3.x = Mathf.Lerp(transform.localPosition.x, targetPos.x, (float) i / path.Length);
            v3.y = Mathf.Lerp(transform.localPosition.y, targetPos.y, (float) i / path.Length / 2f);
            path[i] = v3;
            //Debug.Log("path[" + i + "] = " + path[i]);
        }

        Sequence seq = DOTween.Sequence();
        Vector3 originScale = transform.localScale;
        transform.localScale *= 0.5f;

        Image image = transform.GetComponent<Image>();

        seq.Insert(0, transform.DOScale(originScale, 0.2f));
        seq.Insert(0.2f, transform.DOLocalPath(path, 0.8f).SetEase(ease));
        seq.Insert(0.5f, transform.DOScale(originScale * 0.8f, 0.3f));
        seq.Insert(0.5f, image.DOFade(0.6f, 0.3f));

        seq.OnComplete(() =>
        {
            if (onFinish != null)
            {
                onFinish();
            }
        });
    }

    public static void DoLocalPathToTargetPos1(Transform transform, Vector3 targetPos, Ease ease,
        Action onFinish = null)
    {
        Vector3 originPos = transform.localPosition;
        //float x = (transform.localPosition.x + targetPos.x) / 2;
        //float y = 0 - Mathf.Sqrt(Mathf.Abs(x));

        Vector3[] path = new Vector3[5];
        path[0] = transform.localPosition;
        path[path.Length - 1] = targetPos;

        Vector2 point = new Vector2(targetPos.x, originPos.y);
        //Debug.Log("point" + point);
        float r = Mathf.Abs(targetPos.x - originPos.x);

        Vector3 v3 = transform.localPosition;
        float offsetY = 10f;

        for (int i = 1; i < path.Length - 1; ++i)
        {
            float percent = (float) i / (float) path.Length;
            float angel = (180f + (percent * 90f)) / 360f * 2 * 3.14f;
            float x = Mathf.Cos(angel);
            float y = Mathf.Sin(angel);
            v3.x = x * r + point.x;
            v3.y = y * r + point.y;
            v3.y -= i * offsetY;
            //v3.x = Mathf.Lerp(transform.localPosition.x, targetPos.x, percent);
            //v3.y = Mathf.Lerp(transform.localPosition.y, targetPos.y, percent);
            path[i] = v3;
            //Debug.Log("path[" + i + "] = " + path[i]);
        }

        Sequence seq = DOTween.Sequence();
        Vector3 originScale = transform.localScale;
        transform.localScale *= 0.5f;

        Image image = transform.GetComponent<Image>();

        seq.Insert(0, transform.DOScale(originScale, 0.2f));
        seq.Insert(0.2f, transform.DOLocalPath(path, 0.8f).SetEase(ease));
        seq.Insert(0.5f, transform.DOScale(originScale * 0.8f, 0.3f));
        seq.Insert(0.5f, image.DOFade(0.6f, 0.3f));

        seq.OnComplete(() =>
        {
            if (onFinish != null)
            {
                onFinish();
            }
        });
    }

    public static void DoLocalPathToTargetPos2(Transform transform, Vector3 targetPos, Ease ease,
        Action onFinish = null)
    {
        Vector3 originPos = transform.localPosition;
        //float x = (transform.localPosition.x + targetPos.x) / 2;
        //float y = 0 - Mathf.Sqrt(Mathf.Abs(x));

        Vector3[] path = new Vector3[5];
        path[0] = transform.localPosition;
        path[path.Length - 1] = targetPos;

        Vector2 point = new Vector2(targetPos.x, originPos.y);
        //Debug.Log("point" + point);
        float r = Mathf.Abs(targetPos.x - originPos.x);

        Vector3 v3 = transform.localPosition;
        float offsetY = 10f;

        for (int i = 1; i < path.Length - 1; ++i)
        {
            float percent = (float) i / (float) path.Length;
            float angel = (360f - (percent * 90f)) / 360f * 2 * 3.14f;
            float x = Mathf.Cos(angel);
            float y = Mathf.Sin(angel);
            v3.x = x * r + point.x;
            v3.y = y * r + point.y;
            v3.y -= i * offsetY;
            //v3.x = Mathf.Lerp(transform.localPosition.x, targetPos.x, percent);
            //v3.y = Mathf.Lerp(transform.localPosition.y, targetPos.y, percent);
            path[i] = v3;
            //Debug.Log("path[" + i + "] = " + path[i]);
        }

        Sequence seq = DOTween.Sequence();
        Vector3 originScale = transform.localScale;
        transform.localScale *= 0.5f;

        Image image = transform.GetComponent<Image>();

        seq.Insert(0, transform.DOScale(originScale, 0.2f));
        seq.Insert(0.2f, transform.DOLocalPath(path, 0.8f).SetEase(ease));
        seq.Insert(0.5f, transform.DOScale(originScale * 0.8f, 0.3f));
        seq.Insert(0.5f, image.DOFade(0.6f, 0.3f));

        seq.OnComplete(() =>
        {
            if (onFinish != null)
            {
                onFinish();
            }
        });
    }


    public static void DoScaleEffect(Transform transform, float scale, Action onFinish = null)
    {
        Sequence seq = DOTween.Sequence();
        Vector3 originScale = transform.localScale;
        Vector3 targetScale = originScale * scale;
        seq.Insert(0, transform.DOScale(targetScale, 0.2f));
        seq.Insert(0.2f, transform.DOScale(originScale, 0.2f));

        seq.OnComplete(() =>
        {
            if (onFinish != null)
            {
                onFinish();
            }
        });
    }


    public static void SetPositionX(this Transform t, float x)
    {
        if (t != null)
        {
            Vector3 pos = t.position;
            pos.x = x;
            t.position = pos;
        }
    }

    public static void SetPositionY(this Transform t, float y)
    {
        if (t != null)
        {
            Vector3 pos = t.position;
            pos.y = y;
            t.position = pos;
        }
    }

    public static void SetPositionZ(this Transform t, float z)
    {
        if (t != null)
        {
            Vector3 pos = t.position;
            pos.z = z;
            t.position = pos;
        }
    }

    public static void SetLocalPositionX(this Transform t, float x)
    {
        if (t != null)
        {
            Vector3 localPos = t.localPosition;
            localPos.x = x;
            t.localPosition = localPos;
        }
    }

    public static void SetLocalPositionY(this Transform t, float y)
    {
        if (t != null)
        {
            Vector3 localPos = t.localPosition;
            localPos.y = y;
            t.localPosition = localPos;
        }
    }
    
    public static void SetAnchorPositionY(this RectTransform t, float y)
    {
        if (t != null)
        {
            Vector3 localPos = t.anchoredPosition;
            localPos.y = y;
            t.anchoredPosition = localPos;
        }
    }
    public static void SetAnchorPositionX(this RectTransform t, float x)
    {
        if (t != null)
        {
            Vector3 localPos = t.anchoredPosition;
            localPos.x = x;
            t.anchoredPosition = localPos;
        }
    }
}