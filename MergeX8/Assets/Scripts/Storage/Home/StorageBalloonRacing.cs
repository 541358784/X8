/************************************************
 * Storage class : StorageBalloonRacing
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageBalloonRacing : StorageBase
    {
        
        // KEY
        [JsonProperty]
        string activityId = "";
        [JsonIgnore]
        public string ActivityId
        {
            get
            {
                return activityId;
            }
            set
            {
                if(activityId != value)
                {
                    activityId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 加入时间 
        [JsonProperty]
        long joinTime;
        [JsonIgnore]
        public long JoinTime
        {
            get
            {
                return joinTime;
            }
            set
            {
                if(joinTime != value)
                {
                    joinTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否加入
        [JsonProperty]
        bool isJoin;
        [JsonIgnore]
        public bool IsJoin
        {
            get
            {
                return isJoin;
            }
            set
            {
                if(isJoin != value)
                {
                    isJoin = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 玩家列表
        [JsonProperty]
        StorageList<StorageBalloonRacingPlayer> playerList = new StorageList<StorageBalloonRacingPlayer>();
        [JsonIgnore]
        public StorageList<StorageBalloonRacingPlayer> PlayerList
        {
            get
            {
                return playerList;
            }
        }
        // ---------------------------------//
        
        // 运行轮次
        [JsonProperty]
        int runRounds;
        [JsonIgnore]
        public int RunRounds
        {
            get
            {
                return runRounds;
            }
            set
            {
                if(runRounds != value)
                {
                    runRounds = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前竞速下标
        [JsonProperty]
        int curRacingIndex;
        [JsonIgnore]
        public int CurRacingIndex
        {
            get
            {
                return curRacingIndex;
            }
            set
            {
                if(curRacingIndex != value)
                {
                    curRacingIndex = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 结算列表
        [JsonProperty]
        StorageList<int> doneList = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> DoneList
        {
            get
            {
                return doneList;
            }
        }
        // ---------------------------------//
        
        // 是否结束
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
        
        // 是否结算
        [JsonProperty]
        bool isAward;
        [JsonIgnore]
        public bool IsAward
        {
            get
            {
                return isAward;
            }
            set
            {
                if(isAward != value)
                {
                    isAward = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}