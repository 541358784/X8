using System.Collections.Generic;
using DragonU3DSDK.Asset;
using GamePool;
using UnityEngine;

namespace Decoration
{
    public class RuntimeEffectManager : Manager<RuntimeEffectManager>
    {
        private Dictionary<string, string> _effectName= new Dictionary<string, string>
        {
            {"vexLeaf01", ObjectPoolName.vexLeaf01},
            {"vexLeaf02", ObjectPoolName.vexLeaf02},
            {"vexSmoke", ObjectPoolName.vexSmoke},
            {"vexFlower", ObjectPoolName.vexFlower},
            {"vexClean", ObjectPoolName.vexClean},
            
        };

        private bool isInit = false;
        
        public void InitEffect()
        {
            if(isInit)
                return;
            
            foreach (var kv in _effectName)
            {
                GamePool.ObjectPoolManager.Instance.CreatePool(kv.Value, 2, GamePool.ObjectPoolDelegate.CreateGameItem);
            }

            isInit = true;
        }

        public void PlayEffect(Transform root, string effectName)
        {
            if(!_effectName.ContainsKey(effectName))
                return;

            string poolName = _effectName[effectName];
            var efObj = GamePool.ObjectPoolManager.Instance.Spawn(poolName);
            if(efObj == null)
                return;

            efObj.gameObject.SetActive(false);
            efObj.transform.SetParent(root);
            efObj.transform.localPosition = Vector3.zero;
            efObj.transform.localScale = Vector3.one;
            efObj.transform.rotation = Quaternion.identity;
            efObj.gameObject.SetActive(true);

            StartCoroutine(CommonUtils.DelayWork(2f, () =>
            {
                GamePool.ObjectPoolManager.Instance.DeSpawn(poolName, efObj);
            }));
        }
    }
}