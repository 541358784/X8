using System;
using UnityEngine;

namespace MiniGame.View
{
    public class UIUIMiniGameLoadingController: UIWindowController
    {
        private Animator _animator;
        
        public override void PrivateAwake()
        {
            _animator = transform.GetComponent<Animator>();
        }

        public void Show(Action action)
        {
            StartCoroutine(CommonUtils.PlayAnimation(_animator, "appear", "", () =>
            {
                action?.Invoke();
            }));
        }

        public void Hide(Action action)
        {
            StartCoroutine(CommonUtils.PlayAnimation(_animator, "disappear", "", () =>
            {
                CloseWindowWithinUIMgr(true);
                action?.Invoke();
            })); 
        }

        public static void ShowLoading(Action action)
        {
            var window = UIManager.Instance.OpenWindow(UINameConst.UIMiniGameLoading);
            if(window == null)
                return;
           
            ((UIUIMiniGameLoadingController)(window)).Show(action);
        }

        public static void HideLoading(Action action = null)
        {
            var window = UIManager.Instance.OpenWindow(UINameConst.UIMiniGameLoading);
            if(window == null)
                return;

            ((UIUIMiniGameLoadingController)(window)).Hide(action);
        }
    }
}