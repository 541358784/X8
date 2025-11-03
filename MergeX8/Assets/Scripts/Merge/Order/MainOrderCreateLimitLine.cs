using System.Collections.Generic;
using ABTest;
using Activity.LimitTimeOrder;
using DragonPlus.Config.LimitOrderLine;
using DragonU3DSDK.Storage;
using SomeWhere;

namespace Merge.Order
{
    public class MainOrderCreateLimitLine
    {
        public static SlotDefinition _orgSlot = SlotDefinition.Limit;
        public static MainOrderType _orgType = MainOrderType.Limit;
        
        public static StorageTaskItem TryCreateOrder(StorageTaskItem orderItem)
        {
            if (orderItem == null)
                return null;
            
            if (orderItem.Type != (int)MainOrderType.Limit)
                return null;

            if (!LimitTimeOrderModel.Instance.IsOpened())
                return null;
            
            if (LimitTimeOrderModel.Instance.IsTimeEnd())
                return null;

            if (LimitOrderLineConfigManager.Instance.TableTimeOrderLineGroupList == null)
                return null;
            
            if (LimitTimeOrderModel.Instance.OrderId <= 0)
                return null;

            if (LimitTimeOrderModel.Instance.OrderId != orderItem.Id)
                return null;
            
            var config = LimitOrderLineConfigManager.Instance.TableTimeOrderLineGroupList.Find(a => a.Id == LimitTimeOrderModel.Instance.GroupId);
            if (config == null)
                return null;

            int index = config.OrderIds.FindIndex(a => a == LimitTimeOrderModel.Instance.OrderId);
            if (index < 0 || index >= config.OrderIds.Count - 1)
            {
                LimitTimeOrderModel.Instance.CompleteNum ++;
                LimitTimeOrderModel.Instance.CompleteAllOrder();
                
                List<ResData> resDatas = new List<ResData>();
                MainOrderManager.Instance.AddResource(orderItem.Id, orderItem.Id, config.RewardIds, config.RewardNums, ref resDatas);

                UIManager.Instance.OpenUI(UINameConst.UIPopupLimitOrder, true);
                return null;
            }

            int nextOrderId = config.OrderIds[index + 1];
            
            if(MainOrderManager.Instance.IsCompleteOrder(nextOrderId))
                return null;
                
            if(MainOrderManager.Instance.HaveTask(nextOrderId))
                return null;
            
            var orderConfig = LimitOrderLineConfigManager.Instance.TableTimeOrderLineConfigList.Find(a => a.Id == nextOrderId);
            
            LimitTimeOrderModel.Instance.OrderId = nextOrderId;
            LimitTimeOrderModel.Instance.CompleteNum ++;
            LimitTimeOrderModel.Instance.UpdateJoinTime(orderConfig);
            
            var order = TryCreateOrder(orderConfig);
            if(order == null)
                return null;

            return order;
        }

        public static StorageTaskItem TryCreateOrder(TableTimeOrderLineConfig config, bool isInit = false)
        {
            if (config == null)
                return null;
            
            if(MainOrderManager.Instance.IsCompleteOrder(config.Id))
                return null;
                
            if(MainOrderManager.Instance.HaveTask(config.Id))
                return null;
            
            List<int> requirements = config.Requirements;
            if (ABTestManager.Instance.GetCreateOrderType() == CreateOrderType.Difficulty)
            {
                List<int> difficultyRequirements = new List<int>();
                var availableItems = MainOrderCreatorRandomCommon.GetAvailableItems(_orgSlot);
                
                
                List<int> minDifficultyList = config.MinDifficulty;
                List<int> maxDifficultyList = config.MaxDifficulty;
                
                for (int i = 0; i < minDifficultyList.Count; i++)
                {
                    int minDifficulty = minDifficultyList[i];
                    int maxDifficulty = maxDifficultyList[i];
                    
                    var diffItems = MainOrderCreatorRandomCommon.FilterByDifficulty(availableItems, minDifficulty, maxDifficulty, null, _orgSlot);
                    if (diffItems == null || diffItems.Count == 0)
                    {
                        for(int j = 1; j < 6; j++)
                        {
                            diffItems = MainOrderCreatorRandomCommon.FilterByDifficulty(availableItems, minDifficulty, (int)(1.0f*maxDifficulty * (1f+j*0.25f)), null, _orgSlot);
                            if(diffItems.Count > 0)
                                break;
                        }
                    }
                    
                    if(diffItems == null || diffItems.Count == 0)
                        diffItems = MainOrderCreatorRandomCommon.FilterByDifficulty(availableItems, 0, (int)Difficulty.maxDifficulty, null, _orgSlot);

                    if(diffItems == null || diffItems.Count == 0)
                        continue;
                    
                    difficultyRequirements.Add(diffItems.RandomPickOne()._orderItemId);
                }

                requirements = difficultyRequirements;
            }
            
            var order = MainOrderManager.Instance.AddTask(config.Id, requirements.ToArray(), 
                null, _orgType, _orgSlot, OrderConfigManager.Instance.GetHeadSpines(OrderConfigManager.SpineType.Normal).Random().id);

            if (isInit)
            {
                if(MergeTaskTipsController.Instance == null || !MergeTaskTipsController.Instance.gameObject.activeInHierarchy)
                    return order;
            
                MergeTaskTipsController.Instance.RefreshTask(new List<StorageTaskItem>(){order}, () =>
                {
                    EventDispatcher.Instance.DispatchEvent(EventEnum.TASK_REFRESH,MergeBoardEnum.Main);
                    MergeMainController.Instance?.UpdateTaskRedPoint();
                });
            }
            
            return order;
        }
    }
}