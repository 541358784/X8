/************************************************
 * HappyGo Config Manager class : HappyGoConfigManager
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

namespace DragonPlus.Config.HappyGo
{
    public partial class HappyGoConfigManager : Manager<HappyGoConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<HGVDLevel> HGVDLevelList;
        public List<HGVDBoardGrid> HGVDBoardGridList;
        public List<HGVDTLPhoneReq> HGVDTLPhoneReqList;
        public List<HGVDFlashSale> HGVDFlashSaleList;
        public List<HGVDBundle> HGVDBundleList;
        public List<HGVDConfig> HGVDConfigList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(HGVDLevel)] = "HGVDLevel",
            [typeof(HGVDBoardGrid)] = "HGVDBoardGrid",
            [typeof(HGVDTLPhoneReq)] = "HGVDTLPhoneReq",
            [typeof(HGVDFlashSale)] = "HGVDFlashSale",
            [typeof(HGVDBundle)] = "HGVDBundle",
            [typeof(HGVDConfig)] = "HGVDConfig",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("hg_vd_level")) return false;
            if (!table.ContainsKey("hg_vd_boardgrid")) return false;
            if (!table.ContainsKey("hg_vd_tlphonereq")) return false;
            if (!table.ContainsKey("hg_vd_flashsale")) return false;
            if (!table.ContainsKey("hg_vd_bundle")) return false;
            if (!table.ContainsKey("hg_vd_config")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "HGVDLevel": cfg = HGVDLevelList as List<T>; break;
                case "HGVDBoardGrid": cfg = HGVDBoardGridList as List<T>; break;
                case "HGVDTLPhoneReq": cfg = HGVDTLPhoneReqList as List<T>; break;
                case "HGVDFlashSale": cfg = HGVDFlashSaleList as List<T>; break;
                case "HGVDBundle": cfg = HGVDBundleList as List<T>; break;
                case "HGVDConfig": cfg = HGVDConfigList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("/happygo");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load /happygo error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            HGVDLevelList = JsonConvert.DeserializeObject<List<HGVDLevel>>(JsonConvert.SerializeObject(table["hg_vd_level"]));
            HGVDBoardGridList = JsonConvert.DeserializeObject<List<HGVDBoardGrid>>(JsonConvert.SerializeObject(table["hg_vd_boardgrid"]));
            HGVDTLPhoneReqList = JsonConvert.DeserializeObject<List<HGVDTLPhoneReq>>(JsonConvert.SerializeObject(table["hg_vd_tlphonereq"]));
            HGVDFlashSaleList = JsonConvert.DeserializeObject<List<HGVDFlashSale>>(JsonConvert.SerializeObject(table["hg_vd_flashsale"]));
            HGVDBundleList = JsonConvert.DeserializeObject<List<HGVDBundle>>(JsonConvert.SerializeObject(table["hg_vd_bundle"]));
            HGVDConfigList = JsonConvert.DeserializeObject<List<HGVDConfig>>(JsonConvert.SerializeObject(table["hg_vd_config"]));
            
        }
    }
}