using System.Collections.Generic;
using System.IO;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Config;
using UnityEditor;
using UnityEngine;

namespace Decoration.DynamicMap
{
    [System.Serializable]
    public class Block
    {
        public int _areaId;
        public int _nodeId;

        public Block(int areaId, int nodeId)
        {
            _areaId = areaId;
            _nodeId = nodeId;
        }
    }
    
    [System.Serializable]
    public class Chunk
    {
        public int index;
        public int row;
        public int col;
        public float x;
        public float y;
        public float z;
        public float minX;
        public float minY;
        public float sizeW;
        public float sizeH;

        public List<Block> _blocks = new List<Block>();

        private Rect _rect;
        private bool _isRect = false;
        public Rect GetRect()
        {
            if (!_isRect)
            {
                _rect = new Rect(minX, minY, sizeW, sizeH);
                _isRect = true;
            }

            return _rect;
        }

        public bool ReferenceBlock(int areaId, int nodeId)
        {
            if (_blocks == null || _blocks.Count == 0)
                return false;

            return _blocks.Find(a => a._areaId == areaId && a._nodeId == nodeId) != null;
        }
    }

    [System.Serializable]
    public class DynamicMap
    {
        public int _mapId;
        public float _chunkWidth;
        public float _chunkHeight;
        public int _gridRow;
        public int _gridCol;
        public int _orgPosX;
        public int _orgPosY;
        
        public List<Chunk> _chunks = new List<Chunk>();
    }
    public class DynamicMapConfigManager : Singleton<DynamicMapConfigManager>
    {
        public float _chunkWidth;
        public float _chunkHeight;
        public int _gridRow;//行
        public int _gridCol;//列
        public int _orgPosX;
        public int _orgPosY;

        private Dictionary<int, DynamicMap> _dynamicMaps = new Dictionary<int, DynamicMap>();

        private DynamicMap _currentDynamicMap;

        public DynamicMap DynamicMap => _currentDynamicMap;
        
        public void InitConfig(int worldId)
        {
            _currentDynamicMap = null;
            
            TextAsset json = null;
            if (_dynamicMaps.ContainsKey(worldId))
            {
                _currentDynamicMap = _dynamicMaps[worldId];
                
                _chunkWidth = _currentDynamicMap._chunkWidth;
                _chunkHeight = _currentDynamicMap._chunkHeight;
                _gridRow = _currentDynamicMap._gridRow;
                _gridCol = _currentDynamicMap._gridCol;
                _orgPosX = _currentDynamicMap._orgPosX;
                _orgPosY = _currentDynamicMap._orgPosY;
                return;
            }
#if UNITY_EDITOR
            if (!Application.isPlaying)
                json = AssetDatabase.LoadAssetAtPath<TextAsset>($"Assets/Export/configs/Decoration/DynamicMap_{worldId}.json");
#endif
            if(json == null)
                json = ResourcesManager.Instance.LoadResource<TextAsset>(Path.Combine("configs/Decoration", $"DynamicMap_{worldId}"));
            
            if(json == null)
                return;
            
            DynamicMap dynamicMap = JsonUtility.FromJson<DynamicMap>(json?.text);

            _chunkWidth = dynamicMap._chunkWidth;
            _chunkHeight = dynamicMap._chunkHeight;
            _gridRow = dynamicMap._gridRow;
            _gridCol = dynamicMap._gridCol;
            _orgPosX = dynamicMap._orgPosX;
            _orgPosY = dynamicMap._orgPosY;
            
            _dynamicMaps.Add(worldId, dynamicMap);
            _currentDynamicMap = _dynamicMaps[worldId];
        }

        public Chunk GetChunk(int row, int col)
        {
            int index = row * _gridCol + col;

            return GetChunk(index);
        }

        public Chunk GetChunk(int index)
        {
            if (_currentDynamicMap == null)
                return null;
            
            if (index < 0 || index >= _currentDynamicMap._chunks.Count)
                return null;

            return _currentDynamicMap._chunks[index];
        }
    }
}