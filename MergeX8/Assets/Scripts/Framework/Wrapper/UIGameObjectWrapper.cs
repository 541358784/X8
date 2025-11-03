using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Framework.Wrapper
{
    public class UIGameObjectWrapper : GameObjectWrapper
    {
        public UIGameObjectWrapper(GameObject go) : base(go)
        {
        }

        public GameObject TryGetItem(string key)
        {
            if (key == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(key))
            {
                return _gameObject;
            }

            var obj = _gameObject.transform.Find(key)?.gameObject;
            return obj;
        }

        public T TryGetItem<T>(string key) where T : Component
        {
            var child = TryGetItem(key);
            return TryGetItem<T>(child);
        }

        public T TryGetItem<T>() where T : Component
        {
            return TryGetItem<T>(_gameObject);
        }

        public T TryGetItem<T>(GameObject go) where T : Component
        {
            return go?.GetComponent<T>();
        }

        public GameObject GetItem(string key)
        {
            var obj = TryGetItem(key);
            if (obj == null)
            {
                ; //DragonU3DSDK.DebugUtil.LogError("GetItem failed, window controller name : {0},  key = {1}", GetType().ToString(), key);
            }

            return obj;
        }

        public T GetItem<T>(string key) where T : Component
        {
            var child = GetItem(key);
            return GetItem<T>(child);
        }

        public T GetItem<T>() where T : Component
        {
            return GetItem<T>(_gameObject);
        }

        public T GetItem<T>(GameObject go) where T : Component
        {
            if (go != null)
            {
                var com = TryGetItem<T>(go);
                if (com == null)
                {
                    DragonU3DSDK.DebugUtil.LogError(
                        "GetItem failed, window controller name : {0},  game object name = {1}, Component type:{2}",
                        GetType().ToString(), go.name, typeof(T).ToString());
                }

                return com;
            }

            return default(T);
        }

        public Button BindButton(string key, UnityAction action)
        {
            Button button = GetItem<Button>(key);
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(action);
                return button;
            }
            else
            {
                DragonU3DSDK.DebugUtil.LogError("BindButton failed, button key = {0}", key);
            }

            return null;
        }

        public Button BindButton(UnityAction action)
        {
            Button button = GetItem<Button>();
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(action);
                return button;
            }
            else
            {
                DragonU3DSDK.DebugUtil.LogError($"BindButton failed. game object  = {_gameObject.name}");
            }

            return null;
        }
    }
}