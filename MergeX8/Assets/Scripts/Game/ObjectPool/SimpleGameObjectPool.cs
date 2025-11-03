using UnityEngine;
using System.Collections.Generic;
using DragonU3DSDK.Asset;

public class SimpleGameObjectPool
{
    const int FREE_CAPACITY = 10;
    const int TOTAL_CAPACITY = 15;

    class SharePool
    {
        public Object prefab;
        public int freeCapacity;
        public int totalCapacity;
        public HashSet<GameObject> allSet = new HashSet<GameObject>();
        public Stack<GameObject> freeStack = new Stack<GameObject>();
    }

    Transform root = null;
    Dictionary<string, SharePool> sharePoolDict = new Dictionary<string, SharePool>();
    Dictionary<GameObject, string> nameDict = new Dictionary<GameObject, string>();
    List<string> removeList = new List<string>();

    public SimpleGameObjectPool(Transform root)
    {
        this.root = root;
    }

    SharePool CreatePool(string path)
    {
        SharePool pool;
        if (!sharePoolDict.TryGetValue(path, out pool))
        {
            pool = new SharePool();
            pool.freeCapacity = FREE_CAPACITY;
            pool.totalCapacity = TOTAL_CAPACITY;
            if (!string.IsNullOrEmpty(path))
            {
                pool.prefab = ResourcesManager.Instance.LoadResource<GameObject>(path);
            }

            sharePoolDict.Add(path, pool);
        }

        return pool;
    }

    void ReleasePool(string path)
    {
        SharePool pool;
        if (sharePoolDict.TryGetValue(path, out pool))
        {
            while (pool.freeStack.Count > 0)
            {
                GameObject go = pool.freeStack.Pop();
                GameObject.Destroy(go);
            }

            foreach (GameObject go in pool.allSet)
            {
                nameDict.Remove(go);
            }

            pool.allSet.Clear();

            sharePoolDict.Remove(path);
            pool.prefab = null;
        }
    }

    public void SetCapacity(int freeCapacity, int totalCapacity)
    {
        SetCapacity("", freeCapacity, totalCapacity);
    }

    public void SetCapacity(string path, int freeCapacity, int totalCapacity)
    {
        SharePool pool = CreatePool(path);
        pool.freeCapacity = freeCapacity;
        pool.totalCapacity = totalCapacity;
    }

    public GameObject Create()
    {
        return Create("");
    }

    public GameObject Create(string path)
    {
        SharePool pool = CreatePool(path);
        GameObject go = null;

        if (pool.freeStack.Count > 0)
        {
            go = pool.freeStack.Pop();
            go.SetActive(true);
        }
        else
        {
            if (pool.prefab != null)
            {
                go = GameObject.Instantiate(pool.prefab) as GameObject;
            }
            else
            {
                go = new GameObject("GameObject");
            }

            pool.allSet.Add(go);
            nameDict.Add(go, path);
        }

        return go;
    }

    public void CreateCache(string path, System.Action<GameObject> recycle_callback, int count)
    {
        SharePool pool = CreatePool(path);
        GameObject go;

        int create_count = count - pool.freeStack.Count;
        if (create_count > 0)
        {
            for (int i = 0; i < create_count; ++i)
            {
                if (pool.prefab != null)
                {
                    go = GameObject.Instantiate(pool.prefab) as GameObject;
                }
                else
                {
                    go = new GameObject("GameObject");
                }

                recycle_callback?.Invoke(go);

                pool.freeStack.Push(go);
                pool.allSet.Add(go);
                nameDict.Add(go, path);
            }
        }
    }

    public bool Release(GameObject go)
    {
        bool flag = true;

        string path;
        if (root && go != null && nameDict.TryGetValue(go, out path))
        {
            SharePool pool = sharePoolDict[path];
            if ((pool.freeStack.Count < pool.freeCapacity || pool.allSet.Count < pool.totalCapacity))
            {
                go.transform.SetParent(root);
                go.transform.transform.localScale = Vector3.one;
                go.SetActive(false);
                pool.freeStack.Push(go);
                flag = false;
            }
            else
            {
                GameObject.Destroy(go);
                pool.allSet.Remove(go);
                nameDict.Remove(go);
            }
        }
        else
        {
            GameObject.Destroy(go);
        }

        return flag;
    }

    public void Clear()
    {
        Clear("");
    }

    public void Clear(string path)
    {
        ReleasePool(path);
    }

    public void ClearAll()
    {
        removeList.AddRange(sharePoolDict.Keys);
        for (int i = 0; i < removeList.Count; i++)
        {
            ReleasePool(removeList[i]);
        }

        removeList.Clear();
    }
}