using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace Decoration.DynamicMap
{
    public partial class DynamicMapManager : Manager<DynamicMapManager>
    {
        private GameObject _dynamicObj;
        private int _dynamicMapIndex = -1;

        private int[][] _dynamicDir = new[]
        {
            new[] { 0, -1 },
            new[] { 0, 1 },
            new[] { -1, 0 },
            new[] { 1, 0 },
            new[] { -1, -1 },
            new[] { 1, -1 },
            new[] { 1, 1 },
            new[] { -1, 1 }
        };

        private int _dynamicNum = 1;

        private bool _isPauseLogic = false;

        public bool PauseLogic
        {
            get { return _isPauseLogic; }
            set { _isPauseLogic = value; }
        }
        
        public void InitDynamicObject(GameObject obj)
        {
            _dynamicObj = obj;
            _dynamicMapIndex = -1;
            
            InitLoad();
            
            DynamicLogic();
        }

        public void FixedUpdate()
        {
            if(_dynamicObj == null || !_dynamicObj.activeSelf)
                return;

            if(PauseLogic)
                return;
            
            DynamicLogic();
        }

        private void DynamicLogic()
        {
            if (_dynamicMapIndex > 0)
            {
                Chunk mainChunk = DynamicMapConfigManager.Instance.GetChunk(_dynamicMapIndex);
                if (mainChunk != null)
                {
                    if(mainChunk.GetRect().Contains(_dynamicObj.transform.position))
                        return;
                }
            }
            
            int index = CalculationDynamicIndex();
            if(index < 0)
                return;
            
            if(index == _dynamicMapIndex)
                return;
            
            Chunk chunk = DynamicMapConfigManager.Instance.GetChunk(index);
            if (chunk == null)
                return;

            var oldChunk = CalculateChunksForSudoku(_dynamicMapIndex);
            var newChunk = CalculateChunksForSudoku(index);
            _dynamicMapIndex = index;

            LoadChunk(oldChunk, newChunk);
        }

        public void ForceLoadCurrentChunk()
        {
            if(_dynamicMapIndex < 0)
                return;
            
            var oldChunk = CalculateChunksForSudoku(_dynamicMapIndex);
            LoadChunk(null, oldChunk, true);
        }
        
        private int CalculationDynamicIndex()
        {
            if (_dynamicObj == null)
                return -1;

            if (_dynamicMapIndex < 0)
                return CalculateDynamicIndexForAll();
            
            int index = CalculateDynamicIndexForSudoku();
            if (index > 0)
                return index;

            return CalculateDynamicIndexForAll();
        }
        
        private int CalculateDynamicIndexForSudoku()
        {
            if (_dynamicObj == null || _dynamicMapIndex == -1)
                return -1;

            var chunks = CalculateChunksForSudoku(_dynamicMapIndex);
            if (chunks == null || chunks.Count == 0)
                return -1;
            
            foreach (var chunk in chunks)
            {
                if(!chunk.GetRect().Contains(_dynamicObj.transform.position))
                    continue;

                return chunk.index;
            }

            return -1;
        }

        private List<Chunk> CalculateChunksForSudoku(int index)
        {
            Chunk mainChunk = DynamicMapConfigManager.Instance.GetChunk(index);
            if (mainChunk == null)
                return null;

            List<Chunk> chunks = new List<Chunk>();
            chunks.Add(mainChunk);
            
            for (int i = 0; i < _dynamicNum; i++)
            {
                foreach (var dir in _dynamicDir)
                {
                    int row = dir[0];
                    int col = dir[1];

                    if (mainChunk.col == 0)
                    {
                        if(col < 0)
                            continue;
                    }
                    if (mainChunk.col == DynamicMapConfigManager.Instance._gridCol - 1)
                    {
                        if(col > 0)
                            continue;
                    }

                    if (mainChunk.row == 0)
                    {
                        if(row < 0)
                            continue;
                    }

                    if (mainChunk.row == DynamicMapConfigManager.Instance._gridRow - 1)
                    {
                        if(row > 0)
                            continue;
                    }
                    
                    row = row != 0 ? row+(i*row) : row;
                    col = col != 0 ? col+(i*col) : col;
                    
                    var chunk = DynamicMapConfigManager.Instance.GetChunk(mainChunk.row+row, mainChunk.col+col);
                    if(chunk == null)
                        continue;
                
                    chunks.Add(chunk);
                }
            }

            return chunks;
        }

        private Dictionary<int, Dictionary<int,int>> CalculateAreasForSudoku(int index)
        {
            List<Chunk> chunks = CalculateChunksForSudoku(index);
            if (chunks == null || chunks.Count == 0)
                return null;

            Dictionary<int, Dictionary<int,int>> areas = new Dictionary<int, Dictionary<int,int>>();
            chunks.ForEach(a =>
            {
               a._blocks.ForEach(b =>
               {
                   if (!areas.ContainsKey(b._areaId))
                       areas[b._areaId] = new Dictionary<int, int>();
                   
                   if(!areas[b._areaId].ContainsKey(b._nodeId))
                       areas[b._areaId].Add(b._nodeId, 0);

                   areas[b._areaId][b._nodeId]++;
               });
            });
            
            return areas;
        }

        
        private int CalculateDynamicIndexForAll()
        {
            if (_dynamicObj == null)
                return -1;

            if (DynamicMapConfigManager.Instance.DynamicMap == null)
                return -1;
            
            foreach (var chunk in DynamicMapConfigManager.Instance.DynamicMap._chunks)
            {
                if(!chunk.GetRect().Contains(_dynamicObj.transform.position))
                    continue;

                return chunk.index;
            }

            return -1;
        }
    }
}