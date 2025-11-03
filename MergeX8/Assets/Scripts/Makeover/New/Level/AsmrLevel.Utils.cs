using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using DG.Tweening;
using DragonPlus.Haptics;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Serialization;

namespace MiniGame
{
    public partial class AsmrLevel
    {
        public static void VibrationShort()
        {
#if UNITY_ANDROID
            HapticsManager.Vibrate(10);
            //VibrationAndroid(10);
#else
            HapticsManager.Haptics(HapticTypes.Light);
#endif
        }

        public static void VibrationShortFix()
        {
            var detal = 10;
#if UNITY_ANDROID
            HapticsManager.Vibrate(10);
            //VibrationAndroid(detal);
#else
            HapticsManager.Haptics(HapticTypes.Light);
#endif
        }

        // public static void StartLongVibration()
        // {
        //     HapticsManager.Vibrate(100000);
        // }

        private static AndroidJavaObject _vib;

        private static void VibrationAndroid(long vibrateTime)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (_vib == null)
            {
                AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                var activity = jc.GetStatic<AndroidJavaObject>("currentActivity");
                var service = new AndroidJavaClass("android.app.Service");
                var s = service.GetStatic<string>("VIBRATOR_SERVICE");
                _vib = activity.Call<AndroidJavaObject>("getSystemService", s);
            }

            _vib.Call("vibrate", vibrateTime);
#endif
        }

        public void ResetAllNormals()
        {
            gameObject_fake.gameObject.SetActive(true);
            
            InitMonos(gameObject_fake);

        }

        private void HideNode_Exit(AsmrStep step)
        {
            if (step == null) return;
            if (step.Config.hidePaths_Exit == null) return;

            if (step.Config.hidePaths_Exit.Length > 0)
            {
                foreach (var path in step.Config.hidePaths_Exit)
                {
                    var node = transform.Find(path);
                    if (node) node.gameObject.SetActive(false);
                }
            }
        }
    }
}