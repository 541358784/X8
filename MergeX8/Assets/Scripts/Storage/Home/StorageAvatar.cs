/************************************************
 * Storage class : StorageAvatar
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageAvatar : StorageBase
    {
        
        // 头像ID
        [JsonProperty]
        int avatarIconId;
        [JsonIgnore]
        public int AvatarIconId
        {
            get
            {
                return avatarIconId;
            }
            set
            {
                if(avatarIconId != value)
                {
                    avatarIconId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 玩家名字
        [JsonProperty]
        string userName = "";
        [JsonIgnore]
        public string UserName
        {
            get
            {
                return userName;
            }
            set
            {
                if(userName != value)
                {
                    userName = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 头像框ID
        [JsonProperty]
        int avatarIconFrameId;
        [JsonIgnore]
        public int AvatarIconFrameId
        {
            get
            {
                return avatarIconFrameId;
            }
            set
            {
                if(avatarIconFrameId != value)
                {
                    avatarIconFrameId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 已收集的头像
        [JsonProperty]
        StorageList<int> collectedAvatarList = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> CollectedAvatarList
        {
            get
            {
                return collectedAvatarList;
            }
        }
        // ---------------------------------//
        
        // 未预览的头像
        [JsonProperty]
        StorageList<int> unViewedAvatarList = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> UnViewedAvatarList
        {
            get
            {
                return unViewedAvatarList;
            }
        }
        // ---------------------------------//
        
    }
}