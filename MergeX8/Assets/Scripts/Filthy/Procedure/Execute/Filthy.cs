using System;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.Filthy;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using Filthy.Game;
using Filthy.Game.Spine;
using Filthy.Model;
using Framework;
using UnityEngine;

namespace Filthy.Procedure
{
    public class Filthy : IExecute
    {
        public Transform _root { get; set; }
        public ProcedureBase _procedureBase { get; set; }

        private GameObject _spinePrefab;
        private float _orthographicSize;
        private SpineGameLogic _spineGameLogic;
        private FilthySpine _spineConfig;
        public void Init(Transform root, ProcedureBase procedureBase)
        {           
            _root = root;
            _procedureBase = procedureBase;
        }

        public bool Execute()
        {
            _root.transform.Find("Root/SkipButton").gameObject.SetActive(false);
            LoadSpineLevel(int.Parse(_procedureBase._config.ExecuteParam));
            return true;
        }

        private void LoadSpineLevel(int id)
        {
            ExitSpineLevel();
            
            FilthyGameLogic.Instance.SetFithyUIActive(false);
            _spineConfig = FilthyConfigManager.Instance.FilthySpineList.Find(a => a.Id == id);
            if (_spineConfig == null)
                return;
            
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
            
            EnterSpineFinishAnim();
        }

        private void ExitSpineLevel()
        {
            if(_spinePrefab == null)
                return;
            
            CameraManager.MainCamera.orthographicSize = _orthographicSize;
            
            GameObject.Destroy(_spinePrefab);
            _spinePrefab = null;
            _spineGameLogic = null;
            UIManager.Instance.CloseUI(UINameConst.UIFilthySpineMain, true);
        }
        
        private void EnterSpineFinishAnim()
        {
            if(_spinePrefab == null)
                return;

            Action action = () =>
            {
                FilthyGameLogic.Instance.SetFithyUIActive(true);
                ExitSpineLevel();
                FilthyGameLogic.Instance.TriggerProcedure(TriggerType.LevelFinish, _procedureBase._config.ExecuteParam);
            };
            
            if (_spineConfig.CameraSize != null && _spineConfig.CameraSize > 0)
            {
                float size = CameraManager.MainCamera.orthographicSize;
                DOTween.To(() => size, value => size = value, size-_spineConfig.CameraSize, 1f).OnUpdate(() =>
                {
                    CameraManager.MainCamera.orthographicSize = size;
                    
                }).OnComplete(() =>
                {
                    _spineGameLogic.InitStartAnim(() =>
                    {
                        UIManager.Instance.OpenUI(UINameConst.UIFilthySpineMain, null, _spineConfig, _spineGameLogic, action);
                    });
                });
            }
            else
            {
                XUtility.WaitSeconds(0.5f, () =>
                {
                    _spineGameLogic.InitStartAnim(() =>
                    {
                        UIManager.Instance.OpenUI(UINameConst.UIFilthySpineMain, null, _spineConfig, _spineGameLogic, action);
                    });
                });
            }
        }
    }
}