using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DragonPlus.Config.Filthy;
using Spine.Unity;
using UnityEngine;

namespace Filthy.Game.Spine
{
    public class SpineGameLogic: MonoBehaviour
    {
        private SkeletonAnimation _skeletonAnimation;

        public SkeletonAnimation skeletonAnimation
        {
            get { return _skeletonAnimation; }
        }

        private FilthySpine _config;
        
        public void OnInit(FilthySpine config)
        {
            _config = config;
            
            _skeletonAnimation = transform.Find(config.SpinePath).GetComponent<SkeletonAnimation>();

            if (config.DefaultAnim.IsEmptyString() || !config.StartAnim.IsEmptyString())
                return;

            _skeletonAnimation.AnimationState.SetAnimation(0, config.DefaultAnim, true);
            _skeletonAnimation.Update(0);
        }

        public void InitStartAnim(Action action)
        {
            if (_config == null)
            {
                action?.Invoke();
                return;
            }

            if (_config.StartAnim.IsEmptyString())
            {
                action?.Invoke();
                return; 
            }

            PlaySound();
            SetSpineAnimation(_config.StartAnim, () =>
            {
                if (!_config.DefaultAnim.IsEmptyString())
                {
                    _skeletonAnimation.AnimationState.SetAnimation(0, _config.DefaultAnim, true);
                    _skeletonAnimation.Update(0);
                }
                
                action?.Invoke();
            });
        }
        
        private async UniTask SetSpineAnimation(string animName, Action animEndCall = null)
        {
            if (_skeletonAnimation == null)
            {
                animEndCall?.Invoke();
                return;
            }
            
            var trackEntry = _skeletonAnimation.AnimationState.SetAnimation(
                0,
                animName,
                false
            );
            _skeletonAnimation.Update(0);

            await Task.Delay((int)(trackEntry.AnimationEnd*1000)+1);
            
            animEndCall?.Invoke();
        }
        
        private async void PlaySound()
        {
            if(_config.StartAudioName.IsEmptyString())
                return;

            await Task.Delay(500);
            
            var audioName = _config.StartAudioName;
            FilthyGameLogic.Instance.PlaySound(audioName);
        }
    }
}