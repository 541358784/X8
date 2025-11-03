/************************************************
 * TMatch Config Manager class : TMatchConfigManager
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

namespace DragonPlus.Config.TMatch
{
    public partial class TMatchConfigManager : Manager<TMatchConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<Layout> LayoutList;
        public List<StarChest> StarChestList;
        public List<LevelChest> LevelChestList;
        public List<Global> GlobalList;
        public List<GlobalReward> GlobalRewardList;
        public List<Level> LevelList;
        public List<Item> ItemList;
        public List<LayoutDesign> LayoutDesignList;
        public List<LayoutGroupDesign> LayoutGroupDesignList;
        public List<TargetDesign> TargetDesignList;
        public List<Revive> ReviveList;
        public List<GlodenHatter> GlodenHatterList;
        public List<ShopConfig> ShopConfigList;
        public List<WeeklyChallenge> WeeklyChallengeList;
        public List<WeeklyChallengeNew> WeeklyChallengeNewList;
        public List<WeeklyChallengeReward> WeeklyChallengeRewardList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(Layout)] = "Layout",
            [typeof(StarChest)] = "StarChest",
            [typeof(LevelChest)] = "LevelChest",
            [typeof(Global)] = "Global",
            [typeof(GlobalReward)] = "GlobalReward",
            [typeof(Level)] = "Level",
            [typeof(Item)] = "Item",
            [typeof(LayoutDesign)] = "LayoutDesign",
            [typeof(LayoutGroupDesign)] = "LayoutGroupDesign",
            [typeof(TargetDesign)] = "TargetDesign",
            [typeof(Revive)] = "Revive",
            [typeof(GlodenHatter)] = "GlodenHatter",
            [typeof(ShopConfig)] = "ShopConfig",
            [typeof(WeeklyChallenge)] = "WeeklyChallenge",
            [typeof(WeeklyChallengeNew)] = "WeeklyChallengeNew",
            [typeof(WeeklyChallengeReward)] = "WeeklyChallengeReward",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("layout")) return false;
            if (!table.ContainsKey("starchest")) return false;
            if (!table.ContainsKey("levelchest")) return false;
            if (!table.ContainsKey("global")) return false;
            if (!table.ContainsKey("globalreward")) return false;
            if (!table.ContainsKey("level")) return false;
            if (!table.ContainsKey("item")) return false;
            if (!table.ContainsKey("layoutdesign")) return false;
            if (!table.ContainsKey("layoutgroupdesign")) return false;
            if (!table.ContainsKey("targetdesign")) return false;
            if (!table.ContainsKey("revive")) return false;
            if (!table.ContainsKey("glodenhatter")) return false;
            if (!table.ContainsKey("shopconfig")) return false;
            if (!table.ContainsKey("weeklychallenge")) return false;
            if (!table.ContainsKey("weeklychallengenew")) return false;
            if (!table.ContainsKey("weeklychallengereward")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "Layout": cfg = LayoutList as List<T>; break;
                case "StarChest": cfg = StarChestList as List<T>; break;
                case "LevelChest": cfg = LevelChestList as List<T>; break;
                case "Global": cfg = GlobalList as List<T>; break;
                case "GlobalReward": cfg = GlobalRewardList as List<T>; break;
                case "Level": cfg = LevelList as List<T>; break;
                case "Item": cfg = ItemList as List<T>; break;
                case "LayoutDesign": cfg = LayoutDesignList as List<T>; break;
                case "LayoutGroupDesign": cfg = LayoutGroupDesignList as List<T>; break;
                case "TargetDesign": cfg = TargetDesignList as List<T>; break;
                case "Revive": cfg = ReviveList as List<T>; break;
                case "GlodenHatter": cfg = GlodenHatterList as List<T>; break;
                case "ShopConfig": cfg = ShopConfigList as List<T>; break;
                case "WeeklyChallenge": cfg = WeeklyChallengeList as List<T>; break;
                case "WeeklyChallengeNew": cfg = WeeklyChallengeNewList as List<T>; break;
                case "WeeklyChallengeReward": cfg = WeeklyChallengeRewardList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/TMatch/match");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/TMatch/match error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            LayoutList = JsonConvert.DeserializeObject<List<Layout>>(JsonConvert.SerializeObject(table["layout"]));
            StarChestList = JsonConvert.DeserializeObject<List<StarChest>>(JsonConvert.SerializeObject(table["starchest"]));
            LevelChestList = JsonConvert.DeserializeObject<List<LevelChest>>(JsonConvert.SerializeObject(table["levelchest"]));
            GlobalList = JsonConvert.DeserializeObject<List<Global>>(JsonConvert.SerializeObject(table["global"]));
            GlobalRewardList = JsonConvert.DeserializeObject<List<GlobalReward>>(JsonConvert.SerializeObject(table["globalreward"]));
            LevelList = JsonConvert.DeserializeObject<List<Level>>(JsonConvert.SerializeObject(table["level"]));
            ItemList = JsonConvert.DeserializeObject<List<Item>>(JsonConvert.SerializeObject(table["item"]));
            LayoutDesignList = JsonConvert.DeserializeObject<List<LayoutDesign>>(JsonConvert.SerializeObject(table["layoutdesign"]));
            LayoutGroupDesignList = JsonConvert.DeserializeObject<List<LayoutGroupDesign>>(JsonConvert.SerializeObject(table["layoutgroupdesign"]));
            TargetDesignList = JsonConvert.DeserializeObject<List<TargetDesign>>(JsonConvert.SerializeObject(table["targetdesign"]));
            ReviveList = JsonConvert.DeserializeObject<List<Revive>>(JsonConvert.SerializeObject(table["revive"]));
            GlodenHatterList = JsonConvert.DeserializeObject<List<GlodenHatter>>(JsonConvert.SerializeObject(table["glodenhatter"]));
            ShopConfigList = JsonConvert.DeserializeObject<List<ShopConfig>>(JsonConvert.SerializeObject(table["shopconfig"]));
            WeeklyChallengeList = JsonConvert.DeserializeObject<List<WeeklyChallenge>>(JsonConvert.SerializeObject(table["weeklychallenge"]));
            WeeklyChallengeNewList = JsonConvert.DeserializeObject<List<WeeklyChallengeNew>>(JsonConvert.SerializeObject(table["weeklychallengenew"]));
            WeeklyChallengeRewardList = JsonConvert.DeserializeObject<List<WeeklyChallengeReward>>(JsonConvert.SerializeObject(table["weeklychallengereward"]));
            
        }
    }
}