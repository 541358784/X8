using DragonPlus.Config.Filthy;
using Filthy.Event;
using Filthy.Game;
using Filthy.Model;
using Filthy.SubFsm.Base;
using Filthy.View;

namespace Filthy.SubFsm
{
    public class SubFsm_FindSkin : FsmBase
    {
        private UIFilthyController _controller;
        
        public override void OnEnter(params object[] param)
        {
            _controller = (UIFilthyController)param[0];
            
            var nodes = FilthyConfigManager.Instance.FilthyNodesList.FindAll(a=>a.LevelId == FilthyModel.Instance._config.LevelId);
            foreach (var config in nodes)
            {
                if(config.NextNodeId != 0)
                    continue;

                FilthyGameLogic.Instance.SetSkin(_controller._skeletonGraphic.Skeleton, config.Id.ToString(), config.SpineFinishAnims);
                
                if (!config.ScreenFinishAnim.IsEmptyString())
                {
                    _controller._animator.Play(config.ScreenFinishAnim, -1, 0);
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