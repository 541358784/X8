/************************************************
 * Storage class : StorageConfigHub
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageConfigHub : StorageBase
    {
        
        // 模块GUID
        [JsonProperty]
        string module = "";
        [JsonIgnore]
        public string Module
        {
            get
            {
                return module;
            }
            set
            {
                if(module != value)
                {
                    module = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 版本号
        [JsonProperty]
        int versionIOS;
        [JsonIgnore]
        public int VersionIOS
        {
            get
            {
                return versionIOS;
            }
            set
            {
                if(versionIOS != value)
                {
                    versionIOS = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 版本号
        [JsonProperty]
        int versionAndroid;
        [JsonIgnore]
        public int VersionAndroid
        {
            get
            {
                return versionAndroid;
            }
            set
            {
                if(versionAndroid != value)
                {
                    versionAndroid = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 分层ID
        [JsonProperty]
        int userGroup;
        [JsonIgnore]
        public int UserGroup
        {
            get
            {
                return userGroup;
            }
            set
            {
                if(userGroup != value)
                {
                    userGroup = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 缓存创建时间
        [JsonProperty]
        long createTime;
        [JsonIgnore]
        public long CreateTime
        {
            get
            {
                return createTime;
            }
            set
            {
                if(createTime != value)
                {
                    createTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 持续时间
        [JsonProperty]
        long duration;
        [JsonIgnore]
        public long Duration
        {
            get
            {
                return duration;
            }
            set
            {
                if(duration != value)
                {
                    duration = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}