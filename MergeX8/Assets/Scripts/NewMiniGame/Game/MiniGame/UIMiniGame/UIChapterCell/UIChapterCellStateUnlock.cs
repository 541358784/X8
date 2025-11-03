using Framework.UI.fsm;
using NewMiniGame.Fsm;

namespace Scripts.UI
{
    public class UIChapterCellStateUnlock : UIStateBase
    {
        private UIChapterCell _ui;

        private float _endTime;

        public override void Enter(StateData param)
        {
            base.Enter(param);

            _ui = _data.view as UIChapterCell;

            _endTime = _ui.PlayUnlockAnim();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            
            if (_elapsedTime >= _endTime)
            {
                Finish();
                
                GuideSubSystem.Instance.Trigger(GuideTriggerPosition.MiniGame_Button, UIChapterCell._guideKey + _ui.ChapterId, UIChapterCell._guideKey + _ui.ChapterId);
            }
        }

        private void Finish()
        {
            ChangeState<UIChapterStateNormal>(_data);
        }
    }
}