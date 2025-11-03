using System.Collections.Generic;
using DragonU3DSDK.Storage;

namespace Merge.Order
{
    public class MainOrderCreateAppend
    {
        public static SlotDefinition _orgSlot = SlotDefinition.Append;
        public static MainOrderType _orgType = MainOrderType.Append;
        
          public static List<StorageTaskItem> TryCreateOrder(StorageTaskItem orderItem)
        {
            List<StorageTaskItem> orders = new List<StorageTaskItem>();
            if (orderItem == null)
                return orders;
            
            if (orderItem.Type != (int)MainOrderType.Append)
                return orders;

            var config = OrderConfigManager.Instance._orderAppends.Find(a => a.id == orderItem.Id);
            if (config == null)
                return orders;

            if (config.postOrderIds == null || config.postOrderIds.Length == 0)
                return orders;

            foreach (var id in config.postOrderIds)
            {
                var postConfig = OrderConfigManager.Instance._orderAppends.Find(a => a.id == id);
                if (postConfig == null)
                    continue;
                
                if(MainOrderManager.Instance.IsCompleteOrder(postConfig.id))
                    continue;
                
                if(MainOrderManager.Instance.HaveTask(postConfig.id))
                    continue;
                
                orders.Add(MainOrderManager.Instance.AddTask(postConfig.id, postConfig.requirements, null, _orgType, _orgSlot));
            }

            return orders;
        }
        
        public static StorageTaskItem TryCreateOrder()
        {
            return TryCreateOrder(_orgSlot);
        }
        
        public static StorageTaskItem TryCreateOrder(SlotDefinition slot)
        {
            foreach (var config in OrderConfigManager.Instance._orderVaildAppends)
            {
                if(MainOrderManager.Instance.IsCompleteOrder(config.id))
                    continue;
                
                if(MainOrderManager.Instance.HaveTask(config.id))
                    continue;

                if(config.unLockParam == null || config.unLockParam.Length == 0)
                    continue;
                
                switch (config.unLockType)
                {
                    case 1:
                    {
                        int level = ExperenceModel.Instance.GetLevel();
                        int limitLevel = int.Parse(config.unLockParam[0]);
                        
                        if (level < limitLevel)
                            continue;
                        
                        return MainOrderManager.Instance.AddTask(config.id, config.requirements, null, _orgType, slot, rewardIds:config.rewardIds, rewardNums:config.rewardNums);
                    }
                }
            }

            return null;
        }
    }
}