using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Framework
{
    public class SubSystemManager : IUpdatable, IInitable, IStart, IOnApplicationPause, ILateUpdatable
    {
        private class Group
        {
            public string name;
            public List<SubSystem> subSystems = new List<SubSystem>(16);
        }

        private const int defaultGroupCapacity = 4;
        private List<Group> _groups = new List<Group>(defaultGroupCapacity);
        private Dictionary<string, Group> _groupDict = new Dictionary<string, Group>(defaultGroupCapacity);
        private Group _currentGroup;

        private List<IUpdatable> _updateList = new List<IUpdatable>(4);
        private List<ILateUpdatable> _lateUpdateList = new List<ILateUpdatable>(4);
        private List<IOnApplicationPause> _appPauseList = new List<IOnApplicationPause>(4);


        public void Init()
        {
        }

        public void Start()
        {
            _ForeachSubSystem(_CallStart);
        }


        public void Update(float deltaTime)
        {
            foreach (var updatable in _updateList)
            {
                updatable.Update(deltaTime);
            }
        }

        public void OnApplicationPause(bool pauseStatus)
        {
            foreach (var appPause in _appPauseList)
            {
                appPause.OnApplicationPause(pauseStatus);
            }
        }


        public void Release()
        {
            _ForeachSubSystem(_CallRelease, true);
            _updateList.Clear();
            _lateUpdateList.Clear();
            _groups.Clear();
            _groupDict.Clear();
        }

        public bool AddGroup(string name)
        {
            if (_groupDict.ContainsKey(name))
            {
                DragonU3DSDK.DebugUtil.LogError(
                    "SubSystemManager AddGroup failed, manager has already contains group of name: {0}", name);
                return false;
            }

            var group = new Group();
            group.name = name;
            _groupDict.Add(name, group);
            _groups.Add(group);
            _currentGroup = group;
            return true;
        }

        public T AddSubSystem<T>(string groupName = null) where T : new()
        {
            Group group = null;
            if (string.IsNullOrEmpty(groupName))
            {
                group = _currentGroup;
            }
            else if (_groupDict.ContainsKey(groupName) || AddGroup(groupName))
            {
                group = _groupDict[groupName];
            }

            if (group != null)
            {
                var subSystem = new T();

                var initable = subSystem as IInitable;
                if (initable != null)
                {
                    initable.Init();
                }

                var globalObj = subSystem as IGlobal;
                if (globalObj != null)
                {
                    globalObj.ToGlobal();
                }

                group.subSystems.Add((subSystem) as Framework.SubSystem);
                var updatable = subSystem as IUpdatable;
                if (updatable != null)
                {
                    _updateList.Add(updatable);
                }

                var lateUpdatable = subSystem as ILateUpdatable;
                if (lateUpdatable != null)
                {
                    _lateUpdateList.Add(lateUpdatable);
                }
                    
                var appPause = subSystem as IOnApplicationPause;
                if (appPause != null)
                {
                    _appPauseList.Add(appPause);
                }

                _currentGroup = group;
                return subSystem;
            }

            return default(T);
        }

        private void _ForeachSubSystem(Action<SubSystem> action, bool backward = false)
        {
            if (backward)
            {
                for (int i = _groups.Count - 1; i >= 0; i--)
                {
                    var group = _groups[i];
                    if (group == null)
                        continue;

                    for (int j = group.subSystems.Count - 1; j >= 0; j--)
                    {
                        var sub = group.subSystems[j];
                        if (sub == null)
                            continue;

                        action(sub);
                    }
                }

                return;
            }

            for (int i = 0; i < _groups.Count; i++)
            {
                var group = _groups[i];
                if (group == null)
                    continue;

                for (int j = 0; j < group.subSystems.Count; j++)
                {
                    var sub = group.subSystems[j];
                    if (sub == null)
                        continue;

                    action(sub);
                }
            }
        }

        private void _CallStart(SubSystem subSystem)
        {
            var sub = subSystem as IStart;
            if (sub != null)
            {
                sub.Start();
            }
        }

        private void _CallRelease(SubSystem subSystem)
        {
            var initable = subSystem as IInitable;
            if (initable != null)
            {
                initable.Release();
            }
        }

        public void LateUpdate(float deltaTime)
        {
            foreach (var updatable in _lateUpdateList)
            {
                updatable.LateUpdate(deltaTime);
            }
        }
    }
}