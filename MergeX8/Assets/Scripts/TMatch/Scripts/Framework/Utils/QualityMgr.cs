/*----------------------------------------------------------------
// Copyright (C) 2019 北京，天龙互娱
//
// 模块名：QualityMgr
// 创建日期：2019-1-29
// 创建者：waicheng.wang
// 模块描述：管理显示品质和性能
//----------------------------------------------------------------*/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using DragonU3DSDK.Asset;

namespace TMatch
{


    public class QualityMgr : MonoBehaviour
    {
#if UNITY_EDITOR
        public static QualityLevel Level = QualityLevel.High;
#else
    public static QualityLevel Level = QualityLevel.Medium;
#endif

        public static QualityMgr Instance;

        private int originalHeight;
        private int originalWidth;

        private int scaleWidth = 0;
        private int scaleHeight = 0;


        public enum QualityLevel
        {
            VeryLow = 0,
            Low,
            Medium,
            High,
            VeryHigh,
            // MobileHigh,
            // Ultra
        }

        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            originalHeight = Screen.height;
            originalWidth = Screen.width;
            MatchQuality();
        }

        #region 品质匹配

        public void MatchQuality()
        {
            var memLevel = QualityLevel.High;
            // var cpuLevel = QualityLevel.High;

            if (Application.platform == RuntimePlatform.Android)
            {
                //memLevel
                if (SystemInfo.systemMemorySize != 0 && SystemInfo.systemMemorySize < 2000) //2G及以下
                {
                    memLevel = QualityLevel.VeryLow;
                }
                else if (SystemInfo.systemMemorySize != 0 && SystemInfo.systemMemorySize < 2100) //暂时不用
                {
                    memLevel = QualityLevel.Low;
                }
                else if (SystemInfo.systemMemorySize != 0 && SystemInfo.systemMemorySize < 3000) //3G及以下
                {
                    memLevel = QualityLevel.Medium;
                }
                else if (SystemInfo.systemMemorySize != 0 && SystemInfo.systemMemorySize < 5000) //5G及以下
                {
                    // memLevel = QualityLevel.High;
                    memLevel = QualityLevel.Medium;
                }
                else
                {
                    // memLevel = QualityLevel.VeryHigh;
                    memLevel = QualityLevel.High;
                }

                //cpuLevel
                /*var cpuFrequency = SystemInfo.processorFrequency;
                if (MathUtil.Between(cpuFrequency, 0, 1))
                {
                    cpuLevel = QualityLevel.VeryLow;
                }
                else if (MathUtil.Between(cpuFrequency, 1, 2))
                {
                    cpuLevel = QualityLevel.Low;
                }
                else if (MathUtil.Between(cpuFrequency, 2, 3))
                {
                    cpuLevel = QualityLevel.Medium;
                }
                else if (MathUtil.Between(cpuFrequency, 3, 4))
                {
                    cpuLevel = QualityLevel.High;
                }
                else
                {
                    cpuLevel = QualityLevel.VeryHigh;
                }*/
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer) //苹果设备不能获取cpu频率，这里通过GPU型号和内存判断
            {
#if UNITY_IOS
            var g = UnityEngine.iOS.Device.generation;
            if (g == UnityEngine.iOS.DeviceGeneration.iPhone4 || // <1G iphone
                g == UnityEngine.iOS.DeviceGeneration.iPhone4S ||
                g == UnityEngine.iOS.DeviceGeneration.iPhone5 ||
                g == UnityEngine.iOS.DeviceGeneration.iPhone5S ||
                g == UnityEngine.iOS.DeviceGeneration.iPhone5C ||
                g == UnityEngine.iOS.DeviceGeneration.iPhone6 ||
                g == UnityEngine.iOS.DeviceGeneration.iPhone6Plus ||
                g == UnityEngine.iOS.DeviceGeneration.iPad1Gen || // <1G ipad
                g == UnityEngine.iOS.DeviceGeneration.iPadMini1Gen ||
                g == UnityEngine.iOS.DeviceGeneration.iPad2Gen ||
                g == UnityEngine.iOS.DeviceGeneration.iPadMini2Gen ||
                g == UnityEngine.iOS.DeviceGeneration.iPad3Gen ||
                g == UnityEngine.iOS.DeviceGeneration.iPadMini3Gen ||
                g == UnityEngine.iOS.DeviceGeneration.iPad4Gen ||
                g == UnityEngine.iOS.DeviceGeneration.iPadAir1 || // <1G ipadAir
                g == UnityEngine.iOS.DeviceGeneration.iPodTouch1Gen || // <1G ipod
                g == UnityEngine.iOS.DeviceGeneration.iPodTouch2Gen ||
                g == UnityEngine.iOS.DeviceGeneration.iPodTouch3Gen ||
                g == UnityEngine.iOS.DeviceGeneration.iPodTouch4Gen ||
                g == UnityEngine.iOS.DeviceGeneration.iPodTouch5Gen ||
                g == UnityEngine.iOS.DeviceGeneration.iPodTouch6Gen)
            {
                memLevel = QualityLevel.VeryLow;
            }
            else if (g == UnityEngine.iOS.DeviceGeneration.iPhone6S || 
                     g == UnityEngine.iOS.DeviceGeneration.iPhone6SPlus ||
                     g == UnityEngine.iOS.DeviceGeneration.iPhone7 ||
                     g == UnityEngine.iOS.DeviceGeneration.iPhone7Plus ||
                     g == UnityEngine.iOS.DeviceGeneration.iPodTouch7Gen ||
                     g == UnityEngine.iOS.DeviceGeneration.iPad5Gen ||
                     g == UnityEngine.iOS.DeviceGeneration.iPad6Gen ||
                     g == UnityEngine.iOS.DeviceGeneration.iPadMini4Gen ||
                     g == UnityEngine.iOS.DeviceGeneration.iPadAir2 ||
                     g == UnityEngine.iOS.DeviceGeneration.iPadPro1Gen
            )
            {
                memLevel = QualityLevel.Medium;
            }
            else
            {
                // memLevel = QualityLevel.VeryHigh;
                memLevel = QualityLevel.High;
            }
#endif
            }

            // Level = (QualityLevel)Mathf.Min((int)memLevel, (int)cpuLevel);
            Level = memLevel;

            FitDeviceLevel();
        }

        public void FitDeviceLevel()
        {
            var atlasUseSD = Level <= QualityLevel.Medium;
#if UNITY_ANDROID
            atlasUseSD = Level < QualityLevel.Medium;
#endif

            if (atlasUseSD)
            {
                SetDesignContentScale(956, 538);
            }

            ResourcesManager.Instance.UseSDAtlas(atlasUseSD);

            QualitySettings.SetQualityLevel((int) Level);
        }

        #endregion

        #region 分辨率调整

        public void SetDesignContentScale(int designWidth, int designHeight)
        {
            if (scaleWidth == 0 && scaleHeight == 0)
            {
                int width = Screen.currentResolution.width;
                int height = Screen.currentResolution.height;
                float s1 = (float) designWidth / (float) designHeight;
                float s2 = (float) width / (float) height;
                if (s1 < s2)
                {
                    designWidth = (int) Mathf.FloorToInt(designHeight * s2);
                }
                else if (s1 > s2)
                {
                    designHeight = (int) Mathf.FloorToInt(designWidth / s2);
                }

                float contentScale = (float) designWidth / (float) width;
                if (contentScale < 1.0f)
                {
                    scaleWidth = designWidth;
                    scaleHeight = designHeight;
                }
            }

            if (scaleWidth > 0 && scaleHeight > 0)
            {
                if (scaleWidth % 2 == 0)
                {
                    scaleWidth += 1;
                }
                else
                {
                    scaleWidth -= 1;
                }

                Screen.SetResolution(scaleWidth, scaleHeight, true);
            }
        }

        #endregion
    }
}