using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Decoration.DynamicMap;
using DragonPlus;
using Newtonsoft.Json;
using SomeWhere;
using Spine.Unity;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public partial class WorldMapTool
{
    private float _chunkWidth = 5;
    private float _chunkHeight = 5;
    private int _gridRow = 10;
    private int _gridCol = 20;
    private int _orgPosX = 0;
    private int _orgPosY = 0;

    private bool _editorDynamicmap = false;
    
    private Vector2 orgPosition = Vector2.zero;
    
    private DynamicMapHelpMono _mono;
    private DynamicMap _dynamic = new DynamicMap();
    private void InitDynamicMap(int worldId)
    {
        if (_mono == null)
        {
            var dyObj = new GameObject("DynamicMapHelp");
            dyObj.transform.SetParent(_world.transform);
            dyObj.transform.localPosition = Vector3.zero;
            dyObj.transform.localScale = Vector3.one;
            _mono = UnityEditor.ObjectFactory.AddComponent<DynamicMapHelpMono>(dyObj);
            
            DynamicMapConfigManager.Instance.InitConfig(worldId);
        }
        
        _gridRow = DynamicMapConfigManager.Instance._gridRow  > 0 ? DynamicMapConfigManager.Instance._gridRow : _gridRow;
        _gridCol = DynamicMapConfigManager.Instance._gridCol  > 0 ? DynamicMapConfigManager.Instance._gridCol : _gridCol;
        _chunkWidth = DynamicMapConfigManager.Instance._chunkWidth  > 0 ? DynamicMapConfigManager.Instance._chunkWidth : _chunkWidth;
        _chunkHeight = DynamicMapConfigManager.Instance._chunkHeight  > 0 ? DynamicMapConfigManager.Instance._chunkHeight : _chunkHeight;
        _orgPosX = DynamicMapConfigManager.Instance._orgPosX;
        _orgPosY =  DynamicMapConfigManager.Instance._orgPosY;
        
        _dynamic._gridRow = _gridRow; 
        _dynamic._gridCol = _gridCol;
        _dynamic._chunkHeight = _chunkHeight;
        _dynamic._chunkWidth = _chunkWidth;
        _dynamic._orgPosX = _orgPosX;
        _dynamic._orgPosY = _orgPosY;
    }
    private void drawDynamicMapGroup()
    {
        if(_mono == null)
            return;
        
        using (new VerticalScope("动态地图", GUILayout.Height(50f)))
        {
            _editorDynamicmap = GUILayout.Toggle(_editorDynamicmap, "开启切割地图");
            _mono.gameObject.SetActive(_editorDynamicmap);
            if (_editorDynamicmap)
            {
                _chunkWidth = EditorGUILayout.FloatField("地块尺寸 宽度" + _chunkWidth + "*" + _chunkHeight, _chunkWidth);
                _chunkHeight = EditorGUILayout.FloatField("地块尺寸 高度" + _chunkWidth + "*" + _chunkHeight, _chunkHeight);
                
                _gridRow = EditorGUILayout.IntField("地块行 " + _gridRow, _gridRow);
            
                _gridCol = EditorGUILayout.IntField("地块列 " + _gridCol, _gridCol);
            
                _orgPosX = EditorGUILayout.IntField("地图格子偏移 x", _orgPosX);
                _orgPosY = EditorGUILayout.IntField("地图格子偏移 y", _orgPosY);
                
                if (GUILayout.Button("切割地图"))
                    CutMapChunk();
            
                _dynamic._gridRow = _gridRow;
                _dynamic._gridCol = _gridCol;
                _dynamic._chunkWidth = _chunkWidth;
                _dynamic._chunkHeight = _chunkHeight;
                _dynamic._orgPosX = _orgPosX;
                _dynamic._orgPosY = _orgPosY;
            
                DynamicMapConfigManager.Instance._gridRow = _gridRow;
                DynamicMapConfigManager.Instance._gridCol = _gridCol;
                DynamicMapConfigManager.Instance._chunkWidth = _chunkWidth;
                DynamicMapConfigManager.Instance._chunkHeight = _chunkHeight;
                DynamicMapConfigManager.Instance._orgPosX = _orgPosX;
                DynamicMapConfigManager.Instance._orgPosY = _orgPosY;

                orgPosition.x = _orgPosX;
                orgPosition.y = _orgPosY;
            }
        }
    }

    private void CutMapChunk()
    {
        _dynamic._mapId = _currentWorldId;
        _dynamic._chunks.Clear();

        orgPosition.x = -1.0f * _gridCol / 2 * _chunkWidth + _orgPosX;
        orgPosition.y = 1.0f * _gridRow / 2 * _chunkHeight + _orgPosY;
        
        for (int i = 0; i < _dynamic._gridRow * _dynamic._gridCol; i++)
        {
            var chunk = new Chunk();
            chunk.index = i;
            chunk.row = i / _dynamic._gridCol;
            chunk.col = i % _dynamic._gridCol;

            FillChunk(chunk);
            
            _dynamic._chunks.Add(chunk);
        }
        
        var configStr = JsonConvert.SerializeObject(_dynamic, Formatting.Indented);
        File.WriteAllText($"{Application.dataPath}/Export/Configs/Decoration/DynamicMap_{_dynamic._mapId}.json", configStr);
        AssetDatabase.Refresh();
    }

    private void FillChunk(Chunk chunk)
    { 
        Vector3 centerPos = new Vector3(orgPosition.x+chunk.col * _chunkWidth + _chunkWidth/2, orgPosition.y-chunk.row * _chunkHeight-_chunkHeight/2);
        
        chunk.x = centerPos.x;
        chunk.y = centerPos.y;
        chunk.z = centerPos.z;
        chunk.minX = centerPos.x - _chunkWidth / 2 ;
        chunk.minY = centerPos.y - _chunkHeight / 2;
        chunk.sizeW = _chunkWidth;
        chunk.sizeH = _chunkHeight;
        
        Rect gridRect = new Rect(chunk.minX, chunk.minY, _chunkWidth, _chunkHeight);
        foreach (var area in _areaGroups)
        {
            foreach (var node in area.Value.nodeDic)
            {
                if (Overlaps(gridRect, node.Value.defaultItem))
                {
                    Block block = new Block(area.Key, node.Key);
                    chunk._blocks.Add(block);
                    continue;
                }

                if (node.Value.itemList == null)
                {
                    if (_nodes.ContainsKey(node.Key))
                    {
                        if (Overlaps(gridRect, _nodes[node.Key].transform.Find("CameraTip")))
                        {
                            Block block = new Block(area.Key, node.Key);
                            chunk._blocks.Add(block);
                        }
                    }
                    continue;
                }

                bool isOverlaps = false;
                foreach (var item in node.Value.itemList)
                {
                    if (!Overlaps(gridRect, item))
                        continue;

                    isOverlaps = true;
                    Block block = new Block(area.Key, node.Key);
                    chunk._blocks.Add(block);
                    break;
                }

                if (!isOverlaps)
                {
                    if (_nodes.ContainsKey(node.Key))
                    {
                        if (Overlaps(gridRect, _nodes[node.Key].transform.Find("CameraTip")))
                        {
                            Block block = new Block(area.Key, node.Key);
                            chunk._blocks.Add(block);
                        }
                    }
                }
            }
        }
    }

    private bool Overlaps(Rect orgRect, Item item)
    {
        if (item == null || item.itemObj == null)
            return false;

        if (Overlaps(orgRect, GetComponents<SpriteRenderer>(item.itemObj)))
            return true;
        
        if (Overlaps(orgRect, GetComponents<MeshRenderer>(item.itemObj)))
            return true;
        
        if (Overlaps(orgRect, GetComponents<SkinnedMeshRenderer>(item.itemObj)))
            return true;
        
        return false;
    }

    private bool Overlaps<T>(Rect orgRect, List<T> render) where T : Renderer
    {
        if (render == null || render.Count == 0)
            return false;
        
        foreach (var rd in render)
        {
            Rect rect = new Rect(rd.transform.position.x-rd.bounds.size.x/2, rd.transform.position.y-rd.bounds.size.y/2, rd.bounds.size.x, rd.bounds.size.y);

            if (orgRect.Overlaps(rect))
                return true;
        }

        return false;
    }

    private bool Overlaps(Rect orgRect, Transform transform)
    {
        if (transform == null)
            return false;
        
        int size = 6;
        Rect rect = new Rect(transform.position.x-size/2, transform.position.y-size/2, size, size);

        if (orgRect.Overlaps(rect))
            return true;
        
        return false;
    }

    protected List<T> GetComponents<T>(GameObject obj) where T : Renderer
    {
        List<T> components = new List<T>();
        var com = obj.GetComponent<T>();
        if(com != null)
            components.Add(com);
        
        var comps = obj.GetComponentsInChildren<T>();
        if (comps != null && comps.Length > 0)
        {
            components.AddRange(comps);
        }

        return components;
    }
}