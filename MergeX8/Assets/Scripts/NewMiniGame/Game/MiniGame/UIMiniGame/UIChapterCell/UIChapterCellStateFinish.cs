using Framework.UI.fsm;
using NewMiniGame.Fsm;

namespace Scripts.UI
{
    public class UIChapterCellStateFinish : UIStateNormal
    {
        private UIChapterCell _ui;

        public override void Enter(StateData param)
        {
            base.Enter(param);

            _ui = _data.view as UIChapterCell;

            _ui.SetFinish();
        }
    }
}