using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Diagnostics;
using System.Linq;
using Object = UnityEngine.Object;

public class CheckFullPath : EditorWindow
{
    private const string MenuItemName = "Tools/显示GameObject全局路径";
    

    [MenuItem(MenuItemName, false, 25)]
    public static void FindReferenceAsset()
    {
        CheckFullPath window =
            (CheckFullPath) EditorWindow.GetWindow(typeof(CheckFullPath), false,
                "GetObjectFullPath", true);
        window.Show();
    }

    private string path;
    private void OnGUI()
    {
        var selectTransform = Selection.activeTransform;
        if (selectTransform != null)
        {
            path = "";
            while (selectTransform != null)
            {
                path = "/" + selectTransform.name + path;
                selectTransform = selectTransform.parent;
            }
        }
        EditorGUILayout.TextArea(path, EditorStyles.wordWrappedLabel);
    }
}