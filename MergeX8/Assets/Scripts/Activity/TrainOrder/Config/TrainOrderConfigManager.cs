using System;
using System.Collections;
using System.Collections.Generic;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using Newtonsoft.Json;
using UnityEngine;

namespace Activity.TrainOrder
{
    public partial class TrainOrderConfigManager : TableSingleton<TrainOrderConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<TrainOrderLevel> TrainOrderLevelList;
        public List<TrainOrderOrderGroup> TrainOrderOrderGroupList;
        public List<TrainOrderOrder> TrainOrderOrderList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(TrainOrderLevel)] = "TrainOrderLevel",
            [typeof(TrainOrderOrderGroup)] = "TrainOrderOrderGroup",
            [typeof(TrainOrderOrder)] = "TrainOrderOrder",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("trainorderlevel")) return false;
            if (!table.ContainsKey("trainorderordergroup")) return false;
            if (!table.ContainsKey("trainorderorder")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "TrainOrderLevel": cfg = TrainOrderLevelList as List<T>; break;
                case "TrainOrderOrderGroup": cfg = TrainOrderOrderGroupList as List<T>; break;
                case "TrainOrderOrder": cfg = TrainOrderOrderList as List<T>; break;
                default: throw new ArgumentOutOfRangeException(nameof(subModule), subModule, null);
            }
            return cfg;
        }
        public void InitConfig(String configJson = null)
        {
            ConfigFromRemote = true;
            Hashtable table = null;
            if (!string.IsNullOrEmpty(configJson))
                table = JsonConvert.DeserializeObject<Hashtable>(configJson);

            if (table == null || !CheckTable(table))
            {
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/TrainOrder/trainorder");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/TrainOrder/trainorder error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            TrainOrderLevelList = JsonConvert.DeserializeObject<List<TrainOrderLevel>>(JsonConvert.SerializeObject(table["trainorderlevel"]));
            TrainOrderOrderGroupList = JsonConvert.DeserializeObject<List<TrainOrderOrderGroup>>(JsonConvert.SerializeObject(table["trainorderordergroup"]));
            TrainOrderOrderList = JsonConvert.DeserializeObject<List<TrainOrderOrder>>(JsonConvert.SerializeObject(table["trainorderorder"]));
            

            Trim();
        }
    }
}