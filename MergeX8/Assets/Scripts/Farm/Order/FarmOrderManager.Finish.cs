using Activity.CrazeOrder.Model;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Farm.Model;
using Farm.View;
using Gameplay;
using Merge.Order;

namespace Farm.Order
{
    public partial class FarmOrderManager
    {
        public void FinishOrder(OrderCell order)
        {
            if(order == null || order._storage == null)
                return;
            
            for (int i = 0; i < order._storage.NeedItemIds.Count; i++)
            {
                int id = order._storage.NeedItemIds[i];
                int num = order._storage.NeedItemNums[i];
                
                FarmModel.Instance.ConsumeProductItem(id, num);
            }

            AudioManager.Instance.PlaySound(20);
            FarmOrder.FinishOrderCount++;

            if (order._storage.OrgId != null)
                FarmOrder.FinishOrderIds[order._storage.OrgId] = order._storage.OrgId;

            FarmOrder.Orders.Remove(order._storage);
            
            AddReward(order._storage);

            FarmModel.Instance.SendBI_FinishOrder(order._storage);
            
            var createOrder = TryCreateOrder(order._storage);

            FlyReward(order, () =>
            {
                EventDispatcher.Instance.DispatchEvent(EventEnum.FARM_ORDER_REFRESH, order, createOrder);
            });
        }
    
        public void AddReward(StorageFarmOrderItem storage)
        {
            for (int i = 0; i < storage.RewardIds.Count; i++)
            {
                int id = storage.RewardIds[i];
                int num = storage.RewardNums[i];

                BiEventAdventureIslandMerge.Types.ItemChangeReason reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.TaskRewardFarm;
                if(storage.Slot == (int)OrderSlot.Activity_TimeOrder)
                    reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.FarmTimeOrder;
                
                if (UserData.Instance.IsFarmExp(id))
                {
                    FarmModel.Instance.AddExp(num);
                    GameBIManager.Instance.SendItemChangeEvent(UserData.ResourceId.Farm_Exp, num, (ulong)FarmModel.Instance.storageFarm.Exp, new GameBIManager.ItemChangeReasonArgs()
                    {
                        reason = reason,
                        data1 = storage.Id,
                        data2 = id.ToString(),
                    });
                }
                else if (UserData.Instance.IsFarmRes(id))
                {
                    GameBIManager.Instance.SendItemChangeEvent((UserData.ResourceId)id, num, (ulong)FarmModel.Instance.GetProductItemNum(id), new GameBIManager.ItemChangeReasonArgs()
                    {
                        reason = reason,
                        data1 = storage.Id,
                        data2 = id.ToString(),
                    });
                    FarmModel.Instance.AddProductItem(id, num);
                }
                else if (UserData.Instance.IsResource(id))
                {
                    UserData.Instance.AddRes(id, num, new GameBIManager.ItemChangeReasonArgs()
                    {
                        reason = reason,
                        data1 = storage.Id,
                        data2 = id.ToString(),
                    });
                }
                else
                {
                    var mergeItem = MergeManager.Instance.GetEmptyItem();
                    mergeItem.Id = id;
                    mergeItem.State = 1;
                    MergeManager.Instance.AddRewardItem(mergeItem,MergeBoardEnum.Main, num, true);

                    BiEventAdventureIslandMerge.Types.MergeEventType MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonEasterReward;
                    if (storage.Slot == (int)OrderSlot.Activity_TimeOrder)
                        MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonFarmTimeOrderGet;
                    
                    GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                    {
                        MergeEventType = MergeEventType,
                        itemAId = id,
                        isChange = true,
                    });
                    EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_REWARD_REFRESH);
                }
            }
        }
    }
}