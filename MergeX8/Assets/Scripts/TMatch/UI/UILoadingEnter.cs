using System;
using System.Collections;
using UnityEngine;

namespace TMatch
{
    public class UILoadingEnter : UIWindowControllerEx
    {
        public override bool IsUsedInTaskChokedEvent => false;
        
        public override UIWindowLayer WindowLayer => UIWindowLayer.Loading;

        public override bool EffectUIAnimation => false;

        private class Data : UIWindowData
        {
            public Action Callback;
        }

        private Data _data;

        public override void PrivateAwake()
        {
        }

        private IEnumerator DelayClose()
        {
            yield return new WaitForSeconds(1.5f);
            InternalClose();
            _data?.Callback?.Invoke();
        }

        protected override void OnOpenWindow(UIWindowData data)
        {
            base.OnOpenWindow(data);

            _data = data as Data;
            StartCoroutine(DelayClose());
        }

        public static void Open(Action onClose)
        {
            UIManager.Instance.OpenWindow<UILoadingEnter>("TMatch/Prefabs/TMLoading1",
                new Data {Callback = onClose});
        }
    }
}