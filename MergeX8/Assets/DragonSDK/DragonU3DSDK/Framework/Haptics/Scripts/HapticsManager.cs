// author: yunhan.zeng@dragonplus.com
// date: 2020.12.01

namespace DragonPlus.Haptics
{
    public static class HapticsManager
    {
        public static bool Active { set; get; }

        public static bool Avaiable
        {
            get
            {
                if (handler == null)
                    return false;
                
                return handler.Avaiable;
            }
        }

        private static HapticsBase handler = null;

        private static HapticsBase Handler
        {
            get
            {
                if (handler == null)
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                    handler = new HapticsAndroid();
#elif UNITY_IOS && !UNITY_EDITOR
                    handler = new HapticsiOS();
#endif
                }
                return handler;
            }
        }
        
        public static void Init()
        {
            Active = true;
            
            Handler?.Init();
        }
        
        public static void Release()
        {
            Handler?.Release();
        }
        
        public static void Vibrate(long milliseconds)
        {
            if (!Active)
                return;
            
            Handler?.Vibrate(milliseconds);
        }
        
        public static void Haptics(HapticTypes type)
        {
            if (!Active)
                return;
            
            Handler?.Haptics(type);
        }

        /// <summary>
        /// 是否支持触觉反馈
        /// </summary>
        /// <returns></returns>
        public static bool IsHapticsSupported()
        {
            if (handler == null) return false;
            return handler.IsHapticsSupported;
        }
    }
}