using System.Collections.Generic;
using DragonPlus.Config.TMatchShop;
using DragonU3DSDK.Account;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
// using Hospital;
// using Hospital.Game;
using UnityEngine;
using DragonU3DSDK.Network.API.Protocol;
using BiUtil = DragonPlus.GameBIManager;
namespace TMatch
{


    /// <summary>
    /// 物品自动获得模块
    /// </summary>
    public class ItemAutoClaimModel : Manager<ItemAutoClaimModel>
    {
        private StorageCurrencyTMatch data => StorageManager.Instance.GetStorage<StorageCurrencyTMatch>();
        private Dictionary<int, StorageAutoClaimItem> autoClaimItems => data.AutoClaimItems;
        private List<ItemConfig> autoClaimItemConfigs = new List<ItemConfig>();
        private float updateTime = 0f;
        private const float updateInterval = 1f;

        private bool isInit;

        public void Init()
        {
            if (isInit) return;
            EventDispatcher.Instance.AddEventListener(EventEnum.ItemChange, OnItemChange);
            foreach (var cfg in autoClaimItems)
            {
                if (cfg.Value.IsCD) continue;
                cfg.Value.IsCD = true;
            }
        
            ClientMgr.Instance.SyncEnergy();
            ClaimInitItem();
            AutoClaimItem();
        
            isInit = true;
        }
        
        public void Release()
        {
            if (!isInit) return;
            isInit = false;
            EventDispatcher.Instance.RemoveEventListener(EventEnum.ItemChange, OnItemChange);
        }

        private void OnItemChange(BaseEvent obj)
        {
            if (!(obj is ItemChangeEvent evt)) return;
            if (evt.Delta > 0) return; //这里只处理减少等情况
            bool isFind = false;
            foreach (var cfg in autoClaimItemConfigs)
            {
                if (cfg.id != evt.Id) continue;
                isFind = true;
                break;
            }

            if (!isFind) return;
            ulong serverTime = APIManager.Instance.GetServerTime();
            if (evt.Count - evt.Delta != ItemModel.Instance.GetItemMax(evt.Id)) return;
            if (!autoClaimItems.ContainsKey(evt.Id))
                autoClaimItems.Add(evt.Id, new StorageAutoClaimItem() {Id = evt.Id, IsCD = true, Timestamp = 0});
            autoClaimItems[evt.Id].Timestamp = serverTime;
        }

        // private void OnGameQuit(Hospital.EventOnGameQuit args)
        // {
        //     foreach (var cfg in autoClaimItems)
        //     {
        //         if (cfg.Value.IsCD) continue;
        //         cfg.Value.IsCD = true;
        //         cfg.Value.Timestamp = APIManager.Instance.GetServerTime();
        //     }
        //
        //     AutoClaimItem();
        // }
        //
        // private void OnGameEnd(Hospital.EventOnGameEnd args)
        // {
        //     foreach (var cfg in autoClaimItems)
        //     {
        //         if (cfg.Value.IsCD) continue;
        //         cfg.Value.IsCD = true;
        //         cfg.Value.Timestamp = APIManager.Instance.GetServerTime();
        //     }
        //
        //     AutoClaimItem();
        // }

        private void Update()
        {
            if (!isInit) return;
            if (!(Time.unscaledTime - updateTime > updateInterval)) return;
            updateTime = Time.unscaledTime;
            AutoClaimItem();
        }

        private void AutoClaimItem()
        {
            if (AccountManager.Instance.loginStatus != LoginStatus.LOGIN) return;
            ulong serverTime = APIManager.Instance.GetServerTime();
            foreach (var cfg in autoClaimItemConfigs)
            {
                if (ItemModel.Instance.IsNumMax(cfg.id))
                {
                    // if (autoClaimItems.ContainsKey(cfg.Id)) autoClaimItems.Remove(cfg.Id);
                    continue;
                }

                if (!autoClaimItems.ContainsKey(cfg.id))
                {
                    autoClaimItems.Add(cfg.id,
                        new StorageAutoClaimItem() {Id = cfg.id, IsCD = true, Timestamp = serverTime});
                }

                StorageAutoClaimItem item = autoClaimItems[cfg.id];

                // if (!item.IsCD && cfg.Id == (int) ResourceId.Energy) continue;

                ulong lastTime = item.Timestamp;
                ulong interval = (ulong) cfg.autoClaimInterval * 1000;
                if (lastTime + interval > serverTime) continue;

                while (lastTime + interval <= serverTime)
                {
                    if (ItemModel.Instance.IsNumMax(cfg.id)) break;
                    lastTime += interval;
                    ItemModel.Instance.Add(cfg.id, cfg.autoClaim, new BiUtil.ItemChangeReasonArgs()
                    {
                        reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.TimeEnergy
                    }, true);
                    // lastTime = serverTime;
                }

                item.Timestamp = lastTime;
            }
        }

        /// <summary>
        /// 获取下次领取倒计时
        /// </summary>
        /// <param name="id">物品id</param>
        /// <returns></returns>
        public long GetNextClaimCD(int id)
        {
            if (!autoClaimItems.ContainsKey(id)) return -1;

            int interval = ItemModel.Instance.GetConfigById(id).autoClaimInterval * 1000;
            ulong nextClaimTime = autoClaimItems[id].Timestamp + (ulong) interval;
            return (long) (nextClaimTime - APIManager.Instance.GetServerTime());
        }

        /// <summary>
        /// 领取初始物品
        /// </summary>
        private void ClaimInitItem()
        {
            if (data.IsInit) return;
            foreach (var config in TMatchShopConfigManager.Instance.ItemConfigList)
            {
                if (config.initialNum == 0) continue;
                ItemModel.Instance.Add(config.id, config.initialNum, new BiUtil.ItemChangeReasonArgs()
                {
                    reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.CreateProfile,
                });
            }

            data.IsInit = true;
        }

        /// <summary>
        /// 初始化配置
        /// </summary>
        public void InitCfg()
        {
            foreach (var cfg in TMatchShopConfigManager.Instance.ItemConfigList)
            {
                if (cfg.autoClaim == 0) continue;
                autoClaimItemConfigs.Add(cfg);
            }
        }
    }
}