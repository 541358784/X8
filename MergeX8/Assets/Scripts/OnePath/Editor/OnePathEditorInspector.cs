using System;
using SomeWhere;
using UnityEditor;
using UnityEngine;

namespace OnePath.Editor
{
    [CustomEditor(typeof(OnePathEditor))]
    public class OnePathEditorInspector : UnityEditor.Editor
    {
        private OnePathEditor _pathEditor;

        private void OnEnable()
        {
            _pathEditor = target as OnePathEditor;
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var chunkWidth = serializedObject.FindProperty("_chunkWidth");
            EditorGUILayout.PropertyField(chunkWidth, new GUIContent("格子宽度"));

            var chunkHeight = serializedObject.FindProperty("_chunkHeight");
            EditorGUILayout.PropertyField(chunkHeight, new GUIContent("格子高度"));
            
            var gridRow = serializedObject.FindProperty("_gridRow");
            EditorGUILayout.PropertyField(gridRow, new GUIContent("行"));
            
            var gridCol = serializedObject.FindProperty("_gridCol");
            EditorGUILayout.PropertyField(gridCol, new GUIContent("列"));
            
            var orgPosX = serializedObject.FindProperty("_orgPosX");
            EditorGUILayout.PropertyField(orgPosX, new GUIContent("X 点"));
            
            var orgPosY = serializedObject.FindProperty("_orgPosY");
            EditorGUILayout.PropertyField(orgPosY, new GUIContent("Y 点"));
            
            var threshold = serializedObject.FindProperty("_threshold");
            EditorGUILayout.PropertyField(threshold, new GUIContent("精度"));
            
            var lineRender = serializedObject.FindProperty("_lineRender");
            EditorGUILayout.PropertyField(lineRender, new GUIContent("LineRender"));
            
            if (GUI.Button(EditorGUILayout.GetControlRect(false), "Save"))
            {
                _pathEditor.Save();
            }
            serializedObject.ApplyModifiedProperties();

            if (GUI.changed) 
                EditorUtility.SetDirty(target);
        }
        
        
        //[MenuItem("GameObject/z-增加一笔画编辑脚本", false, 0)]
        static public void AddOnePatch(MenuCommand menuCommand)
        {
            GameObject selectedObject = Selection.activeGameObject;

            if (selectedObject == null)
                return;
            
            if(selectedObject.GetComponent<OnePathEditor>() != null)
                return;

            selectedObject.AddComponent<OnePathEditor>();
        }
    }
}