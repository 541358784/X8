using DragonU3DSDK;

namespace NewMiniGame.Fsm
{
    public abstract class BaseMultiState
    {
    }

    public abstract class MultiStateData
    {
    }

    public abstract class MultiState<TState> : BaseMultiState where TState : BaseMultiState
    {
        protected MultiStateMachine<TState> stateMachine;
        protected float _elapsedTime;

        public void SetStateMachine(MultiStateMachine<TState> stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        public virtual void Enter(MultiStateData param)
        {
            _elapsedTime = 0f;

            DebugUtil.Log($"{GetType()} Enter");
        }

        public virtual void Update()
        {
        }

        public virtual void FixedUpdate(float deltaTime)
        {
            _elapsedTime += deltaTime;
        }

        public virtual void Exit()
        {
        }

        protected void FinishState()
        {
            stateMachine.RemoveState(this as TState);
        }

        public float GetElapsedTime()
        {
            return _elapsedTime;
        }

        protected void AddState<U>(MultiStateData param) where U : TState, new()
        {
            stateMachine.AddState<U>(param);
        }
    }
}