using UnityEngine.Assertions;
using System.Collections.Generic;
using UnityEngine;
using DragonU3DSDK;
using System;
namespace Decoration
{
    public class GameObjectPoolManager
    {
        private Transform _poolRoot;

        public Transform Root
        {
            get { return _poolRoot; }
        }

        private Dictionary<string, GameObjectPool> _gameObjectPools = new Dictionary<string, GameObjectPool>();

        private static GameObject _root = null;
        
        public GameObjectPoolManager(string poolName)
        {
            if (_root == null)
            {
                _root = new GameObject("PoolRoot");
                GameObject.DontDestroyOnLoad(_root);
            }
            
            _poolRoot = new GameObject(poolName).transform;
            _poolRoot.transform.SetParent(_root.transform);
            _poolRoot.transform.localPosition = Vector3.zero;
            _poolRoot.transform.localScale = Vector3.one;
        }

        public GameObjectPool GetGameObjectPool(string path, int initialSize = 1)
        {
            var gameObjectPoolKey = path;
            if (!_gameObjectPools.TryGetValue(gameObjectPoolKey, out var gameObjectPool))
            {
                gameObjectPool = new GameObjectPool(gameObjectPoolKey, _poolRoot, initialSize);
                _gameObjectPools.Add(gameObjectPoolKey, gameObjectPool);
            }

            return gameObjectPool;
        }

        public GameObject SpawnGameObject(string path)
        {
            var gameObjectPool = GetGameObjectPool(path);
            return gameObjectPool.SpawnObject();
        }

        public void PreloadGameObjectAsync(string path, int count, Action callback)
        {
            var gameObjectPool = GetGameObjectPool(path, 0);
            gameObjectPool.PreloadObjectAysnc(count, callback);
        }


        public void ClearObjectPool(string path)
        {
            var gameObjectPool = GetGameObjectPool(path);
            gameObjectPool.ClearObject();
        }

        public void Release()
        {
            foreach (var pair in _gameObjectPools)
            {
                ClearObjectPool(pair.Key);
            }
        }

        public void RecycleGameObject(GameObject gameObject)
        {
            if (!gameObject) return;

            var spawnedObjectInfo = gameObject.transform.GetComponent<SpawnedObjectInfo>();
            if (spawnedObjectInfo != null)
            {
                if (_gameObjectPools.TryGetValue(spawnedObjectInfo.m_AssetPath, out var gameObjectPool))
                {
                    gameObjectPool.RecycleObject(gameObject);
                }
                else
                {
                    DebugUtil.LogError("Error: Game Object pool for " + spawnedObjectInfo.m_AssetPath + " does not exist. Destroying GameObject");
                    UnityEngine.Object.Destroy(gameObject);
                }
            }
            else
            {
                DebugUtil.LogError("Error: Game Object pool for " + gameObject.name + " does not exist. Destroying GameObject");
                UnityEngine.Object.Destroy(gameObject);
            }
        }

        public void RecycleAllSpawnedObjects()
        {
            var gameObjectPoolArray = new GameObjectPool[_gameObjectPools.Count];
            _gameObjectPools.Values.CopyTo(gameObjectPoolArray, 0);
            for (var i = 0; i < gameObjectPoolArray.Length; i++)
            {
                gameObjectPoolArray[i].RecycleAllObjects();
            }
        }
    }
}