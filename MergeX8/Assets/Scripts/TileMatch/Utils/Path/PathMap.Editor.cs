#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using SomeWhereTileMatch;
using UnityEditor;
using UnityEngine;

namespace SomeWhereTileMatch
{
    public partial class PathMap : MonoBehaviour
    {
        public bool isEditorPlay = false;
        private bool isInitUpdate = false;
        
        public void CuttingPoint()
        {
            InitCutRoot();
            ClearCuttingPoint();

            int cutIndex = 0;
            Transform cutTransform = cutRoot;

            PathSegment segment = GetNextSegment(0);
            var previousStep = segment.p1.position;
    
            {
                GameObject newObj = CreateObj(previousStep, cutIndex.ToString(), cutTransform);
                cuttingPointList.Add(newObj.transform);
                cutIndex++;
            }
            
            float _currentJurney = 0;
            var curPosition = previousStep;
            int segmentIndex = 0;

            while (segment != null)
            {
                moveTime += fixedTime;
                
                float speed = fixedTime * moveSpeed;
                Vector3 nextPos = GetBezierPointWithDistance(segment, curPosition, ref _currentJurney, speed, false);

                pathLength += Vector3.Distance(curPosition, nextPos);
                curPosition = nextPos;
                
                GameObject newObj = CreateObj(curPosition, cutIndex.ToString(), cutTransform);
                cuttingPointList.Add(newObj.transform);
                cutIndex++;

                if (_currentJurney >= 1)
                {
                    segmentIndex++;
                    _currentJurney = 0;
                    segment = GetNextSegment(segmentIndex);
                    
                    if(segment != null)
                        curPosition = segment.p1.position;
                }
            }
        }

        PathSegment GetNextSegment(int index = 0)
        {
            if (index < 0 || index >= segmentList.Count)
                return null;

            return segmentList[index];
        }

        public void ClearCuttingPoint()
        {
            pathLength = 0;
            moveTime = 0;
            InitCutRoot();
            cuttingPointList.Clear();

            Transform cutTransform = cutRoot;
            if (cutTransform.childCount == 0)
                return;

            for (int i = cutTransform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(cutTransform.GetChild(i).gameObject);
            }
        }


        private GameObject CreateObj(Vector3 position, string name, Transform parent)
        {
            GameObject newGameObject = new GameObject(name);
            newGameObject.transform.position = position;
            newGameObject.transform.SetParent(parent, true);

            return newGameObject;
        }
        
        public void EditorUpdate()
        {
#if UNITY_EDITOR
            if (!isEditorPlay)
                return;

            curShowTime += fixedTime;

            if (curShowTime > showTime)
            {
                curHideStepTime += fixedTime;
            }
            foreach (var kv in pathMoveItems)
            {
                kv.Update();
            }

            if(pathLoopIndex >= cuttingCount)
                return;

            curLoopTime = curLoopTime + fixedTime;
            if (curLoopTime < pathLoopTime)
                return;
            curLoopTime = 0;

            pathLoopIndex++;
            CreatePokerPrefab();
#endif
        }

        public void StopPathMove()
        {
            RemoveEditorUpdate();
            isEditorPlay = false;
            pathLoopIndex = 0;
            
            foreach (var kv in pathMoveItems)
            {
                GameObject.DestroyImmediate(kv.moveItem);
            }
            pathMoveItems.Clear();
        }
        public void PlayPathMove()
        {
#if UNITY_EDITOR
            if(isEditorPlay)
                return;
            
            if (cuttingPointList == null || cuttingPointList.Count == 0)
                return;

            InitEditorUpdate();
            isEditorPlay = true;
            pathLoopTime = 1.0f * moveTime / cuttingCount / moveStepTime;
            curLoopTime = 0;
            pathLoopIndex = 0;
            hideIndex = 0;
            curShowTime = 0;
            curHideStepTime = 0;
            
            InitCutRoot();
            InitPathMoveItems();
            CreatePokerPrefab();
#endif
        }

        private void CreatePokerPrefab()
        {
#if UNITY_EDITOR
            PathMoveItem moveItem = GetPathMoveItem(pathLoopIndex);
            if(moveItem == null)
                return;
            
            GameObject pokerPrefab = AssetDatabase.LoadAssetAtPath("Assets/Export/GamePrefabs/Match3/M3Item.prefab",
                typeof(GameObject)) as GameObject;

            GameObject clonObj = GameObject.Instantiate(pokerPrefab);
            clonObj.transform.Find("Mask").gameObject.SetActive(false);
            moveItem.AddChild(clonObj, 1);
#endif
        }
        
        private void InitEditorUpdate()
        {
#if UNITY_EDITOR
            if(isInitUpdate)
                return;
 
            EditorApplication.update += EditorUpdate;
            isInitUpdate = true;
#endif
        }

        private void RemoveEditorUpdate()
        {
#if UNITY_EDITOR
            if(!isInitUpdate)
                return;
            
            isInitUpdate = false;
            EditorApplication.update -= EditorUpdate;
#endif
        }
        private void OnDrawGizmos()
        {
           DrawBezierDebug();
        }

        void DrawBezierDebug()
        {
            if (segmentList == null)
            {
                return;
            }

            //ForNpc 黄色
            foreach (var segment in segmentList)
            {
                if (segment.p1 == null || segment.p2 == null) continue;

                var stepSize = 1.0f / interpolation;

                var previousStep = segment.p1.position;
                for (var stepCount = 1; stepCount <= interpolation; ++stepCount)
                {
                    var stepPos = GetBezierPoint(segment, stepSize * stepCount, false);
                    
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(previousStep, stepPos);
                    previousStep = stepPos;
                }
            }

            var fontStyle = new GUIStyle();
            fontStyle.normal.textColor = Color.red;
        }
        
    }
}
#endif