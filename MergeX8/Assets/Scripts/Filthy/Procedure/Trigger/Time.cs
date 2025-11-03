using NotImplementedException = System.NotImplementedException;

namespace Filthy.Procedure
{
    public class Time : ITrigger
    {
        public bool _isTrigger { get; set; }
        public bool _isFinish{ get; set; }
        public string _triggerParam{ get; set; }

        private float _triggerTime = 0;
         
        public void Init(string param)
        {
            _triggerParam = param;
            float.TryParse(param, out _triggerTime);
        }

        public void Trigger(string param)
        {
            _isTrigger = true;
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
           if(!_isTrigger)
               return;
           
           if(_isFinish)
               return;
           
           _triggerTime -= UnityEngine.Time.deltaTime;
           
           if(_triggerTime > 0)
               return;

           _isFinish = true;
        }
    }
}