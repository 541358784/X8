using System;
using System.Collections;
using System.Collections.Generic;
using SomeWhere;
using UnityEngine;

namespace SomeWhere
{
    public partial class PathMap : MonoBehaviour
    {
        public float interpolation = 20;
        public List<PathInfo> pathInfos;

        private void Awake()
        {
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

        private Vector2 GetBezierPoint(PathSegment segment, float percent, bool reverseDirection)
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

            var begin = segment.p1.Position;
            var end = segment.p2.Position;
            var cp1 = Vector2.zero;
            var cp2 = Vector2.zero;

            var cp1IsNull = segment.cp1 == null;
            var cp2IsNUll = segment.cp2 == null;

            if (!cp1IsNull) cp1 = segment.cp1.position;
            if (!cp2IsNUll) cp2 = segment.cp2.position;

            if (reverseDirection)
            {
                begin = segment.p2.Position;
                end = segment.p1.Position;
                if (!cp2IsNUll) cp1 = segment.cp2.position;
                if (!cp1IsNull) cp2 = segment.cp1.position;
            }

            if (!cp1IsNull && !cp2IsNUll)
            {
                result = GetBezierPoint3(begin, end, cp1, cp2, percent);
                segment.CacheResult(percent, result, reverseDirection);
                return result;
            }
            else if (cp1IsNull && cp2IsNUll)
            {
                result = GetLerpPoint(begin, end, percent);
                segment.CacheResult(percent, result, reverseDirection);
                return result;
            }

            var cp = cp1IsNull ? cp2 : cp1;

            result = GetBezierPoint2(begin, end, cp, percent);
            segment.CacheResult(percent, result, reverseDirection);

            return result;
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

        public Vector2 GetBezierPointWithDistance(PathSegment segment, Vector2 currentPos,
            ref float currentJurneyPercent, float distance, bool reverseDirection)
        {
            if (segment == null || segment.p1 == null || segment.p2 == null)
            {
                return Vector3.zero;
            }

            var stepPercent = 0.001f / (segment.Distance() / 10f);
            stepPercent = Mathf.Max(0.0005f, stepPercent);
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
    }
}