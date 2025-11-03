using System;
using DragonPlus.Config.AdConfigExtend;
using DragonPlus.Config.TMatchShop;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using Framework;
using TMatch;
using UnityEngine;

namespace TMatch
{
    
public class RemoveAdModel: GlobalSystem<RemoveAdModel>, IInitable
{
    private StorageRemoveAd _storageRemoveAd;
    
    public void Init()
    {
        _storageRemoveAd = StorageManager.Instance.GetStorage<StorageTMatch>().RemoveAd;
    }

    public void Release()
    {
                
    }

    public bool IsRemoveAd()
    {
        if (Dlugin.SDK.GetInstance().iapManager.IsInitialized())
        {
            if (!_storageRemoveAd.RemoveAd)
            {
                var iapItems = TMatchModel.Instance.IAP.GetIAPItemDatas();
                foreach (var p in iapItems)
                {
                    if (p.shopCfg.GetIAPShopType() == IAPShopType.NoAdPack)
                    {
#if UNITY_IOS
                        if (Dlugin.SDK.GetInstance().iapManager.HasOwnedProduct(p.shopCfg.product_id_ios))
                        {
                            _storageRemoveAd.RemoveAd = true;
                            Debug.Log($"restore removeAd : {p.shopCfg.id}-{p.shopCfg.product_id_ios} from iapManager.HasOwnedProduct.");
                            break;
                        }

#else
                      if (Dlugin.SDK.GetInstance().iapManager.HasOwnedProduct(p.shopCfg.product_id))
                        {
                            _storageRemoveAd.RemoveAd = true;
                            UILobbyMainViewAdsButton.UpdateAllWindowShowState();
                            Debug.Log($"restore removeAd : {p.shopCfg.id}-{p.shopCfg.product_id} from iapManager.HasOwnedProduct.");
                            break;
                        }  
#endif
                    }
                }
            }
        }
        
        return _storageRemoveAd.RemoveAd;
    }

    public void SetRemoveAd()
    {
        _storageRemoveAd.RemoveAd = true;
        UILobbyMainViewAdsButton.UpdateAllWindowShowState();
    }

    public bool IsUnlock()
    {
        var removeAdConfig = GetTmRemoveAdConfig();
        if (removeAdConfig == null)
            return false;
        var unlockLevel = removeAdConfig.UnlockLevel;
        var mainLevel = TMatchModel.Instance.GetMainLevel();
        if(unlockLevel <= mainLevel)
        {
            return true;
        }

        return false;
    }
    
    public bool CanShow()
    {
        if (_storageRemoveAd.RemoveAd) return false;
        return IsUnlock();
    }

    public bool TryToAutoOpen()
    {
        // UIViewSystem.Instance.Open<UIRemoveAdPopup>();
        // return true;
        if (CanShow())
        {
            var removeAdCfg = GetTmRemoveAdConfig();
            if (removeAdCfg == null)
                return false;
            var lastPopupTime = _storageRemoveAd.LastPopupTimestamp;
            var now = (long)APIManager.Instance.GetServerTime();
            var lastShowDays = global::CommonUtils.GetTotalDays((ulong)lastPopupTime);
            var nowDays = global::CommonUtils.GetTotalDays((ulong)now);
            var interval = (now - lastPopupTime)/1000;
            if (nowDays != lastShowDays && interval > removeAdCfg.ShowInterval)
            {
                _storageRemoveAd.PopupDailyTimes = 1;
                _storageRemoveAd.LastPopupTimestamp = now;
                var purchaseId = removeAdCfg.ShopIds != null && removeAdCfg.ShopIds.Count > 1 ? removeAdCfg.ShopIds[1] : 101;
                UIViewSystem.Instance.Open<UIRemoveAdPopup>(new UIRemoveAdPopupData
                {
                    purchaseId = purchaseId
                });
                return true;
            }
            else if (nowDays == lastShowDays)
            {
                if (interval > removeAdCfg.ShowInterval && _storageRemoveAd.PopupDailyTimes < removeAdCfg.LimitPerDay)
                {
                    _storageRemoveAd.PopupDailyTimes ++;
                    _storageRemoveAd.LastPopupTimestamp = now;
                    var purchaseId = removeAdCfg.ShopIds != null && removeAdCfg.ShopIds.Count > 1 ? removeAdCfg.ShopIds[1] : 101;
                    UIViewSystem.Instance.Open<UIRemoveAdPopup>(new UIRemoveAdPopupData
                    {
                        purchaseId = purchaseId
                    });
                    return true;
                }
            }

            return false;
        }
        return false;
    }

    /// <summary>
    /// 获取展示的广告礼包ID 
    /// </summary>
    /// <param name="idx">0为没有金币的，1为有金币的</param>
    /// <returns></returns>
    public int GetRemoveAdPackShopId(int idx)
    {
        var removeAdCfg = GetTmRemoveAdConfig();
        if (idx == 0)
        {
            return removeAdCfg.ShopIds != null && removeAdCfg.ShopIds.Count > 0 ? removeAdCfg.ShopIds[0] : 100;
        }
        return removeAdCfg.ShopIds != null && removeAdCfg.ShopIds.Count > 1 ? removeAdCfg.ShopIds[1] : 101;
    }

    public TmRemoveAd GetTmRemoveAdConfig()
    {
       var list= AdConfigExtendConfigManager.Instance.GetConfig<TmRemoveAd>();
       if (list == null || list.Count <= 0)
           return null;
       Common common = AdConfigHandle.Instance.GetCommon();
       return list.Find(a => a.GroupId == common.TmRemoveAd);
    }
}
}
