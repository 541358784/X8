using System.Collections.Generic;
using System.Linq;
using Activity.CrazeOrder.Model;
using DragonPlus.Config.CrazeOrder;
using DragonPlus.Config.KeepPet;
using DragonU3DSDK.Storage;
using SomeWhere;

namespace Merge.Order
{
    public class MainOrderCreateKeepPet
    {
        public static SlotDefinition _orgSlot = SlotDefinition.KeepPet;
        public static MainOrderType _orgType = MainOrderType.KeepPet;


        public static StorageTaskItem TryCreateOrder(SlotDefinition slot = SlotDefinition.KeepPet)
        {
            if (slot != _orgSlot)
                return null;
            
            if(MainOrderManager.Instance.StorageTaskGroup.CurTasks.Find(a => a.Slot == (int)slot) != null)
                return null;
            
            int level = ExperenceModel.Instance.GetLevel();
            var keepPetOrderConfigList = KeepPetModel.Instance.GetKeepPetOrderConfig();
            KeepPetOrderConfig config = keepPetOrderConfigList[keepPetOrderConfigList.Count-1];
            for (var i = 0; i < keepPetOrderConfigList.Count; i++)
            {
                if (level <= keepPetOrderConfigList[i].Level)
                {
                    config = keepPetOrderConfigList[i];
                    break;
                }
            }
            
            var availableItems = MainOrderCreatorRandomCommon.GetAvailableItems(_orgSlot);
            int[] requirements = new int[config.MinDifficultys.Count];
            for (int i = 0; i < config.MinDifficultys.Count; i++)
            {
                var filterItems = MainOrderCreatorRandomCommon.FilterByDifficulty(availableItems, config.MinDifficultys[i], config.MaxDifficultys[i], null, _orgSlot, config.FirstFilterDiffMergeLine.ToArray());
                if (filterItems == null || filterItems.Count == 0)
                    continue;
                
                requirements[i] = filterItems.RandomPickOne()._orderItemId;
            }
            
            var rewardIds = KeepPetModel.Instance.GlobalConfig.OrderRewardId.ToArray();
            var rewardNums = KeepPetModel.Instance.GlobalConfig.OrderRewardNum.ToArray();
   
            var order = MainOrderManager.Instance.AddTask(MainOrderManager.Instance.StorageTaskGroup.OnlyId++, requirements, 
                null, _orgType, _orgSlot, OrderConfigManager.Instance.GetHeadSpines(OrderConfigManager.SpineType.KeepPet).Random().id, rewardIds:rewardIds, rewardNums:rewardNums);

            if(MergeTaskTipsController.Instance == null || !MergeTaskTipsController.Instance.gameObject.activeInHierarchy)
                return order;
        
            MergeTaskTipsController.Instance.RefreshTask(new List<StorageTaskItem>(){order}, () =>
            {
                EventDispatcher.Instance.DispatchEvent(EventEnum.TASK_REFRESH,MergeBoardEnum.Main);
                MergeMainController.Instance?.UpdateTaskRedPoint();
            });

            return order;
        }
    }
}