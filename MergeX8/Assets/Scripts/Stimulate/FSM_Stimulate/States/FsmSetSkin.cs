using Stimulate.Event;
using Stimulate.FSM_Stimulate;
using Stimulate.Model;
using Stimulate.View;
using UnityEngine;

namespace Stimulate.FSM_Stimulate.States
{
    public class FsmSetSkin : FsmStateBase
    {
        private UIStimulateController _controller;
        private TableStimulateNodes _config;
        
        public override void OnEnter(params object[] param)
        {
            _controller = (UIStimulateController)param[0];
            _config = (TableStimulateNodes)param[1];

            if (!_config.screenDefaultAnim.IsEmptyString())
            {
                _controller._animator.Play(_config.screenDefaultAnim, -1, 0);
            }

            StimulateGameLogic.Instance.SetSkin(_controller._skeletonGraphic.Skeleton, _config.id.ToString(), _config.spineDefaultAnims);

            if (!_config.defaultAudio.IsEmptyString())
            {
                StimulateGameLogic.Instance.PlaySound(_config.defaultAudio, true);
            }
            EventDispatcher.Instance.DispatchEventImmediately(ConstEvent.Event_Refresh_Guide);
        }
        
        public override void OnExit()
        {
            
        }
    }
}