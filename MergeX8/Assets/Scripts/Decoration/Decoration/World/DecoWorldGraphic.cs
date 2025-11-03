using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config;
using DragonU3DSDK.Storage;
using System;
using System.Linq;
using System.Resources;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using DragonU3DSDK.Asset;
using Framework;
using Deco.Area;
using Deco.Graphic;
using SomeWhere;
using DragonU3DSDK.Config;
using Spine.Unity;
using Object = UnityEngine.Object;
//using Gameplay.SubSystems;
using Decoration;
using Decoration.WorldFogManager;

namespace Deco.World
{
    public class DecoWorldGraphic : DecoGraphic
    {
        private DecoWorld _world;

        private bool _maskVisible = false;
        private GameObject _comingSoonTip;

        internal Transform _comingSoonRoot;
        internal PathMap _pathMap;
        internal PinchMapComponent _pinchMap = null;
        internal Dictionary<string, int> _npcAreaDict = new Dictionary<string, int>();

        internal DecoWorldGraphic(DecoWorld world)
        {
            _world = world;
        }

        protected override string PREFAB_PATH => Decoration.Utils.PathPrefabMap(_world.Id) + "/WorldMap";

        protected override void OnUnload()
        {
            _pinchMap.Reset();

            Object.DestroyImmediate(_comingSoonTip);
            _comingSoonTip = null;
            _pathMap = null;
            _pinchMap = null;
            _npcAreaDict.Clear();
            DecoWorld.PathPointLib.Clear();

            WorldFogManager.Instance.UnLoad();
        }

        protected override void OnLoad()
        {
            gameObject.SetActive(false);

            _comingSoonRoot = gameObject.transform.Find("MapLayer/Viewport/Content/ComingSoonTip");

            initPinchMap();
            WorldFogManager.Instance.Load(_world.Id, _pinchMap.transform);
        }

        public void Hide()
        {
            _pinchMap.Reset();

            gameObject.SetActive(false);
            _comingSoonTip?.gameObject?.SetActive(false);
        }

        internal void Show(bool fromLogin)
        {
            gameObject.SetActive(true);

            foreach (var area in _world._areaList)
            {
                area.Show(fromLogin);
            }
            
            if (_world.IsFinish())
            {
                ShowComingSoon();
            }
        }

        public void ShowComingSoon()
        {
            if (!_comingSoonRoot) return;

            if (_comingSoonTip)
            {
                if (!_comingSoonTip.activeSelf)
                {
                    _comingSoonTip.SetActive(true);
                }
                return;
            }
            if (_comingSoonRoot)
            {
                var prefabPath = "Prefabs/Home/UIWorldComingSoonButton";
                var prefab = ResourcesManager.Instance.LoadResource<GameObject>(prefabPath);
                _comingSoonTip = Object.Instantiate(prefab);
                CommonUtils.AddChild(UIRoot.Instance.mWorldUIRoot.transform, _comingSoonTip.transform, false);
                var follow = _comingSoonTip.AddComponent<ComingSoonTipComponent>();
                follow.FollowTarget(_comingSoonRoot);
            }
        }

        private void initPinchMap()
        {
            //pinchMap设置
            _pinchMap = gameObject.transform.Find("MapLayer/Viewport/Content").GetOrCreateComponent<PinchMapComponent>();
            _pinchMap.World = _world;
        }

        internal void focus(Vector2 position, float scale, bool animate, float focusTime = 1f, Action onFinished = null)
        {
            if (animate)
            {
                _pinchMap.FocusPosition(position, scale, focusTime, onFinished);
            }
            else
            {
                _pinchMap.FocusTargetPosition(position, scale, true);
                CoroutineManager.Instance.StartCoroutine(CommonUtils.DelayWorkFrame(1, () =>
                {
                    onFinished?.Invoke();
                }));
            }
        }

        internal Vector3 FindAreaBaseAnchor(int areaId)
        {
            var anchorTransform = _pinchMap.transform.Find(string.Format("Areas/{0}/BaseAnchor", areaId));
            if (anchorTransform)
                return anchorTransform.position;
            return Vector3.zero;
        }
        
        internal void FocusDefaultCameraSize(Action OnFinish)
        {
            _pinchMap.FocusDefaultCameraSize(OnFinish);
        }
    }
}