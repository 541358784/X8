using UnityEngine;

namespace Framework
{
    public abstract class FsmStateBase
    {
        public Fsm Fsm;
        public FsmStateParamBase Param { get; set; }

        public virtual void OnEnter(FsmStateParamBase stateParam)
        {
            Param = stateParam;
            Param.ElapsedTime = 0;

            AddEvent();
            
            Debug.Log($"{ToString()} OnEnter");
        }

        public virtual void OnExit()
        {
            Debug.Log($"{ToString()} OnExit");

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
            if (Param == null)
                return $"{GetHashCode()}:{GetType()}";
            
            return $"{GetHashCode()}:{GetType()}:{Param.ElapsedTime}";
        }
    }
}