using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Deco.Graphic;
using DragonPlus;
using DragonPlus.Config;
using DragonU3DSDK;
using DragonU3DSDK.Storage;
using UnityEngine.Tilemaps;
using Framework;
using Deco.World;
using Deco.Item;
using DragonU3DSDK.Asset;
using EpForceDirectedGraph.cs;
using Spine.Unity;
using Object = UnityEngine.Object;
using Decoration;
namespace Deco.Area
{
    public class DecoAreaGraphic : DecoGraphic
    {
        private DecoArea _area;

        private PolygonCollider2D _collider;
        private bool _maskStatus = false;
        private bool _refreshTilemapDirty = false;
        private Transform _worldArea109Node;

        private bool _refreshDirty = false;
        internal bool _resReady = false;

        internal Transform _loadingTip = null;
        internal UIAreaLoadingComponent _areaLoading = null;
        internal UIAreaLockComponent _areaLock = null;


        internal Transform _cameraOutTransform;
        internal Transform _festivalPreviewTransform;
        internal Transform _cameraPreviewTransform;
        internal Transform _cameraInTransform;
        public GameObject WorldArea109Node => _worldArea109Node?.gameObject;

        public static string LOADING_PREFAB_PATH = "Prefabs/Home/UIWorldAreaLoading";
        //public static string Lock_PREFAB_PATH = "Prefabs/UI/UIWorld/UIWorldAreaLock";

        protected override string PREFAB_PATH => $"Decoration/Worlds/World{_area.World.Id}/Prefabs/Area/{_area._data._config.id}";

        public DecoAreaGraphic(DecoArea area)
        {
            _area = area;
        }

        private void initResStatus()
        {
            _resReady = AssetCheckManager.Instance.GetAreaResNeedToDownload(_area.Id).Count <= 0;
            if (_area.Id == 110)
            {
                if (ABTest.ABTestManager.Instance.IsLockMap())
                    return;
            }
            
            if (!_resReady)
            {
                var prefab = ResourcesManager.Instance.LoadResource<GameObject>(LOADING_PREFAB_PATH);
                var areaLoadingObj = Object.Instantiate(prefab);
                CommonUtils.AddChild(UIRoot.Instance.mWorldUIRoot.transform, areaLoadingObj.transform, false);
                areaLoadingObj.transform.localPosition = new Vector3(10000, 10000, 0);
                _areaLoading = CommonUtils.GetOrCreateComponent<UIAreaLoadingComponent>(areaLoadingObj);
                _areaLoading.SetData(_area.Id);
            }
        }

        private void InitLockStatus()
        {
            // if (_area.Config.unlockLevel > ExpModel.Instance.Level)
            // {
            //    var prefab = ResourcesManager.Instance.LoadResource<GameObject>(Lock_PREFAB_PATH);
            //    if(prefab == null)
            //    {
            //        DebugUtil.LogError($"Lock_PREFAB_PATH = {Lock_PREFAB_PATH} not found!");
            //        return;
            //    }
            //    var areaLockObj = Object.Instantiate(prefab);
                
            //    CommonUtils.AddChild(UIRoot.Instance.mWorldUIRoot.transform, areaLockObj.transform, false);
            //    _areaLock = CommonUtils.GetOrCreateComponent<UIAreaLockComponent>(areaLockObj);
            //    _areaLock.SetData(_area.Id);
            //}
        }

        public void Show(bool fromLogin, bool refreshTilemapDirty = false)
        {
            if (!gameObject) return;

            if (!DecoExtendAreaManager.Instance.CanShowArea(_area.Config.id))
            {
                gameObject.SetActive(false);
                return;
            }
            
            gameObject.SetActive(true);

#if UNITY_EDITOR
            _refreshTilemapDirty = true; // 解决编辑器环境下白块问题
#else
            if (refreshTilemapDirty) _refreshTilemapDirty = true;
#endif

            foreach (var stage in _area._stageDict.Values)
            {
                stage.Show(fromLogin);
            }

            RefreshLoadingButton();
            RefreshMask(false);
        }

        public void SetActive(bool isActive)
        {
            if (!gameObject) 
                return;
            
            gameObject.SetActive(isActive);
        }
        
        public void RefreshLoadingButton()
        {
            if (_areaLoading)
            {
                if (!_resReady)
                { 
                    _areaLoading.gameObject.SetActive(true);
                    _areaLoading.FollowTarget(_loadingTip);
                }
                else
                {
                    HideDownloadButton();
                }
            }
        }

        public void HideDownloadButton()
        {
            _areaLoading?.gameObject?.SetActive(false);
        }

        internal void RefreshMask(bool anim)
        {
            if (!gameObject) return;
            _resReady = AssetCheckManager.Instance.GetAreaResNeedToDownload(_area.Id).Count <= 0;
            //_resReady = true;

            var showMask = !_resReady || _area._data._storage.State < (int)DecoArea.Status.MaskRemove;
            _maskStatus = showMask;

            markAreaGray(_maskStatus);
            RefreshLoadingButton();
            
            foreach (var stage in _area._stageDict.Values)
            {
                foreach (var node in stage._nodeDict.Values)
                {
                    node.CurrentItem?.MarkGray(_maskStatus, anim);
                }
            }
        }

        private void markAreaGray(bool gray)
        {
            var darkValue = _area._data._config.darkColor;
            var color = _maskStatus ? new Color(darkValue, darkValue, darkValue) : Color.white;

            MarkColor(color, false);
        }

        internal bool touchTest(Vector2 screenPos)
        {
            if (_collider == null) return false;

            var worldPos = DecoSceneRoot.Instance.mSceneCamera.ScreenToWorldPoint(screenPos);
            return _collider.OverlapPoint(worldPos);
        }

        protected override void OnLoad()
        {
            if (!gameObject)
            {
                gameObject = new GameObject(_area.Config.id.ToString());
                CommonUtils.AddChild(_parentTransform, gameObject.transform, false);
            }

            _loadingTip = _parentTransform.Find("LoadingTip");
            _collider = _parentTransform.GetComponent<PolygonCollider2D>();
            _cameraOutTransform = _parentTransform.Find("CameraOut");
            _festivalPreviewTransform = _parentTransform.Find("CameraPreview");
            _cameraInTransform = _parentTransform.Find("CameraIn");
            _cameraPreviewTransform = _parentTransform.Find("CameraPreview");
            if (_area.Config.id == 109)
            {
                _worldArea109Node = _parentTransform.Find("109/Common/WorldArea109");
            }

            foreach (var stage in _area._stageDict.Values)
            {
                stage.LoadGraphic(gameObject);
            }

            initResStatus();
            InitLockStatus();
            
            if(!DecoExtendAreaManager.Instance.CanShowArea(_area.Config.id))
                gameObject.SetActive(false);
        }

        protected override void OnUnload()
        {
            OpUtils.UnloadObjFromBundleManager(LOADING_PREFAB_PATH);

            if (_areaLoading != null) Object.DestroyImmediate(_areaLoading.gameObject);

            _areaLoading = null;
            _areaLock = null;
            _collider = null;
            _loadingTip = null;
            _cameraOutTransform = null;
            _festivalPreviewTransform = null;
        }
    }
}