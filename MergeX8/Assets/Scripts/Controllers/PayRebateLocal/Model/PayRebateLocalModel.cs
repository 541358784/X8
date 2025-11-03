using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.PayRebate;
using DragonPlus.ConfigHub.Ad;
using UnityEngine;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class PayRebateLocalModel : Manager<PayRebateLocalModel>
{

    private StoragePayRebate _storagePayRebate;

    public StoragePayRebate StoragePayRebate
    {
        get
        {
            if (_storagePayRebate == null)
            {
                _storagePayRebate= StorageManager.Instance.GetStorage<StorageHome>().PayRebateLocal;
            }
           
            return _storagePayRebate;
        }
    }
    public virtual string GetActivityLeftTimeString()
    {
      

        return CommonUtils.FormatLongToTimeStr((long)GetActivityLeftTime());
    }
    
    public ulong GetActivityLeftTime()
    {
        if (StoragePayRebate.EndTime <= 0)
        {
            StoragePayRebate.EndTime= APIManager.Instance.GetServerTime()+(ulong)GlobalConfigManager.Instance.GetNumValue("payrebate_continuousTime") * 60 * 60 * 1000;
        }
        var left = (long) StoragePayRebate.EndTime - (long) APIManager.Instance.GetServerTime();
        if (left < 0)
            left = 0;
        return (ulong) left;
    }
    public bool IsOpened()
    {
        return false;
        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.PayRebateLocal))
            return false;

        if (StoragePayRebate.IsCliam)
            return false;
        if (GetActivityLeftTime() <= 0)
            return false;
        return true;
    }

    public bool CanShowUI()
    {
        if (!IsOpened())
            return false;
        return true;
    }
    public bool IsCanClaim()
    {
        if (!StoragePayRebate.IsCliam && StoragePayRebate.IsPurchase)
            return true;
        return false;
    }   
    public void Claim()
    {
        StoragePayRebate.IsCliam = true;
        
    }
    
    public void OnPurchase()
    {
        if (!IsOpened())
            return ;
        StoragePayRebate.IsPurchase = true;
    }
    public TablePayRebateLocal GetPayRebateConfig()
    {
      
       var config=  GlobalConfigManager.Instance.tablePayRebateLocals;
       if (config != null && config.Count > 0)
           return config[0];
       return null;
    }

    public void OnPurchaseAniFinish()
    {
        if (!IsOpened())
            return ;
        if (!StoragePayRebate.IsPurchase)
            return;
        UIManager.Instance.OpenUI(UINameConst.UIPopupPayRebateLocal);
        GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventRebatePop);
    }
    
}