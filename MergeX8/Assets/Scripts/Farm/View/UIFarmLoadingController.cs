using System;
using System.Collections;
using UnityEngine;

namespace Farm.View
{
    public class UIFarmLoadingController : UIWindowController
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
           var window = UIManager.Instance.OpenUI(UINameConst.UIFarmLoading);
           if(window == null)
               return;
           
           ((UIFarmLoadingController)(window)).Show(action);
        }

        public static void HideLoading(Action action = null)
        {
            var window = UIManager.Instance.GetOpenedUIByPath(UINameConst.UIFarmLoading);
            if(window == null)
                return;

            ((UIFarmLoadingController)(window)).Hide(action);
        }
    }
    
}