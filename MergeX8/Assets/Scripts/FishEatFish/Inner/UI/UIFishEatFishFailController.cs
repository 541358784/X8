
using System;
using UnityEngine.UI;

namespace FishEatFishSpace
{
    public class UIFishEatFishFailController : UIWindowController
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

        public static UIFishEatFishFailController Open(Action cb)
        {
            return UIManager.Instance.OpenUI(UINameConst.UIFishEatFishFail, cb) as UIFishEatFishFailController;
        }
    }
}