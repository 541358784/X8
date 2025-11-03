
using NewMiniGame.Fsm;

namespace Framework.UI.fsm
{
    public class UIStateNormal : UIStateBase
    {
        public override void Enter(StateData param)
        {
            base.Enter(param);

            _data.view.OnOpenFinish();

            UIManager.IsOpening = true;
        }
    }
}