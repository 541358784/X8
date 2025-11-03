using System.Collections;
using System.Collections.Generic;
using DragonU3DSDK.Asset;
using UnityEngine;

public partial class UIGardenTreasureMainController
{
      private GameObject _bombEffect = null;
      private GameObject _shovelEffect;

      private enum EffectType
      {
            Shovel,
            Bomb,
      }

      private void AwakeAnim()
      {
            _shovelEffect = ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/Activity/GardenTreasure/fx_broken_1");
            _bombEffect = ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/Activity/GardenTreasure/fx_broken_2");

            ObjectPoolManager.CreatePool(_shovelEffect, 5);
            ObjectPoolManager.CreatePool(_bombEffect, 5);

      }
      private void PlayEffect(EffectType type, Vector3 position)
      {
            var effectRoot =_shovelEffect;
            if (type == EffectType.Bomb)
                  effectRoot = _bombEffect;

            var effect = ObjectPoolManager.Spawn(effectRoot, FlyGameObjectManager.Instance.EffectRoot);
            
            effect.gameObject.transform.position = position;
            effect.gameObject.SetActive(true);
            StartCoroutine(WaitRecycle(effect));
      }

      private void DestroyEffect()
      {
            ObjectPoolManager.DestroyPooled(_shovelEffect);
            ObjectPoolManager.DestroyPooled(_bombEffect);
      }

      private IEnumerator WaitRecycle(GameObject recycleObj)
      {
            yield return new WaitForSeconds(3f);

            ObjectPoolManager.Recycle(recycleObj);
            recycleObj.SetActive(false);
      }
}