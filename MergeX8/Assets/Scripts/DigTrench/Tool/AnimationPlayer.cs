using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config;
using DragonU3DSDK.Storage;
using DG.Tweening;
using UnityEngine.Tilemaps;
using System;
using System.Linq;
using System.Threading.Tasks;
using Deco.Graphic;
using Deco.World;
using DragonU3DSDK;
using static Deco.Item.DecoItem;
using DragonU3DSDK.Asset;
using Spine.Unity;
using Object = UnityEngine.Object;
using Decoration;
using Framework;
using Spine;


    public class AnimationPlayer
    {
        protected List<Animator> _loopAnimators = new List<Animator>();
        protected List<Animator> _modelAnimators = new List<Animator>();
        protected SpriteRenderer[] _spRenders;
        protected MeshRenderer[] _meshRenders;
        internal protected SkeletonAnimation[] _skeletonAnimations;
        protected SkeletonRenderer[] _skeletonRenderers;
        protected Dictionary<int, List<Animator>> _animatorDic = new Dictionary<int, List<Animator>>();
        protected Dictionary<string, Transform> _effectDic = new Dictionary<string, Transform>();
        protected Dictionary<string, string> _effectNameDic = new Dictionary<string, string>();
        
        private Tween _tweenAlpha;

        private bool _isInitAnimator = false;

        private Transform _root;
        public AnimationPlayer(Transform root)
        {
            _root = root;
            InitAnimation(_root);
        }
        public void InitAnimation(Transform root)
        {
            InitRenders(root);
            EnableAnimator(true);
            InitLoopAnimator(root);
            InitModelAnimator(root);
            InitAnimator(root);
        }
        public void EnableAnimator(bool enable, bool ignoreSpine = false)
        {
            if(_animatorDic == null)
                return;
            
            if (_animatorDic.Count > 0)
            {
                foreach (var kv in _animatorDic)
                {
                    kv.Value.ForEach(a => a.enabled = enable);
                }
            }

            if (!ignoreSpine)
            {
                if (_skeletonAnimations != null)
                {
                    for (int i = 0; i < _skeletonAnimations.Length; i++)
                    {
                        if (_skeletonAnimations[i]) _skeletonAnimations[i].enabled = enable;
                    }
                }

                if (_skeletonRenderers != null)
                {
                    for (int i = 0; i < _skeletonRenderers.Length; i++)
                    {
                        if (_skeletonRenderers[i]) _skeletonRenderers[i].enabled = enable;
                    }
                }
            }
        }
        private void InitRenders(Transform root)
        {
            _spRenders = root.GetComponentsInChildren<SpriteRenderer>(true);
            if (_spRenders != null)
            {
                foreach (var render in _spRenders)
                {
                    render.allowOcclusionWhenDynamic = true;
                    OpUtils.ReplaceDefaultShader(render);
                }
            }

            _meshRenders = root.GetComponentsInChildren<MeshRenderer>(true);
            _skeletonAnimations = root.GetComponentsInChildren<SkeletonAnimation>();
            _skeletonRenderers = root.GetComponentsInChildren<SkeletonRenderer>();
        }
        private void InitLoopAnimator(Transform root)
        {
            foreach (Transform t in root)
            {
                if (!t.name.StartsWith("Loop"))
                    continue;
                
                var animtor = t.gameObject.GetComponent<Animator>();
            
                if (animtor != null)
                    animtor.enabled = false;
                
                _loopAnimators.Add(animtor);
            }
        }

        private void InitModelAnimator(Transform root)
        {
            foreach (Transform t in root)
            {
                if (t.name.StartsWith("model"))
                {
                    var animtor = t.gameObject.GetComponent<Animator>();
                
                    if (animtor != null)
                        animtor.enabled = false;
                    
                    _modelAnimators.Add(animtor);
                }

                InitModelAnimator(t);
            }
        }
        
        private void InitAnimator(Transform root)
        {
            if(_isInitAnimator)
                return;
            
            foreach (Transform t in root)
            {
                AddEffectToDic(root, t);
                
                var runtimeAnimator = RuntimeAnimatorManager.Instance.GetRuntimeAnimator(t.name);
                if (runtimeAnimator != null)
                {
                    AddAnimToChild(t, runtimeAnimator);
                }
                else if (int.TryParse(t.name, out var tIndex))
                {
                    var animtor = t.gameObject.AddComponent<Animator>();
                    animtor.enabled = false;
                    animtor.runtimeAnimatorController = RuntimeAnimatorManager.Instance.GetCommonAppearAnimator();

                    AddAnimToDic(tIndex, animtor);
                }
            }

            _isInitAnimator = true;
        }

        private void AddAnimToChild(Transform root, RuntimeAnimatorController animatorController)
        {
            foreach (Transform t in root)
            {
                AddEffectToDic(root, t);
                
                if (int.TryParse(t.name, out var tIndex))
                {
                    var animtor = t.gameObject.GetComponent<Animator>();
                    if (animtor != null)
                    {
                        animtor.enabled = false;
                        continue;
                    }

                    animtor = t.gameObject.AddComponent<Animator>();
                    animtor.runtimeAnimatorController = animatorController;
                    animtor.enabled = false;

                    AddAnimToDic(tIndex, animtor);
                }
            }
        }
        
        private void AddAnimToDic(int tIndex, Animator animtor)
        {
            if (!_animatorDic.TryGetValue(tIndex, out var animList))
            {
                animList = new List<Animator>();
                _animatorDic.Add(tIndex, animList);
            }

            animList.Add(animtor);
        }

        private void AddEffectToDic(Transform parent, Transform effectObj)
        {
            if(effectObj == null || parent == null)
                return;
            
            if(!effectObj.name.StartsWith("vex"))
                return;
            
            string[] str = effectObj.name.Split('_');
            if(str == null || str.Length < 2)
                return;

            string key = parent.name + "_" + str[1];
            if(_effectDic.ContainsKey(key))
                return;
            
            _effectDic.Add(key, effectObj);
            _effectNameDic.Add(key, str[0]);
        }

        private void PlayEffect(Transform animObj)
        {
            if(animObj == null || animObj.parent == null)
                return;

            string key = animObj.parent.name + "_" + animObj.name;
            
            if(!_effectDic.ContainsKey(key))
                return;
            
            RuntimeEffectManager.Instance.PlayEffect(_effectDic[key], _effectNameDic[key]);
        }

        private void PlayNormalAnim()
        {
            PlayAnimation(BuildingAnimationDefine.NORMAL);
            PlaySpineNormalAnim();
            _loopAnimators.ForEach(a => a.enabled = true);
            _modelAnimators.ForEach(a => a.enabled = true);
        }

        private void PlaySpineNormalAnim()
        {
            if (_skeletonAnimations != null)
            {
                foreach (var skeletonAnimation in _skeletonAnimations)
                {
                    if (!skeletonAnimation.name.StartsWith("spine_")) continue;

                    try
                    {
                        skeletonAnimation.enabled = true;
                        skeletonAnimation.AnimationState?.SetAnimation(0, "Normal", true);
                        skeletonAnimation.Update(0);
                    }
                    catch (Exception e)
                    {
                        DebugUtil.LogError(e.ToString());
                    }
                }
            }
        }

        public IEnumerator PlayRemoveAnimation()
        {
            _modelAnimators.ForEach(a=>a.gameObject.SetActive(false));
            EnableAnimator(false);
            var maxDurationTime = 0f;
            TrackEntry maxTrackEntry = null;
            if (_skeletonAnimations != null)
            {
                foreach (var skeletonAnimation in _skeletonAnimations)
                {
                    if (!skeletonAnimation.gameObject.name.StartsWith("spine_")) continue;

                    skeletonAnimation.enabled = true;
                    var trackEntry = skeletonAnimation.AnimationState?.SetAnimation(0, BuildingAnimationDefine.FIRST_BUILD_BUILDING_SUCCESS_ANIMATION, false);
                    skeletonAnimation.Update(0);

                    PlayEffect(skeletonAnimation.transform);
                    
                    if (trackEntry.AnimationEnd > maxDurationTime)
                    {
                        maxDurationTime = trackEntry.AnimationEnd;
                        maxTrackEntry = trackEntry;
                    }
                }
            }

            float animWaitTime = 0.3f;
            float animStepTime = 0.08f;
            var keyList = new List<int>(_animatorDic.Keys.ToArray());
            keyList.Sort();
            for (int i = 0; i < keyList.Count; i++)
            {
                if (_animatorDic.TryGetValue(keyList[i], out var animList))
                {
                    for (int j = 0; j < animList.Count; j++)
                    {
                        var a = animList[j];
                        a.gameObject.SetActive(true);
                        a.enabled = true;

                        a.PlayAnim(BuildingAnimationDefine.REMOVE);
                        PlayEffect(a.transform);

                        if (j == animList.Count - 1 && i == keyList.Count - 1)
                            animWaitTime = CommonUtils.GetAnimTime(a, BuildingAnimationDefine.REMOVE);
                    }

                    yield return new WaitForSeconds(animStepTime);
                }
            }
            
            var spineLeftTime = 0f;
            if (maxTrackEntry != null)
            {
                if (!maxTrackEntry.IsComplete)
                {
                    spineLeftTime = maxTrackEntry.AnimationEnd - maxTrackEntry.AnimationTime;
                }
            }
            yield return new WaitForSeconds(Mathf.Max(animWaitTime, spineLeftTime));
            _root.gameObject.SetActive(false);
            //yield return new WaitForSeconds(0.1f);
            EnableAnimator(false, true);
        }

        public void PlayAnimation(string animationName)
        {
            if (_animatorDic != null)
            {
                foreach (var kv in _animatorDic)
                {
                    kv.Value.ForEach(a => a.PlayAnim(animationName));
                }
            }
        }
        
        public IEnumerator PlayShowAnimation(Action aniEndCall = null)
        {
            EnableAnimator(true);
            foreach (var kv in _animatorDic)
            {
                kv.Value.ForEach(a => a.gameObject.SetActive(false));
            }
            _modelAnimators.ForEach(a=>a.gameObject.SetActive(false));

            float maxAnimTime = 0;
            TrackEntry maxTrackEntry = null;
            //同时播spine和animation
            {
                bool isSpineEnd = false;
                StartCoroutine(PlayShowAnim_Spine((a) =>
                {
                    maxTrackEntry = a;
                    isSpineEnd = true;
                }));
                yield return StartCoroutine(
                    PlayShowAnim_Animator((a) => { maxAnimTime = a; }));
                while (!isSpineEnd)
                {
                    yield return new WaitForEndOfFrame();
                }
            }

            float spineLeftTime = 0;
            if (maxTrackEntry != null)
            {
                if (!maxTrackEntry.IsComplete)
                {
                    spineLeftTime = maxTrackEntry.AnimationEnd - maxTrackEntry.AnimationLast;
                }
            }
            
            float leftTime = Mathf.Max(spineLeftTime, maxAnimTime);
            leftTime += 0.1f;
            //等待最后动画结束
            yield return new WaitForSeconds(leftTime);

            _loopAnimators.ForEach(a => a.enabled = true);
            //yield return new WaitForSeconds(3f);
            EnableAnimator(false, true);
            aniEndCall?.Invoke();
        }

        public Coroutine StartCoroutine(IEnumerator routine)
        {
            if (_root.GetComponent<MBCoroutine>() == null)
            {
                _root.gameObject.AddComponent<MBCoroutine>();
            }
            return _root.GetComponent<MBCoroutine>().StartCoroutine(routine);
        }

        private IEnumerator PlayShowAnim_Spine(Action<TrackEntry> endCall)
        {
            var maxDurationTime = 0f;
            float spineLeftTime = 0;
            TrackEntry maxTrackEntry = null;
            if (_skeletonAnimations == null)
            {
                endCall?.Invoke(maxTrackEntry);
                yield break;
            }
            
            foreach (var skeletonAnimation in _skeletonAnimations)
            {
                if (!skeletonAnimation.gameObject.name.StartsWith("spine_")) continue;

                try
                {
                    var trackEntry = skeletonAnimation.AnimationState?.SetAnimation(0, BuildingAnimationDefine.FIRST_BUILD_BUILDING_SUCCESS_ANIMATION, false);
                    skeletonAnimation.UpdateMode = UpdateMode.FullUpdate;
                    skeletonAnimation.Update(0);
                    PlayEffect(skeletonAnimation.transform);
                    if (trackEntry.AnimationEnd > maxDurationTime)
                    {
                        maxDurationTime = trackEntry.AnimationEnd;
                        maxTrackEntry = trackEntry;
                    }
                }
                catch (Exception e)
                {
                    DebugUtil.LogError(e.ToString());
                }
            }

            //spine播放normal动画
            yield return new WaitForSeconds(maxDurationTime);
            PlaySpineNormalAnim();
            endCall?.Invoke(maxTrackEntry);
        }

        private IEnumerator PlayShowAnim_Animator(Action<float> endCall)
        {
            _modelAnimators.ForEach(a =>
            {
                a.gameObject.SetActive(true);
                a.enabled = true;

                a.PlayAnim(BuildingAnimationDefine.SHOW);
            });
            
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
                        PlayEffect(a.transform);
                        maxAnimTime = Math.Max(maxAnimTime, CommonUtils.GetAnimTime(a, BuildingAnimationDefine.REMOVE));
                    }

                    yield return new WaitForSeconds(animStepTime);
                }
            }
            
            yield return new WaitForSeconds(animStepTime);
            endCall?.Invoke(maxAnimTime);
        }



        #region 选中状态呼吸效果

        /// <summary>
        /// 是否显示选中状态
        /// </summary>
        private bool _isShowSelectStatus = false;

        /// <summary>
        /// 材质属性
        /// </summary>
        MaterialPropertyBlock _materialPropertyBlock = new MaterialPropertyBlock();

        /// <summary>
        ///  显示选中状态（呼吸效果）
        /// </summary>
        /// <param name="isShow"></param>
        public void ShowSelectStatus(bool isShow)
        {
            _isShowSelectStatus = isShow;
            if (_isShowSelectStatus)
            {
                ShowSelectStatus();
            }
            else
            {
                HideSelectStatus();
            }
        }

        private void ShowSelectStatus()
        {
            try
            {
                Color currentColor = Color.white;

                float escapeValue = 0.2f;
                Color target = new Color(currentColor.a +escapeValue, currentColor.g + escapeValue, currentColor.b + escapeValue);
               
                if(_tweenAlpha != null)
                    _tweenAlpha.Kill();

                _tweenAlpha = DOTween.To(() => currentColor, x => currentColor = x, target, 0.6f).SetLoops(-1, LoopType.Yoyo).OnUpdate(()=>
                {
                    SetRendersColor(currentColor);
                }).SetDelay(2f);
            }
            catch (Exception e)
            {
                DebugUtil.LogError("ShowSelectStatus() :" + e.ToString());
            }
        }

        /// <summary>
        /// 隐藏选中效果
        /// </summary>
        private void HideSelectStatus()
        {
            if(_tweenAlpha != null)
                _tweenAlpha.Kill();
            
            _tweenAlpha = null;
            
            _isShowSelectStatus = false;
            SetRendersColor(Color.white);
        }

        private void SetRendersColor(Color color)
        {
            if (_spRenders != null)
            {
                foreach (var renderer in _spRenders)
                {
                    if (renderer == null) 
                        continue;
                    renderer.material.color = color;
                }
            }

            //_meshRenders
            if (_meshRenders != null)
            {
                for (int i = 0; i < _meshRenders.Length; ++i)
                {
                    if (_meshRenders[i] == null) 
                        continue;
                    _materialPropertyBlock.SetColor("_SelectColor", color);
                    _meshRenders[i].SetPropertyBlock(_materialPropertyBlock);
                }
            }
        }
        #endregion
    }
