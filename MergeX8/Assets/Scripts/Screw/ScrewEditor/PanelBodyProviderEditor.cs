#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Screw
{
    [CustomEditor(typeof(PanelBodyProvider))]
    public class PanelBodyProviderEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            // 基础的Inspector面板
            DrawDefaultInspector();
     
            // 获取当前脚本实例
            PanelBodyProvider script = (PanelBodyProvider)target;
     
            // 添加一个按钮
            if (GUILayout.Button("UpdatePolygonCollider2D"))
            {
                // 执行方法
                script.UpdatePolygonCollider2D();
            }
        }
    }
}
#endif