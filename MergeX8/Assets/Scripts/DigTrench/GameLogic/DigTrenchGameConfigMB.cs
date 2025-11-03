using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace DigTrench
{
    [Serializable] 
    public class DigTrenchGameConfigMB:MonoBehaviour
    {
        public DigTrenchGameConfig Config;
    }
    
    
#if UNITY_EDITOR
    [CustomEditor(typeof(DigTrenchGameConfigMB))] // 将 YourScript 替换为你的脚本名称
    public class DigTrenchGameConfigEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector(); // 显示默认的检视面板字段

            DigTrenchGameConfigMB script = (DigTrenchGameConfigMB)target; // 获取目标脚本对象

            if (GUILayout.Button("自动生成SideQuest配置"))
            {
                var animationLayer = script.transform.Find("Animation");
                script.Config.SideQuestPosition.Clear();
                foreach (Transform animation in animationLayer)
                {
                    var array = animation.name.Split(',');
                    var x = array[0].ToInt();
                    var y = array[1].ToInt();
                    var pos = new Vector3Int(x, y, 0);
                    var inEndPosition = false;
                    foreach (var endPos in script.Config.EndPosition)
                    {
                        if (pos == endPos)
                        {
                            inEndPosition = true;
                            break;
                        }
                    }
                    if (!inEndPosition)
                        script.Config.SideQuestPosition.Add(pos);
                }
            }
            
            if (GUILayout.Button("保存配置"))
            {
                var fileName = script.gameObject.name + "Config.json";
                var filePath = Path.Combine(Application.dataPath , "Export","DigTrench","Configs",fileName);
                if (!File.Exists(filePath))
                {
                    File.Create(filePath);
                }
                File.WriteAllText(filePath,JsonConvert.SerializeObject(script.Config));
                AssetDatabase.Refresh();
            }

            if (GUILayout.Button("读取配置"))
            {
                var fileName = script.gameObject.name + "Config.json";
                var filePath = Path.Combine(Application.dataPath , "Export","DigTrench","Configs",fileName);
                if (File.Exists(filePath))
                {
                    var configStr = AssetDatabase.LoadAssetAtPath<TextAsset>(filePath.Replace(Application.dataPath,"Assets"));
                    var config = JsonConvert.DeserializeObject<DigTrenchGameConfig>(configStr.text);
                    script.Config = config;
                }
            }
        }
    }
    
    
    public class DigTrenchGameConfigEditorWindow : EditorWindow
    {
        [MenuItem("Tools/DigTrenchEditor")] // 这个MenuItem属性将在Unity菜单中创建一个名为"My Custom Window"的选项
        public static void ShowWindow()
        {
            GetWindow<DigTrenchGameConfigEditorWindow>("挖沟关卡编辑器");
        }

        public DigTrenchGameConfigMB LevelConfig;
        private Vector2 scrollPosition = Vector2.zero;
        void OnGUI()
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            LevelConfig = EditorGUILayout.ObjectField("目标对象:", LevelConfig, typeof(DigTrenchGameConfigMB), true) as DigTrenchGameConfigMB;
            if (LevelConfig)
            {
                Editor targetComponentEditor = Editor.CreateEditor(LevelConfig);
                targetComponentEditor.OnInspectorGUI();   
            }
            GUILayout.EndScrollView();
        }
    }
    
#endif
}