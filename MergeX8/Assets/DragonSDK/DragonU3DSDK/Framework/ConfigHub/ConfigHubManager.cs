/************************************************
 * ConfigHubManager
 * If there's any problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Network.RemoteConfig;

namespace DragonPlus.ConfigHub
{
    public class MetaData
    {
        public int VersionIOS;
        public int VersionAndroid;
        public int GroupId;
        public override string ToString()
        {
            return $"(ios<{VersionIOS}> android<{VersionAndroid}> group<{GroupId}>)";
        }
    }
    
    public partial class ConfigHubManager : Manager<ConfigHubManager>
    {
        private class ConfigHubData
        {
            public readonly Dictionary<string, MetaData> metaData = new Dictionary<string, MetaData>();
            private readonly Dictionary<string, string> jsonData = new Dictionary<string, string>();
            public ConfigHubData(SGetSegmentationConfig src)
            {
                foreach (var kv in src.ConfigData)
                    jsonData[kv.Key] = kv.Value;
                foreach (var kv in src.GroupIdData)
                {
                    if (!src.VersionCode.ContainsKey(kv.Key))
                        continue;
                    var versionInfo = src.VersionCode[kv.Key];
                    metaData[kv.Key] = new MetaData
                    {
                        VersionIOS = (int)versionInfo.IosVersion,
                        VersionAndroid = (int)versionInfo.AndroidVersion,
                        GroupId = (int)kv.Value,
                    };
                }
            }
            public string GetData(string guid)
            {
                return jsonData.ContainsKey(guid) ? jsonData[guid] : "";
            }
            public MetaData GetMetaData(string guid)
            {
                return metaData.ContainsKey(guid) ? metaData[guid] : null;
            }
        }
        
        private readonly Dictionary<string, ConfigManagerBase> modules = new Dictionary<string, ConfigManagerBase>();
        private ConfigManagerBase getModule(string guid)
        {
            return modules.ContainsKey(guid) ? modules[guid] : null;
        }
        
        public bool IsRegistered(ConfigManagerBase cfgMgr)
        {
            return modules.ContainsKey(cfgMgr.Guid);
        }
        
        public void Register(ConfigManagerBase cfgMgr)
        {
            if (modules.ContainsKey(cfgMgr.Guid))
            {
                ConfigHubUtil.E($"register {cfgMgr.Guid} to config hub repeated.");
                return;
            }
            modules[cfgMgr.Guid] = cfgMgr;
            modulesDebug.Add(cfgMgr.Guid);
        }
        
        public void Unregister(ConfigManagerBase cfgMgr)
        {
            if (!modules.ContainsKey(cfgMgr.Guid))
            {
                ConfigHubUtil.E($"unregister {cfgMgr.Guid} from config hub cannot be found.");
                return;
            }
            modules.Remove(cfgMgr.Guid);
            modulesDebug.Remove(cfgMgr.Guid);
        }
        
        public void Init()
        {
            foreach (var kv in modules)
                loadConfig(kv.Value, true);
            // 启动游戏的时候全量更新一次
            getRemoteConfig(true);
        }

        public void LoadConfig(string guid)
        {
            var module = getModule(guid);
            if (module == null)
                return;
            loadConfig(module, true);
        }
        
        public void CheckRemoteConfig()
        {
            getRemoteConfig(false);
        }

        public Dictionary<string, string> GetStatusInfo()
        {
            var ret = new Dictionary<string, string>();
            foreach (var kv in modules)
            {
                if (kv.Value?.MetaData == null)
                    continue;
                var groupId = kv.Value.MetaData.GroupId;
                // if (groupId == ConfigHubUtil.DefaultGroupId)
                //     continue;
                ret[kv.Key] = groupId.ToString();
            }
            return ret;
        }

        private Dictionary<string, uint> getRequestArg()
        {
            var configs = new Dictionary<string, uint>();
            foreach (var kv in modules)
            {
                var guid = kv.Key;
                // 筛选出已固化但是还未从服务器获取的配置
                var groupId = kv.Value.GetCacheGroupId(false);
                // 没有cache，不需要拉取
                if (groupId < 0)
                    continue;
                // 该组已存在，不需要拉取
                if (CacheExist(guid, groupId))
                    continue;
                configs.Add(guid, (uint)groupId);
            }
            return configs;
        }
        
        private void getRemoteConfig(bool all)
        {
            if (all)
                UserGroupConfigManager.Instance.Request(modules.Keys.ToList(),
                    (success, config) => { onRemoteResponse(success, config, true); }
                );
            else
                UserGroupConfigManager.Request(getRequestArg(), 
                    (success, config) => { onRemoteResponse(success, config, false); }
                );
        }
        
        private void onRemoteResponse(bool success, SGetSegmentationConfig config, bool isRemoteSpecify)
        {
            if (!success)
                return;

            var configHubData = new ConfigHubData(config);
            foreach (var kv in configHubData.metaData)
            {
                var guid = kv.Key;
                var module = getModule(guid);
                if (module == null)
                {
                    ConfigHubUtil.E($"the {guid} is unregister when initial from server.");
                    continue;
                }
                // 处理缓存
                if (!cacheUpdate(guid, module.VersionMinIOS, module.VersionMinAndroid, 
                    configHubData.GetMetaData(guid), configHubData.GetData(guid), isRemoteSpecify))
                    continue;

                loadConfig(module, false);
            }
            DragonU3DSDK.EventManager.Instance.Trigger<DragonU3DSDK.SDKEvents.ConfigHubUpdatedEvent>().Trigger();
            
            // 若是首次拉取结束，则还会尝试拉取固化了但缺失一级缓存的配置
            if (isRemoteSpecify)
                getRemoteConfig(false);
        }

        private void loadConfig(ConfigManagerBase module, bool log)
        {
            // 根据模块分组信息加载对应配置表
            var cacheGroup = cacheGet(module.Guid, module.VersionMinIOS, module.VersionMinAndroid, module.GetCacheGroupId(log), log);
            // 缓存中没有则用保底配置（基本不可能）
            module.InitConfig(cacheGroup?.MetaData, cacheGroup?.JsonData);
        }
        
        //////////////////////////////////////////
        // 自动初始化流程
        //////////////////////////////////////////
        public bool AutoSync;
        // 登陆成功，默认不自动同步
        public void OnLoginSuccess()
        {
            if (AutoSync)
                getRemoteConfig(true);
        }
    } 
}