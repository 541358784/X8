using System.Collections.Generic;
using DragonU3DSDK;
using UnityEngine;

namespace TMatch
{
    /// <summary>
    /// 各个状态对象
    /// </summary>
    public interface ICustomState
    {
        void OnEnter();

        void OnExit();

        void OnStateChange(string newState);

        void Update();

        void FixedUpdate();

        string Name { get; }

        string HierachicalName { get; }

        string OwnerName { get; }
    }

    /// <summary>
    /// 给状态机标明继承的对象
    /// </summary>
    public interface IHospitalState
    {
    }

    public class CustomFSMState<T> : ICustomState where T : IHospitalState
    {
        protected string name;
        protected T owner;
        protected HierarchicalStateMachine hsm;

        public CustomFSMState(T owner, HierarchicalStateMachine hsm)
        {
            this.owner = owner;
            this.hsm = hsm;
            this.name = GetType().Name;
        }

        public CustomFSMState(T owner, HierarchicalStateMachine hsm, string name)
        {
            this.owner = owner;
            this.hsm = hsm;
            this.name = name;
        }

        public override string ToString()
        {
            return name;
        }

        #region IState Members

        public virtual void OnEnter()
        {
        }

        public virtual void OnExit()
        {
        }

        public virtual void Update()
        {
        }

        public virtual void FixedUpdate()
        {
        }

        public void OnStateChange(string newState)
        {
        }

        public string Name
        {
            get { return name; }
        }

        public string HierachicalName
        {
            get { return Name; }
        }

        public string OwnerName
        {
            get { return owner.GetType().Name; }
        }

        #endregion
    }

    public class HierarchicalStateMachine
    {
        public bool enableDebug;
        public GameObject owner;

        private ICustomState currentState;
        private ICustomState previousState;
        private Dictionary<string, ICustomState> nameDictionary;
        private string pendingStateChange;
        private bool currentOverwriteFlag;

        public System.Action<string, string> StateChanged;

        public ICustomState CurrentState
        {
            get { return currentState; }
        }

        public ICustomState PreviousState
        {
            get { return previousState; }
        }

        public string CurrentStateName
        {
            get { return currentState.HierachicalName; }
        }

        public string PreviousStateName
        {
            get { return previousState.HierachicalName; }
        }

        public HierarchicalStateMachine(bool enableDebug, GameObject owner)
        {
            this.enableDebug = enableDebug;
            this.owner = owner;
            currentOverwriteFlag = true;
        }

        public void Init(List<ICustomState> states, string defaultState)
        {
            // create name dictionary
            nameDictionary = new Dictionary<string, ICustomState>();
            for (int i = 0; i < states.Count; i++)
            {
                nameDictionary.Add(states[i].Name, states[i]);
            }

            // to default state
            DoChangeState(defaultState);
        }

        public void Update()
        {
            // handle change state
            if (pendingStateChange != null)
            {
                DoChangeState(pendingStateChange);

                // clean state
                pendingStateChange = null;
            }

            // reset flag
            currentOverwriteFlag = true;

            // update current state
            currentState.Update();
        }

        public void FixedUpdate()
        {
            currentState?.FixedUpdate();
        }

        private void DoChangeState(string stateName)
        {
            // save previous state
            previousState = currentState;
            string[] names = stateName.Split(new string[] {"::"}, System.StringSplitOptions.RemoveEmptyEntries);

            switch (names.Length)
            {
                case 1:
                {
                    if (currentState != null)
                    {
                        // exit current state
                        currentState.OnExit();
                    }

                    // set new state
                    ICustomState newState = GetStateByName(stateName);
                    if (newState == null)
                    {
                        DebugUtil.LogError($"Invalid state name: {stateName}");
                    }

                    if (enableDebug)
                    {
                        DebugUtil.Log($"{owner} Change state from {currentState} to {newState}");
                    }

                    currentState = newState;

                    // enter new state
                    currentState.OnEnter();
                    break;
                }
                case 2:
                {
                    // composite state
                    string parentStateName = names[0];

                    // find parent state
                    ICustomState parentState = GetStateByName(parentStateName);
                    if (parentState == null)
                    {
                        DebugUtil.LogError($"Invalid state name: {parentStateName}");
                    }

                    // check if parent state is current state
                    if (parentState != currentState)
                    {
                        // exit current state
                        currentState.OnExit();

                        // warning
                        DebugUtil.LogWarning($"Transfer to internal state {currentState.Name}");
                    }

                    // let state to handle it
                    string subStateName = names[1];
                    parentState.OnStateChange(subStateName);
                    break;
                }
                default:
                    DebugUtil.LogError($"{owner} Invalid state name: {stateName}");
                    break;
            }

            if (StateChanged != null)
            {
                StateChanged(previousState.HierachicalName, currentState.HierachicalName);
            }

            DebugUtil.LogWarning($"{currentState.OwnerName}:{currentState.Name}");
        }

        public void CleanPendingState()
        {
            pendingStateChange = null;
            currentOverwriteFlag = true;
        }

        public void ForceChangeState(string stateName)
        {
            DoChangeState(stateName);

            // reset flag
            pendingStateChange = null;
            currentOverwriteFlag = true;
        }

        public void ChangeState(string stateName, bool overwriteFlag = true)
        {
            // check current flag
            if (currentOverwriteFlag)
            {
                // enable overwrite
                if (pendingStateChange != null && enableDebug)
                {
                    DebugUtil.LogWarning($"{owner} ChangeState will replace state {pendingStateChange} with {stateName}");
                }

                pendingStateChange = stateName;
            }
            else
            {
                // disable overwrite
                if (pendingStateChange != null)
                {
                    if (enableDebug)
                    {
                        DebugUtil.LogWarning($"{owner} Reject state changing from {pendingStateChange} to {stateName}");
                    }

                    return;
                }

                pendingStateChange = stateName;
            }

            // save flag
            currentOverwriteFlag = overwriteFlag;
        }

        public ICustomState GetStateByName(string stateName)
        {
            ICustomState state = null;
            nameDictionary.TryGetValue(stateName, out state);
            return state;
        }
    }
}