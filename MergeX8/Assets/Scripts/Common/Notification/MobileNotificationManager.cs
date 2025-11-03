using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#elif UNITY_IOS
using Unity.Notifications.iOS;
using System.Runtime.InteropServices;
#endif

public enum MobileNotificationImportance
{
    None = 0,
    Low = 2,
    Default = 3,
    High = 4,
}

public class MobileNotification
{
    public int id;
    public string title;
    public string message;
    public string channel;
    public string userData;
}

public static class MobileNotificationManager
{
    public delegate void NotificationReceivedCallback(MobileNotification notification);
    public delegate void NotificationRespondedCallback(int id, string userData);

    public static event NotificationReceivedCallback OnNotificationReceived;
    public static event NotificationRespondedCallback OnNotificationResponded;
    public static bool inited { get; private set; }
    public static string token { get; private set; }
    public static System.DateTime lastRespondedTime { get; private set; }
    static bool initing;
#if UNITY_ANDROID
    static AndroidNotificationIntentData lastNotificationData;
#elif UNITY_IOS
    static iOSNotification lastNotificationData;
    static int firstNotificationId = 0;
    static string firstNotificationUserData = "";
    static List<iOSNotificationAttachment> attachments = new List<iOSNotificationAttachment>();
#endif

    public static IEnumerator Init(bool clearFlag = true)
    {
        if (inited || initing)
        {
            yield break;
        }
        initing = true;

#if UNITY_ANDROID
        inited = AndroidNotificationCenter.Initialize();
        token = "";

        if (inited)
        {
            CreateChannel("default", "Default", "Default channel");
            RefreshData();
            AndroidNotificationCenter.OnNotificationReceived += (notification) =>
            {
                if (OnNotificationReceived != null)
                {
                    OnNotificationReceived(new MobileNotification()
                    {
                        id = notification.Id,
                        title = notification.Notification.Title,
                        message = notification.Notification.Text,
                        channel = notification.Channel,
                        userData = notification.Notification.IntentData,
                    });
                }
            };
        }
#elif UNITY_IOS
        using (var req = new AuthorizationRequest(AuthorizationOption.Badge | AuthorizationOption.Sound | AuthorizationOption.Alert, false))
        {
            while (!req.IsFinished)
            {
                yield return null;
            }
            inited = req.Granted;
            token = req.DeviceToken;
        }

        if (inited)
        {
            RefreshData(firstNotificationId != 0);

            iOSNotificationCenter.OnNotificationReceived += (notification) =>
            {
                if (OnNotificationReceived != null)
                {
                    OnNotificationReceived(new MobileNotification()
                    {
                        id = int.Parse(notification.Identifier),
                        title = notification.Title,
                        message = notification.Body,
                        channel = notification.CategoryIdentifier,
                        userData = notification.Data,
                    });
                }
            };

            iOSNotificationCenter.OnRemoteNotificationReceived += (remoteNotification) =>
            {

            };
        }
#endif
        initing = false;
        if (clearFlag)
        {
            ClearDisplayedNotifications();
            ClearScheduledNotifications();
        }

        yield return RequestNotificationPermission();
    }

    public static IEnumerator Refresh(bool clearFlag = true)
    {
        if (inited)
        {
            RefreshData();
        }
        else
        {
            yield return Init(clearFlag);
        }
    }

    static void RefreshData(bool refreshFlag = false)
    {
#if UNITY_ANDROID
        var notificationData = AndroidNotificationCenter.GetLastNotificationIntent();
        refreshFlag = refreshFlag || (notificationData != null);
        lastNotificationData = notificationData ?? lastNotificationData;
#elif UNITY_IOS
        var notificationData = iOSNotificationCenter.GetLastRespondedNotification();
        refreshFlag = refreshFlag || (notificationData != null);
        lastNotificationData = notificationData ?? lastNotificationData;
#endif
        if (refreshFlag)
        {
            lastRespondedTime = System.DateTime.Now;
            if (OnNotificationResponded != null)
            {
                OnNotificationResponded(GetLastRespondedNotificationId(), GetLastRespondedNotificationData());
            }
        }
    }

    public static void CreateChannel(string id, string name, string description, MobileNotificationImportance importance = MobileNotificationImportance.Default, bool enableLights = true, bool vibration = true, long[] vibrationPattern = null, bool canBypassDnd = false)
    {
        if (!inited)
        {
            return;
        }

#if UNITY_ANDROID
        var channel = new AndroidNotificationChannel();
        channel.Id = id;
        channel.Name = name;
        channel.Description = description;
        channel.Importance = (Importance)importance;
        channel.EnableLights = enableLights;
        channel.EnableVibration = vibration;
        channel.VibrationPattern = vibrationPattern;
        channel.CanBypassDnd = canBypassDnd;
        channel.CanShowBadge = true;
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
#endif
    }

    public static void AddAttachment(string id, string url)
    {
#if UNITY_ANDROID
#elif UNITY_IOS
        attachments.Add(new iOSNotificationAttachment
        {
            Id = id,
            Url = url
        });
#endif
    }

    public static int SendNotification(int id, long delayMs, string title, string message, Color32 color, string userData = "", string largeIcon = "", bool repeats = false, string channel = "default", bool inForeground = false)
    {
        if (!inited)
        {
            return 0;
        }

#if UNITY_ANDROID
        var notification = new AndroidNotification
        {
            Title = title,
            Text = message,
            Color = color,
            FireTime = System.DateTime.Now.AddMilliseconds(delayMs),
            LargeIcon = largeIcon,
            SmallIcon = "notify_icon_small",
            IntentData = userData,
            ShouldAutoCancel = true,
            ShowTimestamp = true,
        };
        if (repeats)
        {
            notification.RepeatInterval = new System.TimeSpan(delayMs * 10000);
        }

        AndroidNotificationCenter.SendNotificationWithExplicitID(notification, channel, id);
        return id;
#elif UNITY_IOS
        var notification = new iOSNotification
        {
            Identifier = id.ToString(),
            Title = title,
            Body = message,
            CategoryIdentifier = channel,
            Trigger = new iOSNotificationTimeIntervalTrigger()
            {
                TimeInterval = new System.TimeSpan(delayMs * 10000),
                Repeats = repeats
            },
            Data = userData,
            ShowInForeground = inForeground,
        };
        if (inForeground)
        {
            notification.ForegroundPresentationOption = PresentationOption.Sound | PresentationOption.Alert;
        }
        if (!string.IsNullOrEmpty(largeIcon))
        {
            AddAttachment("attach" + notification.Identifier, largeIcon);
        }
        if (attachments.Count > 0)
        {
            notification.Attachments = attachments;
            attachments = new List<iOSNotificationAttachment>();
        }
        notification.UserInfo["_id_"] = notification.Identifier;
        iOSNotificationCenter.ScheduleNotification(notification);
        return id;
#else
        return 0;
#endif
    }

    public static int GetLastRespondedNotificationId()
    {
#if UNITY_ANDROID
        AndroidNotificationIntentData data = AndroidNotificationCenter.GetLastNotificationIntent() ?? lastNotificationData;
        return data != null ? data.Id : 0;
#elif UNITY_IOS
        iOSNotification notification = iOSNotificationCenter.GetLastRespondedNotification() ?? lastNotificationData;
        return notification != null ? int.Parse(notification.Identifier) : firstNotificationId;
#else
        return 0;
#endif
    }

    public static string GetLastRespondedNotificationData()
    {
#if UNITY_ANDROID
        AndroidNotificationIntentData data = AndroidNotificationCenter.GetLastNotificationIntent() ?? lastNotificationData;
        return data != null ? data.Notification.IntentData : "";
#elif UNITY_IOS
        iOSNotification notification = iOSNotificationCenter.GetLastRespondedNotification() ?? lastNotificationData;
        return notification != null ? notification.Data : firstNotificationUserData;
#else
        return "";
#endif
    }

    public static void CancelScheduledNotification(int id)
    {
#if UNITY_ANDROID
        AndroidNotificationCenter.CancelScheduledNotification(id);
#elif UNITY_IOS
        iOSNotificationCenter.RemoveScheduledNotification(id.ToString());
#endif
    }

    public static void ClearScheduledNotifications()
    {
#if UNITY_ANDROID
        AndroidNotificationCenter.CancelAllScheduledNotifications();
#elif UNITY_IOS
        iOSNotificationCenter.RemoveAllScheduledNotifications();
#endif
    }

    public static void ClearDisplayedNotifications()
    {
#if UNITY_ANDROID
        AndroidNotificationCenter.CancelAllDisplayedNotifications();
#elif UNITY_IOS
        iOSNotificationCenter.RemoveAllDeliveredNotifications();
#endif
    }

    public static bool GetAuthorizationStatus(string channel = "default")
    {
        bool enabled = DragonU3DSDK.DragonNativeBridge.IsUserNotificationEnabled();
#if UNITY_ANDROID
        return enabled && AndroidNotificationCenter.GetNotificationChannel(channel).Enabled;
#else
        return enabled;
#endif
    }

    public static void OpenNotificationSettings(string channel = "default")
    {
#if UNITY_ANDROID
        if (DragonU3DSDK.DragonNativeBridge.IsUserNotificationEnabled())
        {
            AndroidNotificationCenter.OpenNotificationSettings(channel);
        }
        else
        {
            DragonU3DSDK.DragonNativeBridge.OpenNotifycationSetting();
        }
#elif UNITY_IOS
        iOSNotificationCenter.OpenNotificationSettings();
#endif
    }
        
    public static IEnumerator RequestNotificationPermission()
    {
#if UNITY_ANDROID
        var request = new PermissionRequest();
        while (request.Status == PermissionStatus.RequestPending)
            yield return null;
#elif UNITY_IOS
             yield break;
        // using (var request = new AuthorizationRequest(AuthorizationOption.Badge | AuthorizationOption.Sound | AuthorizationOption.Alert, false))
        // {
        //     while (!request.IsFinished)
        //         yield return null;
        // }
#endif
        yield break;
    }
}