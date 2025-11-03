/************************************************
 * Storage class : StorageDailyRankBotscore
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageDailyRankBotscore : StorageBase
    {
        
        // &lt;80%_MIN
        [JsonProperty]
        int le_Min_80;
        [JsonIgnore]
        public int Le_Min_80
        {
            get
            {
                return le_Min_80;
            }
            set
            {
                if(le_Min_80 != value)
                {
                    le_Min_80 = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // &lt;80%_MAX
        [JsonProperty]
        int le_Max_80;
        [JsonIgnore]
        public int Le_Max_80
        {
            get
            {
                return le_Max_80;
            }
            set
            {
                if(le_Max_80 != value)
                {
                    le_Max_80 = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // &gt;100%_MIN
        [JsonProperty]
        int gr_Min_100;
        [JsonIgnore]
        public int Gr_Min_100
        {
            get
            {
                return gr_Min_100;
            }
            set
            {
                if(gr_Min_100 != value)
                {
                    gr_Min_100 = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // &gt;100%_MAX
        [JsonProperty]
        int gr_Max_100;
        [JsonIgnore]
        public int Gr_Max_100
        {
            get
            {
                return gr_Max_100;
            }
            set
            {
                if(gr_Max_100 != value)
                {
                    gr_Max_100 = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 0_MIN
        [JsonProperty]
        int defaultMin;
        [JsonIgnore]
        public int DefaultMin
        {
            get
            {
                return defaultMin;
            }
            set
            {
                if(defaultMin != value)
                {
                    defaultMin = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 0_MAX
        [JsonProperty]
        int defaultMax;
        [JsonIgnore]
        public int DefaultMax
        {
            get
            {
                return defaultMax;
            }
            set
            {
                if(defaultMax != value)
                {
                    defaultMax = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}