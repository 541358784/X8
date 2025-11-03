/************************************************
 * TMatchShop Config Manager class : TMatchShopConfigManager
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using Newtonsoft.Json;
using UnityEngine;

namespace DragonPlus.Config.TMatchShop
{
    public partial class TMatchShopConfigManager : Manager<TMatchShopConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<Global> GlobalList;
        public List<ItemRewards> ItemRewardsList;
        public List<ItemConfig> ItemConfigList;
        public List<Shop> ShopList;
        public List<IAPAmountBI> IAPAmountBIList;
        public List<GlobalString> GlobalStringList;
        public List<GlobalNumber> GlobalNumberList;
        public List<PushNotification> PushNotificationList;
        public List<NewVersionReward> NewVersionRewardList;
        public List<DecoWorld> DecoWorldList;
        public List<CountryToContinent> CountryToContinentList;
        public List<Guide> GuideList;
        public List<PlayerHead> PlayerHeadList;
        public List<LoadingTransition> LoadingTransitionList;
        public List<StoryRole> StoryRoleList;
        public List<Story> StoryList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(Global)] = "Global",
            [typeof(ItemRewards)] = "ItemRewards",
            [typeof(ItemConfig)] = "ItemConfig",
            [typeof(Shop)] = "Shop",
            [typeof(IAPAmountBI)] = "IAPAmountBI",
            [typeof(GlobalString)] = "GlobalString",
            [typeof(GlobalNumber)] = "GlobalNumber",
            [typeof(PushNotification)] = "PushNotification",
            [typeof(NewVersionReward)] = "NewVersionReward",
            [typeof(DecoWorld)] = "DecoWorld",
            [typeof(CountryToContinent)] = "CountryToContinent",
            [typeof(Guide)] = "Guide",
            [typeof(PlayerHead)] = "PlayerHead",
            [typeof(LoadingTransition)] = "LoadingTransition",
            [typeof(StoryRole)] = "StoryRole",
            [typeof(Story)] = "Story",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("global")) return false;
            if (!table.ContainsKey("itemrewards")) return false;
            if (!table.ContainsKey("itemconfig")) return false;
            if (!table.ContainsKey("shop")) return false;
            if (!table.ContainsKey("iapamountbi")) return false;
            if (!table.ContainsKey("globalstring")) return false;
            if (!table.ContainsKey("globalnumber")) return false;
            if (!table.ContainsKey("pushnotification")) return false;
            if (!table.ContainsKey("newversionreward")) return false;
            if (!table.ContainsKey("decoworld")) return false;
            if (!table.ContainsKey("countrytocontinent")) return false;
            if (!table.ContainsKey("guide")) return false;
            if (!table.ContainsKey("playerhead")) return false;
            if (!table.ContainsKey("loadingtransition")) return false;
            if (!table.ContainsKey("storyrole")) return false;
            if (!table.ContainsKey("story")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "Global": cfg = GlobalList as List<T>; break;
                case "ItemRewards": cfg = ItemRewardsList as List<T>; break;
                case "ItemConfig": cfg = ItemConfigList as List<T>; break;
                case "Shop": cfg = ShopList as List<T>; break;
                case "IAPAmountBI": cfg = IAPAmountBIList as List<T>; break;
                case "GlobalString": cfg = GlobalStringList as List<T>; break;
                case "GlobalNumber": cfg = GlobalNumberList as List<T>; break;
                case "PushNotification": cfg = PushNotificationList as List<T>; break;
                case "NewVersionReward": cfg = NewVersionRewardList as List<T>; break;
                case "DecoWorld": cfg = DecoWorldList as List<T>; break;
                case "CountryToContinent": cfg = CountryToContinentList as List<T>; break;
                case "Guide": cfg = GuideList as List<T>; break;
                case "PlayerHead": cfg = PlayerHeadList as List<T>; break;
                case "LoadingTransition": cfg = LoadingTransitionList as List<T>; break;
                case "StoryRole": cfg = StoryRoleList as List<T>; break;
                case "Story": cfg = StoryList as List<T>; break;
                default: throw new ArgumentOutOfRangeException(nameof(subModule), subModule, null);
            }
            return cfg;
        }
        public void InitConfig(String configJson = null)
        {
            ConfigFromRemote = true;
            Hashtable table = null;
            if (!string.IsNullOrEmpty(configJson))
                table = JsonConvert.DeserializeObject<Hashtable>(configJson);

            if (table == null || !CheckTable(table))
            {
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/TMatch/match_shop");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/TMatch/match_shop error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            GlobalList = JsonConvert.DeserializeObject<List<Global>>(JsonConvert.SerializeObject(table["global"]));
            ItemRewardsList = JsonConvert.DeserializeObject<List<ItemRewards>>(JsonConvert.SerializeObject(table["itemrewards"]));
            ItemConfigList = JsonConvert.DeserializeObject<List<ItemConfig>>(JsonConvert.SerializeObject(table["itemconfig"]));
            ShopList = JsonConvert.DeserializeObject<List<Shop>>(JsonConvert.SerializeObject(table["shop"]));
            IAPAmountBIList = JsonConvert.DeserializeObject<List<IAPAmountBI>>(JsonConvert.SerializeObject(table["iapamountbi"]));
            GlobalStringList = JsonConvert.DeserializeObject<List<GlobalString>>(JsonConvert.SerializeObject(table["globalstring"]));
            GlobalNumberList = JsonConvert.DeserializeObject<List<GlobalNumber>>(JsonConvert.SerializeObject(table["globalnumber"]));
            PushNotificationList = JsonConvert.DeserializeObject<List<PushNotification>>(JsonConvert.SerializeObject(table["pushnotification"]));
            NewVersionRewardList = JsonConvert.DeserializeObject<List<NewVersionReward>>(JsonConvert.SerializeObject(table["newversionreward"]));
            DecoWorldList = JsonConvert.DeserializeObject<List<DecoWorld>>(JsonConvert.SerializeObject(table["decoworld"]));
            CountryToContinentList = JsonConvert.DeserializeObject<List<CountryToContinent>>(JsonConvert.SerializeObject(table["countrytocontinent"]));
            GuideList = JsonConvert.DeserializeObject<List<Guide>>(JsonConvert.SerializeObject(table["guide"]));
            PlayerHeadList = JsonConvert.DeserializeObject<List<PlayerHead>>(JsonConvert.SerializeObject(table["playerhead"]));
            LoadingTransitionList = JsonConvert.DeserializeObject<List<LoadingTransition>>(JsonConvert.SerializeObject(table["loadingtransition"]));
            StoryRoleList = JsonConvert.DeserializeObject<List<StoryRole>>(JsonConvert.SerializeObject(table["storyrole"]));
            StoryList = JsonConvert.DeserializeObject<List<Story>>(JsonConvert.SerializeObject(table["story"]));
            
        }
    }
}