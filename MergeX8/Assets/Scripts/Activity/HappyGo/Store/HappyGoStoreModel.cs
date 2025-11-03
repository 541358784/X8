
using System;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.HappyGo;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;

public class HappyGoStoreModel:Manager<HappyGoStoreModel>
{
    public StorageHGFlashSale flashSale
    {
        get { return StorageManager.Instance.GetStorage<StorageGame>().HappyGo.FlashSale; }
    }
    
    public StorageHGFlashSale GetFlashSaleItems()
    {
        ReMapFlashSaleShop();

        if (CanRefreshDailyShop() || flashSale.ForceRefresh == 0)
        {
            flashSale.Items.Clear();
            flashSale.ForceRefresh = 1;
            flashSale.TotalRefreshCount += 1;
            flashSale.RefreshTime = APIManager.Instance.GetServerTime() / 1000;
            GenerateFlashShopItems();
        }
        return flashSale;
    }

    public long GetFlashRefreshLeftTime()
    {
        if (flashSale.RefreshTime == 0)
        {
            flashSale.RefreshTime =APIManager.Instance.GetServerTime() / 1000;
        }
        long cd = 21600;
        if (HappyGoModel.Instance.HappyGoConfig != null && HappyGoModel.Instance.HappyGoConfig.flashShopFreshTime>0)
            cd=HappyGoModel.Instance.HappyGoConfig.flashShopFreshTime;
        long left =( cd + (long)flashSale.RefreshTime) - (long)APIManager.Instance.GetServerTime()/1000;
        if (left < 0)
            left = 0;
        return left;
    }
    private bool CanRefreshDailyShop()
    {
        long cp =GetFlashRefreshLeftTime();
        if (cp <= 0 )
        {
            return true;
        }

        return false;
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

        HappyGoUIStoreGameController.Instance.RefreshFlashShop();
    }
    private void ReMapFlashSaleShop()// 删除物品id之后 重新映射商店物品
    {
        if (flashSale.Items == null || flashSale.Items.Count <= 0)
            return;
        for (int i = 0; i < flashSale.Items.Count; i++)
        {
            var item = flashSale.Items[i];
          
            var itemConfig = GameConfigManager.Instance.GetItemConfig(item.ItemId);
            if (itemConfig == null)
            {
                Debug.LogError("重新映射商店物品---->存档中物品被删除 Id-->"+item.ItemId);
                flashSale.Items.Clear();
                break;
            }
        }
    }

    public HGVDFlashSale GetCurFlashSaleConfig()
    {
        var starCount = UserData.Instance.GetTotalDecoCoin();
        var flashSales = HappyGoModel.Instance.HappyGoFlashSale;
        for (int i = 0; i < flashSales.Count; i++)
        {
            if (flashSales[i].starNum >= starCount)
                return flashSales[i];
        }

        return flashSales[flashSales.Count - 1];
    }
    public void GenerateFlashShopItems()
    {
        flashSale.Items.Clear();
        Common commonData = AdConfigHandle.Instance.GetCommon();
        if (commonData != null)
            flashSale.CurConfigGroup = commonData.FlashSale;
        var config = GetCurFlashSaleConfig();
        if (config == null)
        {
            Debug.LogError("闪购刷新失败-----------");
            return;
        }
      
        for (int i = 0; i < config.gemItem.Length; i++)
        {
            StorageStoreItem item = new StorageStoreItem();
            item.ItemId = config.gemItem[i];
            item.Price = config.gemPrice[i];
            item.PriceIdex.AddRange(config.gemIncrease);
            item.BuyCount = 0;
            item.CanBuyCount = config.gemTimes;
            item.Site = config.gemLocality[i];
            item.PriceType = 1;
            flashSale.Items.Add(item);
        }
    }
    
     /// <summary>
    /// 根据合成连不重复生成
    /// </summary>
    /// <param name="items"></param>
    /// <param name="widths"></param>
    /// <param name="genList">已生成的元素</param>
    /// <returns></returns>
    public int GenerateRandomItem(List<int> items, List<int> widths, List<int> genList,HGVDFlashSale flashSaleConfig)
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

                if (flashSaleConfig.buildingMergeline.Contains(itemConfig.in_line))
                    buildCount++;
            }
            //处理建筑类只出一个逻辑
            if (buildCount >= flashSaleConfig.maxLocality)
            {
                foreach (var genItem in genList)
                {
                    var itemConfig = GameConfigManager.Instance.GetItemConfig(genItem);
                    if (itemConfig == null)
                        continue;
                    if (flashSaleConfig.buildingMergeline.Contains(itemConfig.in_line))
                    {
                        for (int i = tempItems.Count - 1; i >= 0; i--)
                        {
                            var config = GameConfigManager.Instance.GetItemConfig(tempItems[i]);
                            if (config != null && flashSaleConfig.buildingMergeline.Contains(config.in_line))
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
    public int GetFlashSaleItemPrice(int index)
    {
        if (flashSale != null && flashSale.Items[index].PriceIdex != null)
        {
            int priceAdd = flashSale.Items[index].BuyCount >= flashSale.Items[index].PriceIdex.Count
                ? 0
                : flashSale.Items[index].PriceIdex[flashSale.Items[index].BuyCount];
            return Mathf.CeilToInt(1f * flashSale.Items[index].Price * priceAdd / 100);
        }

        return flashSale.Items[index].Price;
    }
    
    public int GetFlashSaleItemBuyCount(int index)
    {
        return  flashSale.Items[index].BuyCount;
    }
    
    public bool CanGetRvReward(int index)
    {
        return flashSale.Items[index].RvWatched >= GetFlashSaleItemPrice(index);
    }
    public void AddWatchRvCount(int index)
    {
        flashSale.Items[index].RvWatched += 1;
    }  
    public void ClearWatchRvCount(int index)
    {
        flashSale.Items[index].RvWatched =0;
    }      
    public int GetWatchRvCount(int index)
    {
        return flashSale.Items[index].RvWatched ;
    }   
    public void BuyFlashSaleItem(int index)
    {
        flashSale.Items[index].BuyCount += 1;
        flashSale.Items[index].BuyTime = (long) APIManager.Instance.GetServerTime() / 1000;
    }
    
    public bool IsCanBuyFlashSaleItem(int index)
    {
        return flashSale.Items[index].BuyCount < flashSale.Items[index].CanBuyCount;
    }
    public bool IsCanShowRefresh()
    {
        if (flashSale.Items != null && flashSale.Items.Count > 0)
        {
            foreach (var item in flashSale.Items)
            {
                if (item.BuyCount > 0)
                    return true;
            }
        }

        return false;
    }
    
    public int GetBuyFlashSaleCount(int index)
    {
        int count = flashSale.Items[index].CanBuyCount - flashSale.Items[index].BuyCount;
        if (count < 0)
            count = 0;
        return count;
    }

    public void ClearHappyGoStore()
    {
        flashSale.Clear();
    }

   
 


}
