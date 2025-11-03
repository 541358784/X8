using System;
using System.Collections;
using DragonPlus;
using DragonPlus.ConfigHub;
using DragonU3DSDK;
using DragonU3DSDK.Account;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Storage;
using Framework;
using TMatch;
using UnityEngine;
using AudioManager = DragonPlus.AudioManager;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class GameMain : MonoBehaviour, IPrivacyResponsor
{
    ScreenOrientation DeviceOrientation { get; set; }

    private void Awake()
    {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        Debug.unityLogger.logEnabled = true;
#else
        Debug.unityLogger.logEnabled = false;
#endif
        Application.targetFrameRate = 60;
        //PlayerPrefs.DeleteAll();
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.lowMemory += OnLowMemory;
        // DontDestroyOnLoad(this.gameObject);

        //GameObject mUIRoot = Instantiate(mUIGO, null, false);
        //DontDestroyOnLoad(mUIRoot);

        // StartCoroutine(CommonUtils.DelayWork(2, () =>
        // {
        //     GameObject srDebugObj = GameObject.Find("SR_ButtonContainer");
        //     if (srDebugObj != null)
        //     {
        //         srDebugObj.transform.localScale = new Vector3(2, 1, 1);
        //     }
        // }));

        ResourcesManager.Instance.AddAtlasPrefix("SpriteAtlas/Home/");
        ResourcesManager.Instance.AddAtlasPrefix(AtlasPrefix.TMatch);
        ResourcesManager.Instance.AddAtlasPrefix("Screw/Atlas/");
        
        EventDispatcher.Instance.AddEventListener(EventEnum.OnConfigHubUpdated, OnConfigHubUpdated);
        
        MobileNotificationManager.OnNotificationReceived += NotificationReceivedCallback;
        MobileNotificationManager.OnNotificationResponded += NotificationRespondedCallback;
        StartCoroutine(MobileNotificationManager.Init());
    }

    public void NotificationReceivedCallback(MobileNotification notification)
    {
        DebugUtil.LogError("NotificationReceivedCallback!!!");
    }

    public void NotificationRespondedCallback(int id, string userData)
    {
        DebugUtil.LogError("NotificationRespondedCallback!!!");
    }
    
    private void Update()
    {
    }

    private void OnApplicationFocus(bool focus)
    {
      
        if (focus)
        {
            StartCoroutine(InitNotificationRefresh());
            NotificationManager.Instance.ClearNotifications();
        }   
    }
    private  IEnumerator InitNotificationRefresh()
    {
        yield return  MobileNotificationManager.Refresh();
        //yield return new WaitForSeconds(2);
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            NotificationManager.Instance.RegistLocalNotifications();

            // 写磁盘
            PlayerPrefs.Save();

            DragonNativeBridge.PauseAllSound();
        }
        else
        {
            AudioManager.Instance.ResumeAllMusic();
            DragonNativeBridge.ResumeAllSound();
            NotificationManager.Instance.ClearNotifications();
        }

        // IronSource.Agent.onApplicationPause(pause);
    }

    private void OnApplicationQuit()
    {
        DragonNativeBridge.ResumeAllSound();
        Application.lowMemory -= OnLowMemory;
        NotificationManager.Instance.RegistLocalNotifications();
    }

    private void OnLowMemory()
    {
        Resources.UnloadUnusedAssets();
#if !UNITY_EDITOR
        System.GC.Collect();
#endif
    }

    public void OnPrivacyAccepted(string message)
    {
        EventDispatcher.Instance.DispatchEvent(EventEnum.GDPR_ACCEPTED);
    }

    private void OnConfigHubUpdated(BaseEvent e)
    {
        UserGroupManager.Instance.OnConfigHubUpdated();
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.OnConfigHubUpdated, OnConfigHubUpdated);
    }
#if UNITY_IOS && !UNITY_EDITOR
    public void OnPrivacyRefused(string message)
    {
        EventDispatcher.Instance.DispatchEvent(EventEnum.GDPR_REFUSED);
    }

    public void LaterOneDay()
    {
        StorageHome storage = StorageManager.Instance.GetStorage<StorageHome>();
        storage.RateUsData.LastPopUpTime = CommonUtils.GetTimeStamp()+ (long)24 * 3600 * 1000;
    }
    public void LaterOneMonth()
    {
        StorageHome storage = StorageManager.Instance.GetStorage<StorageHome>();
        storage.RateUsData.LastPopUpTime = CommonUtils.GetTimeStamp()+ (long)29 * 24 * 3600 * 1000;
    }
    public void RateUsNow()
    {
        StorageHome storage = StorageManager.Instance.GetStorage<StorageHome>();
        storage.RateUsFinish = true;
    }
#endif
}