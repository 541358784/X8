using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;

namespace Zuma
{
    [CustomEditor(typeof(ZumaPathMap))]
    public class ZumaPathInspector : Editor
    {
        private ZumaPathMap _pathMap;
        
    private SerializedProperty pathIdProp;
    private SerializedProperty radiusProp;
    private SerializedProperty speedProp;
    private SerializedProperty segmentsProp;
    private ReorderableList segmentsList;

    private readonly string[] segmentNames = new[]
    {
        "p1",
        "p2",
        "cp1",
        "cp2",
    };
    
    private void OnEnable()
    {
        pathIdProp = serializedObject.FindProperty("_pathId");
        radiusProp = serializedObject.FindProperty("_radius");
        speedProp = serializedObject.FindProperty("_speed");
        
        segmentsProp = serializedObject.FindProperty("_segments");
        if (segmentsProp == null)
        {
            Debug.LogError("Couldn't find property '_segments'. Make sure the field is properly defined in the script.");
            return;
        }

        segmentsList = new ReorderableList(serializedObject, segmentsProp, true, true, true, true);

        segmentsList.drawHeaderCallback = (Rect rect) => 
        {
            EditorGUI.LabelField(rect, "Segments");
        };

        segmentsList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => 
        {
            var element = segmentsList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight * 3 + 10), element, GUIContent.none, true);
        };

        segmentsList.elementHeightCallback = (int index) =>
        {
            return EditorGUIUtility.singleLineHeight+3;
        };

        segmentsList.onAddCallback = (ReorderableList list) =>
        {
            CreatePath(list);
        };

        segmentsList.onRemoveCallback = (ReorderableList list) =>
        {
            if (EditorUtility.DisplayDialog("Warning!", "Are you sure you want to delete the segment?", "Yes", "No"))
            {
                var obj = GameObject.Find($"{((ZumaPathMap)target).name}/Path_{list.index}");
                if(obj != null)
                    GameObject.DestroyImmediate(obj);
                
                ReorderableList.defaultBehaviours.DoRemoveButton(list);
            }
        };
    }
    
    private void CreatePath(ReorderableList list)
    {
        var index = list.serializedProperty.arraySize;
        list.serializedProperty.arraySize++;
        list.index = index;
        var element = list.serializedProperty.GetArrayElementAtIndex(index);
            
        var pathGO = new GameObject($"Path_{index}");
        var zumaPathMap = (ZumaPathMap)target;
        pathGO.transform.SetParent(zumaPathMap.transform,false);
        pathGO.transform.localPosition = Vector3.zero;


        Segment lastSegment = null;

        if (zumaPathMap._segments != null && zumaPathMap._segments.Count >= 1)
            lastSegment = zumaPathMap._segments[zumaPathMap._segments.Count - 1];
        
        foreach (var name in segmentNames)
        {
            var pointGO = new GameObject(name);
            pointGO.transform.SetParent(pathGO.transform,false);
            pathGO.transform.localPosition = Vector3.zero;
            
            if(lastSegment != null)
                pointGO.transform.position = lastSegment.p2.transform.position;
                
            element.FindPropertyRelative(name).objectReferenceValue  = pointGO.transform;
        }
            
        EditorSceneManager.MarkSceneDirty(zumaPathMap.gameObject.scene);
        SceneView.RepaintAll();
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        //EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(pathIdProp, new GUIContent("PathId"));
        EditorGUILayout.PropertyField(radiusProp, new GUIContent("半径"));
        EditorGUILayout.PropertyField(speedProp, new GUIContent("速度"));
            
        if (GUILayout.Button("导出"))
        {
            var zumaPathMap = (ZumaPathMap)target;
            zumaPathMap.ExportPath();
        }
        //EditorGUILayout.EndHorizontal();
        
        
        segmentsList.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
            if (GUI.changed) 
                EditorUtility.SetDirty(target);
        }
    }
}