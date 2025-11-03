using System.Runtime.InteropServices;
using AppIconChanger;
using DragonU3DSDK.Storage;
using UnityEngine;

public class IconChanger
{
    public static void ChangeIcon(string aliasName)
    {
#if UNITY_ANDROID
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            using (AndroidJavaClass iconChanger = new AndroidJavaClass("com.my.player.AppIconBridge"))
            {
                iconChanger.CallStatic("SetAppIcon", currentActivity, aliasName);
            }
        }
        
#elif UNITY_IOS
        iOS.SetAlternateIconName(aliasName);
#endif
    }

    // Example usage
    public static void OnChangeNewIcon()
    {
#if UNITY_ANDROID
        ChangeIcon("com.unity3d.player.UnityPlayerActivity");
#elif UNITY_IOS
        ChangeIcon("");
#endif
    }

    public static void OnChangeOldIcon()
    {
#if UNITY_ANDROID
        ChangeIcon("com.my.player.IconApp");
#elif  UNITY_IOS
        ChangeIcon("IconOld");
#endif
    }

    private const string changeIconKey = "key_changeIcon_new";
    public static void OnChangeIcon()
    {
        if(StorageManager.Instance.GetStorage<StorageHome>().RcoveryRecord.ContainsKey("1.0.63"))
            return;
        
        if(PlayerPrefs.HasKey(changeIconKey))
            return;

#if UNITY_ANDROID
        OnChangeOldIcon();
#elif  UNITY_IOS
        if (Makeover.Utils.IsNewUser())
        {
            OnChangeOldIcon();
        }
        else
        {
            OnChangeNewIcon();
        }
#endif
        PlayerPrefs.SetString(changeIconKey, changeIconKey);
        PlayerPrefs.Save();
    }
}
