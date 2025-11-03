using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.PayRebate;
using DragonPlus.ConfigHub.Ad;
using UnityEngine;
using DragonU3DSDK;
using DragonU3DSDK.Storage;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class PayRebateModel : ActivityEntityBase
{
    private static PayRebateModel _instance;
    public static PayRebateModel Instance => _instance ?? (_instance = new PayRebateModel());


    private StoragePayRebate _storagePayRebate;

    public StoragePayRebate StoragePayRebate
    {
        get
        {
            var payRebate = StorageManager.Instance.GetStorage<StorageHome>().PayRebate;
            if (_storagePayRebate == null)
            {
                if (!payRebate.ContainsKey(StorageKey))
                    payRebate.Add(StorageKey, new StoragePayRebate());
                _storagePayRebate = payRebate[StorageKey];
            }
           
            return _storagePayRebate;
        }
    }

    public override string Guid => "OPS_EVENT_TYPE_PAY_REBATE";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitAuto()
    {
        Instance.Init();
    }

    public override void InitFromServerData(string activityId, string activityType, ulong startTime, ulong endTime,
        ulong rewardEndTime, bool manualEnd, string configJson, string activitySubType)
    {
        base.InitFromServerData(activityId, activityType, startTime, endTime, rewardEndTime, manualEnd, configJson,
            activitySubType);
        PayRebateConfigManager.Instance.InitConfig(configJson);
        InitServerDataFinish();
        DebugUtil.Log($"InitConfig:{Guid}");
    }

    public override void UpdateActivityState()
    {
        InitServerDataFinish();
    }

    protected override void InitServerDataFinish()
    {
        _storagePayRebate = null;
    }

    public bool IsOpened()
    {
        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.PayRebate))
            return false;
        if (PayRebateLocalModel.Instance.IsOpened())
            return false;
        bool isOpen = base.IsOpened();
        if (!isOpen)
            return false;
        if (StoragePayRebate.IsCliam)
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
    public PayRebateConfig GetPayRebateConfig()
    {
       var config= PayRebateConfigManager.Instance.GetConfig<PayRebateConfig>();
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
        UIManager.Instance.OpenUI(UINameConst.UIPopupPayRebate);
        GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventRebatePop);
    }
    
    public override bool CanDownLoadRes()
    {
        return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.PayRebate);
    }
}