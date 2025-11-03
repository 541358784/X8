/************************************************
 * TMBP Config Manager class : TMBPConfigManager
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

namespace DragonPlus.Config.TMBP
{
    public partial class TMBPConfigManager : Manager<TMBPConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<Base> BaseList;
        public List<Loop> LoopList;
        public List<Rewards> RewardsList;
        public List<Const> ConstList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(Base)] = "Base",
            [typeof(Loop)] = "Loop",
            [typeof(Rewards)] = "Rewards",
            [typeof(Const)] = "Const",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("base")) return false;
            if (!table.ContainsKey("loop")) return false;
            if (!table.ContainsKey("rewards")) return false;
            if (!table.ContainsKey("const")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "Base": cfg = BaseList as List<T>; break;
                case "Loop": cfg = LoopList as List<T>; break;
                case "Rewards": cfg = RewardsList as List<T>; break;
                case "Const": cfg = ConstList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("C:/Program Files/Git/match_tmbp");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load C:/Program Files/Git/match_tmbp error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            BaseList = JsonConvert.DeserializeObject<List<Base>>(JsonConvert.SerializeObject(table["base"]));
            LoopList = JsonConvert.DeserializeObject<List<Loop>>(JsonConvert.SerializeObject(table["loop"]));
            RewardsList = JsonConvert.DeserializeObject<List<Rewards>>(JsonConvert.SerializeObject(table["rewards"]));
            ConstList = JsonConvert.DeserializeObject<List<Const>>(JsonConvert.SerializeObject(table["const"]));
            
        }
    }
}