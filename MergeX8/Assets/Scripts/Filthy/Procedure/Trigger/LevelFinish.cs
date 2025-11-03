namespace Filthy.Procedure
{
    public class LevelFinish: ITrigger
    {
        public bool _isTrigger { get; set; }
        public bool _isFinish { get; set; }
        public string _triggerParam { get; set; }
        public void Init(string param)
        {
            _triggerParam = param;
        }

        public void Trigger(string param)
        {
            if(_isTrigger || _isFinish)
                return;
            
            if(_triggerParam != param)
                return;

            _isTrigger = true;
            _isFinish = true;
        }

        public bool IsTrigger()
        {
            return _isTrigger;
        }

        public bool IsFinish()
        {
            return _isFinish;
        }

        public void Update()
        {
        }
    }
}