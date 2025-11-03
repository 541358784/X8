/************************************************
 * Storage class : StorageArea
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage.Decoration
{
    [System.Serializable]
    public class StorageArea : StorageBase
    {
         
        // 该区域内的挂点详情
        [JsonProperty]
        StorageDictionary<int,StorageStage> stagesData = new StorageDictionary<int,StorageStage>();
        [JsonIgnore]
        public StorageDictionary<int,StorageStage> StagesData
        {
            get
            {
                return stagesData;
            }
        }
        // ---------------------------------//
        
        // 当前状态
        [JsonProperty]
        int state;
        [JsonIgnore]
        public int State
        {
            get
            {
                return state;
            }
            set
            {
                if(state != value)
                {
                    state = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 当前主题
        [JsonProperty]
        int themeId;
        [JsonIgnore]
        public int ThemeId
        {
            get
            {
                return themeId;
            }
            set
            {
                if(themeId != value)
                {
                    themeId = value;
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
        
    }
}