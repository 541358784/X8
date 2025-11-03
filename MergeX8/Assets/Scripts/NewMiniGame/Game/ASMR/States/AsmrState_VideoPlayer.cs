using UnityEngine;
using UnityEngine.Video;

namespace fsm_new
{
    public class AsmrState_VideoPlayer : AsmrState_Click
    {
        public override void OnEnter(FsmStateParamBase stateParam)
        {
            base.OnEnter(stateParam);

            var useSD = false;
            var fileName = _asmrParam.Level.Config.VideoName;
            if (useSD) fileName = $"{_asmrParam.Level.Config.VideoName}_sd";
            if (_asmrParam.Level.VideoPlayer.clip == null) _asmrParam.Level.VideoPlayer.clip = Resources.Load<VideoClip>($"asmr_video/{fileName}");
        }

        public override void OnExit()
        {
            base.OnExit();

            _asmrParam.Level.VideoPlayer.Pause();
            _asmrParam.Level.VideoAudioSource.Pause();
        }

        protected override void HandleClick(AsmrStep step)
        {
            _asmrParam.Level.VideoPlayer.Play();
            _asmrParam.Level.VideoAudioSource.Play();

            foreach (var targetCollider in _targetColliders)
            {
                targetCollider.gameObject.SetActive(false);
            }
        }

        public override void FixedUpdate(float deltaTime)
        {
            base.FixedUpdate(deltaTime);

            switch (_asmrParam.RuningStep.Config.VideoFrame)
            {
                case > 0:
                {
                    if (_asmrParam.Level.VideoPlayer.time >= _asmrParam.RuningStep.Config.VideoFrame)
                    {
                        _asmrParam.Level.CurrentGroup.FinishStep(_asmrParam.RuningStep.Config.Id);
                    }

                    break;
                }
                default:
                {
                    if (_asmrParam.Level.VideoPlayer.time + 0.1f >= _asmrParam.Level.VideoPlayer.length)
                    {
                        _asmrParam.Level.CurrentGroup.FinishStep(_asmrParam.RuningStep.Config.Id);
                    }

                    break;
                }
            }
        }
    }
}