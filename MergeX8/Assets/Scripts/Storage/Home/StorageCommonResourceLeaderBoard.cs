/************************************************
 * Storage class : StorageCommonResourceLeaderBoard
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageCommonResourceLeaderBoard : StorageBase
    {
        
        // 排行榜关键字
        [JsonProperty]
        string keyWord = "";
        [JsonIgnore]
        public string KeyWord
        {
            get
            {
                return keyWord;
            }
            set
            {
                if(keyWord != value)
                {
                    keyWord = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
         
        // 排行榜(KEY为ACTIVITYID)
        [JsonProperty]
        StorageDictionary<string,StorageCommonLeaderBoard> leaderBoardDictionary = new StorageDictionary<string,StorageCommonLeaderBoard>();
        [JsonIgnore]
        public StorageDictionary<string,StorageCommonLeaderBoard> LeaderBoardDictionary
        {
            get
            {
                return leaderBoardDictionary;
            }
        }
        // ---------------------------------//
        
    }
}