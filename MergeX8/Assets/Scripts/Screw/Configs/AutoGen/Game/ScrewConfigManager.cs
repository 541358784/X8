
using System;
using System.Collections;
using System.Collections.Generic;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using Newtonsoft.Json;
using UnityEngine;

namespace DragonPlus.Config.Screw
{
    public partial class ScrewConfigManager : TableSingleton<ScrewConfigManager>
    {   
        public bool ConfigFromRemote;
        public bool ConfigInit = false;
        public List<TableGlobal> TableGlobalList;
        public List<TableLevels> TableLevelsList;
        public List<TableLoopLevels> TableLoopLevelsList;
        public List<TableMiniGameLevels> TableMiniGameLevelsList;
        public List<TableFeatureUnlockInfo> TableFeatureUnlockInfoList;
        public List<TableItem> TableItemList;
        public List<TableShop> TableShopList;
        public List<TableCoinShopConfig> TableCoinShopConfigList;
        public List<TableDailyPackageConfig> TableDailyPackageConfigList;
        public List<TableRebornPackageConfig> TableRebornPackageConfigList;
        public List<TableLevelChest> TableLevelChestList;
        public List<TableFailOfferPack> TableFailOfferPackList;
        public List<TableIceBreakingPack> TableIceBreakingPackList;
        public List<TableAvatar> TableAvatarList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(TableGlobal)] = "TableGlobal",
            [typeof(TableLevels)] = "TableLevels",
            [typeof(TableLoopLevels)] = "TableLoopLevels",
            [typeof(TableMiniGameLevels)] = "TableMiniGameLevels",
            [typeof(TableFeatureUnlockInfo)] = "TableFeatureUnlockInfo",
            [typeof(TableItem)] = "TableItem",
            [typeof(TableShop)] = "TableShop",
            [typeof(TableCoinShopConfig)] = "TableCoinShopConfig",
            [typeof(TableDailyPackageConfig)] = "TableDailyPackageConfig",
            [typeof(TableRebornPackageConfig)] = "TableRebornPackageConfig",
            [typeof(TableLevelChest)] = "TableLevelChest",
            [typeof(TableFailOfferPack)] = "TableFailOfferPack",
            [typeof(TableIceBreakingPack)] = "TableIceBreakingPack",
            [typeof(TableAvatar)] = "TableAvatar",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("global")) return false;
            if (!table.ContainsKey("levels")) return false;
            if (!table.ContainsKey("looplevels")) return false;
            if (!table.ContainsKey("minigamelevels")) return false;
            if (!table.ContainsKey("featureunlockinfo")) return false;
            if (!table.ContainsKey("item")) return false;
            if (!table.ContainsKey("shop")) return false;
            if (!table.ContainsKey("coinshopconfig")) return false;
            if (!table.ContainsKey("dailypackageconfig")) return false;
            if (!table.ContainsKey("rebornpackageconfig")) return false;
            if (!table.ContainsKey("levelchest")) return false;
            if (!table.ContainsKey("failofferpack")) return false;
            if (!table.ContainsKey("icebreakingpack")) return false;
            if (!table.ContainsKey("avatar")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "TableGlobal": cfg = TableGlobalList as List<T>; break;
                case "TableLevels": cfg = TableLevelsList as List<T>; break;
                case "TableLoopLevels": cfg = TableLoopLevelsList as List<T>; break;
                case "TableMiniGameLevels": cfg = TableMiniGameLevelsList as List<T>; break;
                case "TableFeatureUnlockInfo": cfg = TableFeatureUnlockInfoList as List<T>; break;
                case "TableItem": cfg = TableItemList as List<T>; break;
                case "TableShop": cfg = TableShopList as List<T>; break;
                case "TableCoinShopConfig": cfg = TableCoinShopConfigList as List<T>; break;
                case "TableDailyPackageConfig": cfg = TableDailyPackageConfigList as List<T>; break;
                case "TableRebornPackageConfig": cfg = TableRebornPackageConfigList as List<T>; break;
                case "TableLevelChest": cfg = TableLevelChestList as List<T>; break;
                case "TableFailOfferPack": cfg = TableFailOfferPackList as List<T>; break;
                case "TableIceBreakingPack": cfg = TableIceBreakingPackList as List<T>; break;
                case "TableAvatar": cfg = TableAvatarList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("configs/screw/game/screwgameconfig", addToCache:false);
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load configs/screw/game/screwgameconfig error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            TableGlobalList = JsonConvert.DeserializeObject<List<TableGlobal>>(JsonConvert.SerializeObject(table["global"]));
            TableLevelsList = JsonConvert.DeserializeObject<List<TableLevels>>(JsonConvert.SerializeObject(table["levels"]));
            TableLoopLevelsList = JsonConvert.DeserializeObject<List<TableLoopLevels>>(JsonConvert.SerializeObject(table["looplevels"]));
            TableMiniGameLevelsList = JsonConvert.DeserializeObject<List<TableMiniGameLevels>>(JsonConvert.SerializeObject(table["minigamelevels"]));
            TableFeatureUnlockInfoList = JsonConvert.DeserializeObject<List<TableFeatureUnlockInfo>>(JsonConvert.SerializeObject(table["featureunlockinfo"]));
            TableItemList = JsonConvert.DeserializeObject<List<TableItem>>(JsonConvert.SerializeObject(table["item"]));
            TableShopList = JsonConvert.DeserializeObject<List<TableShop>>(JsonConvert.SerializeObject(table["shop"]));
            TableCoinShopConfigList = JsonConvert.DeserializeObject<List<TableCoinShopConfig>>(JsonConvert.SerializeObject(table["coinshopconfig"]));
            TableDailyPackageConfigList = JsonConvert.DeserializeObject<List<TableDailyPackageConfig>>(JsonConvert.SerializeObject(table["dailypackageconfig"]));
            TableRebornPackageConfigList = JsonConvert.DeserializeObject<List<TableRebornPackageConfig>>(JsonConvert.SerializeObject(table["rebornpackageconfig"]));
            TableLevelChestList = JsonConvert.DeserializeObject<List<TableLevelChest>>(JsonConvert.SerializeObject(table["levelchest"]));
            TableFailOfferPackList = JsonConvert.DeserializeObject<List<TableFailOfferPack>>(JsonConvert.SerializeObject(table["failofferpack"]));
            TableIceBreakingPackList = JsonConvert.DeserializeObject<List<TableIceBreakingPack>>(JsonConvert.SerializeObject(table["icebreakingpack"]));
            TableAvatarList = JsonConvert.DeserializeObject<List<TableAvatar>>(JsonConvert.SerializeObject(table["avatar"]));
            

            ConfigInit = true;
            Trim();
        }
    }
}