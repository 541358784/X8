using DragonU3DSDK;
using MiniGame;

namespace asmr_new
{
    public class AsmrState_VideoFinish : AsmrState_ProcessBase
    {
        private TableAsmrStepNew _stepNew;

        public override void OnEnter(FsmStateParamBase stateParam)
        {
            base.OnEnter(stateParam);

            _stepNew = _asmrParam.RuningStep.Config;
            _asmrParam.Level.VideoHandler.Play();
            PlayAudio();
        }

        public override void Update()
        {
            base.Update();

            if (_asmrParam.Level.VideoHandler.IsEnded() ||
                _asmrParam.Level.VideoHandler.Time >= _stepNew.requireValue)
            {
                _asmrParam.Level.VideoHandler.Pause();
                _asmrParam.Level.CurrentGroup.FinishStep(_stepNew.id);
                DebugUtil.Log(
                    $"Video time: {_asmrParam.Level.VideoHandler.Time}, Limit time: {_asmrParam.RuningStep.Config.requireValue}");
            }
        }
    }
}