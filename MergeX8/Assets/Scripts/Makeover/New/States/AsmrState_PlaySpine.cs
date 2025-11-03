using System.Collections.Generic;
using ASMR;
using UnityEngine;

namespace MiniGame
{
    public class AsmrState_PlaySpine : AsmrState_Base
    {
        private string _animName;
        private float _delayWipeTime;

        public override void OnEnter(FsmStateParamBase stateParam)
        {
            base.OnEnter(stateParam);

            if (!_toolAttached) AttackTool();

            foreach (var levelSpinePlayer in _spinePlayers) levelSpinePlayer.Play(_animName);

            if (_asmrParam.RuningStep.Config.toolLayer > 0)
            {
                var toolRender = _toolTransform.GetComponent<Renderer>();
                toolRender.sortingOrder = _asmrParam.RuningStep.Config.toolLayer;
            }

            if (_asmrParam.RuningStep.Config.hidePaths_Player_with_Delay != null && _asmrParam.RuningStep.Config.hidePaths_Player_with_Delay.Length > 1)
            {
                _delayWipeTime = float.Parse(_asmrParam.RuningStep.Config.hidePaths_Player_with_Delay[0]);
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

            CheckVib();
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

                _asmrParam.Level.CurrentGroup.FinishStep(_asmrParam.RuningStep.Config.id);
            }

            OnDelayHideWhenEnter();
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

        private int _vibIndex;
        private bool _vibing;

        private void CheckVib()
        {
            // switch (_asmrParam.RuningStep.Config.Vib_time_type)
            // {
            //     case 1:
            //         if (_vibIndex < _asmrParam.RuningStep.Config.Vib_Player_with_Delay.Count)
            //         {
            //             var delay = _asmrParam.RuningStep.Config.Vib_Player_with_Delay[_vibIndex] / 30f;
            //             if (_asmrParam.ElapsedTime < delay) return;
            //
            //             AsmrLevel.VibrationShort();
            //             _vibIndex++;
            //         }
            //
            //         break;
            //     case 2:
            //         if (_vibing) break;
            //
            //         var start = _asmrParam.RuningStep.Config.Vib_Player_with_Delay[0] / 30f;
            //         var end = _asmrParam.RuningStep.Config.Vib_Player_with_Delay[1] / 30f;
            //         if (_asmrParam.ElapsedTime > start && _asmrParam.ElapsedTime < end)
            //         {
            //             _vibing = true;
            //             AsmrLevel.VibrationShort();
            //         }
            //
            //         if (_asmrParam.ElapsedTime >= end)
            //         {
            //             _vibing = false;
            //         }
            //
            //
            //         break;
            // }
        }


        private bool _wipeDone;

        private void OnDelayHideWhenEnter()
        {
            if (_asmrParam.RuningStep.Config.hidePaths_Player_with_Delay != null && _asmrParam.RuningStep.Config.hidePaths_Player_with_Delay.Length > 1)
            {
                if (_asmrParam.ElapsedTime >= _delayWipeTime)
                {
                    if (!_wipeDone)
                    {
                        _wipeDone = true;
                        for (var i = 1; i < _asmrParam.RuningStep.Config.hidePaths_Player_with_Delay.Length; i++)
                        {
                            var path = _asmrParam.RuningStep.Config.hidePaths_Player_with_Delay[i];
                            var obj = _asmrParam.Level.transform.Find(path);
                            if (obj) obj.gameObject.SetActive(false);
                        }
                    }
                }
            }
        }
    }
}