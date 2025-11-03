using NewMiniGame.Fsm;

namespace Framework.UI.fsm
{
    public abstract class UIStateStep : UIStateNormal
    {
        protected abstract float[] _timeSnaps { get; }
        private            bool[]  _switches;

        private int _currentIndex;

        protected abstract void OnStep(int index);

        public override void Enter(StateData param)
        {
            base.Enter(param);

            _switches = new bool[_timeSnaps.Length];
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (_currentIndex >= _switches.Length) return;

            if (!_switches[_currentIndex])
                if (_elapsedTime >= _timeSnaps[_currentIndex])
                {
                    _switches[_currentIndex] = true;
                    OnStep(_currentIndex);
                    _currentIndex++;
                }
        }
    }
}