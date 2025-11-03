/************************************************
 * WinStreak Config Manager class : WinStreakConfigManager
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

namespace DragonPlus.Config.WinStreak
{
    public partial class WinStreakConfigManager : Manager<WinStreakConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<Base> BaseList;
        public List<OutRate> OutRateList;
        public List<Robot> RobotList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(Base)] = "Base",
            [typeof(OutRate)] = "OutRate",
            [typeof(Robot)] = "Robot",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("base")) return false;
            if (!table.ContainsKey("outrate")) return false;
            if (!table.ContainsKey("robot")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "Base": cfg = BaseList as List<T>; break;
                case "OutRate": cfg = OutRateList as List<T>; break;
                case "Robot": cfg = RobotList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("/tmatch_winstreak");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load /tmatch_winstreak error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            BaseList = JsonConvert.DeserializeObject<List<Base>>(JsonConvert.SerializeObject(table["base"]));
            OutRateList = JsonConvert.DeserializeObject<List<OutRate>>(JsonConvert.SerializeObject(table["outrate"]));
            RobotList = JsonConvert.DeserializeObject<List<Robot>>(JsonConvert.SerializeObject(table["robot"]));
            
        }
    }
}