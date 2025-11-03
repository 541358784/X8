#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using SomeWhere;
using UnityEditor;
using UnityEngine;

namespace SomeWhere
{
    public partial class PathMap : MonoBehaviour
    {
        private int _lastCount = 0;

        GUIStyle guiStyle = new GUIStyle();

        private PathMoveLogic _moveLogic;
        public void SavePath(string pathId)
        {
            if(pathInfos == null || pathInfos.Count == 0)
                return;

            if (PathMapConfigManager.Instance._pathMap == null)
                PathMapConfigManager.Instance._pathMap = new RT_PathMap();
            
            var info = PathMapConfigManager.Instance.GetPathInfo(pathId);
            if (info != null)
                PathMapConfigManager.Instance._pathMap._pathInfos.Remove(info);

            info = new RT_PathInfo();
            info._pathId = pathId;
            info._segmentLists = new List<RT_PathSegment>();
            PathMapConfigManager.Instance._pathMap._pathInfos.Add(info);

            List<Vector2> allPosition = new List<Vector2>();
            int posIndex = 0;
            
            foreach (var pathInfo in pathInfos)
            {
                if(pathInfo._segmentLists == null || pathInfo._segmentLists.Count == 0)
                    continue;

                if(pathInfo._pathId != pathId)
                    continue;
                
                RT_PathSegment segment = new RT_PathSegment();
                segment._waitTime = pathInfo._segmentLists[0].p1._waitTime;
                segment._moveSpeed = pathInfo._segmentLists[0].p1._moveSpeed;
                segment._playAnimName = pathInfo._segmentLists[0].p1._playAnimName;
                segment._defaultAnimName = pathInfo._segmentLists[0].p1._defaultAnimName;
                segment._autoRotation = pathInfo._segmentLists[0].p1._autoRotation;
                segment._rotateAngle = new RT_Point(pathInfo._segmentLists[0].p1._rotateAngle.x,
                    pathInfo._segmentLists[0].p1._rotateAngle.y, pathInfo._segmentLists[0].p1._rotateAngle.z);
                info._segmentLists.Add(segment);

                segment._points.Add(new RT_Point(pathInfo._segmentLists[0].p1.Position.x, pathInfo._segmentLists[0].p1.Position.y, pathInfo._segmentLists[0].p1.Position.z));
                
                allPosition.Add(pathInfo._segmentLists[0].p1.Position);
                segment._points[segment._points.Count-1]._angle = CalculationAngle(allPosition, posIndex++);
                
                int cutIndex = 0;
                PathSegment nextSegment = GetNextSegment(pathInfo._segmentLists, 0);
                var previousStep = nextSegment.p1.Position;

                cutIndex++;
            
                float _currentJurney = 0;
                var curPosition = previousStep;
                int segmentIndex = 0;

                float fixedTime = 1f / 60f;
                float moveTime = 0;
                while (nextSegment != null)
                {
                    moveTime += fixedTime;
            
                    float speed = fixedTime * segment._moveSpeed;
                    Vector3 nextPos = GetBezierPointWithDistance(nextSegment, curPosition, ref _currentJurney, speed, false);

                    segment._points.Add(new RT_Point(nextPos.x, nextPos.y, nextPos.z));
                    allPosition.Add(nextPos);
                    segment._points[segment._points.Count-1]._angle = CalculationAngle(allPosition, posIndex++);
                    
                    curPosition = nextPos;
                    
                    cutIndex++;

                    if (_currentJurney >= 1)
                    {
                        segmentIndex++;
                        _currentJurney = 0;
                        nextSegment = GetNextSegment(pathInfo._segmentLists, segmentIndex);
                
                        if (nextSegment != null)
                        {
                            curPosition = nextSegment.p1.Position;
                            
                            segment = new RT_PathSegment();
                            segment._waitTime = nextSegment.p1._waitTime;
                            segment._moveSpeed = nextSegment.p1._moveSpeed;
                            segment._playAnimName = nextSegment.p1._playAnimName;
                            segment._defaultAnimName = nextSegment.p1._defaultAnimName;
                            segment._autoRotation = nextSegment.p1._autoRotation;
                            segment._rotateAngle = new RT_Point(nextSegment.p1._rotateAngle.x, nextSegment.p1._rotateAngle.y, nextSegment.p1._rotateAngle.z);
                            info._segmentLists.Add(segment);
                        }
                    }
                }
            }
            
            var configStr = JsonConvert.SerializeObject(PathMapConfigManager.Instance._pathMap, Formatting.Indented);
            File.WriteAllText($"{Application.dataPath}/Export/Configs/Decoration/PathMap.json", configStr);
            AssetDatabase.Refresh();
        }

        public void PlayPath(string loadPrefab, string pathId, bool isPlay)
        {
            if (isPlay)
            {
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(loadPrefab);
                if (prefab == null)
                    return;
            
                _moveLogic = (PrefabUtility.InstantiatePrefab(prefab, transform) as GameObject).AddComponent<PathMoveLogic>();
                _moveLogic.PlayPath(pathId);
            }
            else
            {
                if(_moveLogic != null)
                    DestroyImmediate(_moveLogic.gameObject);
                _moveLogic = null;
            }
        }
        PathSegment GetNextSegment(List<PathSegment> segmentList, int index = 0)
        {
            if (index < 0 || index >= segmentList.Count)
                return null;

            return segmentList[index];
        }
        
        private void OnDrawGizmos()
        {
            guiStyle.fontSize = 16;
            DrawBezierDebug();
        }

        void DrawBezierDebug()
        {
            if (pathInfos == null)
                return;

            foreach (var info in pathInfos)
            {
                foreach (var segment in info._segmentLists)
                {
                    if (segment.p1 == null || segment.p2 == null ||
                        (segment.p1 != null && !segment.p1.gameObject.activeSelf) ||
                        (segment.p2 != null && !segment.p2.gameObject.activeSelf)) continue;

                    var stepSize = 1.0f / interpolation;

                    var previousStep = segment.p1.Position;
                    for (var stepCount = 1; stepCount <= interpolation; ++stepCount)
                    {
                        var stepPos = GetBezierPoint(segment, stepSize * stepCount, false);
                        Gizmos.color = Color.yellow;
                        Gizmos.DrawLine(previousStep, stepPos);
                        previousStep = stepPos;
                    }
                }

                if (info._segmentLists.Count > 0)
                {
                    guiStyle.normal.textColor = Color.white;
                    Vector3 position = info._segmentLists[0].p1.Position;
                    Handles.Label(position, info._pathId.ToString(), guiStyle);
                }
            }
        }

        private float CalculationAngle(List<Vector2> allPos, int index)
        {
            if (index == 0)
                return 0;

            Vector2 newPos = allPos[index];
            Vector2 oldPos = allPos[index-1];
            
            var dirVector = newPos-oldPos;

            float angle = 0;
            if (newPos.x > oldPos.x)
            {
                float dot = Vector3.Dot((dirVector).normalized, Vector3.down);
                float acos = Mathf.Acos(dot);
         
                angle = acos * Mathf.Rad2Deg + 180;
            }
            else
            {
                // 计算向量与 y 轴之间的夹角
                float cosTheta = (dirVector.y) / Mathf.Sqrt(dirVector.x * dirVector.x + dirVector.y * dirVector.y);
                float theta = Mathf.Acos(cosTheta);
                angle = theta * Mathf.Rad2Deg;
            }

            return angle;
        }

    }
}
#endif