using System;
using System.Collections.Generic;
using DragonU3DSDK;

namespace NewMiniGame.Fsm
{
    public class MultiStateMachine<TState> where TState : BaseMultiState
    {
        private List<TState> _activeStates = new();
        private List<TState> _removeStates = new();
        public bool Empty => _activeStates.Count == 0;

        public void AddState<U>(MultiStateData param) where U : TState, new()
        {
            var state = new U();
            _activeStates.Add(state);
            var t = state as MultiState<TState>;
            if (t != null)
            {
                t.SetStateMachine(this);
                t.Enter(param);
            }
        }
        
        public bool Contans(Type type)
        {
            foreach (var state in _activeStates)
            {
                if (state.GetType() == type) return true;
            }

            return false;
        }

        public void RemoveState(TState state)
        {
            _removeStates.Add(state);
        }

        public void FixedUpdate(float deltaTime)
        {
            var activeCnt = _activeStates.Count;
            for (var i = 0; i < activeCnt; i++)
            {
                var currActiveState = _activeStates[i];
                (currActiveState as MultiState<TState>)?.FixedUpdate(deltaTime);
            }

            foreach (var state in _removeStates)
            {
                (state as MultiState<TState>)?.Exit();
                _activeStates.Remove(state);
            }

            _removeStates.Clear();
        }
    }
}