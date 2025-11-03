/*----------------------------------------------------------------
// Copyright (C) 2019 北京，天龙互娱
//
// 模块名：QualityManager
// 创建日期：2019-1-29
// 创建者：waicheng.wang
// 模块描述：管理显示品质和设备性能（是否使用低清图集，是否关闭部分广告渠道等），请将该脚本执行顺序设置为自定义脚本中最先
//----------------------------------------------------------------*/
using UnityEngine;

using DragonU3DSDK.Asset;

namespace DragonU3DSDK.Quality
{
    public class QualityManager : MonoBehaviour
    {

        private static bool IsSDTexture = false;//是否使用低清贴图

        //private int originalHeight;
        //private int originalWidth;

        //private int scaleWidth = 0;
        //private int scaleHeight = 0;

        void Awake()
        {
            //originalHeight = Screen.height;
            //originalWidth = Screen.width;
            MatchQuality();
        }

        public static bool IsLowPowerDevice()
        {
            return IsSDTexture;
        }


        #region 品质匹配
        public void MatchQuality()
        {
            if (Application.platform == RuntimePlatform.Android)//CK只需对android机型做优化
            {
                if ((SystemInfo.processorFrequency != 0 && SystemInfo.processorFrequency < 1250) ||// CPU低于1.2GH
                    (SystemInfo.systemMemorySize != 0 && SystemInfo.systemMemorySize < 1200))// 或者内存低于1G
                {
                    QualityManager.IsSDTexture = true;
                    DebugUtil.LogWarning("Simon 安卓选择低清模式");

                    //低轻模式不再改变设计分辨率
                    //SetDesignContentScale(956, 538);
                }
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)//苹果设备不能获取cpu频率，这里通过GPU型号和内存判断
            {//https://zh.wikipedia.org/wiki/IOS%E5%92%8CiPadOS%E8%AE%BE%E5%A4%87%E5%88%97%E8%A1%A8
#if UNITY_IOS
                var g = UnityEngine.iOS.Device.generation;
                if (g == UnityEngine.iOS.DeviceGeneration.iPhone4 ||// <1G iphone
                    g == UnityEngine.iOS.DeviceGeneration.iPhone4S ||
                    g == UnityEngine.iOS.DeviceGeneration.iPhone5 ||
                    g == UnityEngine.iOS.DeviceGeneration.iPhone5S ||
                    g == UnityEngine.iOS.DeviceGeneration.iPhone5C ||
                    g == UnityEngine.iOS.DeviceGeneration.iPhone6 ||
                    g == UnityEngine.iOS.DeviceGeneration.iPhone6Plus ||
                    g == UnityEngine.iOS.DeviceGeneration.iPad1Gen ||// <1G ipad
                    g == UnityEngine.iOS.DeviceGeneration.iPadMini1Gen ||
                    g == UnityEngine.iOS.DeviceGeneration.iPad2Gen ||
                    g == UnityEngine.iOS.DeviceGeneration.iPadMini2Gen ||
                    g == UnityEngine.iOS.DeviceGeneration.iPad3Gen ||
                    g == UnityEngine.iOS.DeviceGeneration.iPadMini3Gen ||
                    g == UnityEngine.iOS.DeviceGeneration.iPad4Gen ||
                    g == UnityEngine.iOS.DeviceGeneration.iPadAir1 ||// <1G ipadAir
                    g == UnityEngine.iOS.DeviceGeneration.iPodTouch1Gen ||// <1G ipod
                    g == UnityEngine.iOS.DeviceGeneration.iPodTouch2Gen ||
                    g == UnityEngine.iOS.DeviceGeneration.iPodTouch3Gen ||
                    g == UnityEngine.iOS.DeviceGeneration.iPodTouch4Gen ||
                    g == UnityEngine.iOS.DeviceGeneration.iPodTouch5Gen ||
                    g == UnityEngine.iOS.DeviceGeneration.iPodTouch6Gen)
                {
                    QualityManager.IsSDTexture = true;
                    DebugUtil.LogWarning("Simon IOS选择低清模式");
                }
#endif
            }
            Application.targetFrameRate = QualityManager.IsSDTexture ? 30 : 60;
            ResourcesManager.Instance.UseSDAtlas(QualityManager.IsSDTexture);
        }
        #endregion
    }
}