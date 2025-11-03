using ASMR;
using Framework;
using Framework.UI;
using UnityEngine;
using System;
namespace fsm_new
{
    public class AsmrState_TransitionOver : AsmrState_Base
    {
        private float _uiAnimTime;
        private Vector3 _cameraInitPos;

        public override void OnEnter(FsmStateParamBase stateParam)
        {
            base.OnEnter(stateParam);
            
             var ui = Framework.UI.UIManager.Instance.GetView<UIASMRTransition>();
             ui.PlayAnim("02");
             _uiAnimTime = ui.GetAnimTime("02");

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
                switch ((AsmrTypes)_asmrParam.RuningStep.Config.ActionType)
                {
                    case AsmrTypes.Click:
                        if (!string.IsNullOrEmpty(_asmrParam.Level.Config.VideoName)) _asmrParam.fsm.ChangeState<AsmrState_VideoPlayer>(new AsmrStateParamBase(_asmrParam.Level, _asmrParam.RuningStep, _asmrParam.fsm));
                        else _asmrParam.fsm.ChangeState<AsmrState_Click>(new AsmrStateParamBase(_asmrParam.Level, _asmrParam.RuningStep, _asmrParam.fsm));
                        break;
                    case AsmrTypes.Drag:
                        _asmrParam.fsm.ChangeState<AsmrState_Drag>(new AsmrStateParamBase(_asmrParam.Level, _asmrParam.RuningStep, _asmrParam.fsm));
                        break;
                    case AsmrTypes.Swipe_Single:
                        _asmrParam.fsm.ChangeState<AsmrState_Swipe_Single>(new AsmrStateParamBase(_asmrParam.Level, _asmrParam.RuningStep, _asmrParam.fsm));
                        break;
                    case AsmrTypes.Swipe_Double:
                        _asmrParam.fsm.ChangeState<AsmrState_Swipe_Double>(new AsmrStateParamBase(_asmrParam.Level, _asmrParam.RuningStep, _asmrParam.fsm));
                        break;
                    case AsmrTypes.LongPress_Double:
                        _asmrParam.fsm.ChangeState<AsmrState_LongPress_Double>(new AsmrStateParamBase(_asmrParam.Level, _asmrParam.RuningStep, _asmrParam.fsm));
                        break;
                    case AsmrTypes.Erase:
                        _asmrParam.fsm.ChangeState<AsmrState_Erase>(new AsmrStateParamBase(_asmrParam.Level, _asmrParam.RuningStep, _asmrParam.fsm));
                        break;
                    case AsmrTypes.Paint:
                        _asmrParam.fsm.ChangeState<AsmrState_Fill>(new AsmrStateParamBase(_asmrParam.Level, _asmrParam.RuningStep, _asmrParam.fsm));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                Framework.UI.UIManager.Instance.Close<UIASMRTransition>();
            }
        }
    }
}