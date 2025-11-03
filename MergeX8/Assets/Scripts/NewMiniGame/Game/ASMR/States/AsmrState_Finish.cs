using ASMR;
using Framework;
using Framework.UI;
using UnityEngine;

namespace fsm_new
{
    public class AsmrState_Finish : AsmrState_Base
    {
        private float _cameraAnimTime = 0.5f;

        private float _uiAnimTime;
        private Vector3 _cameraInitPos;

        public override void OnEnter(FsmStateParamBase stateParam)
        {
            base.OnEnter(stateParam);

            UIASMRFinish.Open();

             var ui = Framework.UI.UIManager.Instance.GetView<UIASMRFinish>();
             _uiAnimTime = ui.GetAnimTime("01") + ui.GetAnimTime("02");

            _cameraInitPos = _asmrParam.Level.camera.transform.position;

            ASMR.ASMRModel.Instance.PlaySound("yx_common_sweep");
        }

        public override void OnExit()
        {
            base.OnExit();

            Framework.UI.UIManager.Instance.Close<UIASMRFinish>();
        }

        private bool _resetAll;

        public override void FixedUpdate(float deltaTime)
        {
            base.FixedUpdate(deltaTime);

            var cameraT = _asmrParam.ElapsedTime / _cameraAnimTime;
            _asmrParam.Level.camera.orthographicSize = Mathf.Lerp(_asmrParam.Level.Config.CameraSize_Finish[1], _asmrParam.Level.Config.CameraSize_Finish[0], cameraT);
            _asmrParam.Level.camera.transform.position = Vector3.Lerp(_cameraInitPos, new Vector3(0, 0, _cameraInitPos.z), cameraT);

            var uiT = (_asmrParam.ElapsedTime - _cameraAnimTime) / _uiAnimTime;

            if (!_resetAll && uiT > 0.3f)
            {
                _resetAll = true;
                _asmrParam.Level.ResetAllNormals();
                _asmrParam.Level.HideNode_Finish(); /* 结束时扫光动画流程希望关闭一些节点 */
            }

            if (uiT >= 1f)
            {
                _asmrParam.fsm.ChangeState<AsmrState_Diff>(new AsmrStateParamBase(_asmrParam.Level, _asmrParam.fsm));
            }
        }
    }
}