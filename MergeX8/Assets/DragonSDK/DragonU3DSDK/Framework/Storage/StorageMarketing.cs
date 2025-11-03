/************************************************
 * Storage class : StorageMarketing
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageMarketing : StorageBase
    {
        
        // （市场）TRACKER
        [JsonProperty]
        string tracker = "";
        [JsonIgnore]
        public string Tracker
        {
            get
            {
                return tracker;
            }
            set
            {
                if(tracker != value)
                {
                    tracker = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // （市场）NETWORK
        [JsonProperty]
        string network = "";
        [JsonIgnore]
        public string Network
        {
            get
            {
                return network;
            }
            set
            {
                if(network != value)
                {
                    network = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // （市场）CAMPAIGN
        [JsonProperty]
        string campaign = "";
        [JsonIgnore]
        public string Campaign
        {
            get
            {
                return campaign;
            }
            set
            {
                if(campaign != value)
                {
                    campaign = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // （市场）ADGROUP
        [JsonProperty]
        string adGroup = "";
        [JsonIgnore]
        public string AdGroup
        {
            get
            {
                return adGroup;
            }
            set
            {
                if(adGroup != value)
                {
                    adGroup = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // （市场）CREATIVE
        [JsonProperty]
        string creative = "";
        [JsonIgnore]
        public string Creative
        {
            get
            {
                return creative;
            }
            set
            {
                if(creative != value)
                {
                    creative = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // （市场）fbInstallReferrer
        [JsonProperty]
        string fbInstallReferrer = "";
        [JsonIgnore]
        public string FbInstallReferrer
        {
            get
            {
                return fbInstallReferrer;
            }
            set
            {
                if(fbInstallReferrer != value)
                {
                    fbInstallReferrer = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        
        // （市场）CLICKLABEL
        [JsonProperty]
        string clickLabel = "";
        [JsonIgnore]
        public string ClickLabel
        {
            get
            {
                return clickLabel;
            }
            set
            {
                if(clickLabel != value)
                {
                    clickLabel = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}