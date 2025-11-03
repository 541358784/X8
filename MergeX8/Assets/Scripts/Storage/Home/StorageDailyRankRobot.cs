/************************************************
 * Storage class : StorageDailyRankRobot
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageDailyRankRobot : StorageBase
    {
        
        // 当前分数
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
        
        // 名字
        [JsonProperty]
        string robotName = "";
        [JsonIgnore]
        public string RobotName
        {
            get
            {
                return robotName;
            }
            set
            {
                if(robotName != value)
                {
                    robotName = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 头像INDEX
        [JsonProperty]
        int headIndex;
        [JsonIgnore]
        public int HeadIndex
        {
            get
            {
                return headIndex;
            }
            set
            {
                if(headIndex != value)
                {
                    headIndex = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}