using System;
using System.Linq;
using DragonPlus;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Facebook.Unity;
using UnityEngine;

namespace Gameplay.UI.Store.Vip.Model
{
    public class VipStoreModel : Manager<VipStoreModel>
    {
        public StorageVipStore vipStore
        {
            get
            {
                return StorageManager.Instance.GetStorage<StorageHome>().VipStore;
            }
        }

        public void Init()
        {
            CancelInvoke("InvokeUpdate");
            InvokeRepeating("InvokeUpdate", 0, 1);
        }

        private void InvokeUpdate()
        {
            AdaptVipStore();
            
            if (VipLevel() <= 0)
                return;

            if ((long)APIManager.Instance.GetServerTime() >= vipStore.RefreshTime)
            {
                var config = GetCurrentConfig();
                vipStore.RefreshTime = (long)APIManager.Instance.GetServerTime() + config.vipRefreshTime * 1000;
                vipStore.BuyRecord.Clear();
                
                EventDispatcher.Instance.DispatchEvent(EventEnum.VipStore_RefreshTime);
            }
            
            if ((long)APIManager.Instance.GetServerTime() >= vipStore.CycleTime)
            {
                var config = GetCurrentConfig();
                if (vipStore.PurchasePrice < config.vipCyclePrice)
                {
                    if (config != GetFirstConfig())
                    {
                       var frontConfig = GetFrontConfig(vipStore.VipLevel);
                       
                       vipStore.VipLevel = frontConfig.id;
                       vipStore.CycleTime = (long)APIManager.Instance.GetServerTime() + frontConfig.vipCycleTime * 1000;
                       vipStore.PurchasePrice = 0;
                       
                       GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventVipStoreLevel,vipStore.VipLevel.ToString(), config.id.ToString());
                    }
                    else
                    {
                        vipStore.CycleTime = (long)APIManager.Instance.GetServerTime() + config.vipCycleTime * 1000;
                        vipStore.PurchasePrice = 0;
                    }
                }
                else
                {
                    vipStore.CycleTime = (long)APIManager.Instance.GetServerTime() + config.vipCycleTime * 1000;
                    vipStore.PurchasePrice = 0;
                }
                
                EventDispatcher.Instance.DispatchEvent(EventEnum.VipStore_RefreshCycleTime);
            }

            if (vipStore.VipLevel >= 3)
                vipStore.HasOpenedVipStore = true;
        }

        public bool IsOpenVipStore()
        {
            return vipStore.VipLevel >= 3 || vipStore.HasOpenedVipStore;
        }

        public int VipLevel()
        {
            return vipStore.VipLevel;
        }
        
        public void Purchase(int price)
        {
            vipStore.PurchasePrice += price;
            
            if (VipLevel() <= 0)
            {
                var storeSetting = GetFirstConfig();
                if (vipStore.PurchasePrice >= storeSetting.vipPrice)
                {
                    vipStore.UnlockTime = (long)APIManager.Instance.GetServerTime();
                    RestVipStore(storeSetting);
                    EventDispatcher.Instance.DispatchEvent(EventEnum.VipStore_PurchaseRefresh);
                    
                    GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventVipStoreLevel,vipStore.VipLevel.ToString(), "0");

                    UserData.Instance.AddRes(714, 1, new GameBIManager.ItemChangeReasonArgs(){});
                    
                    ChangeVipLevel();
                }
            }
            else
            {
                ChangeVipLevel();
            }
        }

        private void ChangeVipLevel()
        {
            var nextSetting = GetLastConfig();
            if (vipStore.VipLevel != nextSetting.id)
            {
                nextSetting = GetNextConfig(vipStore.VipLevel);
                    
                if (vipStore.PurchasePrice >= nextSetting.vipPrice)
                {
                    int oldVipLevel = vipStore.VipLevel;
                    RestVipStore(nextSetting);
                    EventDispatcher.Instance.DispatchEvent(EventEnum.VipStore_PurchaseRefresh);
                        
                    GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventVipStoreLevel,vipStore.VipLevel.ToString(), oldVipLevel.ToString());

                    ChangeVipLevel();
                }
            }
        }

        public int GetBuyCount(int vipLevel, int id)
        {
            string key = "" + vipLevel + id;
            if (!vipStore.BuyRecord.ContainsKey(key))
                return 0;

            return vipStore.BuyRecord[key];
        }

        public void SaveBuyRecord(int vipLevel, int id)
        {
            string key = "" + vipLevel + id;
            if (!vipStore.BuyRecord.ContainsKey(key))
                vipStore.BuyRecord[key] = 0;
            
            vipStore.BuyRecord[key]++;
        }

        public int GetCurrentStarNum()
        {
            return vipStore.PurchasePrice;
        }
        
        private void RestVipStore(TableVipStoreSetting config)
        {
            vipStore.VipLevel = config.id;
            vipStore.RefreshTime = (long)APIManager.Instance.GetServerTime() + config.vipRefreshTime * 1000;
            vipStore.CycleTime = (long)APIManager.Instance.GetServerTime() + config.vipCycleTime * 1000;;
            vipStore.PurchasePrice -= config.vipPrice;
            vipStore.PurchasePrice = Math.Max(vipStore.PurchasePrice, 0);
            vipStore.IsVipStore = config.vipRefreshTime > 0;
            vipStore.BuyRecord.Clear();
        }

        public string GetRefreshTime()
        {
            long time = vipStore.RefreshTime - (long)APIManager.Instance.GetServerTime();
            time = Math.Max(time, 0);
            
            return CommonUtils.FormatLongToTimeStr(time);
        }
        
        public string GetCycleTime()
        {
            long time = vipStore.CycleTime - (long)APIManager.Instance.GetServerTime();
            time = Math.Max(time, 0);
            
            return CommonUtils.FormatLongToTimeStr(time);
        }

        public long GetVipDayNum()
        {
            if (VipLevel() <= 0)
                return 0;
            
            return ((long)APIManager.Instance.GetServerTime() - vipStore.UnlockTime) / (24 * 60 * 60* 1000) + 1;
        }

        public string GetVipLevelString()
        {
            return vipStore.VipLevel.ToString();
        }
        
        public TableVipStoreSetting GetFirstConfig()
        {
            return GlobalConfigManager.Instance._vipStoreSettings.First();
        }

        public TableVipStoreSetting GetLastConfig()
        {
            return GlobalConfigManager.Instance._vipStoreSettings.Last();
        } 
        public TableVipStoreSetting GetNextConfig(int level)
        {
            int index = GlobalConfigManager.Instance._vipStoreSettings.FindIndex(a=>a.id ==level);
            if (index + 1 >= GlobalConfigManager.Instance._vipStoreSettings.Count)
                return GetLastConfig();

            return GlobalConfigManager.Instance._vipStoreSettings[index + 1];
        }
        
        public TableVipStoreSetting GetFrontConfig(int level)
        {
            int index = GlobalConfigManager.Instance._vipStoreSettings.FindIndex(a=>a.id ==level);
            if (index - 1 < 0)
                return GetFirstConfig();

            return GlobalConfigManager.Instance._vipStoreSettings[index - 1];
        }
        
        public TableVipStoreSetting GetCurrentConfig()
        {
            return  GlobalConfigManager.Instance._vipStoreSettings.Find(a=>a.id ==vipStore.VipLevel);
        }

        private void AdaptVipStore()
        {
            if(vipStore.IsAdapt)
                 return;

            if (vipStore.VipLevel > 0)
            {
                vipStore.VipLevel += 2;

                vipStore.VipLevel = Math.Clamp(vipStore.VipLevel, 0, GetLastConfig().id);
                UserData.Instance.AddRes(714, 1, new GameBIManager.ItemChangeReasonArgs(){});
            }
            else
            {
                vipStore.Clear();
            }
            vipStore.IsAdapt = true;
        }

        public int GetVipScore(float price)
        {
            return Mathf.CeilToInt(price) * 100;
        }
        
        public int GetVipScore(int shopId)
        {
            var shopConfig = GlobalConfigManager.Instance.GetTableShopByID(shopId);
            if (shopConfig == null)
                return 0;
            return GetVipScore(shopConfig.price);
        }

        public string GetVipScoreString(int shopId)
        {
            return GetVipScore(shopId).ToString();
        }
        
        public string GetVipScoreString(float price)
        {
            return GetVipScore(price).ToString();
        }
    }
}