using System;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace Stimulate.FSM_Stimulate
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

        public void ChangeState<T>(params object[] param) where T : FsmStateBase, new()
        {
            var newState = new T();
            newState.Fsm = this;

            ChangeState(newState, param);
        }

        private void ChangeState(FsmStateBase newState, params object[] param)
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
            CurrentState?.OnEnter(param);
        }
    }
}