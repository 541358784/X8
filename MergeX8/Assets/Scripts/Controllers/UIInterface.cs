using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Dlugin;
using DragonPlus;
using DragonU3DSDK;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Framework;
using UnityEngine;
using static Gameplay.UserData;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

namespace Gameplay.UI
{
    public class UIInterface
    {
        static UIInterface _instance;

        public static UIInterface Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new UIInterface();
                }

                return _instance;
            }
        }

        private UIInterface()
        {
        }

        public void ShowMsgBox(UIPopupMsgBoxController.PopupInfo popupInfo)
        {
            // 防止连续的弹窗打不开的bug
            CoroutineManager.Instance.StartCoroutine(_CoShowMsgBox(popupInfo));
        }

        private IEnumerator _CoShowMsgBox(UIPopupMsgBoxController.PopupInfo popupInfo)
        {
            yield return null;
            UIPopupMsgBoxController.Popup(popupInfo);
        }
    }
}