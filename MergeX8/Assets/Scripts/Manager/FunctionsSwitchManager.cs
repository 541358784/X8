using System;
using System.Collections.Generic;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Manager
{
    public class FunctionsSwitchManager : Singleton<FunctionsSwitchManager>
    {
        private const string _functionKey = "functionKey";
        private long _installTime = 0;
        
        public StorageHome StorageHome
        {
            get
            {
                return StorageManager.Instance.GetStorage<StorageHome>();
            }
        }

        public bool IsFunctionVersion()
        {
            return StorageHome.AbTestConfig.ContainsKey(_functionKey);
        }

        public enum FuncType : int
        {
            None=0,
            SignIn,//签到
            DailyTask,//每日任务
            RvShop,//电视广告
            RvBalloon,//气球广告
            RvGift,//神秘礼物
            Count,
        }

        private Dictionary<FuncType, bool> _functionSwitch = new Dictionary<FuncType, bool>();

        public bool FunctionOn(FuncType type)
        {
            if (AdConfigHandle.Instance.CanCloseFunction(type.ToString()))
                return false;
            
            if (_installTime > 0)
            {
                if ((ulong)_installTime > StorageManager.Instance.GetStorage<StorageCommon>().InstalledAt)
                    return true;
            }
            
            if (!IsFunctionVersion())
                return true;
            
            if (!_functionSwitch.ContainsKey(type))
                return false;
            
            return _functionSwitch[type];
        }
        
        public void Init()
        {
            _functionSwitch.Clear();
            
            if (!StorageHome.IsFirstLogin)
            {
                if(!StorageHome.AbTestConfig.ContainsKey(_functionKey))
                    StorageHome.AbTestConfig.Add(_functionKey, _functionKey);
            }
            
            if(!IsFunctionVersion())
                return;

            InitServerConfig();
        }
        
        public void InitServerConfig()
        {
            CGetConfig cGetConfig = new CGetConfig
            {
                Route = "function_v1_0_17"//+ AssetConfigController.Instance.RootVersion,
            };

            APIManager.Instance.Send(cGetConfig, (SGetConfig sGetConfig) =>
            {
                if (string.IsNullOrEmpty(sGetConfig.Config.Json))
                {
                    DebugUtil.LogWarning("功能开关 服务器配置为空！");
                    return;
                }

                JObject obj = JObject.Parse(sGetConfig.Config.Json);


                string parseString = "";
#if UNITY_ANDROID
                parseString = obj["android"].ToString();
#elif UNITY_IOS
          parseString = obj["ios"].ToString();
#else
         parseString = obj["android"].ToString();
#endif

                _installTime = long.Parse(obj["installTime"].ToString());
                
                if(parseString.IsEmptyString())
                    return;
                
                var parseJs = JObject.Parse(parseString.ToString());
                
                foreach (FuncType type in Enum.GetValues(typeof(FuncType)))
                {
                    var key = type.ToString();
                    
                    if (parseJs[key] == null)
                        continue;

                    if (_functionSwitch.ContainsKey(type))
                    {
                        _functionSwitch[type] = int.Parse(parseJs[key].ToString()) == 1;
                    }
                    else
                    {
                        _functionSwitch.Add(type, int.Parse(parseJs[key].ToString()) == 1);
                    }
                }
                //isOpen = true;
            }, (errno, errmsg, resp) => { });
        }
    }
}