

using DragonPlus;

namespace fsm_new
{
    public class AsmrState_ProcessBase : AsmrState_Base
    {
        private bool _audioStarting;
        private bool _audioStarting2;
        protected float _progress;


        public override void OnExit()
        {
            base.OnExit();

            AudioManager.Instance.StopAllSound();
        }

        protected void CheckAudio()
        {
            if (!AsmrInputHandler_Base.TouchEnd)
            {
                if (!_audioStarting2)
                {
                    if (!_audioStarting)
                    {
                        _audioStarting = true;
                        if (_asmrParam.RuningStep.Config.Sound != null) ASMR.ASMRModel.Instance.PlaySound(_asmrParam.RuningStep.Config.Sound[0], true);
                    }
                    else
                    {
                        AudioManager.Instance.ResumeAllSounds();
                    }
                }

                if (_audioStarting2 || _asmrParam.RuningStep.Config.Sound == null || _asmrParam.RuningStep.Config.Sound.Count <= 1) return;
                if (_progress > _asmrParam.RuningStep.Config.SoundDelay[1])
                {
                    _audioStarting2 = true;
                    ASMR.ASMRModel.Instance.PlaySound(_asmrParam.RuningStep.Config.Sound[1]);
                }
            }
            else
            {
                if (!_audioStarting2) AudioManager.Instance.PauseAllSounds();
            }
        }
    }
}