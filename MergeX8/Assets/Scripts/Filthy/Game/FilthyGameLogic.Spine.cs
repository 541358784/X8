using System;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus.Config.Filthy;
using DragonU3DSDK.Asset;
using Filthy.Game.Spine;
using Filthy.Model;
using Framework;
using Spine;
using UnityEngine;
using Skeleton = Spine.Skeleton;

namespace Filthy.Game
{
    public partial class FilthyGameLogic
    {
        private GameObject _spinePrefab;
        private SpineGameLogic _spineGameLogic;
        private FilthySpine _spineConfig;
        private FilthyModel.NodeState _nodeState;
        
        public float _orthographicSize;
        private void LoadSpineLevel(FilthyNodes config)
        {
            ExitSpineLevel(true);
            
            _spineConfig = FilthyConfigManager.Instance.FilthySpineList.Find(a => a.Id == config.Param);
            if (_spineConfig == null)
                return;

            _nodeState = FilthyModel.Instance.GetNodeState(_nodeConfig.LevelId, _nodeConfig.Id);
            
            var spinePath = ConstValue.ConstValue.FilthyPrefabPath(FilthyModel.Instance.ResLevelId(), _spineConfig.PrefabName);
            var prefab = ResourcesManager.Instance.LoadResource<GameObject>(spinePath, addToCache:false);
            if (prefab == null)
                return;

            _orthographicSize = CameraManager.MainCamera.orthographicSize;
            _spinePrefab = GameObject.Instantiate(prefab);
            _spinePrefab.transform.Reset();
            _spinePrefab.transform.localPosition = new Vector3(0, 0, -500);
            _spineGameLogic = _spinePrefab.AddComponent<SpineGameLogic>();
            _spineGameLogic.OnInit(_spineConfig);
            
            if(_spineConfig.CameraInitPos != null && _spineConfig.CameraInitPos.Count >= 2)
                CameraManager.MainCamera.transform.position = new Vector3(_spineConfig.CameraInitPos[0], _spineConfig.CameraInitPos[1]);

            if(_spineConfig.OrthographicSize > 0.5f)
                CameraManager.MainCamera.orthographicSize = _spineConfig.OrthographicSize;
        }

        private bool ExitSpineLevel(bool isInit = false)
        {
            if(_spinePrefab == null)
                return false;
            
            CameraManager.MainCamera.orthographicSize = _orthographicSize;
            
            var state = FilthyModel.Instance.GetNodeState(_nodeConfig.LevelId, _nodeConfig.Id);
            
            GameObject.Destroy(_spinePrefab);
            _spinePrefab = null;
            _spineConfig = null;
            _spineGameLogic = null;
            UIManager.Instance.CloseUI(UINameConst.UIFilthySpineMain, true);

            if (!isInit && _nodeState != state)
                return true;

            return false;
        }

        private void EnterSpineFinishAnim()
        {
            if(_spinePrefab == null)
                return;

            if (_spineConfig.CameraSize != null && _spineConfig.CameraSize > 0)
            {
                float size = CameraManager.MainCamera.orthographicSize;
                DOTween.To(() => size, value => size = value, size-_spineConfig.CameraSize, 1f).OnUpdate(() =>
                {
                    CameraManager.MainCamera.orthographicSize = size;
                    
                }).OnComplete(() =>
                {
                    UIManager.Instance.OpenUI(UINameConst.UIFilthySpineMain, _nodeConfig, _spineConfig, _spineGameLogic);
                });
            }
            else
            {
                XUtility.WaitSeconds(3f, () =>
                {
                    UIManager.Instance.OpenUI(UINameConst.UIFilthySpineMain, _nodeConfig, _spineConfig, _spineGameLogic);
                });
            }
        }

        public void SetSkin(Skeleton skeleton, string name, List<string> skinAnims)
        {
            if(skeleton == null)
                return;
            
            if(skinAnims == null || skinAnims.Count == 0)
                return;
            
            var skins = new Skin(name);
            for (int i = 0; i < skinAnims.Count; i++)
            {
                Skin skin = skeleton.Data.FindSkin(skinAnims[i]);
                if (skin != null)
                {
                    skins.AddSkin(skin);
                }
            }
            skeleton.SetSkin(skins);
            skeleton.SetToSetupPose();
        }
    }
}