using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;

namespace Merge.Order
{
    public class MainOrderCreatorRandom1 //1小时cd 每次可刷新5个
    {
        public static bool CanCreate(SlotDefinition slot)
        {
            if (slot != SlotDefinition.Slot1 && slot != SlotDefinition.Slot2)
                return false;

            TableOrderRandom orderRandom = OrderConfigManager.Instance.GetRandomConfig((int)MainOrderType.Random1);
            if (orderRandom.unlockLevel > ExperenceModel.Instance.GetLevel())
                return false;

            long startTimer = -1;
            string orderRefreshTimeKey = ((int)slot).ToString();
            if (MainOrderManager.Instance.StorageTaskGroup.OrderRefreshTime.ContainsKey(orderRefreshTimeKey))
            {
                startTimer = MainOrderManager.Instance.StorageTaskGroup.OrderRefreshTime[orderRefreshTimeKey];
            }

            long serverTime = (long)APIManager.Instance.GetServerTime() / 1000;
            string pointRefreshKey = "MainOrderCreatorRandom1_Point";
            long orderPoints = 0;
            if (startTimer == 0)
            {
                if(MainOrderManager.Instance.StorageTaskGroup.OrderRefreshTime.ContainsKey(pointRefreshKey))
                    orderPoints =  MainOrderManager.Instance.StorageTaskGroup.OrderRefreshTime[pointRefreshKey];

                if (orderPoints == 0)
                {
                    MainOrderManager.Instance.StorageTaskGroup.OrderRefreshTime[orderRefreshTimeKey] = serverTime;
                }
            }
            else
            {
                float refreshTime = orderRandom.refreshLevelTime;
                if (MainOrderCreatorRandomCommon.CreateType == CreateOrderType.Difficulty)
                    refreshTime = orderRandom.refreshDiffTime;
                
                if (startTimer < 0 || serverTime - startTimer >= refreshTime)
                {
                    int random1Count = 0;
                    for (var i = SlotDefinition.Slot1; i >= SlotDefinition.Slot2; i++)
                    {
                        if (MainOrderManager.Instance.StorageTaskGroup.CurTasks.Find(a => a.Type == (int) MainOrderType.Random1) != null)
                            random1Count++;
                    }

                    int point = orderRandom.levelPoint;
                    if (MainOrderCreatorRandomCommon.CreateType == CreateOrderType.Difficulty)
                        point = orderRandom.diffPoint;
                    
                    orderPoints = point- random1Count;
                    MainOrderManager.Instance.StorageTaskGroup.OrderRefreshTime[orderRefreshTimeKey] = 0;
                }
            }

            if (orderPoints <= 0)
                return false;

            orderPoints -= 1;
            MainOrderManager.Instance.StorageTaskGroup.OrderRefreshTime[pointRefreshKey] = orderPoints;
            return true;
        }

        public static StorageTaskItem CreateOrder(SlotDefinition slot)
        {
            return MainOrderCreatorRandomCommon.CreateOrder(MainOrderType.Random1, slot);
        }
    }
}