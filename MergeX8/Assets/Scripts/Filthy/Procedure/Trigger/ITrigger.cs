namespace Filthy.Procedure
{
    public interface ITrigger
    {
        public bool _isTrigger { get; set; }
        public bool _isFinish{ get; set; }
        public string _triggerParam{ get; set; }
        
        public void Init(string param);

        public void Trigger(string param);
        public bool IsTrigger();
        public bool IsFinish();
        public void Update();
    }
}