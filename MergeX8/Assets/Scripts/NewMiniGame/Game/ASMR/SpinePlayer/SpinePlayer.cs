using DragonU3DSDK;
using Framework;
using Spine.Unity;
using UnityEngine;

namespace asmr_new
{
    public class SpinePlayer
    {
        private SkeletonAnimation _animation;

        private float _startTime;
        private float _endTime;
        private bool _autoRunTrigger;

        private string[] _animParams;
        private int _animIndex;


        private bool _startVib;

        public bool Finish { get; private set; }

        public SpinePlayer(SkeletonAnimation skeleton, string animParam, bool forceUpdateFirstFrame)
        {
            _animation = skeleton;
            _animation.loop = false;
            _animation.autoUpdate = false;

            _animParams = animParam.Split('-');

            StartAnim(forceUpdateFirstFrame);

            AddEvent();
        }

        private void AddEvent()
        {
            _animation.AnimationState.Event -= HandleAnimationEvent;
            _animation.AnimationState.Event += HandleAnimationEvent;
        }

        public void ClearEvent()
        {
            _animation.AnimationState.Event -= HandleAnimationEvent;
        }

        private void HandleAnimationEvent(Spine.TrackEntry trackentry, Spine.Event e)
        {
            // 处理触发的事件
            switch (e.Data.Name)
            {
                case "start":
                    _startVib = true;
                    break;
                case "vib":
                    ShakeManager.Instance.ShakeLight();
                    break;
                case "end":
                    _startVib = false;
                    break;
            }
        }

        public void Play(float progress)
        {
            if (_autoRunTrigger) return;
            if (progress >= 0.9f) _autoRunTrigger = true;


            var trackTime = progress * (_endTime - _startTime) + _startTime;

            var deltaTime = trackTime - _animation.AnimationState.GetCurrent(0).TrackTime;

            _animation.Update(deltaTime);

            if (_startVib && deltaTime > 0) ShakeManager.Instance.ShakeLight();
        }

        public void Play(string animName)
        {
            _autoRunTrigger = true;

            if (!string.IsNullOrEmpty(animName))
            {
                //⚠️ 这个地方如果本身的track非空，那么这行代码只是切换了状态，动画并没有立即播放，
                //所以下边的fixUpdate方法在只有一帧的动画的情况下，要Update一下

                _animation.AnimationState.SetAnimation(0, animName, false);
            }
            else
            {
                StartAnim(false);
            }
        }

        public void FixedUpdate(float deltaTime)
        {
            if (!_autoRunTrigger) return;
            if (Finish) return;
            if (_animation.AnimationState.GetCurrent(0).TrackTime >= _endTime)
            {
                if (_animIndex < _animParams.Length - 1)
                {
                    _animIndex++;
                    StartAnim(false);
                }
                else
                {
                    if (_endTime == 0) //只有一帧的动画，_endTime==0,如果return掉，动画的表现不会刷成第一帧的样子，所以这里Update一下
                        _animation.Update(deltaTime);

                    Finish = true;
                    return;
                }
            }

            _animation.Update(deltaTime);

            if (_startVib) ShakeManager.Instance.ShakeLight();
        }

        private void StartAnim(bool forceUpdateFirstFrame)
        {
            var animName = _animParams[_animIndex];
            _animation.AnimationState.SetAnimation(0, animName, false);

            if (forceUpdateFirstFrame)
            {
                _animation.Update(0);
                _animation.LateUpdate();
            }

            var track = _animation.AnimationState.GetCurrent(0);
            _startTime = track.AnimationStart;
            _endTime = track.AnimationEnd;
        }
    }
}