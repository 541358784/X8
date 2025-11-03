using System;
using ASMR;
using Framework.Utils;
using UnityEngine;


// 进入关卡，隐藏UI
//     展示完整的侧脸，停留1-1.5s后
//     镜头拉近，聚焦到耳朵

namespace fsm_new
{
    public class AsmrState_Intro : AsmrState_Base
    {
        private float _startShowTime = 1f;
        private float _cameraAnimTime = 1f;
        private float _stayTime = 1f;

        private Vector3 _cameraInitPos;
        private Vector3 _cameraTargetPos;

        public override void OnEnter(FsmStateParamBase stateParam)
        {
            base.OnEnter(stateParam);

            if (!string.IsNullOrEmpty(_asmrParam.Level.Config.VideoName))
            {
                _asmrParam.Level.ToNextStep();
            }
            else
            {
                _asmrParam.Level.camera.orthographicSize = _asmrParam.Level.Config.CameraSize_Enter[0];
                _cameraInitPos = _asmrParam.Level.camera.transform.position;
                _cameraTargetPos = new Vector3(_cameraInitPos.x, _cameraInitPos.y, _cameraInitPos.z);

                if (Math.Abs(_asmrParam.Level.Config.CameraSize_Enter[0] - _asmrParam.Level.Config.CameraSize_Enter[1]) < 0.0001f)
                {
                    _cameraAnimTime = 0f;
                    _startShowTime = 0f;
                    _stayTime = 0f;
                }
            }

            if (!string.IsNullOrEmpty(_asmrParam.Level.Config.BgMusic))
            {
                ASMR.ASMRModel.Instance.PlayMusic(_asmrParam.Level.Config.BgMusic);
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            if (_asmrParam.Level.Config.CameraSize_Enter != null) _asmrParam.Level.camera.orthographicSize = _asmrParam.Level.Config.CameraSize_Enter[1];

            UIAsmr.Open(_asmrParam.Level.Config.Id);

            EventBus.Send(new EventAsmrStepChange(0, 0, _asmrParam.Level.CurrentGroup.Steps.Count, _asmrParam.Level.Config.GroupIds.Count));
        }

        public override void FixedUpdate(float deltaTime)
        {
            base.FixedUpdate(deltaTime);

            if (!string.IsNullOrEmpty(_asmrParam.Level.Config.VideoName)) return;

            var elapsedTime = _asmrParam.ElapsedTime;

            if (elapsedTime < _startShowTime) return;

            elapsedTime -= _startShowTime;

            var t = elapsedTime / _cameraAnimTime;
            _asmrParam.Level.camera.orthographicSize = Mathf.Lerp(_asmrParam.Level.Config.CameraSize_Enter[0], _asmrParam.Level.Config.CameraSize_Enter[1], t);
            _asmrParam.Level.camera.transform.position = Vector3.Lerp(_cameraInitPos, _cameraTargetPos, t);

            elapsedTime -= _cameraAnimTime;

            if (elapsedTime < _stayTime) return;

            if (t >= 1f) _asmrParam.Level.ToNextStep();
        }
    }
}