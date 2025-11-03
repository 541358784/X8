/************************************************
 * Storage class : StorageTeam
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageTeam : StorageBase
    {
        
        // 公会ID
        [JsonProperty]
        long lastTeamId;
        [JsonIgnore]
        public long LastTeamId
        {
            get
            {
                return lastTeamId;
            }
            set
            {
                if(lastTeamId != value)
                {
                    lastTeamId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 公会名
        [JsonProperty]
        string lastTeamName = "";
        [JsonIgnore]
        public string LastTeamName
        {
            get
            {
                return lastTeamName;
            }
            set
            {
                if(lastTeamName != value)
                {
                    lastTeamName = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 最后一次系统消息时间戳
        [JsonProperty]
        long lastPersonnelChangesChatMessageTimestamp;
        [JsonIgnore]
        public long LastPersonnelChangesChatMessageTimestamp
        {
            get
            {
                return lastPersonnelChangesChatMessageTimestamp;
            }
            set
            {
                if(lastPersonnelChangesChatMessageTimestamp != value)
                {
                    lastPersonnelChangesChatMessageTimestamp = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 最后一次刷新上线时间时间戳
        [JsonProperty]
        long lastPingTeamTimestamp;
        [JsonIgnore]
        public long LastPingTeamTimestamp
        {
            get
            {
                return lastPingTeamTimestamp;
            }
            set
            {
                if(lastPingTeamTimestamp != value)
                {
                    lastPingTeamTimestamp = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 已领取的卡包GIFTID_TEAMID
        [JsonProperty]
        StorageList<string> claimCardState = new StorageList<string>();
        [JsonIgnore]
        public StorageList<string> ClaimCardState
        {
            get
            {
                return claimCardState;
            }
        }
        // ---------------------------------//
        
        // 生命值
        [JsonProperty]
        int life;
        [JsonIgnore]
        public int Life
        {
            get
            {
                return life;
            }
            set
            {
                if(life != value)
                {
                    life = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 生命值更新时间
        [JsonProperty]
        long lifeUpdateTime;
        [JsonIgnore]
        public long LifeUpdateTime
        {
            get
            {
                return lifeUpdateTime;
            }
            set
            {
                if(lifeUpdateTime != value)
                {
                    lifeUpdateTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 刷新任务时间
        [JsonProperty]
        long refreshOrderTime;
        [JsonIgnore]
        public long RefreshOrderTime
        {
            get
            {
                return refreshOrderTime;
            }
            set
            {
                if(refreshOrderTime != value)
                {
                    refreshOrderTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 公会筹
        [JsonProperty]
        int coin;
        [JsonIgnore]
        public int Coin
        {
            get
            {
                return coin;
            }
            set
            {
                if(coin != value)
                {
                    coin = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 周ID
        [JsonProperty]
        long weekId;
        [JsonIgnore]
        public long WeekId
        {
            get
            {
                return weekId;
            }
            set
            {
                if(weekId != value)
                {
                    weekId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 购买状态
        [JsonProperty]
        StorageDictionary<int,int> buyState = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> BuyState
        {
            get
            {
                return buyState;
            }
        }
        // ---------------------------------//
        
        // 抽卡保底次数
        [JsonProperty]
        int cardFailTimes;
        [JsonIgnore]
        public int CardFailTimes
        {
            get
            {
                return cardFailTimes;
            }
            set
            {
                if(cardFailTimes != value)
                {
                    cardFailTimes = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}