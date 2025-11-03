/************************************************
 * Storage class : StorageAdJudgingData
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageAdJudgingData : StorageBase
    {
        
        // 进入该分组活跃天数
        [JsonProperty]
        int enterGroupActiveDay;
        [JsonIgnore]
        public int EnterGroupActiveDay
        {
            get
            {
                return enterGroupActiveDay;
            }
            set
            {
                if(enterGroupActiveDay != value)
                {
                    enterGroupActiveDay = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 累计完成任务
        [JsonProperty]
        int completeOrderNum;
        [JsonIgnore]
        public int CompleteOrderNum
        {
            get
            {
                return completeOrderNum;
            }
            set
            {
                if(completeOrderNum != value)
                {
                    completeOrderNum = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 关闭SHOP次数
        [JsonProperty]
        int closeShopNum;
        [JsonIgnore]
        public int CloseShopNum
        {
            get
            {
                return closeShopNum;
            }
            set
            {
                if(closeShopNum != value)
                {
                    closeShopNum = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 跳过RV次数
        [JsonProperty]
        int skipRvNum;
        [JsonIgnore]
        public int SkipRvNum
        {
            get
            {
                return skipRvNum;
            }
            set
            {
                if(skipRvNum != value)
                {
                    skipRvNum = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 体力消耗总数
        [JsonProperty]
        int energyNum;
        [JsonIgnore]
        public int EnergyNum
        {
            get
            {
                return energyNum;
            }
            set
            {
                if(energyNum != value)
                {
                    energyNum = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}