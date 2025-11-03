using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DragonU3DSDK;
using Framework;
// using GameGuide;
// using Hospital.UI;
using OutsideGuide;
using UnityEngine;

namespace TMatch
{


    public partial class UIManager : Manager<UIManager>
    {
        private Dictionary<Type, UIWindow> _cachedWindows;

        private List<UIWindow> _windowStack;
        private Dictionary<int, LinkedList<UIWindow>> _layerDic;
        private UIWindow _fullScreenWindow;

        public Dictionary<Type, UIWindow> AllUIWindows => _cachedWindows;

        private void Awake()
        {
            DragonU3DSDK.Device.Instance.AddBackButtonCallback(OnBackButtonCallBack);
        }

        private void OnDisable()
        {
            DragonU3DSDK.Device.Instance.RemoveBackButtonCallback(OnBackButtonCallBack);
        }

        private void OnBackButtonCallBack()
        {
            Back();
        }

        protected override void InitImmediately()
        {
            _cachedWindows = new Dictionary<Type, UIWindow>();
            _windowStack = new List<UIWindow>();
            _layerDic = new Dictionary<int, LinkedList<UIWindow>>(16);
        }

        //不再建议使用
        public UIWindow OPEN_WINDOW_WITH_PATH(string uipath, UIWindowData data = null, bool snap = false)
        {
            var dirs = uipath.Split('/');
            var className = $"{dirs[dirs.Length - 1]}Controller";
            var windowType = Type.GetType(className);

            openWindowWithType(windowType, uipath, out var window, data, snap);

            return window;
        }

        public T OpenWindow<T>(string uipath, UIWindowData data = null, bool snap = false, int forceSortingOrder = -1)
            where T : UIWindow
        {
            var windowType = typeof(T);

            openWindowWithType(windowType, uipath, out var window, data, snap);

            if (forceSortingOrder != -1)
            {
                window.Canvas.sortingOrder = forceSortingOrder;
            }

            return (T) window;
        }

        private void openWindowWithType(Type windowType, string uipath, out UIWindow window, UIWindowData data,
            bool snap)
        {
            if (!_cachedWindows.TryGetValue(windowType, out window))
            {
                window = UIRoot.Instance.CreateWindow(uipath, windowType);
                if (window)
                {
                    _cachedWindows[windowType] = window;
                    window.WindowName = uipath;
                }
                else
                {
                    DebugUtil.LogError("Create ui fail: " + uipath);
                }
            }

            openWindow(window, data, snap);
        }

        /// <param name="pushStack">快照模式，关闭后需要恢复原UI状态</param>
        private void openWindow(UIWindow window, UIWindowData data, bool snap)
        {
            if (_windowStack.Contains(window)) // 在打开队列里
            {
                DebugUtil.LogError("同一个UI打开两次，检查逻辑是否有问题");
                return;
            }

            if (window.WindowType == UIWindowType.FullScreen)
            {
                closeAllWindows(window);
                _fullScreenWindow = window;
            }
            else
            {
                if (_fullScreenWindow && !_fullScreenWindow.Pausing && window.WindowLayer != UIWindowLayer.Guide)
                {
                    if (snap)
                    {
                        _fullScreenWindow?.OnSnap(window);
                        _fullScreenWindow.OnSnaping = true;
                    }
                    else
                    {
                        _fullScreenWindow?.OnPause(window);
                    }

                    _fullScreenWindow.Pausing = true;
                }
            }

            _windowStack.Push(window); //添加到打开队列
            layerListInsertWindow(window);

            window.OpenWindow(data);
        }

        IEnumerator lateOnReSume()
        {
            yield return new WaitForEndOfFrame();
            if (_fullScreenWindow && !_fullScreenWindow.Pausing)
            {
                if (_fullScreenWindow.OnSnaping)
                {
                    _fullScreenWindow.OnSnaping = false;
                    _fullScreenWindow.OnRestoreSnap();
                }
                else
                {
                    _fullScreenWindow.OnResume();
                }
            }
        }

        public bool Back()
        {
            // 选档界面屏蔽返回键
            // if (ChooseProgressController.IsOpenWindow)
            // {
            //     Debug.LogError("返回键无效,存档界面不可关闭");
            //     return false;
            // }

            ;

            // 特殊通用弹框界面屏蔽返回键
            if (NoticeController.IsLockSystemBack)
            {
                Debug.LogError("返回键无效,特殊通用弹窗不可关闭");
                return false;
            }

            ;

            //局外引导屏蔽返回键
            if (DecoGuideManager.Instance.IsRunning)
            {
                Debug.LogError("返回键无效,引导中不可关闭");
                return false;
            }

            ;

            //局内引导屏蔽返回键
            // if (HospitalGuideManager.Instance.IsRunning)
            // {
            //     Debug.LogError("返回键无效,引导中不可关闭");
            //     return false;
            // }

            ;

            //UIMask打开屏蔽返回键
            if (GetOpenedWindow<UIMask>() != null)
            {
                Debug.LogError("返回键无效,屏蔽点击过程中不可关闭");
                return false;
            }

            ;

            if (_cachedWindows == null || _cachedWindows.Count <= 0)
            {
                return false;
            }

            UIWindow uiWindow = null;

            var winList = _cachedWindows.Values.ToList();
            winList.Sort((x, y) => x.Canvas.sortingOrder - y.Canvas.sortingOrder);


            foreach (var win in winList)
            {
                if (win == null) continue;

                if (win.WindowType == UIWindowType.Popup /*|| win == UIGame.Get() || win == UIMain.Get() ||
                    win == UIUpgrade.Get()*/)
                    uiWindow = win;

                // if (
                //     _cachedWindows[key] != null 
                //     && _cachedWindows[key].WindowType != UIWindowType.Fixed 
                //     && _cachedWindows[key].WindowType != UIWindowType.FullScreen
                // )
                // {
                //     return _cachedWindows[key].OnBack();
                // }
            }

            return uiWindow != null && uiWindow.OnBack();
        }

        public bool CloseWindow<T>(bool destroy = true) where T : UIWindow
        {
            var windowType = typeof(T);
            return InternalCloseWindow(windowType, destroy);
        }

        public bool CloseWindow(Type type, bool destroy = true)
        {
            return InternalCloseWindow(type, destroy);
        }

        private void layerListInsertWindow(UIWindow window)
        {
            try
            {
                if (window)
                {
                    if (_layerDic.TryGetValue((int) window.WindowLayer, out var windowsWithSameLayer)
                        && windowsWithSameLayer != null
                        && windowsWithSameLayer.Count > 0)
                    {
                        windowsWithSameLayer.Remove(window);
                        windowsWithSameLayer.AddLast(window);

                        // DebugUtil.Log($"{window.windowPath} sibling index is {window.transform.GetSiblingIndex()}");
                    }
                    else
                    {
                        if (windowsWithSameLayer == null)
                        {
                            windowsWithSameLayer = new LinkedList<UIWindow>();
                            _layerDic.Add((int) window.WindowLayer, windowsWithSameLayer);
                        }

                        windowsWithSameLayer.AddLast(window);
                    }

                    var siblingIndex = 500; // 让出loading其他想挂在canvas下的奇怪东西
                    for (int i = 0; i <= (int) UIWindowLayer.Max; i++)
                    {
                        if (_layerDic.TryGetValue(i, out var layerWindows) && layerWindows != null)
                        {
                            if (layerWindows.Count > 0)
                            {
                                using (var e = layerWindows.GetEnumerator())
                                {
                                    while (e.MoveNext())
                                    {
                                        // e.Current?.transform?.SetSiblingIndex(siblingIndex++);
                                        if (siblingIndex < e.Current.JumpOrder) siblingIndex = e.Current.JumpOrder;
                                        else siblingIndex += e.Current.JumpOrder;

                                        string sortingLayerName =
                                            e.Current.UseFixedSort ? e.Current.FixedSortName : "Default";
                                        int sortingOrder = e.Current.UseFixedSort
                                            ? e.Current.FixedSortOrder
                                            : siblingIndex;

                                        e.Current.Canvas.sortingLayerName = sortingLayerName;
                                        e.Current.Canvas.sortingOrder = sortingOrder;
                                    }
                                }
                            }
                        }
                    }

                    var index = window.transform.GetSiblingIndex();
                    // DebugUtil.Log($"{window.windowPath} sibling index is {index}");
                }
            }
            catch (Exception e)
            {
                DebugUtil.LogError(e);
                DebugUtil.LogError($"{GetType()}.LayerListInsertWindow error, window name = {window?.name}");
            }
        }

        public bool InternalCloseWindow(Type windowType, bool destroy = true, bool resumeFullWindow = true)
        {
            var result = false;
            if (_cachedWindows.TryGetValue(windowType, out UIWindow window))
            {
                var needResume = window.WindowLayer != UIWindowLayer.Guide;
                window.Internal_CloseWindow(destroy);
                _windowStack.Remove(window);

                if (_windowStack.Count > 0)
                {
                    if (resumeFullWindow)
                    {
                        var last = _windowStack.StackPeek();
                        if (last != null && last.Equals(_fullScreenWindow))
                        {
                            if (_fullScreenWindow.Pausing)
                            {
                                //恢复回调延迟一帧，根据状态判断是否调用恢复，防止关闭一个弹窗立刻打开另一个弹窗这种连续调用情况
                                if (needResume)
                                {
                                    CoroutineManager.Instance.StartCoroutine(lateOnReSume());
                                    _fullScreenWindow.Pausing = false;
                                }
                            }
                        }
                    }
                }

                if (destroy)
                {
                    LayerListRemoveWindow(window);
                    _cachedWindows.Remove(windowType);
                }

                result = true;
            }

            return result;
        }

        private void closeAllWindows(UIWindow withOutWindow = null)
        {
            var tempRemoveList = new List<UIWindow>();

            foreach (var window in _cachedWindows.Values)
            {
                if (window.Equals(withOutWindow)) continue;
                if (window.WindowLayer == UIWindowLayer.Notice) continue;
                if (window.WindowLayer == UIWindowLayer.Loading) continue;

                window.Internal_CloseWindow(true);

                LayerListRemoveWindow(window);
                tempRemoveList.Add(window);
            }


            foreach (var tempWindow in tempRemoveList)
            {
                _windowStack.Remove(tempWindow);
                _cachedWindows.RemoveByValue(tempWindow);
            }

            tempRemoveList.Clear();
        }

        public void ClearAllWindows()
        {
            closeAllWindows(null);

            _fullScreenWindow = null;
            // TaskSystem.Model.Instance.ResetChokeTaskCount();
        }

        //判断一个ui是否已打开,如果已打开，返回此ui的实例
        public T GetOpenedWindow<T>() where T : UIWindow
        {
            var windowType = typeof(T);

            if (_cachedWindows.ContainsKey(windowType))
            {
                return (T) _cachedWindows[windowType];
            }

            return default(T);
        }

        public bool IsWindowOpened<T>() where T : UIWindow
        {
            return _cachedWindows.ContainsKey(typeof(T));
        }

        public bool IsWindowOpened(Type window)
        {
            return _cachedWindows.ContainsKey(window);
        }

        void LayerListRemoveWindow(UIWindow window)
        {
            if (window)
            {
                if (_layerDic.TryGetValue((int) window.WindowLayer, out var windowsInCurrentLayer))
                {
                    windowsInCurrentLayer?.Remove(window);
                }
            }
        }

        public UIWindow GetOpenedWindowByPath(string uipath)
        {
            foreach (var element in _cachedWindows)
            {
                if (element.Value.WindowName == uipath && element.Value.gameObject.activeSelf)
                {
                    return element.Value;
                }
            }

            return null;

            // if (mMemoryWindows.ContainsKey(uipath))
            // {
            //     var window = mMemoryWindows[uipath];
            //     return window.gameObject.activeSelf ? window : null;
            // }
            //
            // return null;
        }

        public bool HasPopup()
        {
            foreach (var windows in _cachedWindows)
            {
                if (windows.Value as UIPopup)
                    return true;
            }

            return false;
        }
    }
}