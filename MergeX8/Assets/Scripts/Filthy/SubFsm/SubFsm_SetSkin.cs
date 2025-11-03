using DragonPlus.Config.Filthy;
using Filthy.Event;
using Filthy.Game;
using Filthy.SubFsm.Base;
using Filthy.View;

namespace Filthy.SubFsm
{
    public class SubFsm_SetSkin : FsmBase
    {
        private UIFilthyController _controller;
        private FilthyNodes _config;
        
        public override void OnEnter(params object[] param)
        {
            _controller = (UIFilthyController)param[0];
            _config = (FilthyNodes)param[1];

            if (!_config.ScreenDefaultAnim.IsEmptyString())
            {
                _controller._animator.Play(_config.ScreenDefaultAnim, -1, 0);
            }

            FilthyGameLogic.Instance.SetSkin(_controller._skeletonGraphic.Skeleton, _config.Id.ToString(), _config.SpineDefaultAnims);

            if (!_config.DefaultAudio.IsEmptyString())
            {
                FilthyGameLogic.Instance.PlaySound(_config.DefaultAudio, true);
            }
            EventDispatcher.Instance.DispatchEventImmediately(ConstEvent.Event_Refresh_Guide);
        }
        
        public override void OnExit()
        {
            
        }
    }
}