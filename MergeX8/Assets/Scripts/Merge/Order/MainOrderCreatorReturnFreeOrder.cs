using System.Collections.Generic;
using System.Linq;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using SomeWhere;

namespace Merge.Order
{
    public class MainOrderCreatorReturnFreeOrder
    {
        public static int _orderRefreshTime = 28800; //8小时cd
        public static long _expiredTime = 24 * 60 * 60 * 1000; //一天的保护时间
        public static string _orderRefreshTimeKey = "MainOrder_ReturnFree";
        
        public static bool CanCreate(SlotDefinition slot)
        {
            if (slot != SlotDefinition.UserFree)
                return false;

            if ((long)APIManager.Instance.GetServerTime() - (long)StorageManager.Instance.GetStorage<StorageCommon>().InstalledAt < _expiredTime)
                return false;

            long refreshTime = 0;
            if (!MainOrderManager.Instance.StorageTaskGroup.OrderRefreshTime.ContainsKey(_orderRefreshTimeKey))
            {
                MainOrderManager.Instance.StorageTaskGroup.OrderRefreshTime.Add(_orderRefreshTimeKey, 0);
            }
            refreshTime = MainOrderManager.Instance.StorageTaskGroup.OrderRefreshTime[_orderRefreshTimeKey];

            long serverTime = (long)APIManager.Instance.GetServerTime() / 1000;
            if (refreshTime == 0)
            {
                MainOrderManager.Instance.StorageTaskGroup.OrderRefreshTime[_orderRefreshTimeKey] = serverTime;
                return false;
            }

            if (serverTime - refreshTime <= _orderRefreshTime)
                return false;

            MainOrderManager.Instance.StorageTaskGroup.OrderRefreshTime[_orderRefreshTimeKey] = 0;
            return true;
        }

        public static StorageTaskItem CreateOrder(SlotDefinition slot)
        {
            Dictionary<int, List<int>> sortedByKey = MergeManager.Instance.GetCodeLevelMap(true, true,false, false, 20413);

            if (sortedByKey == null || sortedByKey.Count == 0)
            {
                MainOrderCreatorRandomCommon.RestRefreshTime(_orderRefreshTimeKey);
                return null;
            }
            
            int randomId = -1;
            foreach (var kv in sortedByKey)
            {
                if(kv.Key >= 7)
                    continue;

                if(kv.Value == null || kv.Value.Count == 0)
                    continue;
                
                randomId = kv.Value.RandomPickOne();
                break;
            }

            if (randomId < 0)
            {
                MainOrderCreatorRandomCommon.RestRefreshTime(_orderRefreshTimeKey);
                return null;
            }

            List<int> seatIndex = new List<int>();
            seatIndex.Add(1);
            return MainOrderManager.Instance.AddTask(MainOrderManager.Instance.StorageTaskGroup.OnlyId++, new []{randomId}, seatIndex, MainOrderType.ReturnUserFree, slot);
        }
    }
}