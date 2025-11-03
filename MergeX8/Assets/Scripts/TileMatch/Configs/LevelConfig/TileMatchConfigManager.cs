/************************************************
 * TileMatch Config Manager class : TileMatchConfigManager
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

namespace DragonPlus.Config.TileMatch
{
    public partial class TileMatchConfigManager : Manager<TileMatchConfigManager>
    {   
        public bool ConfigFromRemote;
        public List<TileLevel> TileLevelList;
        public List<BlockName> BlockNameList;
        public List<BlockType> BlockTypeList;
        public List<BlockUnlock> BlockUnlockList;
        public List<TileGlobal> TileGlobalList;
        public List<PropUnlock> PropUnlockList;
        
         private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(TileLevel)] = "TileLevel",
            [typeof(BlockName)] = "BlockName",
            [typeof(BlockType)] = "BlockType",
            [typeof(BlockUnlock)] = "BlockUnlock",
            [typeof(TileGlobal)] = "TileGlobal",
            [typeof(PropUnlock)] = "PropUnlock",
        };
        private bool CheckTable(Hashtable table)
        {
            if (!table.ContainsKey("tilelevel")) return false;
            if (!table.ContainsKey("blockname")) return false;
            if (!table.ContainsKey("blocktype")) return false;
            if (!table.ContainsKey("blockunlock")) return false;
            if (!table.ContainsKey("tileglobal")) return false;
            if (!table.ContainsKey("propunlock")) return false;
            
            return true;
        }

        public  List<T> GetConfig<T>()
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "TileLevel": cfg = TileLevelList as List<T>; break;
                case "BlockName": cfg = BlockNameList as List<T>; break;
                case "BlockType": cfg = BlockTypeList as List<T>; break;
                case "BlockUnlock": cfg = BlockUnlockList as List<T>; break;
                case "TileGlobal": cfg = TileGlobalList as List<T>; break;
                case "PropUnlock": cfg = PropUnlockList as List<T>; break;
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
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/LevelConfig/tilematch");
                if (string.IsNullOrEmpty(ta.text))
                {
                    DebugUtil.LogError("Load Configs/LevelConfig/tilematch error!");
                    return;
                }
                table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                ConfigFromRemote = false;
            }
            TileLevelList = JsonConvert.DeserializeObject<List<TileLevel>>(JsonConvert.SerializeObject(table["tilelevel"]));
            BlockNameList = JsonConvert.DeserializeObject<List<BlockName>>(JsonConvert.SerializeObject(table["blockname"]));
            BlockTypeList = JsonConvert.DeserializeObject<List<BlockType>>(JsonConvert.SerializeObject(table["blocktype"]));
            BlockUnlockList = JsonConvert.DeserializeObject<List<BlockUnlock>>(JsonConvert.SerializeObject(table["blockunlock"]));
            TileGlobalList = JsonConvert.DeserializeObject<List<TileGlobal>>(JsonConvert.SerializeObject(table["tileglobal"]));
            PropUnlockList = JsonConvert.DeserializeObject<List<PropUnlock>>(JsonConvert.SerializeObject(table["propunlock"]));
            
        }
    }
}