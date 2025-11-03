using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using SomeWhere;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditorInternal;
using UnityEngine.SceneManagement;

public partial class WorldMapTool
{
    public enum ItemGroup
    {
        defaultItem = 0,
        group1,
        group2,
        group3,
        group4,
        group5,
        group6,
        group7,
        group8,
        group9,
        group10,
        group11,
        group12,
        all,
    }


    private string getGroupToggleSaveKey(ItemGroup group)
    {
        return $"GROUP_{group}";
    }

    private string getNodeGroupToggleSaveKey(int nodeId, ItemGroup group)
    {
        return $"NODE_GROUP_{nodeId}_{group}";
    }

    private string getAreaGroupToggleSaveKey(int areaId)
    {
        return $"AREA_{areaId}";
    }

    private void resetAllItemLayerHeight()
    {
        var areaRoot = _world.transform.Find(AREA_ROOT_PATH);
        if (areaRoot != null)
        {
            var isometricObjects = areaRoot.GetComponentsInChildren<IsometricObject>();
            foreach (var item in isometricObjects)
            {
                if (item.gameObject.activeSelf)
                {
                    item._floorHeight = 0;
                }
            }
        }
    }

    private void saveItem(GameObject itemObj)
    {
        if (PrefabUtility.HasPrefabInstanceAnyOverrides(itemObj, false))
        {
            PrefabUtility.ApplyPrefabInstance(itemObj, InteractionMode.AutomatedAction);
        }

        // PrefabUtility.SavePrefabAsset(building.gameObject);
        // PrefabUtility.ApplyPrefabInstance(building.gameObject, InteractionMode.AutomatedAction);
        // PrefabUtility.ApplyPropertyOverride()
    }

    private void loadAllItemsToCache()
    {
        _areaGroups.Clear();
        _areaGroups = new Dictionary<int, AreaGroup>();

        var allAreaRoot = _world.transform.Find(AREA_ROOT_PATH);
        foreach (Transform areaRoot1 in allAreaRoot.transform)
        {
            if (int.TryParse(areaRoot1.name, out var areaId))
            {
                var areaGroup = new AreaGroup();
                _areaGroups.Add(areaId, areaGroup);

                var areaRoot2 = areaRoot1.Find(areaId.ToString());
                if (areaRoot2 == null)
                {
                    Debug.LogError("Area not found:" + areaId);
                    continue;
                }

                foreach (Transform nodeTranform in areaRoot2.transform)
                {
                    if (int.TryParse(nodeTranform.name, out var nodeId))
                    {
                        var nodeGroup = new NodeGroup();
                        areaGroup.nodeDic.Add(nodeId, nodeGroup);

                        var nodeConfig = _configManager.GetNodeConfig(nodeId);
                        if (nodeConfig == null)
                        {
                            Debug.LogError("nodeConfig is null:" + nodeId);
                            continue;
                        }

                        var itemList = nodeConfig.itemList;

                        if (itemList != null)
                        {
                            var itemid = -1;
                            GameObject itemObj = null;

                            //n选1
                            nodeGroup.itemList = new List<Item>();
                            for (int i = 0; i < (int)ItemGroup.all; i++)
                            {
                                if (itemList.Length > i)
                                {
                                    itemid = itemList[i];
                                    if(itemid <= 0)
                                        continue;
                                    
                                    itemObj = nodeTranform.Find(itemid.ToString())?.gameObject;
                                    if (itemObj != null)
                                    {
                                        nodeGroup.itemList.Add(new Item(itemid, itemObj));
                                    }
                                    else
                                    {
                                        Debug.LogError($"{nodeId}挂点上未找到建筑点:{itemid}");
                                    }
                                }
                            }
                        }

                        //默认建筑
                        if (nodeConfig.defaultItem != 0)
                        {
                            var itemid = nodeConfig.defaultItem;
                            if (itemid > 0)
                            {
                                try
                                {
                                    var itemObj = nodeTranform.Find(itemid.ToString()).gameObject;
                                    if (itemObj != null) nodeGroup.defaultItem = new Item(itemid, itemObj);
                                }
                                catch (Exception e)
                                {
                                    Debug.LogError(e.ToString());
                                }
                                
                            }
                        }
                    }
                }
            }
            else
            {
                Debug.LogError("Area 解析错误:" + areaRoot1.name);
            }
        }
    }
}