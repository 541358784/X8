using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonU3DSDK.Asset;
using DragonU3DSDK;
using System;
namespace Decoration
{
    public interface PoolableComponent
    {
        void OnSpawn();

        void OnDespawn();
    }

    public class SpawnedObjectInfo : MonoBehaviour
    {
        public string m_AssetPath;
    }

    public class GameObjectPool
    {
        // 空闲的对象
        private Queue<GameObject> m_PooledObjects;

        // 使用中的对象
        private List<GameObject> m_spawnedObjects;

        // 所有缓存对象的父节点
        private Transform m_PooledObjectParentTransform;

        private int m_LastCreatedAmount = 0;

        private string m_AssetPath;

        public GameObjectPool(string assetPath, Transform parentTr, int initialSize = 0)
        {
            m_AssetPath = assetPath;
            m_PooledObjects = new Queue<GameObject>();
            m_spawnedObjects = new List<GameObject>();

            m_PooledObjectParentTransform = new GameObject(m_AssetPath).transform;
            CommonUtils.AddChildTo(parentTr, m_PooledObjectParentTransform);

            if (initialSize > 0)
            {
                InstantiatePooledObjects(initialSize);
            }
        }

        public GameObject SpawnObject()
        {
            try
            {
                if (m_PooledObjects.Count == 0)
                {
                    //资源不存在时，不停扩大缓存池导致内存溢出
                    if (m_LastCreatedAmount > 0 && m_spawnedObjects.Count == 0) return null;

                    InstantiatePooledObjects(Mathf.Max(1, m_LastCreatedAmount * 2));
                }

                if (m_PooledObjects.Count == 0) return null;

                var gameObject = m_PooledObjects.Dequeue();
                gameObject.SetActive(true);
                m_spawnedObjects.Add(gameObject);

                var spawnedObjectInfo = gameObject.GetOrCreateComponent<SpawnedObjectInfo>();
                spawnedObjectInfo.m_AssetPath = m_AssetPath;

                var pooledObjectComponent = gameObject.GetComponent<PoolableComponent>();
                if (pooledObjectComponent != null)
                    pooledObjectComponent.OnSpawn();

                return gameObject;
            }
            catch (Exception e)
            {
                DebugUtil.LogError(e.ToString());
            }

            return null;
        }

        public void RecycleObject(GameObject gameObject)
        {
            if (m_spawnedObjects.Contains(gameObject) && !m_PooledObjects.Contains(gameObject))
            {
                var pooledObjectComponent = gameObject.transform.GetComponent<PoolableComponent>();
                pooledObjectComponent?.OnDespawn();

                m_spawnedObjects.Remove(gameObject);
                gameObject.SetActive(false);
                gameObject.transform.SetParent(m_PooledObjectParentTransform, false);
                m_PooledObjects.Enqueue(gameObject);

                //DebugUtil.LogError("RecycleObject:" + gameObject.name + ":" + gameObject.GetHashCode() + " -- " + m_PooledObjects.Count);
            }
            else
            {
                DebugUtil.LogError("##############################################################");
                DebugUtil.LogError("#### 试图重复归还对象:" + gameObject.name + " ####");
                DebugUtil.LogError("##############################################################");
            }
        }

        public void RecycleAllObjects()
        {
            for (int i = 0; i < m_spawnedObjects.Count; i++)
            {
                RecycleObject(m_spawnedObjects[i]);
            }
        }

        private GameObject InstantiatePooledObject()
        {
            var o = ResourcesManager.Instance.LoadResource<GameObject>(m_AssetPath, addToCache: false);
            if (o == null)
            {
                DebugUtil.LogError("!!!!!!!!!!! prefab丢失:" + m_AssetPath);
                return null;
            }

            var gameObject = UnityEngine.Object.Instantiate(o, m_PooledObjectParentTransform, false);
            m_PooledObjects.Enqueue(gameObject);
            //DebugUtil.LogError(":::InstantiatePooledObject:" + gameObject.name);
            gameObject.SetActive(false);
            return gameObject;
        }

        private void InstantiatePooledObjects(int numberOfObjects)
        {
            for (int i = 0; i < numberOfObjects; i += 1)
            {
                InstantiatePooledObject();
            }

            //释放加载的Prefab资源
            OpUtils.UnloadObjFromBundleManager(m_AssetPath);

            m_LastCreatedAmount = numberOfObjects;
        }

        public void PreloadObjectAysnc(int count, Action callback)
        {
            ResourcesManager.Instance.LoadResourceAsync<GameObject>(m_AssetPath, (obj) =>
            {
                if (obj == null)
                {
                    DebugUtil.LogError("!!!!!!!!!!! prefab丢失:" + m_AssetPath);
                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        GameObject gameObject = UnityEngine.Object.Instantiate(obj, m_PooledObjectParentTransform, false);
                        m_PooledObjects.Enqueue(gameObject);
                        //DebugUtil.LogError(":::PreloadObjectAysnc:" + gameObject.name);
                        gameObject.SetActive(false);
                    }

                    callback?.Invoke();
                }
            });
        }

        public void ClearObject()
        {
            // 空闲的对象
            while (m_PooledObjects.Count > 0) GameObject.Destroy(m_PooledObjects.Dequeue());

            // 使用中的对象
            while (m_spawnedObjects.Count > 0) GameObject.Destroy(m_spawnedObjects.Dequeue());

            //重置
            m_LastCreatedAmount = 0;
        }
    }
}