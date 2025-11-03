using Framework.UI.fsm;
using NewMiniGame.Fsm;

namespace Scripts.UI
{
    public class UIChapterCellStateNormal : UIStateNormal
    {
        private UIChapterCell _view;

        private bool _haveTriggered;
        private float _playGroupUnlockAnimTime;

        private const float PreWaitDelay = 1f;

        public override void Enter(StateData param)
        {
            base.Enter(param);
            _view = (_data.view as UIChapterCell)!;

            var firstUnlock = UIChapter.JustFinishLastChapter && !_haveTriggered;
            _view.SetNormal(firstUnlock);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (_elapsedTime < PreWaitDelay) return;

            if (UIChapter.JustFinishLastChapter && !_haveTriggered)
            {
                _haveTriggered = true;
                _view.PlayUnlockAnimWhenFinishLastChapter();
            }
        }
    }
}