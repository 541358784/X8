using Spine.Unity;
using UnityEngine;

namespace MiniGame
{
    public class SpinePlayer
    {
        private SkeletonAnimation _animation;

        private float _startTime;
        private float _endTime;
        private bool _autoRunTrigger;


        private bool _startVib;

        public bool Finish => _animation.AnimationState.GetCurrent(0).TrackTime >= _endTime;

        public SpinePlayer(SkeletonAnimation skeleton, string animName)
        {
            // _animation = obj.GetComponent<SkeletonAnimation>();
            _animation = skeleton;
            _animation.loop = false;
            _animation.autoUpdate = false;

            _animation.AnimationState.SetAnimation(0, animName, false);
            _startTime = _animation.AnimationState.GetCurrent(0).AnimationStart;
            _endTime = _animation.AnimationState.GetCurrent(0).AnimationEnd;
            
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
                    AsmrLevel.VibrationShort();
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
            
            if(_startVib && deltaTime > 0) AsmrLevel.VibrationShort();
        }

        public void Play(string animName)
        {
            _autoRunTrigger = true;

            if (!string.IsNullOrEmpty(animName))
                _animation.AnimationState.SetAnimation(0, animName, false);
        }

        public void FixedUpdate(float deltaTime)
        {
            if (!_autoRunTrigger) return;
            if (_animation.AnimationState.GetCurrent(0).TrackTime >= _endTime) return;

            _animation.Update(deltaTime);

            if(_startVib) AsmrLevel.VibrationShort();
        }
    }
}