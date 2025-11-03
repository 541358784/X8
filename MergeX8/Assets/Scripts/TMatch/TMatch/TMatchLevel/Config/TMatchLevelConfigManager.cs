// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * TMatchLevel ConfigHub Manager class : TMatchLevelConfigManager
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using DragonU3DSDK.Asset;

namespace DragonPlus.ConfigHub.TMatchLevel
{
    public class TMatchLevelConfigManager : ConfigManagerBase
    {
        private static TMatchLevelConfigManager _instance;
        public static TMatchLevelConfigManager Instance => _instance ?? (_instance = new TMatchLevelConfigManager());
        public override string Guid => "config_tmatchlevel";
        public override int VersionMinIOS => 45;
        public override int VersionMinAndroid => 45;
        protected override List<string> SubModules => new List<string> { 
            "Mapping",
        };
        private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(Mapping)] = "Mapping",
        };
        private List<Mapping> MappingList;
        
        public override List<T> GetConfig<T>(CacheOperate cacheOp = CacheOperate.None, long cacheDuration = -1)
        {
            if (!IsLoaded)
                ConfigHubManager.Instance.LoadConfig(Guid);

            processCache(cacheOp, cacheDuration);
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "Mapping": cfg = MappingList as List<T>; break;
                default: throw new ArgumentOutOfRangeException(nameof(subModule), subModule, null);
            }
            return cfg;
        }

        protected override bool CheckTable(Hashtable table)
        {   
            if (!table.ContainsKey("mapping")) return false;
            return true;
        }

        private bool TryParseJsonData(string configJson)
        {
            try
            {
                if (string.IsNullOrEmpty(configJson))
                    return false;
                var table = JsonConvert.DeserializeObject<Hashtable>(configJson);
                if (table == null || !CheckTable(table))
                    return false;
                foreach (var subModule in SubModules)
                {
                    switch (subModule)
                    { 
                        case "Mapping": MappingList = JsonConvert.DeserializeObject<List<Mapping>>(JsonConvert.SerializeObject(table["mapping"])); break;
                        default: throw new ArgumentOutOfRangeException(nameof(subModule), subModule, null);
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                ConfigHubUtil.E("Error when parse json:{e}");
                return false;
            }
        }

        public override void InitConfig(MetaData metaData, string jsonData = null)
        {
            IsRemote = true;
            if (metaData == null || !TryParseJsonData(jsonData))
            {
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/UserGroup/tmatchlevel");
                if (!TryParseJsonData(ta.text))
                {
                    ConfigHubUtil.E("Load Configs/UserGroup/tmatchlevel error!");
                    return;
                }
                IsRemote = false;
                metaData = GetMetaDataCached() ?? GetMetaDataDefault();
            }
            
            MetaData = metaData;
            
            PropertyInfo pInfo;
            foreach (var subModule in SubModules)
            {
                if (IsRemote)
                    continue;

                switch (subModule)
                { 
                    case "Mapping": 
                        pInfo = typeof(Mapping).GetProperty("UserGroup");
                        if (pInfo != null && pInfo.PropertyType == typeof(int))
                            MappingList = MappingList.FindAll(cfg => (int)pInfo.GetValue(cfg) == metaData.GroupId);
                        break;
                    default: throw new ArgumentOutOfRangeException(nameof(subModule), subModule, null);
                }
            }
            
            IsLoaded = true;
            ConfigHubUtil.L($"InitConfig:{getModuleString()}");
        }

        private List<Rules> RulesList;
        protected override bool HasGroup(int groupId)
        {
            if (RulesList == null || RulesList.Count == 0)
            {
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>("Configs/UserGroup/tmatchlevel");
                if (string.IsNullOrEmpty(ta.text))
                {
                    ConfigHubUtil.E("Load Configs/UserGroup/tmatchlevel error!");
                    return false;
                }
                var table = JsonConvert.DeserializeObject<Hashtable>(ta.text);
                RulesList = JsonConvert.DeserializeObject<List<Rules>>(JsonConvert.SerializeObject(table["rules"]));
            }
            return RulesList.Exists(r => r.GroupId == groupId);
        }
    }
}