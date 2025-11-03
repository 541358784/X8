using System;
using System.Collections.Generic;
using System.IO;
using Decoration;
using Newtonsoft.Json;
using SSAtlas;
using TMPro;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.SceneManagement;


// [EditorTool("地图工具")]
public partial class WorldMapTool
{
    class Item
    {
        public int itemId;
        public GameObject itemObj;

        public Item(int itemId, GameObject itemObj)
        {
            this.itemId = itemId;
            this.itemObj = itemObj;
        }
    }

    class NodeGroup
    {
        public Item defaultItem;
        public List<Item> itemList;
    }

    class AreaGroup
    {
        public Dictionary<int, NodeGroup> nodeDic = new Dictionary<int, NodeGroup>();
    }

    public static bool NpcTesting = false;

    private GameObject _world;
    private int _currentWorldId;
    private DecorationConfigManager _configManager;
    private IsometricMoving _testNpc;

    private Dictionary<string, bool> _toggleDic = new Dictionary<string, bool>();
    private Dictionary<int, AreaGroup> _areaGroups = new Dictionary<int, AreaGroup>();
    private Dictionary<int, Transform> _nodes = new Dictionary<int, Transform>();
    private GameObject _pathMap;
    
    private string WORLD_PREFAB_PATH(int worldId)
    {
        return $"Assets/Export/{Decoration.Utils.PathPrefabMap(worldId)}/WorldMap.prefab";
    }

    private string FOG_TEXTURE_PATH(int worldId, string imageName)
    {
        return $"Assets/Export/{Decoration.Utils.PathTextureFog(worldId, imageName)}";
    }
    
    
    private string PATHMAP_PREFAB_PATH(int worldId)
    {
        return $"Assets/Export/{Decoration.Utils.PathPrefabMap(worldId)}/PathMap.prefab";
    }

    private string FogMAP_PREFAB_PATH(int worldId)
    {
        return $"Assets/Export/{Decoration.Utils.PathPrefabMap(worldId)}/FogMap.prefab";
    }
    
    private const string TEST_NPC_PATH = "Assets/Export/Character/Npc1001.prefab";
    private const string AREA_ROOT_PATH = "MapLayer/Viewport/Content/Areas";
    private const string WORLD_ITEM_ROOT_PATH = "MapLayer/Viewport/Content/Surface/WorldItem";

    public void DrawGUI()
    {
        using (new HandleGUIScope())
        {
            using (new VerticalScope("装修菜单"))
            {
                drawOptionsScope();
                GUILayout.Space(10);
                drawLoadAllItemScope();
                GUILayout.Space(10);
                drawGroupToggle();
                GUILayout.Space(10);
                drawAreasGroup();
                GUILayout.Space(10);
                drawResetLine();
                GUILayout.Space(10);
                // drawNpcScope();
                // GUILayout.Space(10);
                drawUnloadItems();
                drawUnloadItemsWithoutArea();
                GUILayout.Space(10);
                drawFogGroup();
                GUILayout.Space(10);
                drawDynamicMapGroup();
            }
        }
    }

    #region 动态图集

    private void drawUpdateAtals()
    {
        if (GUILayout.Button("清空装修图集"))
        {
            DecoSpriteAtlasPacker.ClearDecoAtals();
        }

        if (GUILayout.Button("生成动态图集"))
        {
            DecoSpriteAtlasPacker.ClearDecoAtals();
            DecoSpriteAtlasPacker.BakeAtlas();
        }
    }

    #endregion


    Vector2 _toggleScrollPos = Vector2.zero;
    private void drawGroupToggle()
    {
        _toggleScrollPos = GUILayout.BeginScrollView(_toggleScrollPos);
        using (new HorizontalScope())
        {
            for (int i = 0; i < (int)ItemGroup.all; i++)
            {
                var group = (ItemGroup)i;
                var groupKey = getGroupToggleSaveKey(group);
                drawToggle(groupKey, i.ToString(), on => showGroup(group, on));
            }
        }
        GUILayout.EndScrollView();
    }

    private Vector2 _scrollPos;

    private Vector2 _areascrollPos;
    private void drawAreasGroup()
    {
        _areascrollPos = GUILayout.BeginScrollView(_areascrollPos);
        using (new HorizontalScope())
        {
            foreach (var areaKv in _areaGroups)
            {
                var areaId = areaKv.Key;
                var areaGroup = areaKv.Value;

                drawToggle(getAreaGroupToggleSaveKey(areaId), areaId.ToString(), on => showArea(areaId, on));
            }
        }
        GUILayout.EndScrollView();

        GUILayout.Space(10);
        _scrollPos = GUILayout.BeginScrollView(_scrollPos);

        foreach (var areaKv in _areaGroups)
        {
            var areaId = areaKv.Key;
            var areaGroup = areaKv.Value;

            var areaToggleKey = getAreaGroupToggleSaveKey(areaId);
            var areaToggleValue = getToggleValue(areaToggleKey);
            
            if (areaToggleValue)
            {
                foreach (var nodeKv in areaGroup.nodeDic)
                {
                    var nodeId = nodeKv.Key;
                    var node = nodeKv.Value;
                    drawNode(areaId, nodeId);
                }
            }
        }

        GUILayout.EndScrollView();
    }

    private bool IsSelectArea(string name)
    {
        if (!int.TryParse(name, out var areaId))
            return false;

        return _configManager.GetAreaConfig(areaId) != null;
    }
    
    private void drawNode(int areaId, int nodeId)
    {
        using (new HorizontalScope())
        {
            if (GUILayout.Button($"{nodeId}"))
            {
                var areaRoot = _world.transform.Find(AREA_ROOT_PATH);
                if (areaRoot)
                {
                    foreach (Transform areaParent in areaRoot.transform)
                    {
                        var area = areaParent.Find(areaParent.name);
                        if(area.name != areaId.ToString())
                            continue;

                        Selection.activeGameObject = area.transform.Find(nodeId.ToString())?.gameObject;
                    }
                }
            }
            for (int i = 0; i < (int)ItemGroup.all; i++)
            {
                var group = (ItemGroup)i;
                drawToggle(getNodeGroupToggleSaveKey(nodeId, group), i.ToString(),
                    on => showNodeGroup(areaId, nodeId, group, on));
            }
        }
    }

    private void drawUnloadItems()
    {
        if (GUILayout.Button("卸载所有建筑"))
        {
            if (EditorUtility.DisplayDialog("提示", "确实是否都已保存", "已保存,继续", "取消"))
            {
                // unloadWorldItems();
                unloadAllItems();
                markAllGroupToogle(false);
                
                Undo.DestroyObjectImmediate(_fogMap);
                Undo.DestroyObjectImmediate(_pathMap);
            }
        }
    }

    private void drawUnloadItemsWithoutArea()
    {
        if (GUILayout.Button("卸载所有建筑(保留区域)"))
        {
            if (EditorUtility.DisplayDialog("提示", "确实是否都已保存", "已保存,继续", "取消"))
            {
                // unloadWorldItems();
                
                _buildingList.ForEach(item => Undo.DestroyObjectImmediate(item));
                _buildingList.Clear();
                
                markAllGroupToogle(false);
            }
        }
    }

    private void drawResetLine()
    {
        if (GUILayout.Button("重置激活建筑的层级参考线"))
        {
            if (EditorUtility.DisplayDialog("提示", "确定要重置所有建筑的参考层级吗", "确定", "取消"))
            {
                resetAllItemLayerHeight();
            }
        }
    }

    private void showGroup(ItemGroup group, bool show)
    {
        foreach (var areaKv in _areaGroups)
        {
            var area = areaKv.Value;
            var areaId = areaKv.Key;

            foreach (var nodeKv in area.nodeDic)
            {
                var node = nodeKv.Value;
                var nodeId = nodeKv.Key;
                showNodeGroup(areaId, nodeId, group, show);
            }
        }
    }

    private void showArea(int areaId, bool show)
    {
        var areaGroup = _areaGroups[areaId];
        foreach (var nodeKv in areaGroup.nodeDic)
        {
            var node = nodeKv.Value;
            var nodeId = nodeKv.Key;

            showNodeGroup(areaId, nodeId, ItemGroup.all, show);
        }
    }

    private void showNodeGroup(int areaId, int nodeId, ItemGroup group, bool show)
    {
        var node = _areaGroups[areaId].nodeDic[nodeId];

        switch (group)
        {
            case ItemGroup.defaultItem:
                node?.defaultItem?.itemObj?.SetActive(show);
                break;
            case ItemGroup.all:
                for (int i = 0; i < (int)ItemGroup.all; i++)
                {
                    if (node?.itemList?.Count > i) node?.itemList[i]?.itemObj?.SetActive(show);
                }

                node?.defaultItem?.itemObj?.SetActive(show);
                break;
            default:
                if (node?.itemList?.Count > (int)group - 1) node?.itemList[(int)group - 1]?.itemObj?.SetActive(show);
                break;
        }

        saveToggleValue(getNodeGroupToggleSaveKey(nodeId, group), show);
    }

    private void drawOptionsScope()
    {
        using (new HorizontalScope())
        {
            IsometricObject.HelperLine_Selected = GUILayout.Toggle(IsometricObject.HelperLine_Selected, "选中物体的辅助线");
            IsometricObject.HelperLine_UnSelected = GUILayout.Toggle(IsometricObject.HelperLine_UnSelected, "未选中物体的辅助线");
        }
    }

    private void drawLoadAllItemScope()
    {
        if (GUILayout.Button("加载所有建筑"))
        {
            // loadWorldItems(_currentWorldId);
            loadAllItems(_currentWorldId);
            loadAllItemsToCache();
            showGroup(ItemGroup.all, true);
            markAllGroupToogle(true);
        }
    }

    public void LoadWorld(int mapId)
    {
        var worldId = mapId;
        
        _currentWorldId = worldId;
        _configManager = new DecorationConfigManager();
        _configManager.InitConfigs(true);

        _world = GameObject.Find("WorldMap");
        if (_world != null)
        {
            GameObject.DestroyImmediate(_world.gameObject);
        }

        var worldPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(string.Format(WORLD_PREFAB_PATH(worldId)));
        _world = PrefabUtility.InstantiatePrefab(worldPrefab) as GameObject;

        //加载Pathmap
        var pathMapPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(string.Format(PATHMAP_PREFAB_PATH(worldId)));
        _pathMap= PrefabUtility.InstantiatePrefab(pathMapPrefab) as GameObject;
        _pathMap.transform.SetParent(_world.transform.Find("MapLayer/Viewport/Content"));
        _pathMap.transform.localPosition = Vector3.zero;
        _pathMap.transform.localScale = Vector3.one;
        _pathMap.transform.localRotation = Quaternion.identity;

        //加载fogmap
        loadFogMap();
        InitDynamicMap(worldId);
        
        // loadWorldItems(worldId);
        loadAllItems(worldId);

        loadAllItemsToCache();
        showGroup(ItemGroup.all, true);
        markAllGroupToogle(true);
    }

    private void markAllGroupToogle(bool on)
    {
        for (int i = 0; i < (int)ItemGroup.all; i++)
        {
            saveToggleValue(getGroupToggleSaveKey((ItemGroup)i), on);
        }

        foreach (var areaKv in _areaGroups)
        {
            var areaId = areaKv.Key;
            var area = areaKv.Value;

            saveToggleValue(getAreaGroupToggleSaveKey(areaId), on);
            foreach (var nodeKv in area.nodeDic)
            {
                var nodeId = nodeKv.Key;
                var node = nodeKv.Value;
                for (int i = 0; i < (int)ItemGroup.all; i++)
                {
                    saveToggleValue(getNodeGroupToggleSaveKey(nodeId, (ItemGroup)i), on);
                }
            }
        }
    }

    private void saveToggleValue(string key, bool on)
    {
        if (_toggleDic.ContainsKey(key)) _toggleDic[key] = on;
        else _toggleDic.Add(key, on);
    }

    private bool getToggleValue(string key)
    {
        if (!_toggleDic.TryGetValue(key, out var status))
        {
            return false;
        }

        return status;
    }

    private void drawToggle(string key, string name, Action<bool> action)
    {
        var lastStatus = getToggleValue(key);

        var newStatus = GUILayout.Toggle(lastStatus, name);
        if (newStatus != lastStatus)
        {
            saveToggleValue(key, newStatus);

            action.Invoke(newStatus);
        }
    }

    public void UnloadWorld()
    {
        Undo.DestroyObjectImmediate(GameObject.Find("WorldMap"));
        _world = null;
        // _configManager = null;
    }

    private void unloadAllItems()
    {
        _buildingList.Clear();
        var areaRoot = _world.transform.Find(AREA_ROOT_PATH);
        if (areaRoot)
        {
            foreach (Transform areaParent in areaRoot.transform)
            {
                var area = areaParent.Find(areaParent.name);
                if (area != null)
                {
                    Undo.DestroyObjectImmediate(area.gameObject);
                }
            }
        }
    }

    // private void unloadWorldItems()
    // {
    //     var worldItemRoot = _world.transform.Find(WORLD_ITEM_ROOT_PATH);
    //     if (worldItemRoot)
    //     {
    //         foreach (Transform worldItem in worldItemRoot)
    //         {
    //             Undo.DestroyObjectImmediate(worldItem.gameObject);
    //         }
    //     }
    // }

    // private void loadWorldItems(int worldId)
    // {
    //     unloadWorldItems();
    //
    //     var worldItemRoot = _world.transform.Find(WORLD_ITEM_ROOT_PATH);
    //
    //     var worldConfigList = ConfigManager.ItemWorldList;
    //     foreach (var config in worldConfigList)
    //     {
    //         var worldItemPath = $"Assets/Export/{Decoration.Utils.PathPrefabItem(worldId)}/{config.itemId}.prefab";
    //         // var worldItemPath = string.Format($"Assets/Export/Prefabs/Decoration/WorldItem{worldId}/{config.itemId}.prefab");
    //         var worldItemPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(worldItemPath);
    //         if (worldItemPrefab == null)
    //         {
    //             Debug.LogError(string.Format("not found prefab: [{0}]", worldItemPath));
    //             continue;
    //         }
    //
    //         var worldItem = PrefabUtility.InstantiatePrefab(worldItemPrefab, worldItemRoot) as GameObject;
    //         var itemTransform = worldItem.transform;
    //         itemTransform.position = new Vector3(config.x, config.y, config.z);
    //         itemTransform.localScale = new Vector3(config.scalex, config.scaley, 1);
    //         var obj = worldItem.GetComponent<IsometricObject>();
    //         if (obj != null) obj._floorHeight = config.floorHeight;
    //     }
    // }

    private void loadAllItems(int worldId)
    {
        unloadAllItems();

        var allAreaRoot = _world.transform.Find(AREA_ROOT_PATH);

        var worldConfig = _configManager.WorldConfigs.Find(c => c.id == worldId);
        if (worldConfig == null)
        {
            Debug.LogError($"没有world配置:{worldId}");
            return;
        }
        if (worldConfig.areaIds == null)
        {
            Debug.LogError($"没有areaIds配置:{worldId}");
            return;
        }
            
        _nodes.Clear();
        
        foreach (var areaId in worldConfig.areaIds)
        {
            var areaRoot = loadAreaObj(worldConfig.id, areaId).transform;

            var areaConfig = _configManager.AreaConfigList.Find(c => c.id == areaId);
            if(areaConfig == null || areaConfig.stages == null)
                continue;
            
            foreach (var stageId in areaConfig.stages)
            {
                var stageConfig = _configManager.StageList.Find(c => c.id == stageId);
                if(stageConfig == null || stageConfig.nodes == null)
                {
                    Debug.LogError("Can't find  stage:" + stageId);
                    continue;
                }
                
                foreach (var nodeId in stageConfig.nodes)
                {
                    var nodeRoot = areaRoot.Find($"{nodeId}");
                    
                    _nodes.Add(nodeId, nodeRoot);
                    
                    var nodeConfig = _configManager.nodeConfigs.Find(c => c.id == nodeId);

                    if (nodeConfig == null)
                    {
                        Debug.LogError("Can't find node config:" + nodeId);
                        continue;
                    }

                    loadItemPrefab(worldId, areaId, nodeConfig.defaultItem, nodeRoot);

                    if (nodeConfig.itemList == null)
                        continue;
                    
                    for (var index = 0; index < nodeConfig.itemList.Length; index++)
                    {
                        loadItemGroup(index, nodeConfig, worldId, areaId, nodeRoot);
                    }
                }
            }
        }
    }

    private Transform loadAreaObj(int worldId, int areaId)
    {
        var areaPrefabPath = $"Assets/Export/{Decoration.Utils.PathPrefabArea(worldId, areaId)}.prefab";
        var areaPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(areaPrefabPath);
        if (areaPrefab == null)
        {
            Debug.LogError($"not found prefab: [{areaPrefabPath}]");
            return null;
        }

        var areaRoot = _world.transform.Find($"{AREA_ROOT_PATH}/{areaId}");
        var areaObj = PrefabUtility.InstantiatePrefab(areaPrefab, areaRoot) as GameObject;

        return areaObj.transform;
    }

    private void loadItemGroup(int index, TableNodes nodeConfig, int worldId, int areaId, Transform node)
    {
        if (nodeConfig.itemList == null)
            return;
        
        if (index >= nodeConfig.itemList.Length)
            return;
        loadItemPrefab(worldId, areaId, nodeConfig.itemList[index], node);
    }

    private void loadItemPrefab(int worldId, int areaId, int itemId, Transform node)
    {
        if (itemId <= 0) return;

        var activityType = EActivityType.NONE;
        var itemConfig = _configManager.GetItemConfig(itemId);
        if (itemConfig == null)
        {
            Debug.LogError("Can't find item config: " + itemId);
            return;
        }
        
        var prefabBasePath = Decoration.Utils.PathPrefabBuilding(worldId, areaId, itemId);
        var itemPrefabPath = $"Assets/Export/{prefabBasePath}/{itemId}.prefab";

        var itemPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(itemPrefabPath);
        if (itemPrefab == null)
        {
            var newItem = new GameObject(itemId.ToString());
            var tip = new GameObject("Tip");
            tip.transform.SetParent(newItem.transform);
            tip.transform.localPosition = Vector3.zero;
            var sprite = new GameObject("1");
            sprite.transform.SetParent(newItem.transform);
            sprite.transform.localPosition = Vector3.zero;
            sprite.transform.SetParent(newItem.transform);
            sprite.AddComponent<SpriteRenderer>();
            itemPrefab = PrefabUtility.SaveAsPrefabAsset(newItem, itemPrefabPath, out var saveSuccess);
            Debug.LogError(saveSuccess ? "itemPrefab 创建成功" : "itemPrefab 创建失败");
        }

        var item = PrefabUtility.InstantiatePrefab(itemPrefab, node) as GameObject;

        _buildingList.Add(item);
    }

    private List<GameObject> _buildingList = new List<GameObject>();

    private void saveAllAreas()
    {
        var areaParentRoot = _world.transform.Find(AREA_ROOT_PATH);
        if (areaParentRoot)
        {
            foreach (Transform areaParent in areaParentRoot.transform)
            {
                for (int i = 0; i < areaParent.childCount; ++i)
                {
                    var area = areaParent.GetChild(i);
                    if (int.TryParse(area.name, out var areaId))
                    {
                        PrefabUtility.ApplyPrefabInstance(area.gameObject, InteractionMode.AutomatedAction);
                    }
                }
            }
        }
    }

    public void Save()
    {
        markAllGroupToogle(true);
        showGroup(ItemGroup.all, true);

        var areaParentRoot = _world.transform.Find(AREA_ROOT_PATH);
        if (areaParentRoot)
        {
            var totalCount = 0f;
            var current = 0f;
            foreach (Transform areaParent in areaParentRoot.transform)
            {
                totalCount += areaParent.childCount;
            }

            foreach (Transform areaParent in areaParentRoot.transform)
            {
                for (int i = 0; i < areaParent.childCount; ++i)
                {
                    var area = areaParent.GetChild(i);
                    saveArea(area);

                    current += 1f;
                    EditorUtility.DisplayProgressBar("保存建筑Prefab...", $"{current}/{totalCount}", current / totalCount);
                }
            }

            EditorUtility.ClearProgressBar();
        }

        saveFog();
        // exportWorldItemPosConfig(_currentWorldId);
        // exportItemPosConfig(_currentWorldId);
    }

    private void saveArea(Transform area)
    {
        if (int.TryParse(area.name, out var areaId))
        {
            var totalCount = area.transform.childCount;
            for (int j = 0; j < totalCount; ++j)
            {
                var nodeTransform = area.transform.GetChild(j);
                saveNode(nodeTransform);
            }

            EditorUtility.ClearProgressBar();
        }
    }

    private void saveNode(Transform nodeTransform)
    {
        if (int.TryParse(nodeTransform.name, out var buidingPointId))
        {
            foreach (Transform itemTransform in nodeTransform)
            {
                if (int.TryParse(itemTransform.name, out var itemId))
                {
                    var itemConfig = _configManager.GetItemConfig(itemId);
                    if (itemConfig != null)
                    {
                        saveItem(itemTransform.gameObject);
                    }
                }
            }
        }
    }
    
    public static string GetPathItemWorldConfig(int worldId)
    {
        return $"Decoration/Worlds/World{worldId}/ConfigEditor/itemworld";
    }

    public static string GetPathItemPosConfig(int worldId)
    {
        return $"Decoration/Worlds/World{worldId}/ConfigEditor/itempos";
    }
}
