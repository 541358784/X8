using Stimulate.Configs;
using Stimulate.FSM_Stimulate;
using Stimulate.Model;
using Stimulate.View;

namespace Stimulate.FSM_Stimulate.States
{
    public class FsmInitSkin : FsmStateBase
    {
        private UIStimulateController _controller;
        public override void OnEnter(params object[] param)
        {
            _controller = (UIStimulateController)param[0];

            TableStimulateNodes ownedConfig = null;
            TableStimulateNodes unLockConfig = null;
            var nodes = StimulateConfigManager.Instance.GetNodes(StimulateModel.Instance._config.levelId);
            foreach (var config in nodes)
            {
                if (ownedConfig == null && StimulateModel.Instance.GetNodeState(config.levelId, config.id) == StimulateModel.NodeState.Owned)
                    ownedConfig = config;
                
                if (unLockConfig == null && StimulateModel.Instance.GetNodeState(config.levelId, config.id) == StimulateModel.NodeState.UnLock)
                    unLockConfig = config;
            }

            if (ownedConfig != null)
            {
                Fsm.ChangeState<FsmChangeSkin>(_controller, ownedConfig);
                return;
            }
            
            if (unLockConfig != null)
            {
                Fsm.ChangeState<FsmSetSkin>(_controller, unLockConfig);
                return;
            }
            
            Fsm.ChangeState<FsmFindSkin>(_controller);
        }
        
        public override void OnExit()
        {
            
        }
    }
}