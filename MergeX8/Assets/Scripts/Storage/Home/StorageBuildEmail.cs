/************************************************
 * Storage class : StorageBuildEmail
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageBuildEmail : StorageBase
    {
        
        // 邮件
        [JsonProperty]
        string email = "";
        [JsonIgnore]
        public string Email
        {
            get
            {
                return email;
            }
            set
            {
                if(email != value)
                {
                    email = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前阶段
        [JsonProperty]
        int stage;
        [JsonIgnore]
        public int Stage
        {
            get
            {
                return stage;
            }
            set
            {
                if(stage != value)
                {
                    stage = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 是否领取奖励
        [JsonProperty]
        bool isGetReward;
        [JsonIgnore]
        public bool IsGetReward
        {
            get
            {
                return isGetReward;
            }
            set
            {
                if(isGetReward != value)
                {
                    isGetReward = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 主动弹出次数
        [JsonProperty]
        int autoPopNum;
        [JsonIgnore]
        public int AutoPopNum
        {
            get
            {
                return autoPopNum;
            }
            set
            {
                if(autoPopNum != value)
                {
                    autoPopNum = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 更新隐私协议
        [JsonProperty]
        bool updatePrivacyPolicy;
        [JsonIgnore]
        public bool UpdatePrivacyPolicy
        {
            get
            {
                return updatePrivacyPolicy;
            }
            set
            {
                if(updatePrivacyPolicy != value)
                {
                    updatePrivacyPolicy = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}