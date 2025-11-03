using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DynamicRandom.Editor
{
    public partial class DynamicWindow : EditorWindow
    {
        private const int _boardSize = 1;
        
        private Vector2 _scrollPosition = Vector2.zero;
        private Vector2Int _gridSize = Vector2Int.zero;
        private List<Vector2Int> _shapeList = new List<Vector2Int>();

        private bool _isLoadData = false;
        private string _inputText;
        [MenuItem("Tools/Dynamic", false, 5000)]
        public static void Init(MenuCommand command)
        {
            var window = EditorWindow.GetWindow<DynamicWindow>(false);
            window.minSize = new Vector2(150f, 200f);
            window.maxSize = new Vector2(600f, 1000f);
            window.titleContent = new GUIContent($"Dynamic");
            window.Show();
        }

        private void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        
        private void OnFocus()
        {
            Selection.selectionChanged -= Repaint;
        }

        private void OnLostFocus()
        {
            Selection.selectionChanged += Repaint; 
        }

        private void OnGUI()
        {
            if (EditorApplication.isPlaying)
                return;

            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
            {
                _gridSize = EditorGUILayout.Vector2IntField("格子宽高 (行 - 列)", _gridSize);

                GUILayout.Space(10);
                using (new VerticalScope("形状数据", GUILayout.Height(50f)))
                {
                    for (int i = 0; i < _shapeList.Count; i++)
                    {
                        GUILayout.BeginHorizontal();
                        {
                            bool isRemove = false;
                            if (GUILayout.Button("删除", GUILayout.Width(50)))
                            {
                                _shapeList.RemoveAt(i);
                                i--;
                                isRemove = true;
                            }
                            
                            GUILayout.Space(10);
                            
                            if(!isRemove)
                                _shapeList[i] = EditorGUILayout.Vector2IntField($"形状 {i+1}  (行 - 列)", _shapeList[i]);
                        }

                        GUILayout.EndHorizontal();
                    }
                }

                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("增加形状"))
                    {
                        Vector2Int size = Vector2Int.zero;
                        _shapeList.Add(size);
                    }
                
                    if (GUILayout.Button("生成动态格子"))
                    {
                        CreateShape();
                    }
                }
                GUILayout.EndHorizontal();
                
                GUILayout.Space(20);
                GUILayout.BeginVertical();
                {
                    _isLoadData = GUILayout.Toggle(_isLoadData, "加载数据");
                    if (_isLoadData)
                    {
                        _inputText = GUILayout.TextField(_inputText,  GUILayout.Width(400));
                        
                        if (GUILayout.Button("加载"))
                        {
                            LoadData(_inputText);
                        }
                    }
                }
                GUILayout.EndVertical();
            }

            GUILayout.EndScrollView();
        }
        
        private void OnSceneGUI(SceneView sceneView)
        {
            if(_gridSize.x <= 0 || _gridSize.y <= 0)
                return;

            DrawShape();
            
            //X -> 行
            //y -> 列
            float height = _boardSize * _gridSize.x;
            float width = _boardSize * _gridSize.y;

            Handles.color = Color.green;
            Handles.DrawWireCube(Vector3.zero, new Vector3(width, height));
            
            Vector3 orgPosition = Vector3.zero;
            orgPosition.x = -1.0f * _gridSize.y / 2;
            orgPosition.y = 1.0f * _gridSize.x / 2;
            
            Vector3 endPosition = Vector3.zero;
            endPosition.x = 1.0f * _gridSize.y / 2;
            endPosition.y = -1.0f *_gridSize.x / 2;
            
            for (var y = 0; y < _gridSize.x; y++)
            {
                Handles.DrawLine(new Vector3(orgPosition.x, orgPosition.y - y*_boardSize, 0), new Vector3(endPosition.x, orgPosition.y - y*_boardSize, 0));
            }
            
            for (var x = 0; x < _gridSize.y; x++)
            {
                Handles.DrawLine(new Vector3(orgPosition.x+x*_boardSize, orgPosition.y, 0), new Vector3(orgPosition.x+x*_boardSize, endPosition.y, 0));
            }
            Handles.DrawWireDisc(Vector3.zero,Vector3.forward, 0.05f);
            
            sceneView.Repaint();
        }
    }
}