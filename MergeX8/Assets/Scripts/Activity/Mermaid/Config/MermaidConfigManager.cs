/************************************************
 * Mermaid Config Manager class : MermaidConfigManager
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

namespace DragonPlus.Config.Mermaid
{
    public partial class MermaidConfigManager : Manager<MermaidConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<MermaidConfig> MermaidConfigList;
        public List<ExchangeReward> ExchangeRewardList;
        public List<TaskReward> TaskRewardList;
        public List<StageReward> StageRewardList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(MermaidConfig)] = "MermaidConfig",
            [typeof(ExchangeReward)] = "ExchangeReward",
            [typeof(TaskReward)] = "TaskReward",
            [typeof(StageReward)] = "StageReward",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("mermaidconfig")) return false;
            if (!table.ContainsKey("exchangereward")) return false;
            if (!table.ContainsKey("taskreward")) return false;
            if (!table.ContainsKey("stagereward")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "MermaidConfig": cfg = MermaidConfigList as List<T>; break;
                case "ExchangeReward": cfg = ExchangeRewardList as List<T>; break;
                case "TaskReward": cfg = TaskRewardList as List<T>; break;
                case "StageReward": cfg = StageRewardList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/Mermaid/mermaid");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/Mermaid/mermaid error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            MermaidConfigList = JsonConvert.DeserializeObject<List<MermaidConfig>>(JsonConvert.SerializeObject(table["mermaidconfig"]));
            ExchangeRewardList = JsonConvert.DeserializeObject<List<ExchangeReward>>(JsonConvert.SerializeObject(table["exchangereward"]));
            TaskRewardList = JsonConvert.DeserializeObject<List<TaskReward>>(JsonConvert.SerializeObject(table["taskreward"]));
            StageRewardList = JsonConvert.DeserializeObject<List<StageReward>>(JsonConvert.SerializeObject(table["stagereward"]));
            
        }
    }
}