using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config;
using DragonPlus.Config.TMatchShop;

namespace TMatch
{


    public class FlyEffectManager : Manager<FlyEffectManager>
    {
        public enum ControlType
        {
            Left,
            Right,
        }

        private List<FlyTransformEffect> _flyAnimationObjs = new List<FlyTransformEffect>();
        private List<GameObject> _flyObjs = new List<GameObject>();
        private List<GameObject> _hitObjs = new List<GameObject>();
        static public float PLAY_NUMBER_INCREASE_DURATION = 1f;

        public void DestroyFlyAnimation()
        {
            foreach (var effect in _flyAnimationObjs)
            {
                if (effect != null)
                {
                    effect.gameObject.SetActive(false);
                }
            }

            foreach (var flyObj in _flyObjs)
            {
                if (flyObj != null)
                {
                    Destroy(flyObj);
                }
            }

            foreach (var hintObj in _hitObjs)
            {
                if (hintObj != null)
                {
                    Destroy(hintObj);
                }
            }
        }


        private static Vector2 getControlType(Vector3 source, Vector3 target, ControlType controlType)
        {
            var vDis = target - source;
            var control = Vector2.zero;

            var direction = 1;
            switch (controlType)
            {
                case ControlType.Left:
                    direction = 1;
                    break;
                case ControlType.Right:
                    direction = -1;
                    break;
            }

            control.x = source.x + vDis.x / 2;
            control.y = source.y + vDis.y / 2f;

            var normal = new Vector2(vDis.y, -vDis.x);
            control = Vector3.MoveTowards(control, normal, direction * 1);

            return control;
        }

        public float Fly(Transform srcTransform, Transform destTransform, GameObject flyPrefab, GameObject hitPrefab,
            Transform parent, Action cb,
            int objCount = 3, float duration = 0.5f, int flyType = 0, bool needInstantiate = true, float scaleFrom = 1f,
            float scaleTo = 1f)
        {
            _flyAnimationObjs.Clear();
            var srcPos = srcTransform.position;
            var vDis = destTransform.position - srcPos;
            // var controlPos = getControlType(srcTransform.position, destTransform.position, ControlType.Left);
            var controlPos = new Vector2(srcPos.x + vDis.x * UnityEngine.Random.Range(-0.6f, 0.6f),
                srcPos.y + vDis.y * UnityEngine.Random.Range(0.2f, 0.9f));
            return flyTransform(srcTransform, destTransform, controlPos, flyPrefab, hitPrefab, parent, cb, objCount,
                duration, needInstantiate, scaleFrom, scaleTo);
        }

        private float flyTransform(Transform sourceTransform, Transform targetPos, Vector2 controlTransform,
            GameObject flyObj, GameObject hitObj, Transform parent, Action cb,
            int objCount = 3, float duration = 0.5f, bool needInstantiate = true, float scaleFrom = 1f,
            float scaleTo = 1f)
        {
            //AudioManager.Instance.PlaySound(SfxNameConst.coin_fly);
            var soundEffectPlayed = false;
            var flyDeltaDuration = 0.15f;
            for (var i = 0; i < objCount; i++)
            {
                var time = duration + flyDeltaDuration * i;
                var flyClone = needInstantiate ? Instantiate(flyObj, parent) : flyObj;
                // _flyObjs.Add(flyClone);
                flyClone.transform.position = sourceTransform.position;
                flyClone.gameObject.SetActive(true);

                //缩放
                flyClone.transform.localScale = Vector3.one * scaleFrom;
                flyClone.transform.DOScale(Vector3.one * scaleTo, duration);

                //飞行
                var effect = CommonUtils.GetOrCreateComponent<FlyTransformEffect>(flyClone);
                // _flyAnimationObjs.Add(effect);
                Action secondAction = null;
                if (i == objCount - 1)
                {
                    secondAction = () => cb?.Invoke();
                }

                effect.InitData(sourceTransform, controlTransform, targetPos, time, 0, () =>
                {
                    if (needInstantiate)
                        Destroy(flyClone);
                    if (!soundEffectPlayed)
                    {
                        soundEffectPlayed = true;
                        //AudioManager.Instance.PlaySound(SfxNameConst.add_coins, 0.3f); //音量对安卓平台无效
                    }

                    if (hitObj != null)
                    {
                        var hitClone = Instantiate(hitObj, parent);
                        // _flyObjs.Add(hitClone);
                        hitClone.transform.position = targetPos.position;
                        hitClone.transform.DOScale(1, 0.5f).OnComplete(() =>
                        {
                            Destroy(hitClone);
                            secondAction?.Invoke();
                        });
                    }
                    else
                    {
                        secondAction?.Invoke();
                    }
                });
            }

            return duration + flyDeltaDuration * objCount;
        }

        IEnumerator DestroyClonedObject(GameObject obj)
        {
            yield return new WaitForSeconds(0.5f);
            Destroy(obj);
        }


        public float PlayNumIncreaseAnim(Text text, FlyItemData data, Action callBack)
        {
            var total = data.currentCount + data.deltaCount;
            var currentCount = data.currentCount;
            if (text != null)
                DOTween.To(() => currentCount, value => text.text = value.ToString("N0"), total,
                    PLAY_NUMBER_INCREASE_DURATION).OnComplete(() =>
                {
                    text.text = total.ToString("N0");
                    callBack?.Invoke();
                });

            return PLAY_NUMBER_INCREASE_DURATION;
        }

        public float PlayNumIncreaseAnim(LocalizeTextMeshProUGUI text, FlyItemData data, Action callBack)
        {
            var total = data.currentCount + data.deltaCount;
            var currentCount = data.currentCount;
            if (text != null)
                DOTween.To(() => currentCount, value => text.SetText(value.ToString("N0")), total,
                    PLAY_NUMBER_INCREASE_DURATION).OnComplete(() =>
                {
                    text.SetText(total.ToString("N0"));
                    callBack?.Invoke();
                });

            return PLAY_NUMBER_INCREASE_DURATION;
        }

        public float PlayNumReduceAnim(Text text, FlyItemData data, Action callBack)
        {
            var total = data.currentCount - data.deltaCount;
            var currentCount = data.currentCount;
            DOTween.To(() => currentCount, value => text.text = value.ToString("N0"), total,
                PLAY_NUMBER_INCREASE_DURATION).OnComplete(() =>
            {
                text.text = total.ToString("N0");
                callBack?.Invoke();
            });

            return PLAY_NUMBER_INCREASE_DURATION;
        }
    }

    public struct FlyItemData
    {
        public ResourceId rewardId;
        public long currentCount;
        public long deltaCount;
        public ItemConfig rewardsConfig;

        public FlyItemData(ResourceId rid, long deltaCount)
        {
            this.rewardId = rid;
            this.currentCount = CommonUtils.GetRewardById(rid);
            this.deltaCount = deltaCount;
            this.rewardsConfig = ItemModel.Instance.GetConfigById((int) rid);
        }

        public FlyItemData(ResourceId rid, long deltaCount, long currentCount)
        {
            this.rewardId = rid;
            this.currentCount = currentCount;
            this.deltaCount = deltaCount;
            this.rewardsConfig = ItemModel.Instance.GetConfigById((int) rid);
        }
    }
}