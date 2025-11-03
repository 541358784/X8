using System.Collections.Generic;
using DragonU3DSDK.Storage;

namespace Merge.Order
{
    public class MainOrderCreatorRandom6 
    {
        public static SlotDefinition _orgSlot = SlotDefinition.Slot10;
        public static MainOrderType _orgType = MainOrderType.Random6;
        
        public static bool CanCreate(SlotDefinition slot)
        {
            if (MainOrderCreatorRandomCommon.CreateType == CreateOrderType.Difficulty)
                return false;
            
            string orderRefreshTimeKey = ((int)_orgSlot).ToString();
            return MainOrderCreatorRandomCommon.CanCreate(orderRefreshTimeKey, _orgType,_orgSlot, slot);
        }

        public static StorageTaskItem CreateOrder(SlotDefinition slot)
        {
            if (MainOrderCreatorRandomCommon.CreateType == CreateOrderType.Difficulty)
                return null;
            
            return MainOrderCreatorRandomCommon.CreateOrder(_orgType, slot);
        }
    }
}