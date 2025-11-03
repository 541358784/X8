using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonPlus;
using SomeWhere;
using Spine.Unity;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class WorldMapTool
{
    private GameObject _fogMap;
    private FogMap _fogMapMono;

    
    private void loadFogMap()
    {
        _fogMap = null;
        _fogMapMono = null;
        
        var fogMapPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(string.Format(FogMAP_PREFAB_PATH(_currentWorldId)));
        if (fogMapPrefab == null)
            return;
        
        _fogMap = PrefabUtility.InstantiatePrefab(fogMapPrefab) as GameObject;
        _fogMap.transform.SetParent(_world.transform.Find("MapLayer/Viewport/Content"));
        _fogMap.transform.localPosition = Vector3.zero;
        _fogMap.transform.localScale = Vector3.one;
        _fogMap.transform.localRotation = Quaternion.identity;
        _fogMapMono = _fogMap.GetComponent<FogMap>();
        for (int i = 0; i < _fogMapMono._fogLayers.Count; i++)
        {
            if(_fogMap.transform.Find(_fogMapMono._fogLayers[i]._areaId.ToString()) != null)
                continue;
            
            _fogMapMono._fogLayers.RemoveAt(i);
            i--;
        }

        foreach (var layer in _fogMapMono._fogLayers)
        {
            foreach (var chunk in layer._fogChunks)
            {
                foreach (var blocks in chunk._blocks)
                {
                    var spRender = blocks._gameObject.AddComponent<SpriteRenderer>();
                    spRender.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(string.Format(FOG_TEXTURE_PATH(_currentWorldId, blocks._imageName + ".png")));
                }
            }
        }
        
        _fogMap.gameObject.SetActive(false);
    }

    private Vector2 _fogScrollPos;
    private void drawFogGroup()
    {
        if(_fogMap == null)
            return;

        using (new VerticalScope("迷雾编辑", GUILayout.Height(50f)))
        {
            _fogScrollPos = GUILayout.BeginScrollView(_fogScrollPos);
            using (new HorizontalScope())
            {
                _fogMap.gameObject.SetActive(GUILayout.Toggle(_fogMap.gameObject.activeSelf, "迷雾开启关闭"));

                for (int i = 0; i < _fogMapMono._fogLayers.Count; i++)
                {
                    string key = "fog"+_fogMapMono._fogLayers[i]._areaId;
                    saveToggleValue(key, _fogMapMono.IsShow(_fogMapMono._fogLayers[i]._areaId));
                    drawToggle(key, _fogMapMono._fogLayers[i]._areaId.ToString(), on => showFogGroup(_fogMapMono._fogLayers[i]._areaId, on));
                }
            }
            GUILayout.EndScrollView();
            
            using (new HorizontalScope())
            {
                if (GUILayout.Button("选中迷雾"))
                    Selection.activeGameObject = _fogMap;
            
                bool isCreate = true;
                bool isDelete = true;
                if (Selection.gameObjects != null && Selection.gameObjects.Length > 0)
                {
                    foreach (var obj in Selection.gameObjects)
                    {
                        if (!IsSelectArea(obj.name))
                        {
                            isCreate = false;
                            isDelete = false;
                            break;
                        }

                        if (obj.transform.parent == _fogMap.transform)
                            isCreate = false;
                        else
                            isDelete = false;
                    }
                }
                else
                {
                    isCreate = false;
                    isDelete = false;
                }

                using (new DisabledGroupScope(!isCreate))
                {
                    if (GUILayout.Button("创建(层)"))
                        createFog();
                }

                using (new DisabledGroupScope(!isDelete))
                {
                    if (GUILayout.Button("删除(层)"))
                    {
                        if (EditorUtility.DisplayDialog("提示", "确定删除选择的迷雾层", "确定", "取消"))
                        {
                            deleteFog();
                        }
                    }   
                
                    if (GUILayout.Button("创建迷雾[块]"))
                        createFogCell();
                }

                bool hsaOverrides = PrefabUtility.HasPrefabInstanceAnyOverrides(_fogMap, false);
                using (new DisabledGroupScope(!hsaOverrides))
                {
                    if (GUILayout.Button("保存迷雾"))
                    {
                        saveFog();
                    }
                }
            }
        }
    }
    
    

    private void createFog()
    {
        foreach (var obj in Selection.gameObjects)
        {
            int.TryParse(obj.name, out var areaId);
            
             var layer = _fogMapMono.CreateFogLayer(areaId);
             layer._nodeReference = obj;
             
             Selection.activeGameObject = layer._gameObject;
        }
    }

    private void createFogCell()
    {
        foreach (var obj in Selection.gameObjects)
        {
            int.TryParse(obj.name, out var areaId);
            
            var cell = _fogMapMono.CreateFogChunk(areaId);
            
            cell._gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = AssetDatabase.LoadAssetAtPath<Sprite>(string.Format(FOG_TEXTURE_PATH(_currentWorldId, "1.png")));
            
            Selection.activeGameObject = cell._gameObject;
        }
    }
    
    private void deleteFog()
    {
        foreach (var obj in Selection.gameObjects)
        {
            GameObject cloneObj = GameObject.Instantiate(_fogMap);
            Transform child =  cloneObj.transform.Find(obj.name);
            UnityEngine.GameObject.DestroyImmediate(child.gameObject);

            PrefabUtility.SaveAsPrefabAsset(cloneObj, string.Format(FogMAP_PREFAB_PATH(_currentWorldId)));
            GameObject.DestroyImmediate(cloneObj);
        }
        
        GameObject.DestroyImmediate(_fogMap);
        loadFogMap();
        
        Selection.activeGameObject = _fogMap;
    }

    private void saveFog()
    {
        if(_fogMap == null)
            return;

        // PrefabUtility.SaveAsPrefabAsset(_fogMap, string.Format(FogMAP_PREFAB_PATH(_currentWorldId)));
        // GameObject.DestroyImmediate(_fogMap);

        _fogMapMono.Save();
        
        if (PrefabUtility.HasPrefabInstanceAnyOverrides(_fogMap, false))
        {
            PrefabUtility.ApplyPrefabInstance(_fogMap, InteractionMode.AutomatedAction);
        }
    }
    private void showFogGroup(int areaId, bool isShow)
    {
        _fogMapMono.ShowFogLayer(areaId, isShow);
    }
    
}