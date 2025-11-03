using System;
using DataInspector;
using Gameplay;
using UnityEngine;

public class MBCodeMemInspector : MonoBehaviour
{
    private readonly Inspector v = new Inspector();
    private Vector2 scroll;
    private string inputType = "Framework.Main";

    public void OnGUI()
    {
        try
        {
            var style = new GUIStyle(GUI.skin.label);
            style.fontSize = 32;
            style.normal.textColor = Color.black;
            GUI.skin = new GUISkin()
            {
                textField = style,
                textArea = style,
                label = style,
                button = style,
                toggle = style,
                box = style,
            };

            GUILayout.BeginVertical(GUILayout.Width(Screen.width));
            using (GUITools.Scroll(ref scroll))
            {
                v.Inspect("Inspector Options", "options", v.options);
                inputType = GUITools.TextField("Input Type", inputType);
                var t = Type.GetType(inputType);
                v.Inspect(inputType, inputType, t);
                v.Inspect("QualitySettings", "QualitySettings", typeof(QualitySettings));
                v.Inspect("UIManager", "UIManager", UIManager.Instance);
            }

            GUILayout.EndVertical();
        }
        catch (Exception e)
        {
            DragonU3DSDK.DebugUtil.LogError(e);
        }
    }
}