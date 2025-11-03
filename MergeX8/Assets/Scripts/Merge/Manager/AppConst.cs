/****************************************************
    文件：AppConst.cs
    作者：Cys
    邮箱: yongsheng.chen@dragonplus.com
    日期： 2021-10-27-11:03:56
    功能：....
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class AppConst
{
    public static int firstTaskId = 90001000;
    public static int firstNodeId = 9000101;
    public static int secondTaskId = 90001010;
    public static int thirdTaskId = 90001012;

    public static string ConvertBiNameKey(string key)
    {
        string result = "";
        string[] keys = key.Split('_');
        for (int i = 0; i < keys.Length; i++)
        {
            result += keys[i];
            keys[i].Replace("_", "");
        }

        return result;
    }

    public static string tempButRemoveAdsType; // 购买去除广告场景
    public static string tempBuyStarPackType; // 购买新手礼包场景
    public static string tempBuySpecialPackType; //购买特价礼包场景
    public static string tempBuyWeeklyCardType; //购买周卡场景
    public static string tempBuyBuildBundleType; //购买建筑礼包场景
}