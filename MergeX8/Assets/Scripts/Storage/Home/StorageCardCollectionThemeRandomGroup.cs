/************************************************
 * Storage class : StorageCardCollectionThemeRandomGroup
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageCardCollectionThemeRandomGroup : StorageBase
    {
        
        // 卡册主题ID
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
         
        // 等级随机组字典
        [JsonProperty]
        StorageDictionary<int,StorageCardCollectionLevelRandomGroup> levelRandomGroupDic = new StorageDictionary<int,StorageCardCollectionLevelRandomGroup>();
        [JsonIgnore]
        public StorageDictionary<int,StorageCardCollectionLevelRandomGroup> LevelRandomGroupDic
        {
            get
            {
                return levelRandomGroupDic;
            }
        }
        // ---------------------------------//
        
    }
}