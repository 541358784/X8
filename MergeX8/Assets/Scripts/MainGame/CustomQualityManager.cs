using Decoration.DynamicMap;
using DragonU3DSDK.Asset;
using UnityEngine;

namespace Gameplay
{
    public class CustomQualityManager : MonoBehaviour
    {
        private bool atlasUseSD = false; //是否使用低清贴图

        void Start()
        {
            MatchQuality();
        }

        public void MatchQuality()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                //memLevel
                if (SystemInfo.systemMemorySize != 0)
                {
                    if (SystemInfo.systemMemorySize < 3000) //3g 以下
                        atlasUseSD = true;
                }
                
                if (SystemInfo.processorFrequency != 0 && SystemInfo.processorFrequency < 1250)// CPU低于1.2GH
                {
                    atlasUseSD = true;
                }
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
                    atlasUseSD = true;
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
                    atlasUseSD = true;
                }
#endif
            }

#if DEVELOPMENT_BUILD
            int value = PlayerPrefs.GetInt("useSdAtlas");
            if (value == 1)
            {  
               atlasUseSD = true;
            }
#endif
            ResourcesManager.Instance.UseSDAtlas(atlasUseSD);
            
            Application.targetFrameRate = atlasUseSD ? 30 : 60;
            if (atlasUseSD)
            {
                ResourcesManager.Instance.SetBundleCacheDuration(2);
                DynamicMapManager.Instance.SetUnLoadTime(1.5f);
                DynamicMapManager.Instance.SetDailyLoadCount(2);
                DynamicMapManager.Instance.SetAtlasConfig(6, 2f);
            }
            else
            {
                ResourcesManager.Instance.SetBundleCacheDuration(2);
                DynamicMapManager.Instance.SetUnLoadTime(2);
                DynamicMapManager.Instance.SetDailyLoadCount(4);
                DynamicMapManager.Instance.SetAtlasConfig(6, 2f);
            }
        }
    }
}