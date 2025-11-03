using System.IO;
using LevelEditor;
using Screw.Configs;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Screw.Editor
{
    public class EditorSceneMono : UnityEditor.Editor
    {
        private const string _scenePath = "Assets/Export/Screw/Scene/";
        
        [MenuItem("Tools/Screw/Reset Mono")]
        public static void ResetMono()
        {
            // for (int levelId = 1; levelId <= 100000; levelId++)
            // {
            //     string path = _scenePath + $"Level {levelId}.unity";
            //     if (!File.Exists(path))
            //         continue;
            //
            //     var scene = EditorSceneManager.OpenScene(path);
            //     var levelDataObj = GameObject.Find("LevelData");
            //     var saveMono = levelDataObj.GetComponent<SaveLevelData>();
            //     
            //     if(saveMono == null)
            //         continue;
            //
            //     var generateMono = levelDataObj.GetComponent<GenerateLevelData>();
            //     if (generateMono == null)
            //         generateMono = levelDataObj.AddComponent<GenerateLevelData>();
            //
            //     generateMono._levelId = saveMono.levelId;
            //     generateMono._layerColors.Clear();
            //     generateMono._orders.Clear();
            //     
            //     foreach (var colorType in saveMono.layerColors)
            //     {
            //         generateMono._layerColors.Add((Configs.ColorType)colorType);
            //     }
            //     
            //     foreach (var task in saveMono.tasks)
            //     {
            //         Order order = new Order();
            //         order.slotCount = task.slotCount;
            //         order.colorType = (Configs.ColorType)task.colorType;
            //         foreach (var screwShape in task.shapes)
            //         {
            //             order.shapes.Add((Configs.ScrewShape)screwShape);
            //         }
            //         generateMono._orders.Add(order);
            //     }
            //
            //     EditorSceneManager.SaveScene(scene);
            // }
        }
        
        
        [MenuItem("Tools/Screw/Generate All Level")]
        public static void GenerateAllLevel()
        {
            for (int levelId = 1; levelId <= 100000; levelId++)
            {
                string path = _scenePath + $"Level {levelId}.unity";
                if (!File.Exists(path))
                    continue;

                var scene = EditorSceneManager.OpenScene(path);
                var levelDataObj = GameObject.Find("LevelData");
                var generateMono = levelDataObj.GetComponent<GenerateLevelData>();
                if(generateMono == null)
                    continue;

                generateMono.GenerateLevel();
                
                EditorSceneManager.CloseScene(scene, true);
            }
            
            AssetDatabase.Refresh();
        }
    }
}