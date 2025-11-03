using System;
using UnityEngine;

namespace fsm_new
{
    public class AsmrFsm<T> where T : FsmStateBase
    {
        public FsmStateBase CurrentState;

        private bool _pausing;

        public AsmrFsm()
        {
            _pausing = false;
        }

        public void Release()
        {
            CurrentState?.OnExit();
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

            ChangeState(newState, stateParam, OnFinish);
        }

        private void ChangeState(FsmStateBase newState, FsmStateParamBase stateParam, Action OnFinish)
        {
            if (newState == null)
            {
// #if DEBUG || DEVELOPMENT_BUILD
//                 Debug.LogError($"{GetType()}.ChangeState, new state is null!");
// #endif
                return;
            }

            if (newState == CurrentState)
            {
// #if DEBUG || DEVELOPMENT_BUILD
//                 Debug.LogError($"the same as current state!");
// #endif
                return;
            }


            CurrentState?.OnExit();
            CurrentState = newState;
            CurrentState?.OnEnter(stateParam);
            OnFinish?.Invoke();
        }
    }
}