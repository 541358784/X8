using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;

namespace TMatch
{


    public class FlyTransformEffect : MonoBehaviour
    {
        private Vector3 _srcPos;
        private Vector2 _controlPos;
        private Transform _destTransform;
        private float _timeTotal;
        private float _timeDelay;
        private Action _onFinish;
        private float _tick;

        // Update is called once per frame
        private void Update()
        {
            if (_timeDelay > 0.0f)
            {
                _timeDelay -= Time.deltaTime;
                return;
            }

            if (_tick >= _timeTotal)
            {
                if (_onFinish != null)
                {
                    _onFinish();
                }

                // transform.DOPause();
                return;
            }

            _tick += Time.deltaTime;
            var pos = EffectManager.Bezier(_tick / _timeTotal, _srcPos, _controlPos, _destTransform.position);
            transform.position = new Vector3(pos.x, pos.y, transform.position.z);
            // transform.DOMove(new Vector3(pos.x, pos.y, transform.position.z), Time.deltaTime);
        }

        public void InitData(Transform srcTransform, Vector2 controlPos, Transform destPos, float time = 1.0f,
            float delay = 0, Action action = null)
        {
            _srcPos = srcTransform.position;
            _controlPos = controlPos;
            _destTransform = destPos;
            _timeTotal = time;
            _timeDelay = delay;
            _onFinish = action;
            transform.position = new Vector3(_srcPos.x, _srcPos.y, transform.position.z);
            _tick = 0.0f;
        }

        //另一种形式
        public static void DoBezier(Transform transform, Vector3 srcPos, Vector3 destPos, float time = 1.0f,
            Action action = null)
        {
            List<Vector3> pathNodes =
                FlyTransformEffect.GenerateBezierPoints(srcPos, destPos,
                    (int) Math.Ceiling(time * 20)); //按60帧算, 1秒更新20下位置
            transform.DOPath(pathNodes.ToArray(), time, PathType.CatmullRom).OnComplete(() =>
            {
                if (action != null)
                {
                    action();
                }
            });
        }

        private static List<Vector3> GenerateBezierPoints(Vector3 start, Vector3 end, int pointCount)
        {
            //生成控制点
            List<Vector3> sourcePoints = new List<Vector3>();
            sourcePoints.Add(start);
            sourcePoints.Add(GenerateTurningPoint(start, end));
            sourcePoints.Add(end);
            //生成路径点
            List<Vector3> points = new List<Vector3>();
            Vector3 lastP = Vector3.zero;
            for (int i = 0; i <= pointCount; i++)
            {
                Vector3 p = GetLerpPoint(sourcePoints, (float) i / pointCount);
                if (p != Vector3.zero && p != lastP)
                {
                    points.Add(p);
                    lastP = p;
                }
            }

            return points;
        }

        private static Vector3 GenerateTurningPoint(Vector3 firstPoint, Vector3 secondPoint)
        {
            var vDis = secondPoint - firstPoint;
            var controlPos = new Vector2(firstPoint.x + vDis.x * UnityEngine.Random.Range(-0.4f, 0.4f),
                firstPoint.y + vDis.y * UnityEngine.Random.Range(0.2f, 0.9f));
            float x = controlPos.x;
            float y = controlPos.y;
            //float x = secondPoint.x + (secondPoint.x - firstPoint.x) / 3 * 4;
            //float y = firstPoint.y + (secondPoint.y - firstPoint.y) / 4;
            float z = firstPoint.z;
            return new Vector3(x, y, z);
        }

        //递归产生唯一的路径点
        private static Vector3 GetLerpPoint(List<Vector3> sourcePoints, float delta)
        {
            List<Vector3> points = new List<Vector3>();
            for (int i = 0; i < sourcePoints.Count - 1; i++)
            {
                points.Add(Vector3.Lerp(sourcePoints[i], sourcePoints[i + 1], delta));
            }

            if (points.Count == 1)
            {
                return points[0];
            }

            return GetLerpPoint(points, delta);
        }
    }
}
