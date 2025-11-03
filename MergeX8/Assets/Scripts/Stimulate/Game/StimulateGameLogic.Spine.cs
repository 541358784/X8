using System;
using DG.Tweening;
using DragonU3DSDK.Asset;
using Framework;
using Spine;
using Stimulate.Configs;
using Stimulate.Event;
using Stimulate.Model.Spine;
using Stimulate.System;
using UnityEngine;
using Skeleton = Spine.Skeleton;

namespace Stimulate.Model
{
    public partial class StimulateGameLogic : Manager<StimulateGameLogic>
    {
        private GameObject _spinePrefab;
        private SpineGameLogic _spineGameLogic;
        private TableStimulateSpine _spineConfig;
        private StimulateModel.NodeState _nodeState;
        
        public float _orthographicSize;
        private void LoadSpineLevel(TableStimulateNodes config)
        {
            ExitSpineLevel(true);
            
            _spineConfig = StimulateConfigManager.Instance._stimulateSpine.Find(a => a.id == config.param);
            if (_spineConfig == null)
                return;

            _nodeState = StimulateModel.Instance.GetNodeState(_nodeConfig.levelId, _nodeConfig.id);
            
            var spinePath = $"Stimulate/Levels/Level{StimulateModel.Instance._config.levelId}/Prefabs/{_spineConfig.prefabName}";
            var prefab = ResourcesManager.Instance.LoadResource<GameObject>(spinePath);
            if (prefab == null)
                return;

            _orthographicSize = CameraManager.MainCamera.orthographicSize;
            _spinePrefab = GameObject.Instantiate(prefab);
            _spinePrefab.transform.Reset();
            _spinePrefab.transform.localPosition = new Vector3(0, 0, -500);
            _spineGameLogic = _spinePrefab.AddComponent<SpineGameLogic>();
            _spineGameLogic.OnInit(_spineConfig);
            
            if(_spineConfig.cameraInitPos != null && _spineConfig.cameraInitPos.Length >= 2)
                CameraManager.MainCamera.transform.position = new Vector3(_spineConfig.cameraInitPos[0], _spineConfig.cameraInitPos[1]);
        }

        private bool ExitSpineLevel(bool isInit = false)
        {
            if(_spinePrefab == null)
                return false;
            
            CameraManager.MainCamera.orthographicSize = _orthographicSize;
            
            var state = StimulateModel.Instance.GetNodeState(_nodeConfig.levelId, _nodeConfig.id);
            
            GameObject.Destroy(_spinePrefab);
            _spinePrefab = null;
            _spineConfig = null;
            _spineGameLogic = null;
            UIManager.Instance.CloseUI(UINameConst.UIStimulateSpineMain, true);

            if (!isInit && _nodeState != state)
                return true;

            return false;
        }

        private void EnterSpineFinishAnim()
        {
            if(_spinePrefab == null)
                return;

            if (_spineConfig.cameraSize != null && _spineConfig.cameraSize > 0)
            {
                float size = CameraManager.MainCamera.orthographicSize;
                DOTween.To(() => size, value => size = value, size-_spineConfig.cameraSize, 1f).OnUpdate(() =>
                {
                    CameraManager.MainCamera.orthographicSize = size;
                    
                }).OnComplete(() =>
                {
                    UIManager.Instance.OpenUI(UINameConst.UIStimulateSpineMain, _nodeConfig, _spineConfig, _spineGameLogic);
                });
            }
            else
            {
                XUtility.WaitSeconds(3f, () =>
                {
                    UIManager.Instance.OpenUI(UINameConst.UIStimulateSpineMain, _nodeConfig, _spineConfig, _spineGameLogic);
                });
            }
        }

        public void SetSkin(Skeleton skeleton, string name, string[] skinAnims)
        {
            if(skeleton == null)
                return;
            
            if(skinAnims == null || skinAnims.Length == 0)
                return;
            
            var skins = new Skin(name);
            for (int i = 0; i < skinAnims.Length; i++)
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