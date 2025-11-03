namespace Filthy.SubFsm.Base
{
    public abstract class FsmBase
    {
        public Fsm Fsm;
        public virtual void OnEnter(params object[] param)
        {
        }

        public virtual void OnExit()
        {
        }

        public virtual void OnPause(bool pause)
        {
        }

        public virtual void FixedUpdate(float deltaTime)
        {
        }

        public virtual void Update()
        {
        }
    }
}