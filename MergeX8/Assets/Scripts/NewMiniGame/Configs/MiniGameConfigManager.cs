/************************************************
 * MiniGame Config Manager class : MiniGameConfigManager
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

namespace DragonPlus.Config.MiniGame
{
    public partial class MiniGameConfigManager : Manager<MiniGameConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<MiniGameChapter> MiniGameChapterList;
        public List<MiniGameLevel> MiniGameLevelList;
        public List<MiniGameSelection> MiniGameSelectionList;
        public List<AsmrLevelConfig> AsmrLevelConfigList;
        public List<AsmrGroupConfig> AsmrGroupConfigList;
        public List<AsmrStepConfig> AsmrStepConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(MiniGameChapter)] = "MiniGameChapter",
            [typeof(MiniGameLevel)] = "MiniGameLevel",
            [typeof(MiniGameSelection)] = "MiniGameSelection",
            [typeof(AsmrLevelConfig)] = "AsmrLevelConfig",
            [typeof(AsmrGroupConfig)] = "AsmrGroupConfig",
            [typeof(AsmrStepConfig)] = "AsmrStepConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("minigamechapter")) return false;
            if (!table.ContainsKey("minigamelevel")) return false;
            if (!table.ContainsKey("minigameselection")) return false;
            if (!table.ContainsKey("asmrlevelconfig")) return false;
            if (!table.ContainsKey("asmrgroupconfig")) return false;
            if (!table.ContainsKey("asmrstepconfig")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "MiniGameChapter": cfg = MiniGameChapterList as List<T>; break;
                case "MiniGameLevel": cfg = MiniGameLevelList as List<T>; break;
                case "MiniGameSelection": cfg = MiniGameSelectionList as List<T>; break;
                case "AsmrLevelConfig": cfg = AsmrLevelConfigList as List<T>; break;
                case "AsmrGroupConfig": cfg = AsmrGroupConfigList as List<T>; break;
                case "AsmrStepConfig": cfg = AsmrStepConfigList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("configs/newminigame/newminigameconfig");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load configs/newminigame/newminigameconfig error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            MiniGameChapterList = JsonConvert.DeserializeObject<List<MiniGameChapter>>(JsonConvert.SerializeObject(table["minigamechapter"]));
            MiniGameLevelList = JsonConvert.DeserializeObject<List<MiniGameLevel>>(JsonConvert.SerializeObject(table["minigamelevel"]));
            MiniGameSelectionList = JsonConvert.DeserializeObject<List<MiniGameSelection>>(JsonConvert.SerializeObject(table["minigameselection"]));
            AsmrLevelConfigList = JsonConvert.DeserializeObject<List<AsmrLevelConfig>>(JsonConvert.SerializeObject(table["asmrlevelconfig"]));
            AsmrGroupConfigList = JsonConvert.DeserializeObject<List<AsmrGroupConfig>>(JsonConvert.SerializeObject(table["asmrgroupconfig"]));
            AsmrStepConfigList = JsonConvert.DeserializeObject<List<AsmrStepConfig>>(JsonConvert.SerializeObject(table["asmrstepconfig"]));
            
        }
    }
}