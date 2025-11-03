using System.Collections.Generic;
using ABTest;
using Activity.TimeOrder;
using DragonPlus.Config.TimeOrder;
using DragonU3DSDK.Storage;
using SomeWhere;

namespace Merge.Order
{
    public class MainOrderCreateTime
    {
        public static SlotDefinition _orgSlot = SlotDefinition.Time;
        public static MainOrderType _orgType = MainOrderType.Time;
        
        public static List<StorageTaskItem> TryCreateOrder()
        {
            if (!TimeOrderModel.Instance.IsOpened())
                return null;

            if (TimeOrderModel.Instance.IsTimeEnd())
                return null;

            if (TimeOrderModel.Instance.OrderId <= 0)
                return null;

            if (!MainOrderManager.Instance.IsCompleteOrder(TimeOrderModel.Instance.OrderId) || MainOrderManager.Instance.HaveTask(TimeOrderModel.Instance.OrderId))
                return null;

            if (TimeOrderConfigManager.Instance.TableTimeOrderConfigList == null)
                return null;
            
            var config = TimeOrderConfigManager.Instance.TableTimeOrderConfigList.Find(a => a.Id == TimeOrderModel.Instance.OrderId);
            if (config == null)
                return null;

            if (config.PostOrderIds == null || config.PostOrderIds.Count == 0)
                return null;

            List<StorageTaskItem> orders = new List<StorageTaskItem>();
            TimeOrderModel.Instance.OrderId = config.Id;
            foreach (var id in config.PostOrderIds)
            {
                var postConfig = TimeOrderConfigManager.Instance.TableTimeOrderConfigList.Find(a => a.Id == id);
                if (postConfig == null)
                    continue;
                
                if(MainOrderManager.Instance.IsCompleteOrder(postConfig.Id))
                    continue;
                
                if(MainOrderManager.Instance.HaveTask(postConfig.Id))
                    continue;
                
                var order = TryCreateOrder(postConfig);
                if(order == null)
                    continue;
                
                orders.Add(order);
            }

            return orders;
        }
        
        public static List<StorageTaskItem> TryCreateOrder(StorageTaskItem orderItem)
        {
            List<StorageTaskItem> orders = new List<StorageTaskItem>();
            if (orderItem == null)
                return orders;
            
            if (orderItem.Type != (int)MainOrderType.Time)
                return orders;

            if (!TimeOrderModel.Instance.IsOpened())
                return orders;
            
            if (TimeOrderModel.Instance.IsTimeEnd())
                return orders;

            if (TimeOrderConfigManager.Instance.TableTimeOrderConfigList == null)
                return orders;
            
            var config = TimeOrderConfigManager.Instance.TableTimeOrderConfigList.Find(a => a.Id == orderItem.Id);
            if (config == null)
                return orders;

            if (config.PostOrderIds == null || config.PostOrderIds.Count == 0)
            {
                TimeOrderModel.Instance.CompleteAllOrder();
                return orders;
            }

            TimeOrderModel.Instance.OrderId = config.Id;
            foreach (var id in config.PostOrderIds)
            {
                var postConfig = TimeOrderConfigManager.Instance.TableTimeOrderConfigList.Find(a => a.Id == id);
                if (postConfig == null)
                    continue;

                var order = TryCreateOrder(postConfig);
                if(order == null)
                    continue;
                
                orders.Add(order);
            }

            return orders;
        }
        
        public static StorageTaskItem TryCreateOrder(TableTimeOrderConfig config)
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
                    int loopNum = 5;
                    while (--loopNum >= 0)
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

                        if (loopNum == 0)
                        {
                            difficultyRequirements.Add(diffItems.RandomPickOne()._orderItemId);
                        }
                        else
                        {
                            while (diffItems.Count > 0)
                            {
                                var randomItem = diffItems.RandomPickOne();
                                diffItems.Remove(randomItem);
                            
                                if(MainOrderCreatorRandomCommon.HavenEnoughMerge(randomItem._orderItemId,1))
                                    continue;
                                
                                difficultyRequirements.Add(randomItem._orderItemId);

                                loopNum = -1;
                                break;
                            }
                        }
                    }
                }

                requirements = difficultyRequirements;
            }
            
            var order = MainOrderManager.Instance.AddTask(config.Id, requirements.ToArray(), null, _orgType, _orgSlot, OrderConfigManager.Instance.GetHeadSpines(OrderConfigManager.SpineType.Time).Random().id, extraRewardIds:config.RewardIds.ToArray(), extraRewardNums:config.RewardNums.ToArray());
            for (var i = 0; i < requirements.Count; i++)
            {
                if(!MainOrderCreatorRandomCommon.HavenEnoughMerge(requirements[i],1))
                    continue;
                
                TimeOrderModel.Instance.TimeOrder.OrderGiftIndex.Add(i);
            }
            
            
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