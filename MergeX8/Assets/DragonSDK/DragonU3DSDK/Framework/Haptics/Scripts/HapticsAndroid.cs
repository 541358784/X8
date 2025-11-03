// author: yunhan.zeng@dragonplus.com
// date: 2020.12.01

#if UNITY_ANDROID && !UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEngine;

namespace DragonPlus.Haptics
{
    public class HapticsAndroid : HapticsBase
    {
        private int SdkVersion = -1;
        private AndroidJavaClass UnityPlayer;
        private AndroidJavaObject CurrentActivity;
        private AndroidJavaObject AndroidVibrator;
        private AndroidJavaClass VibrationEffectClass;
        private IntPtr AndroidVibrateMethodRawClass = IntPtr.Zero;
        private jvalue[] AndroidVibrateMethodRawClassParameters;
        
        private const long DurationLight = 20L;
        private const long DurationMedium = 40L;
        private const long DurationHeavy = 80L;
        private const int AmplitudeLight = 40;
        private const int AmplitudeMedium = 120;
        private const int AmplitudeHeavy = 255;
        private Dictionary<HapticTypes, long[]> patterns = new Dictionary<HapticTypes, long[]>()
        {
            [HapticTypes.Selection] = new long[] { DurationLight },
            [HapticTypes.Success] = new long[] { 0, DurationLight, DurationLight, DurationHeavy},
            [HapticTypes.Warning] = new long[] { 0, DurationHeavy, DurationLight, DurationMedium},
            [HapticTypes.Failure] = new long[] { 0, DurationMedium, DurationLight, DurationMedium, DurationLight, DurationHeavy, DurationLight, DurationLight},
            [HapticTypes.Light] = new long[] { 0, DurationLight },
            [HapticTypes.Medium] = new long[] { 0, DurationMedium },
            [HapticTypes.Heavy] = new long[] { 0, DurationHeavy },
        };
        private Dictionary<HapticTypes, int[]> amplitudes = new Dictionary<HapticTypes, int[]>()
        {
            [HapticTypes.Selection] = new int[] { AmplitudeLight },
            [HapticTypes.Success] = new int[] { 0, AmplitudeLight, 0, AmplitudeHeavy},
            [HapticTypes.Warning] = new int[] { 0, AmplitudeHeavy, 0, AmplitudeMedium},
            [HapticTypes.Failure] = new int[] { 0, AmplitudeMedium, 0, AmplitudeMedium, 0, AmplitudeHeavy, 0, AmplitudeLight},
            [HapticTypes.Light] = new int[] { 0, AmplitudeLight },
            [HapticTypes.Medium] = new int[] { 0, AmplitudeMedium },
            [HapticTypes.Heavy] = new int[] { 0, AmplitudeHeavy },
        };

        private void AndroidVibrate(long[] pattern, int[] amplitude, int repeat)
        {
            if (AndroidVibrator == null)
                return;
            
            if ((SdkVersion < 26) || VibrationEffectClass == null)
            { 
                AndroidVibrator.Call ("vibrate", pattern, repeat);
            }
            else
            {
                var VibrationEffect = VibrationEffectClass.CallStatic<AndroidJavaObject> ("createWaveform", new object[] {pattern, amplitude, repeat});
                AndroidVibrator.Call ("vibrate", VibrationEffect);
            }
        }

        public override bool Init()
        {
            if (!base.Init())
                return true;
            
            try
            {
                SdkVersion = int.Parse (SystemInfo.operatingSystem.Substring(SystemInfo.operatingSystem.IndexOf("-") + 1, 3));
                UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                CurrentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidVibrator = CurrentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
                AndroidVibrateMethodRawClass = AndroidJNIHelper.GetMethodID(AndroidVibrator.GetRawClass(), "vibrate", "(J)V", false);
                AndroidVibrateMethodRawClassParameters = new jvalue[1];
                VibrationEffectClass = new AndroidJavaClass ("android.os.VibrationEffect");
                
                Avaiable = true;
                IsHapticsSupported = true;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"cannot init haptics:{e}");
                Avaiable = false;
                IsHapticsSupported = false;
            }
            
            return true;
        }
        
        public override bool Release()
        {
            if (!base.Release())
                return true;
            
            SdkVersion = -1;
            UnityPlayer = null;
            CurrentActivity = null;
            AndroidVibrator = null;
            AndroidVibrateMethodRawClass = IntPtr.Zero;
            AndroidVibrateMethodRawClassParameters = null;
            VibrationEffectClass = null;
            
            Avaiable = false;
            
            return true;
        }
        
        public override void Vibrate(long milliseconds)
        {
            if (AndroidVibrator == null || AndroidVibrateMethodRawClass == IntPtr.Zero || AndroidVibrateMethodRawClassParameters == null)
                return;
            
            AndroidVibrateMethodRawClassParameters[0].j = milliseconds;
            AndroidJNI.CallVoidMethod(AndroidVibrator.GetRawObject(), AndroidVibrateMethodRawClass, AndroidVibrateMethodRawClassParameters);
        }
        
        public override void Haptics(HapticTypes type)
        {
	        if (type == HapticTypes.None)
		        return;

            if (!patterns.ContainsKey(type))
            {
                Vibrate(200);
                return;
            }
            
            AndroidVibrate(patterns[type], amplitudes[type], -1);
        }
    }
}

#endif