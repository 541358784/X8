
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Decoration.Player
{
    public class PlayerStatusBase
    {
        protected PlayerManager.PlayerType _playerType;
        protected TableNpcConfig _npcConfig;
        public TableNpcInfo _npcInfo;
        protected object[] _datas;

        protected Animator _animator;
        protected List<Material> _materials;
        protected SpriteRenderer _spriteRenderer;
        protected GameObject _gameObject;
        protected float _crossTime = 0.1f;

        public PlayerManager.StatusType _statusType = PlayerManager.StatusType.None;
        
        public virtual void OnInit(PlayerManager.PlayerType playerType,  int npcConfigId, float crossTime, params object[] datas)
        {
            _datas = datas;
            _playerType = playerType;
            _crossTime = crossTime;

            if (npcConfigId > 0)
            {
                _npcConfig = DecorationConfigManager.Instance.NpcConfigList.Find(a => a.id == npcConfigId);
                if(_npcConfig == null)
                    return;
                
                int npcInfoId = -1;
                switch (playerType)
                {
                    case PlayerManager.PlayerType.Chief:
                        npcInfoId = _npcConfig.chiefInfoId;
                        break;
                    case PlayerManager.PlayerType.Dog:
                        npcInfoId = _npcConfig.dogInfoId;
                        break;
                    case PlayerManager.PlayerType.Hero:
                        npcInfoId = _npcConfig.heroInfoId;
                        break;
                }
                _npcInfo = DecorationConfigManager.Instance.NpcInfoList.Find(a => a.id == npcInfoId);
            }

            InitPlayerData();
            SetPlayerActive();
            BuildLinkPath();
        }

        public virtual void OnStart()
        {
            
        }

        public virtual void OnStop()
        {
            
        }

        public virtual void OnUpdate()
        {
            
        }

        protected void SetPlayerActive()
        {
            if(_npcInfo == null)
                return;
            
            _gameObject?.SetActive(_npcInfo.isShow);
        }
        
        private void InitPlayerData()
        { 
            _animator = PlayerManager.Instance.GetAnimator(_playerType);
           _materials = PlayerManager.Instance.GetMaterials(_playerType);
           _spriteRenderer = PlayerManager.Instance.GetSpriteRenderer(_playerType);
           _gameObject = PlayerManager.Instance.GetPlayer(_playerType);
        }

        private void BuildLinkPath()
        {
            if(_npcInfo == null)
                return;
            
            Transform parent = DecoSceneRoot.Instance.transform;
            if (_npcInfo.linkPath.IsEmptyString())
            {
                _gameObject.transform.SetParent(parent);
                return;
            }
            
            parent = DecoManager.Instance.CurrentWorld.PinchMap.transform.Find(_npcInfo.linkPath);
            parent = parent == null ? DecoSceneRoot.Instance.transform : parent;
            _gameObject.transform.SetParent(parent);
        }
        
        protected void SetLocalPosition()
        {
            if(_gameObject == null)
                return;
            
            _gameObject.transform.localPosition = GetVector3(_npcInfo.position);
        }

        protected void PlayAnimation()
        {
            if(_npcInfo == null || _animator == null)
                return;

            _animator.CrossFade(_npcInfo.animName, _crossTime);
        }
        
        public void SetRotation()
        {     
            if(_gameObject == null)
                return;

            _gameObject.transform.rotation = Quaternion.Euler(GetVector3(_npcInfo.rotation));
        }

        protected Vector3 GetVector3(float[] array)
        {
            Vector3 vector3 = Vector3.zero;

            if (array == null)
                return vector3;

            if (array.Length >= 1)
                vector3.x = array[0];
            if(array.Length >= 2)
                vector3.y = array[1];
            if(array.Length >= 3)
                vector3.z = array[2];

            return vector3;
        }
        
        protected T GetParam<T>(int index)
        {
            if (_datas == null)
                return default(T);
            
            if(index < 0 || index >= _datas.Length)
                return default(T);

            return (T)_datas[index];
        }

        public void HidePlayer()
        {
            if(_gameObject == null)
                return;

            var skinnedRenders = _gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
            if(skinnedRenders != null)
            {
                foreach (var skinnedMeshRenderer in skinnedRenders)
                {
                    skinnedMeshRenderer.enabled = false;
                }
            }
            
            var spriteRenders = _gameObject.GetComponentsInChildren<SpriteRenderer>();
            if(spriteRenders != null)
            {
                foreach (var spRenderer in spriteRenders)
                {
                    spRenderer.enabled = false;
                }
            }
        }
        public void RecoverPlayer()
        {
            if(_gameObject == null)
                return;

            var skinnedRenders = _gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
            if(skinnedRenders != null)
            {
                foreach (var skinnedMeshRenderer in skinnedRenders)
                {
                    skinnedMeshRenderer.enabled = true;
                }
            }
            
            var spriteRenders = _gameObject.GetComponentsInChildren<SpriteRenderer>();
            if(spriteRenders != null)
            {
                foreach (var spRenderer in spriteRenders)
                {
                    spRenderer.enabled = true;
                }
            }
        }
    }
}