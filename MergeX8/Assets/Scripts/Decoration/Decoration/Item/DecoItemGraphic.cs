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
using Cysharp.Threading.Tasks;
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

namespace Deco.Item
{
    public class DecoItemGraphic : DecoGraphic
    {
        private DecoItem _item;

        private PolygonCollider2D _collider;


        
        internal Transform _tipTransform = null;
        internal Transform _cameraTipTransform = null;

        private List<SkeletonAnimation> _attachedNpcList = new List<SkeletonAnimation>();
        private List<SpriteRenderer> _attachedNpcShadowList = new List<SpriteRenderer>();
        protected List<Animator> _loopAnimators = new List<Animator>();
        protected List<Animator> _modelAnimators = new List<Animator>();
        
        private Tween _tweenAlpha;

        private bool _isInitAnimator = false;
        protected override string PREFAB_PATH
        {
            get
            {
                var area = _item._node.Stage.Area;
                var prefabBasePath = Decoration.Utils.PathPrefabBuilding(area.World.Id, area.Id, _item._data._config.id);
                return $"{prefabBasePath}/{_item._data._config.id}";
            }
        }

        protected override void OnUnload()
        {
            _tipTransform = null;
            _cameraTipTransform = null;
            _collider = null;
            _animatorDic.Clear();
            
            _attachedNpcList.Clear();
            _attachedNpcShadowList.Clear();
            _loopAnimators.Clear();
            _modelAnimators.Clear();
        }

        protected override void OnLoad()
        {
            if (!gameObject) return;

            gameObject.name = _item._data._config.id.ToString();
            _tipTransform = gameObject.transform.Find("Tip");
            _cameraTipTransform = gameObject.transform.Find("CameraTip");
            _collider = gameObject.transform.GetComponent<PolygonCollider2D>();

            //InitAttachedNpc();
            InitLoopAnimator();
            InitModelAnimator(gameObject.transform);
            
            if (!_item.Node.Stage.Area.Graphic._resReady || !_item.Node.Stage.Area.Unlocked)
            {
                MarkGray(true, false);
            }
        }

        public DecoItemGraphic(DecoItem item)
        {
            _item = item;
        }

        
        private void InitAttachedNpc()
        {
            foreach (Transform t in gameObject.transform)
            {
                if (t.name.StartsWith("npc_"))
                {
                    _attachedNpcList.AddRange(t.GetComponentsInChildren<SkeletonAnimation>());
                    _attachedNpcShadowList.AddRange(t.GetComponentsInChildren<SpriteRenderer>());
                }
            }
        }

        private void InitLoopAnimator()
        {
            foreach (Transform t in gameObject.transform)
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
        
        private void InitAnimator()
        {
            if(_isInitAnimator)
                return;
            
            foreach (Transform t in gameObject.transform)
            {
                AddEffectToDic(gameObject.transform, t);
                
                var runtimeAnimator = RuntimeAnimatorManager.Instance.GetRuntimeAnimator(t.name);
                if (runtimeAnimator != null)
                {
                    AddAnimToChild(t, runtimeAnimator);
                }
                else if (int.TryParse(t.name, out var tIndex))
                {
                    var animtor = t.gameObject.AddComponent<Animator>();
                    animtor.enabled = false;
                    animtor.runtimeAnimatorController = _item._isDefault ? RuntimeAnimatorManager.Instance.GetCommonDisappearAnimator() :  RuntimeAnimatorManager.Instance.GetCommonAppearAnimator();

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
        
        public void Show(bool playNormalAnim = false)
        {
            if (!_item.Node.Config.hideAfterShow)
            {
                gameObject?.SetActive(true);
                if(playNormalAnim)
                    PlayNormalAnim();

            }
            else
            {
                gameObject?.SetActive(false);
            }

            ActiveObj(false);
        }

        public void ActiveObj(bool isActive)
        {
            if (_item.Config.activeObjName == null || gameObject == null)
                return;
            
            var activeObj = gameObject.transform.Find(_item.Config.activeObjName);
            if(activeObj == null)
                return;
            
            activeObj.gameObject.SetActive(isActive);
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
        
        public IEnumerator Hide(bool playAnim = true)
        {
            if (playAnim)
            { 
                gameObject?.SetActive(true);
                yield return CoroutineManager.Instance.StartCoroutine(PlayRemoveAnimation());
            }
            else
            {
                gameObject?.SetActive(false);
                yield break;
            }
        }

        public IEnumerator PlayRemoveAnimation()
        {
            InitAnimator();

            _modelAnimators.ForEach(a=>a.gameObject.SetActive(false));
            
            _item.Graphic.ActiveObj(true);
            
            EnableAnimator(false);

            var maxDurationTime = 0f;
            TrackEntry maxTrackEntry = null;
            if (_skeletonAnimations != null)
            {
                foreach (var skeletonAnimation in _skeletonAnimations)
                {
                    if (!skeletonAnimation.gameObject.name.StartsWith("spine_")) continue;

                    skeletonAnimation.enabled = true;
                    skeletonAnimation.UpdateCullMode = false;
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
            float animStepTime = _item.Config.animCleanStepTime < 0.001f ? 0.08f : _item.Config.animCleanStepTime;
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

            gameObject.SetActive(false);
            
            //yield return new WaitForSeconds(0.1f);
            EnableAnimator(false, true);
        }
        
        public void Show()
        {
            gameObject?.SetActive(true);
        }

        public void PlayAnimation(string animationName)
        {
            if (gameObject)
            {
                if (_animatorDic != null)
                {
                    foreach (var kv in _animatorDic)
                    {
                        kv.Value.ForEach(a => a.PlayAnim(animationName));
                    }
                }
            }
        }

        public async UniTaskVoid PlaySpineAnimation(string animName, Action action)
        {
            if (_skeletonAnimations == null || _skeletonAnimations[0] == null)
            {
                float time = 0;
                while (time <= 5)
                {
                    if(_skeletonAnimations != null && _skeletonAnimations[0]!= null)
                        break;
                    
                    await UniTask.WaitForEndOfFrame();
                    time += Time.deltaTime;
                }

                if (_skeletonAnimations == null || _skeletonAnimations[0] == null)
                {
                    action?.Invoke();
                    return;
                }
            }
            
            float animEndTime = 0;
            foreach (var skeletonAnimation in _skeletonAnimations)
            {
                if (!skeletonAnimation.gameObject.name.StartsWith("spine_")) continue;

                try
                {
                    skeletonAnimation.UpdateCullMode = false;
                    var trackEntry = skeletonAnimation.AnimationState?.SetAnimation(0, animName, false);
                    skeletonAnimation.UpdateMode = UpdateMode.FullUpdate;
                    skeletonAnimation.Update(0);
                    PlayEffect(skeletonAnimation.transform);

                    animEndTime = trackEntry.AnimationEnd;
                }
                catch (Exception e)
                {
                    DebugUtil.LogError(e.ToString());
                }
            }

            await UniTask.WaitForSeconds(animEndTime-1);
            PlaySpineNormalAnim();
            action?.Invoke();
        }
        
        public IEnumerator PlayShowAnimation(Action aniEndCall = null)
        {
            InitAnimator();
            EnableAnimator(true);
            FadeOutNpc();
            _item.Node.GameObject.SetActive(true);

            _item.Graphic.ActiveObj(true);
            
            foreach (var kv in _animatorDic)
            {
                kv.Value.ForEach(a => a.gameObject.SetActive(false));
            }
            _modelAnimators.ForEach(a=>a.gameObject.SetActive(false));

            float maxAnimTime = 0;
            TrackEntry maxTrackEntry = null;
            switch (_item.Config.animShowType)
            {
                case 0:
                {
                    yield return CoroutineManager.Instance.StartCoroutine(PlayShowAnim_Spine((a) => { maxTrackEntry = a;}));
                    yield return CoroutineManager.Instance.StartCoroutine(PlayShowAnim_Animator((a) => { maxAnimTime = a;}));
                    break;
                }
                case 1:
                {
                    yield return CoroutineManager.Instance.StartCoroutine(PlayShowAnim_Animator((a) => { maxAnimTime = a;}));
                    yield return CoroutineManager.Instance.StartCoroutine(PlayShowAnim_Spine((a) => { maxTrackEntry = a;}));
                    break;
                }
                case 2:
                {
                    bool isSpineEnd = false;
                    CoroutineManager.Instance.StartCoroutine(PlayShowAnim_Spine((a) => { maxTrackEntry = a;
                        isSpineEnd = true;
                    }));
                    yield return CoroutineManager.Instance.StartCoroutine(PlayShowAnim_Animator((a) => { maxAnimTime = a;}));

                    while (!isSpineEnd)
                    {
                        yield return new WaitForEndOfFrame();
                    }
                    break;
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

            FadeInNpc();

            if (_item.Node.Config.hideAfterShow)
            {
                gameObject.SetActive(false);
            }

            _loopAnimators.ForEach(a => a.enabled = true);
            //yield return new WaitForSeconds(3f);
            EnableAnimator(false, true);
            aniEndCall?.Invoke();
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
                    skeletonAnimation.UpdateCullMode = false;
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
            float animStepTime = _item.Config.animShowStepTime < 0.001f ? 0.08f : _item.Config.animShowStepTime;
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
        private void FadeInNpc()
        {
            var duration = 0.7f;

            foreach (var skeletonAnimation in _attachedNpcList)
            {
                foreach (var slot in skeletonAnimation.skeleton.Slots)
                {
                    DOTween.To(setter: value => { slot.SetColor(new Color(1, 1, 1, value)); }, startValue: 0, endValue: 1, duration: duration);
                }
            }

            foreach (var spriteRenderer in _attachedNpcShadowList)
            {
                spriteRenderer.DOColor(Color.white, duration);
            }
        }

        private void FadeOutNpc()
        {
            foreach (var skeletonAnimation in _attachedNpcList)
            {
                foreach (var slot in skeletonAnimation.skeleton.Slots)
                {
                    slot.SetColor(new Color(1, 1, 1, 0));
                }
            }

            foreach (var spriteRenderer in _attachedNpcShadowList)
            {
                spriteRenderer.color = new Color(1, 1, 1, 0);
            }
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
            if (!gameObject) return;

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
            //_tilemapRenders
            if (_tilemapRenders != null)
            {
                for (int i = 0; i < _tilemapRenders.Length; ++i)
                {
                    if (_tilemapRenders[i] == null) continue;
                    _tilemapRenders[i].material.color = color;
                }
            }
        }
        #endregion

        private static bool animationTest(Animator animator, string animationName)
        {
            var clips = animator.runtimeAnimatorController.animationClips;
            foreach (var clip in clips)
            {
                if (clip.name.Equals(animationName))
                {
                    return true;
                }
            }

            return false;
        }

        internal DecoItemTouchResult TouchTest(Vector2 screenPos)
        {
            var result = new DecoItemTouchResult();
            if (_collider == null) return result;

            var worldPos = DecoSceneRoot.Instance.mSceneCamera.ScreenToWorldPoint(screenPos);
            var shapeList = new List<Vector2>();
            //先判断大的碰撞体是否点中
            if (_collider.OverlapPoint(worldPos))
            {
                var renderChecked = false;
                if (_spRenders != null && _spRenders.Length > 0)
                {
                    var tempZ = World.DecoWorld.DefaultZ;
                    //判断每个render内是否精确点中
                    foreach (var render in _spRenders)
                    {
                        if (render.sprite && render.gameObject.activeSelf)
                        {
                            var shapeCount = render.sprite?.GetPhysicsShapeCount();
                            for (int i = 0; i < shapeCount; i++)
                            {
                                shapeList.Clear();
                                render.sprite.GetPhysicsShape(i, shapeList);
                                var localPos = render.transform.InverseTransformPoint(worldPos);
                                if (IsPointInPolygon(localPos, shapeList))
                                {
                                    var newZ = render.transform.position.z;
                                    if (World.DecoWorld.FrontTest(newZ, tempZ))
                                    {
                                        tempZ = newZ;
                                        break; //断掉本render内多个shape的判断
                                    }
                                }
                            }

                            renderChecked = true;
                        }
                    }

                    if (renderChecked)
                    {
                        if (World.DecoWorld.FrontTest(tempZ, World.DecoWorld.DefaultZ))
                        {
                            result.z = tempZ;
                            result.result = true;
                            return result;
                        }
                    }
                }

                // //TileMap也要检测，有的建筑内spriteRender和TileMap都有
                if (_tilemapRenders != null && _tilemapRenders.Length > 0)
                {
                    var tileMap = _tilemapRenders[0];
                    var layerIndex = LayerMask.NameToLayer(tileMap.sortingLayerName);
                    var sortingOrder = tileMap.sortingOrder;
                    result.result = true;
                    result.z = World.DecoWorld.ConvertZFromSoringLayerAndOrder(layerIndex, sortingOrder);
                    return result;
                }
                // else
                // {
                    //检测Spine
                    if (_meshRenders != null && _meshRenders.Length > 0)
                    {
                        result.result = true;
                        result.z = _meshRenders[0].transform.position.z;
                        return result;
                    }
                //}
            }

            result.result = false;
            return result;
        }

        public bool IsPointInPolygon(Vector2 point, List<Vector2> polygon)
        {
            int polygonLength = polygon.Count, i = 0;
            var inside = false;

            float pointX = point.x, pointY = point.y;

            float startX, startY, endX, endY;
            var endPoint = polygon[polygonLength - 1];
            endX = endPoint.x;
            endY = endPoint.y;
            while (i < polygonLength)
            {
                startX = endX;
                startY = endY;
                endPoint = polygon[i++];
                endX = endPoint.x;
                endY = endPoint.y;
                inside ^= (endY > pointY ^ startY > pointY) &&
                          ((pointX - endX) < (pointY - endY) * (startX - endX) / (startY - endY));
            }

            return inside;
        }

        internal void MarkGray(bool gray, bool anim)
        {
            // if (_item.Node.Config.GrayBindArea > 0)
            // {
            //     gray = !_item.Node.Stage.Area.World.AreaLib[_item.Node.Config.GrayBindArea].MaskRemoved;
            // }

            var darkValue = _item._node._stage.Area._data._config.darkColor;
            var color = gray ? new Color(darkValue, darkValue, darkValue) : Color.white;

            MarkColor(color, anim);
        }
    }
}