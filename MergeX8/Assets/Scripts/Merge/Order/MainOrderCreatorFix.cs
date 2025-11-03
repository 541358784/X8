using System.Collections.Generic;
using Decoration;
using DragonU3DSDK.Storage;
using Gameplay;

namespace Merge.Order
{
    public class MainOrderCreatorFix
    {
        public static StorageTaskItem CreateOrder(SlotDefinition slot)
        {
            int index = MainOrderManager.Instance.StorageTaskGroup.OrderFixIndex;
            if(index >= OrderConfigManager.Instance.OrderFixConfigs.Count)
                return null;

            TableOrderFix config = OrderConfigManager.Instance.OrderFixConfigs[index];
            if (ExperenceModel.Instance.GetLevel() < config.unlockLevel)
                return null;

            if (!config.slots.Contains((int)slot))
                return null;
            
            MainOrderManager.Instance.StorageTaskGroup.OrderFixIndex++;
 
            List<int> seatIndex = new List<int>();
            for(int i = 0; i< config.requirements.Length; i++)
                seatIndex.Add(i+1);
            
            return MainOrderManager.Instance.AddTask(config.id, config.requirements, seatIndex,MainOrderType.Fixed, slot, config.headId);
        }
    }
}