using System;
using System.Linq;
using DragonU3DSDK.Asset;
using UnityEngine;
using System.Reflection;

namespace Dynamic
{
    public abstract class DynamicEntryBase
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void AutoRegisterAllEntries()
        {
            var entryTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsSubclassOf(typeof(DynamicEntryBase)) && !type.IsAbstract&&
                               type.GetCustomAttribute<CancelAutoRegister>() == null);

            foreach (var entryType in entryTypes)
            {
                DynamicEntryManager.Instance.RegisterDynamicEntry(entryType);
            }
        }
        
        public DynamicEntryBase(){}

        public DynamicEntryBase(object parma)
        {
            _parma = parma;
        }
        
        protected abstract string _entryPath { get; }
        protected abstract Type _dynamicType { get; }
        protected abstract Transform _parent { get; }
        protected abstract bool CanCreateEntry();
        protected virtual void InvokerCreateEntry(){}
        
        protected GameObject _entryObj;
        private MonoBehaviour _monoBehaviour { get; set; }

        public object _parma { get; set; }
        
        public MonoBehaviour MonoBehaviour
        {
            get
            {
                if (_monoBehaviour == null)
                {
                    var obj = EntryObj;
                }

                return _monoBehaviour;
            }
        }
        
        public GameObject EntryObj
        {
            get
            {
                if (_entryObj == null)
                {
                    if (!CanCreateEntry())
                        return null;

                    CreateEntry();
                }

                return _entryObj;
            }
        }

        private void CreateEntry()
        {
            var entry = ResourcesManager.Instance.LoadResource<GameObject>(_entryPath);
            if (entry == null)
                return;
            
            _entryObj = GameObject.Instantiate(entry, Vector3.zero, Quaternion.identity, _parent);
            _entryObj.transform.localPosition = Vector3.zero;
            
            _monoBehaviour = _entryObj.AddComponent(_dynamicType) as MonoBehaviour;

            InvokerCreateEntry();
        }
    }
}