/************************************************
 * OutsideGuide Config Manager class : OutsideGuideConfigManager
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

namespace DragonPlus.Config.OutsideGuide
{
    public partial class OutsideGuideConfigManager : Manager<OutsideGuideConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<GuideGroup> GuideGroupList;
        public List<GuideStep> GuideStepList;
        public List<ActionType> ActionTypeList;
        public List<EventType> EventTypeList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(GuideGroup)] = "GuideGroup",
            [typeof(GuideStep)] = "GuideStep",
            [typeof(ActionType)] = "ActionType",
            [typeof(EventType)] = "EventType",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("guidegroup")) return false;
            if (!table.ContainsKey("guidestep")) return false;
            if (!table.ContainsKey("actiontype")) return false;
            if (!table.ContainsKey("eventtype")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "GuideGroup": cfg = GuideGroupList as List<T>; break;
                case "GuideStep": cfg = GuideStepList as List<T>; break;
                case "ActionType": cfg = ActionTypeList as List<T>; break;
                case "EventType": cfg = EventTypeList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/TMatch/match_guide");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/TMatch/match_guide error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            GuideGroupList = JsonConvert.DeserializeObject<List<GuideGroup>>(JsonConvert.SerializeObject(table["guidegroup"]));
            GuideStepList = JsonConvert.DeserializeObject<List<GuideStep>>(JsonConvert.SerializeObject(table["guidestep"]));
            ActionTypeList = JsonConvert.DeserializeObject<List<ActionType>>(JsonConvert.SerializeObject(table["actiontype"]));
            EventTypeList = JsonConvert.DeserializeObject<List<EventType>>(JsonConvert.SerializeObject(table["eventtype"]));
            
        }
    }
}