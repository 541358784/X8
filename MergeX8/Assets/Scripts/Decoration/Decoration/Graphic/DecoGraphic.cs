using System;
using System.Collections;
using System.Collections.Generic;
using DragonU3DSDK.Asset;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Tilemaps;
using Decoration;
using DG.Tweening;
using Object = UnityEngine.Object;

namespace Deco.Graphic
{
    public abstract class DecoGraphic
    {
        public GameObject gameObject;
        protected Transform _parentTransform;
        protected TilemapRenderer[] _tilemapRenders;
        // protected Tilemap[] _tilemaps;
        protected SpriteRenderer[] _spRenders;
        protected MeshRenderer[] _meshRenders;
        internal protected SkeletonAnimation[] _skeletonAnimations;
        protected SkeletonRenderer[] _skeletonRenderers;
        protected Dictionary<int, List<Animator>> _animatorDic = new Dictionary<int, List<Animator>>();

        protected Dictionary<string, Transform> _effectDic = new Dictionary<string, Transform>();
        protected Dictionary<string, string> _effectNameDic = new Dictionary<string, string>();
        
        private static Material _tilemapRenderMaterial;
        public SpriteRenderer[] Renders => _spRenders;

        protected abstract string PREFAB_PATH { get; }
        protected abstract void OnUnload();
        protected abstract void OnLoad();

        protected bool _isAsync = false;

        private bool _isMarkUnLoad = false;
        
        public bool IsAsync
        {
            get { return _isAsync; }
        }
        public void Load(Transform parent)
        {
            if (gameObject) 
                return;
            
            if (parent) 
                _parentTransform = parent;

            if (!string.IsNullOrEmpty(PREFAB_PATH))
            {
                var prefab = ResourcesManager.Instance.LoadResource<GameObject>(PREFAB_PATH, addToCache: false);
                if (prefab)
                {
                    gameObject = Object.Instantiate(prefab);
#if UNITY_EDITOR
                    gameObject.name = gameObject.name.Replace("(Clone)", "");
#endif
                    CommonUtils.AddChild(_parentTransform, gameObject.transform, false);
                    //gameObject.SetActive(false);
                }
            }

            LoadInit();
        }
        
        public IEnumerator AsyncLoad(Transform parent, Action onFinished, bool isPreview = false)
        {
            _isAsync = true;
            if (gameObject)
            {
                _isAsync = false;
                onFinished?.Invoke();
                yield break;
            }
            
            if (parent) 
                _parentTransform = parent;

            bool waitAsync = true;
            if (!string.IsNullOrEmpty(PREFAB_PATH))
            {
                ResourcesManager.Instance.LoadResourceAsync<GameObject>(PREFAB_PATH, 
                    (prefab) =>
                    {
                        if (prefab)
                        {
                            gameObject = Object.Instantiate(prefab);
#if UNITY_EDITOR
                            gameObject.name = gameObject.name.Replace("(Clone)", "");
#endif
                            CommonUtils.AddChild(_parentTransform, gameObject.transform, false);
                            
                            if(isPreview)
                                gameObject.SetActive(false);
                        }

                        waitAsync = false;
                    }, isAddCache:true, false);
            }

            while (waitAsync)
            {
                yield return new WaitForEndOfFrame();
            }

            LoadInit();
            onFinished?.Invoke();
        }

        private void LoadInit()
        {
            InitRenders();
            ShowGraphic(true);
            EnableAnimator(true);
            OnLoad();
            _isAsync = false;

            if (_isMarkUnLoad)
            {
                Unload();
                _isMarkUnLoad = false;
            }
        }
        
        public void Unload()
        {
            if (_isAsync)
            {
                _isMarkUnLoad = true;
                return;
            }
            
            OnUnload();

            if (_spRenders != null)
            {
                foreach (var spriteRenderer in _spRenders)
                    spriteRenderer.sprite = null;

                _spRenders = null;
            }
                
            OpUtils.UnloadObjFromBundleManager(PREFAB_PATH, false);
            Object.DestroyImmediate(gameObject);

            gameObject = null;
        }

        public virtual void ShowGraphic(bool show)
        {
            if (_spRenders != null)
                foreach (var spriteRenderer in _spRenders)
                    spriteRenderer.enabled = show;
            if (_meshRenders != null)
                foreach (var meshRenderer in _meshRenders)
                    meshRenderer.enabled = show;
            if (_skeletonRenderers != null)
                foreach (var render in _skeletonRenderers)
                    render.enabled = show;
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

        protected void InitRenders()
        {
            if (!gameObject) return;

            _tilemapRenders = gameObject.transform.GetComponentsInChildren<TilemapRenderer>(true);
            // _tilemaps = gameObject.transform.GetComponentsInChildren<Tilemap>(true);
            if (_tilemapRenders != null && _tilemapRenders.Length > 0)
            {
                if (_tilemapRenderMaterial == null) _tilemapRenderMaterial = _tilemapRenders[0].material;
                foreach (var render in _tilemapRenders)
                {
                    // render.material = _tilemapRenderMaterial;
                    render.sharedMaterial = _tilemapRenderMaterial;
                }
            }


            _spRenders = gameObject.transform.GetComponentsInChildren<SpriteRenderer>(true);
            if (_spRenders != null)
            {
                foreach (var render in _spRenders)
                {
                    render.allowOcclusionWhenDynamic = true;
                    OpUtils.ReplaceDefaultShader(render);
                }
            }

            _meshRenders = gameObject.transform.GetComponentsInChildren<MeshRenderer>(true);
            _skeletonAnimations = gameObject.GetComponentsInChildren<SkeletonAnimation>();
            _skeletonRenderers = gameObject.GetComponentsInChildren<SkeletonRenderer>();

            if (_skeletonAnimations != null)
            {
                foreach (var skeletonAnimation in _skeletonAnimations)
                {
                    if(skeletonAnimation != null)
                        skeletonAnimation.UpdateCullMode = true;
                }
            }
        }

        protected void MarkColor(Color color, bool anim)
        {
            return;
            
            anim = anim && gameObject.activeSelf;

            if (_spRenders != null)
            {
                foreach (var renderer in _spRenders)
                {
                    if (renderer)
                    {
                        if (anim)
                        {
                            renderer.DOColor(color, 0.5f);
                        }
                        else
                        {
                            renderer.color = color;
                        }
                    }
                }
            }

            if (_skeletonAnimations != null && _skeletonAnimations.Length > 0)
            {
                foreach (var skeletonAnimation in _skeletonAnimations)
                {
                    foreach (var slot in skeletonAnimation.skeleton.Slots)
                    {
                        if (anim)
                        {
                            slot.SetColor(color);
                        }
                        else
                        {
                            slot.SetColor(color);
                        }
                    }
                }
            }
        }
    }
}