using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DragonPlus;
using DragonPlus.Config.AdConfigExtend;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;
using BiEvent = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public partial class PigBankModel : ActivityEntityBase
{
    public string GetAuxItemAssetPath()
    {
        return "Prefabs/Activity/PigBox/Aux_PigBox";
    }
    public string GetTaskItemAssetPath()
    {
        return "Prefabs/Activity/PigBox/TaskList_PigBox";
    }
    public string[] _boxImageName =
    {
        "PigBox_1",
        "PigBox_2",
        "PigBox_3",
      
    };

    public string[] _mergeBoxImageName =
    {
        "Game_Pig_BG3",
        "Game_Pig_BG4",
        "Game_Pig_BG5",
      
    };
    private static PigBankModel _instance;
    public static PigBankModel Instance => _instance ?? (_instance = new PigBankModel());

    public override string Guid => "OPS_EVENT_TYPE_PIGGY_BANK";

    private StoragePigBank _storagePigBank;
    public StoragePigBank storagePigBank
    {
        get
        {
            if (_storagePigBank == null)
                _storagePigBank = StorageManager.Instance.GetStorage<StorageHome>().PigBankData;

            return _storagePigBank;
        }
    }
    
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitAuto()
    {
        Instance.Init();
    }

    public const string LocalActivityId = "LocalPigBox";
    public const string LocalActivityStorageKey = "Activity_" + LocalActivityId;
    public override void InitFromServerData(string activityId, string activityType, ulong startTime, ulong endTime, ulong rewardEndTime, bool manualEnd, string configJson, string activitySubType)
    {
        if (activityId == LocalActivityId)
        {
            if (!storagePigBank.PigBankIds.ContainsKey(LocalActivityStorageKey))
            {
                foreach (var pair in storagePigBank.PigBankIds)
                {
                    if (!storagePigBank.IgnoreIds.Contains(pair.Key))
                        storagePigBank.IgnoreIds.Add(pair.Key);
                }   
            }
        }
        else
        {
            if (storagePigBank.PigBankIds.ContainsKey(LocalActivityStorageKey) && APIManager.Instance.GetServerTime() < storagePigBank.LocalEndTime)
            {
                if (!storagePigBank.IgnoreIds.Contains(activityId))
                    storagePigBank.IgnoreIds.Add(activityId);
            }
        }
        if (storagePigBank.IgnoreIds.Contains(activityId))
            return;
        base.InitFromServerData(activityId, activityType, startTime, endTime, rewardEndTime, manualEnd, configJson, activitySubType);
        InitServerDataFinish();
        DebugUtil.Log($"InitConfig:{Guid}");
    }

    public void InitLocal()
    {
        if (storagePigBank.PigBankIds.ContainsKey(LocalActivityStorageKey) &&
            APIManager.Instance.GetServerTime() < storagePigBank.LocalEndTime)
        {
            InitFromServerData(LocalActivityId, Guid, storagePigBank.LocalStartTime, storagePigBank.LocalEndTime,
                storagePigBank.LocalEndTime, false, "", "");
        }
    }

    public bool TryCreateLocalGame()
    {
        var localConfig = GlobalConfigManager.Instance.LocalPigBoxList[0];
        if (PayLevelModel.Instance.GetCurPayLevelConfig().LocalPigBank &&
            ExperenceModel.Instance.GetLevel() >= localConfig.unlockLevel &&
            !storagePigBank.PigBankIds.ContainsKey(LocalActivityStorageKey) && storagePigBank.LocalEndTime == 0)
        {
            var curTime = APIManager.Instance.GetServerTime();
            var endTime = curTime + (ulong)localConfig.activeTime * XUtility.Hour;
            storagePigBank.LocalStartTime = curTime;
            storagePigBank.LocalEndTime = endTime;
            InitFromServerData(LocalActivityId, Guid, curTime, endTime,
                endTime, false, "", "");
            
            return true;
        }
        return false;
    }
    
    public override void UpdateActivityState()
    {
        InitServerDataFinish();
    }
      
    protected override void InitServerDataFinish()
    {
        CleanStorage();
        
        if (GetCurActiveId() != null)
            return;

        if (ActivityId != LocalActivityId)
        {
            if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.PigBank))
                return;   
        }
        // Common common = AdConfigHandle.Instance.GetCommon();
        // if(common == null)
        //     return;
        var pigBankIdList = PayLevelModel.Instance.GetCurPayLevelConfig().PigBankIdList;
        storagePigBank.PigBankIds[StorageKey] = new StoragePigBankId();
        for(int i = 0; i < pigBankIdList.Count; i++)
            storagePigBank.PigBankIds[StorageKey].Ids.Add(pigBankIdList[i]);
    }

    private void CleanStorage()
    {
        CleanStorage(storagePigBank.PigBankIds, StorageKey);
        CleanStorage(storagePigBank.PigBankValue, StorageKey);
        CleanStorage(storagePigBank.PigBankAutoPop, StorageKey);
        CleanStorage(storagePigBank.Indexs, StorageKey);
    }

    private void CleanStorage<T>(Dictionary<string, T> storage, string activityId)
    {
        List<string> removeKey = new List<string>();
        foreach (var kv in storage)
        {
            if(kv.Key == activityId)
                continue;
            
            removeKey.Add(kv.Key);
        }
        foreach (var key in removeKey)
        {
            storage.Remove(key);
        }
    }
    
    public int GetCurIndex()
    {
        if (!storagePigBank.Indexs.ContainsKey(StorageKey))
            storagePigBank.Indexs.Add(StorageKey, 0);

        return storagePigBank.Indexs[StorageKey];
    }
    
    public void AddCurIndex()
    {
        int index = GetCurIndex() + 1;
        
        StoragePigBankId id = GetCurActiveId();
        if (id != null)
        {
            index = Math.Min(index, id.Ids.Count-1);
        }
        
        storagePigBank.Indexs[StorageKey] = index;
    }

    public int GetCurCollectValue()
    {
        if (!storagePigBank.PigBankValue.ContainsKey(StorageKey))
            storagePigBank.PigBankValue.Add(StorageKey, 0);

        return storagePigBank.PigBankValue[StorageKey];
    }

    public void InitCurCollectValue()
    {
        if (!storagePigBank.PigBankValue.ContainsKey(StorageKey))
            storagePigBank.PigBankValue.Add(StorageKey, 0);

        storagePigBank.PigBankValue[StorageKey] = 0;
    }
    
    public void AddCollectValue()
    {
        PigBank adPigBankData = GetAdPigBankTable();
        if(adPigBankData == null)
            return;
            
        GetCurCollectValue();
        storagePigBank.PigBankValue[StorageKey] += adPigBankData.Value;
        TableShop tableShop = GlobalConfigManager.Instance.GetTableShopByID(adPigBankData.ShopId);
        if (tableShop != null)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("pigIndex",""+GetCurIndex());
            dic.Add("pigPrice",""+tableShop.price);
            dic.Add("diamond",""+GetCurCollectValue());
            dic.Add("collectDiamond",""+adPigBankData.Value);
            GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventPiggyBankObtain,null,null,null,dic);
         
        }

    }
    public bool IsFull()
    {
        PigBank adPigBankData = GetAdPigBankTable();
        if(adPigBankData == null)
            return false;

        return GetCurCollectValue() >= adPigBankData.Stage_2;
    }
    
    public int GetCollectValue()
    {
        PigBank adPigBankData = GetAdPigBankTable();
        if (adPigBankData == null)
            return 0;
        
        return adPigBankData.Value;
    }
    
    public int GetCanCollectValue()
    {
        PigBank adPigBankData = GetAdPigBankTable();
        if (adPigBankData == null)
            return GetCurCollectValue();
        
        return Math.Min(GetCurCollectValue(), adPigBankData.Stage_2);
    }
    
    public PigBank GetAdPigBankTable()
    {
        int id = GetCurAdId();
        if(id <= 0)
            return null;
            
        PigBank adPigBankData = AdConfigHandle.Instance.GetPigBank(id);
        return adPigBankData;
    }
    public int GetCurAdId()
    {
        int curIndex = GetCurIndex();
        if (curIndex < 0)
            return -1;
        StoragePigBankId id = GetCurActiveId();
        if(id == null)
            return -1;

        if (id.Ids == null || id.Ids.Count == 0)
            return -1;
        
        curIndex = Math.Min(curIndex, id.Ids.Count-1);

        return id.Ids[curIndex];
    }
    
    public StoragePigBankId GetCurActiveId()
    {
        if (!storagePigBank.PigBankIds.ContainsKey(StorageKey))
            return null;

        return storagePigBank.PigBankIds[StorageKey];
    }
    
    public void PurchaseSuccess(TableShop tableShop)
    {
        if(tableShop == null)
            return;
        
        List<PigBank> configData = AdConfigExtendConfigManager.Instance.GetConfig<PigBank>();
        if (configData == null || configData.Count == 0)
            return;

        List<string> keys = storagePigBank.PigBankIds.Keys.ToList();
        for (int i = keys.Count - 1; i >= 0; i--)
        {
            StoragePigBankId pigBankId = storagePigBank.PigBankIds[keys[i]];
            if(pigBankId == null || pigBankId.Ids == null || pigBankId.Ids.Count == 0)
                continue;

            int index = !storagePigBank.Indexs.ContainsKey(keys[i]) ? 0 : storagePigBank.Indexs[keys[i]];
            index = Math.Min(index, pigBankId.Ids.Count - 1);
            
            int collectValue = !storagePigBank.PigBankValue.ContainsKey(keys[i]) ? 0 : storagePigBank.PigBankValue[keys[i]];
            int adId = pigBankId.Ids[index];
            PigBank adTable = AdConfigHandle.Instance.GetPigBank(adId);
            if(adTable == null)
                continue;
            
            if(collectValue < adTable.Stage_1)
                continue;
                
            foreach (var id in pigBankId.Ids)
            {
                PigBank data = AdConfigHandle.Instance.GetPigBank(id);
                if(data == null)
                    continue;
                
                if(data.ShopId != tableShop.id)
                    continue;
                
                var ret = new List<ResData>();
                int value = Math.Min(collectValue, adTable.Stage_2);
                 ret.Add(new ResData((int)UserData.ResourceId.Diamond, value));
            
                var reasonArgs = new GameBIManager.ItemChangeReasonArgs(BiEventCooking.Types.ItemChangeReason.GemsPiggybankBuy);
                reasonArgs.data1 = tableShop.id.ToString();
                reasonArgs.data2 = GetCurIndex().ToString();
              
                Dictionary<string, string> extras = new Dictionary<string, string>();
                extras.Add("pigIndex", PigBankModel.Instance.GetCurIndex().ToString());
                extras.Add("pigShopId", tableShop.id.ToString());
                extras.Add("diamond", adTable.Stage_2.ToString());
                extras.Add("collectDiamond", value.ToString());
                GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventPiggyBankBuySuccess, null, null, null, extras);
                
                AddCurIndex();
                InitCurCollectValue();
                storagePigBank.PigBankAutoPop[ActivityId] = false;
                EventDispatcher.Instance.DispatchEvent(EventEnum.PURCHASE_SUCCESS_REWARD, ret);
                CommonRewardManager.Instance.PopCommonReward(ret, CurrencyGroupManager.Instance.currencyController, true,reasonArgs, () =>
                {
                    EventDispatcher.Instance.DispatchEvent(EventEnum.PIGBANK_PURCHASE_REFRESH, index, tableShop);
                    PayRebateModel.Instance.OnPurchaseAniFinish();
                    PayRebateLocalModel.Instance.OnPurchaseAniFinish();
                });
            
                UIWindow uiView = UIManager.Instance.GetOpenedUIByPath(UINameConst.UIPopupPigBox);
                if (uiView != null)
                {
                    CurrencyGroupManager.Instance?.currencyController?.SetCanvasSortOrder(uiView.canvas.sortingOrder + 1);
                }
                
                return;
            }
        }
    }
    
    public bool IsCanBuy()
    {
        if (!IsOpened())
            return false;
        
        PigBank adPigBankData = GetAdPigBankTable();
        if(adPigBankData == null)
            return false;

        return GetCurCollectValue() >= adPigBankData.Stage_1;
    }
    
    public bool IsOpened()
    {
        if (ConfigurationController.Instance.version == VersionStatus.DEBUG)
        {
            if (SROptions.Current.isSetOpen)
                return SROptions.Current.isOpenPigBank;
        }

        if (storagePigBank.PigBankValue.ContainsKey(LocalActivityStorageKey) &&
            APIManager.Instance.GetServerTime() < storagePigBank.LocalEndTime)
            return true;
        
        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.PigBank))
            return false;
          
        bool isOpen = base.IsOpened();
        if (!isOpen)
            return false;

        return true;
    }

    private static string constPlaceId = "PigBankCooling";
    private int randomValue = UnityEngine.Random.Range(3, 7);
    public bool CanShow()
    {
        if (!IsOpened())
            return false;
        
        if(!IsFull())
            return false;

        if (CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, constPlaceId))
            return false;
        
        if (UIManager.Instance.GetOpenedUIByPath(UINameConst.UIPopupPigBox))
            return false;

        if (storagePigBank.PigBankAutoPop.ContainsKey(ActivityId) &&
            storagePigBank.PigBankAutoPop[ActivityId])
        {
            randomValue--;
            if (randomValue > 0)
                return false;
        }
        else
        {
            storagePigBank.PigBankAutoPop[ActivityId] = true;
        }
        
        CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, constPlaceId,
            CommonUtils.GetTimeStamp());

        GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventPiggyBankOp);
        UIManager.Instance.OpenUI(UINameConst.UIPopupPigBox);
        return true;
    }
    public override bool CanDownLoadRes()
    {
        return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.PigBank);
    }
}
