using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMatch
{


    public class UIWaitingController : UIWindow
    {
        public class Data : UIWindowData
        {
            public float TimeOut = 30;
            public float Delay;

            public Data(float timeOut, float delay)
            {
                TimeOut = timeOut;
                Delay = delay;
            }
        }

        private float _timeout = 10f;
        private float _delay = 0f;
        private Action _onTimeout;
        private GameObject _background;


        public override UIWindowLayer WindowLayer => UIWindowLayer.Waiting;
        public override bool IsUsedInTaskChokedEvent { get; } = false;

        public static void Open(float timeout, float delay, Action onTimeout)
        {
            var dlg = UIManager.Instance.OpenWindow<UIWaitingController>(GlobalPrefabPath.UIWaiting,
                new Data(timeout, delay));
            dlg._timeout = timeout;
            dlg._delay = delay;
            dlg._onTimeout = onTimeout;
        }

        public override void PrivateAwake()
        {
            // 触发动画
            this.gameObject.SetActive(false);
            this.gameObject.SetActive(true);

            isPlayDefaultCloseAudio = false;
            isPlayDefaultOpenAudio = false;

            EffectUIAnimation = false;

            _background = transform.Find("image").gameObject;
        }

        protected override void OnOpenWindow(UIWindowData data)
        {
            base.OnOpenWindow(data);

            if (data is Data uiData && uiData.Delay > 0)
            {
                _background.SetActive(false);
                StartCoroutine(Show());
            }
        }

        private IEnumerator Show()
        {
            yield return new WaitForSeconds(1f);
            _background.gameObject.SetActive(true);
        }

        private void Update()
        {
            _timeout -= Time.deltaTime;
            if (_timeout <= 0)
            {
                _onTimeout?.Invoke();
                CloseWindowWithinUIMgr(true);
            }
        }
    }
}
