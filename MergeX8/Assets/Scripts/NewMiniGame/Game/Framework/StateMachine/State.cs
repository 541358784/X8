using DragonU3DSDK;
using UnityEngine;

namespace NewMiniGame.Fsm
{
    public abstract class BaseState
    {
    }

    public abstract class StateData
    {
    }

    public abstract class State<TState> : BaseState where TState : BaseState
    {
        protected StateMachine<TState> stateMachine;
        protected float _elapsedTime;

        public void SetStateMachine(StateMachine<TState> stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        public virtual void Enter(StateData param)
        {
            _elapsedTime = 0f;

            DebugUtil.Log($"{GetType()} Enter");
        }

        public virtual void Update()
        {
        }

        public virtual void FixedUpdate()
        {
            _elapsedTime += Time.deltaTime;
        }

        public virtual void Exit()
        {
        }

        public float GetElapsedTime()
        {
            return _elapsedTime;
        }

        protected void ChangeState<U>(StateData param) where U : TState, new()
        {
            stateMachine.SetState<U>(param);
        }
    }
}