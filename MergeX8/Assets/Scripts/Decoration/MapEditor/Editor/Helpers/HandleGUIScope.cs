using UnityEditor;
using UnityEngine;

public class HandleGUIScope : GUI.Scope
{
    public HandleGUIScope()
    {
        Handles.BeginGUI();
    }

    protected override void CloseScope()
    {
        Handles.EndGUI();
    }
}