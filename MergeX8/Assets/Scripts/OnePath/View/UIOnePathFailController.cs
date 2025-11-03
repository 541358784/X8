
using System;
using UnityEngine.UI;

namespace OnePath.View
{
    public class UIOnePathFailController : UIWindowController
    {
        private Button CloseBtn;
        private Button StartBtn;
        public override void PrivateAwake()
        {
            CloseBtn = GetItem<Button>("Root/BgPopupBoand/ButtonClose");
            CloseBtn.onClick.AddListener(OnClickStart);
            StartBtn = GetItem<Button>("Root/ButtonGroup/ButtonRetry");
            StartBtn.onClick.AddListener(OnClickStart);
        }

        private Action Callback;
        protected override void OnOpenWindow(params object[] objs)
        {
            base.OnOpenWindow(objs);
            if (objs.Length > 0)
            {
                Callback = objs[0] as Action;
            }
        }

        public void OnClickStart()
        {
            AnimCloseWindow(Callback);
        }

        public static UIOnePathFailController Open(Action cb = null)
        {
            return UIManager.Instance.OpenUI(UINameConst.UIOnePathFail, cb) as UIOnePathFailController;
        }
    }
}