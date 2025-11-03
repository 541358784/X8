using System;
using System.Linq;
using UnityEngine;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using DragonU3DSDK.Asset;
using System.Collections.Generic;
using DragonPlus.ConfigHub;
using Newtonsoft.Json;


namespace DragonU3DSDK.Network.RemoteConfig
{
    public class UserGroupConfigManager : Manager<UserGroupConfigManager>
    {
        // @param configNames 传入需要获取的功能配置标识键名(支持多个,人为约定名称,唯一标识一款功能)，默认为空，若为空，则返回所有有效存在的功能配置
        // @callback(bool, SGetSegmentationConfig) 布尔值false => 使用包内配置，true => 使用协议返回结果内的配置字符串
        // @onlyGroupConfig 若传true，仅下发与rules表内groupId关联的配置数据表内容
        // 返回结果内configData为dict，key为请求的功能配置标识键名，value为配置数值JSON 字符串。IMPORTANT!!![若value为空字符串""，则表示对应功能未获取到远端配置，需要使用包内配置]
        // Example:
        // request param: ["reward_ad_config", "newbie_bundle_config"] 请求获取功能reward_ad_config(奖励广告)和newbie_bundle_config(新手礼包)两项功能的分层配置数据
        // success response data: {"reward_ad_config": "{\"bonus\": [...], \"configrewardad\": [...], ...}", "newbie_bundle_config":""} 
        // 响应数据表示成功获取到了reward_ad_config(奖励广告)远端配置，可直接使用JSON字符串序列化替换相同功能的包内配置；未获取到newbie_bundle_config(新手礼包)远端配置，仍然使用包内配置
        public void Request(List<string> configNames, Action<bool, SGetSegmentationConfig> callback, bool onlyGroupConfig = false)
        {
            CGetSegmentationConfig cConfig = new CGetSegmentationConfig();
            if (configNames != null && configNames.Count > 0) {
                cConfig.ConfigKey.AddRange(configNames);
            }
            cConfig.OnlyGroupConfig = onlyGroupConfig;

            APIManager.Instance.Send(cConfig, (SGetSegmentationConfig sConfig) =>
            {
                callback(true, sConfig); // 通知业务层使用返回的配置数据
                // 发送对应common game event事件BI
                var storageCommon = StorageManager.Instance.GetStorage<StorageCommon>();
                storageCommon.CampaignTypeCode = sConfig.CampaignTypeCode;
                foreach (var kvp in sConfig.GroupIdData.ToArray())
                {
                    storageCommon.RemoteGroupIdDatas[kvp.Key] = kvp.Value;
                    DragonU3DSDK.Network.BI.BIManager.Instance.SendCommonGameEvent(DragonU3DSDK.Network.API.Protocol.BiEventCommon.Types.CommonGameEventType.UserGroupConfigSuccess, kvp.Key, kvp.Value.ToString());
                }
            }, (errno, errmsg, resp) =>
            {
                // 发送对应common game event事件BI
                DragonU3DSDK.Network.BI.BIManager.Instance.SendCommonGameEvent(DragonU3DSDK.Network.API.Protocol.BiEventCommon.Types.CommonGameEventType.UserGroupConfigFailure, errmsg);
                DebugUtil.LogError("### SGetSegmentationConfig response Error ###" + errmsg);
                callback(false, null);// 通知业务层使用本地配置
            });
        }
        
        public static void Request(Dictionary<string, uint> configs, Action<bool, SGetSegmentationConfig> callback, bool onlyGroupConfig = false)
        {
            if (configs == null || configs.Count == 0)
                return;
            
            var cConfig = new CGetSegmentationConfig();
            cConfig.ConfigKey.AddRange(configs.Keys.ToList());
            cConfig.ConfigGroupId.AddAllKVPFrom(configs);
            cConfig.OnlyGroupConfig = onlyGroupConfig;

            APIManager.Instance.Send(cConfig, (SGetSegmentationConfig sConfig) =>
            {
                callback(true, sConfig); // 通知业务层使用返回的配置数据
                // 发送对应common game event事件BI
                var storageCommon = StorageManager.Instance.GetStorage<StorageCommon>();
                foreach (var kvp in sConfig.GroupIdData.ToArray())
                {
                    storageCommon.RemoteGroupIdDatas[kvp.Key] = kvp.Value;
                    DragonU3DSDK.Network.BI.BIManager.Instance.SendCommonGameEvent(DragonU3DSDK.Network.API.Protocol.BiEventCommon.Types.CommonGameEventType.UserGroupConfigSuccess, kvp.Key, kvp.Value.ToString());
                }
            }, (errno, errmsg, resp) =>
            {
                // 发送对应common game event事件BI
                DragonU3DSDK.Network.BI.BIManager.Instance.SendCommonGameEvent(DragonU3DSDK.Network.API.Protocol.BiEventCommon.Types.CommonGameEventType.UserGroupConfigFailure, errmsg);
                DebugUtil.LogError("### SGetSegmentationConfig response Error ###" + errmsg);
                callback(false, null);// 通知业务层使用本地配置
            });
        }
    }
}