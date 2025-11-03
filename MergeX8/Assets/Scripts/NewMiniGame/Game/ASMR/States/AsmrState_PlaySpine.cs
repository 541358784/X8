using UnityEngine;

namespace fsm_new
{
    public class AsmrState_PlaySpine : AsmrState_Base
    {
        private string _animName;
        private float _delayWipeTime;

        private bool _showNodeRequied;

        public override void OnEnter(FsmStateParamBase stateParam)
        {
            base.OnEnter(stateParam);

            if (!_toolAttached) AttackTool();

            foreach (var levelSpinePlayer in _spinePlayers) levelSpinePlayer.Play(_animName);

            if (_asmrParam.RuningStep.Config.HidePaths_Player_with_Delay != null && _asmrParam.RuningStep.Config.HidePaths_Player_with_Delay.Count > 1)
            {
                _delayWipeTime = float.Parse(_asmrParam.RuningStep.Config.HidePaths_Player_with_Delay[0]);
            }

            ShowTargets(false);
        }

        public override void OnExit()
        {
            base.OnExit();

            if (_toolAttached) ResetTool();

            UpAllToolsLayer();
        }

        public override void FixedUpdate(float deltaTime)
        {
            base.FixedUpdate(deltaTime);

            if (!_showNodeRequied)
            {
                _showNodeRequied = true;

                if (_asmrParam.RuningStep.Config.ShowPaths_SpinePlay != null)
                {
                    foreach (var s in _asmrParam.RuningStep.Config.ShowPaths_SpinePlay)
                    {
                        var t = _asmrParam.Level.transform.Find(s);
                        if (t) t.gameObject.SetActive(true);
                    }
                }
            }


            CheckAudio();

            var allFinish = true;
            foreach (var levelSpinePlayer in _spinePlayers)
            {
                if (!levelSpinePlayer.Finish)
                {
                    allFinish = false;
                    break;
                }
            }

            if (allFinish)
            {
//                Debug.LogError($"SpinePlayer Finish");

                if (!AsmrInputHandler_Base.TouchEnd) return;
                if (_asmrParam.RuningStep.Config.TransitionAnimationType == 1)
                {
                    _asmrParam.fsm.ChangeState<AsmrState_Transition>(new AsmrStateParamBase(_asmrParam.Level, _asmrParam.RuningStep, _asmrParam.fsm));
                }
                else
                {
                    _asmrParam.Level.CurrentGroup.FinishStep(_asmrParam.RuningStep.Config.Id);
                }
                
            }

            OnDelayHideWhenEnter();
        }

        private bool _audioPlaying;

        private void CheckAudio()
        {
            if (_audioPlaying) return;
            var delay = 0f;
            if (_asmrParam.RuningStep.Config.SoundDelay != null) delay = _asmrParam.RuningStep.Config.SoundDelay[0];
            if (!(_asmrParam.ElapsedTime > delay)) return;
            if (_asmrParam.RuningStep.Config.Sound == null) return;

            _audioPlaying = true;
            ASMR.ASMRModel.Instance.PlaySound(_asmrParam.RuningStep.Config.Sound[0]);
        }

        private int _vibIndex;
        private bool _vibing;


        private bool _wipeDone;

        private void OnDelayHideWhenEnter()
        {
            if (_asmrParam.RuningStep.Config.HidePaths_Player_with_Delay == null || _asmrParam.RuningStep.Config.HidePaths_Player_with_Delay.Count <= 1) return;
            if (!(_asmrParam.ElapsedTime >= _delayWipeTime)) return;
            if (_wipeDone) return;
            _wipeDone = true;

            for (var i = 1; i < _asmrParam.RuningStep.Config.HidePaths_Player_with_Delay.Count; i++)
            {
                var path = _asmrParam.RuningStep.Config.HidePaths_Player_with_Delay[i];
                var obj = _asmrParam.Level.transform.Find(path);
                if (obj) obj.gameObject.SetActive(false);
            }
        }
    }
}