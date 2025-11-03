using System.Collections.Generic;
using DragonU3DSDK.Storage;

namespace Merge.Order
{
    public class MainOrderCreatorBranch
    {
        public static SlotDefinition _orgSlot = SlotDefinition.BranchSlot;
        public static MainOrderType _orgType = MainOrderType.Branch;
        
        public static bool CanCreate(SlotDefinition slot)
        {
            if (slot != _orgSlot)
                return false;

            TableOrderRandom orderRandom = OrderConfigManager.Instance.GetRandomConfig((int)_orgType);
            if (orderRandom.unlockLevel > ExperenceModel.Instance.GetLevel())
                return false;

            return true;
        }

        public static StorageTaskItem CreateOrder(SlotDefinition slot)
        {
            var availableItems = MainOrderCreatorRandomCommon.GetAvailableItems(_orgSlot);
            availableItems = MainOrderCreatorRandomCommon.FilterByChainId(availableItems, new int[] { 20413 }); //宝石
            if (availableItems != null && availableItems.Count > 0)
            {
                return MainOrderCreatorRandomCommon.CreateOrder(_orgType, _orgSlot, availableItems);
            }

            return null;
        }
    }
}