using NewMiniGame.Fsm;

namespace Framework.UI.fsm
{
    public class UIStateClose : UIStateBase
    {
        private float _endTime;

        private bool _removed;

        public override void Enter(StateData param)
        {
            base.Enter(param);

            _endTime = _data.view.PlayCloseAnim();

            if (_endTime == 0) Close();
        }

        public override void Exit()
        {
            base.Exit();

            Close();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (_removed) return;

            if (_elapsedTime >= _endTime)
            {
                Close();
            }
        }

        private void Close()
        {
            _removed = true;

            _data.view.OnRemove();

            UIManager.Instance.ClearUI(_data.view.GetType());
        }
    }
}