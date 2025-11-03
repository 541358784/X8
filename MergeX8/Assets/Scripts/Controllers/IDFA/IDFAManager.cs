using System;
using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine;

public class IDFAManager : Manager<IDFAManager>, IATTResponsor
{
    private string minVersion = "14.5";

    public bool isVersionSucc()
    {
#if UNITY_IOS
        Version localVersion = new Version(UnityEngine.iOS.Device.systemVersion);
        Version webVersion = new Version(minVersion);
        if (localVersion.CompareTo(webVersion) < 0)
        {
            return false;
        }
        return true;
#endif
        return false;
    }

    public void RequestIDFA()
    {
        if (isVersionSucc() == false)
            return;
        DragonNativeBridge.RequestTrackingAuthorization(gameObject.name);
    }

    public void OnATTAccepted(string message)
    {
        // GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventIdfaSuccess, "true");
    }

    public void OnATTRefused(string message)
    {
        // GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventIdfaSuccess, "false");
    }
}