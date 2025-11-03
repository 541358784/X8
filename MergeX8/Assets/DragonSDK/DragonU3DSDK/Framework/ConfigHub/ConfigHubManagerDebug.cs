/************************************************
 * ConfigHubManager
 * If there's any problem, ask yunhan.zeng@dragonplus.com
 ************************************************/
using System.Collections.Generic;

namespace DragonPlus.ConfigHub
{
    public partial class ConfigHubManager
    {
        private readonly List<string> modulesDebug = new List<string>();
        
        // L1
        public string DebugCacheGetRemote()
        {
            getRemoteConfig(true);
            return "ok";
        }
        public string DebugGroupList()
        {
            var s = "Modules:\n";
            for (var i = 0; i < modulesDebug.Count; ++i)
            {
                var moduleGuid = modulesDebug[i];
                s += $"{i} - [{moduleGuid}]:{getModule(moduleGuid).MetaData}\n";
            }
            return s;
        }
        private string DebugCacheL1List(bool cacheStatus = true)
        {
            var s = "Modules:\n";
            for (var i = 0; i < modulesDebug.Count; ++i)
            {
                var moduleGuid = modulesDebug[i];
                if (cacheStatus)
                    s += $"{i} - [{moduleGuid}]:{(cacheModuleExist(moduleGuid) ? "cached" : "null")}\n";
                else
                    s += $"{i} - [{moduleGuid}]\n";
            }
            return s;
        }
        public string DebugCacheL1Get(int idx)
        {
            if (idx < 0 || idx >= modulesDebug.Count)
                return DebugCacheL1List();

            var moduleGuid = modulesDebug[idx];
            var cache = cacheModuleGet(moduleGuid);
            if (cache == null)
                return $"[{moduleGuid}] - null";
            var s = cache.GetString(true);
            ConfigHubUtil.L(s);
            return ConfigHubUtil.DebugString(s);
        }
        private string DebugCacheL1ClearAll()
        {
            foreach (var moduleGuid in modulesDebug)
                cacheModuleClear(moduleGuid, "debug clear all");
            return "L1 all cleared";
        }
        public string DebugCacheL1Clear(int idx)
        {
            if (idx < 0 || idx >= modulesDebug.Count)
                return DebugCacheL1ClearAll();
            cacheModuleClear(modulesDebug[idx], $"debug clear {idx}");
            return $"[{modulesDebug[idx]}] cleared!";
        }

        // L2
        public string DebugCacheL2Get(int idx)
        {
            if (idx < 0 || idx >= modulesDebug.Count)
                return DebugCacheL1List(false);

            return getModule(modulesDebug[idx]).getCacheString();
        }
        public string DebugCacheL2Clear(int idx)
        {
            if (idx < 0 || idx >= modulesDebug.Count)
            {
                var s = "";
                foreach (var module in modulesDebug)
                    s += getModule(module).DebugCacheClear() + "\n";
                return s;
            }
            
            return getModule(modulesDebug[idx]).DebugCacheClear();
        }
    } 
}