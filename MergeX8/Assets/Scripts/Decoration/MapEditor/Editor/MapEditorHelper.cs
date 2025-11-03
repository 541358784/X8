// using UnityEngine;
// using UnityEngine.U2D;
// using UnityEditor;
// using UnityEditor.U2D;
// using DragonPlus.Config;
// using System.Collections;
// using System.Collections.Generic;
// using System.IO;
// using System;
// using Newtonsoft.Json;
// using UnityEngine.Tilemaps;

// namespace DragonPlus
// {
//     public class MapEditorHelper : MonoBehaviour
//     {
//         public const int BUILDING_POINT_CANDY = 3100010;
//         public const int BUILDING_POINT_CANDY_UPGRADE = 3100020;
//         public const int BUILDING_POINT_FRIEND_TREE = 3100030;

//         static int MIN_BUILDING_ID = 1000000;
//         static string PATH = "MapEditorCache";
//         static string TEMP_PATH = "MapEditorCacheTemp";

//         [MenuItem("Helper/MapEditor/Unload")]
//         public static void Unload()
//         {
//             PureUnload();
//         }

//         [MenuItem("Helper/MapEditor/UnloadBuilding")]
//         public static void UnloadBuilding()
//         {
//             PureUnloadBuilding();
//         }

//         /*         [MenuItem("Helper/MapEditor/Load")]
//                 public static void Load()
//                 {
//                     PureUnload();
//                     PureLoad(PATH);
//                 }

//                 [MenuItem("Helper/MapEditor/Save")]
//                 public static void Save()
//                 {
//                     PureSave(PATH);
//                 } */

//         /*         [MenuItem("Helper/MapEditor/Load(Temp)")]
//                 public static void LoadTemp()
//                 {
//                     PureUnload();
//                     PureLoad(TEMP_PATH);
//                 }

//                 [MenuItem("Helper/MapEditor/Save(Temp)")]
//                 public static void SaveTemp()
//                 {
//                     PureSave(TEMP_PATH);
//                 }
//          */
//         public static bool IsUpgrade(int buildingPointId)
//         {
//             return buildingPointId == BUILDING_POINT_CANDY || buildingPointId == BUILDING_POINT_CANDY_UPGRADE || buildingPointId == BUILDING_POINT_FRIEND_TREE;
//         }

//         [MenuItem("Helper/MapEditor/Load(Config)")]
//         public static void LoadByConfig()
//         {
//             PureUnload();

//             var areaParentRoot = GameObject.Find("MapLayer/Viewport/Content/Areas");
//             if (areaParentRoot)
//             {
//                 var decorationConfigManager = new DecorationConfigManager();
//                 decorationConfigManager.InitConfigs(true);

//                 var prefabDict = new Dictionary<string, bool>();

//                 foreach (Transform areaParent in areaParentRoot.transform)
//                 {
//                     if (int.TryParse(areaParent.name, out var areaId))
//                     {
//                         var prefabPath = string.Format("Assets/Export/Prefabs/Decoration/Area/{0}.prefab", areaId);
//                         var areaPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
//                         if (areaPrefab)
//                         {
//                             var area = PrefabUtility.InstantiatePrefab(areaPrefab, areaParent) as GameObject;
//                             foreach (Transform node in area.transform)
//                             {
//                                 if (int.TryParse(node.name, out var nodeId))
//                                 {
//                                     var nodeConfig = decorationConfigManager.GetNodeConfig(nodeId);
//                                     if (nodeConfig != null)
//                                     {
//                                         var itemIdList = new List<int>();

//                                         var isUpgrade = IsUpgrade(nodeId);

//                                         if (nodeConfig.defaultItem > 0)
//                                         {
//                                             /* if (isUpgrade)
//                                             {
//                                                 var buildConfig = decorationConfigManager.GetItemConfig(buildingPointConfig.defaultItem);
//                                                 while (buildConfig != null)
//                                                 {
//                                                     // Debug.LogError("buildConfig.id: " + buildConfig.id);
//                                                     buildingIdList.Add(buildConfig.id);
//                                                     buildConfig = decorationConfigManager.GetItemConfig(buildConfig.buildingNextLevel);
//                                                 }
//                                             }
//                                             else */
//                                             {
//                                                 itemIdList.Add(nodeConfig.defaultItem);
//                                             }
//                                         }

//                                         if (nodeConfig.itemList != null)
//                                         {
//                                             itemIdList.AddRange(nodeConfig.itemList);
//                                         }

//                                         foreach (var itemId in itemIdList)
//                                         {
//                                             var item = decorationConfigManager.GetItemConfig(itemId);
//                                             if (item != null)
//                                             {
//                                                 var buildingPrefabPath = string.Format("Assets/Export/Prefabs/Decoration/Building/{0}/{1}.prefab", areaId, item.id);
//                                                 if (!prefabDict.ContainsKey(buildingPrefabPath))
//                                                 {
//                                                     prefabDict[buildingPrefabPath] = true;
//                                                     var itemPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(buildingPrefabPath);
//                                                     // Debug.LogError("areaId: " + areaId + ", buildingPointId: " + buildingPointId + ", buildingId: " + buildingId + ", buildingPrefabPath: " + buildingPrefabPath + ", buildingPrefab: " + buildingPrefab);
//                                                     if (itemPrefab == null)
//                                                     {
//                                                         var newItem = new GameObject(item.id.ToString());
//                                                         var tip = new GameObject("Tip");
//                                                         tip.transform.SetParent(newItem.transform);
//                                                         tip.transform.localPosition = Vector3.zero;
//                                                         var sprite = new GameObject("Sprite");
//                                                         sprite.transform.SetParent(newItem.transform);
//                                                         sprite.transform.localPosition = Vector3.zero;
//                                                         sprite.transform.SetParent(newItem.transform);
//                                                         sprite.AddComponent<SpriteRenderer>();
//                                                         var saveSuccess = false;
//                                                         itemPrefab = PrefabUtility.SaveAsPrefabAsset(newItem, buildingPrefabPath, out saveSuccess);
//                                                         if (saveSuccess)
//                                                         {
//                                                             Debug.LogError("itemPrefab 创建成功");
//                                                         }
//                                                         else
//                                                         {
//                                                             Debug.LogError("itemPrefab 创建失败");
//                                                         }
//                                                     }

//                                                     PrefabUtility.InstantiatePrefab(itemPrefab, node);
//                                                 }
//                                             }
//                                             else
//                                             {
//                                                 Debug.LogError("itemConfig not exist: " + itemId);
//                                             }
//                                         }
//                                     }
//                                 }
//                             }
//                         }
//                         else
//                         {
//                             Debug.LogError(string.Format("not found prefab: [{0}]", prefabPath));
//                         }
//                     }
//                 }
//             }
//         }
//         /* 
//                 [MenuItem("Helper/层级工具/所有")]
//                 public static void GenerateZ()
//                 {
//                     PureGenerateZ(true, true, true, true);
//                 }

//                 [MenuItem("Helper/层级工具/公用地表")]
//                 public static void GenerateCommonSurfaceZ()
//                 {
//                     PureGenerateZ(generateCommonSurface: true);
//                 }

//                 [MenuItem("Helper/层级工具/建筑")]
//                 public static void GenerateBuildingZ()
//                 {
//                     PureGenerateZ(generateBuilding: true);
//                 }

//                 [MenuItem("Helper/层级工具/区域地表")]
//                 public static void GenerateSurfaceZ()
//                 {
//                     PureGenerateZ(generateSurface: true);
//                 }
//          */
//         // [MenuItem("Helper/保存预制件/All")]
//         // public static void SaveAllPrefab ()
//         // {
//         //     SaveBuildingPrefab();
//         //     SaveAreaPrefab();
//         // }

//         [MenuItem("Helper/保存预制件/Area")]
//         public static void SaveAreaPrefab()
//         {
//             var areaParentRoot = GameObject.Find("MapLayer/Viewport/Content/Areas");
//             if (areaParentRoot)
//             {
//                 foreach (Transform areaParent in areaParentRoot.transform)
//                 {
//                     for (int i = 0; i < areaParent.childCount; ++i)
//                     {
//                         var area = areaParent.GetChild(i);
//                         // Debug.LogError("area.name: " + area.name);
//                         if (int.TryParse(area.name, out var areaId)) // building
//                         {
//                             PrefabUtility.ApplyPrefabInstance(area.gameObject, InteractionMode.AutomatedAction);
//                         }
//                     }
//                 }
//             }
//         }

//         [MenuItem("Helper/保存预制件/Building")]
//         public static void SaveBuildingPrefab()
//         {
//             var areaParentRoot = GameObject.Find("MapLayer/Viewport/Content/Areas");
//             if (areaParentRoot)
//             {
//                 var decorationConfigManager = new DecorationConfigManager();
//                 decorationConfigManager.InitConfigs(true);

//                 var saveCount = 0;

//                 foreach (Transform areaParent in areaParentRoot.transform)
//                 {
//                     for (int i = 0; i < areaParent.childCount; ++i)
//                     {
//                         var area = areaParent.GetChild(i);
//                         // Debug.LogError("area.name: " + area.name);
//                         if (int.TryParse(area.name, out var areaId)) // building
//                         {
//                             for (int j = 0; j < area.transform.childCount; ++j)
//                             {
//                                 var buildingPoint = area.transform.GetChild(j);
//                                 // Debug.LogError("buildingPoint.name: " + buildingPoint.name);
//                                 if (int.TryParse(buildingPoint.name, out var buidingPointId))
//                                 {
//                                     foreach (Transform building in buildingPoint)
//                                     {
//                                         if (int.TryParse(building.name, out var buildingId))
//                                         {
//                                             var buildingConfig = decorationConfigManager.GetItemConfig(buildingId);
//                                             if (buildingConfig != null)
//                                             {
//                                                 // Debug.LogError("[" + saveCount + "] buildingId: " + buildingId + ", " + PrefabUtility.HasPrefabInstanceAnyOverrides(building.gameObject, false));
//                                                 if (PrefabUtility.HasPrefabInstanceAnyOverrides(building.gameObject, false))
//                                                 {
//                                                     ++saveCount;
//                                                     Debug.Log("[" + saveCount + "] buildingId: " + buildingId);
//                                                     PrefabUtility.ApplyPrefabInstance(building.gameObject, InteractionMode.AutomatedAction);
//                                                 }
//                                                 // PrefabUtility.SavePrefabAsset(building.gameObject);
//                                                 // PrefabUtility.ApplyPrefabInstance(building.gameObject, InteractionMode.AutomatedAction);
//                                                 // PrefabUtility.ApplyPropertyOverride()
//                                             }
//                                         }
//                                     }
//                                 }
//                             }
//                         }
//                     }
//                 }
//             }
//         }

//         [MenuItem("Helper/检查预制件")]
//         public static void CheckPrefab()
//         {
//             var decorationConfigManager = new DecorationConfigManager();
//             decorationConfigManager.InitConfigs(true);

//             // var checkBuildingIdList = new List<int>();

//             var areaParentRoot = GameObject.Find("MapLayer/Viewport/Content/Areas");
//             if (areaParentRoot)
//             {
//                 foreach (Transform areaParent in areaParentRoot.transform)
//                 {
//                     for (int i = 0; i < areaParent.childCount; ++i)
//                     {
//                         var area = areaParent.GetChild(i);
//                         // Debug.LogError("area.name: " + area.name);
//                         if (int.TryParse(area.name, out var areaId)) // building
//                         {
//                             for (int j = 0; j < area.transform.childCount; ++j)
//                             {
//                                 var buildingPoint = area.transform.GetChild(j);
//                                 // Debug.LogError("buildingPoint.name: " + buildingPoint.name);
//                                 if (int.TryParse(buildingPoint.name, out var buildingPointId))
//                                 {
//                                     var buildingPointConfig = decorationConfigManager.GetNodeConfig(buildingPointId);
//                                     if (buildingPointConfig != null)
//                                     {
//                                         // if (buildingPointConfig.defaultItem > 0)
//                                         //     checkBuildingIdList.Add(buildingPointConfig.defaultItem);
//                                         // if (buildingPointConfig.itemList != null)
//                                         //     foreach (var strBuildingId in buildingPointConfig.itemList)
//                                         //         checkBuildingIdList.Add(int.Parse(strBuildingId));
//                                     }
//                                     else
//                                     {
//                                         if (buildingPointId >= 3000000 && buildingPointId < 4000000)
//                                             Debug.LogError(string.Format("多余的BuildingPoint: {0} in area {1}", buildingPointId, areaId));
//                                     }
//                                 }
//                             }
//                         }
//                     }
//                 }
//             }

//             foreach (var buildingConfig in decorationConfigManager.ItemConfigList)
//             {
//                 // var buildingConfig = decorationConfigManager.GetBuildingConfig(buildingId);
//                 // if (buildingConfig != null)
//                 {
//                     var buildingPrefabPath = string.Format("Assets/Export/{0}.prefab", buildingConfig.prefabPath[0]);
//                     var buildingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(buildingPrefabPath);
//                     if (buildingPrefab == null)
//                         Debug.LogError(string.Format("未找到预制件：{0}", buildingPrefabPath));
//                 }
//                 // else
//                 // {
//                 //     Debug.LogError(string.Format("未找到Building配置：{0}", buildingId));
//                 // }
//             }
//         }

//         [MenuItem("Helper/修复Building资源引用")]
//         public static void CheckBuildingReferenceBrushResource()
//         {
//             var root = GameObject.Find("MapLayer/Viewport/Content");
//             if (root)
//             {
//                 var arr = root.GetComponentsInChildren<SpriteRenderer>();
//                 var pathDict = new Dictionary<string, bool>();
//                 foreach (var spriteRenderer in arr)
//                 {
//                     var sprite = spriteRenderer.sprite;
//                     if (sprite != null)
//                     {
//                         var path = AssetDatabase.GetAssetPath(sprite);
//                         if (path.Contains("Brush"))
//                         {
//                             var fileName = Path.GetFileName(path);
//                             var newPath = "Assets/Res/Buildings/999/br_" + fileName;
//                             if (!File.Exists(newPath))
//                                 AssetDatabase.CopyAsset(path, newPath);
//                             var newSprite = AssetDatabase.LoadAssetAtPath(newPath, typeof(Sprite)) as Sprite;
//                             spriteRenderer.sprite = newSprite;
//                             // pathDict[path] = true;

//                             var nodePath = GetPath(spriteRenderer.transform);
//                             nodePath = nodePath.Replace("DecoSceneRoot/WorldMapIso/MapLayer/Viewport/Content/", "");
//                             // path = path.Replace("Assets/Res/Brush/", "");
//                             Debug.LogError(string.Format("{0} --> {1}", nodePath, path));
//                         }
//                     }
//                 }

//                 // foreach (var item in pathDict)
//                 // {
//                 //     var path = item.Key;
//                 //     var fileName = Path.GetFileName(path);
//                 //     var newPath = "Assets/Res/Buildings/999/br_" + fileName;
//                 //     if (!File.Exists(newPath))
//                 //         AssetDatabase.CopyAsset(path, newPath);


//                 // }
//             }
//         }

//         private static void PureLoad(string path)
//         {
//             var areaParentRoot = GameObject.Find("WorldMap/MapLayer/Viewport/Content/Areas");
//             if (areaParentRoot)
//             {
//                 var cache = Resources.Load<MapEditorCache>(path);
//                 foreach (Transform areaParent in areaParentRoot.transform)
//                 {
//                     if (int.TryParse(areaParent.name, out var areaId))
//                     {
//                         var areaPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(string.Format("Assets/Export/Prefabs/Decoration/Area/{0}.prefab", areaId));
//                         var area = PrefabUtility.InstantiatePrefab(areaPrefab, areaParent) as GameObject;
//                         foreach (Transform node in area.transform)
//                         {
//                             if (int.TryParse(node.name, out var nodeId))
//                             {
//                                 var data = cache.buildingPointCacheList.Find((v) => v.buildingPointId == nodeId);
//                                 if (data != null)
//                                 {
//                                     var itemPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(string.Format("Assets/Export/Prefabs/Decoration/Building/{0}/{1}.prefab", areaId, data.buildingId));
//                                     PrefabUtility.InstantiatePrefab(itemPrefab, node);
//                                 }
//                                 else
//                                 {
//                                     Debug.LogError("未找到node缓存的数据");
//                                 }
//                             }
//                             else
//                             {
//                                 Debug.LogError($"未解析出NodeId:{node.name}");
//                             }
//                         }
//                     }
//                     else
//                     {
//                         Debug.LogError("未解析出AreaId:{areaParent.name}");
//                     }
//                 }
//             }
//             else
//             {
//                 Debug.LogError("未找到WorldMap层级 WorldMap/MapLayer/Viewport/Content/Areas");
//             }
//         }

//         private static void PureUnload()
//         {
//             var areaParentRoot = GameObject.Find("MapLayer/Viewport/Content/Areas");
//             if (areaParentRoot)
//             {
//                 foreach (Transform areaParent in areaParentRoot.transform)
//                 {
//                     var area = areaParent.Find(areaParent.name);
//                     if (area != null)
//                         try
//                         {
//                             GameObject.DestroyImmediate(area.gameObject);
//                         }
//                         catch (System.Exception e)
//                         {
//                             Debug.LogError(e);
//                         }
//                 }
//             }
//         }

//         private static void PureUnloadBuilding()
//         {
//             var areaParentRoot = GameObject.Find("MapLayer/Viewport/Content/Areas");
//             if (areaParentRoot)
//             {
//                 var decorationConfigManager = new DecorationConfigManager();
//                 decorationConfigManager.InitConfigs(true);

//                 foreach (Transform areaParent in areaParentRoot.transform)
//                 {
//                     if (int.TryParse(areaParent.name, out var areaId))
//                     {
//                         var area = areaParent.Find(areaParent.name);
//                         foreach (Transform buildingPoint in area.transform)
//                         {
//                             if (int.TryParse(buildingPoint.name, out var buildingPointId))
//                             {
//                                 var isUpgrade = buildingPointId == BUILDING_POINT_CANDY || buildingPointId == BUILDING_POINT_CANDY_UPGRADE || buildingPointId == BUILDING_POINT_FRIEND_TREE;
//                                 var buildingList = new List<Transform>();
//                                 foreach (Transform building in buildingPoint)
//                                 {
//                                     if (int.TryParse(building.name, out var buildingId))
//                                         if (isUpgrade || decorationConfigManager.GetItemConfig(buildingId) != null)
//                                             buildingList.Add(building);
//                                 }
//                                 foreach (var building in buildingList)
//                                 {
//                                     try
//                                     {
//                                         GameObject.DestroyImmediate(building.gameObject);
//                                     }
//                                     catch (System.Exception e)
//                                     {
//                                         Debug.LogError(e);
//                                     }
//                                 }
//                             }
//                         }
//                     }
//                 }
//             }
//         }

//         public static void PureSave(string path)
//         {
//             var areaParentRoot = GameObject.Find("MapLayer/Viewport/Content/Areas");
//             if (areaParentRoot)
//             {
//                 var cache = Resources.Load<MapEditorCache>(path);
//                 cache.buildingPointCacheList.Clear();
//                 foreach (Transform areaParent in areaParentRoot.transform)
//                 {
//                     for (int i = 0; i < areaParent.childCount; ++i)
//                     {
//                         var area = areaParent.GetChild(i);
//                         // Debug.LogError("area.name: " + area.name);
//                         if (int.TryParse(area.name, out var areaId))
//                         {
//                             for (int j = 0; j < area.transform.childCount; ++j)
//                             {
//                                 var buildingPoint = area.transform.GetChild(j);
//                                 // Debug.LogError("buildingPoint.name: " + buildingPoint.name);
//                                 if (int.TryParse(buildingPoint.name, out var buidingPointId))
//                                 {
//                                     for (int k = 0; k < buildingPoint.transform.childCount; ++k)
//                                     {
//                                         var mayBuilding = buildingPoint.transform.GetChild(k);
//                                         // Debug.LogError("mayBuilding.name: " + mayBuilding.name);
//                                         if (int.TryParse(mayBuilding.name, out var buildingId))
//                                         {
//                                             // Debug.LogError("buildingId: " + buildingId);
//                                             if (buildingId > MIN_BUILDING_ID)
//                                             {
//                                                 cache.Add(buidingPointId, buildingId);
//                                             }
//                                         }
//                                     }
//                                 }
//                             }
//                         }
//                     }
//                 }
//             }
//         }

//         public static void PureGenerateZ(
//             bool generateCommonSurface = false,
//             bool generateBuilding = false,
//             bool generateSurface = false,
//             bool generateAreaAnchor = false,
//             bool repairIgnoreBuilding = false)
//         {
//             /* if (generateCommonSurface)
//             {
//                 var commonSurfaceParent = GameObject.Find("MapLayer/Viewport/Content/Surface").transform;
//                 foreach (Transform grid in commonSurfaceParent)
//                 {
//                     var gridZ = grid.position.z;
//                     for (int i = 0; i < grid.childCount; ++i)
//                     {
//                         var tilemap = grid.GetChild(i);
//                         tilemap.position = DecorationPropertyComponent.ConvertCommonSurfacePosition(tilemap.position, i, gridZ);
//                     }
//                 }
//             }

//             var areaParentRoot = GameObject.Find("MapLayer/Viewport/Content/Areas");
//             if (areaParentRoot)
//             {
//                 foreach (Transform areaParent in areaParentRoot.transform)
//                 {
//                     var areaBaseAnchor = areaParent.Find("BaseAnchor");
//                     var areaMaxAnchor = areaParent.Find("MaxAnchor");
//                     var parentAreaId = int.Parse(areaParent.name);

//                     if (generateAreaAnchor)
//                     {
//                         CalculateAreaRect(areaParent, out var min, out var max);
//                         var centerX = (max.x + min.x) / 2;

//                         var baseProperty = areaBaseAnchor.GetComponent<DecorationPropertyComponent>();
//                         if (!baseProperty || !baseProperty.IsIgnore)
//                             areaBaseAnchor.position = new Vector2(centerX, min.y);

//                         var maxProperty = areaMaxAnchor.GetComponent<DecorationPropertyComponent>();
//                         if (!maxProperty || !maxProperty.IsIgnore)
//                             areaMaxAnchor.position = new Vector2(centerX, max.y);
//                     }

//                     var baseAnchorPosition = DecorationPropertyComponent.ConvertBaseAnchorPosition(parentAreaId, areaBaseAnchor.position);
//                     var maxAnchorPosition = DecorationPropertyComponent.ConvertMaxAnchorPosition(baseAnchorPosition, areaMaxAnchor.position); ;
//                     for (int i = 0; i < areaParent.childCount; ++i)
//                     {
//                         var area = areaParent.GetChild(i);
//                         // Debug.LogError("area.name: " + area.name);
//                         if (int.TryParse(area.name, out var areaId)) // building
//                         {
//                             if (generateAreaAnchor)
//                             {
//                                 areaBaseAnchor.position = baseAnchorPosition;
//                                 areaMaxAnchor.position = maxAnchorPosition;
//                             }

//                             if (!generateBuilding)
//                                 continue;

//                             for (int j = 0; j < area.transform.childCount; ++j)
//                             {
//                                 var buildingPoint = area.transform.GetChild(j);
//                                 // Debug.LogError("buildingPoint.name: " + buildingPoint.name);
//                                 if (int.TryParse(buildingPoint.name, out var buidingPointId))
//                                 {
//                                     var decorationProperty = buildingPoint.GetComponent<DecorationPropertyComponent>();
//                                     if (decorationProperty != null)
//                                     {
//                                         if (decorationProperty.IsSupportChildProperty)
//                                         {
//                                             foreach (Transform building in buildingPoint)
//                                                 GenerateZ(areaId, baseAnchorPosition, maxAnchorPosition, building, decorationProperty, true);
//                                         }
//                                         else if (decorationProperty.IsDiscrete)
//                                         {
//                                             foreach (Transform building in buildingPoint)
//                                                 GenerateZ(areaId, baseAnchorPosition, maxAnchorPosition, building, decorationProperty, false);
//                                         }
//                                         else if (decorationProperty.IsGround)
//                                         {
//                                             buildingPoint.position = DecorationPropertyComponent.ConvertGroundPosition(areaId, maxAnchorPosition, buildingPoint.position);
//                                         }
//                                         else if (!decorationProperty.IsIgnore)
//                                         {
//                                             buildingPoint.position = DecorationPropertyComponent.ConvertBuildingPosition(areaId, baseAnchorPosition, buildingPoint.position);
//                                         }
//                                         else if (repairIgnoreBuilding)
//                                         {
//                                             buildingPoint.position = DecorationPropertyComponent.ConvertBuildingPosition(areaId, baseAnchorPosition, buildingPoint.position);
//                                         }
//                                     }
//                                     else
//                                     {
//                                         buildingPoint.position = DecorationPropertyComponent.ConvertBuildingPosition(areaId, baseAnchorPosition, buildingPoint.position);
//                                     }
//                                 }
//                             }
//                         }
//                         else // surface
//                         {
//                             if (!generateSurface)
//                                 continue;

//                             var decorationProperty = area.GetComponent<DecorationPropertyComponent>();
//                             if (decorationProperty && decorationProperty.IsIgnore)
//                                 continue;

//                             for (int j = 0; j < area.transform.childCount; ++j)
//                             {
//                                 var tilemap = area.transform.GetChild(j);
//                                 tilemap.position = DecorationPropertyComponent.ConvertSurfacePosition(parentAreaId, baseAnchorPosition, tilemap.position, j);
//                             }
//                         }
//                     }
//                 }
//             } */
//         }

//         private static void CalculateAreaRect(Transform areaRoot, out Vector2 min, out Vector2 max)
//         {
//             min = new Vector2(int.MaxValue, int.MaxValue);
//             max = new Vector2(int.MinValue, int.MinValue);

//             var spriteArray = areaRoot.GetComponentsInChildren<SpriteRenderer>(true);
//             foreach (var spriteRenderer in spriteArray)
//             {
//                 if (spriteRenderer.sprite != null)
//                 {
//                     var spritePosition = spriteRenderer.transform.position;
//                     var spriteSize = spriteRenderer.sprite.bounds.size;
//                     var spriteMin = spritePosition - spriteSize / 2;
//                     var spriteMax = spritePosition + spriteSize / 2;
//                     min.x = Math.Min(min.x, spriteMin.x);
//                     min.y = Math.Min(min.y, spriteMin.y);
//                     max.x = Math.Max(max.x, spriteMax.x);
//                     max.y = Math.Max(max.y, spriteMax.y);
//                 }
//             }
//         }

//         /* public static void GenerateZ(int areaId, Vector3 baseAnchorPosition, Vector3 maxAnchorPosition, Transform obj, DecorationPropertyComponent property, bool isSupportChildProperty)
//         {
//             if (isSupportChildProperty)
//             {
//                 var tempProperty = obj.GetComponent<DecorationPropertyComponent>();
//                 if (tempProperty != null)
//                 {
//                     if (tempProperty.IsDiscrete)
//                     {
//                         for (int i = 0; i < obj.childCount; ++i)
//                         {
//                             var child = obj.GetChild(i);
//                             var childProperty = child.GetComponent<DecorationPropertyComponent>();
//                             if (childProperty != null)
//                                 GenerateZ(areaId, baseAnchorPosition, maxAnchorPosition, child, childProperty, true);
//                             else
//                                 GenerateDiscreteChildZ(areaId, baseAnchorPosition, child, tempProperty);
//                         }
//                     }
//                     else// if (tempProperty.IsGround)
//                     {
//                         GenerateZ(areaId, baseAnchorPosition, maxAnchorPosition, obj, property, false);
//                         // for (int i = 0; i < obj.childCount; ++i)
//                         //     GenerateGroundChildZ(obj.GetChild(i));
//                     }
//                 }
//                 else
//                 {
//                     GenerateZ(areaId, baseAnchorPosition, maxAnchorPosition, obj, property, false);
//                 }
//             }
//             else
//             {
//                 if (property.IsDiscrete)
//                 {
//                     if (obj.childCount == 0)
//                         GenerateDiscreteChildZ(areaId, baseAnchorPosition, obj, property);
//                     else
//                         for (int i = 0; i < obj.childCount; ++i)
//                             GenerateDiscreteChildZ(areaId, baseAnchorPosition, obj.GetChild(i), property);
//                 }
//                 else if (property.IsGround)
//                 {
//                     if (obj.childCount == 0)
//                         GenerateGroundChildZ(areaId, maxAnchorPosition, obj);
//                     else
//                         for (int i = 0; i < obj.childCount; ++i)
//                             GenerateGroundChildZ(areaId, maxAnchorPosition, obj.GetChild(i));
//                 }
//             }
//         } */

//         /* public static void GenerateDiscreteChildZ(int areaId, Vector3 baseAnchorPosition, Transform child, DecorationPropertyComponent property)
//         {
//             var childPosition = child.position;
//             var z = 0.0f;

//             var childSprite = property.IsUseChildSprite ? child.GetComponentInChildren<SpriteRenderer>() : child.GetComponent<SpriteRenderer>();
//             if (childSprite != null)
//             {
//                 if (property.IsAnchorBottom)
//                 {
//                     z = DecorationPropertyComponent.ConvertBuildingZ(areaId, baseAnchorPosition, childPosition.y - childSprite.bounds.size.y * 0.5f);
//                 }
//                 else if (property.IsAnchorTop)
//                 {
//                     z = DecorationPropertyComponent.ConvertBuildingZ(areaId, baseAnchorPosition, childPosition.y + childSprite.bounds.size.y * 0.5f);
//                 }
//                 else
//                 {
//                     z = DecorationPropertyComponent.ConvertBuildingZ(areaId, baseAnchorPosition, childPosition.y + childSprite.bounds.size.y * (property.AnchorValue - 0.5f));
//                 }
//             }
//             else
//             {
//                 z = DecorationPropertyComponent.ConvertBuildingZ(areaId, baseAnchorPosition, childPosition.y);
//             }

//             child.position = new Vector3(child.position.x, child.position.y, z);
//         } */

//         /*         public static void GenerateGroundChildZ(int areaId, Vector3 maxAnchorPosition, Transform child)
//                 {
//                     child.position = DecorationPropertyComponent.ConvertGroundPosition(areaId, maxAnchorPosition, child.position);
//                 } */

//         private static String GetPath(Transform t)
//         {
//             var name = t.name;
//             var parent = t.parent;
//             while (parent != null)
//             {
//                 name = parent.name + "/" + name;
//                 parent = parent.parent;
//             }
//             return name;
//         }
//         /* 
//                 [MenuItem("Helper/层级工具/生成区域BaseAnchor层级")]
//                 public static void ResetAreaBaseZ()
//                 {
//                     PureGenerateZ(generateAreaAnchor: true);
//                 }

//                 [MenuItem("Helper/层级工具/修复Ignore的建筑层级")]
//                 public static void RepairIgnoreBuildingZ()
//                 {
//                     PureGenerateZ(repairIgnoreBuilding: true);
//                 }

//                 [MenuItem("Helper/层级工具/查看区域基础层级")]
//                 public static void PopupAreaBaseZ()
//                 {
//                     var dict = DecorationPropertyComponent.AreaBaseZDict;
//                     var outInfo = "";
//                     foreach (var item in dict)
//                     {
//                         outInfo += string.Format("area[{0}] = [{1}, {2}]\n", item.Key, item.Value, item.Value + 100);
//                     }

//                     EditorUtility.DisplayDialog("区域层级", outInfo, "ok");
//                 }

//                 [MenuItem("Helper/层级工具/区域层级错误检测")]
//                 public static void CheckAreaBuildingZ()
//                 {
//                     var root = GameObject.Find("MapLayer/Viewport/Content");
//                     if (root)
//                     {
//                         var spriteRendererList = DecorationPropertyComponent.GetSortSpriteRendererList(root);
//                         // var preAreaId = -1;
//                         foreach (var spriteRenderer in spriteRendererList)
//                         {
//                             var areaId = ParseAreaId(spriteRenderer.transform);
//                             if (!DecorationPropertyComponent.CheckAreaZ(areaId, spriteRenderer.transform.position.z, out var err))
//                             {
//                                 var path = DecorationPropertyComponent.GetPath(spriteRenderer.transform);
//                                 path = path.Replace("DecoSceneRoot/WorldMapIso/MapLayer/Viewport/Content/", "");
//                                 Debug.LogError(string.Format("[{0}]层级错误： {1}", path, err));
//                             }
//                         }
//                     }
//                 }

//                 [MenuItem("Helper/层级工具/批量缩小层级(10->0.0001)")]
//                 public static void ScaleZ()
//                 {
//                     var obj = Selection.activeGameObject;
//                     if (obj != null)
//                     {
//                         // Debug.LogError("select: " + obj.name);
//                         var spriteRendererArray = obj.GetComponentsInChildren<SpriteRenderer>(true);
//                         foreach (var spriteRenderer in spriteRendererArray)
//                         // for (var i = 0; i < obj.transform.childCount; ++i)
//                         {
//                             // var spriteRenderer = obj.transform.GetChild(i);
//                             var spriteObject = spriteRenderer.gameObject;
//                             // Debug.LogError("spriteObject: " + spriteObject.name + ", " + spriteObject.transform.localPosition);
//                             spriteObject.transform.localPosition = new Vector3(
//                                 spriteObject.transform.localPosition.x,
//                                 spriteObject.transform.localPosition.y,
//                                 spriteObject.transform.localPosition.z * 0.00001f);
//                         }
//                     }
//                     else
//                     {
//                         Debug.LogError("需要选中一个节点");
//                     }
//                 }

//                 [MenuItem("Helper/层级工具/批量整理子节点层级")]
//                 public static void SortZ()
//                 {
//                     var obj = Selection.activeGameObject;
//                     if (obj != null)
//                     {
//                         var diff = 0.0002f;
//                         if (obj.transform.childCount > 100)
//                             diff = 0.00002f;
//                         for (var i = 0; i < obj.transform.childCount; ++i)
//                         {
//                             var child = obj.transform.GetChild(i);
//                             child.transform.localPosition = new Vector3(
//                                 child.transform.localPosition.x,
//                                 child.transform.localPosition.y,
//                                 -i * diff
//                             );
//                         }
//                         EditorUtility.SetDirty(obj);
//                     }
//                     else
//                     {
//                         Debug.LogError("需要选中一个节点");
//                     }
//                 } */

//         [MenuItem("Helper/移除选中节点的所有碰撞组件")]
//         public static void RemoveAll()
//         {
//             var obj = Selection.activeGameObject;
//             if (obj != null)
//             {
//                 var componentList = obj.GetComponentsInChildren<PolygonCollider2D>(true);
//                 foreach (var component in componentList)
//                 {
//                     DestroyImmediate(component);
//                 }
//             }
//             else
//             {
//                 Debug.LogError("需要选中一个节点");
//             }
//         }

//         /* [MenuItem("Helper/生成WorldAtlas信息")]
//         public static void GenerateWorldAtlasInfo()
//         {
//             var root = GameObject.Find("MapLayer/Viewport/Content");
//             if (root)
//             {
//                 var decorationConfigManager = new DecorationConfigManager();
//                 decorationConfigManager.InitConfigs(true);

//                 DecorationPropertyComponent.GenerateSortSpriteRenderer(decorationConfigManager, root.transform, out var sortSpriteRendererList, out var sortDefaultSpriteRendererList);
//                 // var spriteRendererList = DecorationPropertyComponent.GetSortSpriteRendererList(root);

//                 var infoList = GenerateInfoList(sortSpriteRendererList, 1980, false, out var packSpriteListList);
//                 var defaultInfoList = GenerateInfoList(sortDefaultSpriteRendererList, 1980, true, out var defaultPackSpriteListList);

//                 var strInfoList = JsonConvert.SerializeObject(infoList);
//                 var jsonObj = new Dictionary<string, List<DecorationAtlasConfig>>();
//                 jsonObj["spriteAtlasInfo"] = infoList;
//                 jsonObj["defaultSpriteAtlasInfo"] = defaultInfoList;
//                 var strJsonObj = JsonConvert.SerializeObject(jsonObj);
//                 if (!AssetDatabase.IsValidFolder("Assets/Export/Configs/GlobalAndDecoration"))
//                     AssetDatabase.CreateFolder("Assets/Export/Configs", "GlobalAndDecoration");
//                 FileInfo file = new FileInfo("Assets/Export/Configs/GlobalAndDecoration/decorationatlasinfo.json");
//                 StreamWriter sw = file.CreateText();
//                 sw.WriteLine(strJsonObj);
//                 sw.Close();
//                 sw.Dispose();

//                 // 移除Atlas目录
//                 var path = Application.dataPath + "/Export/SpriteAtlas/HomeAtlas";
//                 if (Directory.Exists(path))
//                     Directory.Delete(path, true);
//                 Directory.CreateDirectory(path);
//                 AssetDatabase.Refresh();

//                 PackSpriteAtlas(packSpriteListList, infoList);
//                 PackSpriteAtlas(defaultPackSpriteListList, defaultInfoList);
//                 AssetDatabase.Refresh();
//             }
//         } */

//         static private void PackSpriteAtlas(List<List<Sprite>> packSpriteListList, List<DecorationAtlasConfig> infoList)
//         {
//             for (var i = 0; i < packSpriteListList.Count; ++i)
//             {
//                 var packSpriteList = packSpriteListList[i];
//                 var info = infoList[i];
//                 var atlas = CreateSpriteAtlas();
//                 atlas.Add(packSpriteList.ToArray());
//                 var dirPath = string.Format("Assets/Export/SpriteAtlas/HomeAtlas/{0}", info.AreaId);
//                 var hdDirPath = dirPath + "/Hd";
//                 var sdDirPath = dirPath + "/Sd";
//                 if (!Directory.Exists(hdDirPath))
//                     Directory.CreateDirectory(hdDirPath);
//                 if (!Directory.Exists(sdDirPath))
//                     Directory.CreateDirectory(sdDirPath);

//                 // if (!AssetDatabase.IsValidFolder(hdDirPath))
//                 //     AssetDatabase.CreateFolder("Assets/Export/SpriteAtlas/HomeAtlas", info.AreaId.ToString());

//                 var atlasName = info.IsHD ? string.Format("HomeAtlas_{0}_HD", info.AreaId) : string.Format("HomeAtlas_{0}_{1}", info.AreaId, info.AtlasIndex);
//                 AssetDatabase.CreateAsset(atlas, string.Format("{0}/{1}.spriteatlas", hdDirPath, atlasName));
//                 var sdAtlas = info.IsHD ? CreateSpriteAtlas() : CreateSDSpriteAtlas(atlas);
//                 sdAtlas.Add(packSpriteList.ToArray());
//                 AssetDatabase.CreateAsset(sdAtlas, string.Format("{0}/{1}.spriteatlas", sdDirPath, atlasName));
//                 // atlasList.Add(atlas);
//             }
//         }

//         /* static private List<DecorationAtlasConfig> GenerateInfoList(List<SpriteRenderer> sortSpriteRendererList, int maxSize, bool isDefault, out List<List<Sprite>> packSpriteListList)
//         {
//             var method = MaxRectsBinPack.FreeRectChoiceHeuristic.RectBestAreaFit;
//             // var maxSize = 2048;
//             // var method = MaxRectsBinPack.FreeRectChoiceHeuristic.RectContactPointRule;

//             packSpriteListList = new List<List<Sprite>>();
//             var packSpriteList = new List<Sprite>();
//             packSpriteListList.Add(packSpriteList);

//             var pack = new MaxRectsBinPack(maxSize, maxSize, false);
//             var dict = new Dictionary<Sprite, bool>();
//             var infoList = new List<DecorationAtlasConfig>();
//             var info = new DecorationAtlasConfig();
//             info.MinZ = Int16.MaxValue;
//             info.MaxZ = Int16.MinValue;
//             info.Sprites = new HashSet<string>();
//             // info.AtlasIndex = 0;
//             // info.AreaId = -1;
//             info.AllAtlasIndex = 0;
//             infoList.Add(info);

//             var hdInfoDict = new Dictionary<int, DecorationAtlasConfig>();
//             var hdPackSpriteDict = new Dictionary<int, List<Sprite>>();

//             var preAreaId = -1;

//             foreach (var spriteRenderer in sortSpriteRendererList)
//             {
//                 var areaId = isDefault ? 999 : ParseAreaId(spriteRenderer.transform);
//                 if (!dict.ContainsKey(spriteRenderer.sprite))
//                 {
//                     // if (spriteRenderer.sprite.name.EndsWith("_hd"))
//                     if (false) // 暂时去除低清的提取高清资源功能
//                     {
//                         if (!hdInfoDict.TryGetValue(areaId, out var hdInfo))
//                         {
//                             hdInfo = new DecorationAtlasConfig();
//                             hdInfo.Sprites = new HashSet<string>();
//                             hdInfo.AreaId = areaId;
//                             hdInfo.IsHD = true;
//                             hdInfoDict[areaId] = hdInfo;
//                         }
//                         if (!hdPackSpriteDict.TryGetValue(areaId, out var hdPackSprite))
//                         {
//                             hdPackSprite = new List<Sprite>();
//                             hdPackSpriteDict[areaId] = hdPackSprite;
//                         }
//                         hdInfo.Sprites.Add(spriteRenderer.sprite.name);
//                         hdPackSprite.Add(spriteRenderer.sprite);
//                     }
//                     else
//                     {
//                         var textureSize = spriteRenderer.sprite.textureRect.size;
//                         var result = pack.Insert((int)textureSize.x, (int)textureSize.y, method);
//                         if (result.height == 0 || (!isDefault && (preAreaId != -1 && preAreaId != areaId)))
//                         {
//                             pack = new MaxRectsBinPack(maxSize, maxSize, false);
//                             result = pack.Insert((int)textureSize.x, (int)textureSize.y, method);
//                             packSpriteList = new List<Sprite>();
//                             packSpriteListList.Add(packSpriteList);

//                             info = new DecorationAtlasConfig();
//                             info.MinZ = Int16.MaxValue;
//                             info.MaxZ = Int16.MinValue;
//                             info.Sprites = new HashSet<string>();
//                             info.AllAtlasIndex = infoList.Count;
//                             infoList.Add(info);

//                             dict.Clear();
//                         }

//                         dict.Add(spriteRenderer.sprite, true);
//                         packSpriteList.Add(spriteRenderer.sprite);
//                         info.Sprites.Add(spriteRenderer.sprite.name);
//                         info.AreaId = areaId;
//                         preAreaId = areaId;
//                     }
//                 }

//                 var z = spriteRenderer.transform.position.z;
//                 if (info.MaxZ < z)
//                     info.MaxZ = z;
//                 if (info.MinZ > z)
//                     info.MinZ = z;
//             }

//             foreach (var item in hdInfoDict)
//             {
//                 infoList.Add(item.Value);
//                 packSpriteListList.Add(hdPackSpriteDict[item.Key]);
//             }

//             var areaDict = new Dictionary<int, int>();
//             foreach (var i in infoList)
//             {
//                 var atlasCount = 0;
//                 areaDict.TryGetValue(i.AreaId, out atlasCount);
//                 i.AtlasIndex = atlasCount;
//                 areaDict[i.AreaId] = atlasCount + 1;
//             }

//             return infoList;
//         } */

//         /* static private int ParseAreaId(Transform t)
//         {
//             var path = DecorationPropertyComponent.GetPath(t);
//             var dirList = new List<string>(path.Split('/'));
//             var index = dirList.IndexOf("Areas");
//             if (index < dirList.Count - 1)
//             {
//                 var dirName = dirList[index + 1];
//                 if (int.TryParse(dirName, out var areaId))
//                 {
//                     return areaId;
//                 }
//                 else
//                 {
//                     if (!path.Contains("Surface"))
//                         Debug.LogError("parse areaId error: " + path + ", dirName: " + dirName);
//                 }
//             }
//             else
//             {
//                 Debug.LogError("not found Buildings in path: " + path);
//             }
//             return 999;
//         } */


//         // [MenuItem("Helper/创建测试用SpriteAtlas")]
//         // public static void CreateTestSpriteAtlas ()
//         // {
//         //     var spriteAtlas = CreateSpriteAtlas();
//         //     var dirPath = string.Format("Assets/Export/SpriteAtlas/");
//         //     var atlasName = string.Format("Test");
//         //     AssetDatabase.CreateAsset(spriteAtlas, string.Format("{0}/{1}.spriteatlas", dirPath, atlasName));
//         // }

//         static private SpriteAtlas CreateSpriteAtlas()
//         {
//             var atlas = new SpriteAtlas();
//             atlas.SetIncludeInBuild(false);
//             // atlas.SetIncludeInBuild(true);

//             var packingSetting = atlas.GetPackingSettings();
//             packingSetting.enableRotation = false;
//             packingSetting.enableTightPacking = false;
//             packingSetting.padding = 2;
//             atlas.SetPackingSettings(packingSetting);

//             var textureSetting = atlas.GetTextureSettings();
//             textureSetting.readable = false;
//             textureSetting.generateMipMaps = false;
//             textureSetting.sRGB = true;
//             textureSetting.filterMode = FilterMode.Bilinear;
//             atlas.SetTextureSettings(textureSetting);

//             var defaultSetting = atlas.GetPlatformSettings("DefaultTexturePlatform");
//             defaultSetting.name = "DefaultTexturePlatform";
//             defaultSetting.maxTextureSize = 4096;
//             atlas.SetPlatformSettings(defaultSetting);

//             var platformSettings = atlas.GetPlatformSettings("iPhone");
//             platformSettings.name = "iPhone";
//             platformSettings.maxTextureSize = 2048;
//             platformSettings.overridden = true;
//             platformSettings.format = TextureImporterFormat.ASTC_4x4;
//             // platformSettings.compressionQuality = 100;
//             platformSettings.compressionQuality = 50;
//             atlas.SetPlatformSettings(platformSettings);

//             platformSettings = atlas.GetPlatformSettings("Android");
//             platformSettings.name = "Android";
//             platformSettings.maxTextureSize = 2048;
//             platformSettings.overridden = true;
//             // platformSettings.format = TextureImporterFormat.ETC2_RGBA8;
//             platformSettings.format = TextureImporterFormat.ETC2_RGBA8;
//             // platformSettings.compressionQuality = 100;
//             platformSettings.compressionQuality = 50;
//             atlas.SetPlatformSettings(platformSettings);

//             // var platformSettings = atlas.GetPlatformSettings(BuildTarget.NoTarget);
//             return atlas;
//         }

//         static private SpriteAtlas CreateSDSpriteAtlas(SpriteAtlas masterAtlas)
//         {
//             var atlas = new SpriteAtlas();
//             atlas.SetIncludeInBuild(false);
//             // atlas.SetIncludeInBuild(true);

//             // atlas.SetIsVariant(true);
//             // atlas.SetMasterAtlas(masterAtlas);

//             var packingSetting = atlas.GetPackingSettings();
//             packingSetting.enableRotation = false;
//             packingSetting.enableTightPacking = false;
//             packingSetting.padding = 2;
//             atlas.SetPackingSettings(packingSetting);

//             var textureSetting = atlas.GetTextureSettings();
//             textureSetting.readable = false;
//             textureSetting.generateMipMaps = false;
//             textureSetting.sRGB = true;
//             textureSetting.filterMode = FilterMode.Bilinear;
//             atlas.SetTextureSettings(textureSetting);

//             var defaultSetting = atlas.GetPlatformSettings("DefaultTexturePlatform");
//             defaultSetting.name = "DefaultTexturePlatform";
//             defaultSetting.maxTextureSize = 4096;
//             atlas.SetPlatformSettings(defaultSetting);

//             var platformSettings = atlas.GetPlatformSettings("iPhone");
//             platformSettings.name = "iPhone";
//             platformSettings.maxTextureSize = 2048;
//             platformSettings.overridden = true;
//             platformSettings.format = TextureImporterFormat.PVRTC_RGBA4;
//             // platformSettings.compressionQuality = 100;
//             platformSettings.compressionQuality = 0;
//             atlas.SetPlatformSettings(platformSettings);

//             platformSettings = atlas.GetPlatformSettings("Android");
//             platformSettings.name = "Android";
//             platformSettings.maxTextureSize = 2048;
//             platformSettings.overridden = true;
//             platformSettings.format = TextureImporterFormat.ETC2_RGBA8Crunched;
//             // platformSettings.format = TextureImporterFormat.ETC2_RGB4_PUNCHTHROUGH_ALPHA;
//             // platformSettings.compressionQuality = 100;
//             platformSettings.compressionQuality = 0;
//             atlas.SetPlatformSettings(platformSettings);

//             return atlas;
//         }

//         [MenuItem("Helper/更新Decoration表")]
//         public static void UpdateDecorationTable()
//         {
//             var fileName = Directory.GetCurrentDirectory() + "/../ExcelTools/gbuild_decoration.sh";
//             UnityEngine.Debug.LogError("fileName: " + fileName);
//             var startInfo = new System.Diagnostics.ProcessStartInfo()
//             {
//                 // WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
//                 // CreateNoWindow = true,
//                 FileName = fileName,
//                 // Arguments = assetBundlePath,
//                 UseShellExecute = false
//             };
//             var proc = new System.Diagnostics.Process()
//             {
//                 EnableRaisingEvents = true,
//                 StartInfo = startInfo,
//             };
//             proc.Start();
//             proc.WaitForExit();
//         }

//         [MenuItem("Helper/输出选中GameObject的全局坐标")]
//         public static void PrintGlobalLocation()
//         {
//             var obj = Selection.activeGameObject;
//             if (obj != null)
//             {
//                 Debug.LogError("Position.: " + obj.transform.position.x + ", " + obj.transform.position.y + ", " + obj.transform.position.z);
//             }
//         }

//         [MenuItem("Helper/TileMap/优化所有TileMap的边界")]
//         public static void OptimizeTileMapSize()
//         {
//             var root = GameObject.Find("MapLayer/Viewport/Content");
//             if (root)
//             {
//                 var tilemaps = root.GetComponentsInChildren<UnityEngine.Tilemaps.Tilemap>(true);
//                 foreach (var tilemap in tilemaps)
//                 {
//                     tilemap.CompressBounds();
//                 }
//             }
//         }

//         [MenuItem("Helper/TileMap/优化选中TileMap的边界")]
//         public static void OptimizeSelectedTileMapSize()
//         {
//             var objs = Selection.gameObjects;
//             foreach (var obj in objs)
//             {
//                 var tilemaps = obj.GetComponentsInChildren<UnityEngine.Tilemaps.Tilemap>(true);
//                 foreach (var tilemap in tilemaps)
//                 {
//                     tilemap.CompressBounds();
//                 }
//             }
//         }

//         [MenuItem("Helper/TileMap/重新排列选中的TileMap，使得TileMap的位置在中心")]
//         public static void RepositionTileMap()
//         {
//             var objs = Selection.gameObjects;
//             foreach (var obj in objs)
//             {
//                 var tilemaps = obj.GetComponentsInChildren<UnityEngine.Tilemaps.Tilemap>(true);
//                 foreach (var tilemap in tilemaps)
//                 {
//                     tilemap.CompressBounds();

//                     var bounds = tilemap.cellBounds;
//                     var diff = new Vector3Int(-(bounds.xMin + bounds.xMax) / 2, -(bounds.yMin + bounds.yMax) / 2, 0);
//                     var xMin = bounds.xMin + diff.x;
//                     var xMax = bounds.xMax + diff.x;
//                     var yMin = bounds.yMin + diff.y;
//                     var yMax = bounds.yMin + diff.y;
//                     var tileDict = new Dictionary<Vector3Int, Tile>();
//                     for (int x = bounds.xMin; x <= bounds.xMax; ++x)
//                     {
//                         for (int y = bounds.yMin; y <= bounds.yMax; ++y)
//                         {
//                             var point = new Vector3Int(x, y, 0);
//                             var tile = tilemap.GetTile(point) as Tile;
//                             if (tile && tile.sprite)
//                             {
//                                 var newPoint = point + diff;
//                                 tileDict[newPoint] = tile;
//                             }
//                         }
//                     }
//                     tilemap.ClearAllTiles();
//                     foreach (var item in tileDict)
//                         tilemap.SetTile(item.Key, item.Value);

//                     tilemap.CompressBounds();
//                     tilemap.transform.position -= tilemap.CellToLocal(diff);
//                     EditorUtility.SetDirty(tilemap.gameObject);
//                 }
//             }
//         }

//         [MenuItem("Helper/TileMap/检查并修复选中节点的的TileMap")]
//         public static void CheckTileMapError()
//         {
//             // var root = GameObject.Find("MapLayer/Viewport/Content");
//             var root = Selection.activeGameObject;
//             if (root)
//             {
//                 // var tilemap = root.GetComponent<UnityEngine.Tilemaps.Tilemap>();
//                 // tilemap.GetTilesBlock(tilemap.cellBounds);
//                 var tilemaps = root.GetComponentsInChildren<UnityEngine.Tilemaps.Tilemap>(true);
//                 foreach (var tilemap in tilemaps)
//                 {
//                     var bounds = tilemap.cellBounds;
//                     // var cells = tilemap.GetTilesBlock(bounds);
//                     UnityEngine.Tilemaps.TileBase replaceTile = null;

//                     var tileDict = new Dictionary<Vector3Int, UnityEngine.Tilemaps.TileBase>();
//                     var spDict = new Dictionary<Vector3Int, Sprite>();

//                     var errorDirty = false;
//                     for (int x = bounds.xMin; x <= bounds.xMax; ++x)
//                     {
//                         for (int y = bounds.yMin; y <= bounds.yMax; ++y)
//                         {
//                             var pos = new Vector3Int(x, y, 0);
//                             var sp = tilemap.GetSprite(pos);
//                             var tile = tilemap.GetTile(pos);
//                             if (tile && sp)
//                             {
//                                 replaceTile = tile;

//                                 tileDict[pos] = tile;
//                                 spDict[pos] = sp;
//                             }

//                             if (sp && !tile)
//                             {
//                                 Debug.LogError(string.Format("miss tile in : [{0}], [{1},{2}]: {3}", tilemap.gameObject, x, y, sp.name));
//                                 tilemap.SetTile(pos, replaceTile);
//                                 errorDirty = true;
//                             }

//                             if (tile && !sp)
//                             {
//                                 Debug.LogError(string.Format("miss sprite in : [{0}], [{1},{2}]: {3}", tilemap.gameObject, x, y, sp.name));
//                                 tilemap.SetTile(pos, replaceTile);
//                                 errorDirty = true;
//                             }

//                             // Debug.LogErrorFormat("[{0}]: {1}, {2}", pos, tile, sp);
//                         }
//                     }

//                     // if (errorDirty)
//                     {
//                         tilemap.ClearAllTiles();
//                         foreach (var item in tileDict)
//                         {
//                             tilemap.SetTile(item.Key, item.Value);
//                         }
//                     }
//                 }
//                 EditorUtility.SetDirty(root);
//             }
//         }

//         [MenuItem("Helper/TileMap/TestCase")]
//         public static void TileMapTestCase()
//         {
//             var root = Selection.activeGameObject;
//             if (root)
//             {
//                 var tilemap = root.GetComponent<UnityEngine.Tilemaps.Tilemap>();
//                 tilemap.GetTilesBlock(tilemap.cellBounds);
//             }
//         }
//     }
// }