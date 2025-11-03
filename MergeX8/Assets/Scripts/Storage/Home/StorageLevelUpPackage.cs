/************************************************
 * Storage class : StorageLevelUpPackage
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageLevelUpPackage : StorageBase
    {
        
        // 存在的礼包列表
        [JsonProperty]
        StorageList<StorageLevelUpPackageSinglePackage> packageList = new StorageList<StorageLevelUpPackageSinglePackage>();
        [JsonIgnore]
        public StorageList<StorageLevelUpPackageSinglePackage> PackageList
        {
            get
            {
                return packageList;
            }
        }
        // ---------------------------------//
        
        // 当前等级
        [JsonProperty]
        int cueLevel;
        [JsonIgnore]
        public int CueLevel
        {
            get
            {
                return cueLevel;
            }
            set
            {
                if(cueLevel != value)
                {
                    cueLevel = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}