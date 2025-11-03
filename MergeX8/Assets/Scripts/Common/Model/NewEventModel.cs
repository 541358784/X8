/*
 * @file NewEventModel
 * 游戏公告
 * @author lu
 */

using System;
using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK;
using DragonU3DSDK.Account;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Google.Protobuf;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class NewEventModel : Manager<NewEventModel>
{
    public Announcement Data = null;
    public long LastPopWindowTime = 0;
    public Texture2D wwwImage;
    bool OmLoadImage;

    private IEnumerator LoadImage()
    {
        DebugUtil.Log("New Event : 开始获取公告图片 - 开始");

        if (wwwImage != null)
        {
            DebugUtil.Log("New Event : 开始获取公告图片 - 有缓存的图片未弹出,不继续下载");
            yield break;
        }

        if (OmLoadImage)
        {
            DebugUtil.Log("New Event : 开始获取公告图片 - 正在下载图片,不在发起新的下载");
            yield break;
        }

        OmLoadImage = true;
        //Data.ImgUrl = Data.ImgUrl.Replace("http", "https");
        using (var uwr = UnityWebRequestTexture.GetTexture(Data.ImgUrl))
        {
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError || uwr.isHttpError)
            {
                DebugUtil.LogError(uwr.error);
            }
            else
            {
                // Get downloaded asset bundle
                wwwImage = DownloadHandlerTexture.GetContent(uwr);
            }

            OmLoadImage = false;
        }
        // using (WWW www = new WWW(Data.ImgUrl))
        // {
        //     // Wait for download to complete
        //     yield return www;

        //     if (!string.IsNullOrEmpty(www.error))
        //     {
        //         DebugUtil.Log("New Event : 开始获取公告图片 - 失败 - " + www.error);
        //     }
        //     else
        //     {
        //         DebugUtil.Log("New Event : 开始获取公告图片 - 成功");

        //         wwwImage = www.texture;
        //     }

        //     OmLoadImage = false;
        // }
    }

    public string GetCustomizeMsg()
    {
        var locale = LocalizationManager.Instance.GetCurrentLocale();
        string locale_content;
        if (Data.LocaleMsgInfos != null)
        {
            Data.LocaleMsgInfos.TryGetValue(locale, out locale_content);
            if (locale_content == null)
                Data.LocaleMsgInfos.TryGetValue(Locale.ENGLISH, out locale_content);

            if (locale_content == null)
                locale_content = Data.MsgText;
        }
        else
            locale_content = Data.MsgText;

        return locale_content;
    }

    // 获得是否可以弹出公告
    public bool GetOpenState()
    {
        // 没有公告
        if (Data == null)
        {
            DebugUtil.Log("New Event : 没有公告");
            return false;
        }

        // 是否满足解锁等级


        // 弹出间隔CD
        if ((long) Data.CdSeconds * 1000 + LastPopWindowTime > (long) APIManager.Instance.GetServerTime())
        {
            DebugUtil.Log("New Event : 获得是否可以弹出公告 - false - 弹出间隔CD");

            return false;
        }

        try
        {
            if (!string.IsNullOrEmpty(Data.PlayerCondition))
            {
                var o = JObject.Parse(Data.PlayerCondition);
                //var storageRoot = StorageManager.Instance.GetStorage<StorageCook>();
                var storageCommon = StorageManager.Instance.GetStorage<StorageCommon>();

                // 筛选条件,注册时间;
                o.TryGetValue("createdTimeRange", out var createdTimeRange);
                if (createdTimeRange != null)
                {
                    var times = createdTimeRange.ToObject<ulong[]>();
                    var startTime = times[0];
                    var endTime = times[1];

                    if (startTime > storageCommon.InstalledAt || endTime < storageCommon.InstalledAt)
                    {
                        DebugUtil.Log("New Event : 获得是否可以弹出公告 - false - 筛选条件,注册时间;");
                        return false;
                    }
                }

                // 筛选条件,是否绑定facebook;
                o.TryGetValue("isFacebook", out var isFacebook);
                if (isFacebook != null)
                {
                    var IsFacebook = isFacebook.ToObject<bool>();

                    if (AccountManager.Instance.HasBindFacebook() != IsFacebook)
                    {
                        DebugUtil.Log("New Event : 获得是否可以弹出公告 - false - 筛选条件,是否绑定facebook;");
                        return false;
                    }
                }

                // 筛选条件,是否付费;
                o.TryGetValue("isPaid", out var isPaid);
                if (isPaid != null)
                {
                    var IsPaid = isPaid.ToObject<bool>();

                    if ((storageCommon.RevenueUSDCents > 0) != IsPaid)
                    {
                        DebugUtil.Log("New Event : 获得是否可以弹出公告 - false - 筛选条件,是否付费;");
                        return false;
                    }
                }

                // 筛选条件,平台;
                o.TryGetValue("platform", out var platform);
                if (platform != null)
                {
                    var Platform = platform.ToObject<int>();

                    if ((int) DeviceHelper.GetPlatform() != Platform)
                    {
                        var a = DeviceHelper.GetPlatform();

                        DebugUtil.Log("New Event : 获得是否可以弹出公告 - false - 筛选条件,平台;");
                        return false;
                    }
                }

                // 筛选条件,最大版本号;
                JToken maxNativeVersion;
                o.TryGetValue("maxNativeVersion", out maxNativeVersion);
                if (maxNativeVersion != null)
                {
                    DebugUtil.Log("New Event : 获得是否可以弹出公告 - 有版本信息,开始按照平台筛选;");

                    var MaxNativeVersionList = maxNativeVersion.ToObject<Dictionary<string, string>>();
                    foreach (var item in MaxNativeVersionList)
                    {
                        if (int.Parse(item.Key) != (int) DeviceHelper.GetPlatform()) continue;
                        DebugUtil.Log("New Event : 获得是否可以弹出公告 - 有版本信息,找到对应平台信息;");

                        var MaxNativeVersion = item.Value;

                        if (int.Parse(MaxNativeVersion) <= DragonNativeBridge.GetVersionCode())
                        {
                            DebugUtil.Log("New Event : 获得是否可以弹出公告 - false - 筛选条件,最大版本号;");
                            return false;
                        }

                        break;
                    }
                }
            }
        }
        catch (Exception e)
        {
            DebugUtil.LogError(e);
        }

        DebugUtil.Log("New Event : 获得是否可以弹出公告 - true");

        return true;
    }

    // 获取版本升级公告
    public void GetUpdateInfo()
    {
        APIManager.Instance.Send(new CListAnnouncements(), (IMessage obj) =>
        {
            var info = (obj as SListAnnouncements);

            // 没有公告
            if (info.Announcement == null || info.Announcement.Count == 0)
            {
                DebugUtil.Log("New Event : 获取版本升级公告 - 获取成功,但内容为空,或0个公告");
                Data = null;
                return;
            }

            for (var index = 0; index < info.Announcement.Count; ++index)
            {
                Data = info.Announcement[index];
                if (!GetOpenState()) continue;
                DebugUtil.Log("New Event : 获取版本升级公告 - 获取成功,保存数据 - 使用第" + (index + 1).ToString() + "条消息");

                StartCoroutine(LoadImage());
                break;
            }

            if (Data == null)
            {
                DebugUtil.Log("New Event : 获取版本升级公告 - 获取成功,保存数据 - 没有符合弹出的公告");
            }
        }, (arg1, arg2, arg3) =>
        {
            DebugUtil.LogError("New Event : 获取版本升级公告 - 失败, " +
                               $"error code : {arg1.ToString()}, string : {arg2}, message : {arg3}");
        });
    }
}