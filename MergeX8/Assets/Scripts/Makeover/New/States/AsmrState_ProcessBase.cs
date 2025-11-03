using ASMR;

namespace MiniGame
{
    public class AsmrState_ProcessBase : AsmrState_Base
    {
        private bool _audioStarting;
        private bool _audioStarting2;
        protected float _progress;


        public override void OnExit()
        {
            base.OnExit();
            
            DragonU3DSDK.Audio.AudioManager.StopAllSounds();
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
                        if (_asmrParam.RuningStep.Config.sound != null) Model.Instance.PlaySound(_asmrParam.RuningStep.Config.sound[0], true);
                    }
                    else
                    {
                        DragonU3DSDK.Audio.AudioManager.ResumeAllSounds();
                    }
                }

                if (!_audioStarting2 && _asmrParam.RuningStep.Config.sound.Length > 1)
                {
                    if (_progress > _asmrParam.RuningStep.Config.soundDelay[1])
                    {
                        _audioStarting2 = true;
                        Model.Instance.PlaySound(_asmrParam.RuningStep.Config.sound[1]);
                    }
                }
            }
            else
            {
                if (!_audioStarting2) DragonU3DSDK.Audio.AudioManager.PauseAllSounds();
            }
        }
        protected void PlayAudio()
        {
            if (_asmrParam.RuningStep.Config.sound != null)
                ASMR.Model.Instance.PlaySound(_asmrParam.RuningStep.Config.sound[0]);
        }
    }
}