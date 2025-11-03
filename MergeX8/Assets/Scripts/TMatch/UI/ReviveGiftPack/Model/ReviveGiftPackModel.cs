
using System;
using System.Collections.Generic;
using DragonPlus.Config.AdConfigExtend;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using Framework;

namespace TMatch
{
    public class ReviveGiftPackModel
{
    public StorageReviveGiftPack storage;
    private TMReviveGiftPackLevel reviveCfg;
    public ReviveGiftPackModel()
    {
        storage = StorageManager.Instance.GetStorage<StorageTMatch>().ReviveGiftPack;
    }

    public TMReviveGiftPackLevel GetReviveCfg()
    {
        InitReviveCfg();
        return reviveCfg;
    }

    public void InitReviveCfg()
    {
        if (reviveCfg == null)
        {
            Common common = AdConfigHandle.Instance.GetCommon();
            var reviveCfgs = AdConfigExtendConfigManager.Instance.GetConfig<TMReviveGiftPackLevel>();;
            reviveCfg = reviveCfgs.Find(x => x.Groupid == common.TMRevivePack);
        } 
    }

    /// <summary>
    /// 获取当前展示的礼包ID
    /// </summary>
    /// <returns></returns>
    public int GetCurrentShowGiftPackId()
    {
        var cfg = GetReviveCfg();
        // 规定等级下标是从1开始，存档为0的情况表示第一次进入
        if (storage.Level == 0)
        {
            storage.Level = cfg.FirstShow;
        }

        List<int> ids;
        if (storage.Level == 1)
        {
            ids = cfg.Gift1;
        }else if (storage.Level == 2)
        {
            ids = cfg.Gift2;
        }
        else
        {
            ids = cfg.Gift3;
        }

        var count = ids.Count;
        var randomIndex = UnityEngine.Random.Range(0, count);
        return ids[randomIndex];
    }

    public TMReviveGiftPack GetCurrentShowPack()
    {
        var id = GetCurrentShowGiftPackId();
        return AdConfigExtendConfigManager.Instance.GetConfig<TMReviveGiftPack>().Find(x => x.Id == id);
    }

    /// <summary>
    /// 是否解锁
    /// </summary>
    /// <returns></returns>
    public bool IsUnLock()
    {
        InitReviveCfg();
        return reviveCfg.OpenLevel <= TMatchModel.Instance.GetMainLevel();
    }

    public void SetShowTime()
    {
        storage.LastShowTime = (long)APIManager.Instance.GetServerTime();
    }

    /// <summary>
    /// 检测是否需要降档
    /// </summary>
    public void CheckDownBuyLevel()
    {
        var maxNotBuyTimes = 5;
        if (storage.NotBuyTimes >= maxNotBuyTimes)
        {
            if (storage.Level > 1)
            {
                storage.Level--;
            }

            storage.NotBuyTimes = 0;
        }
    }
    
    /// <summary>
    /// 检测升档
    /// </summary>
    public void CheckUpBuyLevel()
    {
        if (storage.Level < 3)
        {
            storage.Level++;
        }
    }
    
    /// <summary>
    /// 添加未购买次数
    /// </summary>
    public void AddNotBuyTimes()
    {
        storage.NotBuyTimes++;
        CheckDownBuyLevel();
    }
    
    public void BuySuccess()
    {
        storage.NotBuyTimes = 0;
        storage.LastBuyTime = (long)APIManager.Instance.GetServerTime();
        CheckUpBuyLevel();
    }

    /// <summary>
    /// 失败时是否展示
    /// </summary>
    /// <returns></returns>
    public bool CanShow()
    {
        if (!IsUnLock()) return false;
        var now = (long)APIManager.Instance.GetServerTime();
        if (storage.LastShowTime > now) return false;
        return now - storage.LastShowTime > reviveCfg.Cd * 1000;
    }
    
    #region 复活 显示礼包的逻辑处理
    public TMatchReviveSystem GetShowRevivePackType()
    {
        if (CanShow()) return TMatchReviveSystem.ReviveGiftPack;
        return TMatchReviveSystem.NoSystem;
    }
    #endregion
}
}
