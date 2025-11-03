/************************************************
 * Storage class : StorageRecoverCoinRobot
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageRecoverCoinRobot : StorageBase
    {
        
        // 七天星星总数
        [JsonProperty]
        int maxStarCount;
        [JsonIgnore]
        public int MaxStarCount
        {
            get
            {
                return maxStarCount;
            }
            set
            {
                if(maxStarCount != value)
                {
                    maxStarCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 机器人编号
        [JsonProperty]
        int id;
        [JsonIgnore]
        public int Id
        {
            get
            {
                return id;
            }
            set
            {
                if(id != value)
                {
                    id = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 机器人名字
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
        
        // 机器人头像ID
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
        
        // 上一次刷新的星星数
        [JsonProperty]
        int lastUpdateStarCount;
        [JsonIgnore]
        public int LastUpdateStarCount
        {
            get
            {
                return lastUpdateStarCount;
            }
            set
            {
                if(lastUpdateStarCount != value)
                {
                    lastUpdateStarCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 下一次刷新的间隔
        [JsonProperty]
        int nextUpdateInterval;
        [JsonIgnore]
        public int NextUpdateInterval
        {
            get
            {
                return nextUpdateInterval;
            }
            set
            {
                if(nextUpdateInterval != value)
                {
                    nextUpdateInterval = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}