/************************************************
 * Storage class : StorageDecoDialog
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageDecoDialog : StorageBase
    {
        
        // 完成的对话ID
        [JsonProperty]
        StorageList<int> finishedDialog = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> FinishedDialog
        {
            get
            {
                return finishedDialog;
            }
        }
        // ---------------------------------//
        
        // 组ID，跳过的步骤
        [JsonProperty]
        StorageDictionary<int,int> skipInfoDic = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> SkipInfoDic
        {
            get
            {
                return skipInfoDic;
            }
        }
        // ---------------------------------//
        
    }
}