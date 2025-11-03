using System;
using System.Collections;
using System.Collections.Generic;
using Deco.Item;
using Deco.Node;
using Decoration.Bubble;
using DragonU3DSDK.Asset;
using Farm.Model;
using SomeWhere;
using UnityEngine;
using UnityEngine.U2D;

namespace Decoration.DynamicMap
{
    public class DynamicLoadData
    {
        public float _time;
        public Chunk _chunk;
        public bool _isForce;

        public DynamicLoadData(Chunk chunk, float time, bool isForce)
        {
            _chunk = chunk;
            _time = time;
            _isForce = isForce;
        }
    }
    
    public partial class DynamicMapManager : Manager<DynamicMapManager>
    {
        private float _unLoadTime = 3;
        private List<DynamicLoadData> _unLoadData = new List<DynamicLoadData>();
        private List<DynamicLoadData> _loadData = new List<DynamicLoadData>();
        private int _dailyLoadCount = 4;
        private Coroutine _loadCoroutine;
        private int _unloadIndex = -1;
        private Dictionary<int, Dictionary<int, int>> _currentChunkAreas = null;
        private Dictionary<int, SpriteAtlas> _areaAtlas = new Dictionary<int, SpriteAtlas>();
        private int _atlasCacheCount = 4;
        private float _unLoadAtlasTime = 3;
        private float _unLoadAtlasRecordTime = 0;
        private Dictionary<DecoItem, DecoItem> _loadItems = new Dictionary<DecoItem, DecoItem>();
        
        public void SetUnLoadTime(float time)
        {
            _unLoadTime = time;
        }
        
        public void SetDailyLoadCount(int count)
        {
            _dailyLoadCount = count;
        }

        public void SetAtlasConfig(int cacheCount, float unLoadTime)
        {
            _atlasCacheCount = cacheCount;
            _unLoadAtlasTime = unLoadTime;
        }
        
        private void InitLoad()
        {
            _unLoadData.Clear();
            _loadData.Clear();
            _loadItems.Clear();
            
            _unloadIndex = -1;
            _currentChunkAreas = null;
            
            if(_loadCoroutine != null)
                StopCoroutine(_loadCoroutine);
            
            _loadCoroutine = StartCoroutine(LoadEnumerator());
            
            CancelInvoke("UnLoadInvoke");
            InvokeRepeating("UnLoadInvoke", 0, 0.5f);
        }

        private void LoadChunk(List<Chunk> oldChunk, List<Chunk> newChunk, bool isForce = false)
        {
            if (oldChunk != null)
            {
                for (int i = 0; i < oldChunk.Count; i++)
                {
                    if(newChunk.Find(a=>a == oldChunk[i]) == null)
                        continue;
                    
                    oldChunk.RemoveAt(i);
                    i--;
                }
                
                oldChunk.ForEach(a =>
                {
                    _unLoadData.Add(new DynamicLoadData(a, Time.realtimeSinceStartup,isForce));
                });
            }

            if (newChunk != null)
            {
                for (int i = 0; i < newChunk.Count; i++)
                {
                    int findIndex = _unLoadData.FindIndex(a => a._chunk == newChunk[i]);
                    if(findIndex < 0)
                        continue;
                    
                    _unLoadData.RemoveAt(findIndex);
                    newChunk.RemoveAt(i);
                    i--;
                }
                
                newChunk.ForEach(a=>
                {
                    _loadData.Add(new DynamicLoadData(a, Time.realtimeSinceStartup,isForce));
                });
            }
        }

        private IEnumerator LoadEnumerator()
        {
            while (true)
            {
                if (_loadData.Count == 0)
                    yield return new WaitForEndOfFrame();
                int index = 0;
                for (int i = 0; i < _loadData.Count; i++)
                {
                    if (_loadData[i]._chunk._blocks == null || _loadData[i]._chunk._blocks.Count == 0)
                    {
                        _loadData.RemoveAt(i);
                        i--;
                        continue;
                    }
            
                    foreach (var block in _loadData[i]._chunk._blocks)
                    {
                        DecoNode decoNode =  DecoManager.Instance.FindNode(block._nodeId);
                        if (decoNode == null)
                            continue;

                        if (decoNode._stage != null && decoNode._stage.Area != null)
                        {
                            if (!decoNode._stage.Area.IsUnlock && !decoNode.Config.isLockShow)
                                continue;
                        }
                        
                        if(!_loadData[i]._isForce && !DecoExtendAreaManager.Instance.CanShowArea(decoNode.Stage.Area.Config.id))
                            continue;

                        if (!_areaAtlas.ContainsKey(block._areaId))
                            _areaAtlas[block._areaId] =ResourcesManager.Instance.LoadSpriteAtlasVariant(AssetCheckManager.GetAreaBuildAtlasName(block._areaId));
                        
                        if (decoNode.CurrentItem != null && decoNode.CurrentItem.Graphic.gameObject == null && !_loadItems.ContainsKey(decoNode.CurrentItem))
                        {
                            index++;
                            _loadItems.Add(decoNode.CurrentItem, decoNode.CurrentItem);

                            int currentItem = decoNode._data._storage.CurrentItemId;
                            decoNode.CurrentItem.AsyncLoadGraphic(decoNode.GameObject, decoNode.IsPreview, () =>
                            {
                                if(decoNode.IsPreview)
                                    decoNode.CurrentItem.GameObject?.SetActive(false);
                                else
                                    decoNode.CurrentItem.Show(true);

                                if (decoNode._data._storage.CurrentItemId != currentItem)
                                {
                                    decoNode.CurrentItem.GameObject?.SetActive(false);
                                }
                                
                                _loadItems.Remove(decoNode.CurrentItem);
                                
                                if (FarmModel.Instance.IsFarmModel())
                                {
                                    FarmModel.Instance.Load(decoNode);
                                }
                            });
                        }
                        NodeBubbleManager.Instance.OnLoadBubble(decoNode);
                        
                        // if (index >= _dailyLoadCount)
                        // {
                        //     yield return new WaitForEndOfFrame();
                        //     index = 0;
                        // }
                    }
            
                    _loadData.RemoveAt(i);
                    i--;
                    // if (index >= _dailyLoadCount)
                    // {
                    //     yield return new WaitForEndOfFrame();
                    //     index = 0;
                    // }
                }

                //yield return new WaitForEndOfFrame();
            }
        }

        private void UnLoadInvoke()
        {
            if(_unLoadData == null)
                return;

            if (_unloadIndex != _dynamicMapIndex || _unloadIndex < 0)
            {
                _currentChunkAreas = CalculateAreasForSudoku(_dynamicMapIndex);
                _unloadIndex = _dynamicMapIndex;
            }
            
            for (int i = 0; i < _unLoadData.Count; i++)
            {
                if (_unLoadData[i]._chunk._blocks == null || _unLoadData[i]._chunk._blocks.Count == 0)
                {
                    _unLoadData.RemoveAt(i);
                    i--;
                    continue;
                }
                
                if(Time.realtimeSinceStartup - _unLoadData[i]._time < _unLoadTime)
                    continue;

                bool isWaitAsync = false;
                foreach (var block in _unLoadData[i]._chunk._blocks)
                {
                    if (_currentChunkAreas != null)
                    {
                        if(_currentChunkAreas.ContainsKey(block._areaId) && _currentChunkAreas[block._areaId].ContainsKey(block._nodeId))
                            continue;
                    }
                    
                    DecoNode decoNode =  DecoManager.Instance.FindNode(block._nodeId);
                    if(decoNode == null)
                        continue;

                    if (FarmModel.Instance.IsFarmModel())
                    {
                        FarmModel.Instance.Unload(decoNode);
                    }
                    
                    if (decoNode.CurrentItem != null)
                    {
                        if(!decoNode.CurrentItem.Graphic.IsAsync)
                            decoNode.CurrentItem.UnloadGraphic();
                        else
                            isWaitAsync = true;
                    }
                    
                    NodeBubbleManager.Instance.UnLoadBubble(decoNode);
                }
                
                if(isWaitAsync)
                    continue;
                
                _unLoadData.RemoveAt(i);
                i--;
            }

            UnLoadAreaBuildAtlas();
        }

        private void UnLoadAreaBuildAtlas()
        {
            if(Time.realtimeSinceStartup - _unLoadAtlasRecordTime < _unLoadAtlasTime)
                return;
            
            _unLoadAtlasRecordTime = Time.realtimeSinceStartup;
            
            if(_areaAtlas.Count <= _atlasCacheCount)
                return;

            List<int> keys = new List<int>(_areaAtlas.Keys);
            for (int i = 0; i < keys.Count; i++)
            {
                if (_currentChunkAreas.ContainsKey(keys[i]))
                {
                    keys.RemoveAt(i);
                    i--;
                    continue;
                }

                foreach (var data in _unLoadData)
                {
                    if(data._chunk == null)
                        continue;
                    
                    if(data._chunk._blocks == null || data._chunk._blocks.Count == 0)
                        continue;

                    var block = data._chunk._blocks.Find(a => a._areaId == keys[i]);
                    if (block != null)
                    {
                        keys.RemoveAt(i);
                        i--;
                        break;
                    }
                }
            }

            if(keys.Count == 0)
                return;

            int removeAreaId = keys.RandomPickOne();
            _areaAtlas.Remove(removeAreaId);
            OpUtils.UnloadSpriteAtlas(AssetCheckManager.GetAreaBuildAtlasName(removeAreaId));
            Resources.UnloadUnusedAssets();
        }
    }
}