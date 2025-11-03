using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using DragonPlus;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Config;
using Framework;
using Gameplay;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class Global
{
    public static void ShowUIWaiting(float timeout, Action onTimeout = null)
    {
        WaitingController dlg = UIManager.Instance.OpenUI(UINameConst.UIWaiting, timeout) as WaitingController;
        dlg.FinishAction = onTimeout;
    }

    public static void HideUIWaiting()
    {
        WaitingController uIWaitingController =
            UIManager.Instance.GetOpenedUIByPath<WaitingController>(UINameConst.UIWaiting);
        if (uIWaitingController != null)
        {
            uIWaitingController.CloseWindowWithinUIMgr(true);
        }
    }

    public static bool IsUIWaiting()
    {
        return UIManager.Instance.GetOpenedUIByPath<WaitingController>(UINameConst.UIWaiting) != null;
    }

    public static void LogError(string content, params object[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            content = content + ", " + args[i];
        }

        DebugUtil.LogError("异常：" + content, args);
    }

    public static string GetSignResName(string typeStr)
    {
        switch (typeStr)
        {
            case "prize":
                return "ui_public_pic_prize_bg";
            case "new":
                return "ui_public_pic_new_bg";
        }

        return string.Empty;
    }

    // 飘字提示
    public static void ShowHint(string text)
    {
        UIPublicTextFlyController.Popup(text);
    }

    /// <summary>
    /// 本地化处理一个对象下所有的子text
    /// </summary>
    /// <param name="obj"></param>
    public static void SetLocal(GameObject obj)
    {
        Text[] labels = obj.transform.GetComponentsInChildren<Text>(true);

        foreach (Text label in labels)
        {
            label.text = LocalizationManager.Instance.GetLocalizedString(label.text.Trim());
        }
    }

    public static bool isPadSize()
    {
        float uiRatio = 1.0f * Mathf.Max(Screen.height, Screen.width) / Mathf.Min(Screen.height, Screen.width);
        return uiRatio <= 1.605f;
    }

    public static GameObject GetParObjByPlatform(GameObject uiObj, string objNamePad, string objName)
    {
        GameObject MasterContentIPAD = uiObj.transform.Find(objNamePad).gameObject;
        GameObject masterContent = uiObj.transform.Find(objName).gameObject;
        GameObject nowMasterConstent;
        if (isPadSize())
        {
            nowMasterConstent = MasterContentIPAD;
            MasterContentIPAD.SetActive(true);
            masterContent.SetActive(false);
        }
        else
        {
            nowMasterConstent = masterContent;
            MasterContentIPAD.SetActive(false);
            masterContent.SetActive(true);
        }

        return nowMasterConstent;
    }

    public static void OpenAppStore()
    {
#if UNITY_IPHONE
        string iosStore =
 GlobalConfigManager.Instance.GetGlobal_Config_Number_Value(GlobalStringConfigKey.appstore_url);
        DebugUtil.Log($"ios app store url is empty.not config key:{GlobalStringConfigKey.appstore_url},value={GlobalConfigManager.Instance.GetGlobal_Config_Number_Value(GlobalStringConfigKey.appstore_url)}");
        Application.OpenURL(iosStore);
#elif SUB_CHANNEL_AMAZON
        string gStore = GlobalConfigManager.Instance.GetGlobal_Config_Number_Value(GlobalStringConfigKey.amazon_url);
        DebugUtil.Log($"amazon play store url is empty.not config key:{GlobalStringConfigKey.gpstore_url},value={GlobalConfigManager.Instance.GetGlobal_Config_Number_Value(GlobalStringConfigKey.gpstore_url)}");
        Application.OpenURL(gStore);
#else
        string gStore = GlobalConfigManager.Instance.GetGlobal_Config_Number_Value(GlobalStringConfigKey.gpstore_url);
        DebugUtil.Log(
            $"google play store url is empty.not config key:{GlobalStringConfigKey.gpstore_url},value={GlobalConfigManager.Instance.GetGlobal_Config_Number_Value(GlobalStringConfigKey.gpstore_url)}");
        Application.OpenURL(gStore);
#endif
    }

    public static Vector3 GetFullScreenIOSBias()
    {
        return new Vector3(50, 0, 0);
    }

    public static Vector3 BottomMargin()
    {
        return new Vector3(0, 50, 0);
    }
}