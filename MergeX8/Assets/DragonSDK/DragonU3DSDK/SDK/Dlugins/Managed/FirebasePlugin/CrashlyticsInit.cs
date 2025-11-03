
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dlugin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Dlugin.PluginStructs;
using com.adjust.sdk;
using System;
using System.Web;
using Firebase.DynamicLinks;
using System.Collections.Specialized;
using DragonU3DSDK;
using DragonU3DSDK.Asset;

public class CrashlyticsInit : MonoBehaviour
{
    // Use this for initialization
    bool userIdSet = false;
    bool tokenUpdated = false;

    void Start()
    {
#if !UNITY_EDITOR
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            DebugUtil.Log("Firebase CheckAndFixDependenciesAsync() : " + dependencyStatus);
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {

                DebugUtil.Log("Firebase Plugin.Initialized1");
                // Create and hold a reference to your FirebaseApp, i.e.
                //   app = Firebase.FirebaseApp.DefaultInstance;
                // where app is a Firebase.FirebaseApp property of your application class.
                Firebase.FirebaseApp app = Firebase.FirebaseApp.DefaultInstance;
                // Set a flag here indicating that Firebase is ready to use by your
                // application.

                //初始化消息
                DebugUtil.Log("Firebase Init Messaging");
                Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
                Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;

                DebugUtil.Log("Firebase DynamicLinks Initialize ");
                //初始化动态链接
                DynamicLinks.DynamicLinkReceived += OnDynamicLink;

                //Firebase.Messaging.FirebaseMessaging.sen
                DebugUtil.Log("Firebase Plugin.Initialized2");

                FirebaseState.Instance.Initialized = true;
                DebugUtil.Log("Firebase Plugin.Initialized4");

                ChangeableConfig.Instance.FetchDataAsync();
            }
            else
            {
                DebugUtil.Log("Firebase Plugin.Initialized5");
                DebugUtil.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
#endif
    }

    // Update is called once per frame
    void Update()
    {
        if (!userIdSet && DragonU3DSDK.Storage.StorageManager.Instance.Inited && FirebaseState.Instance.Initialized)
        {
            var storageCommon = DragonU3DSDK.Storage.StorageManager.Instance.GetStorage<DragonU3DSDK.Storage.StorageCommon>();
            var playerId = storageCommon.PlayerId;
            if (playerId > 0)
            {
                Firebase.Analytics.FirebaseAnalytics.SetUserId(storageCommon.PlayerId.ToString());
                Firebase.Crashlytics.Crashlytics.SetUserId(playerId.ToString());
                userIdSet = true;
            }
        }

        if (tokenUpdated && DragonU3DSDK.Account.AccountManager.Instance.HasLogin && DragonU3DSDK.Network.API.APIManager.Instance.HasNetwork)
        {
            var storageCommon = DragonU3DSDK.Storage.StorageManager.Instance.GetStorage<DragonU3DSDK.Storage.StorageCommon>();
            DragonU3DSDK.Network.API.APIManager.Instance.Send(new DragonU3DSDK.Network.API.Protocol.CBindFirebase
            {
                FirebaseInstanceId = storageCommon.FirebaseInstanceId
            },
            (DragonU3DSDK.Network.API.Protocol.SBindFirebase resp) =>
            {
                DebugUtil.Log("new firebase token sent to server");
            },
            (errno, errmsg, resp) =>
            {
                DebugUtil.Log("new firebase token sent to server failed, retrying");
                waitAndRun(10, () =>
                {
                    tokenUpdated = true;
                });
            });
            tokenUpdated = false;
        }
    }

    void waitAndRun(int seconds, Action cb)
    {
        StartCoroutine(_waitAndRun(seconds, cb));
    }

    IEnumerator _waitAndRun(int seconds, Action cb)
    {
        yield return new WaitForSeconds(seconds);
        cb?.Invoke();
    }

    public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
    {
        DebugUtil.Log("Firebase Received Registration Token: " + token.Token);
        DragonU3DSDK.EventManager.Instance.Trigger<DragonU3DSDK.SDKEvents.FirebaseInstanceIdReceived>().Data(token.Token).Trigger();
        DragonU3DSDK.Network.BI.BIManager.Instance.SendCommonGameEvent(DragonU3DSDK.Network.API.Protocol.BiEventCommon.Types.CommonGameEventType.FirebaseIdUpdated, token.Token);
        var storageCommon = DragonU3DSDK.Storage.StorageManager.Instance.GetStorage<DragonU3DSDK.Storage.StorageCommon>();
        if (storageCommon != null)
        {
            storageCommon.FirebaseInstanceId = token.Token;
        }
        tokenUpdated = true;
    }

    public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
    {
        try
        {
            if (e != null && e.Message != null && e.Message.Data != null && e.Message.Data.TryGetValue("MarketingURL", out string url) && !string.IsNullOrEmpty(url))
            {
                DebugUtil.Log("FireBase Notice Open Url: " + url);
                Application.OpenURL(url);
            }
        }
        catch (Exception exception)
        {
            return;
        }
    }

    // Display the dynamic link received by the application.
    void OnDynamicLink(object sender, EventArgs args)
    {
        var dynamicLinkEventArgs = args as ReceivedDynamicLinkEventArgs;

        Uri uri = dynamicLinkEventArgs.ReceivedDynamicLink.Url;

        DebugUtil.Log("Received dynamic link {0}",
                        dynamicLinkEventArgs.ReceivedDynamicLink.Url.OriginalString);
        DebugUtil.Log("Received dynamic link Host {0}",
            dynamicLinkEventArgs.ReceivedDynamicLink.Url.Host);
        DebugUtil.Log("Received dynamic link HostType {0}",
            dynamicLinkEventArgs.ReceivedDynamicLink.Url.HostNameType);
        DebugUtil.Log("Received dynamic link Query {0}",
            dynamicLinkEventArgs.ReceivedDynamicLink.Url.Query);


        NameValueCollection nvc = DragonU3DSDK.Utils.ParseQueryString(uri.Query);

        DragonU3DSDK.EventManager.Instance.Trigger<DragonU3DSDK.SDKEvents.DeepLinkEvent>().Data(nvc).Trigger();
    }
}
