/************************************************
 * Storage class : StorageBuyDiamondTicket
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageBuyDiamondTicket : StorageBase
    {
        
        // 结束时间
        [JsonProperty]
        long endTime;
        [JsonIgnore]
        public long EndTime
        {
            get
            {
                return endTime;
            }
            set
            {
                if(endTime != value)
                {
                    endTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 券ID
        [JsonProperty]
        int ticketId;
        [JsonIgnore]
        public int TicketId
        {
            get
            {
                return ticketId;
            }
            set
            {
                if(ticketId != value)
                {
                    ticketId = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
    }
}