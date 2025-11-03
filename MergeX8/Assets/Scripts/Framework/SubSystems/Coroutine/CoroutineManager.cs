using System;
using System.Collections;
using DragonU3DSDK;
using UnityEngine;

namespace Framework
{
    public class CoroutineManager : GlobalSystem<CoroutineManager>, IInitable
    {
        private MonoBehaviour _monoBehaviour;
        private GameObject _go = GameObjectFactory.Create(true);

        
        public void Init()
        {
            _go.name = GetType().ToString();
            _monoBehaviour = _go.AddComponent<MBCoroutine>();
        }

        public void Release()
        {
            _monoBehaviour = null;
            GameObjectFactory.Destroy(_go);
        }


        public Coroutine StartCoroutine(IEnumerator routine)
        {
            return (_monoBehaviour != null && routine != null) ? _monoBehaviour.StartCoroutine(routine) : null;
        }

        public void StopCoroutine(Coroutine routine)
        {
            try
            {
                if (_monoBehaviour != null && routine != null) _monoBehaviour.StopCoroutine(routine);
            }
            catch (Exception e)
            {
                DebugUtil.LogError(e);
            }
        }

        public void StopAllCoroutines()
        {
            if (_monoBehaviour != null) _monoBehaviour.StopAllCoroutines();
        }
    }
}