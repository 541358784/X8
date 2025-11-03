using System;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace MiniGame
{
    public class Fsm
    {
        public FsmStateBase CurrentState;

        private bool _pausing;
        
        public Fsm()
        {
            _pausing = false;
        }

        public void Update()
        {
            if (_pausing) return;

            // 执行正常的逻辑或操作
            CurrentState?.Update();
        }

        public void FixedUpdate(float deltaTime)
        {
            if (_pausing) return;

            CurrentState?.FixedUpdate(deltaTime);
        }

        public void Pause(bool pause)
        {
            CurrentState?.OnPause(pause);

            _pausing = pause;
        }

        public void ChangeState<T>(FsmStateParamBase stateParam, Action OnFinish = null) where T : FsmStateBase, new()
        {
            if (stateParam == null) stateParam = new FsmStateParamBase();

            var newState = new T();
            newState.Fsm = this;

            ChangeState(newState, stateParam, OnFinish);
        }

        private void ChangeState(FsmStateBase newState, FsmStateParamBase stateParam, Action OnFinish)
        {
            if (newState == null)
            {
                Debug.LogError($"{GetType()}.ChangeState, new state is null!");
                return;
            }

            if (newState == CurrentState)
            {
                Debug.LogError($"the same as current state!");
                return;
            }


            CurrentState?.OnExit();
            CurrentState = newState;
            CurrentState?.OnEnter(stateParam);
            OnFinish?.Invoke();
        }
    }
}