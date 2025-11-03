using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using Framework;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public static class ShareUtils
{
    private const string SHARE_IMG_NAME = "shared_img{0}.jpg";
    private const string SHARE_PATH = "Share";
    private static Texture2D watermark;

    private static string dirPath;

    //TODO 设置水印图片，注意此图片的texture import setting 一定要设置可读可写，否则合成会失败
    private static string deafaultWaterMarkImgPath = "UI/Basic/Login/logo";

    // private static int shareImgWidth = 1080;
    private static int index = 1;
    public static int jpgQuality = 100;

    public static void ShareImg(Texture2D texture2D, string shareTitle, string shareContent, string shareSubject,
        string waterMarkPath = "")
    {
        try
        {
            watermark = GetWatermark(waterMarkPath);
            if (watermark != null)
            {
                texture2D = AddWatermark(texture2D, watermark, 0, 0);
            }

            byte[] bytes = texture2D.EncodeToJPG(jpgQuality);

            var cachePath = Path.Combine(FilePathTools.persistentDataPath_Platform, SHARE_PATH);
            if (!Directory.Exists(cachePath))
            {
                Directory.CreateDirectory(cachePath);
            }

            var cacheName = Path.Combine(cachePath, string.Format(SHARE_IMG_NAME, index));
            index = 1 - index; // 切换文件名，尝试解决QQ总是分享缓存中的文件
            if (File.Exists(cacheName))
            {
                File.Delete(cacheName);
            }

            File.WriteAllBytes(cacheName, bytes);
            DebugUtil.Log($"file saved : {cacheName}");

            GamePauseManager.Instance.RegisterPauseReason(GamePauseManager.PauseReasonMask.Share);
            // new NativeShare().AddFile(cacheName).SetTitle(shareTitle).SetSubject(shareSubject).SetText(shareContent)
            //     .Share();
        }
        catch (Exception e)
        {
            DebugUtil.LogError(e);
        }
    }

    public static Texture2D GetWatermark(string waterMarkPath = "")
    {
        Texture2D result;
        if (string.IsNullOrEmpty(waterMarkPath))
        {
            result = ResourcesManager.Instance.LoadResource<Texture2D>(deafaultWaterMarkImgPath);
        }
        else
        {
            result = ResourcesManager.Instance.LoadResource<Texture2D>(waterMarkPath);
        }

        return result;
    }

    /// <summary>
    /// 添加水印
    /// </summary>
    /// <param name="background">背景图</param>
    /// <param name="watermark">水印</param>
    /// <param name="foffsetX">x偏移量</param>
    /// <param name="offsetY">y偏移量</param>
    /// <returns>结果图</returns>
    public static Texture2D AddWatermark(Texture2D background, Texture2D watermark, int foffsetX, int offsetY)
    {
        try
        {
            int startX = background.width - watermark.width - foffsetX;
            int endX = startX + watermark.width;
            int startY = offsetY;
            int endY = startY + watermark.height;
            for (int x = startX; x < endX; x++)
            {
                for (int y = startY; y < endY; y++)
                {
                    Color bgColor = background.GetPixel(x, y);
                    Color wmColor = watermark.GetPixel(x - startX, y - startY);
                    Color final_color = Color.Lerp(bgColor, wmColor, wmColor.a / 1.0f);
                    background.SetPixel(x, y, final_color);
                }
            }

            background.Apply();
        }
        catch (Exception e)
        {
            DebugUtil.LogError(e);
        }

        return background;
    }
}