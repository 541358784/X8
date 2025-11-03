using UnityEditor;
using UnityEngine;

public class VerticalScope : GUI.Scope
{
    public VerticalScope(string title, float height, float width)
    {
        GUILayout.BeginVertical("", "window", new[] { GUILayout.Height(height), GUILayout.Width(width) });
    }

    public VerticalScope(string title)
    {
        GUILayout.BeginVertical(title, "window");
    }

    public VerticalScope(string title, GUIStyle style)
    {
        GUILayout.BeginVertical(title, style);
    }
    
    public VerticalScope(string title, GUILayoutOption option)
    {
        GUILayout.BeginVertical(title, "window", new GUILayoutOption[]{option});
    }
    
    private GUIStyle _style;

    public VerticalScope(GUIStyle style)
    {
        _style = style;
        // EditorGUILayout.BeginVertical(GUIStyleCache.GetStyle("HelpBoxEx"));
    }

    protected override void CloseScope()
    {
        if (_style != null)
        {
            EditorGUILayout.EndVertical();
        }
        else
        {
            GUILayout.EndHorizontal();
        }
    }
}