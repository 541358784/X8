#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DG.Tweening;
using Mosframe;
using Newtonsoft.Json;
using SomeWhere;
using UnityEditor;
using UnityEngine;


namespace Zuma
{
    public partial class ZumaPathMap
    {
        GUIStyle guiStyle = new GUIStyle();
        private float interpolation = 1000;

        private List<Vector2> _pathPoints = new List<Vector2>();
        private Queue<GameObject> _useSpheres = new Queue<GameObject>();
        private Queue<GameObject> _freeSpheres = new Queue<GameObject>();

        private GameObject _spheresRoot = null;

        public void ExportPath()
        {
            _point.Clear();
            int posIndex = 0;

            if (_segments == null || _segments.Count == 0)
                return;

            _point.Add(new Vector2Wrapper(transform.InverseTransformPoint(_segments[0].p1.position)));

            int cutIndex = 0;
            Segment nextSegment = GetNextSegment(_segments, 0);
            var previousStep = nextSegment.p1.position;

            cutIndex++;

            float _currentJurney = 0;
            var curPosition = previousStep;
            int segmentIndex = 0;

            float fixedTime = 1f / 60f;
            float moveTime = 0;
            while (nextSegment != null)
            {
                moveTime += fixedTime;

                float speed = fixedTime * _speed;
                Vector3 nextPos = GetBezierPointWithDistance(nextSegment, curPosition, ref _currentJurney, speed, false);

                _point.Add(new Vector2Wrapper(transform.InverseTransformPoint(nextPos)));

                curPosition = nextPos;
                cutIndex++;

                if (_currentJurney >= 1)
                {
                    segmentIndex++;
                    _currentJurney = 0;
                    nextSegment = GetNextSegment(_segments, segmentIndex);

                    if (nextSegment != null)
                    {
                        curPosition = nextSegment.p1.position;
                    }
                }
            }

            // var configStr = JsonConvert.SerializeObject(_point, Formatting.Indented);
            // string filePath = string.Format($"{Application.dataPath}/" + ZumaPathMapConfigManager._exportPath, _pathId)+".json";
            // File.WriteAllText(filePath, configStr);
            // AssetDatabase.Refresh();
        }

        public Vector2 GetBezierPointWithDistance(Segment segment, Vector2 currentPos,
            ref float currentJurneyPercent, float distance, bool reverseDirection)
        {
            if (segment == null || segment.p1 == null || segment.p2 == null)
            {
                return Vector3.zero;
            }

            var stepPercent = 0.00001f / (segment.Distance() / 10f);
            stepPercent = Mathf.Max(0.000005f, stepPercent);
            var jurneyDistance = 0f;
            while (true)
            {
                currentJurneyPercent += stepPercent;
                var newPos = GetBezierPoint(segment, currentJurneyPercent, reverseDirection);

                var deltaDistance = Vector2.Distance(currentPos, newPos);
                jurneyDistance += deltaDistance;

                currentPos = newPos;

                if (jurneyDistance >= distance) break;
                if (currentJurneyPercent >= 1f) break;
            }

            return currentPos;
        }


        Segment GetNextSegment(List<Segment> segmentList, int index = 0)
        {
            if (index < 0 || index >= segmentList.Count)
                return null;

            return segmentList[index];
        }

        private void OnDrawGizmos()
        {
            guiStyle.fontSize = 16;

            if (_segments == null)
                return;

            _pathPoints.Clear();
            Vector2 startPosition = Vector2.zero;
            foreach (var segment in _segments)
            {
                if (_pathPoints.Count == 0)
                {
                    startPosition = segment.p1.position;
                    _pathPoints.Add(startPosition);
                }

                if (segment.p1 == null || segment.p2 == null ||
                    (segment.p1 != null && !segment.p1.gameObject.activeSelf) ||
                    (segment.p2 != null && !segment.p2.gameObject.activeSelf)) continue;

                var stepSize = 1.0f / interpolation;

                var previousStep = segment.p1.position;
                for (var stepCount = 0; stepCount <= interpolation; ++stepCount)
                {
                    var stepPos = GetBezierPoint(segment, stepSize * stepCount, false);
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(previousStep, stepPos);
                    previousStep = stepPos;

                    if (Vector2.Distance(stepPos, startPosition) >= _radius)
                    {
                        startPosition = stepPos;
                        _pathPoints.Add(startPosition);
                    }
                }
            }


            // while (_useSpheres.Count > 0)
            // {
            //     var obj = _useSpheres.Dequeue();
            //     obj.gameObject.SetActive(false);
            //
            //     _freeSpheres.Enqueue(obj);
            // }
            //
            // if (_pathPoints.Count == 0)
            //     return;

            // foreach (var pathPoint in _pathPoints)
            // {
            //     GameObject sphere = null;
            //     if (_freeSpheres.Count > 0)
            //     {
            //         sphere = _freeSpheres.Dequeue();
            //     }
            //     else
            //     {
            //         if (_spheresRoot == null)
            //             _spheresRoot = GameObject.Find("Spheres");
            //
            //         if (_spheresRoot == null)
            //             _spheresRoot = new GameObject("Spheres");
            //
            //         sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //         sphere.transform.parent = _spheresRoot.transform;
            //     }
            //
            //     _useSpheres.Enqueue(sphere);
            //     sphere.gameObject.SetActive(true);
            //     sphere.transform.position = pathPoint;
            //     sphere.transform.localScale = new Vector3(_radius, _radius, _radius);
            // }
        }

        private Vector2 GetBezierPoint(Segment segment, float percent, bool reverseDirection)
        {
            var result = Vector2.zero;
            if (segment == null || segment.p1 == null || segment.p2 == null)
            {
                return Vector3.zero;
            }

#if !UNITY_EDITOR
            if (reverseDirection)
            {
                if (segment.CacheReverseDic.ContainsKey(percent)) return segment.CacheReverseDic[percent];
            }
            else
            {
                if (segment.CacheDic.ContainsKey(percent)) return segment.CacheDic[percent];
            }
#endif

            var begin = segment.p1.position;
            var end = segment.p2.position;
            var cp1 = Vector2.zero;
            var cp2 = Vector2.zero;

            var cp1IsNull = segment.cp1 == null;
            var cp2IsNUll = segment.cp2 == null;

            if (!cp1IsNull) cp1 = segment.cp1.position;
            if (!cp2IsNUll) cp2 = segment.cp2.position;

            if (reverseDirection)
            {
                begin = segment.p2.position;
                end = segment.p1.position;
                if (!cp2IsNUll) cp1 = segment.cp2.position;
                if (!cp1IsNull) cp2 = segment.cp1.position;
            }

            if (!cp1IsNull && !cp2IsNUll)
            {
                result = GetBezierPoint3(begin, end, cp1, cp2, percent);
                return result;
            }
            else if (cp1IsNull && cp2IsNUll)
            {
                result = GetLerpPoint(begin, end, percent);
                return result;
            }

            var cp = cp1IsNull ? cp2 : cp1;

            result = GetBezierPoint2(begin, end, cp, percent);

            return result;
        }

        internal Vector3 GetBezierPoint2(Vector2 begin, Vector2 end, Vector2 control, float percent)
        {
            var A = begin;
            var B = control;
            var C = end;

            var AB = B - A;
            var BC = C - B;

            var D = A + AB * percent;
            var E = B + BC * percent;

            var DF = (E - D) * percent;

            var F = D + DF;

            return F;
        }

        private Vector2 GetLerpPoint(Vector2 begin, Vector2 end, float percent)
        {
            return Vector3.Lerp(begin, end, percent);
        }

        private Vector2 GetBezierPoint3(Vector2 begin, Vector2 end, Vector2 cp1, Vector2 cp2, float percent)
        {
            float ax, bx, cx;
            float ay, by, cy;
            float tSquared, tCubed;
            Vector2 result;

            /*計算多項式係數*/
            cx = 3f * (cp1.x - begin.x);
            bx = 3f * (cp2.x - cp1.x) - cx;
            ax = end.x - begin.x - cx - bx;

            cy = 3f * (cp1.y - begin.y);
            by = 3f * (cp2.y - cp1.y) - cy;
            ay = end.y - begin.y - cy - by;

            /*計算位於參數值t的曲線點*/
            tSquared = percent * percent;
            tCubed = tSquared * percent;

            result.x = (ax * tCubed) + (bx * tSquared) + (cx * percent) + begin.x;
            result.y = (ay * tCubed) + (by * tSquared) + (cy * percent) + begin.y;

            return result;
        }
    }
}
#endif