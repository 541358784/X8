using System;

namespace Framework
{
    public class Timer
    {
        private float _current;
        private float _interval;

        public float Current => _current;

        public float Interval => _interval;
        public float Remain => (_interval - _current);

        public Timer(float interval = Single.MaxValue)
        {
            _interval = interval;
        }

        public bool Update(float deltaTime)
        {
            _current += deltaTime;
            if (_current >= _interval)
            {
                _current = _interval;
                return true;
            }

            return false;
        }

        public bool IsOver()
        {
            return _current >= _interval;
        }

        public float Normalize()
        {
            if (_interval <= 0)
            {
                return 1;
            }
            else
            {
                return _current / _interval;
            }
        }

        public void Reset()
        {
            _current = 0f;
        }

        public void SetInterval(float interval)
        {
            _interval = interval;
        }

        public void SetCurrent(float current)
        {
            _current = current;
        }
    }

    public class AdvancedTimer : Timer
    {
        private float _cycle;
        private float _curr;
        private bool _pause;

        public Action<AdvancedTimer> onCycle;
        public Action<AdvancedTimer> onEnd;

        public AdvancedTimer(float interval, float cycle) : base(interval)
        {
            _cycle = cycle;
        }

        public new bool Update(float deltaTime)
        {
            if (_pause)
            {
                return false;
            }

            var isOver = base.Update(deltaTime);

            _curr += deltaTime;
            if (_curr >= _cycle)
            {
                onCycle?.Invoke(this);
                _curr -= _cycle;
            }

            if (isOver)
            {
                onEnd?.Invoke(this);
                _pause = true;
            }

            return isOver;
        }


        public void Pause()
        {
            _pause = true;
        }

        public void Start()
        {
            _pause = false;
        }
    }
}