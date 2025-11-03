
using System;
using DG.Tweening;
using Framework;
using UnityEngine;

namespace Decoration.Player
{
    public class PlayerStatusAutoFade : PlayerStatusFade
    {
        private Vector3 _localPosition = Vector3.zero;
        private bool _isShowNow = true;
        
        public override void OnInit(PlayerManager.PlayerType playerType, int npcConfigId, float crossTime, params object[] datas)
        {
            var playerObj = PlayerManager.Instance.GetPlayer(playerType);
            if (playerObj != null)
                _isShowNow = playerObj.gameObject.activeSelf;
            
            base.OnInit(playerType, npcConfigId, crossTime, datas);
            
        }
        public override void OnStart()
        {
            if(_npcInfo == null)
                return;
            
            if(!_npcInfo.isShow)
                return;

            Vector3 newPos = GetVector3(_npcInfo.position);
            if (!_isShowNow)
            {
                SetLocalPosition();
            }
            _localPosition = _gameObject.transform.localPosition;

            if (_isAnim)
            {
                if (Vector3.Distance(_localPosition, newPos) > 0.01f)
                {
                    Fade(false, _animTime, true);
                    _coroutine = CoroutineManager.Instance.StartCoroutine(CommonUtils.DelayWork(_animTime*2, () =>
                    {
                        SetLocalPosition();
                        SetRotation();
                        PlayAnimation();
                        Fade(true, _animTime, true, SwitchIdleStatus);
                    }));
                }
                else
                {
                    SetLocalPosition();
                    SetRotation();
                    PlayAnimation();
                    Fade(true, _animTime, false, SwitchIdleStatus);
                }
            }
            else
            {
                SetLocalPosition();
                SetRotation();
                PlayAnimation();
                Fade(true, _animTime, false, SwitchIdleStatus);
            }
        }
        
        private void SwitchIdleStatus()
        {
            if (_playerType == PlayerManager.PlayerType.Chief)
            {
                if(_npcInfo != null && _npcInfo.animName == PlayerManager.Instance.GetAnimationName(_playerType, PlayerManager.ActionName.Sit))
                    return;
            }
            
            PlayerManager.Instance.SwitchPlayerStatus(_playerType, PlayerManager.StatusType.Idle);
        }
        
        public override void OnStop()
        {
            if(_tween != null)
                _tween.Kill();

            if(_spriteRenderer != null)
                _spriteRenderer.DOKill();
            
            if(_coroutine != null)
                CoroutineManager.Instance.StopCoroutine(_coroutine);
        }
    }
}