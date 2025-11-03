using Framework.UI.fsm;
using NewMiniGame.Fsm;

namespace Scripts.UI
{
    public class UIChapterStateSelect : UIStateNormal
    {
        private UIChapter _ui;

        public override void Enter(StateData param)
        {
            base.Enter(param);

            _ui = _data.view as UIChapter;
            
            AnimControlManager.Instance.AnimShow(AnimKey.Main_ResBar, false, true);
            _ui.TopGroup.Show(false);
            _ui.SelectGroup?.Show(true);

            ChangeState<UIChapterStateNormal>(_data);
        }
    }
}