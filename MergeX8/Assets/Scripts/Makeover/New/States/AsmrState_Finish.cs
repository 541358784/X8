using System;
using ASMR;
using DragonU3DSDK.Audio;
using UnityEngine;

namespace MiniGame
{
    public class AsmrState_Finish : AsmrState_Base
    {
        private float _cameraAnimTime = 0.5f;

        private float _uiAnimTime;
        private Vector3 _cameraInitPos;

        public override void OnEnter(FsmStateParamBase stateParam)
        {
            base.OnEnter(stateParam);

            var window = UIManager.Instance.OpenUI(UINameConst.UIGameFinish);
            var animator = window.gameObject.GetComponent<Animator>();
            _uiAnimTime = CommonUtils.GetAnimTime(animator, "01") + CommonUtils.GetAnimTime(animator, "02");

            _cameraInitPos = _asmrParam.Level.camera.transform.position;

            Model.Instance.PlaySound("yx_common_sweep");
        }

        public override void OnExit()
        {
            base.OnExit();

            UIManager.Instance.CloseUI(UINameConst.UIGameFinish, true);
        }

        private bool _resetAll;

        public override void FixedUpdate(float deltaTime)
        {
            base.FixedUpdate(deltaTime);

            var cameraT = _asmrParam.ElapsedTime / _cameraAnimTime;
            _asmrParam.Level.camera.orthographicSize = Mathf.Lerp(_asmrParam.Level.Config.cameraSize_Finish[1], _asmrParam.Level.Config.cameraSize_Finish[0], cameraT);
            _asmrParam.Level.camera.transform.position = Vector3.Lerp(_cameraInitPos, new Vector3(0, 0, _cameraInitPos.z), cameraT);

            var uiT = (_asmrParam.ElapsedTime - _cameraAnimTime) / _uiAnimTime;

            if (!_resetAll && uiT > 0.3f)
            {
                _resetAll = true;
                _asmrParam.Level.ResetAllNormals();
            }

            if (uiT >= 1f)
            {
                Fsm.ChangeState<AsmrState_Diff>(new AsmrStateParamBase(_asmrParam.Level));
            }
        }
    }
}