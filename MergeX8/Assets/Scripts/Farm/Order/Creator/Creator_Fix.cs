using DragonPlus.Config.Farm;
using DragonU3DSDK.Storage;
using Farm.Model;

namespace Farm.Order.Creator
{
    public class Creator_Fix
    {
        public static bool CanCreate(OrderSlot slot)
        {
            int index = FarmOrderManager.Instance.FarmOrder.FixIndex;
            if (index >= FarmConfigManager.Instance.TableFarmOrderFixList.Count)
                return false;

            return true;
        }
        
        public static StorageFarmOrderItem TryCreateOrder(OrderSlot slot, StorageFarmOrderItem finishOrder = null)
        {
            int index = FarmOrderManager.Instance.FarmOrder.FixIndex;
            if (index >= FarmConfigManager.Instance.TableFarmOrderFixList.Count)
                return null;

            var config = FarmConfigManager.Instance.TableFarmOrderFixList[index];
            if (!config.Slots.Contains((int)slot))
                return null;
            
            if (FarmModel.Instance.GetLevel() < config.UnlockLevel)
                return null;

            if (!config.Slots.Contains((int)slot))
                return null;
            
            FarmOrderManager.Instance.FarmOrder.FixIndex++;
            return FarmOrderManager.Instance.CreatorOrder(config.Id, config.RequirementIds, config.RequirementsNums, slot, config.RewardIds, config.RewardNums);
        }
    }
}