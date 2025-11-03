using System.Collections;
using System.Collections.Generic;
using DragonU3DSDK.Asset;
using UnityEngine;

namespace FishEatFishSpace
{
    public class EffectManager : Singleton<EffectManager>
    {
        string resourceFolder = "FishEatFish/FishDynamic/Prefabs/Effects";
        Dictionary<string, GameObject> effectResourceDict = new Dictionary<string, GameObject>();
        Dictionary<string, List<GameObject>> effectInstanceDict = new Dictionary<string, List<GameObject>>();

        GameObject AddNewInstance(string effectName)
        {
            if (!effectResourceDict.ContainsKey(effectName))
            {
                GameObject prefab = ResourcesManager.Instance.LoadResource<GameObject>(resourceFolder+"/"+effectName);
                if (prefab == null)
                {
                    Debug.LogError("missing effect asset: " + effectName);
                    return null;
                }

                effectResourceDict.Add(effectName, prefab);

            }

            GameObject effect = Instantiate(effectResourceDict[effectName]);
            effect.SetActive(false);
            effectInstanceDict[effectName].Add(effect);
            return effect;
        }

        public void PlayEffectInPool(string effectName, Transform effectParent, Vector3 effectPos, Material mat = null)
        {
            if (!effectInstanceDict.ContainsKey(effectName))
            {
                effectInstanceDict.Add(effectName, new List<GameObject>());
                AddNewInstance(effectName);
            }

            List<GameObject> effect_list = effectInstanceDict[effectName];
            GameObject effect = null;
            for (int i = 0; i < effect_list.Count; i++)
            {
                if (!effect_list[i].GetComponent<ParticleSystem>().isPlaying)
                {
                    effect = effect_list[i];
                    break;
                }
            }

            if (effect == null)
            {
                effect = AddNewInstance(effectName);
            }

            if (effect == null)
            {
                return;
            }

            if (mat != null)
            {
                effect.GetComponent<ParticleSystemRenderer>().material = mat;
            }

            effect.transform.SetParent(effectParent);
            effect.transform.position = effectPos;

            effect.SetActive(true);
            effect.GetComponent<ParticleSystem>().Play();
        }

        public GameObject PlayEffect(string effectName, Transform transform, Vector3 offset, bool destroyEnd = true,
            float delayTime = 0)
        {
            if (!effectResourceDict.ContainsKey(effectName))
            {
                GameObject effectPrefab = ResourcesManager.Instance.LoadResource<GameObject>($"{resourceFolder}/{effectName}");
                if (effectPrefab == null)
                {
                    Debug.LogError("missing effect asset: " + effectName);
                    return null;
                }

                effectResourceDict.Add(effectName, effectPrefab);
            }

            GameObject effect = Instantiate(effectResourceDict[effectName], transform);
            if (effect == null)
                return null;
            if (offset != null)
            {
                effect.transform.position = effect.transform.position + offset;
            }

            ParticleSystem particleSystem = effect.GetComponent<ParticleSystem>();
            if (particleSystem == null)
                return null;

            if (delayTime > 0)
            {
                effect.SetActive(false);
                StartCoroutine(DelayPlay(particleSystem, delayTime));
            }
            else
            {
                particleSystem.Play();
            }

            if (destroyEnd)
            {
                Destroy(effect, particleSystem.main.duration + delayTime);
                return null;
            }

            return effect;
        }

        private IEnumerator DelayPlay(ParticleSystem particleSystem, float delayTime)
        {
            yield return new WaitForSeconds(delayTime);
            if (particleSystem != null)
            {
                particleSystem.gameObject.SetActive(true);
                particleSystem.Play();
            }
        }
    }
}
