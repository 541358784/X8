using System;
using System.Collections.Generic;
using DragonU3DSDK.Asset;
using Framework.Utils;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Framework.UI
{
    public class UIManager
    {
        private UIManager()
        {
        }


        public static bool IsOpening;

        public static UIManager Instance { get; } = new();

        private Dictionary<Type, UIElement> _viewDic;
        private List<UIElement> _viewList;

         public Camera Camera;

        public CanvasScaler _canvasScaler;
        
        public void Init()
        {
            Camera = UIRoot.Instance.mUICamera;
            _viewDic = new();
            _viewList = new();
            _canvasScaler = UIRoot.Instance.mRootCanvas.GetComponent<CanvasScaler>();
        }

        public T Open<T>(string filePath, UIData data = null, UIWindowLayer layer = UIWindowLayer.Normal) where T : UIView, new()
        {
            var window = global::UIManager.Instance.OpenUI(filePath, windowType:UIWindowType.Normal, layer, typeof(T), false, data);

            var obj = window.gameObject;
            CacheViews(((T)window));

            ((T)window).Create(obj, data);

            return ((T)window);
        }

        public void Close<T>() where T : UIView
        {
            EventBus.Send(new EVENT_UI_CLOSE(typeof(T)));
        }

        public void Close(UIView view)
        {
            EventBus.Send(new EVENT_UI_CLOSE(view.GetType()));
        }

        private void CacheViews<T>(T view) where T : UIElement, new()
        {
            _viewDic.Add(typeof(T), view);
            _viewList.Add(view);
        }

        public void ClearUI(Type viewType)
        {
            if (_viewDic.TryGetValue(viewType, out var view))
            {
                _viewDic.Remove(viewType);
                _viewList.Remove(view);

                global::UIManager.Instance.CloseUI(view.windowPath, true);
            }
        }

        public T GetView<T>() where T : UIView
        {
            _viewDic.TryGetValue(typeof(T), out var view);

            return view as T;
        }

        public void FixedUpdate()
        {
            for (var index = 0; index < _viewList.Count; index++)
            {
                var uiView = _viewList[index];
                uiView.FixedUpdate();
            }
        }

        public bool HasPopup(UIPopupView ignore)
        {
            foreach (var uiElement in _viewList)
            {
                if (uiElement == ignore) continue;

                if (uiElement is UIPopupView) return true;
            }

            return false;
        }

        public bool HasView(UIView ignore)
        {
            foreach (var uiElement in _viewList)
            {
                if (uiElement == ignore)
                    continue;

                if (uiElement.GameObject.name == "DebugMenu")
                    continue;

                if (uiElement is UIView) return
                        true;
            }

            return false;
        }

        public void SetUIIndex<T>(int index) where T : UIView
        {
            var ui = UIManager.Instance.GetView<T>();
            if (ui != null)
            {
                ui.Transform.SetSiblingIndex(index);
            }
        }
    }
}