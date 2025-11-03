
using System.Collections.Generic;
using System.Linq;
using Activity.Matreshkas.Model;
using JetBrains.Annotations;

namespace DragonPlus.Config.Matreshkas
{
    public partial class MatreshkasConfigManager
    {
        public void Trim()
        {
        }
        
        public Dictionary<int, List<MatreshkasConfig>> GetConfigsByPayLevel()
        {
            var payLevelGroup = MatreshkasModel.Instance.Matreshkas.PayLevelGroup;
            var configs = new List<MatreshkasConfig>();
            var allConfigs = MatreshkasConfigList;
            foreach (var config in allConfigs)
            {
                if (config.PayLevelGroup == payLevelGroup)
                    configs.Add(config);
            }
            if (configs.Count == 0)
            {
                foreach (var config in allConfigs)
                {
                    if (config.PayLevelGroup == 0)
                        configs.Add(config);
                }
            }

            var configDic = new Dictionary<int, List<MatreshkasConfig>>();
            foreach (var config in configs)
            {
                if (!configDic.ContainsKey(config.Level))
                    configDic.Add(config.Level,new List<MatreshkasConfig>());
                configDic[config.Level].Add(config);
            }
            return configDic;
        }
        

        public List<MatreshkasConfig> GetMatreshkasConfigs(int level)
        {
            var configs = GetConfigsByPayLevel();
            if (!configs.ContainsKey(level))
                return null;

            return configs[level];
        }

        public int AdaptMatreshkasLevel(int level)
        {
            var configs = GetConfigsByPayLevel();
            foreach (var kv in configs)
            {
                if (level <= kv.Key)
                    return kv.Key;
            }

            return configs.Last().Key;
        }
    }
}