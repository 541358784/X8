/************************************************
 * Storage class : StorageCommonLeaderBoard
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageCommonLeaderBoard : StorageBase
    {
        
        // 排行榜关键字
        [JsonProperty]
        string leaderBoardKeyWord = "";
        [JsonIgnore]
        public string LeaderBoardKeyWord
        {
            get
            {
                return leaderBoardKeyWord;
            }
            set
            {
                if(leaderBoardKeyWord != value)
                {
                    leaderBoardKeyWord = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 活动ID
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
        
        // REWARDCONFIG转JSON
        [JsonProperty]
        string jsonRewardConfig = "";
        [JsonIgnore]
        public string JsonRewardConfig
        {
            get
            {
                return jsonRewardConfig;
            }
            set
            {
                if(jsonRewardConfig != value)
                {
                    jsonRewardConfig = value;
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
        
        // 活动资源的文件名
        [JsonProperty]
        StorageList<string> activityResList = new StorageList<string>();
        [JsonIgnore]
        public StorageList<string> ActivityResList
        {
            get
            {
                return activityResList;
            }
        }
        // ---------------------------------//
        
        // 活动资源的下载路径
        [JsonProperty]
        StorageList<string> activityResMd5List = new StorageList<string>();
        [JsonIgnore]
        public StorageList<string> ActivityResMd5List
        {
            get
            {
                return activityResMd5List;
            }
        }
        // ---------------------------------//
        
        // 最少入榜分数
        [JsonProperty]
        int leastStarCount;
        [JsonIgnore]
        public int LeastStarCount
        {
            get
            {
                return leastStarCount;
            }
            set
            {
                if(leastStarCount != value)
                {
                    leastStarCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 大厅入口资源路径
        [JsonProperty]
        string auxItemAssetPath = "";
        [JsonIgnore]
        public string AuxItemAssetPath
        {
            get
            {
                return auxItemAssetPath;
            }
            set
            {
                if(auxItemAssetPath != value)
                {
                    auxItemAssetPath = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 任务栏入口资源路径
        [JsonProperty]
        string taskEntranceAssetPath = "";
        [JsonIgnore]
        public string TaskEntranceAssetPath
        {
            get
            {
                return taskEntranceAssetPath;
            }
            set
            {
                if(taskEntranceAssetPath != value)
                {
                    taskEntranceAssetPath = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 主界面弹窗资源路径
        [JsonProperty]
        string mainPopupAssetPath = "";
        [JsonIgnore]
        public string MainPopupAssetPath
        {
            get
            {
                return mainPopupAssetPath;
            }
            set
            {
                if(mainPopupAssetPath != value)
                {
                    mainPopupAssetPath = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 收集类型资源ID
        [JsonProperty]
        int collectItemResourceId;
        [JsonIgnore]
        public int CollectItemResourceId
        {
            get
            {
                return collectItemResourceId;
            }
            set
            {
                if(collectItemResourceId != value)
                {
                    collectItemResourceId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}