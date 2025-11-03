using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace SomeWhere
{
    [CustomEditor(typeof(PathMap))]
    public class PathMapInspectorEditor : Editor
    {
        private ReorderableList _reorderableList;
        private SerializedProperty _pathInfoProperty;

        private PathMap _pathMap;

        void OnEnable()
        {
            _pathMap = target as PathMap;

            _pathInfoProperty = serializedObject.FindProperty("pathInfos");
            _reorderableList = new ReorderableList(serializedObject, _pathInfoProperty);
            _reorderableList.drawElementCallback = drawElementCallback;
            _reorderableList.drawHeaderCallback = rect => GUI.Label(rect, "Path"); 
            _reorderableList.elementHeightCallback = (int index) => {
                SerializedProperty element = _pathInfoProperty.GetArrayElementAtIndex(index);
                SerializedProperty listProperty = element.FindPropertyRelative("_segmentLists");
                return EditorGUI.GetPropertyHeight(listProperty, GUIContent.none, true) + EditorGUIUtility.singleLineHeight + 6f;
            };
            _reorderableList.onAddCallback = (ReorderableList list) => {
                int index = list.serializedProperty.arraySize;
                list.serializedProperty.arraySize++;
                list.index = index;
                SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);
                element.FindPropertyRelative("_segmentLists").arraySize = 0;
            };
            _reorderableList.onRemoveCallback = (ReorderableList list) => {
                if (EditorUtility.DisplayDialog("Remove List", "Are you sure you want to remove this list?", "Yes", "No")) {
                    ReorderableList.defaultBehaviours.DoRemoveButton(list);
                }
            };
        }

        public override void OnInspectorGUI()
        {
            // base.OnInspectorGUI();

            serializedObject.Update();
            
            _reorderableList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed) 
                EditorUtility.SetDirty(target);
        }

        private string _loadPath;
        public void drawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = _pathInfoProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            
            SerializedProperty pathId = element.FindPropertyRelative("_pathId");
            pathId.stringValue = EditorGUI.TextField(new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight), pathId.stringValue);
            
                
            SerializedProperty isPlay = element.FindPropertyRelative("_isPlay");
            string text = isPlay.boolValue ? "Stop" : "Play";
            if (GUI.Button(new Rect(rect.x+rect.width-300, rect.y, 50, EditorGUIUtility.singleLineHeight), text))
            {
                isPlay.boolValue = !isPlay.boolValue;
                _pathMap.PlayPath(_loadPath, pathId.stringValue, isPlay.boolValue);
            }
            
            _loadPath = EditorGUI.TextField(new Rect(rect.x+rect.width-250, rect.y, 100, EditorGUIUtility.singleLineHeight),  _loadPath);
            
            if (GUI.Button(new Rect(rect.x+rect.width-50, rect.y, 50, EditorGUIUtility.singleLineHeight), "Save")) 
            {
                _pathMap.SavePath(pathId.stringValue);
            }
            SerializedProperty listProperty = element.FindPropertyRelative("_segmentLists");
            float listHeight = EditorGUI.GetPropertyHeight(listProperty, GUIContent.none, true);
            EditorGUI.PropertyField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight + 2f, rect.width, listHeight), listProperty, true);
        }
    }
}