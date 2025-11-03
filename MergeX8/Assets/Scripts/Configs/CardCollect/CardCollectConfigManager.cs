
using System;
using System.Collections;
using System.Collections.Generic;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using Newtonsoft.Json;
using UnityEngine;

namespace DragonPlus.Config.CardCollect
{
    public partial class CardCollectConfigManager : TableSingleton<CardCollectConfigManager>
    {   
        public bool ConfigFromRemote;
        public bool ConfigInit = false;
        public List<TableCardCollectionTheme> TableCardCollectionThemeList;
        public List<TableCardCollectionCardBook> TableCardCollectionCardBookList;
        public List<TableCardCollectionCardItem> TableCardCollectionCardItemList;
        public List<TableCardCollectionRandomGroup> TableCardCollectionRandomGroupList;
        public List<TableCardCollectionCardPackage> TableCardCollectionCardPackageList;
        public List<TableCardCollectionCardPackageItem> TableCardCollectionCardPackageItemList;
        public List<TableCardCollectionWildCard> TableCardCollectionWildCardList;
        public List<TableCardCollectionCardPackageExchange> TableCardCollectionCardPackageExchangeList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(TableCardCollectionTheme)] = "TableCardCollectionTheme",
            [typeof(TableCardCollectionCardBook)] = "TableCardCollectionCardBook",
            [typeof(TableCardCollectionCardItem)] = "TableCardCollectionCardItem",
            [typeof(TableCardCollectionRandomGroup)] = "TableCardCollectionRandomGroup",
            [typeof(TableCardCollectionCardPackage)] = "TableCardCollectionCardPackage",
            [typeof(TableCardCollectionCardPackageItem)] = "TableCardCollectionCardPackageItem",
            [typeof(TableCardCollectionWildCard)] = "TableCardCollectionWildCard",
            [typeof(TableCardCollectionCardPackageExchange)] = "TableCardCollectionCardPackageExchange",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("cardcollectiontheme")) return false;
            if (!table.ContainsKey("cardcollectioncardbook")) return false;
            if (!table.ContainsKey("cardcollectioncarditem")) return false;
            if (!table.ContainsKey("cardcollectionrandomgroup")) return false;
            if (!table.ContainsKey("cardcollectioncardpackage")) return false;
            if (!table.ContainsKey("cardcollectioncardpackageitem")) return false;
            if (!table.ContainsKey("cardcollectionwildcard")) return false;
            if (!table.ContainsKey("cardcollectioncardpackageexchange")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "TableCardCollectionTheme": cfg = TableCardCollectionThemeList as List<T>; break;
                case "TableCardCollectionCardBook": cfg = TableCardCollectionCardBookList as List<T>; break;
                case "TableCardCollectionCardItem": cfg = TableCardCollectionCardItemList as List<T>; break;
                case "TableCardCollectionRandomGroup": cfg = TableCardCollectionRandomGroupList as List<T>; break;
                case "TableCardCollectionCardPackage": cfg = TableCardCollectionCardPackageList as List<T>; break;
                case "TableCardCollectionCardPackageItem": cfg = TableCardCollectionCardPackageItemList as List<T>; break;
                case "TableCardCollectionWildCard": cfg = TableCardCollectionWildCardList as List<T>; break;
                case "TableCardCollectionCardPackageExchange": cfg = TableCardCollectionCardPackageExchangeList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("configs/cardcollect/cardcollectconfig", addToCache:false);
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load configs/cardcollect/cardcollectconfig error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            TableCardCollectionThemeList = JsonConvert.DeserializeObject<List<TableCardCollectionTheme>>(JsonConvert.SerializeObject(table["cardcollectiontheme"]));
            TableCardCollectionCardBookList = JsonConvert.DeserializeObject<List<TableCardCollectionCardBook>>(JsonConvert.SerializeObject(table["cardcollectioncardbook"]));
            TableCardCollectionCardItemList = JsonConvert.DeserializeObject<List<TableCardCollectionCardItem>>(JsonConvert.SerializeObject(table["cardcollectioncarditem"]));
            TableCardCollectionRandomGroupList = JsonConvert.DeserializeObject<List<TableCardCollectionRandomGroup>>(JsonConvert.SerializeObject(table["cardcollectionrandomgroup"]));
            TableCardCollectionCardPackageList = JsonConvert.DeserializeObject<List<TableCardCollectionCardPackage>>(JsonConvert.SerializeObject(table["cardcollectioncardpackage"]));
            TableCardCollectionCardPackageItemList = JsonConvert.DeserializeObject<List<TableCardCollectionCardPackageItem>>(JsonConvert.SerializeObject(table["cardcollectioncardpackageitem"]));
            TableCardCollectionWildCardList = JsonConvert.DeserializeObject<List<TableCardCollectionWildCard>>(JsonConvert.SerializeObject(table["cardcollectionwildcard"]));
            TableCardCollectionCardPackageExchangeList = JsonConvert.DeserializeObject<List<TableCardCollectionCardPackageExchange>>(JsonConvert.SerializeObject(table["cardcollectioncardpackageexchange"]));
            

            ConfigInit = true;
            Trim();
        }
    }
}