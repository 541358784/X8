using System;
using DataInspector;
using DragonU3DSDK;
using UnityEditor;
using UnityEngine;

public class CodeMemInspector : EditorWindow
{
    private readonly Inspector v;
    private Vector2 scroll;
    private string inputType = "Framework.Main";

    public CodeMemInspector()
    {
        // The most safe place to initialize v is here, expecially when you need to call member functions to setup v.
        // Awake() may not be called after recompile.
        v = new Inspector();
    }

    [MenuItem("Window/CodeMemInspector")]
    public static void ShowWindow()
    {
        GetWindow<CodeMemInspector>();
    }

    public void OnGUI()
    {
        try
        {
            using (GUITools.Scroll(ref scroll))
            {
                v.Inspect("Inspector Options", "options", v.options);
                v.Inspect("选取对象", "selection", Selection.objects);
                v.Inspect("Main", "Main", typeof(Gameplay.MyMain));

                // add your object here ------------


                // ---------------------------------

                inputType = GUITools.TextField("类型", inputType);
                var t = Type.GetType(inputType);
                if (t == null)
                {
                    t = Type.GetType(string.Format("{0}, Assembly-CSharp", inputType));
                }

                v.Inspect(inputType, inputType, t);
                //             v.Inspect("RoomManager", "RoomManager", Gameplay.RoomManager.Instance);
                // v.Inspect("PopupSubSystem", "PopupSubSystem", PopupSubSystem.Instance);
                //v.Inspect("NewDecoAssetManager", "NewDecoAssetManager", NewDecoAssetManager.Instance);
                //v.Inspect("Texture2DFactory", "Texture2DFactory", Texture2DFactory.Instance);
                //v.Inspect("LevelManager", "LevelManager", Gameplay.RoomManager.Instance);
            }
        }
        catch (Exception e)
        {
            DebugUtil.LogError(e);
        }
    }

    public void OnSelectionChange()
    {
        Repaint();
    }
}