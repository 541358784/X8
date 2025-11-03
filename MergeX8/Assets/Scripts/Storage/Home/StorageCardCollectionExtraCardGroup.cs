/************************************************
 * Storage class : StorageCardCollectionExtraCardGroup
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageCardCollectionExtraCardGroup : StorageBase
    {
        
        // 多余的卡牌
        [JsonProperty]
        StorageList<int> collectedCards = new StorageList<int>();
        [JsonIgnore]
        public StorageList<int> CollectedCards
        {
            get
            {
                return collectedCards;
            }
        }
        // ---------------------------------//
        
        // 卡牌等级
        [JsonProperty]
        int cardLevel;
        [JsonIgnore]
        public int CardLevel
        {
            get
            {
                return cardLevel;
            }
            set
            {
                if(cardLevel != value)
                {
                    cardLevel = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}