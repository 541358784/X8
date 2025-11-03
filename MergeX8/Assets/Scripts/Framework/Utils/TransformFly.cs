using System;
using System.Collections;
using DG.Tweening;
using Framework;
using UnityEngine;
using Random = UnityEngine.Random;

public class TransformFly
{
    public enum ControlType
    {
        Left,
        Right,
        Custom,
    }

    public struct FlyData
    {
        public int objCount;
        public float duration;
        public bool scaleAnimate; //飞行中缩放到目标大小
        public float scaleTo;
        public ControlType controlType;
        public Transform targetTF;

        private static FlyData _defaultData = new FlyData(3, 0.5f, false, 1f, ControlType.Left);

        public static FlyData defaultOne
        {
            get => _defaultData;
        }

        public FlyData(int objCount, float duration, bool scaleAnimate, float scaleTo, ControlType controlType,
            Transform target = null)
        {
            this.objCount = objCount;
            this.duration = duration;
            this.scaleAnimate = scaleAnimate;
            this.scaleTo = scaleTo;
            this.controlType = controlType;
            this.targetTF = target;
        }
    }

    public static float FlyToTargetTransform(Vector3 srcPos, Transform targetTransform, GameObject flyPrefab,
        GameObject attachEffect, Transform parent, FlyData data, Action<int> callback, float delayCallback = -1)
    {
        var control = getControlType(srcPos, targetTransform.position, data.controlType);

        var flyDeltaDuration = 0.1f;
        for (var i = 0; i < data.objCount; i++)
        {
            var time = data.duration + flyDeltaDuration * i;

            //创建飞行物体
            var flyClone = GameObject.Instantiate(flyPrefab, Vector3.zero, Quaternion.identity, (Transform) parent);
            if (flyClone.transform is RectTransform)
            {
                (flyClone.transform as RectTransform).CopyPositionFrom(flyPrefab.transform as RectTransform);
            }

            flyClone.transform.position = srcPos;
            flyClone.SetActive(true);
            if (attachEffect != null)
            {
                var effect =
                    GameObject.Instantiate(attachEffect, Vector3.zero, Quaternion.identity, flyClone.transform);
                effect.transform.localPosition = Vector3.zero;
            }

#if UNITY_EDITOR
            flyClone.name = "FLY_OBJ";
#endif

            //缩放
            if (data.scaleAnimate)
            {
                flyClone.transform.DOScale(Vector3.one * data.scaleTo, time);
            }

            //飞行
            var flyUnit = new FlyUnit();
            var flyIndex = i;
            flyUnit.Start(srcPos, targetTransform, control, flyClone.transform, time, 0, () =>
            {
                GameObject.Destroy(flyClone);
                CoroutineManager.Instance.StartCoroutine(DelayCall(delayCallback, callback, flyIndex));
            });
        }

        var totalDuration = data.duration + flyDeltaDuration * data.objCount;
        return totalDuration;
    }

    public static float FlyToTargetPos(Vector3 srcPos, Vector3 targetPos, GameObject flyPrefab, GameObject attachEffect,
        Transform parent, FlyData data, Action<int> callback, float delayCallback = -1)
    {
        var control = getControlType(srcPos, targetPos, data.controlType);

        var flyDeltaDuration = 0.1f;
        for (var i = 0; i < data.objCount; i++)
        {
            var time = data.duration + flyDeltaDuration * i;

            //创建飞行物体
            var flyClone = GameObject.Instantiate(flyPrefab, Vector3.zero, Quaternion.identity, (Transform) parent);
            if (flyClone.transform is RectTransform)
            {
                (flyClone.transform as RectTransform).CopyPositionFrom(flyPrefab.transform as RectTransform);
            }

            flyClone.transform.position = srcPos;
            flyClone.SetActive(true);
            if (attachEffect != null)
            {
                var effect =
                    GameObject.Instantiate(attachEffect, Vector3.zero, Quaternion.identity, flyClone.transform);
                effect.transform.localPosition = Vector3.zero;
            }

#if UNITY_EDITOR
            flyClone.name = "FLY_OBJ";
#endif

            //缩放
            if (data.scaleAnimate)
            {
                flyClone.transform.DOScale(Vector3.one * data.scaleTo, time);
            }

            //飞行
            var flyUnit = new FlyUnit();
            var flyIndex = i;
            flyUnit.Start(srcPos, targetPos, control, flyClone.transform, time, 0, () =>
            {
                GameObject.Destroy(flyClone);
                CoroutineManager.Instance.StartCoroutine(DelayCall(delayCallback, callback, flyIndex));
            });
        }

        var totalDuration = data.duration + flyDeltaDuration * data.objCount;
        return totalDuration;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tempSource"></param>
    /// <param name="target"></param>
    /// <param name="flyPrefab"></param>
    /// <param name="hitObj"></param>
    /// <param name="tempParent"></param>
    /// <param name="callback"></param>
    /// <param name="data"></param>
    /// <returns>返回动画的总耗时</returns>
    public static float FlyToTargetTransform(Transform source, Transform target, GameObject flyPrefab,
        GameObject attachEffect, Transform parent, FlyData data, Action<int> callback)
    {
        var srcPos = source.position;
        var desPos = target.position;

        return FlyToTargetTransform(srcPos, target, flyPrefab, attachEffect, parent, data, callback);
    }


    public static float FlyToTargetPosByOne(Vector3 srcPos, Vector3 targetPos, GameObject flyPrefab,
        GameObject attachEffect, FlyData data, Action<int> callback, float delayCallback = -1)
    {
        var control = getControlType(srcPos, targetPos, data.controlType);

        var flyDeltaDuration = 0.15f;
        var time = data.duration;

        //创建飞行物体
        var flyClone = flyPrefab;
        flyClone.transform.position = srcPos;
        flyClone.SetActive(true);
        if (attachEffect != null)
        {
            var effect = GameObject.Instantiate(attachEffect, Vector3.zero, Quaternion.identity, flyClone.transform);
            effect.transform.localPosition = Vector3.zero;
        }

#if UNITY_EDITOR
        flyClone.name = "FLY_OBJ";
#endif

        //缩放
        if (data.scaleAnimate)
        {
            flyClone.transform.DOScale(Vector3.one * data.scaleTo, time);
        }

        //飞行
        var flyUnit = new FlyUnit();
        flyUnit.Start(srcPos, targetPos, control, flyClone.transform, time, 0, () =>
        {
            GameObject.Destroy(flyClone);
            CoroutineManager.Instance.StartCoroutine(DelayCall(delayCallback, callback, 0));
        });

        var totalDuration = data.duration;
        return totalDuration;
    }

    public static Vector2 getControlType(Vector3 source, Vector3 target, ControlType controlType)
    {
        var vDis = target - source;
        var control = Vector2.zero;

        var direction = 1;
        switch (controlType)
        {
            case ControlType.Left:
                direction = 1;
                break;
            case ControlType.Right:
                direction = -1;
                break;
        }

        // if (controlType == 0) {
        // } else if (data.flyType == 1) {
        //     control.x = source.x + UnityEngine.Random.Range (-1.5f, 1.5f);
        //     control.y = source.y + vDis.y * UnityEngine.Random.Range (0.2f, 0.9f);
        // }

        if (controlType == ControlType.Custom)
        {
            control.x = source.x + Random.Range(vDis.x, vDis.x / 2);
            control.y = source.y - Random.Range(vDis.y / 3f, vDis.y / 2f);
            return control;
        }

        control.x = source.x + vDis.x / 2;
        control.y = source.y + vDis.y / 2f;

        var normal = new Vector2(vDis.y, -vDis.x);
        control = Vector3.MoveTowards(control, normal, direction * 1);

        return control;
    }

    private static IEnumerator DelayCall(float delay, Action<int> action, int param1)
    {
        if (delay < 0)
        {
            action?.Invoke(param1);
            yield break;
        }

        yield return new WaitForSeconds(delay);
        action?.Invoke(param1);
    }
}