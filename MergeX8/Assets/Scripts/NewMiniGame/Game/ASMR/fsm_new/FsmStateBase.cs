using DragonU3DSDK;
using UnityEngine;

namespace fsm_new
{
    public abstract class FsmStateBase
    {
        public FsmStateParamBase Param { get; set; }

        public virtual void OnEnter(FsmStateParamBase stateParam)
        {
            Param = stateParam;
            Param.ElapsedTime = 0;

            AddEvent();

// #if DEBUG || DEVELOPMENT_BUILD
//             DebugUtil.Log($"{ToString()} OnEnter");
// #endif
        }

        public virtual void OnExit()
        {
// #if DEBUG || DEVELOPMENT_BUILD
//             DebugUtil.Log($"{ToString()} OnExit");
// #endif

            RemoveEvent();
        }

        protected virtual void AddEvent()
        {
        }

        protected virtual void RemoveEvent()
        {
        }

        public virtual void OnPause(bool pause)
        {
        }

        public virtual void FixedUpdate(float deltaTime)
        {
            Param.ElapsedTime += deltaTime;
        }

        public virtual void Update()
        {
        }

        public override string ToString()
        {
            if (Param == null) return $"{GetHashCode()}:{GetType()}";

            return $"{GetType()}:{Param.ElapsedTime}";
        }
    }
}