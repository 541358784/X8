using UnityEditor;
using UnityEngine;

public class HorizontalScope : GUI.Scope
{
    public HorizontalScope()
    {
        GUILayout.BeginHorizontal();
    }

    protected override void CloseScope()
    {
        GUILayout.EndHorizontal();
    }
    
    public HorizontalScope(string title, GUILayoutOption option)
    {
        GUILayout.BeginHorizontal(title, "window", new GUILayoutOption[]{option});
    }
}