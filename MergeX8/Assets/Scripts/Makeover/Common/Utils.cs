using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using DragonPlus.Config.Makeover;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay.UI.MiniGame;
using Newtonsoft.Json.Linq;

namespace Makeover
{
    public enum MiniGroup
    {
        None=-1,
        Old, //老玩家
        Puzzle,//拼图游戏
        DigTrench //挖沟
    }
    
    public class Utils
    {
        public static int makeOverLevel = -1;
        public static bool isOpen = false;
        public static bool isOpenScrew = false;
        
        private static string debugIsOpenPrefsKey = "debugIsOpen";
        private static string debugNewUserPrefsKey = "debugNewUser";
        
        public const string NewUserStorageKey = "NewUserStorageKey";
        public const string MiniGameGroupKey = "MiniGameGroupKey";
        
        public static bool debugIsOpen
        {
            get => PlayerPrefs.GetInt(debugIsOpenPrefsKey,0) == 1;
            set
            {
                PlayerPrefs.SetInt(debugIsOpenPrefsKey, value ? 1 : 0);
                PlayerPrefs.Save();
            }
        }

        public static void InitMiniGameGroup(MiniGroup group)
        {
            if(StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig.ContainsKey(MiniGameGroupKey))
                return;

            if (group == MiniGroup.None)
            {
                if(IsNewUser())
                    StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig.Add(MiniGameGroupKey, "1");
                else
                {
                    StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig.Add(MiniGameGroupKey, "0");
                }
            }
            else
            {
                StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig.Add(MiniGameGroupKey, ((int)group).ToString());
            }
        }

        public static MiniGroup GetMiniGroup()
        {
#if UNITY_ANDROID || UNITY_EDITOR
            if (StorageManager.Instance.GetStorage<StorageHome>().RcoveryRecord.ContainsKey("1.0.69"))
            {
                return MiniGroup.None;
            }  
#endif
            
            if(!StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig.ContainsKey(MiniGameGroupKey))
                return MiniGroup.Old;

            var value = StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig[MiniGameGroupKey];

            int intValue = 0;
            int.TryParse(value, out intValue);

            return (MiniGroup)intValue;
        }

        public static bool IsUseNewMiniGame()
        {
            var miniGroup = GetMiniGroup();
            return miniGroup != MiniGroup.Old && miniGroup != MiniGroup.None;
        }
        
        public static bool debugNewUser
        {
            get => PlayerPrefs.GetInt(debugNewUserPrefsKey,0) == 1 || StorageManager.Instance.GetStorage<StorageHome>().RcoveryRecord.ContainsKey(NewUserStorageKey);
            set
            {
                PlayerPrefs.SetInt(debugNewUserPrefsKey, value ? 1 : 0);
                PlayerPrefs.Save();
            }
        }
        
        private static List<UIPopupMiniGameController.MiniGameType> _onList = new List<UIPopupMiniGameController.MiniGameType>();

        public static bool IsOn(UIPopupMiniGameController.MiniGameType type)
        {
            return _onList.Contains(type);
        }

        public static bool IsNewUser()
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            return debugNewUser || StorageManager.Instance.GetStorage<StorageHome>().RcoveryRecord.ContainsKey(NewUserStorageKey);
#else
            return StorageManager.Instance.GetStorage<StorageHome>().RcoveryRecord.ContainsKey(NewUserStorageKey);
#endif
        }
        public static bool IsOpen
        {
            get
            {
                return false;
#if DEVELOPMENT_BUILD || UNITY_EDITOR
                return debugIsOpen || isOpen;
#else
                return isOpen;
#endif
            }
        }

        public static bool IsOpenScrew
        {
            get
            {
                return isOpenScrew;
            }
        }
        public static GameObject GetOrCreateRoot()
        {
            GameObject root = GameObject.Find("Makeover");
            if (root == null) root = new GameObject("Makeover");
            return root;
        }

        public static void DestroyRoot()
        {
            GameObject.DestroyImmediate(GetOrCreateRoot());
        }

        public static TableMoLevel GetLevelCfg(int id)
        {
            return MakeoverConfigManager.Instance.levelList.Find(x => x.id == id);
        }
        
        public static void InitServerConfig()
        {
            CGetConfig cGetConfig = new CGetConfig
            {
                Route = "asmr_" + AssetConfigController.Instance.RootVersion,
            };

            APIManager.Instance.Send(cGetConfig, (SGetConfig sGetConfig) =>
            {
                if (string.IsNullOrEmpty(sGetConfig.Config.Json))
                {
                    DebugUtil.LogWarning("MasterCard 服务器配置为空！");
                    return;
                }

                JObject obj = JObject.Parse(sGetConfig.Config.Json);

#if UNITY_ANDROID
                isOpen = int.Parse(obj["Android"].ToString()) == 1;
                isOpenScrew = int.Parse(obj["AndroidScrewOn"].ToString()) == 1;
#elif UNITY_IOS
         isOpen = int.Parse(obj["iOS"].ToString()) == 1;
                isOpenScrew = int.Parse(obj["IosScrewOn"].ToString()) == 1;
#else
         isOpen = int.Parse(obj["Android"].ToString()) == 1;
                isOpenScrew = int.Parse(obj["AndroidScrewOn"].ToString()) == 1;
#endif

                string offKey = "AndroidOn";
#if UNITY_IOS
                offKey = "IosOn";
#endif

                if (obj.ContainsKey(offKey))
                {
                    JArray offList = (JArray)obj[offKey];
                    foreach (var item in offList)
                    {
                        _onList.Add((UIPopupMiniGameController.MiniGameType)item.ToObject<int>());
                    }
                }
                //isOpen = true;
                CheckIsChinesePlayer();
            }, (errno, errmsg, resp) => { });
        }
        
        
        
        
        static async void CheckIsChinesePlayer()
        {
            if (Application.systemLanguage == SystemLanguage.ChineseSimplified)
            {
                isOpen = false;
            }
            if (isOpen)
            {
                var ipAddress = await GetExternalIPAddressAsync();
                if (ipAddress != null && ipAddress.loc == "CN")
                {
                    Console.WriteLine("IP address is in the Chinese range.");
                    isOpen = false;
                }
            }
        }

        class IpStruct
        {
            public string ip;
            public string loc;
        }
        static async Task<IpStruct> GetExternalIPAddressAsync()
        {
            try
            {
                // 使用一个可以返回当前设备的外部IP地址的Web服务
                using (HttpClient httpClient = new HttpClient())
                {
                    string response = await httpClient.GetStringAsync("https://www.cloudflare.com/cdn-cgi/trace");
                    // 这里假设返回的JSON数据包含了"ip"字段
                    // 实际中你可能需要根据具体的API响应格式来处理
                    var a = new IpStruct();
                    var aJson = JsonUtility.ToJson(a);
                    response = response.Replace("=", "\":\"");
                    response = response.Replace("\n", "\",\"");
                    response = "{\"" + response+"\"}";
                    response = response.Replace(",\"\"", "");
                    IpStruct jsonResponse = JsonUtility.FromJson<IpStruct>(response);
                    return jsonResponse;
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error getting external IP address: " + e.Message);
                return null;
            }
        }

        public static List<TableMiniGameSetting> GetMiniGameSettings()
        {
            List<TableMiniGameSetting> settings = new List<TableMiniGameSetting>();

            var group = GetMiniGroup();
            
            foreach (var config in GlobalConfigManager.Instance.MiniGameSettingList)
            {
                if(config.group == (int)group)
                    settings.Add(config);
            }

            return settings;
        }
    }
}