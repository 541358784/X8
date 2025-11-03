using System;
using System.Collections.Generic;
using System.Linq;
using Decoration;
using Spine.Unity;
using UnityEngine;

namespace DigTrench.Tool
{
    public class StaticSpinPlayer
    {
        private List<SkeletonAnimation> _spineAnimationList = new List<SkeletonAnimation>();
        private List<SkeletonAnimation> _spineConflictList = new List<SkeletonAnimation>();
        protected Dictionary<int, List<Animator>> _animatorDic = new Dictionary<int, List<Animator>>();
        public StaticSpinPlayer(Transform root)
        {
            foreach (Transform spineTransform in root)
            {
                if (spineTransform.name.StartsWith("Static_"))
                {
                    _spineAnimationList.Add(spineTransform.GetComponent<SkeletonAnimation>());
                }
                else if (spineTransform.name.StartsWith("spine"))
                {
                    _spineConflictList.Add(spineTransform.GetComponent<SkeletonAnimation>());
                }
                else
                {
                    var runtimeAnimator = RuntimeAnimatorManager.Instance.GetRuntimeAnimator(spineTransform.name);
                    if (runtimeAnimator != null)
                    {
                        foreach (Transform t in spineTransform)
                        {
                            if (int.TryParse(t.name, out var tIndex))
                            {
                                var animtor = t.gameObject.AddComponent<Animator>();
                                animtor.runtimeAnimatorController = runtimeAnimator;
                                animtor.enabled = false;
                                
                                if (!_animatorDic.TryGetValue(tIndex, out var animList))
                                {
                                    animList = new List<Animator>();
                                    _animatorDic.Add(tIndex, animList);
                                }
                                animList.Add(animtor);
                            }
                        }
                    }
                }
            }
        }

        public async void PlayGetWaterSpineState(string animationName = "sad_happy")
        {
            foreach (var spineAnimation in _spineAnimationList)
            {
                var trackEntry = spineAnimation.AnimationState?.SetAnimation(0, animationName, false);
                spineAnimation.UpdateMode = UpdateMode.FullUpdate;
                spineAnimation.Update(0);
                XUtility.WaitSeconds(trackEntry.AnimationEnd, () =>
                {
                    if (spineAnimation)
                    {
                        spineAnimation.AnimationState?.SetAnimation(0, "happy", true);   
                    }
                });
            }

            foreach (var spineAnimation in _spineConflictList)
            {
                var trackEntry = spineAnimation.AnimationState?.SetAnimation(0, "first_confirm", false);
                spineAnimation.UpdateMode = UpdateMode.FullUpdate;
                spineAnimation.Update(0);
            }
            
            var keyList = new List<int>(_animatorDic.Keys.ToArray());
            keyList.Sort();
            float maxAnimTime = 0;
            float animStepTime = 0.08f;
            for (int i = 0; i < keyList.Count; i++)
            {
                if (_animatorDic.TryGetValue(keyList[i], out var animList))
                {
                    for (int j = 0; j < animList.Count; j++)
                    {
                        var a = animList[j];
                        a.gameObject.SetActive(true);
                        a.enabled = true;

                        a.PlayAnim(BuildingAnimationDefine.SHOW);
                        maxAnimTime = Math.Max(maxAnimTime, CommonUtils.GetAnimTime(a, BuildingAnimationDefine.REMOVE));
                    }

                    await XUtility.WaitSeconds(animStepTime);
                }
            }
        }
        
    }
}