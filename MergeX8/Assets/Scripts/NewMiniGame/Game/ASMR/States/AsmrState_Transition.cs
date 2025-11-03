using ASMR;
using Framework;
using Framework.UI;
using UnityEngine;

namespace fsm_new
{
    public class AsmrState_Transition : AsmrState_Base
    {
        private float _uiAnimTime;
        private Vector3 _cameraInitPos;

        public override void OnEnter(FsmStateParamBase stateParam)
        {
            base.OnEnter(stateParam);

            UIASMRTransition.Open();

             var ui = Framework.UI.UIManager.Instance.GetView<UIASMRTransition>();
             _uiAnimTime = ui.GetAnimTime("01");

            ASMR.ASMRModel.Instance.PlaySound("yx_common_sweep");
        }

        public override void OnExit()
        {
            base.OnExit();
            
        }

        private bool _resetAll;

        public override void FixedUpdate(float deltaTime)
        {
            base.FixedUpdate(deltaTime);

            var uiT = _asmrParam.ElapsedTime / _uiAnimTime;
            

            if (uiT >= 1f)
            {
                _asmrParam.Level.CurrentGroup.FinishStep(_asmrParam.RuningStep.Config.Id);
            }
        }
    }
}