
using System;
using DG.Tweening;
using Framework;
using UnityEngine;

namespace Decoration.Player
{
    public class PlayerStatusFade : PlayerStatusBase
    {
        protected Tween _tween;
        protected Coroutine _coroutine;
        
        protected bool _isShow = false;
        protected bool _isAnim = false;
        protected float _animTime = 0;
        
        
        private Vector3 _localPosition = Vector3.zero;
        public override void OnInit(PlayerManager.PlayerType playerType,int npcConfigId,float crossTime, params object[] datas)
        {
            base.OnInit(playerType, npcConfigId,crossTime, datas);

            _isShow = GetParam<bool>(0);
            _isAnim = GetParam<bool>(1);
            _animTime = GetParam<float>(2);
        }

        public override void OnStart()
        {
            if(_npcInfo != null)
                _gameObject?.SetActive(true); 
            
            Fade(_isShow, _animTime, _isAnim, () =>
            {
                //PlayerManager.Instance.SwitchPlayerStatus(_playerType, PlayerManager.StatusType.Idle);
            });
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
        
        protected void Fade(bool isShow, float animTime = 1f, bool isAnim = true, Action action = null)
        {
            if (_materials == null)
            {
                action?.Invoke();
                return;
            }

            float normalAlpha = isAnim ? (isShow ? 0 : 1) : (isShow ? 1 : 0);
        
            foreach (var material in _materials)
            {
                material.SetFloat("_Alpha", normalAlpha);
            }
            if (_spriteRenderer != null)
            {
                var alphaColor = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, normalAlpha);
                _spriteRenderer.color = alphaColor;
            }

            if (!isAnim)
            {
                action?.Invoke();
                return;
            }
        
            if(_tween != null)
                _tween.Kill();
        
            float alpha = (isShow ? 1 : 0);
            _tween = DOTween.To(() => normalAlpha, x => alpha = x, alpha, animTime).OnUpdate(() =>
            {
                foreach (var material in _materials)
                {
                    material.SetFloat("_Alpha", alpha);
                }
            }).OnComplete(() =>
            {
                _tween = null;
                action?.Invoke();
            });

            if (_spriteRenderer != null)
            {
                _spriteRenderer.DOFade(alpha, animTime);
            }
        }
    }
}