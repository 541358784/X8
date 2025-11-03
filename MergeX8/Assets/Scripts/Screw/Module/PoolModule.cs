using System;
using Decoration;
using UnityEngine;

namespace Screw.Module
{
    public class PoolModule : Singleton<PoolModule>
    {
        private GameObjectPoolManager _poolManager = new GameObjectPoolManager("ScrewObjectPoolRoot");

        public void CreatePool(string path, int num)
        {
            _poolManager.GetGameObjectPool(path, num);
        }
        
        public GameObject SpawnGameObject(string path)
        {
            return _poolManager.SpawnGameObject(path);
        }

        public void ClearObjectPool(string path)
        {
            var gameObjectPool = _poolManager.GetGameObjectPool(path);
            if(gameObjectPool == null)
                return;
            
            gameObjectPool.ClearObject();
        }

        public void Release()
        {
            _poolManager.Release();
        }

        public void RecycleGameObject(GameObject gameObject)
        {
            _poolManager.RecycleGameObject(gameObject);
        }

        public void RecycleAllSpawnedObjects()
        {
            _poolManager.RecycleAllSpawnedObjects();
        }
    }
}