using System.Collections.Generic;
using Activity.CrazeOrder.Model;
using DragonPlus.Config.CrazeOrder;
using DragonU3DSDK.Storage;
using SomeWhere;

namespace Merge.Order
{
    public class MainOrderCreateCraze
    {
        public static SlotDefinition _orgSlot = SlotDefinition.Craze;
        public static MainOrderType _orgType = MainOrderType.Craze;
        
        public static StorageTaskItem TryCreateOrder(StorageTaskItem orderItem)
        {
            if (orderItem == null)
                return null;
            
            if (orderItem.Type != (int)_orgType)
                return null;

            if (!CrazeOrderModel.Instance.IsOpened())
                return null;
            
            if (CrazeOrderModel.Instance.IsTimeEnd())
                return null;

            var stageConfigs = CrazeOrderConfigManager.Instance.GetStageConfigs();
            if (stageConfigs == null)
                return null;
            
            var config = CrazeOrderConfigManager.Instance.GetOrderConfig(CrazeOrderModel.Instance.GroupId);
            if (config == null)
                return null;

            CrazeOrderModel.Instance.CompleteNum ++;
            CrazeStageConfig stageConfig = stageConfigs.Find(a => a.Id == CrazeOrderModel.Instance.Stage);
            if (stageConfig == null)
                return null;
            if (CrazeOrderModel.Instance.CompleteNum >= stageConfig.OrderNum)
                CrazeOrderModel.Instance.Stage++;
            
            if (CrazeOrderModel.Instance.CompleteNum >= stageConfigs[stageConfigs.Count - 1].OrderNum)
            {
                CrazeOrderModel.Instance.CompleteAllOrder();

                List<ResData> resDatas = new List<ResData>();
                MainOrderManager.Instance.AddResource(orderItem.Id, orderItem.Id, CrazeOrderConfigManager.Instance.CrazeOrderSettingList[0].RewardIds, CrazeOrderConfigManager.Instance.CrazeOrderSettingList[0].RewardNums, ref resDatas);

                UIManager.Instance.OpenUI(UINameConst.UICrazeOrderMain, true);
                return null;
            }

            var orderConfig = config.Find(a => a.Stage == CrazeOrderModel.Instance.Stage);
            
            var order = TryCreateOrder(orderConfig);
            if(order == null)
                return null;

            return order;
        }

        public static StorageTaskItem TryCreateOrder(CrazeOrderConfig config, bool isInit = false)
        {
            if (config == null)
                return null;
            
            var availableItems = MainOrderCreatorRandomCommon.GetAvailableItems(_orgSlot);
            var firstAvailableItems = MainOrderCreatorRandomCommon.FilterByDifficulty(availableItems, config.MinDifficulty, config.MaxDifficulty, null, _orgSlot);
            if (firstAvailableItems == null || firstAvailableItems.Count == 0)
            {
                int minDifficulty = config.MinDifficulty;
                int maxDifficulty = config.MaxDifficulty;
                    
                for(int j = 1; j < 6; j++)
                {
                    firstAvailableItems = MainOrderCreatorRandomCommon.FilterByDifficulty(availableItems, minDifficulty, (int)(1.0f*maxDifficulty * (1f+j*0.25f)), null, _orgSlot);
                    if(firstAvailableItems.Count > 0)
                        break;
                }
            }
                    
            if(firstAvailableItems == null || firstAvailableItems.Count == 0)
                firstAvailableItems = MainOrderCreatorRandomCommon.FilterByDifficulty(availableItems, 0, (int)Difficulty.maxDifficulty, null, _orgSlot);

            if (firstAvailableItems == null || firstAvailableItems.Count == 0)
                return null;
            
            int[] requirements = new int[1];
            requirements[0] = firstAvailableItems.RandomPickOne()._orderItemId;
            
            var stageConfigs = CrazeOrderConfigManager.Instance.GetStageConfigs();
            CrazeStageConfig stageConfig = stageConfigs.Find(a => a.Id == CrazeOrderModel.Instance.Stage);
            if (stageConfig == null)
                return null;
            
            int[] extraRewardIds = null;
            int[] extraRewardNums = null;
            if (CrazeOrderModel.Instance.CompleteNum == stageConfig.OrderNum - 1)
            {
                extraRewardIds = stageConfig.RewardIds.ToArray();
                extraRewardNums = stageConfig.RewardNums.ToArray();
            }
            var order = MainOrderManager.Instance.AddTask(MainOrderManager.Instance.StorageTaskGroup.OnlyId++, requirements, 
                null, _orgType, _orgSlot, OrderConfigManager.Instance.GetHeadSpines(OrderConfigManager.SpineType.Normal).Random().id, extraRewardIds:extraRewardIds, extraRewardNums:extraRewardNums ,coinFactor:config.CoinFactor);

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