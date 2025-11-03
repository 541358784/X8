/************************************************
 * Storage class : StorageRecoverCoinWeek
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageRecoverCoinWeek : StorageBase
    {
        
        // 皮肤名
        [JsonProperty]
        string skinName = "";
        [JsonIgnore]
        public string SkinName
        {
            get
            {
                return skinName;
            }
            set
            {
                if(skinName != value)
                {
                    skinName = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 周ID
        [JsonProperty]
        int weekId;
        [JsonIgnore]
        public int WeekId
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
        
        // 是否已开始
        [JsonProperty]
        bool isStart;
        [JsonIgnore]
        public bool IsStart
        {
            get
            {
                return isStart;
            }
            set
            {
                if(isStart != value)
                {
                    isStart = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否已结算
        [JsonProperty]
        bool isFinish;
        [JsonIgnore]
        public bool IsFinish
        {
            get
            {
                return isFinish;
            }
            set
            {
                if(isFinish != value)
                {
                    isFinish = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 星星数量
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
        
        // 参与活动的玩家数量
        [JsonProperty]
        int maxPlayerCount;
        [JsonIgnore]
        public int MaxPlayerCount
        {
            get
            {
                return maxPlayerCount;
            }
            set
            {
                if(maxPlayerCount != value)
                {
                    maxPlayerCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 生成机器人的区间编号
        [JsonProperty]
        int robotIndex;
        [JsonIgnore]
        public int RobotIndex
        {
            get
            {
                return robotIndex;
            }
            set
            {
                if(robotIndex != value)
                {
                    robotIndex = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 玩家入榜时的金币存量分组编号
        [JsonProperty]
        int playerCoinCountGroupIndex;
        [JsonIgnore]
        public int PlayerCoinCountGroupIndex
        {
            get
            {
                return playerCoinCountGroupIndex;
            }
            set
            {
                if(playerCoinCountGroupIndex != value)
                {
                    playerCoinCountGroupIndex = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 机器人列表
        [JsonProperty]
        StorageList<StorageRecoverCoinRobot> robotList = new StorageList<StorageRecoverCoinRobot>();
        [JsonIgnore]
        public StorageList<StorageRecoverCoinRobot> RobotList
        {
            get
            {
                return robotList;
            }
        }
        // ---------------------------------//
         
        // 玩家列表,KEY为PLAYERID
        [JsonProperty]
        StorageDictionary<ulong,StorageRecoverCoinPlayer> playerList = new StorageDictionary<ulong,StorageRecoverCoinPlayer>();
        [JsonIgnore]
        public StorageDictionary<ulong,StorageRecoverCoinPlayer> PlayerList
        {
            get
            {
                return playerList;
            }
        }
        // ---------------------------------//
        
        // 服务器记录的自己所属榜单ID
        [JsonProperty]
        string leaderBoardId = "";
        [JsonIgnore]
        public string LeaderBoardId
        {
            get
            {
                return leaderBoardId;
            }
            set
            {
                if(leaderBoardId != value)
                {
                    leaderBoardId = value;
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
        
        // 开始时间戳(如果活动开启则进行更新，否则用本地存的)
        [JsonProperty]
        long startTime;
        [JsonIgnore]
        public long StartTime
        {
            get
            {
                return startTime;
            }
            set
            {
                if(startTime != value)
                {
                    startTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 结束时间戳(同STARTTIME)
        [JsonProperty]
        long endTime;
        [JsonIgnore]
        public long EndTime
        {
            get
            {
                return endTime;
            }
            set
            {
                if(endTime != value)
                {
                    endTime = value;
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
        
        // 触发活动时的ACTIVITYID
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
        
        // RECOVERCOINREWARDCONFIG转JSON
        [JsonProperty]
        string jsonRecoverCoinRewardConfig = "";
        [JsonIgnore]
        public string JsonRecoverCoinRewardConfig
        {
            get
            {
                return jsonRecoverCoinRewardConfig;
            }
            set
            {
                if(jsonRecoverCoinRewardConfig != value)
                {
                    jsonRecoverCoinRewardConfig = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // RECOVERCOINEXCHANGESTARCONFIG转JSON
        [JsonProperty]
        string jsonRecoverCoinExchangeStarConfig = "";
        [JsonIgnore]
        public string JsonRecoverCoinExchangeStarConfig
        {
            get
            {
                return jsonRecoverCoinExchangeStarConfig;
            }
            set
            {
                if(jsonRecoverCoinExchangeStarConfig != value)
                {
                    jsonRecoverCoinExchangeStarConfig = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // RECOVERCOINROBOTMINSTARUPDATEINTERVALCONFIG转JSON
        [JsonProperty]
        string jsonRecoverCoinRobotMinStarUpdateIntervalConfig = "";
        [JsonIgnore]
        public string JsonRecoverCoinRobotMinStarUpdateIntervalConfig
        {
            get
            {
                return jsonRecoverCoinRobotMinStarUpdateIntervalConfig;
            }
            set
            {
                if(jsonRecoverCoinRobotMinStarUpdateIntervalConfig != value)
                {
                    jsonRecoverCoinRobotMinStarUpdateIntervalConfig = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}