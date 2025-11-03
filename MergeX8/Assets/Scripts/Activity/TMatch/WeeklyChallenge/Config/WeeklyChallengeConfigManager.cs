/************************************************
 * WeeklyChallenge Config Manager class : WeeklyChallengeConfigManager
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

namespace DragonPlus.Config.WeeklyChallenge
{
    public partial class WeeklyChallengeConfigManager : Manager<WeeklyChallengeConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<Base> BaseList;
        public List<EventWeeklyChallenge> EventWeeklyChallengeList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(Base)] = "Base",
            [typeof(EventWeeklyChallenge)] = "EventWeeklyChallenge",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("base")) return false;
            if (!table.ContainsKey("eventweeklychallenge")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "Base": cfg = BaseList as List<T>; break;
                case "EventWeeklyChallenge": cfg = EventWeeklyChallengeList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("C:/Program Files/Git/match_weeklychallenge");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load C:/Program Files/Git/match_weeklychallenge error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            BaseList = JsonConvert.DeserializeObject<List<Base>>(JsonConvert.SerializeObject(table["base"]));
            EventWeeklyChallengeList = JsonConvert.DeserializeObject<List<EventWeeklyChallenge>>(JsonConvert.SerializeObject(table["eventweeklychallenge"]));
            
        }
    }
}