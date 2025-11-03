using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dynamic
{
    public class DynamicEntryManager : Singleton<DynamicEntryManager>
    {
         private Dictionary<Type, List<DynamicEntryBase>> _dynamicEntryMap = new Dictionary<Type, List<DynamicEntryBase>>();
         private Dictionary<object, object> _dynamicEntryKey = new Dictionary<object, object>();
         
         public void RegisterDynamicEntry(Type entryType)
         {
             var entry = (DynamicEntryBase)Activator.CreateInstance(entryType);
            
             if(_dynamicEntryMap.ContainsKey(entryType))
                 return;
            
             _dynamicEntryMap.Add(entryType, new List<DynamicEntryBase>());
             _dynamicEntryMap[entryType].Add(entry);
         }
         
        public void Clean()
        {
            _dynamicEntryMap.Clear();
        }
        
        private void RegisterDynamicEntry<T>() where T : DynamicEntryBase, new()
        {
            if(_dynamicEntryMap.ContainsKey(typeof(T)))
                return;
            
            _dynamicEntryMap.Add(typeof(T), new List<DynamicEntryBase>());
            _dynamicEntryMap[typeof(T)].Add(new T());
        }

        public GameObject GetDynamicEntry<T>()  where T : DynamicEntryBase
        {
            if (!_dynamicEntryMap.ContainsKey(typeof(T)))
                return null;

            return _dynamicEntryMap[typeof(T)].First().EntryObj;
        }
        
        public List<GameObject> GetDynamicEntrys<T>()  where T : DynamicEntryBase
        {
            if (!_dynamicEntryMap.ContainsKey(typeof(T)))
                return null;

            List<GameObject> objects = new List<GameObject>();
            _dynamicEntryMap[typeof(T)].ForEach(a=>
            {
                if(a.EntryObj != null)
                    objects.Add(a.EntryObj);
            });
            
            return objects;
        }
        
        public T1 GetDynamicEntryMonoBehaviour<T1, T2>() where T1 : MonoBehaviour where T2:DynamicEntryBase
        {
            if (!_dynamicEntryMap.ContainsKey(typeof(T2)))
                return null;

            return _dynamicEntryMap[typeof(T2)].First().MonoBehaviour as T1;
        }
        
        public List<T1> GetDynamicEntryMonoBehaviours<T1, T2>() where T1 : MonoBehaviour where T2:DynamicEntryBase
        {
            if (!_dynamicEntryMap.ContainsKey(typeof(T2)))
                return null;

            List<T1> monoBehaviours = new List<T1>();
            _dynamicEntryMap[typeof(T2)].ForEach(a=>
            {
                if(a.MonoBehaviour != null)
                    monoBehaviours.Add(a.MonoBehaviour as T1);
            });
            
            return monoBehaviours;
        }
        
        public T GetDynamicEntryMonoBehaviour<T>(Type type) where T : MonoBehaviour
        {
            if (!_dynamicEntryMap.ContainsKey(type))
                return null;

            return _dynamicEntryMap[type].First().MonoBehaviour as T;
        }

        public List<T> GetDynamicEntryMonoBehaviours<T>(Type type) where T : MonoBehaviour
        {
            if (!_dynamicEntryMap.ContainsKey(type))
                return null;

            List<T> monoBehaviours = new List<T>();
            _dynamicEntryMap[type].ForEach(a=>
            {
                if(a.MonoBehaviour != null)
                    monoBehaviours.Add(a.MonoBehaviour as T);
            });
            
            return monoBehaviours;
        }
        

        #region 他俩一组动态使用 这种是动态添加的入口
        public void RegisterDynamicEntry<T>(object onlyKey, DynamicEntryBase entryBase)
        {
            if(_dynamicEntryKey.ContainsKey(onlyKey))
                return;
            
            var type = typeof(T);
            if (!_dynamicEntryMap.ContainsKey(type))
                _dynamicEntryMap[type] = new List<DynamicEntryBase>();
            
            if(_dynamicEntryMap[type].Contains(entryBase))
                return;
            
            _dynamicEntryKey.Add(onlyKey, onlyKey);
            _dynamicEntryMap[type].Add(entryBase);
        }
        public GameObject GetDynamicEntry<T>(DynamicEntryBase entryBase)  where T : DynamicEntryBase
        {
            if (!_dynamicEntryMap.ContainsKey(typeof(T)))
                return null;

            var findEntry = _dynamicEntryMap[typeof(T)].Find(a=>a == entryBase);
            if (findEntry == null)
                return null;

            return findEntry.EntryObj;
        }

        #endregion
    }
}