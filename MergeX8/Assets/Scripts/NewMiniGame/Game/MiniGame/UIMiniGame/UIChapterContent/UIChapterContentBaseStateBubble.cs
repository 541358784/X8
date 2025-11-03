using Framework.UI.fsm;
using MiniGame;
using NewMiniGame.Fsm;

namespace Scripts.UI
{
    public class UIChapterContentBaseStateBubble : UIStateBase
    {
        private UIChapterContentBase _ui;

        private bool _bubbleCreated;

        private float _delayCreateTime;

        private bool _nextFrameFinish;

        public override void Enter(StateData param)
        {
            base.Enter(param);

            _ui = _data.view as UIChapterContentBase;

            if (_data.view is UIChapterContentB)
            {
                var minUnFinishedChapter = MiniGameModel.Instance.GetMinUnFinishedChapterId();
                var minLevel = MiniGameModel.Instance.GetMinUnfinishLevelInChapter(minUnFinishedChapter);
                //先注释
                // if (minLevel == 1 && !GuideTool.Instance.IsFinish(1003))
                // {
                //     _delayCreateTime = 3f;
                // }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (!_bubbleCreated)
            {
                if (_elapsedTime >= _delayCreateTime)
                {
                    _bubbleCreated = true;
                    _ui.ShowBubble();
                    _nextFrameFinish = true;
                    return;
                }
            }

            if (_nextFrameFinish)
            {
                Finish();
            }
        }

        private void Finish()
        {
            _ui.TriggerGuide();
            ChangeState<UIStateNormal>(_data);
        }
    }
}