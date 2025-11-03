using Stimulate.Configs;
using Stimulate.Event;
using Stimulate.FSM_Stimulate;
using Stimulate.Model;
using Stimulate.View;
using UnityEngine;

namespace Stimulate.FSM_Stimulate.States
{
    public class FsmFindSkin : FsmStateBase
    {
        private UIStimulateController _controller;
        
        public override void OnEnter(params object[] param)
        {
            _controller = (UIStimulateController)param[0];
            
            var nodes = StimulateConfigManager.Instance.GetNodes(StimulateModel.Instance._config.levelId);
            foreach (var config in nodes)
            {
                if(config.nextNodeId != 0)
                    continue;

                StimulateGameLogic.Instance.SetSkin(_controller._skeletonGraphic.Skeleton, config.id.ToString(), config.spineFinishAnims);
                
                if (!config.screenFinishAnim.IsEmptyString())
                {
                    _controller._animator.Play(config.screenFinishAnim, -1, 0);
                }
                break;
            }
            
            EventDispatcher.Instance.DispatchEventImmediately(ConstEvent.Event_Refresh_Guide);
        }
        
        public override void OnExit()
        {
            
        }
    }
}