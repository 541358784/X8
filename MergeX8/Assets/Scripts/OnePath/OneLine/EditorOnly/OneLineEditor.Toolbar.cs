#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace OneLine.Editor
{
    partial class OneLineEditor
    {
        [SerializeField]
        private bool isEditingPoint = true;

        private static string[] ToolBarContents = new string[]
        {
            "点",
            "边",
        };

        private void DrawToolbar(SceneView sceneView)
        {
            Handles.BeginGUI();

            GUILayout.BeginArea(new Rect(0, 0, 120f, 22f * 6), GUI.skin.window);

            Texture2D texture2D = (Texture2D) EditorGUILayout.ObjectField(m_PathTexture, typeof(Texture2D), false);
            if (texture2D != m_PathTexture)
            {
                m_PathTexture = texture2D;
                NewEdit();
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("笔刷大小", GUILayout.Width(60f));
            int brushSize = EditorGUILayout.DelayedIntField(m_BrushSize);
            EditorGUILayout.EndHorizontal();

            if (brushSize != m_BrushSize && brushSize > 0) UpdateBrushSize(brushSize, true);

            DrawReset(sceneView);
            DrawSave(sceneView);

            isEditingPoint = GUILayout.Toolbar(isEditingPoint ? 0 : 1, ToolBarContents) == 0;

            GUILayout.EndArea();

            var titleRect = GUILayoutUtility.GetLastRect();
            titleRect.width  = 120f;
            titleRect.height = 22f;
            EditorGUI.LabelField(titleRect, "一笔画小游戏", GUI.skin.box);

            Handles.EndGUI();
        }

        private void DrawReset(SceneView sceneView)
        {
            bool guiEnable = GUI.enabled;
            GUI.enabled = m_PathTexture != null;
            if (GUILayout.Button("重置"))
            {
                NewEdit();
            }

            GUI.enabled = guiEnable;
        }

        private void DrawSave(SceneView sceneView)
        {
            bool guiEnable = GUI.enabled;
            GUI.enabled = CanSave();
            if (GUILayout.Button("保存"))
            {
                Save(sceneView);
            }

            GUI.enabled = guiEnable;
        }
    }
}
#endif