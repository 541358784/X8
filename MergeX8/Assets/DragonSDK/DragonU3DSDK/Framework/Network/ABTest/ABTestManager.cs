using System;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using DragonU3DSDK.Util;
using UnityEngine;

namespace DragonU3DSDK.Network.ABTest
{
    public class ABTestManager : Manager<ABTestManager>
    {
        private const string ABTEST_CONFIG_PREFIX = "ABTEST_";

        public void Init()
        {
#if ABTEST_ENABLE
            try
            {
                DragonU3DSDK.Network.API.APIManager.Instance.Send(new CGetABTestConfig(), (SGetABTestConfig response) =>
                {
                    DebugUtil.Log($"### GetABTestConfigRequest response Success ###");

                    var storageCommon = StorageManager.Instance.GetStorage<StorageCommon>();
                    foreach (var kv in response.AbtestConfig)
                    {
                        storageCommon.Abtests[kv.Key] = kv.Value.Group;

                        //因配置可能过大，未存储在存档内
                        var bytes = RijndaelManager.Instance.EncryptStringToBytes(kv.Value.Payload);
                        PlayerPrefs.SetString($"{ABTEST_CONFIG_PREFIX}{kv.Key}", System.Convert.ToBase64String(bytes));

                        DebugUtil.Log($"### GetABTestConfigRequest response Success ### {kv.Key} : {kv.Value.Group}");
                    }
                    
                    DragonU3DSDK.EventManager.Instance.Trigger<DragonU3DSDK.SDKEvents.ABTestRequestFinishEvent>().Data(true).Trigger();

                }, (errorCode, errorMsg, response) =>
                {
                    DebugUtil.LogError("### GetABTestConfigRequest response Error ###" + errorMsg);
                    DragonU3DSDK.EventManager.Instance.Trigger<DragonU3DSDK.SDKEvents.ABTestRequestFinishEvent>().Data(false).Trigger();

                });
            }
            catch (Exception e)
            {
                DebugUtil.LogError("### GetABTestConfigRequest response Exception ###" + e.Message);
                DragonU3DSDK.EventManager.Instance.Trigger<DragonU3DSDK.SDKEvents.ABTestRequestFinishEvent>().Data(false).Trigger();
            }
#endif
        }


        /// <summary>
        /// 获取配置
        /// </summary>
        /// <param name="key"></param>
        /// <returns>返回string.Empty时未参与abtest，使用包内配置</returns>
        public string GetConfig(string key)
        {
            do
            {
                if (string.IsNullOrEmpty(key)) break;

                var realKey = $"{ABTEST_CONFIG_PREFIX}{key}";
                if (!PlayerPrefs.HasKey(realKey)) break;

                var encryptString = PlayerPrefs.GetString(realKey);
                if (string.IsNullOrEmpty(encryptString)) break;

                var encryptData = System.Convert.FromBase64String(encryptString);
                var configString = RijndaelManager.Instance.DecryptStringFromBytes(encryptData);
                if (string.IsNullOrEmpty(configString)) break;

                return configString;
            } while (false);

            return string.Empty;
        }
    }
}