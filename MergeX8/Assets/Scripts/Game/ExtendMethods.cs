/*******************************************************************************
 * 
 * Extend Utility Methods
 * 
 * author: guo.cheng <guo@dragonplus.com>
 * 
 ******************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 一些扩展的工具函数
/// </summary>
public static class ExtendMethods
{
    #region Components

    /// <summary>
    /// 获取指定子节点上的指定组件<para>
    /// 如果子节点不存在或者节点上没有该组件 返回 null</para>
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    /// <param name="transform">根节点</param>
    /// <param name="path">子节点相对根结点的路径</param>
    public static T GetComponent<T>(this Transform transform, string path) where T : Component
    {
        if (!transform) return null;
        return _GetComponent<T>(transform, path);
    }

    /// <summary>
    /// 获取指定子节点上的指定组件<para>
    /// 如果子节点不存在或者节点上没有该组件 返回 null</para>
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    /// <param name="gameObject">根节点</param>
    /// <param name="path">子节点相对根结点的路径</param>
    public static T GetComponent<T>(this GameObject gameObject, string path) where T : Component
    {
        if (!gameObject) return null;
        return _GetComponent<T>(gameObject.transform, path);
    }

    /// <summary>
    /// 获取指定子节点上的指定组件<para>
    /// 如果子节点不存在或者节点上没有该组件 返回 null</para>
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    /// <param name="component">根节点</param>
    /// <param name="path">子节点相对根结点的路径</param>
    public static T GetComponent<T>(this Component component, string path) where T : Component
    {
        if (!component) return null;
        return _GetComponent<T>(component.transform, path);
    }

    /// <summary>
    /// 获取指定子节点上的指定组件 特别当 path 为空的时候子节点即根结点<para>
    /// 如果子节点不存在 返回 null</para>
    /// 如果子节点不存在该组件 则添加该组件
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    /// <param name="transform">根节点</param>
    /// <param name="path">子节点相对根结点的路径</param>
    public static T GetComponentDefault<T>(this Transform transform, string path = null) where T : Component
    {
        if (!transform) return null;
        return _GetComponentDefault<T>(transform, path);
    }

    /// <summary>
    /// 获取指定子节点上的指定组件 特别当 path 为空的时候子节点即根结点<para>
    /// 如果子节点不存在 返回 null</para>
    /// 如果子节点不存在该组件 则添加该组件
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    /// <param name="gameObject">根节点</param>
    /// <param name="path">子节点相对根结点的路径</param>
    public static T GetComponentDefault<T>(this GameObject gameObject, string path = null) where T : Component
    {
        if (!gameObject) return null;
        return _GetComponentDefault<T>(gameObject.transform, path);
    }

    /// <summary>
    /// 获取指定子节点上的指定组件 特别当 path 为空的时候子节点即根结点<para>
    /// 如果子节点不存在 返回 null</para>
    /// 如果子节点不存在该组件 则添加该组件
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    /// <param name="component">根节点</param>
    /// <param name="path">子节点相对根结点的路径</param>
    public static T GetComponentDefault<T>(this Component component, string path = null) where T : Component
    {
        if (!component) return null;
        return _GetComponentDefault<T>(component.transform, path);
    }

    private static T _GetComponent<T>(this Transform transform, string path) where T : Component
    {
        var node = transform.Find(path);
        return node ? node.GetComponent<T>() : null;
    }

    private static T _GetComponentDefault<T>(this Transform transform, string path = null) where T : Component
    {
        var node = string.IsNullOrEmpty(path) ? transform : transform.Find(path);
        if (!node) return null;
        var comp = node.GetComponent<T>();
        return comp ? comp : node.gameObject.AddComponent<T>();
    }

    /// <summary>
    /// 设置 component 对象的 active 状态<br/>
    /// 设置前会先检查 component
    /// </summary>
    /// <param name="component"></param>
    /// <param name="active"></param>
    public static void SafeActive(this Component component, bool active)
    {
        if (!component) return;
        component.gameObject.SetActive(active);
    }

    /// <summary>
    /// 设置 gameobject 对象的 active 状态<br/>
    /// 设置前会先检查 o
    /// </summary>
    /// <param name="o"></param>
    /// <param name="active"></param>
    public static void SafeActive(this GameObject o, bool active)
    {
        if (!o) return;
        o.SetActive(active);
    }

    #endregion

    #region Localized & Text

    /// <summary>
    /// 设置多语言文本到目标
    /// </summary>
    /// <param name="lt">多语言组件</param>
    /// <param name="key">多语言 key</param>
    public static void SetString(this LocalizeTextMeshProUGUI lt, string key)
    {
        if (!lt) return;
        lt.SetTerm(string.Empty);
        lt.SetText(LocalizationManager.Instance.GetLocalizedString(key));
    }

    /// <summary>
    /// 设置多语言文本到目标
    /// </summary>
    /// <param name="lt">多语言组件</param>
    /// <param name="key">多语言 key</param>
    /// <param name="args">参数列表</param>
    public static void SetString(this LocalizeTextMeshProUGUI lt, string key, params object[] args)
    {
        if (!lt) return;
        lt.SetTerm(string.Empty);
        lt.SetText(string.IsNullOrEmpty(key)
            ? string.Empty
            : LocalizationManager.Instance.GetLocalizedStringWithFormats(key, args));
    }

    /// <summary>
    /// 设置参数到目标
    /// </summary>
    /// <param name="t">组件</param>
    /// <param name="format">格式文本</param>
    /// <param name="args">参数列表</param>
    public static void SetFormat(this LocalizeTextMeshProUGUI t, string format, params object[] args)
    {
        if (!t) return;
        t.SetTerm(string.Empty);
        t.SetText(string.IsNullOrEmpty(format) ? string.Empty : args.Length < 1 ? format : string.Format(format, args));
    }

    /// <summary>
    /// 设置参数到目标
    /// </summary>
    /// <param name="t">组件</param>
    /// <param name="format">格式文本</param>
    /// <param name="args">参数列表</param>
    public static void SetFormat(this Text t, string format, params object[] args)
    {
        if (!t) return;
        t.text = string.IsNullOrEmpty(format) ? string.Empty : args.Length < 1 ? format : string.Format(format, args);
    }

    #endregion

    #region Animator & Animation

    /// <summary>
    /// 播放指定状态<br/>
    /// 注意，如果 animator 所在对象不可见，将强制设置 active<br/>
    /// 注意，如果动画在播放过程中被意外中断，比如目标对象销毁或隐藏，onComplete 可能不会被调用<br/>
    /// See <see cref="DG.Tweening.DOVirtual.DelayedCall(float, DG.Tweening.TweenCallback, bool)"/><br/>
    /// 如果状态不存在，返回 false
    /// </summary>
    /// <param name="animator">Animator</param>
    /// <param name="state">状态 hash</param>
    /// <param name="onComplete"></param>
    /// <returns></returns>
    public static bool Play(this Animator animator, string state, Action<Animator> onComplete)
    {
        return animator && animator.Play(state, 0, 0, onComplete);
    }

    public static void PlayAnim(this Animator animator, string name)
    {
        int state = Animator.StringToHash(name);
        if (!animator || !animator.enabled || !animator.gameObject.activeSelf || !animator.runtimeAnimatorController || state == 0 || !animator.HasState(0, state))
            return;
        
        animator.Play(state);
    }
    
    /// <summary>
    /// 播放指定状态 通过 <see cref="Animator.StringToHash(string)"/> 获取 state<br/>
    /// 注意，如果 animator 所在对象不可见，将强制设置 active<br/>
    /// 注意，如果动画在播放过程中被意外中断，比如目标对象销毁或隐藏，onComplete 可能不会被调用<br/>
    /// See <see cref="DG.Tweening.DOVirtual.DelayedCall(float, DG.Tweening.TweenCallback, bool)"/><br/>
    /// 如果状态不存在，返回 false
    /// </summary>
    /// <param name="animator">Animator</param>
    /// <param name="state">状态 hash</param>
    /// <param name="onComplete"></param>
    /// <returns></returns>
    public static bool Play(this Animator animator, int state, Action<Animator> onComplete)
    {
        return animator && animator.Play(state, 0, 0, onComplete);
    }

    /// <summary>
    /// 播放指定状态<br/>
    /// 注意，如果 animator 所在对象不可见，将强制设置 active<br/>
    /// 注意，如果动画在播放过程中被意外中断，比如目标对象销毁或隐藏，onComplete 可能不会被调用<br/>
    /// See <see cref="DG.Tweening.DOVirtual.DelayedCall(float, DG.Tweening.TweenCallback, bool)"/><br/>
    /// 如果状态不存在，返回 false
    /// </summary>
    /// <param name="animator">Animator</param>
    /// <param name="state">状态 hash</param>
    /// <param name="layer">状态所在 layer</param>
    /// <param name="normalized">播放起始位置</param>
    /// <param name="onComplete"></param>
    /// <returns></returns>
    public static bool Play(this Animator animator, string state, int layer, float normalized,
        Action<Animator> onComplete)
    {
        return animator && animator.Play(Animator.StringToHash(state), layer, normalized, onComplete);
    }

    /// <summary>
    /// 播放指定状态 通过 <see cref="Animator.StringToHash(string)"/> 获取 state<br/>
    /// 注意，如果 animator 所在对象不可见，将强制设置 active<br/>
    /// 注意，如果动画在播放过程中被意外中断，比如目标对象销毁或隐藏，onComplete 可能不会被调用<br/>
    /// See <see cref="DG.Tweening.DOVirtual.DelayedCall(float, DG.Tweening.TweenCallback, bool)"/><br/>
    /// 如果状态不存在，返回 false
    /// </summary>
    /// <param name="animator">Animator</param>
    /// <param name="state">状态 hash</param>
    /// <param name="layer">状态所在 layer</param>
    /// <param name="normalized">播放起始位置</param>
    /// <param name="onComplete"></param>
    /// <returns></returns>
    public static bool Play(this Animator animator, int state, int layer, float normalized, Action<Animator> onComplete)
    {
        if (!animator || !animator.runtimeAnimatorController || state == 0 || !animator.HasState(0, state))
            return false;
        var o = animator.gameObject;
        if (!o.activeSelf) o.SetActive(true);

        animator.enabled = true;
        animator.Play(state, layer, normalized);
        animator.Update(0);

        if (onComplete != null)
        {
            var length = animator.GetCurrentAnimatorStateInfo(0).length;
            var duration = (1 - normalized) * length;
            if (duration <= 0)
            {
                onComplete?.Invoke(animator);
                return true;
            }

            DG.Tweening.DOVirtual.DelayedCall(duration < 0 ? 0 : duration > 0.01f ? duration - 0.01f : duration, () =>
            {
                if (!animator || !animator.runtimeAnimatorController || !animator.enabled ||
                    !animator.gameObject.activeSelf) return; // 目标如果被隐藏或者销毁 相当于动画中断 不调用 onComplete
                if (animator.GetCurrentAnimatorStateInfo(layer).shortNameHash == state)
                    onComplete.Invoke(animator); // 结束动画和播放动画不一致 不调用 onComplete
            }, animator.updateMode == AnimatorUpdateMode.UnscaledTime);
        }

        return true;
    }

    /// <summary>
    /// 播放指定动画<br/>
    /// 注意，如果 animation 所在对象不可见，将强制设置 active<br/>
    /// 注意，如果动画在播放过程中被意外中断，比如目标对象销毁或隐藏，onComplete 可能不会被调用<br/>
    /// See <see cref="DG.Tweening.DOVirtual.DelayedCall(float, DG.Tweening.TweenCallback, bool)"/><br/>
    /// 如果动画化剪辑不存在，返回 false
    /// </summary>
    /// <returns></returns>
    public static bool Play(this Animation animation, Action onComplete)
    {
        if (!animation || !animation.clip) return false;
        var o = animation.gameObject;
        if (!o.activeSelf) o.SetActive(true);

        animation.enabled = true;
        animation.Stop();
        animation.Play();
        animation.Sample();

        if (onComplete != null && animation.clip.length <= 0)
        {
            onComplete.Invoke();
            onComplete = null;
        }

        UIRoot.Instance.StartCoroutine(WaitPlay(animation, () =>
        {
            var enabled = false;
            if (animation)
            {
                animation.Stop();
                enabled = animation.enabled;
                animation.enabled = false;
            }

            if (!enabled || !animation || !animation.clip || !animation.gameObject.activeSelf)
                return; // 目标如果被隐藏或者销毁 相当于动画中断 不调用 onComplete
            onComplete?.Invoke();
        }));

        return true;
    }

    private static IEnumerator WaitPlay(Animation animation, Action action)
    {
        var wait = new WaitUntil(() => !animation || !animation.isPlaying);
        yield return wait;
        action?.Invoke();
    }

    #endregion

    #region Containners

    /// <summary>
    /// 乱序列表 并返回<br/>
    /// 当指定 clone 为 true 时，将返回一个新的列表 而不改变自身
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list">要乱序的源列表</param>
    /// <param name="clone">是否创建新的副本</param>
    /// <returns></returns>
    public static List<T> Shuffle<T>(this List<T> list, bool clone = false)
    {
        if (list == null) return null;
        var l = clone ? new List<T>(list) : list;
        for (var i = 0; i < l.Count; ++i)
        {
            var v = l[i];
            var r = UnityEngine.Random.Range(i, l.Count);
            l[i] = l[r];
            l[r] = v;
        }

        return l;
    }

    /// <summary>
    /// 乱序列表 并返回<br/>
    /// 当指定 clone 为 true 时，将返回一个新的列表 而不改变自身
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list">要乱序的源列表</param>
    /// <param name="clone">是否创建新的副本</param>
    /// <returns></returns>
    public static T[] Shuffle<T>(this T[] arr, bool clone = false)
    {
        if (arr == null) return null;
        var l = arr;
        if (clone)
        {
            l = new T[arr.Length];
            Array.Copy(arr, l, arr.Length);
        }

        for (var i = 0; i < l.Length; ++i)
        {
            var v = l[i];
            var r = UnityEngine.Random.Range(i, l.Length);
            l[i] = l[r];
            l[r] = v;
        }

        return l;
    }

    #endregion

    public static int GetRandomWithPower(this int[] powers)
    {
        var sum = 0;
        foreach (var power in powers)
        {
            sum += power;
        }

        UnityEngine.Random.InitState((int) System.DateTime.Now.Ticks);
        var randomNum = UnityEngine.Random.Range(0, sum);
        var currentSum = 0;
        for (var i = 0; i < powers.Length; i++)
        {
            var nextSum = currentSum + powers[i];
            if (randomNum >= currentSum && randomNum <= nextSum)
            {
                return i;
            }

            currentSum = nextSum;
        }

        Debug.LogError("权值范围计算错误！");
        return -1;
    }

    public static bool Contains(this int[] array, int value)
    {
        if (array == null || array.Length == 0)
            return false;

        foreach (var v in array)
        {
            if (v == value)
                return true;
        }

        return false;
    }
}