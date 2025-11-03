/************************************************
 * Storage class : StorageCommon
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageCommon : StorageBase
    {
        
        // 
        [JsonProperty]
        ulong playerId;
        [JsonIgnore]
        public ulong PlayerId
        {
            get
            {
                return playerId;
            }
            set
            {
                if(playerId != value)
                {
                    playerId = value;
                    StorageManager.Instance.LocalVersion++;
                    StorageManager.Instance.SyncForce = true;
                    
                }
            }
        }
        // ---------------------------------//
        
        // 
        [JsonProperty]
        string deviceId = "";
        [JsonIgnore]
        public string DeviceId
        {
            get
            {
                return deviceId;
            }
            set
            {
                if(deviceId != value)
                {
                    deviceId = value;
                    StorageManager.Instance.LocalVersion++;
                    StorageManager.Instance.SyncForce = true;
                    
                }
            }
        }
        // ---------------------------------//
        
        // 
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
                    StorageManager.Instance.SyncForce = true;
                    
                }
            }
        }
        // ---------------------------------//
        
        // 
        [JsonProperty]
        string facebookId = "";
        [JsonIgnore]
        public string FacebookId
        {
            get
            {
                return facebookId;
            }
            set
            {
                if(facebookId != value)
                {
                    facebookId = value;
                    StorageManager.Instance.LocalVersion++;
                    StorageManager.Instance.SyncForce = true;
                    
                }
            }
        }
        // ---------------------------------//
        
        // 
        [JsonProperty]
        string facebookName = "";
        [JsonIgnore]
        public string FacebookName
        {
            get
            {
                return facebookName;
            }
            set
            {
                if(facebookName != value)
                {
                    facebookName = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 
        [JsonProperty]
        string facebookEmail = "";
        [JsonIgnore]
        public string FacebookEmail
        {
            get
            {
                return facebookEmail;
            }
            set
            {
                if(facebookEmail != value)
                {
                    facebookEmail = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 
        [JsonProperty]
        string name = "";
        [JsonIgnore]
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if(name != value)
                {
                    name = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 
        [JsonProperty]
        uint revenueCount;
        [JsonIgnore]
        public uint RevenueCount
        {
            get
            {
                return revenueCount;
            }
            set
            {
                if(revenueCount != value)
                {
                    revenueCount = value;
                    StorageManager.Instance.LocalVersion++;
                    StorageManager.Instance.SyncForce = true;
                    
                }
            }
        }
        // ---------------------------------//
        
        // 
        [JsonProperty]
        ulong revenueUSDCents;
        [JsonIgnore]
        public ulong RevenueUSDCents
        {
            get
            {
                return revenueUSDCents;
            }
            set
            {
                if(revenueUSDCents != value)
                {
                    revenueUSDCents = value;
                    StorageManager.Instance.LocalVersion++;
                    StorageManager.Instance.SyncForce = true;
                    
                }
            }
        }
        // ---------------------------------//
        
        // 
        [JsonProperty]
        ulong lastRevenueTime;
        [JsonIgnore]
        public ulong LastRevenueTime
        {
            get
            {
                return lastRevenueTime;
            }
            set
            {
                if(lastRevenueTime != value)
                {
                    lastRevenueTime = value;
                    StorageManager.Instance.LocalVersion++;
                    StorageManager.Instance.SyncForce = true;
                    
                }
            }
        }
        // ---------------------------------//
        
        // 
        [JsonProperty]
        ulong installedAt;
        [JsonIgnore]
        public ulong InstalledAt
        {
            get
            {
                return installedAt;
            }
            set
            {
                if(installedAt != value)
                {
                    installedAt = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 
        [JsonProperty]
        ulong updatedAt;
        [JsonIgnore]
        public ulong UpdatedAt
        {
            get
            {
                return updatedAt;
            }
            set
            {
                if(updatedAt != value)
                {
                    updatedAt = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 
        [JsonProperty]
        string adjustId = "";
        [JsonIgnore]
        public string AdjustId
        {
            get
            {
                return adjustId;
            }
            set
            {
                if(adjustId != value)
                {
                    adjustId = value;
                    StorageManager.Instance.LocalVersion++;
                    StorageManager.Instance.SyncForce = true;
                    
                }
            }
        }
        // ---------------------------------//
        
        // 
        [JsonProperty]
        string adid = "";
        [JsonIgnore]
        public string Adid
        {
            get
            {
                return adid;
            }
            set
            {
                if(adid != value)
                {
                    adid = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 
        [JsonProperty]
        string idfa = "";
        [JsonIgnore]
        public string Idfa
        {
            get
            {
                return idfa;
            }
            set
            {
                if(idfa != value)
                {
                    idfa = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 
        [JsonProperty]
        string idfv = "";
        [JsonIgnore]
        public string Idfv
        {
            get
            {
                return idfv;
            }
            set
            {
                if(idfv != value)
                {
                    idfv = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 
        [JsonProperty]
        string gaid = "";
        [JsonIgnore]
        public string Gaid
        {
            get
            {
                return gaid;
            }
            set
            {
                if(gaid != value)
                {
                    gaid = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 
        [JsonProperty]
        int platform;
        [JsonIgnore]
        public int Platform
        {
            get
            {
                return platform;
            }
            set
            {
                if(platform != value)
                {
                    platform = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 
        [JsonProperty]
        string locale = "";
        [JsonIgnore]
        public string Locale
        {
            get
            {
                return locale;
            }
            set
            {
                if(locale != value)
                {
                    locale = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 
        [JsonProperty]
        string origLocale = "";
        [JsonIgnore]
        public string OrigLocale
        {
            get
            {
                return origLocale;
            }
            set
            {
                if(origLocale != value)
                {
                    origLocale = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 
        [JsonProperty]
        string country = "";
        [JsonIgnore]
        public string Country
        {
            get
            {
                return country;
            }
            set
            {
                if(country != value)
                {
                    country = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 
        [JsonProperty]
        string region = "";
        [JsonIgnore]
        public string Region
        {
            get
            {
                return region;
            }
            set
            {
                if(region != value)
                {
                    region = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 
        [JsonProperty]
        string timeZone = "";
        [JsonIgnore]
        public string TimeZone
        {
            get
            {
                return timeZone;
            }
            set
            {
                if(timeZone != value)
                {
                    timeZone = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 
        [JsonProperty]
        string resVersion = "";
        [JsonIgnore]
        public string ResVersion
        {
            get
            {
                return resVersion;
            }
            set
            {
                if(resVersion != value)
                {
                    resVersion = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 
        [JsonProperty]
        string naviveVersion = "";
        [JsonIgnore]
        public string NaviveVersion
        {
            get
            {
                return naviveVersion;
            }
            set
            {
                if(naviveVersion != value)
                {
                    naviveVersion = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 
        [JsonProperty]
        string deviceModel = "";
        [JsonIgnore]
        public string DeviceModel
        {
            get
            {
                return deviceModel;
            }
            set
            {
                if(deviceModel != value)
                {
                    deviceModel = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 
        [JsonProperty]
        string deviceType = "";
        [JsonIgnore]
        public string DeviceType
        {
            get
            {
                return deviceType;
            }
            set
            {
                if(deviceType != value)
                {
                    deviceType = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 
        [JsonProperty]
        string deviceMemory = "";
        [JsonIgnore]
        public string DeviceMemory
        {
            get
            {
                return deviceMemory;
            }
            set
            {
                if(deviceMemory != value)
                {
                    deviceMemory = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 
        [JsonProperty]
        string inviteCode = "";
        [JsonIgnore]
        public string InviteCode
        {
            get
            {
                return inviteCode;
            }
            set
            {
                if(inviteCode != value)
                {
                    inviteCode = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 
        [JsonProperty]
        StorageDictionary<string,string> abtests = new StorageDictionary<string,string>();
        [JsonIgnore]
        public StorageDictionary<string,string> Abtests
        {
            get
            {
                return abtests;
            }
        }
        // ---------------------------------//
        
        // 
        [JsonProperty]
        string firebaseInstanceId = "";
        [JsonIgnore]
        public string FirebaseInstanceId
        {
            get
            {
                return firebaseInstanceId;
            }
            set
            {
                if(firebaseInstanceId != value)
                {
                    firebaseInstanceId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 
        [JsonProperty]
        string appleAccountId = "";
        [JsonIgnore]
        public string AppleAccountId
        {
            get
            {
                return appleAccountId;
            }
            set
            {
                if(appleAccountId != value)
                {
                    appleAccountId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 广告预测用户组
        [JsonProperty]
        int adsPredictUserGroup;
        [JsonIgnore]
        public int AdsPredictUserGroup
        {
            get
            {
                return adsPredictUserGroup;
            }
            set
            {
                if(adsPredictUserGroup != value)
                {
                    adsPredictUserGroup = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 市场追踪相关
        [JsonProperty]
        StorageMarketing marketing = new StorageMarketing();
        [JsonIgnore]
        public StorageMarketing Marketing
        {
            get
            {
                return marketing;
            }
        }
        // ---------------------------------//
        
        // 市场广告分组
        [JsonProperty]
        bool adsUserGroupMarketingOverride;
        [JsonIgnore]
        public bool AdsUserGroupMarketingOverride
        {
            get
            {
                return adsUserGroupMarketingOverride;
            }
            set
            {
                if(adsUserGroupMarketingOverride != value)
                {
                    adsUserGroupMarketingOverride = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 玩家活跃统计数据
        [JsonProperty]
        StorageActiveData activeData = new StorageActiveData();
        [JsonIgnore]
        public StorageActiveData ActiveData
        {
            get
            {
                return activeData;
            }
        }
        // ---------------------------------//
        
        // 广告计数重置时间
        [JsonProperty]
        long adCountResetTime;
        [JsonIgnore]
        public long AdCountResetTime
        {
            get
            {
                return adCountResetTime;
            }
            set
            {
                if(adCountResetTime != value)
                {
                    adCountResetTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 广告计数阶段状态
        [JsonProperty]
        int adCountStageState;
        [JsonIgnore]
        public int AdCountStageState
        {
            get
            {
                return adCountStageState;
            }
            set
            {
                if(adCountStageState != value)
                {
                    adCountStageState = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 激励视频累计观看次数
        [JsonProperty]
        int adRewardWatchedCount;
        [JsonIgnore]
        public int AdRewardWatchedCount
        {
            get
            {
                return adRewardWatchedCount;
            }
            set
            {
                if(adRewardWatchedCount != value)
                {
                    adRewardWatchedCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 服务器分组结果
        [JsonProperty]
        StorageDictionary<string,uint> remoteGroupIdDatas = new StorageDictionary<string,uint>();
        [JsonIgnore]
        public StorageDictionary<string,uint> RemoteGroupIdDatas
        {
            get
            {
                return remoteGroupIdDatas;
            }
        }
        // ---------------------------------//
         
        // 配置中心缓存，以模块为单位
        [JsonProperty]
        StorageDictionary<string,StorageConfigHub> configHub = new StorageDictionary<string,StorageConfigHub>();
        [JsonIgnore]
        public StorageDictionary<string,StorageConfigHub> ConfigHub
        {
            get
            {
                return configHub;
            }
        }
        // ---------------------------------//
        
        // SOURCECAMPAIGN
        [JsonProperty]
        int campaignTypeCode;
        [JsonIgnore]
        public int CampaignTypeCode
        {
            get
            {
                return campaignTypeCode;
            }
            set
            {
                if(campaignTypeCode != value)
                {
                    campaignTypeCode = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 拓展,各项目自己维护key value
        [JsonProperty]
        StorageDictionary<string,string> extensions = new StorageDictionary<string,string>(true, true);
        [JsonIgnore]
        public StorageDictionary<string,string> Extensions
        {
            get
            {
                return extensions;
            }
        }
        // ---------------------------------//
        
    }
}