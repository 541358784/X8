using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Framework;
using UnityEngine;

public struct FlyUnit
{
    private Vector2 _srcPos;
    private Vector2 _controlPos;
    private Transform _destTransform;
    private Vector3 _destPos;
    private float _timeTotal;
    private float _timeDelay;
    private Action _callBack;
    private float _tick;
    private Transform _transform;

    // Update is called once per frame
    IEnumerator update()
    {
        while (true)
        {
            while (_timeDelay > 0.0f)
            {
                _timeDelay -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            if (_transform == null)
            {
                yield break;
            }

            if (_tick >= _timeTotal)
            {
                _transform.DOPause();
                _callBack?.Invoke();
                _callBack = null;
                yield break;
            }

            _tick += Time.deltaTime;
            var pos = bezier(_tick / _timeTotal, _srcPos, _controlPos,
                _destTransform == null ? _destPos : _destTransform.position);
            // _transform.DOMove(new Vector3(pos.x, pos.y, _transform.position.z), Time.deltaTime);
            _transform.position = new Vector3(pos.x, pos.y, _transform.position.z);

            yield return new WaitForEndOfFrame();
        }
    }

    public void Start(Vector2 srcPos, Transform targetTransform, Vector2 controlPos, Transform transform,
        float totalTime = 1.0f, float delay = 0, Action callback = null)
    {
        _srcPos = srcPos;
        _destTransform = targetTransform;
        _controlPos = controlPos;
        _timeTotal = totalTime;
        _timeDelay = delay;
        _callBack = callback;
        _transform = transform;
        _tick = 0.0f;

        CoroutineManager.Instance.StartCoroutine(update());
    }

    public void Start(Vector2 srcPos, Vector3 targetPos, Vector2 controlPos, Transform transform,
        float totalTime = 1.0f, float delay = 0, Action callback = null)
    {
        _srcPos = srcPos;
        _destTransform = null;
        _destPos = targetPos;
        _controlPos = controlPos;
        _timeTotal = totalTime;
        _timeDelay = delay;
        _callBack = callback;
        _transform = transform;
        _tick = 0.0f;

        CoroutineManager.Instance.StartCoroutine(update());
    }

    private static Vector2 bezier(float t, Vector2 a, Vector2 b, Vector2 c)
    {
        var ab = Vector2.Lerp(a, b, t);
        var bc = Vector2.Lerp(b, c, t);
        return Vector2.Lerp(ab, bc, t);
    }
}