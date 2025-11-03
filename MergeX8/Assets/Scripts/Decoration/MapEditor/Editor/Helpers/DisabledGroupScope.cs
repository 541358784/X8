using UnityEditor;
using UnityEngine;

public class DisabledGroupScope : GUI.Scope
{
    public DisabledGroupScope(bool disable)
    {
        EditorGUI.BeginDisabledGroup(disable);
    }

    protected override void CloseScope()
    {
        EditorGUI.EndDisabledGroup();
    }
}