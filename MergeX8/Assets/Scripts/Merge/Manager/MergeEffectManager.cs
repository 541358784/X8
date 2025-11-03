using Framework;
using GamePool;
using UnityEngine;

public class MergeEffectManager : Singleton<MergeEffectManager>
{
    public GameObject PlayBombEffect(Transform parent)
    {
        GameObject effect = GamePool.ObjectPoolManager.Instance.Spawn(ObjectPoolName.CommonOpen);
        if (effect == null)
            return null;

        effect.transform.SetParent(parent);
        effect.transform.localPosition = Vector3.zero;
        effect.transform.localScale = Vector3.one;
        effect.SetActive(false);
        effect.SetActive(true);
        CoroutineManager.Instance.StartCoroutine(CommonUtils.DelayWork(1,
            () => { GamePool.ObjectPoolManager.Instance.DeSpawn(ObjectPoolName.CommonOpen, effect); }));

        return effect;
    }

    public void PlayBombEffect(Vector3 position)
    {
        GameObject effect = PlayBombEffect(FlyGameObjectManager.Instance.EffectRoot);
        if (effect == null)
            return;

        effect.transform.position = position;
    }
}