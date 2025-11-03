using System.Collections.Generic;
// using DragonPlus.Config.Game;
using DragonPlus.Config.TMatch;
using DragonPlus.Config.TMatchShop;
using DragonPlus.ConfigHub;
// using DragonPlus.ConfigHub.IAP;
using DragonU3DSDK.Storage;
using Framework;

namespace TMatch
{
    public enum IAPShopType
    {
        Coin = 1, //金币
        Pack = 2, //礼包
        NoAdPack = 3, //去广告
        Revive = 5, //复活
        IceBreaking = 5, //破冰
    }

    public class IAPItemData
    {
        public Shop shopCfg;
        // public Store storeCfg;
    }

    public class IAPModel
    {
        public StorageTMShop stoage;

        // private RemoveAd removeAdCfg;
        // private Piggybank piggyBankCfg;
        // private List<Store> storeList = new List<Store>();
        private List<IAPItemData> iapItemDatas = new List<IAPItemData>();

        public IAPModel()
        {
            stoage = StorageManager.Instance.GetStorage<StorageTMatch>().Shop;
        }

        // public Store GetStoreCfg(int shopId)
        // {
        //     List<Store> stores = GetStores();
        //     return stores.Find(x => x.ShopId == shopId);
        // }

        // public RemoveAd GetRemoveAdCfg()
        // {
        //     if (null == removeAdCfg)
        //     {
        //         int groupId = 100;
        //         var groups = IAPConfigManager.Instance.GetConfig<Mapping>();
        //         if (groups != null && groups.Count > 0) groupId = groups[0].RemoveAdGroup;
        //         var removeAds = IAPConfigManager.Instance.GetConfig<RemoveAd>();
        //         removeAdCfg = removeAds.Find(x => x.GroupId == groupId);
        //     }
        //     return removeAdCfg;
        // }

        // public Piggybank GetPiggyBankCfg()
        // {
        //     if (null == piggyBankCfg)
        //     {
        //         int groupId = 100;
        //         var groups = IAPConfigManager.Instance.GetConfig<Mapping>();
        //         if (groups != null && groups.Count > 0) groupId = groups[0].PigbankGroup;
        //         var piggyBank = IAPConfigManager.Instance.GetConfig<Piggybank>();
        //         piggyBankCfg = piggyBank.Find(x => x.PigbankGroup == groupId);
        //     }
        //     return piggyBankCfg;
        // }

        // public List<Store> GetStores()
        // {
        //     if (storeList.Count > 0) return storeList;
        //     int groupId = 100;
        //     var groups = IAPConfigManager.Instance.GetConfig<Mapping>();
        //     if (groups != null && groups.Count > 0) groupId = groups[0].IapGroup;
        //     var stores = IAPConfigManager.Instance.GetConfig<Store>();
        //     foreach (var p in stores)
        //     {
        //         if (p.IapGroup == groupId)
        //         {
        //             storeList.Add(p);
        //         }
        //     }
        //     storeList.Sort((a, b) =>
        //     {
        //         return a.OrderId - b.OrderId;
        //     });
        //     return storeList;
        // }

        public List<IAPItemData> GetIAPItemDatas()
        {
            if (iapItemDatas.Count > 0) return iapItemDatas;
            // List<Store> stores = GetStores();
            // foreach (var p in stores)
            // {
            //     IAPItemData itemData = new IAPItemData();
            //     // itemData.storeCfg = p;
            //     itemData.shopCfg = GameConfigManager.Instance.GetShopCfgById(p.ShopId);
            //     iapItemDatas.Add(itemData);
            // }

            foreach (var shopConfig in TMatchConfigManager.Instance.ShopConfigList)
            {
                var config = StoreModel.Instance.GetShopConfigById(shopConfig.id);
                if (!(config is {productType: (int) StoreModel.eProductType.TMatchBundle}))
                    continue;
                iapItemDatas.Add(new IAPItemData {shopCfg = config.ChangeTableShopToTMatchShop()});
            }

            return iapItemDatas;
        }

        public IAPItemData GetRemoveAdItemData(int purchaseId)
        {
            var datas = GetIAPItemDatas();
            return datas.Find(x => x.shopCfg.id == purchaseId);
        }

        public bool IsInBundleList(int shopId)
        {
            // List<IAPItemData> itemDatas = GetIAPItemDatas();
            // return itemDatas.Find(x => x.shopCfg.BundleList != null && x.shopCfg.BundleList.FindAll(x => x == shopId).Count > 0) != null;
            return false;
        }

        public IAPItemData GetDrivedWithBundleList(int shopId)
        {
            List<IAPItemData> iapItemDatas = GetIAPItemDatas();
            // IAPItemData temp = null;
            // // var shopCfg = GameConfigManager.Instance.GetShopCfgById(shopId);
            // var shopCfg = StoreModel.Instance.GetShopConfigById(shopId);
            // if (shopCfg.LmtNum > 0)
            // {
            //     int purchasedTimes = IAPController.Instance.model.GetPurchasedTimes(shopCfg.Id);
            //     if (purchasedTimes >= shopCfg.LmtNum)
            //     {
            //         if (!(shopCfg.BundleList == null || shopCfg.BundleList.Count == 0))
            //         {
            //             int index = purchasedTimes - shopCfg.LmtNum;
            //             if (index > shopCfg.BundleList.Count - 1) index = shopCfg.BundleList.Count - 1;
            //             temp = iapItemDatas.Find(x => x.shopCfg.Id == shopCfg.BundleList[index]);
            //         }
            //     }
            //     else
            //     {
            //         temp = iapItemDatas.Find(x => x.shopCfg.Id == shopId);
            //     }
            // }
            // else
            // {
            //     temp = iapItemDatas.Find(x => x.shopCfg.Id == shopId);
            // }
            return iapItemDatas.Find(x => x.shopCfg.id == shopId);
        }

        public int GetPurchasedTimes(int shopId)
        {
            if (stoage.PurchasedTimes.ContainsKey(shopId)) return stoage.PurchasedTimes[shopId];
            return 0;
        }

        public void AddPurchasedTimes(int shopId, int cnt)
        {
            if (!stoage.PurchasedTimes.ContainsKey(shopId))
            {
                stoage.PurchasedTimes.Add(shopId, cnt);
            }
            else
            {
                stoage.PurchasedTimes[shopId] += cnt;
            }
        }
    }
}