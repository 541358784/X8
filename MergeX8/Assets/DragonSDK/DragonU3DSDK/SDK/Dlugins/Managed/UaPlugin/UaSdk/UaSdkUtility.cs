using System;
using System.Collections;
using System.Collections.Generic;
using DragonU3DSDK;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace DragonPlus.Ad.UA
{
    public static class UaSdkUtility
    {
        private const  string        GameObjectName = "[UaSdkUtility]";
        private static MainBehaviour _mainBehaviour;

        public static void DelayCall(Action action, float delayInSeconds)
        {
            EnsureCreated();

            _mainBehaviour.StartCoroutine(DelayCallInternal(action, delayInSeconds));
        }

        public static Coroutine StartCoroutine(IEnumerator routine)
        {
            if (routine == null)
            {
                return null;
            }

            EnsureCreated();

            return _mainBehaviour.StartCoroutine(routine);
        }

        public static void EnqueueMainThreadTask(Action action)
        {
            EnsureCreated();

            _mainBehaviour.AddMainThreadTask(action);
        }

        public static void AddLooseUpdateListener(UnityAction fun, float inUpdateInterval = 1.0f)
        {
            EnsureCreated();

            _mainBehaviour.AddLooseUpdateListener(fun, inUpdateInterval);
        }

        public static void AddApplicationPauseCallback(Action<bool> callback)
        {
            EnsureCreated();

            _mainBehaviour.AddApplicationPauseCallback(callback);
        }

        private static IEnumerator DelayCallInternal(Action action, float delayInSeconds)
        {
            yield return new WaitForSeconds(delayInSeconds);

            action?.Invoke();
        }

        private static void EnsureCreated()
        {
            if (_mainBehaviour)
            {
                return;
            }

            var gameObject = GameObject.Find(GameObjectName);
            if (gameObject != null)
            {
                _mainBehaviour = gameObject.GetComponent<MainBehaviour>();
                if (_mainBehaviour == null)
                {
                    _mainBehaviour = gameObject.AddComponent<MainBehaviour>();
                }
            }
            else
            {
                gameObject     = new GameObject(GameObjectName);
                _mainBehaviour = gameObject.AddComponent<MainBehaviour>();
            }

            Object.DontDestroyOnLoad(gameObject);

            Application.quitting += Release;
        }

        private static void Release()
        {
            if (_mainBehaviour == null) return;

            _mainBehaviour.Release();

            Object.DestroyImmediate(_mainBehaviour.gameObject);
            _mainBehaviour = null;
        }

        private class MainBehaviour : MonoBehaviour
        {
            private readonly Queue<Action> _actions = new();
            private readonly List<Action<bool>> _applicationPauseCallbacks = new();

            private class UpdateEntity
            {
                public float       UpdateInterval;
                public float       LastUpdate;
                public UnityAction UpdateAction;
            }

            private readonly List<UpdateEntity> _looseUpdateEvent = new();

            public void AddMainThreadTask(Action action)
            {
                _actions.Enqueue(action);
            }

            public void AddApplicationPauseCallback(Action<bool> action)
            {
                _applicationPauseCallbacks.Add(action);
            }

            public void AddLooseUpdateListener(UnityAction fun, float inUpdateInterval = 1.0f)
            {
                _looseUpdateEvent.Add(new UpdateEntity()
                {
                    LastUpdate     = 0.0f,
                    UpdateAction   = fun,
                    UpdateInterval = inUpdateInterval
                });
            }


            public void Release()
            {
                _actions.Clear();
                _looseUpdateEvent.Clear();
            }

            private void Update()
            {
                while (_actions.TryDequeue(out var action))
                {
                    try
                    {
                        action?.Invoke();
                    }
                    catch (Exception e)
                    {
                        DebugUtil.LogError($"MainThreadTask failed: {e.Message}\n{e.StackTrace}");
                    }
                }

                if (_looseUpdateEvent.Count > 0)
                {
                    var time = Time.realtimeSinceStartup;

                    foreach (var entity in _looseUpdateEvent)
                    {
                        if (entity.UpdateInterval + entity.LastUpdate < time)
                        {
                            if (entity.UpdateAction != null)
                            {
                                entity.UpdateAction();
                            }

                            entity.LastUpdate = time;
                        }
                    }
                }
            }

            private void OnApplicationPause(bool pauseStatus)
            {
                foreach (var callback in _applicationPauseCallbacks)
                {
                    try
                    {
                        callback?.Invoke(pauseStatus);
                    }
                    catch (Exception exception)
                    {
                        DebugUtil.LogError($"[UaSdk] OnApplicationPause: {exception.Message}");
                    }
                }
            }
        }
    }
}