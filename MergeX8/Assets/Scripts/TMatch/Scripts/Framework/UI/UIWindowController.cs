using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TMatch
{


    public abstract class UIWindowController : UIWindow
    {
        protected GameObject GetItem(string key, GameObject parObj = null)
        {
            if (parObj == null)
            {
                parObj = this.gameObject;
            }

            var obj = FindObj(key, parObj);
            if (obj == null)
            {
                Debug.LogErrorFormat("GetItem failed, window controller name : {0},  key = {1}", GetType().ToString(),
                    key);
            }

            return obj;
        }

        protected bool TryGetItem(string key, out GameObject go, GameObject parObj = null)
        {
            if (parObj == null)
            {
                parObj = this.gameObject;
            }

            var trans = parObj.transform.Find(key);
            go = trans ? trans.gameObject : null;
            return go != null;
        }

        protected T GetItem<T>(string key, GameObject parObj = null)
        {
            var go = GetItem(key, parObj);
            return GetItem<T>(go);
        }

        protected T GetItem<T>(GameObject go)
        {
            if (go != null)
            {
                var com = go.GetComponent<T>();
                if (com == null)
                {
                    Debug.LogErrorFormat(
                        "GetItem failed, window controller name : {0},  game object name = {1}, Component type:{2}",
                        GetType().ToString(), go.name, typeof(T).ToString());
                }

                return com;
            }

            return default(T);
        }

        protected void SetClickListener(string path, UnityAction onClick, bool defaultAudio = true)
        {
            Button btn = GetItem<Button>(path);
            if (btn == null)
            {
                Debug.LogError($"Can't set click listener, {path} not exist!");
                return;
            }

            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() =>
            {
                if (defaultAudio) PlayBtnTapSound();
                onClick?.Invoke();
            });
        }

        protected void SetClickListener(int index, UnityAction onClick, bool defaultAudio = true)
        {
            var btn = widgets.buttons[index];
            if (btn == null)
            {
                Debug.LogError($"Can't set click listener, {index} not exist!");
                return;
            }

            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() =>
            {
                if (defaultAudio) PlayBtnTapSound();
                onClick?.Invoke();
            });
        }
        protected void SetClickListener(Button btn, UnityAction onClick, bool defaultAudio = true)
        {
            if (btn == null)
            {
                Debug.LogError($"Can't set click listener, btn is null");
                return;
            }
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(()=>
            {
                if (defaultAudio) PlayBtnTapSound();
                onClick?.Invoke();
            });
        }

        public Toggle BindValueChange(string key, UnityAction<bool> onValueChanged, GameObject parObj = null)
        {
            var toggle = GetItem<Toggle>(key, parObj);
            if (toggle)
            {
                toggle.onValueChanged.RemoveAllListeners();
                toggle.onValueChanged.AddListener(onValueChanged);
            }

            return toggle;
        }
    }
}
