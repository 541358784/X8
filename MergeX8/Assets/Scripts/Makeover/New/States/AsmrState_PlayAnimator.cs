using System;
using ASMR;
using UnityEngine;

namespace MiniGame
{
    public class AsmrState_PlayAnimatorParam : AsmrStateParamBase
    {
        public AsmrState_PlayAnimatorParam(AsmrLevel level, AsmrStep step, Animator animator, string animName, Action onFinish) : base(level, step)
        {
            Animator = animator;
            this.animName = animName;
            OnFinish = onFinish;
        }

        public Animator Animator;
        public string animName;
        public Action OnFinish;
    }


    public class AsmrState_PlayAnimator : AsmrState_Base
    {
        private AsmrState_PlayAnimatorParam _param;

        private float _animTime;

        private float _delayWipeTime;

        public override void OnEnter(FsmStateParamBase stateParam)
        {
            base.OnEnter(stateParam);

            _param = stateParam as AsmrState_PlayAnimatorParam;

            _animTime = CommonUtils.GetAnimTime(_param.Animator, _param.animName);

            _param.Animator.gameObject.SetActive(true);
            _param.Animator.Play(_param.animName);

            if (_param.RuningStep.Config.hidePaths_Player_with_Delay != null && _param.RuningStep.Config.hidePaths_Player_with_Delay.Length > 1)
            {
                _delayWipeTime = float.Parse(_param.RuningStep.Config.hidePaths_Player_with_Delay[0]);
            }
        }

        public override void FixedUpdate(float deltaTime)
        {
            base.FixedUpdate(deltaTime);

            var nomalizedTime = _param.ElapsedTime / _animTime;
            _param.Animator.Play(_param.animName, -1, nomalizedTime);

            if (_param.ElapsedTime >= _animTime)
            {
                _param.OnFinish.Invoke();
            }

            OnDelayHideWhenEnter();

            CheckAudio();
        }

        private bool _audioPlaying;

        private void CheckAudio()
        {
            if (!_audioPlaying)
            {
                var delay = 0f;
                if (_asmrParam.RuningStep.Config.soundDelay != null) delay = _asmrParam.RuningStep.Config.soundDelay[0];
                if (_asmrParam.ElapsedTime > delay)
                {
                    if (_asmrParam.RuningStep.Config.sound != null)
                    {
                        _audioPlaying = true;
                        Model.Instance.PlaySound(_asmrParam.RuningStep.Config.sound[0]);
                    }
                }
            }
        }

        private bool _wipeDone;

        private void OnDelayHideWhenEnter()
        {
            if (_param.RuningStep.Config.hidePaths_Player_with_Delay != null && _param.RuningStep.Config.hidePaths_Player_with_Delay.Length > 1)
            {
                if (_param.ElapsedTime >= _delayWipeTime)
                {
                    if (!_wipeDone)
                    {
                        _wipeDone = true;
                        for (var i = 1; i < _param.RuningStep.Config.hidePaths_Player_with_Delay.Length; i++)
                        {
                            var path = _param.RuningStep.Config.hidePaths_Player_with_Delay[i];
                            var obj = _param.Level.transform.Find(path);
                            if (obj) obj.gameObject.SetActive(false);
                        }
                    }
                }
            }
        }
    }
}