using System.Collections.Generic;
using DragonU3DSDK.Storage;

namespace Merge.Order
{
    public class MainOrderCreatorRandom2
    {
        public static SlotDefinition _orgSlot = SlotDefinition.Slot3;
        public static MainOrderType _orgType = MainOrderType.Random2;
        
        public static bool CanCreate(SlotDefinition slot)//30分钟cd 
        {
            return CanCreate_Level(slot);
            
            string orderRefreshTimeKey = ((int)slot).ToString();
            return MainOrderCreatorRandomCommon.CanCreate(orderRefreshTimeKey, _orgType, _orgSlot, slot);
        }
 
        public static StorageTaskItem CreateOrder(SlotDefinition slot)
        {
            return MainOrderCreatorRandomCommon.CreateOrder(_orgType, slot);
        }

        private static bool CanCreate_Level(SlotDefinition slot)
        {
            if (slot != SlotDefinition.Slot3)
                return false;

            TableOrderRandom orderRandom = OrderConfigManager.Instance.GetRandomConfig((int)MainOrderType.Random2);
            if (orderRandom.unlockLevel > ExperenceModel.Instance.GetLevel())
                return false;

            //如果有没有完成的任务 除去特殊任务
            //任务都完成了 立马就刷新
            bool hasOtherOrder = false;
            Dictionary<int, int> mergeItemCounts = MergeManager.Instance.GetMergeItemCounts(MergeBoardEnum.Main);
            foreach (var kv in MainOrderManager.Instance.StorageTaskGroup.CurTasks)
            {
                if(!MainOrderCreatorRandomCommon.IsRandomOrder(kv))
                    continue;

                bool isFinish = true;
                foreach (var item in kv.ItemIds)
                {
                    if (!mergeItemCounts.ContainsKey(item))
                    {
                        isFinish = false;
                        break;
                    }
                }

                if (!isFinish)
                {
                    hasOtherOrder = true;
                    break;
                }
            }

            string orderRefreshTimeKey = ((int)slot).ToString();
            if (!hasOtherOrder)
            {
                MainOrderManager.Instance.StorageTaskGroup.OrderRefreshTime[orderRefreshTimeKey] = 0;
                return true;
            }

            return MainOrderCreatorRandomCommon.CanCreate(orderRefreshTimeKey, orderRandom);
        }
    }
}