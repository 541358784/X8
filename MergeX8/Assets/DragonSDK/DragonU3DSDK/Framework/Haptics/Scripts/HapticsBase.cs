// author: yunhan.zeng@dragonplus.com
// date: 2020.12.01

using UnityEngine;

namespace DragonPlus.Haptics
{
    public enum HapticTypes { Selection, Success, Warning, Failure, Light, Medium, Heavy, None }
    
    public class HapticsBase
    {
        public bool init = false;
        public bool Avaiable = false;
        public bool IsHapticsSupported = true;

        public virtual bool Init() 
        {
            if (init)
                return false;

            init = true;

            return true;
        }

        public virtual bool Release()
        {
            if (!init)
                return false;

            init = false;
            
            return true;
        }

        public virtual void Vibrate(long milliseconds)
        {
#if UNITY_IOS || UNITY_ANDROID
            // unity default vibrate
            Handheld.Vibrate();
#endif
        }
        public virtual void Haptics(HapticTypes type) {}
    }
}