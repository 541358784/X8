/************************************************
 * Storage class : StorageRecoverCoinPlayer
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageRecoverCoinPlayer : StorageBase
    {
        
        // 星星数
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
        
        // 星星数量更新的时间戳
        [JsonProperty]
        ulong starUpdateTime;
        [JsonIgnore]
        public ulong StarUpdateTime
        {
            get
            {
                return starUpdateTime;
            }
            set
            {
                if(starUpdateTime != value)
                {
                    starUpdateTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 玩家PLAYERID
        [JsonProperty]
        ulong playerId;
        [JsonIgnore]
        public ulong PlayerId
        {
            get
            {
                return playerId;
            }
            set
            {
                if(playerId != value)
                {
                    playerId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 玩家名字
        [JsonProperty]
        string playerName = "";
        [JsonIgnore]
        public string PlayerName
        {
            get
            {
                return playerName;
            }
            set
            {
                if(playerName != value)
                {
                    playerName = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 玩家头像ID
        [JsonProperty]
        int avatarIconId;
        [JsonIgnore]
        public int AvatarIconId
        {
            get
            {
                return avatarIconId;
            }
            set
            {
                if(avatarIconId != value)
                {
                    avatarIconId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 完成星星任务的数量
        [JsonProperty]
        int completedTaskCount;
        [JsonIgnore]
        public int CompletedTaskCount
        {
            get
            {
                return completedTaskCount;
            }
            set
            {
                if(completedTaskCount != value)
                {
                    completedTaskCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 购买星星次数
        [JsonProperty]
        int buyTimes;
        [JsonIgnore]
        public int BuyTimes
        {
            get
            {
                return buyTimes;
            }
            set
            {
                if(buyTimes != value)
                {
                    buyTimes = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否更新最终的数据
        [JsonProperty]
        bool isUpdateFinalData;
        [JsonIgnore]
        public bool IsUpdateFinalData
        {
            get
            {
                return isUpdateFinalData;
            }
            set
            {
                if(isUpdateFinalData != value)
                {
                    isUpdateFinalData = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}