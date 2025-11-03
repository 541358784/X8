/************************************************
 * Storage class : StorageBattlePassTask
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageBattlePassTask : StorageBase
    {
        
        // 刷新时间
        [JsonProperty]
        long refreshTime;
        [JsonIgnore]
        public long RefreshTime
        {
            get
            {
                return refreshTime;
            }
            set
            {
                if(refreshTime != value)
                {
                    refreshTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 领取奖励时间
        [JsonProperty]
        long getRewardTime;
        [JsonIgnore]
        public long GetRewardTime
        {
            get
            {
                return getRewardTime;
            }
            set
            {
                if(getRewardTime != value)
                {
                    getRewardTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 完成的任务 ID NUM
        [JsonProperty]
        StorageDictionary<int,int> completeDatas = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> CompleteDatas
        {
            get
            {
                return completeDatas;
            }
        }
        // ---------------------------------//
        
        // 每日任务
        [JsonProperty]
        StorageBattlePassTaskData dailyTask = new StorageBattlePassTaskData();
        [JsonIgnore]
        public StorageBattlePassTaskData DailyTask
        {
            get
            {
                return dailyTask;
            }
        }
        // ---------------------------------//
        
        // 挑战任务
        [JsonProperty]
        StorageBattlePassTaskData challengeTask = new StorageBattlePassTaskData();
        [JsonIgnore]
        public StorageBattlePassTaskData ChallengeTask
        {
            get
            {
                return challengeTask;
            }
        }
        // ---------------------------------//
        
        // 固定任务
        [JsonProperty]
        StorageBattlePassTaskData fixationTask = new StorageBattlePassTaskData();
        [JsonIgnore]
        public StorageBattlePassTaskData FixationTask
        {
            get
            {
                return fixationTask;
            }
        }
        // ---------------------------------//
        
    }
}