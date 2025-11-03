using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DragonU3DSDK;
using SomeWhereTileMatch;
using UnityEditor;
using UnityEngine;

namespace SomeWhereTileMatch
{
    public partial class PathMap : MonoBehaviour
    {
        private float interpolation = 20;
        public List<PathSegment> segmentList;
        public List<Transform> cuttingPointList = new List<Transform>();
        public List<PathMoveItem> pathMoveItems = new List<PathMoveItem>();
        
        public float pathLength = 0;
        public float moveSpeed = 10;
        public float moveTime = 0;
        public float moveStepTime = 1;
        public float flySpeed = 20;
        public float flyStep = 0.1f;
        public float showTime = 3;
        public float hideTime = 0.5f;


        private float curShowTime = 0;
        private float curHideStepTime = 0;
        private int hideIndex = 0;
        
        private const int cuttingCount = 52;
        private const string cutObjName = "Cutting";
        private const string pathObjName = "Path";
        private const string moveObjName = "Move";

        private Transform cutRoot = null;
        private Transform pathRoot = null;
        private Transform moveRoot = null;

        private float editorTime = 0;
        private float editorCreateTime = 0;

        private float fixedTime = 0.02f;

        private float pathLoopTime = 0;
        private float curLoopTime = 0;
        private int pathLoopIndex = 0;
        
        private void Awake()
        {
            InitRoot();
        }

        public void InitRoot()
        {
            InitCutRoot();
            InitPathRoot();
            InitMoveRoot();
        }

        private void InitCutRoot()
        {
            if (cutRoot != null)
                return;

            cutRoot = CreateGameObject(cutObjName);
        }

        private void InitPathRoot()
        {
            if (pathRoot != null)
                return;

            pathRoot = CreateGameObject(pathObjName);
        }

        private void InitMoveRoot()
        {
            if (moveRoot != null)
                return;

            moveRoot = CreateGameObject(moveObjName);
        }
        
        private Transform CreateGameObject(string objName)
        {
            Transform findObj = transform.Find(objName);
            if (findObj != null)
                return findObj;

            findObj = new GameObject(objName).transform;
            findObj.SetParent(transform);
            findObj.transform.localPosition = Vector3.zero;
            findObj.transform.localScale = Vector3.one;

            return findObj;
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
            if (segment == null || segment.p1 == null || segment.p2 == null)
            {
                return Vector3.zero;
            }

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
                return GetBezierPoint3(begin, end, cp1, cp2, percent);
            }
            else if (cp1IsNull && cp2IsNUll)
            {
                return GetLerpPoint(begin, end, percent);
            }

            var cp = cp1IsNull ? cp2 : cp1;

            return GetBezierPoint2(begin, end, cp, percent);
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

            var stepPercent = 0.0005f;
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

        public Vector3 GetCuttingPoint(int index)
        {
            index = index < 0 ? 0 : index;
            index = index >= cuttingPointList.Count ? cuttingPointList.Count - 1 : index;
            return cuttingPointList[index].transform.position;
        }

        public void InitPathMoveItems()
        {
            if (pathMoveItems.Count > 0)
            {
                foreach (var kv in pathMoveItems)
                {
                    PathMoveItem moveItem = kv;
                    moveItem.Rest();
                }
                return;
            }
            
            InitMoveRoot();
            
            int moveStep = (int)Mathf.Ceil(1.0f*cuttingPointList.Count / cuttingCount);
            
            for (int i = 0; i < cuttingCount; i++)
            {
                PathMoveItem moveItem = new PathMoveItem();
                moveItem.Init(moveRoot, i*moveStep, this);
                pathMoveItems.Add(moveItem);
            }
        }

        public PathMoveItem GetPathMoveItem(int index)
        {
            if (pathMoveItems == null || pathMoveItems.Count == 0)
                return null;

            if(index >= pathMoveItems.Count)
                index -= pathMoveItems.Count;
            
            if (index < 0)
                index = 0;

            return pathMoveItems[index];
        }

        public Vector3 GetPathMovePosition(int index)
        {
            PathMoveItem moveItem = GetPathMoveItem(index);
            if (moveItem == null)
                return Vector3.zero;

            return moveItem.moveItem.transform.position;
        }
    }
}