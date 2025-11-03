#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Screw;
using Screw.Configs;
using UnityEditor;
using UnityEngine;

namespace LevelEditor
{
    public class CheckLevelTools
    {
        [MenuItem("Tools/Screw/CheckLevel")]
        public static void CheckLevel()
        {
            var directoryInfo = new DirectoryInfo($"{Application.dataPath}{"/Export/Configs/Screw"}");

            var files = directoryInfo.GetFiles("*.json", SearchOption.AllDirectories);

            for (var i = 0; i < files.Length; i++)
            {
                var text = File.ReadAllText(files[i].FullName);
                var levelLayout = JsonConvert.DeserializeObject<LevelLayout>(text);
                // Debug.Log("LevelT:" + levelDataT.LevelId);

                Dictionary<ColorType, int> screwCountDic = new Dictionary<ColorType, int>();
                Dictionary<ColorType, int> taskCountDic = new Dictionary<ColorType, int>();

                for (int j = 0; j < levelLayout.orders.Count; j++)
                {
                    if (!taskCountDic.ContainsKey(levelLayout.orders[j].colorType))
                    {
                        taskCountDic.Add(levelLayout.orders[j].colorType, 0);
                    }

                    taskCountDic[levelLayout.orders[j].colorType] += levelLayout.orders[j].slotCount;
                }

                for (int l = 0; l < levelLayout.layers.Count; l++)
                {
                    var layerT = levelLayout.layers[l];
                    foreach (var screw in layerT.screws)
                    {
                        if (!screwCountDic.ContainsKey(screw.colorType))
                        {
                            screwCountDic.Add(screw.colorType, 0);
                        }

                        screwCountDic[screw.colorType]++;
                    }
                }

                for (int l = 0; l < levelLayout.layers.Count; l++)
                {
                    var layerT = levelLayout.layers[l];
                    foreach (var panel in layerT.panels)
                    {
                        if (AssetDatabase
                            .AssetPathToGUID(
                                $"Assets/Export/Screw/NamedTextures/Panel/{panel.bodyImageName}.png")
                            .Length <= 0)
                        {
                            Debug.LogError(
                                $"关卡id:{levelLayout.levelId}中出现了不存在的资源:{panel.bodyImageName}.png");
                        }
                    }
                }

                if (taskCountDic.Count != screwCountDic.Count)
                {
                    Debug.LogError($"关卡id为{levelLayout.levelId} 的任务和螺丝不匹配");
                    continue;
                }

                foreach (ColorType color in Enum.GetValues(typeof(ColorType)))
                {
                    if (!taskCountDic.ContainsKey(color) && !screwCountDic.ContainsKey(color))
                        continue;
                    if (taskCountDic.ContainsKey(color) && screwCountDic.ContainsKey(color) &&
                        taskCountDic[color] == screwCountDic[color])
                        continue;
                    Debug.LogError($"关卡id为{levelLayout.levelId} 的{color.ToString()}颜色任务和螺丝数量不同");
                }
            }

            // 获取Assets文件夹下的path路径对应的文件
            string fullPath = Application.dataPath + "/Export/Configs/Screw/Game/screwgameconfig.json";
            var json = File.ReadAllText(fullPath);
            DragonPlus.Config.Screw.GameConfigManager.Instance.InitConfig(json);
            // 读取文件内容
            var levelsList = DragonPlus.Config.Screw.GameConfigManager.Instance.TableLevelsList;
            for (int i = 0; i < levelsList.Count; i++)
            {
                var levelId = levelsList[i].LevelId;

                string folderName = "Group{0}";
                if (levelId < 10000)
                {
                    folderName = string.Format(folderName, 0);
                }
                else
                {
                    int group = int.Parse(levelId.ToString()[0].ToString());
                    folderName = string.Format(folderName, group);
                }

                string folderPath = ConstConfig.FolderNameConfigPath(folderName);
                folderPath += "/"+levelId + ".json";
                
                if (AssetDatabase.AssetPathToGUID(folderPath).Length <= 0)
                {
                    Debug.LogError($"不存在关卡id为 {levelId} 的关卡");
                }
            }

            var levelsLoopList = DragonPlus.Config.Screw.GameConfigManager.Instance.TableLoopLevelsList;
            for (int i = 0; i < levelsLoopList.Count; i++)
            {
                var levelId = levelsLoopList[i].LevelId;

                if (levelId < 10000)
                {
                    Debug.LogError($"不存在关卡id为 {levelId} 的循环关卡");
                    continue;
                }
                else
                {
                    string folderName = "Group{0}";
                        int group = int.Parse(levelId.ToString()[0].ToString());
                        folderName = string.Format(folderName, group);

                    string folderPath = ConstConfig.FolderNameConfigPath(folderName);
                    folderPath += "/"+levelId + ".json";
                    
                    if (AssetDatabase.AssetPathToGUID(folderPath).Length <= 0)
                    {
                        Debug.LogError($"不存在关卡id为 {levelId} 的循环关卡");
                    }
                }

            }

            Debug.Log("Check Finish!!!!!!");
        }
    }
}
#endif