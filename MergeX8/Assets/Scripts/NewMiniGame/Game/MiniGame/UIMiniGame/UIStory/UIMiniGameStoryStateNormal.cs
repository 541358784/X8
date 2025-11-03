using Framework.UI.fsm;
using Framework.Utils;
using MiniGame;
using NewMiniGame.Fsm;
using UnityEngine;

namespace Scripts.UI
{
    public class UIMiniGameStoryStateNormal : UIStateNormal
    {
        private UIMiniGameStory _story;
        private double _showBtnTime;
        private double _endingTime;

        public override void Enter(StateData param)
        {
            base.Enter(param);

            _story = _data.view as UIMiniGameStory;
            _showBtnTime = MiniGameUtil.GetEndTimeWithSignal(_story?.Director);

            _story?.SetState(this);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            switch (_endingTime)
            {
                case < 0:
                    return;
                case > 0:
                    _endingTime -= Time.deltaTime;
                    break;
            }

            if (_endingTime < 0)
            {
                EventBus.Send<EventMiniGameStoryFinished>();
                return;
            }

            if (_showBtnTime <= 0)
            {
                return;
            }

            _showBtnTime -= Time.deltaTime;
            if (_showBtnTime < 0)
            {
                _story.ShowButton(true);
            }
        }

        public void SetEndTime(double time)
        {
            _endingTime = time;
        }
    }
}