using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public sealed class ObjectPoolManager : Manager<ObjectPoolManager>
{
    public enum StartupPoolMode
    {
        Awake,
        Start,
        CallManually
    };

    // [System.Serializable]
    public class StartupPool
    {
        public int size;
        public GameObject prefab;
    }

    // static ObjectPoolManager this;
    static List<GameObject> tempList = new List<GameObject>();

    Dictionary<GameObject, List<GameObject>> pooledObjects = new Dictionary<GameObject, List<GameObject>>();
    Dictionary<GameObject, GameObject> spawnedObjects = new Dictionary<GameObject, GameObject>();

    public StartupPoolMode startupPoolMode;
    public StartupPool[] startupPools;

    bool startupPoolsCreated;

    protected override void InitImmediately()
    {
        CreateStartupPools();
    }

    // void Awake()
    // {
    // 	this = this;
    // 	if (startupPoolMode == StartupPoolMode.Awake)
    // 		CreateStartupPools();
    // }

    // void Start()
    // {
    // 	if (startupPoolMode == StartupPoolMode.Start)
    // 		CreateStartupPools();
    // }

    public static void CreateStartupPools()
    {
        if (!ObjectPoolManager.Instance.startupPoolsCreated)
        {
            ObjectPoolManager.Instance.startupPoolsCreated = true;
            var pools = ObjectPoolManager.Instance.startupPools;
            if (pools != null && pools.Length > 0)
                for (int i = 0; i < pools.Length; ++i)
                    CreatePool(pools[i].prefab, pools[i].size);
        }
    }

    public static void CreatePool<T>(T prefab, int initialPoolSize) where T : Component
    {
        CreatePool(prefab.gameObject, initialPoolSize);
    }

    public static void CreatePool(GameObject prefab, int initialPoolSize)
    {
        if (prefab != null && !ObjectPoolManager.Instance.pooledObjects.ContainsKey(prefab))
        {
            var list = new List<GameObject>();
            ObjectPoolManager.Instance.pooledObjects.Add(prefab, list);

            if (initialPoolSize > 0)
            {
                bool active = prefab.activeSelf;
                prefab.SetActive(false);
                Transform parent = ObjectPoolManager.Instance.transform;
                while (list.Count < initialPoolSize)
                {
                    var obj = (GameObject) Object.Instantiate(prefab);
                    obj.transform.parent = parent;
                    list.Add(obj);
                }

                prefab.SetActive(active);
            }
        }
    }

    public static T Spawn<T>(T prefab, Transform parent, Vector3 position, Quaternion rotation) where T : Component
    {
        return Spawn(prefab.gameObject, parent, position, rotation).GetComponent<T>();
    }

    public static T Spawn<T>(T prefab, Vector3 position, Quaternion rotation) where T : Component
    {
        return Spawn(prefab.gameObject, null, position, rotation).GetComponent<T>();
    }

    public static T Spawn<T>(T prefab, Transform parent, Vector3 position) where T : Component
    {
        return Spawn(prefab.gameObject, parent, position, Quaternion.identity).GetComponent<T>();
    }

    public static T Spawn<T>(T prefab, Vector3 position) where T : Component
    {
        return Spawn(prefab.gameObject, null, position, Quaternion.identity).GetComponent<T>();
    }

    public static T Spawn<T>(T prefab, Transform parent) where T : Component
    {
        return Spawn(prefab.gameObject, parent, Vector3.zero, Quaternion.identity).GetComponent<T>();
    }

    public static T Spawn<T>(T prefab) where T : Component
    {
        return Spawn(prefab.gameObject, null, Vector3.zero, Quaternion.identity).GetComponent<T>();
    }

    public static GameObject Spawn(GameObject prefab, Transform parent, Vector3 position, Quaternion rotation)
    {
        List<GameObject> list;
        Transform trans;
        GameObject obj;
        if (ObjectPoolManager.Instance.pooledObjects.TryGetValue(prefab, out list))
        {
            obj = null;
            if (list.Count > 0)
            {
                while (obj == null && list.Count > 0)
                {
                    obj = list[0];
                    list.RemoveAt(0);
                }

                if (obj != null)
                {
                    trans = obj.transform;
                    trans.parent = parent;
                    trans.localPosition = position;
                    trans.localRotation = rotation;
                    trans.localScale = new Vector3(1, 1, 1);
                    obj.SetActive(true);
                    ObjectPoolManager.Instance.spawnedObjects.Add(obj, prefab);
                    return obj;
                }
            }

            obj = (GameObject) Object.Instantiate(prefab);
            trans = obj.transform;
            trans.parent = parent;
            trans.localPosition = position;
            trans.localRotation = rotation;
            trans.localScale = new Vector3(1, 1, 1);
            ObjectPoolManager.Instance.spawnedObjects.Add(obj, prefab);
            return obj;
        }
        else
        {
            obj = (GameObject) Object.Instantiate(prefab);
            trans = obj.GetComponent<Transform>();
            trans.parent = parent;
            trans.localPosition = position;
            trans.localRotation = rotation;
            trans.localScale = new Vector3(1, 1, 1);
            return obj;
        }
    }

    public static GameObject Spawn(GameObject prefab, Transform parent, Vector3 position)
    {
        return Spawn(prefab, parent, position, Quaternion.identity);
    }

    public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        return Spawn(prefab, null, position, rotation);
    }

    public static GameObject Spawn(GameObject prefab, Transform parent)
    {
        return Spawn(prefab, parent, Vector3.zero, Quaternion.identity);
    }

    public static T Spawn<T>(GameObject prefab, Transform parent) where T : MonoBehaviour
    {
        var obj = Spawn(prefab, parent, Vector3.zero, Quaternion.identity);
        if (obj.GetComponent<T>() == null)
        {
            obj.AddComponent<T>();
        }

        return obj.GetComponent<T>();
    }

    public static GameObject Spawn(GameObject prefab, Vector3 position)
    {
        return Spawn(prefab, null, position, Quaternion.identity);
    }

    public static GameObject Spawn(GameObject prefab)
    {
        return Spawn(prefab, null, Vector3.zero, Quaternion.identity);
    }

    public static void Recycle<T>(T obj) where T : Component
    {
        Recycle(obj.gameObject);
    }

    public static void Recycle(GameObject obj)
    {
        GameObject prefab;
        if (ObjectPoolManager.Instance.spawnedObjects.TryGetValue(obj, out prefab))
            Recycle(obj, prefab);
        else
            Object.Destroy(obj);
    }

    static void Recycle(GameObject obj, GameObject prefab)
    {
        ObjectPoolManager.Instance.pooledObjects[prefab].Add(obj);
        ObjectPoolManager.Instance.spawnedObjects.Remove(obj);
        obj.transform.parent = ObjectPoolManager.Instance.transform;
        obj.SetActive(false);
    }

    public static void RecycleAll<T>(T prefab) where T : Component
    {
        RecycleAll(prefab.gameObject);
    }

    public static void RecycleAll(GameObject prefab)
    {
        foreach (var item in ObjectPoolManager.Instance.spawnedObjects)
            if (item.Value == prefab)
                tempList.Add(item.Key);
        for (int i = 0; i < tempList.Count; ++i)
            Recycle(tempList[i]);
        tempList.Clear();
    }

    public static void RecycleAll()
    {
        tempList.AddRange(ObjectPoolManager.Instance.spawnedObjects.Keys);
        for (int i = 0; i < tempList.Count; ++i)
            Recycle(tempList[i]);
        tempList.Clear();
    }

    public static bool IsSpawned(GameObject obj)
    {
        return ObjectPoolManager.Instance.spawnedObjects.ContainsKey(obj);
    }

    public static int CountPooled<T>(T prefab) where T : Component
    {
        return CountPooled(prefab.gameObject);
    }

    public static int CountPooled(GameObject prefab)
    {
        List<GameObject> list;
        if (ObjectPoolManager.Instance.pooledObjects.TryGetValue(prefab, out list))
            return list.Count;
        return 0;
    }

    public static int CountSpawned<T>(T prefab) where T : Component
    {
        return CountSpawned(prefab.gameObject);
    }

    public static int CountSpawned(GameObject prefab)
    {
        int count = 0;
        foreach (var instancePrefab in ObjectPoolManager.Instance.spawnedObjects.Values)
            if (prefab == instancePrefab)
                ++count;
        return count;
    }

    public static int CountAllPooled()
    {
        int count = 0;
        foreach (var list in ObjectPoolManager.Instance.pooledObjects.Values)
            count += list.Count;
        return count;
    }

    public static List<GameObject> GetPooled(GameObject prefab, List<GameObject> list, bool appendList)
    {
        if (list == null)
            list = new List<GameObject>();
        if (!appendList)
            list.Clear();
        List<GameObject> pooled;
        if (ObjectPoolManager.Instance.pooledObjects.TryGetValue(prefab, out pooled))
            list.AddRange(pooled);
        return list;
    }

    public static List<T> GetPooled<T>(T prefab, List<T> list, bool appendList) where T : Component
    {
        if (list == null)
            list = new List<T>();
        if (!appendList)
            list.Clear();
        List<GameObject> pooled;
        if (ObjectPoolManager.Instance.pooledObjects.TryGetValue(prefab.gameObject, out pooled))
            for (int i = 0; i < pooled.Count; ++i)
                list.Add(pooled[i].GetComponent<T>());
        return list;
    }

    public static List<GameObject> GetSpawned(GameObject prefab, List<GameObject> list, bool appendList)
    {
        if (list == null)
            list = new List<GameObject>();
        if (!appendList)
            list.Clear();
        foreach (var item in ObjectPoolManager.Instance.spawnedObjects)
            if (item.Value == prefab)
                list.Add(item.Key);
        return list;
    }

    public static List<T> GetSpawned<T>(T prefab, List<T> list, bool appendList) where T : Component
    {
        if (list == null)
            list = new List<T>();
        if (!appendList)
            list.Clear();
        var prefabObj = prefab.gameObject;
        foreach (var item in ObjectPoolManager.Instance.spawnedObjects)
            if (item.Value == prefabObj)
                list.Add(item.Key.GetComponent<T>());
        return list;
    }

    public static void DestroyPooled(GameObject prefab)
    {
        List<GameObject> pooled;
        if (ObjectPoolManager.Instance.pooledObjects.TryGetValue(prefab, out pooled))
        {
            for (int i = 0; i < pooled.Count; ++i)
                GameObject.Destroy(pooled[i]);
            pooled.Clear();
        }
    }

    public static void DestroyPooled<T>(T prefab) where T : Component
    {
        DestroyPooled(prefab.gameObject);
    }

    public static void DestroyAll(GameObject prefab)
    {
        RecycleAll(prefab);
        DestroyPooled(prefab);
    }

    public static void DestroyAll<T>(T prefab) where T : Component
    {
        DestroyAll(prefab.gameObject);
    }
}

public static class ObjectPoolExtensions
{
    public static void CreatePool<T>(this T prefab) where T : Component
    {
        ObjectPoolManager.CreatePool(prefab, 0);
    }

    public static void CreatePool<T>(this T prefab, int initialPoolSize) where T : Component
    {
        ObjectPoolManager.CreatePool(prefab, initialPoolSize);
    }

    public static void CreatePool(this GameObject prefab)
    {
        ObjectPoolManager.CreatePool(prefab, 0);
    }

    public static void CreatePool(this GameObject prefab, int initialPoolSize)
    {
        ObjectPoolManager.CreatePool(prefab, initialPoolSize);
    }

    public static T Spawn<T>(this T prefab, Transform parent, Vector3 position, Quaternion rotation) where T : Component
    {
        return ObjectPoolManager.Spawn(prefab, parent, position, rotation);
    }

    public static T Spawn<T>(this T prefab, Vector3 position, Quaternion rotation) where T : Component
    {
        return ObjectPoolManager.Spawn(prefab, null, position, rotation);
    }

    public static T Spawn<T>(this T prefab, Transform parent, Vector3 position) where T : Component
    {
        return ObjectPoolManager.Spawn(prefab, parent, position, Quaternion.identity);
    }

    public static T Spawn<T>(this T prefab, Vector3 position) where T : Component
    {
        return ObjectPoolManager.Spawn(prefab, null, position, Quaternion.identity);
    }

    public static T Spawn<T>(this T prefab, Transform parent) where T : Component
    {
        return ObjectPoolManager.Spawn(prefab, parent, Vector3.zero, Quaternion.identity);
    }

    public static T Spawn<T>(this T prefab) where T : Component
    {
        return ObjectPoolManager.Spawn(prefab, null, Vector3.zero, Quaternion.identity);
    }

    public static GameObject Spawn(this GameObject prefab, Transform parent, Vector3 position, Quaternion rotation)
    {
        return ObjectPoolManager.Spawn(prefab, parent, position, rotation);
    }

    public static GameObject Spawn(this GameObject prefab, Vector3 position, Quaternion rotation)
    {
        return ObjectPoolManager.Spawn(prefab, null, position, rotation);
    }

    public static GameObject Spawn(this GameObject prefab, Transform parent, Vector3 position)
    {
        return ObjectPoolManager.Spawn(prefab, parent, position, Quaternion.identity);
    }

    public static GameObject Spawn(this GameObject prefab, Vector3 position)
    {
        return ObjectPoolManager.Spawn(prefab, null, position, Quaternion.identity);
    }

    public static GameObject Spawn(this GameObject prefab, Transform parent)
    {
        return ObjectPoolManager.Spawn(prefab, parent, Vector3.zero, Quaternion.identity);
    }

    public static GameObject Spawn(this GameObject prefab)
    {
        return ObjectPoolManager.Spawn(prefab, null, Vector3.zero, Quaternion.identity);
    }

    public static void Recycle<T>(this T obj) where T : Component
    {
        ObjectPoolManager.Recycle(obj);
    }

    public static void Recycle(this GameObject obj)
    {
        ObjectPoolManager.Recycle(obj);
    }

    public static void RecycleAll<T>(this T prefab) where T : Component
    {
        ObjectPoolManager.RecycleAll(prefab);
    }

    public static void RecycleAll(this GameObject prefab)
    {
        ObjectPoolManager.RecycleAll(prefab);
    }

    public static int CountPooled<T>(this T prefab) where T : Component
    {
        return ObjectPoolManager.CountPooled(prefab);
    }

    public static int CountPooled(this GameObject prefab)
    {
        return ObjectPoolManager.CountPooled(prefab);
    }

    public static int CountSpawned<T>(this T prefab) where T : Component
    {
        return ObjectPoolManager.CountSpawned(prefab);
    }

    public static int CountSpawned(this GameObject prefab)
    {
        return ObjectPoolManager.CountSpawned(prefab);
    }

    public static List<GameObject> GetSpawned(this GameObject prefab, List<GameObject> list, bool appendList)
    {
        return ObjectPoolManager.GetSpawned(prefab, list, appendList);
    }

    public static List<GameObject> GetSpawned(this GameObject prefab, List<GameObject> list)
    {
        return ObjectPoolManager.GetSpawned(prefab, list, false);
    }

    public static List<GameObject> GetSpawned(this GameObject prefab)
    {
        return ObjectPoolManager.GetSpawned(prefab, null, false);
    }

    public static List<T> GetSpawned<T>(this T prefab, List<T> list, bool appendList) where T : Component
    {
        return ObjectPoolManager.GetSpawned(prefab, list, appendList);
    }

    public static List<T> GetSpawned<T>(this T prefab, List<T> list) where T : Component
    {
        return ObjectPoolManager.GetSpawned(prefab, list, false);
    }

    public static List<T> GetSpawned<T>(this T prefab) where T : Component
    {
        return ObjectPoolManager.GetSpawned(prefab, null, false);
    }

    public static List<GameObject> GetPooled(this GameObject prefab, List<GameObject> list, bool appendList)
    {
        return ObjectPoolManager.GetPooled(prefab, list, appendList);
    }

    public static List<GameObject> GetPooled(this GameObject prefab, List<GameObject> list)
    {
        return ObjectPoolManager.GetPooled(prefab, list, false);
    }

    public static List<GameObject> GetPooled(this GameObject prefab)
    {
        return ObjectPoolManager.GetPooled(prefab, null, false);
    }

    public static List<T> GetPooled<T>(this T prefab, List<T> list, bool appendList) where T : Component
    {
        return ObjectPoolManager.GetPooled(prefab, list, appendList);
    }

    public static List<T> GetPooled<T>(this T prefab, List<T> list) where T : Component
    {
        return ObjectPoolManager.GetPooled(prefab, list, false);
    }

    public static List<T> GetPooled<T>(this T prefab) where T : Component
    {
        return ObjectPoolManager.GetPooled(prefab, null, false);
    }

    public static void DestroyPooled(this GameObject prefab)
    {
        ObjectPoolManager.DestroyPooled(prefab);
    }

    public static void DestroyPooled<T>(this T prefab) where T : Component
    {
        ObjectPoolManager.DestroyPooled(prefab.gameObject);
    }

    public static void DestroyAll(this GameObject prefab)
    {
        ObjectPoolManager.DestroyAll(prefab);
    }

    public static void DestroyAll<T>(this T prefab) where T : Component
    {
        ObjectPoolManager.DestroyAll(prefab.gameObject);
    }
}