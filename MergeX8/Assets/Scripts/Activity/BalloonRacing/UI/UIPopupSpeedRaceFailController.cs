using System;
using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using UnityEngine;
using UnityEngine.UI;

namespace Activity.BalloonRacing.UI
{
    public class UIPopupSpeedRaceFailController : UIWindowController
    {
        private Button _buttonClose;
        private Button _buttonJoin;

        private Action _endCall;

        public override void PrivateAwake()
        {

            _buttonClose = GetItem<Button>("Root/CloseButton");
            _buttonJoin = GetItem<Button>("Root/Button");

            _buttonJoin.onClick.AddListener(delegate { AnimCloseWindow(delegate { _endCall?.Invoke(); }); });

            _buttonClose.onClick.AddListener(delegate { AnimCloseWindow(delegate { _endCall?.Invoke(); }); });
        }

        protected override void OnOpenWindow(params object[] objs)
        {
            base.OnOpenWindow(objs);
            if (objs.Length > 0)
            {
                _endCall = (Action)objs[0];

            }
        }
    }
}
