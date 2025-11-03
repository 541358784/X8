using System.Collections.Generic;
using DragonU3DSDK.Storage;

namespace Merge.Order
{
    public class MainOrderCreatorRandom5 //20分钟cd
    {
        public static SlotDefinition _orgSlot = SlotDefinition.Slot8;
        public static MainOrderType _orgType = MainOrderType.Random5New;
        
        public static bool CanCreate(SlotDefinition slot)
        {
            string orderRefreshTimeKey = ((int)_orgSlot).ToString();
            return MainOrderCreatorRandomCommon.CanCreate(orderRefreshTimeKey, _orgType,_orgSlot, slot);
        }

        public static StorageTaskItem CreateOrder(SlotDefinition slot)
        {
            return MainOrderCreatorRandomCommon.CreateOrder(_orgType, slot);
        }
    }
}