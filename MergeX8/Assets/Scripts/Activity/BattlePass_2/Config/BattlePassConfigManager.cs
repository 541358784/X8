using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Config;
using Gameplay;
using Newtonsoft.Json;
using SomeWhere;
using UnityEngine;

namespace Activity.BattlePass_2
{
    public class BattlePassConfigManager : Manager<BattlePassConfigManager>
    {
        public List<TableBattlePassConfig> _battlePassConfigs = null;
        public List<TableBattlePassPackage> _battlePassPackages = null;
        private List<TableBattlePassReward> _battlePassRewards = null;
        public List<TableBattlePassTask> _battlePassTasks = null;
        public List<TableBattlePassShopConfig> _battlePassShopConfigs = null;
        public List<TableBattlePassLoopRewardConfig> _battlePassLoopRewardConfigs = null;

        private Dictionary<int, List<TableBattlePassTask>> _battlePassTaskMap =
            new Dictionary<int, List<TableBattlePassTask>>();

        private Dictionary<int, TableBattlePassShopConfig> _shopConfigMap = new Dictionary<int, TableBattlePassShopConfig>();
        private Dictionary<int, List<TableBattlePassReward>> _rewardConfigMap = new Dictionary<int, List<TableBattlePassReward>>();
        
        private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type, string>
        {
            { typeof(TableBattlePassConfig), "BattlePassConfig" },
            { typeof(TableBattlePassPackage), "BattlePassReward" },
            { typeof(TableBattlePassReward), "BattlePassTask" },
            { typeof(TableBattlePassTask), "BattlePassPackage" },
            { typeof(TableBattlePassLoopRewardConfig), "BattlePassLoopRewardConfig" },
            { typeof(TableBattlePassShopConfig), "BattlePassShopConfig" },
        };

        public Dictionary<int, Dictionary<int, List<TableBattlePassTask>>> _dicBattlePassTask = new Dictionary<int, Dictionary<int, List<TableBattlePassTask>>>();

        public void InitFromServerData(string configJson)
        {
            _battlePassTaskMap.Clear();
            _dicBattlePassTask.Clear();
            _shopConfigMap.Clear();
            _rewardConfigMap.Clear();
            
            Hashtable table = null;
            try
            {
                if (string.IsNullOrEmpty(configJson) == false)
                {
                    table = JsonConvert.DeserializeObject<Hashtable>(configJson);
                }
            }
            catch (Exception ex)
            {
                DebugUtil.LogError(ex.ToString());
            }


            foreach (var subModule in typeToEnum)
            {
                try
                {
                    switch (subModule.Value)
                    {
                        case "BattlePassConfig":
                            _battlePassConfigs = JsonConvert.DeserializeObject<List<TableBattlePassConfig>>(JsonConvert.SerializeObject(table["battlepassconfig"]));
                            break;
                        case "BattlePassReward":
                            _battlePassRewards = JsonConvert.DeserializeObject<List<TableBattlePassReward>>(JsonConvert.SerializeObject(table["battlepassreward"]));
                            break;
                        case "BattlePassTask":
                            _battlePassTasks = JsonConvert.DeserializeObject<List<TableBattlePassTask>>(JsonConvert.SerializeObject(table["battlepasstask"]));
                            break;
                        case "BattlePassPackage":
                            _battlePassPackages = JsonConvert.DeserializeObject<List<TableBattlePassPackage>>(JsonConvert.SerializeObject(table["battlepasspackage"]));
                            break;
                        case "BattlePassLoopRewardConfig":
                            _battlePassLoopRewardConfigs = JsonConvert.DeserializeObject<List<TableBattlePassLoopRewardConfig>>(JsonConvert.SerializeObject(table["battlepasslooprewardconfig"]));
                            break;
                        case "BattlePassShopConfig":
                            _battlePassShopConfigs = JsonConvert.DeserializeObject<List<TableBattlePassShopConfig>>(JsonConvert.SerializeObject(table["battlepassshopconfig"]));
                            break;
                        default: throw new ArgumentOutOfRangeException(nameof(subModule), subModule, null);
                    }
                }
                catch (Exception ex)
                {
                    DebugUtil.LogError(ex.ToString());
                }
            }

            foreach (var config in _battlePassShopConfigs)
            {
                _shopConfigMap[config.payLevelGroup] = config;
            }
            
            foreach (var config in _battlePassRewards)
            {
                if (!_rewardConfigMap.ContainsKey(config.payLevelGroup))
                    _rewardConfigMap[config.payLevelGroup] = new List<TableBattlePassReward>();
                
                _rewardConfigMap[config.payLevelGroup].Add(config);
            }
            
            
            _battlePassTasks.ForEach(a =>
            {
                if (!_battlePassTaskMap.ContainsKey(a.difficulty))
                    _battlePassTaskMap[a.difficulty] = new List<TableBattlePassTask>();

                _battlePassTaskMap[a.difficulty].Add(a);

                if (a.difficulty <= 6)
                {
                    if (!_dicBattlePassTask.ContainsKey(a.type))
                        _dicBattlePassTask.Add(a.type, new Dictionary<int, List<TableBattlePassTask>>());

                    var tasks = _dicBattlePassTask[a.type];
                    if (!tasks.ContainsKey(a.difficulty))
                        tasks[a.difficulty] = new List<TableBattlePassTask>();

                    tasks[a.difficulty].Add(a);
                }
            });
        }


        public List<TableBattlePassTask> GetTasksByDifficulty(int difficulty)
        {
            if (!_battlePassTaskMap.ContainsKey(difficulty))
                return null;

            return _battlePassTaskMap[difficulty];
        }

        public TableBattlePassTask GetTaskConfig(int id)
        {
            return _battlePassTasks.Find(a => a.id == id);
        }

        public TableBattlePassShopConfig GetShopConfig()
        {
            int group = BattlePassModel.Instance.PayLevelGroup();

            if (_shopConfigMap.ContainsKey(group))
                return _shopConfigMap[group];

            return _shopConfigMap[_shopConfigMap.Keys.First()];
        }

        public List<TableBattlePassReward> GetRewardConfig()
        {
            int group = BattlePassModel.Instance.PayLevelGroup();

            if (_rewardConfigMap.ContainsKey(group))
                return _rewardConfigMap[group];

            return _rewardConfigMap[_rewardConfigMap.Keys.First()];
        }
    }
}