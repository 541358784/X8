/************************************************
 * MixMaster Config Manager class : MixMasterConfigManager
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

namespace DragonPlus.Config.MixMaster
{
    public partial class MixMasterConfigManager : Manager<MixMasterConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<MixMasterGlobalConfig> MixMasterGlobalConfigList;
        public List<MixMasterMixTaskConfig> MixMasterMixTaskConfigList;
        public List<MixMasterGiftBagConfig> MixMasterGiftBagConfigList;
        public List<MixMasterFormulaConfig> MixMasterFormulaConfigList;
        public List<MixMasterMaterialConfig> MixMasterMaterialConfigList;
        public List<MixMasterOrderOutPutConfig> MixMasterOrderOutPutConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(MixMasterGlobalConfig)] = "MixMasterGlobalConfig",
            [typeof(MixMasterMixTaskConfig)] = "MixMasterMixTaskConfig",
            [typeof(MixMasterGiftBagConfig)] = "MixMasterGiftBagConfig",
            [typeof(MixMasterFormulaConfig)] = "MixMasterFormulaConfig",
            [typeof(MixMasterMaterialConfig)] = "MixMasterMaterialConfig",
            [typeof(MixMasterOrderOutPutConfig)] = "MixMasterOrderOutPutConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("mixmasterglobalconfig")) return false;
            if (!table.ContainsKey("mixmastermixtaskconfig")) return false;
            if (!table.ContainsKey("mixmastergiftbagconfig")) return false;
            if (!table.ContainsKey("mixmasterformulaconfig")) return false;
            if (!table.ContainsKey("mixmastermaterialconfig")) return false;
            if (!table.ContainsKey("mixmasterorderoutputconfig")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "MixMasterGlobalConfig": cfg = MixMasterGlobalConfigList as List<T>; break;
                case "MixMasterMixTaskConfig": cfg = MixMasterMixTaskConfigList as List<T>; break;
                case "MixMasterGiftBagConfig": cfg = MixMasterGiftBagConfigList as List<T>; break;
                case "MixMasterFormulaConfig": cfg = MixMasterFormulaConfigList as List<T>; break;
                case "MixMasterMaterialConfig": cfg = MixMasterMaterialConfigList as List<T>; break;
                case "MixMasterOrderOutPutConfig": cfg = MixMasterOrderOutPutConfigList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/Activity/MixMaster/mixmaster");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/Activity/MixMaster/mixmaster error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            MixMasterGlobalConfigList = JsonConvert.DeserializeObject<List<MixMasterGlobalConfig>>(JsonConvert.SerializeObject(table["mixmasterglobalconfig"]));
            MixMasterMixTaskConfigList = JsonConvert.DeserializeObject<List<MixMasterMixTaskConfig>>(JsonConvert.SerializeObject(table["mixmastermixtaskconfig"]));
            MixMasterGiftBagConfigList = JsonConvert.DeserializeObject<List<MixMasterGiftBagConfig>>(JsonConvert.SerializeObject(table["mixmastergiftbagconfig"]));
            MixMasterFormulaConfigList = JsonConvert.DeserializeObject<List<MixMasterFormulaConfig>>(JsonConvert.SerializeObject(table["mixmasterformulaconfig"]));
            MixMasterMaterialConfigList = JsonConvert.DeserializeObject<List<MixMasterMaterialConfig>>(JsonConvert.SerializeObject(table["mixmastermaterialconfig"]));
            MixMasterOrderOutPutConfigList = JsonConvert.DeserializeObject<List<MixMasterOrderOutPutConfig>>(JsonConvert.SerializeObject(table["mixmasterorderoutputconfig"]));
            
        }
    }
}