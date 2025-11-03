using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TextureFormatSettings))]
public class TextureFormatSettingInspector : Editor
{
    private TextureFormatSettings targetSettings;


    public void OnEnable()
    {
        targetSettings = target as TextureFormatSettings;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("排除后缀为", TextureFormatSettingConst.ExcludePostFix);

        
        for (var i = 0; i < targetSettings.settings.Count; i++)
        {
            var setting = targetSettings.settings[i];
            using (new EditorGUILayout.HorizontalScope())
            {
                setting.Name = EditorGUILayout.TextField("种类", setting.Name);
                if (GUILayout.Button("↑") && i > 0)
                {
                    targetSettings.settings[i] = targetSettings.settings[i - 1];
                    targetSettings.settings[i - 1] = setting;
                }

                if (GUILayout.Button("↓") && i < targetSettings.settings.Count - 1)
                {
                    targetSettings.settings[i] = targetSettings.settings[i + 1];
                    targetSettings.settings[i + 1] = setting;
                }

                if (GUILayout.Button("删"))
                {
                    for (int j = i + 1; j < targetSettings.settings.Count; j++)
                    {
                        targetSettings.settings[j - 1] = targetSettings.settings[j];
                    }
                    targetSettings.settings.RemoveAt(targetSettings.settings.Count - 1);
                    setting = targetSettings.settings[i];
                }
            }
            setting.InspectorFoldOut = EditorGUILayout.Foldout(setting.InspectorFoldOut, "");
            if (setting.InspectorFoldOut)
            {
                setting.PreFix = EditorGUILayoutPopup("前缀", setting.PreFix, TextureFormatSettingConst.NameFormatPre);
                setting.PostFix = EditorGUILayoutPopup("后缀", setting.PostFix, TextureFormatSettingConst.NameFormatPost);

                setting.ImporterType = (TextureImporterType) EditorGUILayout.EnumPopup("Texture Type", setting.ImporterType);
                setting.ImporterShape = (TextureImporterShape) EditorGUILayout.EnumPopup("Texture Shape", setting.ImporterShape);
                setting.GeneratePhysicShape = EditorGUILayout.Toggle("Generate Physic Shape", setting.GeneratePhysicShape);
                setting.MaxSize = int.Parse(EditorGUILayoutPopup("Max Size", setting.MaxSize.ToString(), TextureFormatSettingConst.TextureMaxSize));
                setting.StandaloneMaxSize = int.Parse(EditorGUILayoutPopup("standalone max size", setting.StandaloneMaxSize.ToString(), TextureFormatSettingConst.TextureMaxSize));
                setting.MipMap = EditorGUILayout.Toggle("Mip Maps", setting.MipMap);
                setting.ForceAlphaSetting = EditorGUILayout.Toggle("force alpha", setting.ForceAlphaSetting);
                setting.Alpha = (TextureImporterAlphaSource)EditorGUILayout.EnumPopup("alpha source", setting.Alpha);
                
                setting.FormatIos = (TextureImporterFormat)EditorGUILayout.EnumPopup("IOS Format", setting.FormatIos);
                setting.FormatAndroid = (TextureImporterFormat)EditorGUILayout.EnumPopup("Android Format", setting.FormatAndroid);
                setting.FormatIosAlpha = (TextureImporterFormat)EditorGUILayout.EnumPopup("IOS Alpha Format", setting.FormatIosAlpha);
                setting.FormatAndroidAlpha = (TextureImporterFormat)EditorGUILayout.EnumPopup("Android Alpha Format", setting.FormatAndroidAlpha);
                setting.FormatStandalone = (TextureImporterFormat)EditorGUILayout.EnumPopup("Standalone Format", setting.FormatStandalone);
                setting.FormatStandaloneAlpha = (TextureImporterFormat)EditorGUILayout.EnumPopup("Standalone Alpha Format", setting.FormatStandaloneAlpha);
            }
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            
        }
        if (GUILayout.Button("+"))
        {
            targetSettings.settings.Add(new TextureFormatSetting(){Name = "未知"});
        }

        TextureFormatSettings.IsDirtyLock = true;
        EditorUtility.SetDirty(targetSettings);
        
        using(new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Save"))
            {
            
            }

            if (GUILayout.Button("ReImport All Textures"))
            {
            }
        }
        
    }

    private int FindIndex<T>(IReadOnlyList<T> collection, T value)
    {
        if (collection != null)
        {
            for (int i = 0; i < collection.Count; i++)
            {
                if (collection[i].Equals(value))
                {
                    return i;
                }
            }
        }
        return -1;
    }

    private string EditorGUILayoutPopup(string label, string selected, string[] displayOption)
    {
        try
        {
            var index = EditorGUILayout.Popup(label, FindIndex(displayOption, selected), displayOption);
            return displayOption[index >= 0 ? index : 0];
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        return string.Empty;
    }
}
