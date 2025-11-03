using System.Collections;
using System.Collections.Generic;
using DragonU3DSDK;
using UnityEngine;

namespace Dlugin
{
    //for object, not struct
    public interface IObjectPool
    {
        object Alloc();
        void Free(object obj);
        void Shrink();
    }

    public class ObjectPoolImp<T> : IObjectPool where T : class, new() 
    {
        public object Alloc()
        {
            return AllocTyped();
        }
        public T AllocTyped()
        {
            if (m_CachedObjects.Count > 0)
            {
                return m_CachedObjects.Pop();
            }
            else
            {
                return new T();
            }
        }
        public void Free(object obj)
        {
            m_CachedObjects.Push(obj as T);
        }
        public void Shrink()
        {
            int target = m_CachedObjects.Count / 2;
            if (target <= 0)
                m_CachedObjects.Clear();
            else
                while(m_CachedObjects.Count > target) m_CachedObjects.Pop();
        }

        private Stack<T> m_CachedObjects = new Stack<T>();
    }

    public static class ObjectPool
    {
        private static class ObjectPoolContainer<T> where T:class, IObjectPool
        {
            public static T s_Instance = null;
        }

        public static ObjectPoolImp<T> Get<T>() where T:class, new()
        {
            if (ObjectPoolContainer<ObjectPoolImp<T>>.s_Instance == null)
            {
                ObjectPoolContainer<ObjectPoolImp<T>>.s_Instance = new ObjectPoolImp<T>();
                m_AllPools.Add(ObjectPoolContainer<ObjectPoolImp<T>>.s_Instance);
            }
            return ObjectPoolContainer<ObjectPoolImp<T>>.s_Instance;
        }

        public static void Shrink()
        {
            DebugUtil.Log("ObjectPool.Shrink");
            for(int i = 0; i < m_AllPools.Count; i++)
            {
                m_AllPools[i].Shrink();
            }
        }

        private static List<IObjectPool> m_AllPools = new List<IObjectPool>();
    }
}
