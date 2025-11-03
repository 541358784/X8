using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace SomeWhereTileMatch
{
    [CustomEditor(typeof(PathMap))]
    public class PathMapInspectorEditor : Editor
    {
        private ReorderableList _reorderableList;
        private ReorderableList _cutPointList;
        private SerializedProperty _segmentListProperty;
        private SerializedProperty _cutPointProperty;

        private PathMap _pathMap;

        void OnEnable()
        {
            _pathMap = target as PathMap;

            _segmentListProperty = serializedObject.FindProperty("segmentList");
            _reorderableList = new ReorderableList(serializedObject, _segmentListProperty);
            _reorderableList.drawElementCallback = drawElementCallback;
            _reorderableList.drawHeaderCallback = rect => GUI.Label(rect, "Path"); 
            
            _cutPointProperty = serializedObject.FindProperty("cuttingPointList");
            _cutPointList  = new ReorderableList(serializedObject, _cutPointProperty);
            _cutPointList.drawElementCallback = drawCutElementCallback;
            _cutPointList.drawHeaderCallback = rect => GUI.Label(rect, "CutPoint"); 
        }

        public override void OnInspectorGUI()
        {
            // base.OnInspectorGUI();
            EditorGUILayout.LabelField("PATH 长度", _pathMap.pathLength.ToString());
            EditorGUILayout.LabelField("移动时间", _pathMap.moveTime.ToString());
            
            _pathMap.moveSpeed = EditorGUILayout.FloatField("移动速度", _pathMap.moveSpeed);
            _pathMap.moveStepTime = EditorGUILayout.FloatField("移动间隔", _pathMap.moveStepTime);
            EditorGUILayout.Space();
            
            _pathMap.flySpeed = EditorGUILayout.FloatField("起始飞行速度", _pathMap.flySpeed);
            _pathMap.flyStep = EditorGUILayout.FloatField("飞行速度步进", _pathMap.flyStep);
            EditorGUILayout.Space();
            
            _pathMap.showTime = EditorGUILayout.FloatField("展示时间", _pathMap.showTime);
            _pathMap.hideTime = EditorGUILayout.FloatField("消失时间", _pathMap.hideTime);
            
            
            EditorGUILayout.BeginVertical();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            if (GUILayout.Button("创建必要节点"))
            {
                _pathMap.InitRoot();
            }
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            if (GUILayout.Button("清除切割点"))
            {
                _pathMap.ClearCuttingPoint();
            }
            
            if (GUILayout.Button("切割点"))
            {
                _pathMap.CuttingPoint();
            }
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            if(!_pathMap.isEditorPlay)
            {
                if (GUILayout.Button("播放"))
                {
                    _pathMap.PlayPathMove();
                }
            }
            else  if (GUILayout.Button("停止"))
            {
                _pathMap.StopPathMove();
            }
            
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            
           EditorGUILayout.EndVertical();   
            serializedObject.Update();
            
            _reorderableList.DoLayoutList();
            _cutPointList.DoLayoutList();
            
            serializedObject.ApplyModifiedProperties();
            
            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }

        public void drawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = _segmentListProperty.GetArrayElementAtIndex(index);
            rect.height -= 4;
            rect.y += 2;
            EditorGUI.PropertyField(rect, element);
        }
        
        public void drawCutElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = _cutPointProperty.GetArrayElementAtIndex(index);
            rect.height -= 4;
            rect.y += 2;
            EditorGUI.PropertyField(rect, element);
        }
    }
}