using DragonPlus.Config.Filthy;
using Filthy.Model;
using Filthy.SubFsm.Base;
using Filthy.View;

namespace Filthy.SubFsm
{
    public class SubFsm_InitSkin : FsmBase
    {
        private UIFilthyController _controller;

        public override void OnEnter(params object[] param)
        {
            _controller = (UIFilthyController)param[0];

            FilthyNodes ownedConfig = null;
            FilthyNodes unLockConfig = null;
            var nodes = FilthyConfigManager.Instance.FilthyNodesList.FindAll(a=>a.LevelId == FilthyModel.Instance.LevelId());
            foreach (var config in nodes)
            {
                if (ownedConfig == null && FilthyModel.Instance.GetNodeState(config.LevelId, config.Id) == FilthyModel.NodeState.Owned)
                    ownedConfig = config;

                if (unLockConfig == null && FilthyModel.Instance.GetNodeState(config.LevelId, config.Id) == FilthyModel.NodeState.UnLock)
                    unLockConfig = config;
            }

            if (ownedConfig != null)
            {
                Fsm.ChangeState<SubFsm_ChangeSkin>(_controller, ownedConfig);
                return;
            }

            if (unLockConfig != null)
            {
                Fsm.ChangeState<SubFsm_SetSkin>(_controller, unLockConfig);
                return;
            }

            Fsm.ChangeState<SubFsm_FindSkin>(_controller);
        }

        public override void OnExit()
        {

        }
    }
}