using UnityEngine.UI;

namespace Framework.UI
{
    public abstract class UIPopupView : UIView
    {
        protected virtual bool AutoBgClose    => true;
        protected virtual bool CommonCloseBtn => true;

        protected Button _closeBtn;

        public static int PopupCount;

        protected override void OnCreate()
        {
            base.OnCreate();

            if (AutoBgClose)
            {
                BindButtonEvent("Root/CommonPopupItem/ImgMask", OnBgImgClick);
            }

            if (CommonCloseBtn)
            {
                _closeBtn = BindButtonEvent("Root/CommonPopupItem/CommonGroup/ButtonClose", OnCloseBtnClick);
            }
        }

        protected void OnCreate(string popupCommonItemPath)
        {
            base.OnCreate();

            if (AutoBgClose)
            {
                BindButtonEvent(popupCommonItemPath + "/ImgMask", OnBgImgClick);
            }

            _closeBtn = BindButtonEvent(popupCommonItemPath + "/CommonGroup/ButtonClose", OnCloseBtnClick);
        }

        protected internal override void OnOpen<T>(UIData data)
        {
            base.OnOpen<T>(data);

            PopupCount++;

            // EventBus.Send<EventUIPopupShow>();
            //
            // EventBus.Register<EventStorageReload>(OnEventStorageReload);
            // EventBus.Register<EVENT_UI_CLOSE_ALL_POPUP>(OnEventCloseAllPopup);
        }

        protected override void OnClose()
        {
            base.OnClose();

            // EventBus.Send<EventUIPopupClose>();
            //
            // EventBus.UnRegister<EventStorageReload>(OnEventStorageReload);
            // EventBus.UnRegister<EVENT_UI_CLOSE_ALL_POPUP>(OnEventCloseAllPopup);
        }

        private void OnEventCloseAllPopup(EVENT_UI_CLOSE_ALL_POPUP e)
        {
            Close();
        }

        protected internal override void OnRemove()
        {
            base.OnRemove();

            if (!UIManager.Instance.HasPopup(this)) PopupCount = 0;
        }

        // protected virtual void OnEventStorageReload(EventStorageReload obj)
        // {
        //     Close();
        // }

        protected virtual void OnBgImgClick()
        {
            if (!AutoBgClose) return;

            Close();
        }

        protected virtual void OnCloseBtnClick()
        {
            Close();
        }

        protected void ShowCloseBtn(bool show)
        {
            _closeBtn.gameObject.SetActive(show);
        }
    }
}