/************************************************
 * SlotMachine Config Manager class : SlotMachineConfigManager
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using Newtonsoft.Json;
using UnityEngine;

namespace DragonPlus.Config.SlotMachine
{
    public partial class SlotMachineConfigManager : Manager<SlotMachineConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<SlotMachineGlobalConfig> SlotMachineGlobalConfigList;
        public List<SlotMachineResultConfig> SlotMachineResultConfigList;
        public List<SlotMachineReSpinConfig> SlotMachineReSpinConfigList;
        public List<SlotMachineRewardConfig> SlotMachineRewardConfigList;
        public List<SlotMachineTaskRewardConfig> SlotMachineTaskRewardConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(SlotMachineGlobalConfig)] = "SlotMachineGlobalConfig",
            [typeof(SlotMachineResultConfig)] = "SlotMachineResultConfig",
            [typeof(SlotMachineReSpinConfig)] = "SlotMachineReSpinConfig",
            [typeof(SlotMachineRewardConfig)] = "SlotMachineRewardConfig",
            [typeof(SlotMachineTaskRewardConfig)] = "SlotMachineTaskRewardConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("slotmachineglobalconfig")) return false;
            if (!table.ContainsKey("slotmachineresultconfig")) return false;
            if (!table.ContainsKey("slotmachinerespinconfig")) return false;
            if (!table.ContainsKey("slotmachinerewardconfig")) return false;
            if (!table.ContainsKey("slotmachinetaskrewardconfig")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "SlotMachineGlobalConfig": cfg = SlotMachineGlobalConfigList as List<T>; break;
                case "SlotMachineResultConfig": cfg = SlotMachineResultConfigList as List<T>; break;
                case "SlotMachineReSpinConfig": cfg = SlotMachineReSpinConfigList as List<T>; break;
                case "SlotMachineRewardConfig": cfg = SlotMachineRewardConfigList as List<T>; break;
                case "SlotMachineTaskRewardConfig": cfg = SlotMachineTaskRewardConfigList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/SlotMachine/slotmachine");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/SlotMachine/slotmachine error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            SlotMachineGlobalConfigList = JsonConvert.DeserializeObject<List<SlotMachineGlobalConfig>>(JsonConvert.SerializeObject(table["slotmachineglobalconfig"]));
            SlotMachineResultConfigList = JsonConvert.DeserializeObject<List<SlotMachineResultConfig>>(JsonConvert.SerializeObject(table["slotmachineresultconfig"]));
            SlotMachineReSpinConfigList = JsonConvert.DeserializeObject<List<SlotMachineReSpinConfig>>(JsonConvert.SerializeObject(table["slotmachinerespinconfig"]));
            SlotMachineRewardConfigList = JsonConvert.DeserializeObject<List<SlotMachineRewardConfig>>(JsonConvert.SerializeObject(table["slotmachinerewardconfig"]));
            SlotMachineTaskRewardConfigList = JsonConvert.DeserializeObject<List<SlotMachineTaskRewardConfig>>(JsonConvert.SerializeObject(table["slotmachinetaskrewardconfig"]));
            
        }
    }
}