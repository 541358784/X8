
using System;
using System.Collections.Generic;
using System.Linq;
using Deco.Item;
using Deco.World;
using Decoration;
using DragonPlus;
using DragonPlus.Config.HappyGo;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Framework;
using Gameplay;
using UnityEngine;

public enum HappyGoLevelStatus
{
    Lock,
    UnLock,
    Get,
    Finish
}
public class HappyGoModel: ActivityEntityBase
{
    public override string Guid => "OPS_EVENT_TYPE_HAPPY_GO";
    public override bool CanDownLoadRes()
    {
        return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.HappyGo);
    }
    private static HappyGoModel _instance;
    public static HappyGoModel Instance => _instance ?? (_instance = new HappyGoModel());

    private StorageHappyGo _storageHappy;
    public StorageHappyGo storageHappy
    {
        get
        {
            if (_storageHappy == null)
                _storageHappy = StorageManager.Instance.GetStorage<StorageGame>().HappyGo;

            return _storageHappy;
        }
    }

    public List<HGVDLevel> HappyGoLevelList
    {
        get
        {
            return HappyGoConfigManager.Instance.HGVDLevelList;
        }
    }

    public List<HGVDFlashSale> HappyGoFlashSale
    {
        get
        {
            return HappyGoConfigManager.Instance.HGVDFlashSaleList;
        }
    }

    public List<HGVDTLPhoneReq> HamsterRequestList
    {
        get
        {
            return HappyGoConfigManager.Instance.HGVDTLPhoneReqList;
        }
    }
    public List<HGVDBoardGrid> HappyGoBoardGridList
    {
        get
        {
            return HappyGoConfigManager.Instance.HGVDBoardGridList;
        }
    }

    public List<HGVDBundle> HGVDBundleList
    {
        get
        {
            return HappyGoConfigManager.Instance.HGVDBundleList;
        }
    }

    public HGVDConfig _happyGoConfig;
    public HGVDConfig HappyGoConfig
    {
        get
        {
            if (_happyGoConfig == null)
            {
                _happyGoConfig = GetHappyConfig();
            }

            return _happyGoConfig;
        }
    }
    public HGVDConfig GetHappyConfig()
    {
        var configs=HappyGoConfigManager.Instance.GetConfig<HGVDConfig>();
        if (configs == null || configs.Count <= 0)
            return null;
        return configs[0];
    }

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
        HappyGoConfigManager.Instance.InitConfig(configJson);
        InitServerDataFinish();
    }

    protected override void InitServerDataFinish()
    {
        base.InitServerDataFinish();
        if (!string.IsNullOrEmpty(ActivityId) && storageHappy.ActivtyId != ActivityId)
        {
            ClearHappyGo();
            storageHappy.ActivtyId = ActivityId;
            storageHappy.HgEnergy = HappyGoEnergyModel.Instance.MaxEnergyNum;
        }
        if (!storageHappy.IsBuyEntendDay)
        {
            storageHappy.EndTime = (long) EndTime - HappyGoConfig.extendBuyWaitTime * 60 * 1000 -
                                   HappyGoConfig.extendBuyTime * 60 * 1000;
        }
        else
        {
            storageHappy.EndTime = (long)EndTime;
        }

        storageHappy.EndBuyTime = HappyGoConfig.extendBuyWaitTime;
        //写入配置到存档
        if (storageHappy.LvConfig == null || storageHappy.LvConfig.Count == 0)
        {
            for (int i = 0; i <HappyGoLevelList.Count; i++)
            {
                StorageHappyGoLevelConfig config = new StorageHappyGoLevelConfig();
                config.Id = HappyGoLevelList[i].id;
                config.Lv = HappyGoLevelList[i].lv;
                config.Xp = HappyGoLevelList[i].xp;
                config.Reissue = HappyGoLevelList[i].reissue;
                config.Reward.AddRange(HappyGoLevelList[i].reward);
                config.Amount.AddRange(HappyGoLevelList[i].amount);
                storageHappy.LvConfig.Add(config);
            }
                
        }
    }

    private int _happyGoTime;

    private int[] hamsterBiscuit = new[]
    {
        901001,
        901002,
        901003,
        901004,
        901005,
        901006,
        901007,
        901008,
        901009,
        901010,
    };
    
    public override bool IsOpened(bool hasLog = false)
    {
        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.HappyGo))
            return false;
        bool isOpen = base.IsOpened();
        if (!isOpen)
            return false;
        var isWait = IsWaitBuyExtendDay();
        if (GetActivityLeftTime() <= 0 && storageHappy.IsBuyEntendDay)
            return false;      
        if (GetActivityLeftTime() <= 0 && !isWait)
            return false;
        if (isWait && !IsStart())
            return false;
        if (storageHappy.IsGetReward)
            return false;
        return base.IsOpened(hasLog);
    }
    
    public bool IsStart()
    {
        if (storageHappy.StartTime > 0)
            return true;
        
        return false;
    }
    
    public bool IsCanPlay()
    {
        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.HappyGo))
            return false;
        bool isOpen = base.IsOpened();
        if (!isOpen)
            return false;
        if (storageHappy.IsGetReward)
            return false;
        if (!storageHappy.IsShowStartView)
            return false;
        if (GetActivityLeftTime() <= 0)
            return false;
        if (IsWaitBuyExtendDay())
            return false;
        return true;
    }
    
    public void InitHappyGo()
    {
        storageHappy.Exp = 0;
        storageHappy.StartTime =(long)(APIManager.Instance.GetServerTime()/1000);
        storageHappy.IsGetReward = false;
        storageHappy.IsPlayGame = false;
        storageHappy.RequestId = -1;
        storageHappy.RequestIndex = 0;
        storageHappy.RequestCount = 0;
        storageHappy.CompleteRequestIds.Clear();
        storageHappy.IsShowStartView = true;
        InitRequest();
    }

    #region 预热
    public bool IsPreheating()
    {
        ulong serverTime =APIManager.Instance.GetServerTime();
        var config = GetHappyConfig();
        if (config == null)
            return false;
        if ( serverTime-StartTime <=(ulong)config.preheatTime * 3600 * 1000)
            return true;
        
        return false;
    }
    // 活动剩余预热时间的字符串显示
    public virtual string GetActivityPreheatLeftTimeString()
    {
        return CommonUtils.FormatLongToTimeStr((long) GetActivityPreheatLeftTime());
    }

    public ulong GetActivityPreheatLeftTime()
    {
        var config = GetHappyConfig();
        if (config == null)
            return 0;
        var left =(ulong)config.preheatTime * 3600 * 1000- (APIManager.Instance.GetServerTime()-  StartTime);
        if (left < 0)
            left = 0;
        return left;
    }
    #endregion

    #region 延期购买
    
    public ulong GetActivityLeftTime()
    {
        var left = storageHappy.EndTime - (long) APIManager.Instance.GetServerTime();
        if (left < 0)
            left = 0;
        return (ulong) left;
    }
    public string GetActivityLeftTimeString()
    {
        return CommonUtils.FormatLongToTimeStr((long) GetActivityLeftTime());
    }

    public bool IsWaitBuyExtendDay()
    {
        if (storageHappy.IsBuyEntendDay)
            return false;
        
        return GetActivityExtendBuyWaitLeftTime() > 0;
    }
    public ulong GetActivityExtendBuyWaitLeftTime()
    {
        if ( storageHappy.EndTime > (long)APIManager.Instance.GetServerTime())
            return 0;
        var left = HappyGoConfig.extendBuyWaitTime* 60 * 1000- ((long) APIManager.Instance.GetServerTime()-(long) storageHappy.EndTime) ;
        if (left < 0)
            left = 0;
        return (ulong) left;
    }
    public string GetActivityExtendBuyWaitLeftTimeString()
    {
        return CommonUtils.FormatLongToTimeStr((long) GetActivityExtendBuyWaitLeftTime());
    }
    public void PurchaseSuccess(TableShop cfg)
    {
        storageHappy.IsBuyEntendDay = true;
        storageHappy.EndTime =(long) EndTime;
        
        var bundle=  HappyGoModel.Instance.GetTableHgBundle(cfg.id);
        if (bundle != null)
        {
            var ret= CommonUtils.FormatReward(bundle.bundleItemList, bundle.bundleItemCountList);
            var reasonArgs =
                new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.HgVdBuyPackage);


            CommonRewardManager.Instance.PopHappyGoReward(ret, CurrencyGroupManager.Instance.currencyController, true,
                reasonArgs, () => {  });
            foreach (var res in ret)
            {
                if (!UserData.Instance.IsResource(res.id))
                {
                    var itemConfig = GameConfigManager.Instance.GetItemConfig(res.id);
                    if (itemConfig != null)
                    {
                        GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                        {
                            MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeItemChangeReasonHgVdBuyPackage,
                            itemAId = itemConfig.id,
                            isChange = true,
                        });
                    }
                }
            }
        }

        EventDispatcher.Instance.DispatchEvent(EventEnum.HAPPYGO_EXTEND_PURCHASE_SUCCESS);
    }
    #endregion
    //
    public bool IsCanGetReward()
    {
        foreach (var level in HappyGoLevelList)
        {
            if (IsCanGet(level.lv))
                return true;
        }
        return false;
    }

    public HappyGoLevelStatus GetLevelStatus(HGVDLevel level)
    {
        if (level.lv <= storageHappy.ClaimLevel)
            return HappyGoLevelStatus.Finish;
        else
        {
            if (storageHappy.ClaimLevel + 1 == level.lv)
            {
                return HappyGoLevelStatus.Get;
            }
        }

        if (IsCanGet(level.lv))
            return HappyGoLevelStatus.UnLock;
        return HappyGoLevelStatus.Lock;
    }

 
    public bool IsCanGet(int level)
    {
        var current = HappyGoLevelList.Find(a => a.lv == level);
        if (current == null)
            current =HappyGoLevelList.LastOrDefault();
        if (storageHappy.Exp >= current.xp && storageHappy.ClaimLevel<level)
            return true;
        
        return false;
    }    
  
    public void ClearHappyGo()
    {
        storageHappy.Clear();
        storageHappy.IsPlayGame = false;
        UIManager.Instance.CloseUI(UINameConst.HappyGoMain, true);
        MergeManager.Instance.ClearMerBoard(MergeBoardEnum.HappyGo);
       
        MergeManager.Instance.Refresh(MergeBoardEnum.HappyGo);
    }

    public bool IsClaimAllReward()
    {
        if (HappyGoLevelList != null && HappyGoLevelList.Count > 0)
        {
            if ( storageHappy.ClaimLevel == HappyGoLevelList[HappyGoLevelList.Count - 1].lv)
                return true;
        }
        return false;
    }

    public void ClaimRewards(HGVDLevel levelConfig)
    {
        storageHappy.ClaimLevel = levelConfig.lv;
       var rewards=   CommonUtils.FormatReward(levelConfig.reward, levelConfig.amount);
       if (rewards != null && rewards.Count > 0)
       {
           if (Instance.IsClaimAllReward())
           {
                Instance.storageHappy.IsGetReward = true;
           }
           CommonRewardManager.Instance.PopHappyGoReward(rewards,CurrencyGroupManager.Instance.currencyController,true,
               new GameBIManager.ItemChangeReasonArgs {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.HgVdRewardsClaim}, () =>
               {
                   List<int> decoIds = new List<int>();
                   foreach (var resData in rewards)
                   {
                       if (!DecoWorld.ItemLib.ContainsKey(resData.id))
                           continue;
                       
                       decoIds.Add(resData.id);
                   }

                   if (decoIds.Count == 0)
                       return;

                   Action action = () =>
                   {
                       if (Instance.IsClaimAllReward())
                       {
                           CommonUtils.DelayedCall(5f, () =>
                           {
                               UIManager.Instance.OpenUI(UINameConst.UIPopupHappyGoEnd);
                               Instance.storageHappy.IsShowEnd = true;
                               Instance.storageHappy.IsGetReward = true;
                           });
                       }
                   };
                   SceneFsm.mInstance.ChangeState(StatusType.Transition,StatusType.BackHome,DecoOperationType.Install, decoIds,action);
                   UIManager.Instance.CloseUI(UINameConst.UIPopupHappyGoReward, true);
               }); 
           foreach (var res in rewards)
           {
               if (!UserData.Instance.IsResource(res.id))
               {
                   var itemConfig = GameConfigManager.Instance.GetItemConfig(res.id);
                   if (itemConfig != null)
                   {
                       GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                       {
                           MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeItemChangeReasonHgVdRewardsClaim,
                           itemAId = itemConfig.id,
                           isChange = true,
                       });
                   }
               }
           }
       }
       EventDispatcher.Instance.DispatchEvent(EventEnum.HAPPYGO_CLAIM_REWARD);

    }
    private void InitRequest()
    {
        HGVDTLPhoneReq request = null;
        if (!storageHappy.IsPlayGame)
        {
            request = GetFirstRequest();
            storageHappy.IsPlayGame = true;
        }
        else
            request = RandomRequest();
        
        if(request == null)
            return;
        
        storageHappy.RequestId = request.id;
    }

    public int GetRequestValue()
    {
        return GetRequestValueByIndex(storageHappy.RequestId, storageHappy.RequestIndex);
    }

    public int GetRequestProductId()
    {
        return GetHamsterBiscuitId(GetRequestValue());
    }

  
    public void AddRequestCount()
    {
        storageHappy.RequestCount++;
    }
    public void AddRequestIndex()
    {
        int length = GetRequestValueLength(storageHappy.RequestId);
        if(length < 0)
            return;

        storageHappy.RequestIndex++;
        if(storageHappy.RequestIndex < length)
            return;

        storageHappy.RequestIndex = 0;
        
        storageHappy.CompleteRequestIds.Add(storageHappy.RequestId);
        if(storageHappy.CompleteRequestIds.Count >= GetRequestDataLength())
            storageHappy.CompleteRequestIds.Clear();
        
        HGVDTLPhoneReq request = RandomRequest(storageHappy.CompleteRequestIds);
        if(request == null)
            return;

        storageHappy.RequestId = request.id;
    }
    
    public void AddExp(int exp)
    {
        storageHappy.Exp +=exp;
       var r=  new GameBIManager.ItemChangeReasonArgs()
            {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.HgVdExp};
        GameBIManager.Instance.SendItemChangeEvent(UserData.ResourceId.HappyGo, (long)exp, (ulong) storageHappy.Exp , r);
    }
    
    public int GetExp()
    {
        return storageHappy.Exp;
    }
    public int GetLevel()
    {
        int level = 0;
        foreach (var tableLevel in HappyGoLevelList)
        {
            if (storageHappy.Exp >= tableLevel.xp)
                level = tableLevel.lv;
            else
            {
                break;
            }
        }
        return level;
    }

    public HGVDLevel GetNextLevelConfig()
    {
        var leveConfig = HappyGoLevelList.Find(a => a.lv == GetLevel() + 1);
        if (leveConfig == null)
            leveConfig =HappyGoLevelList.LastOrDefault();
        return leveConfig;
    }

    public bool IsMaxExp()
    {
        var level = HappyGoLevelList.LastOrDefault();
        return storageHappy.Exp >= level.xp;
    }
    public float GetNextLevelProgress()
    {
        var nextLevel = GetNextLevelConfig();
        var currentLevel = HappyGoLevelList.Find(a => a.lv == GetLevel());
        if (nextLevel == null)
            return 0;
        if (IsMaxExp())
            return 1;
        return 1f*(storageHappy.Exp - currentLevel.xp) / (nextLevel.xp - currentLevel.xp);
    }

    public HGVDLevel GetCurrentLevel()
    {
        return HappyGoLevelList.Find(a => a.lv == GetLevel());
    }
    public HGVDLevel GetLevelConfig(int level)
    {
        return HappyGoLevelList.Find(a => a.lv == level);
    }
    public string GetProgressStr()
    {
        var nextLevel = GetNextLevelConfig();
        var currentLevel =HappyGoLevelList.Find(a => a.lv == GetLevel());
        if (IsMaxExp())
        {
            currentLevel=HappyGoLevelList.Find(a => a.lv == GetLevel()-1);
        }
        string progressStr = (storageHappy.Exp - currentLevel.xp) + "/" + (nextLevel.xp - currentLevel.xp);
        return progressStr;
    }
    
    public int GetHamsterBiscuitId(int index)
    {
        if (index < 0 || index >= hamsterBiscuit.Length)
            return -1;

        return hamsterBiscuit[index];
    }

    public TableMergeItem GetBiscuitMergeTable()
    {
        int biscuitId = GetRequestProductId();
        
        return GameConfigManager.Instance.GetItemConfig(biscuitId);
    }

    public void AddHappyGoReward(ResData resData)
    {
        var mergeItem = MergeManager.Instance.GetEmptyItem();
        mergeItem.Id = resData.id;
        mergeItem.State = 1;
        MergeManager.Instance.AddRewardItem(mergeItem,MergeBoardEnum.HappyGo, resData.count);
 
        EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.HAPPYGO_MERGE_REWARD_REFRESH);
    }

    public bool isInHappyGo()
    {
        HappyGoMainController mergeController = UIManager.Instance.GetOpenedUIByPath(UINameConst.HappyGoMain) as HappyGoMainController;
        return mergeController != null && mergeController.gameObject.activeSelf;
   
    }
    
    public HGVDTLPhoneReq GetFirstRequest()
    {
        if (HamsterRequestList == null)
            return null;

        return HamsterRequestList.Find(a => a.first_group);
    }

    public HGVDTLPhoneReq RandomRequest(List<int> filter = null)
    {
        if (HamsterRequestList == null)
            return null;

        List<HGVDTLPhoneReq> randomData = new List<HGVDTLPhoneReq>(HamsterRequestList);
        if (filter != null && filter.Count > 0)
        {
            for (int i = 0; i < randomData.Count; i++)
            {
                if (!filter.Contains(randomData[i].id))
                    continue;

                randomData.RemoveAt(i);
                i--;
            }
        }

        if (randomData.Count == 0)
            return null;

        int totalWeight = 0;
        randomData.ForEach(a => totalWeight += a.weight);

        int randomWeight = UnityEngine.Random.Range(0, totalWeight);

        int weight = 0;
        foreach (var data in randomData)
        {
            weight += data.weight;
            if (randomWeight > weight)
                continue;

            return data;
        }

        return null;
    }
    public int GetRequestValueByIndex(int id, int index)
    {
        HGVDTLPhoneReq requestData = GetRequestData(id);
        if (requestData == null || requestData.request == null || index < 0 || index >= requestData.request.Length)
            return -1;

        return requestData.request[index];
    }

    public int GetRequestValueLength(int id)
    {
        HGVDTLPhoneReq requestData = GetRequestData(id);
        if (requestData == null || requestData.request == null)
            return 0;

        return requestData.request.Length;
    }

    public int GetRequestDataLength()
    {
        if (HamsterRequestList == null)
            return 0;

        return HamsterRequestList.Count;
    }

    public HGVDTLPhoneReq GetRequestData(int id)
    {
        return HamsterRequestList.Find(a=>a.id==id);
    }

    public HGVDFlashSale GetTableHGFlashSale(int id)
    {
        return HappyGoFlashSale.Find(a => a.id == id);
    }
    public HGVDBundle GetTableHgBundle(int shopID)
    {
        if (HGVDBundleList == null)
            return null;
        return HGVDBundleList.Find(a => a.shopItemId == shopID);
    }    
    public HGVDBundle GetTableHgBundleById(int id)
    {
        if (HGVDBundleList == null)
            return null;
        return HGVDBundleList.Find(a => a.id == id);
    }
    public bool CanShowActivityEnd(StorageHappyGo data)
    {
        if (data.IsShowEnd)
            return false;
        if (!data.IsShowStartView)
            return false;
        var left = data.EndTime - (long) APIManager.Instance.GetServerTime();
        var waitTime = data.EndBuyTime* 60 * 1000- ((long) APIManager.Instance.GetServerTime()-(long) data.EndTime) ;
        bool isWait = !storageHappy.IsBuyEntendDay && waitTime>0 ;
        if (left <= 0 && data.IsBuyEntendDay)
            return true;      
        if (left <= 0 && !isWait)
            return true;
            
        return false;
    }

    public void CheckReissue(StorageHappyGo data)
    {
        if (data.IsGetReward)
            return;
        data.IsGetReward = true;
        var rewards = new List<ResData>();
        if (data.LvConfig != null && data.LvConfig.Count > 0)
        {
            for (int i = 0; i < data.LvConfig.Count; i++)
            {

                if (data.LvConfig[i].Lv > data.ClaimLevel)
                {
                    if (data.Exp >= data.LvConfig[i].Xp)
                    {
                        if(data.LvConfig[i].Reissue)
                            rewards.AddRange(CommonUtils.FormatReward(data.LvConfig[i].Reward, data.LvConfig[i].Amount));  
                    }
                }
            }
        }
        
        if (rewards != null && rewards.Count > 0)
        {
            CommonRewardManager.Instance.PopHappyGoReward(rewards,CurrencyGroupManager.Instance.currencyController,true,
                new GameBIManager.ItemChangeReasonArgs {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.ShopBuyEnergy}, () =>
                {
                    List<int> decoIds = new List<int>();
                    foreach (var resData in rewards)
                    {
                        if (!DecoWorld.ItemLib.ContainsKey(resData.id))
                            continue;
                       
                        decoIds.Add(resData.id);
                    }

                    if (decoIds.Count == 0)
                        return;
                    if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.HappyGoGame)
                    {
                        SceneFsm.mInstance.ChangeState(StatusType.Transition,StatusType.BackHome,DecoOperationType.Install, decoIds);
                    }
                    else
                    {
                        DecoManager.Instance.InstallItem(decoIds);
                    }
                    UIManager.Instance.CloseUI(UINameConst.UIPopupHappyGoReward, true);
                }); 
        }
    }
    
    private static string HappyGoPreheating = "HappyGoPreheating";
    private static string coolTimeKey = "HappyGoStart";
    private static string coolTimeKeyExtend = "HappyGoExtend";
    public static bool CanShowUI()
    {
        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.HappyGo))
            return false;
        if (Instance.CanShowActivityEnd(Instance.storageHappy))
        {
            UIManager.Instance.OpenUI(UINameConst.UIPopupHappyGoEnd);
            Instance.storageHappy.IsShowEnd = true;
            return true;
        }
        
        if (!HappyGoModel.Instance.IsOpened())
            return false;

        if (!CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, coolTimeKeyExtend))
        {
            if (HappyGoModel.Instance.IsWaitBuyExtendDay())
            {
                CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKeyExtend, CommonUtils.GetTimeStamp());
                UIManager.Instance.OpenUI(UINameConst.UIPopupHappyGoExtend);
                return true;
            }
        }

        if (!CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, HappyGoPreheating))
        {
            if (HappyGoModel.Instance.IsPreheating())
            {
                UIManager.Instance.OpenUI(UINameConst.UIPopupHappyGoStart);
                CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, HappyGoPreheating, CommonUtils.GetTimeStamp());
                return true;
            }
        }

        if (!CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, coolTimeKey))
        {
            if (!HappyGoModel.Instance.IsPreheating())
            {
                if (!HappyGoModel.Instance.IsStart())
                {
                    if (SceneFsm.mInstance.GetCurrSceneType() != StatusType.HappyGoGame)
                    {
                        UIManager.Instance.OpenUI(UINameConst.UIPopupHappyGoStart);
                        CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey, CommonUtils.GetTimeStamp());
                        return true;
                    }
                }
                else
                {
                    CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey, CommonUtils.GetTimeStamp());
                    UIManager.Instance.OpenUI(UINameConst.UIPopupHappyGoStart);
                    return true;
                }
         
            }
        }
        return false;
    }
}
