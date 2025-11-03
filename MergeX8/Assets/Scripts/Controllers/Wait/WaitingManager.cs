using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingManager : Manager<WaitingManager>
{
    public void OpenWindow(float timeOut = 30.0f, float delayTime = 0.0f, Action onTimeout = null)
    {
        WaitingController.Open(timeOut, delayTime, onTimeout);
    }

    public void CloseWindow()
    {
        UIManager.Instance.CloseUI(UINameConst.UIWaiting, true);
    }
}