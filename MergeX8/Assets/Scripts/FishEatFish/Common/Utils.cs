using System;
using DragonPlus.Config.FishEatFish;
using UnityEngine;
// using DragonPlus.Config.Makeover;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using Newtonsoft.Json.Linq;

namespace FishEatFishSpace
{
    public class Utils
    {
        public static bool isOpen = false;
        
        public static GameObject GetOrCreateRoot()
        {
            GameObject root = GameObject.Find("FishEatFishRootObject");
            if (root == null) root = new GameObject("FishEatFishRootObject");
            return root;
        }

        public static void DestoryRoot()
        {
            GameObject.DestroyImmediate(GetOrCreateRoot());
        }

        public static FishEatFishLevel GetLevelCfg(int id)
        {
            return 
                FishEatFishConfigManager.Instance.FishEatFishLevelList.Find(x => x.id == id);
        }
        
        public static void InitServerConfig()
        {
            CGetConfig cGetConfig = new CGetConfig
            {
                Route = "FishEatFish_" + AssetConfigController.Instance.RootVersion,
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
#elif UNITY_IOS
         isOpen = int.Parse(obj["iOS"].ToString()) == 1;
#else
         isOpen = int.Parse(obj["Android"].ToString()) == 1;
#endif

                //isOpen = true;
            }, (errno, errmsg, resp) => { });
        }
    }
}