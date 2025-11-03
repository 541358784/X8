using System.Collections.Generic;
using ABTest;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Storage;

namespace Merge.Order
{
    public partial class MainOrderManager
    {
        public string AbKey = "AB_ORDER_RANDOM_PLAN_B";
        
        public bool IsOpenOrderPlanB()
        {
            if(StorageManager.Instance.GetStorage<StorageHome>().OrderRules <= 0)
                return false;

            if (!OpenOderRules())
            {
                if (AssignedBGroup())
                    return true;
                
                return false;
            }
            
            return ABTestManager.Instance.IsOpenBeginnerOrder();
        }

        public void RecordCreateRandomOrder()
        {
            if(!StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig.ContainsKey(AbKey))
                return;

            HandleOrderRules();
        }

        private void HandleOrderRules()
        {
            if(StorageManager.Instance.GetStorage<StorageHome>().OrderRules > 0)
                return;
            
            List<OrderRules> datas = AdConfigManager.Instance.GetConfig<OrderRules>();

            string country = StorageManager.Instance.GetStorage<StorageCommon>().Country;
            int campaignTypeCode = StorageManager.Instance.GetStorage<StorageCommon>().CampaignTypeCode;

            string platform = "google";
#if UNITY_IPHONE || UNITY_IOS 
            platform = "ios";
#endif
            foreach (var rulese in datas)
            {
                if(!rulese.Campaign.Contains(campaignTypeCode))
                    continue;
                
                if(rulese.Platform != platform)
                    continue;

                if (rulese.Country == null || rulese.Country.Count == 0 || rulese.Country.Contains(country))
                {
                    StorageManager.Instance.GetStorage<StorageHome>().OrderRules = rulese.Id;
                    break;
                }
            }

            if (StorageManager.Instance.GetStorage<StorageHome>().OrderRules < 0)
                StorageManager.Instance.GetStorage<StorageHome>().OrderRules = 101;//默认分组
        }

        private bool OpenOderRules()
        {
            var orderRules = GetOrderRules();
            if (orderRules == null)
                return false;

            return orderRules.OpenOrderAB;
        }

        private bool AssignedBGroup()
        {
            var orderRules = GetOrderRules();
            if (orderRules == null)
                return false;

            return orderRules.AssignedABGroup == 2;
        }

        private OrderRules GetOrderRules()
        {
            List<OrderRules> datas = AdConfigManager.Instance.GetConfig<OrderRules>();
            if (StorageManager.Instance.GetStorage<StorageHome>().OrderRules < 0)
                return null;
            
            return datas.Find(a => a.Id == StorageManager.Instance.GetStorage<StorageHome>().OrderRules);
        }
    }
}