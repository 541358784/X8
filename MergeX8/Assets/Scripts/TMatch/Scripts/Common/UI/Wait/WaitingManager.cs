using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMatch
{


    public class WaitingManager : Manager<WaitingManager>
    {
        public void Open(float timeOut = 30.0f, float delay = 1f, Action onTimeout = null)
        {
            UIWaitingController.Open(timeOut, delay, onTimeout);
        }

        public void Close()
        {
            UIManager.Instance.CloseWindow<UIWaitingController>(true);
        }

    }
}
