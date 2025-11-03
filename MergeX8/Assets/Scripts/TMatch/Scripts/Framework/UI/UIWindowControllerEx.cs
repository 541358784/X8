using System;

namespace TMatch
{


    public abstract class UIWindowControllerEx : UIWindowController
    {
        private Action _onClose;

        protected bool IsCloseEnable = true;

        protected override void OnBackButtonCallBack()
        {
            base.OnBackButtonCallBack();

            if (IsCloseEnable && !global::Global.IsUIWaiting())
            {
                CloseWindowWithinUIMgr();
            }
        }

        protected override void OnCloseWindow(bool destroy = true)
        {
            base.OnCloseWindow(destroy);

            _onClose?.Invoke();
        }

        protected void InternalClose()
        {
            InternalClose(null);
        }

        protected void InternalClose(Action onClose)
        {
            if (!IsCloseEnable)
                return;
            _onClose = onClose;
            CloseWindowWithinUIMgr();
        }
    }
}