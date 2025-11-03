
using System;
using System.Collections;
using DG.Tweening;
using Framework;
using UnityEngine;

namespace Decoration.Player
{
    public class PlayerStatusPlayAnim : PlayerStatusBase
    {
        protected Coroutine _coroutine;
        
        public string _animName = "";
        protected int _playNum = 1;
        
        public override void OnInit(PlayerManager.PlayerType playerType,int npcConfigId,float crossTime, params object[] datas)
        {
            base.OnInit(playerType, npcConfigId,crossTime, datas);

            if(_npcInfo != null)
                _gameObject?.SetActive(true);
            
            _animName = GetParam<string>(0);
            if(_datas.Length >= 2)
                _playNum = GetParam<int>(1);
        }

        public override void OnStart()
        {
            if (_animName.IsEmptyString())
                return;
            
            if (_animator == null)
            {
                SwitchIdleStatus();
                return;
            }
           
            if(_crossTime > 0)
                _animator.CrossFade(_animName, _crossTime);
            else
            {
                _animator.Play(_animName, 0, 0);
            }

            float animTime = CommonUtils.GetAnimTime(_animator, _animName);

            _coroutine = CoroutineManager.Instance.StartCoroutine(PlayAnimLogic(animTime));
        }
        
        private void SwitchIdleStatus()
        {
            if(_animator == null)
                return;

            AnimatorClipInfo[] infos = _animator.GetCurrentAnimatorClipInfo(0);
            if (infos == null || infos.Length == 0)
            {
                PlayerManager.Instance.SwitchPlayerStatus(_playerType, PlayerManager.StatusType.Idle, 0.2f);
                return;
            }
            
            if(infos[0].clip.isLooping)
                return;
            
            PlayerManager.Instance.SwitchPlayerStatus(_playerType, PlayerManager.StatusType.Idle, 0.2f);
        }

        private IEnumerator PlayAnimLogic(float animTime)
        {
            yield return new WaitForSeconds(animTime);

            while (--_playNum > 0)
            {
                _animator.CrossFade(_animName, 0, -1, 0f);
                yield return new WaitForSeconds(animTime);
            }
                
            SwitchIdleStatus();
        }
        public override void OnStop()
        {
            if(_coroutine != null)
                CoroutineManager.Instance.StopCoroutine(_coroutine);
        }
    }
}