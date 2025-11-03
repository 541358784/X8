/************************************************
 * Storage class : StorageTMatch
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageTMatch : DragonU3DSDK.Storage.StorageBase
    {
        
        // 主线关卡ID
        [JsonProperty]
        int mainLevel;
        [JsonIgnore]
        public int MainLevel
        {
            get
            {
                return mainLevel;
            }
            set
            {
                if(mainLevel != value)
                {
                    mainLevel = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前主线关卡失败次数
        [JsonProperty]
        int mainLevelFailCnt;
        [JsonIgnore]
        public int MainLevelFailCnt
        {
            get
            {
                return mainLevelFailCnt;
            }
            set
            {
                if(mainLevelFailCnt != value)
                {
                    mainLevelFailCnt = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 关卡选择的道具
        [JsonProperty]
        StorageLevelBoost levelBoost = new StorageLevelBoost();
        [JsonIgnore]
        public StorageLevelBoost LevelBoost
        {
            get
            {
                return levelBoost;
            }
        }
        // ---------------------------------//
        
        // 黄金帽
        [JsonProperty]
        StorageGlodenHatter glodenHatter = new StorageGlodenHatter();
        [JsonIgnore]
        public StorageGlodenHatter GlodenHatter
        {
            get
            {
                return glodenHatter;
            }
        }
        // ---------------------------------//
        
        // 等级宝箱
        [JsonProperty]
        StorageLevelChest levelChest = new StorageLevelChest();
        [JsonIgnore]
        public StorageLevelChest LevelChest
        {
            get
            {
                return levelChest;
            }
        }
        // ---------------------------------//
        
        // 限时物品
        [JsonProperty]
        StorageDictionary<int,long> unlimitItemEndUTCTimeInSecondsDict = new StorageDictionary<int,long>();
        [JsonIgnore]
        public StorageDictionary<int,long> UnlimitItemEndUTCTimeInSecondsDict
        {
            get
            {
                return unlimitItemEndUTCTimeInSecondsDict;
            }
        }
        // ---------------------------------//
        
        // 商店购买信息
        [JsonProperty]
        StorageTMShop shop = new StorageTMShop();
        [JsonIgnore]
        public StorageTMShop Shop
        {
            get
            {
                return shop;
            }
        }
        // ---------------------------------//
        
        // 星星宝箱
        [JsonProperty]
        StorageStarChest starChest = new StorageStarChest();
        [JsonIgnore]
        public StorageStarChest StarChest
        {
            get
            {
                return starChest;
            }
        }
        // ---------------------------------//
        
        // 周挑战
        [JsonProperty]
        StorageWeeklyChallenge weeklyChallenge = new StorageWeeklyChallenge();
        [JsonIgnore]
        public StorageWeeklyChallenge WeeklyChallenge
        {
            get
            {
                return weeklyChallenge;
            }
        }
        // ---------------------------------//
         
        // TM战令
        [JsonProperty]
        StorageDictionary<string,StorageEventTMBP> eventTMBP = new StorageDictionary<string,StorageEventTMBP>();
        [JsonIgnore]
        public StorageDictionary<string,StorageEventTMBP> EventTMBP
        {
            get
            {
                return eventTMBP;
            }
        }
        // ---------------------------------//
        
        // 引导
        [JsonProperty]
        StorageDecoGuide decoGuide = new StorageDecoGuide();
        [JsonIgnore]
        public StorageDecoGuide DecoGuide
        {
            get
            {
                return decoGuide;
            }
        }
        // ---------------------------------//
        
        // 引导附属
        [JsonProperty]
        StorageDecorationGuide decorationGuide = new StorageDecorationGuide();
        [JsonIgnore]
        public StorageDecorationGuide DecorationGuide
        {
            get
            {
                return decorationGuide;
            }
        }
        // ---------------------------------//
        
        // 是否已解锁
        [JsonProperty]
        bool isUnlock;
        [JsonIgnore]
        public bool IsUnlock
        {
            get
            {
                return isUnlock;
            }
            set
            {
                if(isUnlock != value)
                {
                    isUnlock = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 
        [JsonProperty]
        StorageRemoveAd removeAd = new StorageRemoveAd();
        [JsonIgnore]
        public StorageRemoveAd RemoveAd
        {
            get
            {
                return removeAd;
            }
        }
        // ---------------------------------//
        
        // 
        [JsonProperty]
        StorageIceBreakingPack iceBreakingPack = new StorageIceBreakingPack();
        [JsonIgnore]
        public StorageIceBreakingPack IceBreakingPack
        {
            get
            {
                return iceBreakingPack;
            }
        }
        // ---------------------------------//
        
        // 
        [JsonProperty]
        StorageReviveGiftPack reviveGiftPack = new StorageReviveGiftPack();
        [JsonIgnore]
        public StorageReviveGiftPack ReviveGiftPack
        {
            get
            {
                return reviveGiftPack;
            }
        }
        // ---------------------------------//
        
        // 复活界面显示（1：复活礼包 2:BP战令）
        [JsonProperty]
        int reviveShowTag;
        [JsonIgnore]
        public int ReviveShowTag
        {
            get
            {
                return reviveShowTag;
            }
            set
            {
                if(reviveShowTag != value)
                {
                    reviveShowTag = value;
                    DragonU3DSDK.Storage.StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
         
        // TM战令
        [JsonProperty]
        StorageDictionary<string,StorageActivityWinStreak> winStreak = new StorageDictionary<string,StorageActivityWinStreak>();
        [JsonIgnore]
        public StorageDictionary<string,StorageActivityWinStreak> WinStreak
        {
            get
            {
                return winStreak;
            }
        }
        // ---------------------------------//
        
    }
}