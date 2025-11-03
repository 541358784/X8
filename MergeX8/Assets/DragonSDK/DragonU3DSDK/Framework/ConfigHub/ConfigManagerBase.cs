/************************************************
 * ConfigManagerBase
 * If there's any problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using DragonU3DSDK;
using DragonU3DSDK.Storage;
using Facebook.Unity;

namespace DragonPlus.ConfigHub
{
    public enum CacheOperate
    {
        None = 0,
        UseCache = 1,
        UpdateCache = 2
    }
    public abstract partial class ConfigManagerBase
    {
        public abstract string Guid { get; }
        public abstract int VersionMinIOS { get; }
        public abstract int VersionMinAndroid { get; }
        protected abstract List<string> SubModules { get; }
        public MetaData MetaData;
        public bool IsRemote;
        public bool IsLoaded;
        
        protected abstract bool CheckTable(Hashtable table);
        protected abstract bool HasGroup(int groupId);
        public abstract List<T> GetConfig<T>(CacheOperate cacheOp = CacheOperate.None, long cacheDuration = 0);
        public abstract void InitConfig(MetaData metaData, string jsonData = null);

        public void Init()
        {
            if (!ConfigHubManager.Instance.IsRegistered(this))
                ConfigHubManager.Instance.Register(this);
            
            checkCache();
        }

        public void Release()
        {
            if (ConfigHubManager.Instance.IsRegistered(this))
                ConfigHubManager.Instance.Unregister(this);
        }

        public int GetCacheGroupId(bool log)
        {
            return getCache(log)?.UserGroup ?? -1;
        }

        protected MetaData GetMetaDataCached()
        {
            var cache = getCache(true);
            if (cache == null)
                return null;
            return new MetaData {
                VersionIOS = cache.VersionIOS,
                VersionAndroid = cache.VersionAndroid,
                GroupId = cache.UserGroup
            };
        }

        protected virtual MetaData GetMetaDataDefault()
        {
            return new MetaData
            {
                VersionIOS = VersionMinIOS,
                VersionAndroid = VersionMinAndroid,
                GroupId = ConfigHubUtil.DefaultGroupId
            };
        }
        
        // cache
        private void checkCache()
        {
            var cache = getCache();
            if (cache == null)
                return;
            // 失效
            if (!ConfigHubManager.Instance.CacheExist(cache.Module, cache.UserGroup) && !HasGroup(cache.UserGroup))
            {
                clearCache($"the group({cache.UserGroup}) is not available, remove it.");
            }
        }

        // 之前固化的id和服务器指定的id是否有变化
        public bool IsGroupIdChanged(bool log)
        {
            var groupIdCached = GetCacheGroupId(log);
            if (groupIdCached < 0)
                return false;
            var groupIdRemote = ConfigHubManager.Instance.GetGroupIdSpecified(Guid);
            if (groupIdRemote < 0)
                return false;
            if (log) ConfigHubUtil.L($"IsGroupIdChanged:groupIdCached({groupIdCached})/groupIdRemote({groupIdRemote})");
            return groupIdCached != groupIdRemote;
        }
        
        // 获取当前固化缓存的创建时间
        public long GetCacheCreateTime()
        {
            var cache = getCache();
            if (cache == null)
                return -1;
            return cache.CreateTime;
        }

        private StorageConfigHub getCache(bool log = false)
        {
            var cacheModules = StorageManager.Instance.GetStorage<StorageCommon>().ConfigHub;
            if (!cacheModules.ContainsKey(Guid))
                return null;

            var cache = cacheModules[Guid];
            // 版本号过低
            var cacheVersion = ConfigHubUtil.GetCurrentVersion(cache.VersionIOS, cache.VersionAndroid);
            var minVersion = ConfigHubUtil.GetCurrentVersion(VersionMinIOS, VersionMinAndroid);
            if (cacheVersion < minVersion)
            {
                clearCache($"version out of date, remove it.(v{cacheVersion} < v{minVersion})");
                return null;
            }
            // 过期
            if (cache.Duration > 0 && Utils.GetTimeStamp() > cache.CreateTime + cache.Duration)
            {
                clearCache($"version out of time, remove it.");
                return null;
            }
            if (log) ConfigHubUtil.L($"load cacheL2:{getCacheString()}");
            
            return cache;
        }
        
        protected void processCache(CacheOperate op, long cacheDuration)
        {
            var cache = getCache();
            MetaData metaData;
            switch (op)
            {
                case CacheOperate.None: return;
                case CacheOperate.UseCache:
                    if (cacheDuration == 0)
                        return;
                    // 若cache存在，则说明其有效，无需更新
                    if (cache != null)
                        return;
                    // 使用当前metaData固化
                    metaData = MetaData;
                    break;
                case CacheOperate.UpdateCache:
                    // 若cache存在，则先清除
                    if (cache != null)
                        clearCache($"remove cache when update it manually.");
                    // 变更为无缓存则直接返回
                    if (cacheDuration == 0)
                        return;
                    // 从当前服务器指定的分组信息获取或使用默认分组信息
                    metaData = ConfigHubManager.Instance.GetMetaDataCached(Guid) ?? GetMetaDataDefault();
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(op), op, null);
            }
            
            var cacheModules = StorageManager.Instance.GetStorage<StorageCommon>().ConfigHub;
            cacheModules[Guid] = new StorageConfigHub
            {
                Module = Guid,
                VersionIOS = metaData.VersionIOS,
                VersionAndroid = metaData.VersionAndroid,
                UserGroup = metaData.GroupId,
                CreateTime = Utils.GetTimeStamp(),
                Duration = cacheDuration
            };
            ConfigHubUtil.L($"save cacheL2:{getCacheString()}");
            
            switch (op)
            {
                case CacheOperate.UpdateCache:
                    // 重新加载配置表
                    ConfigHubManager.Instance.LoadConfig(Guid);
                    break;
            }
        }

        private string clearCache(string reason = "unknown")
        {
            var cacheModules = StorageManager.Instance.GetStorage<StorageCommon>().ConfigHub;
            if (!cacheModules.ContainsKey(Guid))
                return $"the cacheL2 of {Guid} is not exist";
            cacheModules.Remove(Guid);
            var s = $"remove cacheL2:[{Guid}] - {reason}";
            ConfigHubUtil.L(s);
            return s;
        }
    }
}