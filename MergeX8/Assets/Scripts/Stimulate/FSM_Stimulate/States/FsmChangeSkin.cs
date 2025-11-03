using System;
using System.Threading.Tasks;
using Stimulate.FSM_Stimulate;
using Stimulate.Model;
using Stimulate.View;
using UnityEngine;

namespace Stimulate.FSM_Stimulate.States
{
    public class FsmChangeSkin : FsmStateBase
    {
        private UIStimulateController _controller;
        private TableStimulateNodes _config;
        
        public override void OnEnter(params object[] param)
        {
            _controller = (UIStimulateController)param[0];
            _config = (TableStimulateNodes)param[1];

            AnimLogic();
        }

        private async void AnimLogic()
        {
            UIRoot.Instance.EnableEventSystem = false;
            double waitTime = 0.5f;
            StimulateGameLogic.Instance.SetSkin(_controller._skeletonGraphic.Skeleton, _config.id.ToString(), _config.spineDefaultAnims);

            if(_config.stopAudio)
                StimulateGameLogic.Instance.StopSounds();
                
            if (!_config.finishAudio.IsEmptyString())
            {
                StimulateGameLogic.Instance.PlaySound(_config.finishAudio);
            }
            
            if (!_config.screenChangeAnim.IsEmptyString())
            {
                _controller._animator.Play(_config.screenChangeAnim, -1, 0);
                waitTime += CommonUtils.GetAnimTime(_controller._animator, _config.screenChangeAnim);
            }
            
            await Task.Delay((int)(waitTime*1000));
            StimulateModel.Instance.FinishNode(_config.levelId, _config.id);
            StimulateGameLogic.Instance.SetSkin(_controller._skeletonGraphic.Skeleton, _config.id.ToString(), _config.spineFinishAnims);
            
            Fsm.ChangeState<FsmTryGetReward>(_controller, _config);
            UIRoot.Instance.EnableEventSystem = true;
        }
        
        public override void OnExit()
        {
            
        }
    }
}