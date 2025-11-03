using System.Collections.Generic;
using System.Linq;
using Activity.TimeOrder;
using DragonPlus.Config.Team;
using DragonPlus.Config.TimeOrder;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using Scripts.UI;
using SomeWhere;

namespace Merge.Order
{
    public class MainOrderCreateTeam
    {
        public static SlotDefinition _orgSlot = SlotDefinition.Team;
        public static MainOrderType _orgType = MainOrderType.Team;

        public static bool CanCreate()
        {
            if (!TeamManager.Instance.TeamIsUnlock())
                return false;
            
            if (!TeamManager.Instance.HasOrder())
                return false;

            if (MainOrderManager.Instance.StorageTaskGroup.CurTasks.Find(a => a.Type == (int)_orgType) != null)
                return false;

            if (DragonU3DSDK.Utils.IsSameDay((long)APIManager.Instance.GetServerTime() / 1000, TeamManager.Instance.Storage.RefreshOrderTime/1000))
                return false;

            return true;
        }
        
        public static StorageTaskItem TryCreateOrder()
        {
            int payLevel = PayLevelModel.Instance.GetCurPayLevelConfig().TeamOrder;
            var teamOrder = GetOrderConfig(TeamConfigManager.Instance.GetTeamOrdersByPayLevel(payLevel));
            
            var availableItems = MainOrderCreatorRandomCommon.GetAvailableItems(_orgSlot);
            
            List<int> requirements = new List<int>();

            for (int i = 0; i < 3; i++)
            {
                int min = 0;
                int max = 0;

                switch (i)
                {
                    case 0:
                    {
                        min = teamOrder.FirstMin;
                        max = teamOrder.FirstMax;
                        break;
                    }
                    case 1:
                    {
                        min = teamOrder.SecondMin;
                        max = teamOrder.SecondMax;
                        break;
                    }
                    case 2:
                    {
                        min = teamOrder.ThirdMin;
                        max = teamOrder.ThirdMax;
                        break;
                    }
                }
                
                if(min == 0 && max == 0)
                    continue;
                
                var items = MainOrderCreatorRandomCommon.FilterByDifficulty(availableItems, min, max , null, _orgSlot);
                if(items == null)
                    continue;
                
                var itemId = MainOrderCreatorRandomCommon.RandomAvailableItem(items);
                if(itemId < 0)
                    continue;
                
                requirements.Add(itemId);
            }

            if (requirements.Count == 0)
                return null;
            
            TeamManager.Instance.Storage.RefreshOrderTime = (long)APIManager.Instance.GetServerTime();
            var numbers = teamOrder.RewardNums.DeepCopy();
            var multi = 1f;
            var teamLevelConfig = TeamManager.Instance.MyTeamLevelConfig;
            if (teamLevelConfig != null)
                multi += teamLevelConfig.TaskAddition;
            for (var i = 0; i < numbers.Count; i++)
            {
                numbers[i] = (int)(numbers[i] * multi);
            }

            var order = MainOrderManager.Instance.AddTask(-1, requirements.ToArray(), null, _orgType, _orgSlot, 0,  rewardIds:teamOrder.RewardIds.ToArray(), rewardNums:numbers.ToArray());
            return order;
        }

        public static bool Refresh()
        {
            if (TeamManager.Instance.Storage.RefreshOrderTime <= 0)
                return false;
            
            if (TeamManager.Instance.HasOrder())
                return false;
            
            var order = MainOrderManager.Instance.StorageTaskGroup.CurTasks.Find(a => a.Slot == (int)_orgSlot);
            if (order == null)
                return false;
            
            MainOrderManager.Instance.RemoveOrder(_orgType);
            TeamManager.Instance.Storage.RefreshOrderTime = -1;

            return true;
        }
        

        private static TableTeamOrder GetOrderConfig(List<TableTeamOrder> configs)
        {
            int level = ExperenceModel.Instance.GetLevel();
            foreach (var config in configs)
            {
                if(config.PlayerMin > 0 && level < config.PlayerMin)
                    continue;
                
                if(config.PlayerMax > 0 && level > config.PlayerMax)
                    continue;

                return config;
            }

            return configs.First();
        }
    }
}