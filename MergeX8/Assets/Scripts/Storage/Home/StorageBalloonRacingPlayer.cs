/************************************************
 * Storage class : StorageBalloonRacingPlayer
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageBalloonRacingPlayer : StorageBase
    {
        
        // 名字
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
        
        // 头像
        [JsonProperty]
        int playerHead;
        [JsonIgnore]
        public int PlayerHead
        {
            get
            {
                return playerHead;
            }
            set
            {
                if(playerHead != value)
                {
                    playerHead = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否是自己
        [JsonProperty]
        bool isMe;
        [JsonIgnore]
        public bool IsMe
        {
            get
            {
                return isMe;
            }
            set
            {
                if(isMe != value)
                {
                    isMe = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
         
        // 积分变化列表
        [JsonProperty]
        StorageDictionary<ulong,int> addScoreList = new StorageDictionary<ulong,int>();
        [JsonIgnore]
        public StorageDictionary<ulong,int> AddScoreList
        {
            get
            {
                return addScoreList;
            }
        }
        // ---------------------------------//
        
        // 上次积分
        [JsonProperty]
        int lastScore;
        [JsonIgnore]
        public int LastScore
        {
            get
            {
                return lastScore;
            }
            set
            {
                if(lastScore != value)
                {
                    lastScore = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前积分
        [JsonProperty]
        int curScore;
        [JsonIgnore]
        public int CurScore
        {
            get
            {
                return curScore;
            }
            set
            {
                if(curScore != value)
                {
                    curScore = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 刷新时间
        [JsonProperty]
        ulong curRefreshTime;
        [JsonIgnore]
        public ulong CurRefreshTime
        {
            get
            {
                return curRefreshTime;
            }
            set
            {
                if(curRefreshTime != value)
                {
                    curRefreshTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否已经完成
        [JsonProperty]
        bool isDone;
        [JsonIgnore]
        public bool IsDone
        {
            get
            {
                return isDone;
            }
            set
            {
                if(isDone != value)
                {
                    isDone = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 位置
        [JsonProperty]
        int seat;
        [JsonIgnore]
        public int Seat
        {
            get
            {
                return seat;
            }
            set
            {
                if(seat != value)
                {
                    seat = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 上次打开界面时积分
        [JsonProperty]
        int lastShowScore;
        [JsonIgnore]
        public int LastShowScore
        {
            get
            {
                return lastShowScore;
            }
            set
            {
                if(lastShowScore != value)
                {
                    lastShowScore = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 上次打开界面排行
        [JsonProperty]
        int lastShowRank;
        [JsonIgnore]
        public int LastShowRank
        {
            get
            {
                return lastShowRank;
            }
            set
            {
                if(lastShowRank != value)
                {
                    lastShowRank = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}