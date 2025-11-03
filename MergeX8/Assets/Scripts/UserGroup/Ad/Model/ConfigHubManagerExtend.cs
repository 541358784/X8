using UnityEngine;

/************************************************
 * ConfigHubManager
 * If there's any problem, ask yunhan.zeng@dragonplus.com
 ************************************************/
using System.Collections.Generic;
using System.Linq;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Zlib;
using DragonU3DSDK.Network.RemoteConfig;
using UnityEngine;
using Newtonsoft.Json;
using DragonU3DSDK.Util;

namespace DragonPlus.ConfigHub
{
    public partial class ConfigHubManager
    {
        public void UpdateRemoteConfig(bool onlyGroupConfig)
        {
            UserGroupConfigManager.Instance.Request(modules.Keys.ToList(),
                (success, config) =>
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

                        string configData =  configHubData.GetData(guid);
                        // module.MetaData = kv.Value;
                        // module.IsRemote = module.MetaData != null && !string.IsNullOrEmpty(configData);
                        module.InitConfig(kv.Value, configData);
                    }
                    DragonU3DSDK.EventManager.Instance.Trigger<DragonU3DSDK.SDKEvents.ConfigHubUpdatedEvent>().Trigger();
                },onlyGroupConfig
            );
        }
    }
}