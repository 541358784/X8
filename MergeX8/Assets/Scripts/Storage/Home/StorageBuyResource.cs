/************************************************
 * Storage class : StorageBuyResource
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageBuyResource : StorageBase
    {
        
        // 购买ID 24小时刷新
        [JsonProperty]
        int buyResourcesId;
        [JsonIgnore]
        public int BuyResourcesId
        {
            get
            {
                return buyResourcesId;
            }
            set
            {
                if(buyResourcesId != value)
                {
                    buyResourcesId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 弹出界面时间
        [JsonProperty]
        StorageDictionary<int,long> popupResTime = new StorageDictionary<int,long>();
        [JsonIgnore]
        public StorageDictionary<int,long> PopupResTime
        {
            get
            {
                return popupResTime;
            }
        }
        // ---------------------------------//
        
        // 是否关闭过资源界面
        [JsonProperty]
        StorageDictionary<int,bool> isCloseResView = new StorageDictionary<int,bool>();
        [JsonIgnore]
        public StorageDictionary<int,bool> IsCloseResView
        {
            get
            {
                return isCloseResView;
            }
        }
        // ---------------------------------//
        
        // 当前购买第几个奖励
        [JsonProperty]
        StorageDictionary<int,int> buyResIndex = new StorageDictionary<int,int>();
        [JsonIgnore]
        public StorageDictionary<int,int> BuyResIndex
        {
            get
            {
                return buyResIndex;
            }
        }
        // ---------------------------------//
        
    }
}