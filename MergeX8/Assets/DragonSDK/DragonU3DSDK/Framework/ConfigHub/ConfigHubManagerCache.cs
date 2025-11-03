/************************************************
 * ConfigHubManager
 * If there's any problem, ask yunhan.zeng@dragonplus.com
 ************************************************/
using System.Collections.Generic;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Zlib;
using UnityEngine;
using Newtonsoft.Json;
using DragonU3DSDK.Util;

namespace DragonPlus.ConfigHub
{
    public partial class ConfigHubManager
    {
        private class CacheGroup
        {
            public MetaData MetaData;
            public string JsonData;

            public override string ToString()
            {
                return GetString();
            }
            public string GetString(bool withJson = false)
            {
                return $"{MetaData} {(withJson ? $"json:\n{JsonData}" : "")}";
            }
        }
        private class CacheModule
        {
            public string Guid;
            // 服务器分配的分组id
            public int GroupIdSpecified;
            public readonly Dictionary<int, CacheGroup> Groups = new Dictionary<int, CacheGroup>();

            public CacheGroup GetCacheGroupSpecified()
            {
                return Groups.ContainsKey(GroupIdSpecified) ? Groups[GroupIdSpecified] : null;
            }
            public override string ToString()
            {
                return GetString();
            }
            public string GetString(bool withJson = false)
            {
                var s = $"[{Guid}]\n";
                foreach (var kv in Groups)
                    s += kv.Value.GetString(withJson) + "\n";
                return s;
            }
        }
        private readonly Dictionary<string, CacheModule> cacheModules = new Dictionary<string, CacheModule>();

        public int GetGroupIdSpecified(string guid)
        {
            var cache = cacheModuleGet(guid);
            if (cache == null)
                return -1;
            return cache.GroupIdSpecified;
        }
        
        public MetaData GetMetaDataCached(string guid)
        {
            var module = getModule(guid);
            if (module == null)
                return null;
            var cache = cacheModuleGet(guid);
            var cacheGroup = cache?.GetCacheGroupSpecified();
            return cacheGroup?.MetaData;
        }
        
        // module
        private bool cacheModuleExist(string guid)
        {
            var cacheKey = ConfigHubUtil.GetCacheModuleKey(guid);
            return cacheModules.ContainsKey(cacheKey) || PlayerPrefs.HasKey(cacheKey);
        }
        private void cacheModuleClear(string guid, string reason = "unknown")
        {
            var cacheKey = ConfigHubUtil.GetCacheModuleKey(guid);
            if (cacheModules.ContainsKey(cacheKey))
                cacheModules.Remove(cacheKey);
            if (PlayerPrefs.HasKey(cacheKey))
                PlayerPrefs.DeleteKey(cacheKey);
            ConfigHubUtil.L($"remove cacheL1:[{guid}] - {reason}");
        }
        private CacheModule cacheModuleGet(string guid)
        {
            var cacheKey = ConfigHubUtil.GetCacheModuleKey(guid);
            // load from mem
            if (!cacheModules.ContainsKey(cacheKey))
            {
                // try load from disk
                if (!PlayerPrefs.HasKey(cacheKey))
                    return null;
                var encryptData = System.Convert.FromBase64String(PlayerPrefs.GetString(cacheKey));
                var cacheString = RijndaelManager.Instance.DecryptStringFromBytes(encryptData);
                var cacheModule = JsonConvert.DeserializeObject<CacheModule>(cacheString);
                if (cacheModule == null)
                    return null;
                // hold in mem
                cacheModules[cacheKey] = cacheModule;
            }
            return cacheModules[cacheKey];
        }
        
        // group
        public bool CacheExist(string guid, int groupId)
        {
            var cacheModule = cacheModuleGet(guid);
            return cacheModule != null && cacheModule.Groups.ContainsKey(groupId);
        }
        private CacheGroup cacheGet(string guid, int minVersionIOS, int minVersionAndroid, int groupId, bool log)
        {
            var cacheModule = cacheModuleGet(guid);
            if (cacheModule == null)
                return null;
            // 若外部传入的id小于0，则说明没有固化该id，则使用服务器指定的分组id
            groupId = groupId < 0 ? cacheModule.GroupIdSpecified : groupId;
            if (!cacheModule.Groups.ContainsKey(groupId))
                return null;
            var cache = cacheModule.Groups[groupId];
            var minVersion = ConfigHubUtil.GetCurrentVersion(minVersionIOS, minVersionAndroid);
            var minVersionCache = ConfigHubUtil.GetCurrentVersion(cache.MetaData.VersionIOS, cache.MetaData.VersionAndroid);
            // 缓存版本号低于当前最低版本号则不使用
            if (minVersionCache < minVersion)
            {
                cacheModuleClear(guid, $"version out of date.(v{minVersionCache} < v{minVersion})");
                return null;
            }
            if (log) ConfigHubUtil.L($"load cacheL1:[{guid}] - {cache.GetString(true)}");
            return cache;
        }
        private bool cacheUpdate(string guid, int minVersionIOS, int minVersionAndroid, MetaData metaData, string jsonDataRemote, bool isRemoteSpecify)
        {
            if (metaData == null || string.IsNullOrEmpty(jsonDataRemote))
                return false;
            var minVersion = ConfigHubUtil.GetCurrentVersion(minVersionIOS, minVersionAndroid);
            var minVersionRemote = ConfigHubUtil.GetCurrentVersion(metaData.VersionIOS, metaData.VersionAndroid);
            // 服务器版本号低于当前最低版本号则不使用
            if (minVersionRemote < minVersion)
                return false;
            var cacheGroup = cacheGet(guid, minVersionIOS, minVersionAndroid, metaData.GroupId, false);
            if (cacheGroup != null)
            {
                var minVersionCache = ConfigHubUtil.GetCurrentVersion(cacheGroup.MetaData.VersionIOS, cacheGroup.MetaData.VersionAndroid);
                // 服务器版本号低于当前缓存的最低版本号则不使用
                if (minVersionRemote < minVersionCache)
                    return false;
                // 缓存的版本和服务器版本一致，则直接使用
                // if (minVersionCache == minVersionRemote && cacheGroup.JsonData == jsonDataRemote)
                //     return true;
            }
            // 创建或更新缓存
            cacheGroup = new CacheGroup {
                MetaData = metaData,
                JsonData = jsonDataRemote
            };
            
            var cacheKey = ConfigHubUtil.GetCacheModuleKey(guid);
            if (!cacheModules.ContainsKey(cacheKey))
                cacheModules[cacheKey] = new CacheModule{Guid = guid};
            var cacheModule = cacheModules[cacheKey];
            cacheModule.Groups[cacheGroup.MetaData.GroupId] = cacheGroup;
            // 更新服务器分组
            if (isRemoteSpecify)
                cacheModule.GroupIdSpecified = cacheGroup.MetaData.GroupId;
            
            var cacheString = JsonConvert.SerializeObject(cacheModule);
            var encryptData = RijndaelManager.Instance.EncryptStringToBytes(cacheString);
            PlayerPrefs.SetString(cacheKey, System.Convert.ToBase64String(encryptData));
            ConfigHubUtil.L($"save cacheL1:[{guid}] - groupIdSpecified:{cacheModule.GroupIdSpecified}, group:{cacheGroup.GetString(true)}");
            return true;
        }
    } 
}