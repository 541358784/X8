using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePool
{
    public class SubPool
    {
        public string poolName = "";
        public int poolMaxCount = 0;
        public ObjectPoolManager.OnSpawn onSpawn = null;
        public List<PoolData> freeQueue = new List<PoolData>();
        public Dictionary<GameObject, PoolData> useQueue = new Dictionary<GameObject, PoolData>();

        public void InitPool(string poolName, int poolMaxCount, ObjectPoolManager.OnSpawn onSpawn)
        {
            this.poolName = poolName;
            this.poolMaxCount = poolMaxCount;
            this.onSpawn = onSpawn;

            for (int i = 0; i < poolMaxCount; i++)
            {
                PoolData poolData = new PoolData();
                poolData.useTime = 0;
                poolData.poolObj = onSpawn(poolName);

                poolData.poolObj.gameObject.SetActive(false);
                poolData.poolObj.transform.SetParent(ObjectPoolManager.Instance.PoolManager.transform);
                poolData.poolObj.transform.localPosition = Vector3.zero;

                freeQueue.Add(poolData);
            }
        }

        public void Update()
        {
            if (freeQueue == null || freeQueue.Count == 0)
                return;

            if (freeQueue.Count <= poolMaxCount)
                return;

            for (int i = 0; i < freeQueue.Count; i++)
            {
                freeQueue[i].Update();

                if (!freeQueue[i].IsTimeOut())
                    continue;

                GameObject.Destroy(freeQueue[i].poolObj);
                freeQueue.RemoveAt(i);
                i = i - 1;

                if (freeQueue.Count <= poolMaxCount)
                    return;
            }
        }

        public GameObject Spawn()
        {
            if (freeQueue.Count > 0)
            {
                PoolData poolData = freeQueue.Dequeue();
                poolData.poolObj.gameObject.SetActive(true);

                poolData.useTime = Time.realtimeSinceStartup;

                AddUseQueue(poolData);

                return poolData.poolObj;
            }

            {
                PoolData poolData = new PoolData();
                poolData.useTime = Time.realtimeSinceStartup;
                poolData.poolObj = onSpawn(poolName);
                poolData.poolObj.gameObject.SetActive(true);

                AddUseQueue(poolData);

                return poolData.poolObj;
            }
        }

        public void DeSpawn(GameObject poolObj)
        {
            if (!useQueue.ContainsKey(poolObj))
                return;

            PoolData poolData = useQueue[poolObj];
            useQueue.Remove(poolObj);

            if (poolData.poolObj == null)
                return;

            poolData.poolObj.SetActive(false);
            poolData.poolObj.transform.SetParent(ObjectPoolManager.Instance.PoolManager.transform);
            poolData.poolObj.transform.localPosition = Vector3.zero;
            poolData.poolObj.transform.transform.localScale = Vector3.one;
            //poolData.poolObj.SetActive(true);

            poolData.UpdateUseTime();
            freeQueue.Enqueue(poolData);
        }

        private void AddUseQueue(PoolData poolData)
        {
            if (poolData == null)
                return;

            if (useQueue.ContainsKey(poolData.poolObj))
                return;

            poolData.useTime = Time.realtimeSinceStartup;
            useQueue.Add(poolData.poolObj, poolData);
        }

        public void Destroy()
        {
            foreach (var poolData in freeQueue)
            {
                GameObject.Destroy(poolData.poolObj);
            }
            freeQueue.Clear();
            
            foreach (var kv in useQueue)
            {
                GameObject.Destroy(kv.Value.poolObj);
            }
            useQueue.Clear();
        }
    }

    public class PoolData
    {
        public GameObject poolObj = null;
        public float useTime = 0f;

        public bool IsTimeOut()
        {
            return Time.realtimeSinceStartup - useTime > ObjectPoolManager.TIMEOUT;
        }

        public void UpdateUseTime()
        {
            useTime = Time.realtimeSinceStartup;
        }

        public void Update()
        {
        }
    }

    public partial class ObjectPoolManager : Manager<ObjectPoolManager>
    {
        public static int TIMEOUT = 2 * 60;

        public delegate GameObject OnSpawn(string path);

        private const int updateLimit = 5;
        private float updateTime = 0f;

        private Dictionary<string, SubPool> subPoolCache = new Dictionary<string, SubPool>();

        private const string poolManagerName = "ObjectPoolManager";

        private GameObject poolManager = null;

        public GameObject PoolManager
        {
            get { return poolManager; }
        }
        public void CreatePool(string poolName, int poolMaxCount, OnSpawn onSpawn)
        {
            Init();

            if (subPoolCache.ContainsKey(poolName))
            {
                //DragonU3DSDK.DebugUtil.LogWarning("重复创建池　" + poolName + "/" + poolMaxCount);
                return;
            }

            if (onSpawn == null)
            {
                DragonU3DSDK.DebugUtil.LogError("创建池 回掉是NULL　" + poolName + "/" + poolMaxCount);
                return;
            }

            SubPool subPool = new SubPool();
            subPool.InitPool(poolName, poolMaxCount, onSpawn);
            subPoolCache.Add(poolName, subPool);
        }

        public void Update()
        {
            updateTime += Time.deltaTime;
            if (updateTime < updateLimit)
                return;

            updateTime -= updateLimit;

            foreach (KeyValuePair<string, SubPool> kv in subPoolCache)
            {
                kv.Value.Update();
            }
        }

        public GameObject Spawn(string poolName)
        {
            SubPool subPool = GetSubPool(poolName);
            if (subPool == null)
                return null;

            return subPool.Spawn();
        }

        public void DeSpawn(string poolName, GameObject poolObj)
        {
            if (poolObj == null)
                return;

            SubPool subPool = GetSubPool(poolName);
            if (subPool == null)
                return;

            subPool.DeSpawn(poolObj);
        }

        public void DestroyPool(string poolName)
        {
            SubPool subPool = GetSubPool(poolName);
            if (subPool == null)
                return;

            subPool.Destroy();

            subPoolCache.Remove(poolName);
        }

        private SubPool GetSubPool(string poolName)
        {
            if (!subPoolCache.ContainsKey(poolName))
                return null;

            return subPoolCache[poolName];
        }

        private void Init()
        {
            if (poolManager != null)
                return;

            poolManager = new GameObject();
            poolManager.name = poolManagerName;
            DontDestroyOnLoad(poolManager);
            PoolManager.transform.position = new Vector3(10000, 10000, 1);
        }
    }
}