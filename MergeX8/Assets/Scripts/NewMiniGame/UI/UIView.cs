using System;
using DragonPlus;
using DragonU3DSDK.Asset;
using Framework.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UI
{
    public abstract class UIView : UIElement
    {
        protected virtual bool _fitIphoneX    => false;
        protected virtual bool _playOpenSound => true;
        protected virtual bool _playCloseSound => true;

        private static AudioClip _commonOpenSound;
        private static AudioClip _commonCloseSound;
        private        Type      _type;

        private const string Sound_common_popup_open = "Audio/Common/Sound_common_popup_open";
        private const string sound_common_popup_close = "Audio/Common/Sound_common_popup_close";


        protected override void OnCreate()
        {
            base.OnCreate();
            _type = GetType();

            if (_fitIphoneX) FitIphoneX();

            BindRes(true);
        }

        protected internal override void OnOpen<M, T>(UIData data)
        {
            base.OnOpen<M, T>(data);
        }

        protected internal override void OnOpen<T>(UIData data)
        {
            base.OnOpen<T>(data);

            if (_playOpenSound) PlayOpenSound();

            EventBus.Register<EVENT_UI_CLOSE>(OnEventUIClose);
            EventBus.Register<EVENT_UI_CLOSE_ALL_VIEW>(OnEVENT_UI_CLOSE_ALL_VIEW);
        }


        protected override void OnClose()
        {
            base.OnClose();
            
            if(_playCloseSound) PlayCloseSound();

            EventBus.UnRegister<EVENT_UI_CLOSE>(OnEventUIClose);
            EventBus.UnRegister<EVENT_UI_CLOSE_ALL_VIEW>(OnEVENT_UI_CLOSE_ALL_VIEW);
        }

        protected internal override void OnRemove()
        {
            base.OnRemove();

            BindRes(false);
        }

        protected virtual void PlayOpenSound()
        {
            if (_commonOpenSound == null)
                _commonOpenSound = ResourcesManager.Instance.LoadResource<AudioClip>(Sound_common_popup_open);
            AudioManager.Instance.PlaySound(_commonOpenSound);
        }

        protected virtual void PlayCloseSound()
        {
            if (_commonCloseSound == null)
                _commonCloseSound = ResourcesManager.Instance.LoadResource<AudioClip>(sound_common_popup_close);
            AudioManager.Instance.PlaySound(_commonCloseSound);
        }

        private void BindRes(bool bind)
        {
            // if (bind)
            // {
            //     ResTool.Instance.BindBundle(BundleDependences);
            //     ResTool.Instance.BindAtlas(AtlasDependences);
            // }
            // else
            // {
            //     ResTool.Instance.UnBindBundle(BundleDependences);
            //     ResTool.Instance.UnBindAtlas(AtlasDependences);
            // }
        }

        private void OnEventUIClose(EVENT_UI_CLOSE e)
        {
            if (e.t != _type) return;

            OnClose();
        }


        private void OnEVENT_UI_CLOSE_ALL_VIEW(EVENT_UI_CLOSE_ALL_VIEW obj)
        {
            Close();
        }


        public virtual void Close()
        {
            // OnEventUIClose(new EVENT_UI_CLOSE(_type));
            EventBus.Send(new EVENT_UI_CLOSE(_type));
        }

        public virtual RectTransform IphoneXRoot()
        {
            return _transform as RectTransform;
        }

        private void FitIphoneX()
        {
            var root = IphoneXRoot();
            // var offsetMax = root!.offsetMax;
            // offsetMax.y += GetSafeAreaOffset();
            // root.offsetMax = offsetMax;

            var safeArea = Screen.safeArea;
            var screenHeight = Screen.height;
            var topOffset = screenHeight - safeArea.yMax - safeArea.yMin;
            var offsetMax = root.offsetMax;

            if (topOffset != 0)
            {
                offsetMax.y += GetSafeAreaOffset(); // offsetMax.y 应为负值
                root.offsetMax = offsetMax;
            }
        }

        public float GetSafeAreaOffset()
        {
            int safeAreaOffset = (int)(Screen.height - Screen.safeArea.yMax);
            if (safeAreaOffset == 0) return 0.0f;

            safeAreaOffset = safeAreaOffset / 2;
            float scaleRatio = UIManager.Instance._canvasScaler.referenceResolution.y / Screen.height;
            safeAreaOffset = (int)(safeAreaOffset * scaleRatio);
            safeAreaOffset += 5;

// #if UNITY_EDITOR
//             safeAreaOffset = -(int)(Screen.currentResolution.height - Screen.safeArea.yMax);
// #endif

            return -safeAreaOffset;
        }
    }
}