using System;
using Decoration;
using UnityEngine;
using UnityEngine.UI;

namespace Screw.Module
{
    public class UIModule : Singleton<UIModule>
    {
        public Vector2 DesignSize=>new Vector2(1536, 768);
        
        public float GetSafeAreaOffset()
        {
            int safeAreaOffset = (int) (Screen.height - Screen.safeArea.yMax);
            if (safeAreaOffset == 0) return 0.0f;

            safeAreaOffset = safeAreaOffset / 2;
            float scaleRatio = UIRoot.Instance.mRootCanvas.GetComponent<CanvasScaler>().referenceResolution.y / Screen.height;
            safeAreaOffset = (int) (safeAreaOffset * scaleRatio);
            safeAreaOffset += 30;
            return -safeAreaOffset;
        }
        
        public Camera UICamera
        {
            get
            {
                return UIRoot.Instance.mUICamera;
            }
        }

        public Camera WorldCamera
        {
            get
            {
                return DecoSceneRoot.Instance.mSceneCamera;
            }
        }

        public Transform UiRoot
        {
            get
            {
                return UIRoot.Instance.mRoot.transform;
            }
        }
        public bool EnableEventSystem
        {
            get
            {
                return UIRoot.Instance.EnableEventSystem;
            }
            set
            {
                UIRoot.Instance.EnableEventSystem = value;
            }
        }

        public UIWindow ShowUI(Type type, params object[] objs)
        {
            return UIManager.Instance.OpenUI(type, objs);
        }

        public void CloseWindow(Type type, bool destroy = true)
        {
            UIManager.Instance.CloseUI(type, destroy);
        }  
        public void CloseWindow<T>(bool destroy = true)
        {
            UIManager.Instance.CloseUI(typeof(T), destroy);
        }
    }
}