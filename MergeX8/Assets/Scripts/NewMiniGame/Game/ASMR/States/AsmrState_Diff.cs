using Framework;

namespace fsm_new
{
    public class AsmrState_Diff : AsmrState_Base
    {
        private float _animTime;
        private string _animName = "finish";

        public override void OnEnter(FsmStateParamBase stateParam)
        {
            base.OnEnter(stateParam);

            _asmrParam.Level.Animator.Play(_animName);
            _animTime = CommonUtils.GetAnimTime(_asmrParam.Level.Animator, _animName);

            ASMR.ASMRModel.Instance.PlayCommonSound("ASMR_change");
        }

        public override void FixedUpdate(float deltaTime)
        {
            base.FixedUpdate(deltaTime);

            _asmrParam.Level.Animator.Update(deltaTime);

            if (_asmrParam.ElapsedTime > _animTime)
            {
                _asmrParam.fsm.ChangeState<AsmrState_Win>(new AsmrStateParamBase(_asmrParam.Level, _asmrParam.fsm));
            }
        }
    }
}