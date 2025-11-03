using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;

namespace JungleAdventure
{
    [CustomEditor(typeof(PathMap))]
    public class PathInspector : Editor
    {
        private PathMap _pathMap;

        private SerializedProperty speedProp;
        private SerializedProperty segmentsProp;
        private ReorderableList segmentsList;


        private List<bool> showInnerListStates = new List<bool>(); // 控制每个内层列表的折叠状态

        private readonly string[] segmentNames = new[]
        {
            "p1",
            "p2",
            "cp1",
            "cp2",
        };

        private void OnEnable()
        {
            speedProp = serializedObject.FindProperty("_speed");

            segmentsProp = serializedObject.FindProperty("_segments");


            if (segmentsProp == null)
            {
                Debug.LogError("Couldn't find property '_segments'. Make sure the field is properly defined in the script.");
                return;
            }


            segmentsList = new ReorderableList(serializedObject, segmentsProp, true, true, true, true);

            segmentsList.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Segments"); };

            segmentsList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = segmentsList.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight * 3 + 10), element, GUIContent.none, true);
            };

            segmentsList.elementHeightCallback = (int index) => { return EditorGUIUtility.singleLineHeight + 3; };

            segmentsList.onAddCallback = (ReorderableList list) => { CreatePath(list); };

            segmentsList.onRemoveCallback = (ReorderableList list) =>
            {
                if (EditorUtility.DisplayDialog("Warning!", "Are you sure you want to delete the segment?", "Yes", "No"))
                {
                    var obj = GameObject.Find($"{((PathMap)target).name}/Path_{list.index}");
                    if (obj != null)
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
            var zumaPathMap = (PathMap)target;
            pathGO.transform.SetParent(zumaPathMap.transform, false);
            pathGO.transform.localPosition = Vector3.zero;


            Segment lastSegment = null;

            if (zumaPathMap._segments != null && zumaPathMap._segments.Count >= 1)
                lastSegment = zumaPathMap._segments[zumaPathMap._segments.Count - 1];

            foreach (var name in segmentNames)
            {
                var pointGO = new GameObject(name);
                pointGO.transform.SetParent(pathGO.transform, false);
                pathGO.transform.localPosition = Vector3.zero;

                if (lastSegment != null)
                    pointGO.transform.position = lastSegment.p2.transform.position;

                element.FindPropertyRelative(name).objectReferenceValue = pointGO.transform;
            }

            EditorSceneManager.MarkSceneDirty(zumaPathMap.gameObject.scene);
            SceneView.RepaintAll();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            //EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(speedProp, new GUIContent("速度"));

            var pathMap = (PathMap)target;
            if (GUILayout.Button("导出"))
            {
                pathMap.ExportPath();
            }
            //EditorGUILayout.EndHorizontal();

            segmentsList.DoLayoutList();


            DrawPoint(pathMap);
            DrawPathLength(pathMap);
            
            serializedObject.ApplyModifiedProperties();
            if (GUI.changed)
                EditorUtility.SetDirty(target);
        }

        private void DrawPoint(PathMap pathMap)
        {
            if (pathMap._points == null)
                return;

            if(showInnerListStates.Count != pathMap._points.Count)
                showInnerListStates.Clear();
            
            EditorGUILayout.LabelField("Points List", EditorStyles.boldLabel);
            while (showInnerListStates.Count < pathMap._points.Count)
            {
                showInnerListStates.Add(false);
            }

            while (showInnerListStates.Count > pathMap._points.Count)
            {
                showInnerListStates.RemoveAt(showInnerListStates.Count - 1);
            }

            // 遍历外层列表
            for (int i = 0; i < pathMap._points.Count; i++)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox); // 添加边框

                // 内层列表的折叠开关
                showInnerListStates[i] = EditorGUILayout.Foldout(showInnerListStates[i], $"List {i}", true);
                if (showInnerListStates[i])
                {
                    // 遍历内层列表
                    for (int j = 0; j < pathMap._points[i].points.Count; j++)
                    {
                        EditorGUILayout.BeginHorizontal(); // 开始一行

                        // 显示索引
                        EditorGUILayout.LabelField($"Element {j}", GUILayout.Width(70));

                        // 显示 Vector2 的 X 和 Y 字段
                        EditorGUILayout.Vector2Field(GUIContent.none, pathMap._points[i].points[j]);

                        EditorGUILayout.EndHorizontal(); // 结束一行
                    }
                }

                EditorGUILayout.EndVertical(); // 结束边框
            }
        }

        private void DrawPathLength(PathMap pathMap)
        {
            EditorGUILayout.LabelField("PathLength List", EditorStyles.boldLabel);
            for (int i = 0; i < pathMap._pathLength.Count; i++)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUILayout.FloatField($"path {i}", pathMap._pathLength[i]);
                EditorGUILayout.EndVertical(); // 结束边框
            }
        }
    }
}