using NewMiniGame.Fsm;
using UnityEngine;

namespace Framework.UI.fsm
{
    public class UIStateOpen<T> : UIStateBase where T : UIStateNormal, new()
    {
        protected float _endTime;

        public override void Enter(StateData param)
        {
            base.Enter(param);

            if (_data.view.HasDirector)
            {
                _endTime = _data.view.OpenTimelineTime;
            }
            else
            {
                _endTime = _data.view.PlayOpenAnim();
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (_elapsedTime >= _endTime)
            {
                ChangeState<T>(new UIStateData(_data.view));
            }
        }
    }
}