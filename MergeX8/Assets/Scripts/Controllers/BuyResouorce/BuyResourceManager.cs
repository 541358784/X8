using System;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.AdConfigExtend;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;

public class BuyResourceManager : Singleton<BuyResourceManager>
{
    private StorageBuyResource _storageBuyResource;

    private StorageBuyResource StorageBuyResource
    {
        get
        {
            if(_storageBuyResource == null)
                _storageBuyResource = StorageManager.Instance.GetStorage<StorageHome>().BuyResource;

            return _storageBuyResource;
        }
    }

    public bool TryShowBuyResource(UserData.ResourceId resId, string source, string data, string storeSource, bool showRecharge = true, int needCount = 0)
    {
        if (showRecharge)
        {
            if (CanShowBuyResource(resId, source, data, storeSource))
                return true;
        }

        switch (resId)
        {
            case UserData.ResourceId.Coin:
            {
                // if (UIBuyCoinController.TryShow(source, data))
                //     return true;
                //
                // UIStoreController.OpenUI(storeSource);
                return true;
            }
            case UserData.ResourceId.Diamond:
            {
                if (UIBuyDiamondController.TryShow(source, data))
                    return true;
                Action callback= () =>
                {
                    UIStoreController.OpenUI(storeSource,ShowArea.gem_shop);
                };
                UIManager.Instance.OpenUI(UINameConst.UIPopupDiamondLnsufficient,callback,needCount);
                return true;
            }
            case UserData.ResourceId.Energy:
            {
                EndlessEnergyGiftBagModel.Instance.TryOpen();
                if (EndlessEnergyGiftBagModel.Instance.IsOpen)
                {
                    UIEndlessEnergyGiftBagController.Open();
                }
                else
                {
                    UIPopupBuyEnergyController.OpenUI(source);   
                }
                return true;
            }  
            case UserData.ResourceId.HappyGo_Energy:
            {
                UIPopupHappyGoBuyEnergyController.OpenUI(source);
                return true;
            }
        }

        return false;
    }
    
    private bool CanShowBuyResource(UserData.ResourceId resId, string source, string data, string storeSource = "")
    {
        return false;
        
        if (Utils.TotalSeconds() > GetPopUpTime(resId))
            InitResData(resId);

        if (StorageBuyResource.BuyResourcesId <= 0)
            return false;
        
        if(IsCloseResView(resId))
            return false;

        UIManager.Instance.OpenUI(UINameConst.UIBuyResources, resId,source, data, storeSource);
        return true;
    }

    public bool IsCloseResView(UserData.ResourceId resId)
    {
        int rsId = (int)resId;
        if (StorageBuyResource.IsCloseResView.ContainsKey(rsId))
            return StorageBuyResource.IsCloseResView[rsId];
        
        StorageBuyResource.IsCloseResView.Add(rsId, false);

        return false;
    }

    public long GetPopUpTime(UserData.ResourceId resId)
    {
        int rsId = (int)resId;
        if (StorageBuyResource.PopupResTime.ContainsKey(rsId))
            return StorageBuyResource.PopupResTime[rsId];
        
        SetPopUpData(resId,0);

        return StorageBuyResource.PopupResTime[rsId];;
    }

    public int GetResIndex(UserData.ResourceId resId)
    {
        int rsId = (int)resId;
        if (StorageBuyResource.BuyResIndex.ContainsKey(rsId))
            return StorageBuyResource.BuyResIndex[rsId];
        
        StorageBuyResource.BuyResIndex.Add(rsId, 0);

        return 0;
    }

    public void AddResIndex(UserData.ResourceId resId)
    {
        SetIndexData(resId, GetResIndex(resId)+1);
    }
    public BuyResource GetBuyResource()
    {
        if (StorageBuyResource.BuyResourcesId <= 0)
            return null;

        return AdConfigHandle.Instance.GetBuyResource(StorageBuyResource.BuyResourcesId);
    }

    public void PurchaseSuccess(TableShop config)
    {
        BuyResource buyResource = GetBuyResource();
        if (buyResource == null)
            return;

        UserData.ResourceId resId = UserData.ResourceId.None;
        int rewardNum = 0;

        if (!FindRewardByShopId(config.id, buyResource, out resId, out rewardNum))
        {
            List<BuyResource> buyResources = AdConfigExtendConfigManager.Instance.GetConfig<BuyResource>();
            foreach (var kv in buyResources)
            {
                if(FindRewardByShopId(config.id, kv, out resId, out rewardNum))
                    break;
            }
        }
        
        if(resId == UserData.ResourceId.None)
            return;
        
        var ret = new List<ResData>(); 
        ret.Add(new ResData((int)resId, rewardNum));
        
        var reasonArgs = new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.RescurcPopBuy);
        reasonArgs.data1 = config.id.ToString();
        EventDispatcher.Instance.DispatchEvent(EventEnum.PURCHASE_SUCCESS_REWARD, ret);
        EventDispatcher.Instance.DispatchEvent(EventEnum.BUYSOURCES_PURCHASE, config);
        CommonRewardManager.Instance.PopCommonReward(ret, CurrencyGroupManager.Instance.GetCurrencyUseController(), true,reasonArgs, () =>
        {
            PayRebateModel.Instance.OnPurchaseAniFinish();
            PayRebateLocalModel.Instance.OnPurchaseAniFinish();
        });

        AddResIndex(resId);
        SetResCloseData(resId, false);
    }

    private bool FindRewardByShopId(int shopId, BuyResource buyResource, out UserData.ResourceId resId, out int rewardNum)
    {
        resId = UserData.ResourceId.None;
        rewardNum = 0;
        
        if (buyResource == null)
            return false;
        
        if (FindRewardByShopId(shopId, buyResource.CoinGroup, buyResource.CoinReward, out rewardNum))
        {
            resId = UserData.ResourceId.Coin;
            return true;
        }
        
        if (FindRewardByShopId(shopId, buyResource.DiamondGroup, buyResource.DiamondRward, out rewardNum))
        {
            resId = UserData.ResourceId.Diamond;
            return true;
        }
        
        if (FindRewardByShopId(shopId, buyResource.EnergyGroup, buyResource.EnergyReward, out rewardNum))
        {
            resId = UserData.ResourceId.Energy;
            return true;
        }
        return false;
    }
    
    private bool FindRewardByShopId(int shopId, List<int> group, List<int> reward, out int rewardNum)
    {
        rewardNum = 0;
        
        if (group == null || reward == null)
            return false;

        int index = -1;
        
        for (int i = 0; i < group.Count; i++)
        {
            if(group[i] != shopId)
                continue;
            
            index = i;
            break;
        }
        
        if(index < 0)
            return false;

        index = Math.Min(index, reward.Count - 1);
        rewardNum = reward[index];

        return true;
    }
    
    public bool GetBuyResourceData(UserData.ResourceId resId, out int shopId, out int rewardNum, out string iconName)
    {
        shopId = -1;
        rewardNum = 0;
        iconName = "";

        BuyResource buyResource = GetBuyResource();
        if (buyResource == null)
            return false;

        List<int> shopIds = null;
        List<int> rewards = null;
        List<string> iconNames = null;
        switch (resId)
        {
            case UserData.ResourceId.Coin:
            {
                shopIds = buyResource.CoinGroup;
                rewards = buyResource.CoinReward;
                iconNames = buyResource.CoinIcons;
                break;
            } 
            case UserData.ResourceId.Diamond:
            {
                shopIds = buyResource.DiamondGroup;
                rewards = buyResource.DiamondRward;
                iconNames = buyResource.DiamondIcons;
                break;
            }
            case UserData.ResourceId.Energy:
            {
                shopIds = buyResource.EnergyGroup;
                rewards = buyResource.EnergyReward;
                iconNames = buyResource.EnergyIcons;
                break;
            }
        }

        if (shopIds == null || rewards == null)
            return false;

        int index = GetResIndex(resId);
        int shopIndex = index;
        int rewardIndex = index;
        int nameIndex = index;
        shopIndex = Math.Min(shopIndex, shopIds.Count - 1);
        rewardIndex = Math.Min(rewardIndex, rewards.Count - 1);
        nameIndex = Math.Min(nameIndex, iconNames.Count - 1);
        
        shopId = shopIds[shopIndex];
        rewardNum = rewards[rewardIndex];
        iconName = iconNames[nameIndex];
        return true;
    }
    
    private void InitResData(UserData.ResourceId resId)
    {
        Common common = AdConfigHandle.Instance.GetCommon();
        if (common == null)
            StorageBuyResource.BuyResourcesId = 0;
        else
            StorageBuyResource.BuyResourcesId = common.BuyResource;
        
        SetResCloseData(resId, false);
        SetPopUpData(resId, Utils.GetTomorrowTimestamp());
        SetIndexData(resId, 0);
    }

    public void SetResCloseData(UserData.ResourceId resId, bool isClose)
    {
        int rsId = (int)resId;
        if (!StorageBuyResource.IsCloseResView.ContainsKey(rsId))
            StorageBuyResource.IsCloseResView.Add(rsId, false);

        StorageBuyResource.IsCloseResView[rsId] = isClose;
    }
    
    private void SetPopUpData(UserData.ResourceId resId, long time)
    {
        int rsId = (int)resId;
        if (!StorageBuyResource.PopupResTime.ContainsKey(rsId))
            StorageBuyResource.PopupResTime.Add(rsId, time);

        StorageBuyResource.PopupResTime[rsId] = time;
    }
    private void SetIndexData(UserData.ResourceId resId, int index)
    {
        int rsId = (int)resId;
        if (!StorageBuyResource.BuyResIndex.ContainsKey(rsId))
            StorageBuyResource.BuyResIndex.Add(rsId, index);

        StorageBuyResource.BuyResIndex[rsId] = index;
    }
}