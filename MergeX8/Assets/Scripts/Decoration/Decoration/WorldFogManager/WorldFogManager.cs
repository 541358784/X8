using System;
using System.Collections.Generic;
using Deco.World;
using DG.Tweening;
using DragonU3DSDK.Asset;
using SomeWhere;
using UnityEngine;

namespace Decoration.WorldFogManager
{
    public class WorldFogManager : Manager<WorldFogManager>
    {
        private int _worldId = -1;

        private FogMap _fogMap;
        public FogMap fogMap
        {
            get
            {
                return _fogMap;
            }
            private set
            {
                _fogMap = value;
            }
        }

        private Dictionary<int, List<SpriteRenderer>> _areaRenders = new Dictionary<int, List<SpriteRenderer>>();
        
        private string FOG_PATH => Utils.PathPrefabMap(_worldId) + "/FogMap";

        private string AtlasName = "WorldAtlas";
        
        public void Load(int world, Transform parent)
        {
            if (fogMap != null)
            {
                if(_worldId == world)
                    return;
                else
                    UnLoad();
            }

            _worldId = world;
            var fogPrefab = ResourcesManager.Instance.LoadResource<GameObject>(FOG_PATH);
            if (fogPrefab == null)
                return;
            
            var fogObj = GameObject.Instantiate(fogPrefab, parent);
            fogObj.gameObject.SetActive(true);
            fogObj.transform.localScale = Vector3.one;
            fogObj.transform.localRotation = Quaternion.identity;
            fogObj.transform.localPosition = Vector3.zero;
            fogMap = fogObj.GetComponent<FogMap>();
        }

        public void UnLoad()
        {
            if(fogMap == null)
                return;

            foreach (var renders in _areaRenders)
            {
                foreach (var red in renders.Value)
                {
                    DestroyImmediate(red);
                }
            }
            DestroyImmediate(fogMap.gameObject);
            
            _areaRenders.Clear();
            fogMap = null;
        }

        public void Init()
        {
            if (fogMap == null)
                return;

            if(fogMap._fogLayers == null)
                return;
            
            foreach (var layer in fogMap._fogLayers)
            {
                if(layer == null)
                    continue;
                
                if (!DecoWorld.AreaLib.ContainsKey(layer._areaId))
                    continue;
                
                var decoArea = DecoWorld.AreaLib[layer._areaId];
                if(decoArea == null)
                    continue;

                if (decoArea.Unlocked)
                {
                    if (_areaRenders.ContainsKey(layer._areaId) && _areaRenders[layer._areaId] != null)
                    {
                        _areaRenders[layer._areaId].ForEach(a=>a.gameObject.SetActive(false));
                    }
                    continue;
                }

                if(layer._fogChunks == null)
                    continue;

                if (!_areaRenders.ContainsKey(layer._areaId))
                    _areaRenders[layer._areaId] = new List<SpriteRenderer>();
               
                var renders = _areaRenders[layer._areaId];
                if(renders.Count > 0)
                    continue;
                
                foreach (var chunks in layer._fogChunks)
                {
                   if(chunks == null)
                       continue;

                   foreach (var blocks in chunks._blocks)
                   {
                       if(blocks == null)
                           continue;
                       
                       if(blocks._gameObject == null)
                           continue;
                       
                       renders.Add(blocks._gameObject.AddComponent<SpriteRenderer>());
                       renders[renders.Count-1].sprite = ResourcesManager.Instance.GetSpriteVariant(AtlasName, blocks._imageName);
                   }
                }
            }
        }

        public void Hide(int areaId, bool isAnim = true, Action action = null, float hideTime = 0.5f)
        {
            if (areaId == 110)
            {
                if(ABTest.ABTestManager.Instance.IsLockMap())
                  {
                      action?.Invoke();
                      return;
                   }
            }
            
            if (fogMap == null)
            {
                action?.Invoke();
                return;
            }

            GameObject parent = GetAreaParent(areaId);
            if (parent == null)
            {
                action?.Invoke();
                return;
            }

            if (!isAnim)
            {
                parent.gameObject.SetActive(false);
                action?.Invoke();
                return;
            }

            if (!_areaRenders.ContainsKey(areaId))
            {
                action?.Invoke();
                return;
            }

            foreach (var render in _areaRenders[areaId])
            {
                var rder = render;
                render.transform.DOLocalMoveY(render.transform.localPosition.y + 2f, hideTime*2);
                render.DOFade(0, hideTime);
            }

            StartCoroutine(CommonUtils.DelayWork(hideTime, () =>
            {
                action?.Invoke();
            }));
        }

        private GameObject GetAreaParent(int areaId)
        {
            if(fogMap == null)
                return null;

            if (fogMap._fogLayers == null)
                return null;
            
            var layer = fogMap._fogLayers.Find(a => a._areaId == areaId);
            if (layer == null)
                return null;

            return layer._gameObject;
        }
    }
}