using System;
using System.Collections.Generic;
using System.Linq;
using DragonPlus.Config.Farm;
using DragonU3DSDK.Storage;
using Farm.Model;
using SomeWhere;
using UnityEngine;

public sealed partial class StoreModel
{
    private const int FarmSaleNum = 3;
    
    public void GenerateFarmSaleItem()
    {
        var config = GetFarmSaleConfig();
        if(config == null)
            return;
        
        var normal = GenerateNormalFarmSaleItem(config);
        if(normal == null)
            return;
        
        var sp = GenerateSpFarmSaleItem(config);
        if (sp != null && sp.Count > 0)
        {
            int site = normal[normal.Count - 1].Site;
            normal[normal.Count - 1] = sp[0];
            normal[normal.Count - 1].Site = site;
        }
        
        foreach (var storageStoreItem in normal)
        {
            flashSale.Items.Add(storageStoreItem);
        }
    }

    private TableFarmSale GetFarmSaleConfig()
    {
        int level = FarmModel.Instance.GetLevel();

        for (int i = FarmConfigManager.Instance.TableFarmSaleList.Count - 1; i >= 0; i--)
        {
            if (FarmConfigManager.Instance.TableFarmSaleList[i].Level <= level)
                return FarmConfigManager.Instance.TableFarmSaleList[i];
        }

        if (FarmConfigManager.Instance.TableFarmSaleList.Last().Level < level)
            return FarmConfigManager.Instance.TableFarmSaleList.Last();
                
        return null;
    }

    private List<StorageStoreItem> GenerateNormalFarmSaleItem(TableFarmSale config)
    {
        if (config == null)
            return null;

        return GenerateFarmSaleItem(FarmSaleNum, config.Items, config.Weights, config.Prices, config.Nums, false);
    }

    
    private List<StorageStoreItem> GenerateSpFarmSaleItem(TableFarmSale config)
    {
        if (config == null)
            return null;

        int random = UnityEngine.Random.Range(1, 101);
        if(random <= config.SpProbability)
            return GenerateFarmSaleItem(1, config.SpItems, config.SpWeights, config.SpPrices, config.SpNums, true);

        return null;
    }

    public bool CanBuyFarmSale(int id)
    {
        var item = flashSale.Items.Find(a => a.ItemId == id);
        if (item == null)
            return false;

        return item.BuyCount < item.CanBuyCount;
    }
    
    private List<StorageStoreItem> GenerateFarmSaleItem(int num, List<int> items, List<int> weights, List<int> prices, List<int> nums, bool isSp)
    {
        List<int> newItems = new List<int>(items);
        List<int> newWeights = new List<int>(weights);
        List<int> newPrices = new List<int>(prices);
        List<int> newNums = new List<int>(nums);

        List<StorageStoreItem> storeItems = new List<StorageStoreItem>();

        if (isSp)
        {
            int minNum = int.MaxValue;
            foreach (var itemId in newItems)
            {
                minNum = Math.Min(minNum, FarmModel.Instance.GetProductItemNum(itemId));
            }

            for (int i = 0; i < newItems.Count; i++)
            {
                if (FarmModel.Instance.GetProductItemNum(newItems[i]) == minNum)
                {
                    newWeights[i] *= 100;
                    Debug.LogWarning($" 提升权重id={newItems[i]}, num={minNum}");
                }
            }
        }
        
        
        for (int i = 0; i < num; i++)
        {
            if (newWeights.Count == 0)
                return storeItems;
            
            int index = newWeights.RandomIndexByWeight();
            
            StorageStoreItem item = new StorageStoreItem();
            item.ItemId = newItems[index];
            item.Price = newPrices[index];
            item.BuyCount = 0;
            item.CanBuyCount = newNums[index];
            for (int j = 0; j < item.CanBuyCount; j++)
                item.PriceIdex.Add(100);
            
            item.Site = FlashSaleCount + i;
            item.PriceType = 1;
            
            storeItems.Add(item);
            
            newItems.RemoveAt(index);
            newWeights.RemoveAt(index);
            newPrices.RemoveAt(index);
            newNums.RemoveAt(index);
        }

        return storeItems;
    }
}