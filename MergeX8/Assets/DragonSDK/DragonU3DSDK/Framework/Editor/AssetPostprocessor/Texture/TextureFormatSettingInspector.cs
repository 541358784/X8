using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DragonU3DSDK
{
    [CustomEditor(typeof(TextureFormatSettings))]
    public class TextureFormatSettingInspector : Editor
    {
        private static readonly string[] TextureMaxSize =
        {
            "32",
            "64",
            "128",
            "256",
            "512",
            "1024",
            "2048",
            "4096",
            "8192",
        };
    
        private TextureFormatSettings targetSettings;

        private bool DocumentInspectorFoldOut = false;

        public void OnEnable()
        {
            targetSettings = target as TextureFormatSettings;
        }

        public override void OnInspectorGUI()
        {
            DocumentInspectorFoldOut = EditorGUILayout.Foldout(DocumentInspectorFoldOut, "说明");
            if (DocumentInspectorFoldOut)
            {
                EditorGUILayout.LabelField("通过正则表达式(Regex)进行匹配。\r\n" +
                                           "一个种类可以添加多组Regex,一组有多个表达式。\r\n" +
                                           "组内表达式全部匹配代表这个组匹配，最终只要一个组匹配即可。\r\n" +
                                           "种类的权重从上到下。\r\n" +
                                           "大小写不敏感。\r\n" +
                                           "创建的TextureFormatSetting.asset配置可以放在任意位置。",
                    GUILayout.MinHeight(90));
            }
            
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
                        if (targetSettings.settings.Count == 0 || targetSettings.settings.Count == i)
                        {
                            break;
                        }
                        setting = targetSettings.settings[i];
                    }
                }
                setting.InspectorFoldOut = EditorGUILayout.Foldout(setting.InspectorFoldOut, "");
                if (setting.InspectorFoldOut)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Regex");
                    if (GUILayout.Button("+"))
                    {
                        setting.RegexsOR.Add(new RegexsAnd());
                    }
                    EditorGUILayout.EndHorizontal();
                    for (int or = 0; or < setting.RegexsOR.Count; or++)
                    {
                        RegexsAnd ands = setting.RegexsOR[or];
                        ands.InspectorFoldOut = EditorGUILayout.Foldout(ands.InspectorFoldOut, $"OR_{or+1}");
                        if (ands.InspectorFoldOut)
                        {
                            EditorGUILayout.BeginHorizontal();
                            if (GUILayout.Button("+"))
                            {
                                ands.Regex.Add("");
                            }
                            if (GUILayout.Button("删"))
                            {
                                for (int j = or + 1; j < setting.RegexsOR.Count; j++)
                                {
                                    setting.RegexsOR[j - 1] = setting.RegexsOR[j];
                                }
                                setting.RegexsOR.RemoveAt(setting.RegexsOR.Count - 1);
                                if (setting.RegexsOR.Count == 0 || setting.RegexsOR.Count == or)
                                {
                                    break;
                                }
                            }
                            EditorGUILayout.EndHorizontal();
                            for (int and = 0; and < ands.Regex.Count; and++)
                            {
                                EditorGUILayout.BeginHorizontal();
                                ands.Regex[and] = EditorGUILayout.TextField($"And_{and+1}", ands.Regex[and]);
                                if (GUILayout.Button("删"))
                                {
                                    for (int j = and + 1; j < ands.Regex.Count; j++)
                                    {
                                        ands.Regex[j - 1] = ands.Regex[j];
                                    }
                                    ands.Regex.RemoveAt(ands.Regex.Count - 1);
                                    if (ands.Regex.Count == 0 || ands.Regex.Count == and)
                                    {
                                        break;
                                    }
                                }
                                EditorGUILayout.EndHorizontal();
                            }
                        }
                    }
                
                    setting.ImporterType = (TextureImporterType) EditorGUILayout.EnumPopup("Texture Type", setting.ImporterType);
                    setting.ImporterShape = (TextureImporterShape) EditorGUILayout.EnumPopup("Texture Shape", setting.ImporterShape);
                    setting.GeneratePhysicShape = EditorGUILayout.Toggle("Generate Physic Shape", setting.GeneratePhysicShape);
                    setting.MaxSize = int.Parse(EditorGUILayoutPopup("Max Size", setting.MaxSize.ToString(), TextureMaxSize));
                    setting.StandaloneMaxSize = int.Parse(EditorGUILayoutPopup("standalone max size", setting.StandaloneMaxSize.ToString(), TextureMaxSize));
                    setting.MipMap = EditorGUILayout.Toggle("Mip Maps", setting.MipMap);
                    setting.ForceAlphaSetting = EditorGUILayout.Toggle("force alpha", setting.ForceAlphaSetting);
                    setting.Alpha = (TextureImporterAlphaSource)EditorGUILayout.EnumPopup("alpha source", setting.Alpha);
                
                    setting.FormatIos = (TextureImporterFormat)EditorGUILayout.EnumPopup("IOS Format", setting.FormatIos);
                    setting.FormatIosAlpha = (TextureImporterFormat)EditorGUILayout.EnumPopup("IOS Alpha Format", setting.FormatIosAlpha);
                    setting.FormatAndroid = (TextureImporterFormat)EditorGUILayout.EnumPopup("Android Format", setting.FormatAndroid);
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
}