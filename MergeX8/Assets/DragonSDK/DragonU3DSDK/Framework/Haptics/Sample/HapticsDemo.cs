// author: yunhan.zeng@dragonplus.com
// date: 2020.12.01

using UnityEngine;
using DragonPlus.Haptics;

namespace DragonPlus
{
    public class HapticsDemo : MonoBehaviour
    {
        void Awake()
        {
            HapticsManager.Init();
        }
        
        void OnDestroy()
        {
            HapticsManager.Release();
        }

        public void OnClickVibrate()
        {
            HapticsManager.Vibrate(500);
        }
        
        public void OnClickHaptics(int type)
        {
            HapticsManager.Haptics((HapticTypes)type);
        }
    }
}