using System.Threading.Tasks;
using DragonPlus.Config.Filthy;
using Filthy.Game;
using Filthy.Model;
using Filthy.SubFsm.Base;
using Filthy.View;

namespace Filthy.SubFsm
{
    public class SubFsm_ChangeSkin : FsmBase
    {
        private UIFilthyController _controller;
        private FilthyNodes _config;
        
        public override void OnEnter(params object[] param)
        {
            _controller = (UIFilthyController)param[0];
            _config = (FilthyNodes)param[1];

            AnimLogic();
        }

        private async void AnimLogic()
        {
            UIRoot.Instance.EnableEventSystem = false;
            double waitTime = 0.5f;
            FilthyGameLogic.Instance.SetSkin(_controller._skeletonGraphic.Skeleton, _config.Id.ToString(), _config.SpineDefaultAnims);

            if(_config.StopAudio)
                FilthyGameLogic.Instance.StopSounds();
                
            if (!_config.FinishAudio.IsEmptyString())
            {
                FilthyGameLogic.Instance.PlaySound(_config.FinishAudio);
            }
            
            if (!_config.ScreenChangeAnim.IsEmptyString())
            {
                _controller._animator.Play(_config.ScreenChangeAnim, -1, 0);
                waitTime += CommonUtils.GetAnimTime(_controller._animator, _config.ScreenChangeAnim);
            }
            
            await Task.Delay((int)(waitTime*1000));
            FilthyModel.Instance.FinishNode(_config.LevelId, _config.Id);
            FilthyGameLogic.Instance.SetSkin(_controller._skeletonGraphic.Skeleton, _config.Id.ToString(), _config.SpineFinishAnims);
            
            Fsm.ChangeState<SubFsm_GetReward>(_controller, _config);
            UIRoot.Instance.EnableEventSystem = true;
        }
        
        public override void OnExit()
        {
            
        }
    }
}