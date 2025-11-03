using System.Collections.Generic;
using UnityEngine;
using System;
using DragonPlus.Config.Farm;
using DragonU3DSDK.Storage;
using DragonU3DSDK.Network.API;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK;
using Gameplay;
using Merge.Order;
using SomeWhere;

public sealed partial class StoreModel
{
    private const int FlashSaleCount = 6;
    [NonSerialized] public Vector3 nowItemPos;
    private bool isInit = false;

    private StorageDictionary<int, StorageStoreItem> storeItems
    {
        get { return StorageManager.Instance.GetStorage<StorageHome>().StoreItems; }
    }

    private StorageFlashSale flashSale
    {
        get { return StorageManager.Instance.GetStorage<StorageHome>().FlashSale; }
    }

    private StoragePigSale pigSale
    {
        get { return StorageManager.Instance.GetStorage<StorageHome>().PigSale; }
    }

    private Dictionary<int, FlashSale> flashSaleConfigs = new Dictionary<int, FlashSale>();
    private List<int> flashSaleStarNums = new List<int>();

    protected override void InitImmediately()
    {
        if (!isInit)
        {
            InitConfig();
        }
    }

    public void InitConfig()
    {
        flashSaleConfigs.Clear();
        flashSaleStarNums.Clear();
        if (flashSale.CurConfigGroup == 0)
        {
            Common commonData = AdConfigHandle.Instance.GetCommon();
            flashSale.CurConfigGroup = commonData.FlashSale;
        }

        foreach (var config in AdConfigHandle.Instance.GetFlashSales(flashSale.CurConfigGroup))
        {
            if (!flashSaleConfigs.ContainsKey(config.StarNum) && config.Type == 0)
            {
                flashSaleConfigs.Add(config.StarNum, new FlashSale());
                flashSaleStarNums.Add(config.StarNum);
                flashSaleConfigs[config.StarNum] = config;
            }
        }

        isInit = true;
    }

    private bool IsBuyFlashSaleItem()
    {
        if (flashSale.Items == null || flashSale.Items.Count <= 0)
            return false;

        foreach (var flashSaleItem in flashSale.Items)
        {
            if (flashSaleItem.BuyCount > 0) return true;
        }

        return false;
    }

    private void GenerateFlashShopItems(bool isAuto = false)
    {
        Common commonData = AdConfigHandle.Instance.GetCommon();
        if (commonData != null)
            flashSale.CurConfigGroup = commonData.FlashSale;
        InitConfig();
        bool isBuy = IsBuyFlashSaleItem();
        var config = GetCurFlashSaleConfig();
        if (config == null)
        {
            Debug.LogError("闪购刷新失败-----------");
            return;
        }

        flashSale.Items.Clear();
        //前三次固定刷新
        if (flashSale.TotalRefreshCount <= 1)
        {
            config = AdConfigHandle.Instance.GetFlashSales(flashSale.CurConfigGroup)
                .Find(x => x.Type == flashSale.TotalRefreshCount);
            // for (int i = 0; i < config.CoinLocality.Count; i++)
            // {
            //     StorageStoreItem item = new StorageStoreItem();
            //     item.ItemId = config.CoinItem[i];
            //     item.Price = config.CoinPrice[i];
            //     item.PriceIdex.AddRange(config.CoinIncrease);
            //     item.BuyCount = 0;
            //     item.CanBuyCount = config.CoinTimes;
            //     item.Site = config.CoinLocality[i];
            //     item.PriceType = 2;
            //     flashSale.Items.Add(item);
            // }

            for (int i = 0; i < config.GemLocality.Count; i++)
            {
                StorageStoreItem item = new StorageStoreItem();
                item.ItemId = config.GemItem[i];
                item.Price = config.GemPrice[i];
                item.PriceIdex.AddRange(config.GemIncrease);
                item.BuyCount = 0;
                var itemConfig = GameConfigManager.Instance.GetItemConfig(item.ItemId);
                if (itemConfig!=null && config.BuildingMergeline.Contains(itemConfig.in_line))
                {
                    item.CanBuyCount = config.MaxTimes;
                }
                else
                {
                    item.CanBuyCount = config.GemTimes;
                }
                item.Site = config.GemLocality[i];
                item.PriceType = 1;
                flashSale.Items.Add(item);
            }
        }
        else
        {
            List<int> genItemList = new List<int>();
            // for (int i = 0; i < config.CoinLocality.Count; i++)
            // {
            //     var randomItemId = GenerateRandomItem(config.CoinItem, config.CionWighht, genItemList);
            //     genItemList.Add(randomItemId);
            //     int indexOfRandom = config.CoinItem.IndexOf(randomItemId);
            //     if (indexOfRandom < 0)
            //         indexOfRandom = 0;
            //     StorageStoreItem item = new StorageStoreItem();
            //     item.ItemId = config.CoinItem[indexOfRandom];
            //     item.Price = config.CoinPrice[indexOfRandom];
            //     item.PriceIdex.AddRange(config.CoinIncrease);
            //     item.BuyCount = 0;
            //     item.CanBuyCount = config.CoinTimes;
            //     item.Site = config.CoinLocality[i];
            //     item.PriceType = 2;
            //     flashSale.Items.Add(item);
            // }

            genItemList.Clear();
            for (int i = 0; i < config.GemLocality.Count; i++)
            {
                var randomItemId = GenerateRandomItem(config.GemItem, config.GmeWighht, genItemList,config);
                genItemList.Add(randomItemId);
                int indexOfRandom = config.GemItem.IndexOf(randomItemId);
                if (indexOfRandom < 0)
                    indexOfRandom = 0;
                StorageStoreItem item = new StorageStoreItem();
                item.ItemId = config.GemItem[indexOfRandom];
                item.Price = config.GemPrice[indexOfRandom];
                item.PriceIdex.AddRange(config.GemIncrease);
                item.BuyCount = 0;
                var itemConfig = GameConfigManager.Instance.GetItemConfig(randomItemId);
                if (itemConfig!=null && config.BuildingMergeline.Contains(itemConfig.in_line))
                {
                    item.CanBuyCount = config.MaxTimes;
                }
                else
                {
                    item.CanBuyCount = config.GemTimes;
                }
                item.Site = config.GemLocality[i];
                item.PriceType = 1;
                flashSale.Items.Add(item);
            }
            HardTaskLogic(config);
        }

        GenerateFarmSaleItem();
    }

    /// <summary>
    /// 给两个困难任务需要的
    /// </summary>
    /// <param name="flashSaleConfig"></param>
    public void HardTaskLogic(FlashSale flashSaleConfig)
    {
        List<int> tempItem=new List<int>();
        for (int j = 0; j < flashSaleConfig.HardItem.Count; j++)
        {
            if (MainOrderManager.Instance.IsTaskNeedInLineItem(flashSaleConfig.HardItem[j]))
            {
                tempItem.Add(flashSaleConfig.HardItem[j]);
            }
        }
        var itemList = flashSale.Items.FindAll(a => { return a.PriceType == 1; });
        int k = tempItem.Count > 2 ? 2 : tempItem.Count;
        for (int i = 0; i < k; i++)
        {
            if (itemList.Count <= 0)
                break;
            var item = itemList.RandomPickOne();
            itemList.Remove(item);
            for (int j = i; j < itemList.Count; j++)
            {       
                if (!flashSaleConfig.BuildingMergeline.Contains(itemList[j].ItemId))
                {
                    item = itemList[j];
                    break;
                }
            }

            int radndomId = tempItem.RandomPickOne();
            tempItem.Remove(radndomId);
            Debug.Log("[flashsale]---->Gem替换为任务需要物品---itemid-->" +
                      radndomId + "位置 --->" + item.Site);
            item.ItemId =radndomId;
            item.Price = flashSaleConfig.HardPrice[flashSaleConfig.HardItem.IndexOf(radndomId)];
            item.CanBuyCount =1;
            item.PriceIdex.Clear();
        }
    }

    public bool IsHaveFlashItem(int id)
    {
        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.FlashSale))
            return false;
        var _storeItem=GetStorageStoreItem(id);
        if (_storeItem != null && _storeItem.BuyCount<_storeItem.CanBuyCount)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 根据合成连不重复生成
    /// </summary>
    /// <param name="items"></param>
    /// <param name="widths"></param>
    /// <param name="genList">已生成的元素</param>
    /// <returns></returns>
    public int GenerateRandomItem(List<int> items, List<int> widths, List<int> genList,FlashSale flashSaleConfig)
    {
        int itemID = 0;
        if (items.Count <= 0 || widths.Count <= 0 || items.Count != widths.Count)
        {
            DebugUtil.LogError("GenerateRandomItem错误--------------------------" + items.Count + "  " + widths.Count);
            return itemID;
        }

        List<int> tempItems = new List<int>();
        tempItems.AddRange(items);
        List<int> tempWidths = new List<int>();
        tempWidths.AddRange(widths);
        //如果已生成
        if (genList != null && genList.Count > 0)
        {
            int buildCount=0;
            //移除临时列表中相同合成链的物品
            foreach (var genItem in genList)
            {
                var itemConfig = GameConfigManager.Instance.GetItemConfig(genItem);
                if (itemConfig == null)
                    continue;
                for (int i = tempItems.Count - 1; i >= 0; i--)
                {
                    var config = GameConfigManager.Instance.GetItemConfig(tempItems[i]);
                    if (config != null && config.in_line == itemConfig.in_line)
                    {
                        tempItems.RemoveAt(i);
                        tempWidths.RemoveAt(i);
                    }
                }

                if (flashSaleConfig.BuildingMergeline.Contains(itemConfig.in_line))
                    buildCount++;
            }
            //处理建筑类只出一个逻辑
            if (buildCount >= flashSaleConfig.MaxLocality)
            {
                foreach (var genItem in genList)
                {
                    var itemConfig = GameConfigManager.Instance.GetItemConfig(genItem);
                    if (itemConfig == null)
                        continue;
                    if (flashSaleConfig.BuildingMergeline.Contains(itemConfig.in_line))
                    {
                        for (int i = tempItems.Count - 1; i >= 0; i--)
                        {
                            var config = GameConfigManager.Instance.GetItemConfig(tempItems[i]);
                            if (config != null && flashSaleConfig.BuildingMergeline.Contains(config.in_line))
                            {
                                tempItems.RemoveAt(i);
                                tempWidths.RemoveAt(i);
                            }
                        }
                    }
                   
                }
            }
            
            
        }

        if (tempItems.Count > 0 && tempWidths.Count > 0 && tempItems.Count == tempWidths.Count)
        {
            itemID = tempItems[CommonUtils.RandomIndexByWeight(tempWidths)];
        }
        else
        {
            itemID = items[CommonUtils.RandomIndexByWeight(widths)];
        }

        return itemID;
    }

    public void GeneratePigShopItems()
    {
        var configs = AdConfigHandle.Instance.GetPigSale();
        if (configs == null)
        {
            Debug.LogError("pigSale刷新失败-----------");
            return;
        }

        pigSale.Items.Clear();
        
        for (int i = 0; i < configs.Count; i++)
        {
            StorageStoreItem item = new StorageStoreItem();
            item.ItemId = configs[i].Item;
            item.Price = configs[i].Price;
            item.PriceIdex.AddRange(configs[i].Increase);
            item.BuyCount = 0;
            item.CanBuyCount = configs[i].Times;
            item.Site = i;
            item.PriceType = configs[i].CostType;
            pigSale.Items.Add(item);
        }
    }

    private FlashSale GetCurFlashSaleConfig()
    {
        int group = GetStarNumGroup();
        return GetTableFlashSalesByStar(group);
    }

    private int GetStarNumGroup()
    {
        var starCount = UserData.Instance.GetTotalDecoCoin();
        int maxStarNum = 0;
        foreach (var value in flashSaleStarNums)
        {
            maxStarNum = Math.Max(value, maxStarNum);
            if (starCount > value)
                continue;

            return value;
        }

        return maxStarNum;
    }

    private FlashSale GetTableFlashSalesByStar(int starNum)
    {
        List<int> starList = new List<int>();
        starList.Add(starNum);

        foreach (var kv in flashSaleConfigs)
        {
            if (kv.Key < starNum)
                starList.Add(kv.Key);
        }

        if (flashSaleConfigs.ContainsKey(starNum))
            return flashSaleConfigs[starNum];

        return null;
    }

    public StorageStoreItem GetStorageStoreItem(int id)
    {
        if (flashSale.Items ==null  || flashSale.Items.Count <= 0)
            return null;
        foreach (var item in flashSale.Items)
        {
            if (item.ItemId == id)
                return item;
        }
        return null;
    }
    public StorageFlashSale GetFlashSaleItems()
    {
        ReMapFlashSaleShop();
        //todo 检查是否该刷新 
        if (CanRefreshDailyShop() || flashSale.ForceRefresh == 0)
        {
            flashSale.ForceRefresh = 1;
            flashSale.TotalRefreshCount += 1;
            flashSale.RefreshTime = APIManager.Instance.GetServerTime() / 1000;
            GenerateFlashShopItems(true);
        }

        return flashSale;
    }

    public StoragePigSale GetPigSaleItems()
    {
        if (CanRefreshPigShop() || pigSale.ForceRefresh == 0)
        {
            pigSale.ForceRefresh = 1;
            pigSale.TotalRefreshCount += 1;
            pigSale.RefreshTime = APIManager.Instance.GetServerTime() / 1000;
            GeneratePigShopItems();
        }

        return pigSale;
    }

    private bool CanRefreshDailyShop() // 12 和 00  点刷新 
    {
        bool result = false;
        ulong now = APIManager.Instance.GetServerTime() / 1000;
        int nowTime = (int) (now) / (24 * 3600);
        int lastTime = (int) (flashSale.RefreshTime / (24 * 3600));

        DateTime serNow = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).AddSeconds(now);
        DateTime refreshTime1 = serNow.AddDays(1);
        DateTime refreshTime2 = new DateTime(refreshTime1.Year, refreshTime1.Month, refreshTime1.Day, 0, 0, 0);
        int cp = DateTime.Compare(serNow, refreshTime2);
        if (cp > 0) // t1 大于t2
        {
            result = flashSale.RefreshCount == 0;
            if (result)
            {
                flashSale.Items.Clear();
                flashSale.RefreshCount += 1;
            }
        }
        else
        {
            result = nowTime > lastTime; //不是同一天
            if (result)
            {
                flashSale.Items.Clear();
            }
        }

        return result;
    }

    private bool CanRefreshPigShop() // 12 和 00  点刷新 
    {
        bool result = false;
        ulong now = APIManager.Instance.GetServerTime() / 1000;
        int nowTime = (int) (now) / (24 * 3600);
        int lastTime = (int) (pigSale.RefreshTime / (24 * 3600));

        DateTime serNow = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).AddSeconds(now);
        DateTime refreshTime1 = serNow.AddDays(1);
        DateTime refreshTime2 = new DateTime(refreshTime1.Year, refreshTime1.Month, refreshTime1.Day, 0, 0, 0);
        int cp = DateTime.Compare(serNow, refreshTime2);
        if (cp > 0) // t1 大于t2
        {
        }
        else
        {
            result = nowTime > lastTime; //不是同一天
            if (result)
            {
                pigSale.Items.Clear();
            }
        }

        return result;
    }

    public void RefreshFlashShop(int type) // type 0 rv 刷新  1 钻石刷新
    {
        flashSale.TotalRefreshCount += 1;
        switch (type)
        {
            case 0:
                GenerateFlashShopItems();
                flashSale.RefreshCount_Rv += 1;
                flashSale.Rv_refresh_time = APIManager.Instance.GetServerTime() / 1000;
                break;
            case 1:
                GenerateFlashShopItems();
                flashSale.RefreshCount_diamonds += 1;
                flashSale.Diamongs_refresh_time = APIManager.Instance.GetServerTime() / 1000;
                break;
            default:
                GenerateFlashShopItems();
                break;
        }
        EventDispatcher.Instance.DispatchEvent(EventEnum.FLASH_SALE_REFRESH);
        UIStoreController.Instance.RefreshFlashShop();
    }

    public void RefreshDailyShop()
    {
        UIStoreController.Instance.RefreshDailyShop();
    }

    public void BuyFlashSaleItem(int index)
    {
        flashSale.Items[index].BuyCount += 1;
        flashSale.Items[index].BuyTime = (long) APIManager.Instance.GetServerTime() / 1000;
        EventDispatcher.Instance.DispatchEvent(EventEnum.FLASH_SALE_REFRESH);
        EventDispatcher.Instance.SendEventImmediately(new EventBuyFlashSale(flashSale.Items[index]));
    }

    public void BuyFlashSaleItem(StorageStoreItem item)
    {
        item.BuyCount += 1;
        item.BuyTime = (long) APIManager.Instance.GetServerTime() / 1000;
        EventDispatcher.Instance.DispatchEvent(EventEnum.FLASH_SALE_REFRESH);
        EventDispatcher.Instance.SendEventImmediately(new EventBuyFlashSale(item));
    }

    public void BuyPigSaleItem(int index)
    {
        pigSale.Items[index].BuyCount += 1;
        pigSale.Items[index].BuyTime = (long) APIManager.Instance.GetServerTime() / 1000;
    }

    public void AddWatchRvCount(int index)
    {
        flashSale.Items[index].RvWatched += 1;
    }

    public void AddPigWatchRvCount(int index)
    {
        pigSale.Items[index].RvWatched += 1;
    }

    public void ClearWatchRvCount(int index)
    {
        flashSale.Items[index].RvWatched = 0;
    }

    public void ClearPigWatchRvCount(int index)
    {
        pigSale.Items[index].RvWatched = 0;
    }

    public int GetWatchRvCount(int index)
    {
        return flashSale.Items[index].RvWatched;
    }

    public int GetPigWatchRvCount(int index)
    {
        return pigSale.Items[index].RvWatched;
    }

    public bool CanGetRvReward(int index)
    {
        return flashSale.Items[index].RvWatched >= GetFlashSaleItemPrice(index);
    }

    public bool CanGetPigRvReward(int index)
    {
        return pigSale.Items[index].RvWatched >= GetPigSaleItemPrice(index);
    }

    public int GetFlashSaleItemPrice(int index)
    {
        return GetFlashSaleItemPrice(flashSale.Items[index]);
      
    }

    public int GetFlashSaleItemPrice(StorageStoreItem item)
    {
        if (flashSale != null && item.PriceIdex != null && item.PriceIdex.Count>0)
        {
            int priceAdd = item.BuyCount >= item.PriceIdex.Count
                ? 0
                : item.PriceIdex[item.BuyCount];
            return Mathf.CeilToInt(1f * item.Price * priceAdd / 100);
        }

        return item.Price;
    }

    public int GetPigSaleItemPrice(int index)
    {
        if (pigSale != null && pigSale.Items[index].PriceIdex != null)
        {
            int priceAdd = pigSale.Items[index].BuyCount >= pigSale.Items[index].PriceIdex.Count
                ? 0
                : pigSale.Items[index].PriceIdex[pigSale.Items[index].BuyCount];
            return Mathf.CeilToInt(1f * pigSale.Items[index].Price * priceAdd / 100);
        }

        return pigSale.Items[index].Price;
    }

    public int GetFlashSaleItemBuyCount(int index)
    {
        return flashSale.Items[index].BuyCount;
    }

    public int GetPigSaleItemBuyCount(int index)
    {
        return pigSale.Items[index].BuyCount;
    }

    public bool IsCanBuyFlashSaleItem(int index)
    {
        return flashSale.Items[index].BuyCount < flashSale.Items[index].CanBuyCount;
    }

    public bool IsCanBuyPigSaleItem(int index)
    {
        return pigSale.Items[index].BuyCount < pigSale.Items[index].CanBuyCount;
    }

    public int GetBuyFlashSaleCount(int index)
    {
        int count = flashSale.Items[index].CanBuyCount - flashSale.Items[index].BuyCount;
        if (count < 0)
            count = 0;
        return count;
    }

    public int GetBuyPigSaleCount(int index)
    {
        int count = pigSale.Items[index].CanBuyCount - pigSale.Items[index].BuyCount;
        if (count < 0)
            count = 0;
        return count;
    }

    private void ReMapFlashSaleShop() // 删除物品id之后 重新映射商店物品
    {
        if ((flashSale.Items.Count != FlashSaleCount && flashSale.Items.Count != FlashSaleCount+FarmSaleNum)&& flashSale.ForceRefresh != 0)
        {
            Debug.LogError("重新映射商店物品---->存档中物品大于" + FlashSaleCount);
            GenerateFlashShopItems(true);
            return;
        }

        for (int i = 0; i < flashSale.Items.Count; i++)
        {
            var item = flashSale.Items[i];
            //存档错误重新生成 
            if (item.CanBuyCount == 0 || item.Price == 0)
            {
                Debug.LogError("重新映射商店物品---->存档中物品被删除 Id-->" + item.ItemId);
                GenerateFlashShopItems(true);
                return;
            }

            if (GameConfigManager.Instance.GetItemConfig(item.ItemId) == null && FarmConfigManager.Instance.TableFarmPropList.Find(a => a.Id == item.ItemId) == null)
            {
                Debug.LogError("重新映射商店物品---->存档中物品被删除 Id-->" + item.ItemId);
                GenerateFlashShopItems(true);
            }
        }

        if (flashSale.Items.Count == FlashSaleCount && flashSale.Items.Count != FlashSaleCount + FarmSaleNum)
        {
            GenerateFarmSaleItem();
        }
    }
    
    /// <summary>
    /// 每日商店是否有免费物品
    /// </summary>
    /// <param name="itemID"></param>
    /// <returns></returns>
    public bool IsCanBuyFreeStoreItem(out int itemID)
    {
        itemID = 0;
        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.FlashSale))
            return false;
        int level = ExperenceModel.Instance.GetLevel();
        Common cdata = AdConfigHandle.Instance.GetCommon();
        if (cdata.RvUnlock > level)
            return false;
        bool result = false;
        var pigSaleItem = GetPigSaleItems();
        foreach (var item in pigSaleItem.Items)
        {
            if (item.BuyCount < 1 && item.PriceIdex.Count>0 && item.PriceIdex[0]==0)
            {
                if (itemID <= 0)
                    itemID = item.ItemId;
                return true;
            }
        }

        return result;
    }
}