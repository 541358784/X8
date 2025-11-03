/************************************************
 * Storage class : StorageCardCollection
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageCardCollection : StorageBase
    {
         
        // 已完成的卡册主题(KEY为卡册主题ID)
        [JsonProperty]
        StorageDictionary<int,StorageCardCollectionCardTheme> completedCardThemes = new StorageDictionary<int,StorageCardCollectionCardTheme>();
        [JsonIgnore]
        public StorageDictionary<int,StorageCardCollectionCardTheme> CompletedCardThemes
        {
            get
            {
                return completedCardThemes;
            }
        }
        // ---------------------------------//
         
        // 已完成的卡册(KEY为卡册ID)
        [JsonProperty]
        StorageDictionary<int,StorageCardCollectionCardBook> completedCardBooks = new StorageDictionary<int,StorageCardCollectionCardBook>();
        [JsonIgnore]
        public StorageDictionary<int,StorageCardCollectionCardBook> CompletedCardBooks
        {
            get
            {
                return completedCardBooks;
            }
        }
        // ---------------------------------//
         
        // 已收集的卡牌(KEY为卡牌ID)
        [JsonProperty]
        StorageDictionary<int,StorageCardCollectionCardItem> collectedCards = new StorageDictionary<int,StorageCardCollectionCardItem>();
        [JsonIgnore]
        public StorageDictionary<int,StorageCardCollectionCardItem> CollectedCards
        {
            get
            {
                return collectedCards;
            }
        }
        // ---------------------------------//
         
        // 多余的卡牌(用于计算星星数量,KEY为卡牌等级)(未启用)
        [JsonProperty]
        StorageDictionary<int,StorageCardCollectionExtraCardGroup> extraCards = new StorageDictionary<int,StorageCardCollectionExtraCardGroup>();
        [JsonIgnore]
        public StorageDictionary<int,StorageCardCollectionExtraCardGroup> ExtraCards
        {
            get
            {
                return extraCards;
            }
        }
        // ---------------------------------//
        
        // 补齐粒度的星星数量
        [JsonProperty]
        int starCount;
        [JsonIgnore]
        public int StarCount
        {
            get
            {
                return starCount;
            }
            set
            {
                if(starCount != value)
                {
                    starCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 未开启的卡包
        [JsonProperty]
        StorageList<StorageCardCollectionCardPackage> unOpenPackageList = new StorageList<StorageCardCollectionCardPackage>();
        [JsonIgnore]
        public StorageList<StorageCardCollectionCardPackage> UnOpenPackageList
        {
            get
            {
                return unOpenPackageList;
            }
        }
        // ---------------------------------//
        
        // 万能卡(KEY为万能卡ID,VALUE为数量)
        [JsonProperty]
        StorageDictionary<int,int> wildCards = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> WildCards
        {
            get
            {
                return wildCards;
            }
        }
        // ---------------------------------//
        
        // 是否初始化
        [JsonProperty]
        bool isInit;
        [JsonIgnore]
        public bool IsInit
        {
            get
            {
                return isInit;
            }
            set
            {
                if(isInit != value)
                {
                    isInit = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否第二次初始化(未启用)
        [JsonProperty]
        bool isInit2;
        [JsonIgnore]
        public bool IsInit2
        {
            get
            {
                return isInit2;
            }
            set
            {
                if(isInit2 != value)
                {
                    isInit2 = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 开启过的主题ID
        [JsonProperty]
        StorageList<int> openThemeList = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> OpenThemeList
        {
            get
            {
                return openThemeList;
            }
        }
        // ---------------------------------//
         
        // 随机组字典
        [JsonProperty]
        StorageDictionary<int,StorageCardCollectionThemeRandomGroup> themeRandomGroupDic = new StorageDictionary<int,StorageCardCollectionThemeRandomGroup>();
        [JsonIgnore]
        public StorageDictionary<int,StorageCardCollectionThemeRandomGroup> ThemeRandomGroupDic
        {
            get
            {
                return themeRandomGroupDic;
            }
        }
        // ---------------------------------//
        
        // 未坍缩的卡包
        [JsonProperty]
        StorageList<StorageResData> abstractPackages = new StorageList<StorageResData>();
        [JsonIgnore]
        public StorageList<StorageResData> AbstractPackages
        {
            get
            {
                return abstractPackages;
            }
        }
        // ---------------------------------//
        
        // 未坍缩的卡包源
        [JsonProperty]
        StorageList<string> abstractPackagesSource = new StorageList<string>();
        [JsonIgnore]
        public StorageList<string> AbstractPackagesSource
        {
            get
            {
                return abstractPackagesSource;
            }
        }
        // ---------------------------------//
        
        // 解锁的主题ID
        [JsonProperty]
        StorageList<int> unlockThemeList = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> UnlockThemeList
        {
            get
            {
                return unlockThemeList;
            }
        }
        // ---------------------------------//
        
    }
}