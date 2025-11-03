using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using DragonU3DSDK;
using UnityEngine.U2D;
using UnityEngine.UI;
using DragonU3DSDK.Asset;
using System.Collections;
using System.Text.RegularExpressions;
using FrameworkRoot = UIRoot;

namespace TMatch
{

    public partial class UIRoot : Manager<UIRoot>
    {
        // 记录当前界面最高 到了多少层
        private int _canvasOrder = 500;

        public Canvas mRootCanvas;

        // 所有UI的根节点
        public GameObject mRoot;

        public Camera mUICamera;

        public GameObject mGuideRoot;

        private void Awake()
        {
            mRootCanvas = FrameworkRoot.Instance.mRootCanvas;
            mRoot = FrameworkRoot.Instance.mUIRoot;
            mUICamera = FrameworkRoot.Instance.mUICamera;
            mGuideRoot = FrameworkRoot.Instance.mGuideRoot;
        }

        /// <summary>
        /// Creates the window.
        /// </summary>
        /// <returns>The window.</returns>
        /// <param name="windowName">UI预设名</param>
        /// <param name="type">UI类型</param>
        /// <param name="layer">UI层级</param>
        public UIWindow CreateWindow(string windowName, Type windowType)
        {
            var uiPrefab = LoadPrefab(windowName);
            if (uiPrefab == null)
            {
                DebugUtil.LogError($"{GetType()}.CreateWindow, cannot find window resource : {windowName}");
                return null;
            }

            var obj = GameObject.Instantiate(uiPrefab, mRoot.transform, false);
            var window = obj.AddComponent(windowType) as UIWindow;
            if (window == null)
            {
                Debug.LogErrorFormat("Cant find UIWindow: {0}, check the name or remove any outter namespace.",
                    windowType.ToString());
                return null;
            }

            // OpUtils.ReleaseRes(windowPath, uiPrefab);

            SetCanvasOrder(obj);
            return window;
        }

        private static GameObject LoadPrefab(string windowName)
        {
            var prefab = loadPrefab(windowName);
            if (prefab != null)
                return prefab;

            return null;
        }

        private static GameObject loadPrefab(string prefabPath)
        {
             GameObject uiPrefab = null;
             // Pad优先加载PadUI
             if (global::CommonUtils.IsLE_16_10())
                 uiPrefab = ResourcesManager.Instance.LoadResource<GameObject>($"{prefabPath}_Pad");
             // 加载普通UI
             if (uiPrefab == null)
                 uiPrefab = ResourcesManager.Instance.LoadResource<GameObject>(prefabPath);
             return uiPrefab;
             // return ResourcesManager.Instance.LoadResource<GameObject>(prefabPath);
        }

        public void SetCanvasOrder(GameObject obj)
        {
            var canvas = obj.GetComponentsInChildren<Canvas>(true);
            for (int i = 0, count = canvas.Length; i < count; i++)
            {
                var c = canvas[i];
                //c.overrideSorting = true;
                c.sortingOrder = c.sortingOrder + _canvasOrder;
            }

            _canvasOrder += 10;
            //mCanvasOrder = obj.layer;
        }

        public Vector2 GetScreenCanvasScale()
        {
            var rectTransform = mRoot.GetComponent<RectTransform>();
            var screenSize = new Vector2(Screen.width, Screen.height);
            return new Vector2(screenSize.x / rectTransform.rect.width, screenSize.y / rectTransform.rect.height);
        }
    }

}