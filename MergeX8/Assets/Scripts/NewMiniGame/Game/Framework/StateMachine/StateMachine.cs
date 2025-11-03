using System;
using DragonU3DSDK;
using UnityEngine;

namespace NewMiniGame.Fsm
{
    public class StateMachine<TState> where TState : BaseState
    {
        private TState _currentState;
        public TState CurrentState => _currentState;
        public Type LasStateType;
        
        public void SetState<U>(StateData param) where U : TState, new()
        {
            if (_currentState != null)
            {
                LasStateType = _currentState.GetType();

                (_currentState as State<TState>)?.Exit();
            }

            _currentState = new U();
            var t = _currentState as State<TState>;
            if (t != null)
            {
                t.SetStateMachine(this);
                t.Enter(param);
            }
            else
            {
                DebugUtil.LogError("t is null");
            }
        }

        public void Update()
        {
            (_currentState as State<TState>)?.Update();
        }

        public void FixedUpdate()
        {
            (_currentState as State<TState>)?.FixedUpdate();
        }

        public void Release()
        {
            (_currentState as State<TState>)?.Exit();
        }
    }
}