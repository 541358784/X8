/************************************************
 * ConfigHub Util class : ConfigHubUtil
 * If there's any problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;
using DragonU3DSDK;

namespace DragonPlus.ConfigHub
{
    public static class ConfigHubUtil
    {
        public const int DefaultGroupId = 0;
        public static void L(string log) { DebugUtil.Log($"ConfigHub -> {log}"); }
        public static void E(string log) { DebugUtil.LogError($"ConfigHub -> {log}"); }
        
        public static int GetCurrentVersion(int versionIOS, int versionAndroid)
        {
#if UNITY_ANDROID
            return versionAndroid;
#else
            return versionIOS;
#endif
        }
        
        public static string GetCacheModuleKey(string guid)
        {
            return $"ConfigHub_{guid}";
        }
        
        public static void AddAllKVPFrom<T1, T2>(
            this IDictionary<T1, T2> dest,
            IDictionary<T1, T2> source)
        {
            foreach (var key in source.Keys)
                dest[key] = source[key];
        }
        
        public static string DebugString(string s)
        {
            if (string.IsNullOrEmpty(s))
                return "";
            if (s.Length > 200)
                return s.Substring(0, 200) + "...";
            return s;
        }
    } 
}