using System;
using UnityEngine;


// 进入关卡，隐藏UI
//     展示完整的侧脸，停留1-1.5s后
//     镜头拉近，聚焦到耳朵

namespace MiniGame
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

            _asmrParam.Level.camera.orthographicSize = _asmrParam.Level.Config.cameraSize_Enter[0];
            _cameraInitPos = _asmrParam.Level.camera.transform.position;
            _cameraTargetPos = new Vector3(0, 0, _cameraInitPos.z);
        }

        public override void OnExit()
        {
            base.OnExit();

            _asmrParam.Level.camera.orthographicSize = _asmrParam.Level.Config.cameraSize_Enter[1];

            UIManager.Instance.OpenUI(UINameConst.UIGameMain, _asmrParam.Level.Config);
        }

        public override void FixedUpdate(float deltaTime)
        {
            if (_asmrParam.Level.Config.isVideo)
            {
                _asmrParam.Level.ToNextStep();
                return;
            }
            
            base.FixedUpdate(deltaTime);

            var elapsedTime = _asmrParam.ElapsedTime;

            if (elapsedTime < _startShowTime) return;

            elapsedTime -= _startShowTime;

            var t = elapsedTime / _cameraAnimTime;
            _asmrParam.Level.camera.orthographicSize = Mathf.Lerp(_asmrParam.Level.Config.cameraSize_Enter[0], _asmrParam.Level.Config.cameraSize_Enter[1], t);
            _asmrParam.Level.camera.transform.position = Vector3.Lerp(_cameraInitPos, _cameraTargetPos, t);

            elapsedTime -= _cameraAnimTime;

            if (elapsedTime < _stayTime) return;

            if (t >= 1f) _asmrParam.Level.ToNextStep();
        }
    }
}