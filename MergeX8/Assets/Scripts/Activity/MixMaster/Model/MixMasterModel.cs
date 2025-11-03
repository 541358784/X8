using System;
using System.Collections.Generic;
using System.Linq;
using Activity.MixMaster.View;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.MixMaster;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Dynamic;
using Gameplay;
using SomeWhere;
using UnityEngine;

public partial class MixMasterModel: ActivityEntityBase
{
    private static MixMasterModel _instance;
    public static MixMasterModel Instance => _instance ?? (_instance = new MixMasterModel());
    public override string Guid => "OPS_EVENT_TYPE_MIX_MASTER";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitAuto()
    {
        Instance.Init();
    }

    public MixMasterModel()
    {
        EventDispatcher.Instance.AddEventListener(EventEnum.BackLogin,InitEntranceAgain);
    }

    public bool IsUnlock => UnlockManager.IsOpen(UnlockManager.MergeUnlockType.MixMaster);

    public bool IsOpenPrivate()
    {
        return IsUnlock && IsOpened();
    }
    public override bool CanDownLoadRes()
    {
        return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.MixMaster);
    }

    public MixMasterGlobalConfig GlobalConfig =>
        MixMasterConfigManager.Instance.GetConfig<MixMasterGlobalConfig>()[0];
    public List<MixMasterFormulaConfig> FormulaConfig =>
        MixMasterConfigManager.Instance.GetConfig<MixMasterFormulaConfig>();
    public List<MixMasterMaterialConfig> MaterialConfig =>
        MixMasterConfigManager.Instance.GetConfig<MixMasterMaterialConfig>();
    public List<MixMasterOrderOutPutConfig> OrderOutPutConfig =>
        MixMasterConfigManager.Instance.GetConfig<MixMasterOrderOutPutConfig>();
    public List<MixMasterGiftBagConfig> GiftBagConfig =>
        MixMasterConfigManager.Instance.GetConfig<MixMasterGiftBagConfig>();
    
    public override void InitFromServerData(string activityId, string activityType, ulong startTime, ulong endTime,
        ulong rewardEndTime, bool manualEnd, string configJson, string activitySubType)
    {
        base.InitFromServerData(activityId, activityType, startTime, endTime, rewardEndTime, manualEnd, configJson,
            activitySubType);
        MixMasterConfigManager.Instance.InitConfig(configJson);
        DebugUtil.Log($"InitConfig:{Guid}");
        InitStorage();
        EventDispatcher.Instance.RemoveEvent<EventCreateMergeOrder>(OnCreateOrder);
        EventDispatcher.Instance.AddEvent<EventCreateMergeOrder>(OnCreateOrder);
    }

    public void InitEntranceAgain(BaseEvent e)
    {
        if (!IsInitFromServer())
            return;
        var turtlePang = 1;
        Debug.Log(turtlePang);
    }

    public long PreheatTimeOffset =>  IsSkipActivityPreheating()?0:(GlobalConfig.PreheatTime * (long)XUtility.Hour);
    public void InitStorage()
    {
        if (!IsInitFromServer())
            return;
        if (Storage.ActiviryId != ActivityId)
        {
            Storage.ActiviryId = ActivityId;
            Storage.Bag.Clear();
            Storage.Desktop.Clear();
            Storage.BuyTimes = 0;
            Storage.GiftBag.Clear();
            var giftBagConfig = GiftBagConfig[0];
            Storage.GiftBag.SelectItem.Add(0,giftBagConfig.Item1[0]);
            Storage.GiftBag.SelectItem.Add(1,giftBagConfig.Item2[0]);
            Storage.GiftBag.SelectItem.Add(2,giftBagConfig.Item3[0]);
            Storage.FirstMixCount = 0;
            Storage.GiftBagPopupDayId = -1;
            Storage.GiftBag.ActivityId = ActivityId;
            Storage.AlreadyCollectLevels.Clear();
            Storage.CanCollectLevels.Clear();
            Storage.OrderOutPutPool.Clear();
            Storage.OrderOutPutState.Clear();
            Storage.OrderOutPutCountState.Clear();
            if (Storage.FormulaVersion != GlobalConfig.FormulaVersion)
            {
                //切换配方版本全清记录
                Storage.FormulaVersion = GlobalConfig.FormulaVersion;
                Storage.History.Clear();
            }
            else
            {
                //不切换配方版本清调制次数
                var keyList = Storage.History.Keys.ToList();
                foreach (var key in keyList)
                {
                    Storage.History[key] = 0;
                }
            }

            foreach (var formula in FormulaConfig)
            {
                if (!formula.IsHide)
                    Storage.History.TryAdd(formula.Id, 0);
            }
        }
        Storage.StartTime = (long) StartTime;
        Storage.EndTime = (long) EndTime;
        Storage.PreheatTime = (long) StartTime + PreheatTimeOffset;
    }

    public StorageMixMaster Storage => StorageManager.Instance.GetStorage<StorageHome>().MixMaster;
    
    private static string CanShowUICoolTimeKey = "MixMaster_CanShowUI";
    public bool CanShowUI()
    {
        if (!IsOpenPrivate())
            return false;
        if (Storage.GetPreheatTime() > 0)
            return false;
        if (!CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, CanShowUICoolTimeKey))
        {
            CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, CanShowUICoolTimeKey, CommonUtils.GetTimeStamp());
            UIMixMasterMainController.Open(Storage);
            return true;
        }
        return false;
    }
    private static string CanShowPreheatUICoolTimeKey = "MixMaster_CanShowPreheatUI";
    public bool CanShowPreheatUI()
    {
        if (!IsOpenPrivate())
            return false;
        if (Storage.GetPreheatTime() <= 0)
            return false;
        if (!CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, CanShowPreheatUICoolTimeKey))
        {
            CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, CanShowPreheatUICoolTimeKey, CommonUtils.GetTimeStamp());
            UIPopupMixMasterPreviewController.Open(Storage);
            return true;
        }
        return false;
    }
    public void AddMaterial(int materialId, int count,bool fromUserdata = false)
    {
        Storage.Bag.TryAdd(materialId,0);
        Storage.Bag[materialId] += count;
        if (fromUserdata)
        {
            EventDispatcher.Instance.SendEventImmediately(new EventMixMasterUpdateRedPoint());
        }
    }

    public int GetMaterialCount(int materialId)
    {
        return Storage.Bag.TryGetValue(materialId,out var count)?count:0;
    }
    public void ReturnMaterial(int position)
    {
        if (Storage.Desktop.ContainsKey(position))
        {
            AddMaterial(Storage.Desktop[position].Id, Storage.Desktop[position].Count);
            Storage.Desktop.Remove(position);
        }
    }
    public void PutMaterial(int position,int materialId)
    {
        ReturnMaterial(position);
        var materialConfig = MaterialConfig.Find(a => a.Id == materialId);
        if (materialConfig != null)
        {
            if (materialConfig.PoolIndex.Contains(position))
            {
                var leftCount = GetMaterialCount(materialConfig.Id);
                if (leftCount >= materialConfig.Count)
                {
                    Storage.Desktop.Add(position,new StorageResData()
                    {
                        Id = materialConfig.Id,
                        Count = materialConfig.Count,
                    });
                    AddMaterial(materialConfig.Id, -materialConfig.Count);   
                }   
            }
        }
    }

    public void Mix()
    {
        if (Storage.Desktop.Count != GlobalConfig.MaterialNeedCount)
            return;
        var deskState = new Dictionary<int, int>();
        foreach (var desktop in Storage.Desktop)
        {
            deskState.TryAdd(desktop.Value.Id,0);
            deskState[desktop.Value.Id] += desktop.Value.Count;
        }

        var biData3 = "";
        foreach (var material in Storage.Desktop)
        {
            biData3 += material.Value.Id + "*" + material.Value.Count+",";
        }
        Storage.Desktop.Clear();
        MixMasterFormulaConfig pairFormula = null;
        foreach (var formula in FormulaConfig)
        {
            if (CheckFormula(formula, deskState))
            {
                pairFormula = formula;
                break;
            }
        }
        var reason =
            new GameBIManager.ItemChangeReasonArgs(reason: BiEventAdventureIslandMerge.Types.ItemChangeReason
                .MixMasterGet);
        if (pairFormula == null)
        {
            var failRewards = CommonUtils.FormatReward(GlobalConfig.FailRewardId, GlobalConfig.FailRewardNum);
            if (failRewards.Count > 0)
            {
                UserData.Instance.AddRes(failRewards,reason);
            }
            UIMixMasterMakeFailedController.Open(() =>
            {
                if (failRewards.Count > 0)
                {
                    CommonRewardManager.Instance.PopCommonReward(failRewards, CurrencyGroupManager.Instance.currencyController,
                        false, reason);  
                }
            });
            //合成失败
            EventDispatcher.Instance.SendEventImmediately(new EventMixMasterUpdateRedPoint());
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventMixMasterMaking,data1:"fail",data2:"",data3:biData3);
            return;
        }
        AudioManager.Instance.PlaySoundById(167);
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventMixMasterMaking,data1:"success",data2:pairFormula.Id.ToString(),data3:biData3);
        var unlock = false;
        if (!Storage.History.ContainsKey(pairFormula.Id))
        {
            Storage.History.Add(pairFormula.Id,0);
            EventDispatcher.Instance.SendEventImmediately(new EventMixMasterUnlockFormula(pairFormula));
            unlock = true;
        }
        Storage.History[pairFormula.Id]++;
        var isFirstMix = Storage.History[pairFormula.Id] == 1;
        var rewards = isFirstMix
            ? CommonUtils.FormatReward(pairFormula.FirstRewardId, pairFormula.FirstRewardNum)
            : CommonUtils.FormatReward(pairFormula.RepeatRewardId, pairFormula.RepeatRewardNum);
        if ( isFirstMix)
        {
            Storage.FirstMixCount++;
        }
        if (rewards.Count > 0)
        {
            UserData.Instance.AddRes(rewards,reason);
        }
        UIMixMasterMakeSuccessController.Open(pairFormula,isFirstMix,() =>
        {
            if (rewards.Count > 0)
            {
                CommonRewardManager.Instance.PopCommonReward(rewards, CurrencyGroupManager.Instance.currencyController,
                    false, reason);   
            }
        });
        if ( isFirstMix)
        {
            CheckMixTaskCompleted();
        }
        EventDispatcher.Instance.SendEventImmediately(new EventMixMasterUpdateRedPoint());
    }

    public bool CheckFormula(MixMasterFormulaConfig formula,Dictionary<int, int> materials)
    {
        var tempMaterials = new Dictionary<int, int>(materials);
        for (var i = 0; i < formula.MaterialId.Count; i++)
        {
            var materialId = formula.MaterialId[i];
            var needCount = formula.MaterialNum[i];
            if (tempMaterials.TryGetValue(materialId, out var materialCount) && materialCount >= needCount)
            {
                tempMaterials[materialId] -= needCount;
            }
            else
                return false;
        }
        return true;
    }

    public void UnlockFormula(int formulaId)
    {
        var formulaConfig = FormulaConfig.Find(a => a.Id == formulaId);
        if (formulaConfig == null)
            return;
        if (Storage.History.ContainsKey(formulaId))
        {
            return;
        }
        Storage.History.Add(formulaId,0);
        EventDispatcher.Instance.SendEventImmediately(new EventMixMasterUnlockFormula(formulaConfig));
        UIMixMasterUnlockController.Open(formulaConfig);
        EventDispatcher.Instance.SendEventImmediately(new EventMixMasterUpdateRedPoint());
    }
    public Transform GetCommonFlyTarget()
    {
        if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
        {
            var entrance = DynamicEntryManager.Instance.GetDynamicEntry<DynamicEntry_Game_MixMaster>();
            if (entrance)
                return entrance.transform;
            else
                return MergeMainController.Instance.rewardBtnTrans;
        }
        else
        {
            var auxItem = DynamicEntryManager.Instance.GetDynamicEntry<DynamicEntry_Home_MixMaster>();
            if (auxItem != null && auxItem.gameObject.activeInHierarchy)
                return auxItem.transform;
            else
                return UIHomeMainController.mainController.MainPlayTransform;
        }
    }

    public void CreateOrderReward(StorageTaskItem order)
    {
        if (!IsOpenPrivate())
            return;
        if (Storage.GetPreheatTime() > 0)
            return;
        if (Storage.OrderOutPutState.ContainsKey(order.Id))
            return;
        var config = OrderOutPutConfig.Find(a => a.OrderGroup.Contains(order.Slot));
        if (config == null)
            return;
        // Storage.OrderOutPutPool.TryAdd(config.Id, 0);
        // var material = config.MaterialList[Storage.OrderOutPutPool[config.Id]];
        // Storage.OrderOutPutPool[config.Id]++;
        // if (Storage.OrderOutPutPool[config.Id] >= config.MaterialList.Count)
        //     Storage.OrderOutPutPool[config.Id] = 0;
        var highMaterialList = new List<int>();
        var highMaterialCountList = new List<int>();
        var highWeightList = new List<int>();
        var lowMaterialList = new List<int>();
        var lowMaterialCountList = new List<int>();
        var lowWeightList = new List<int>();
        for (var i = 0; i < config.MaterialList.Count; i++)
        {
            var count = config.MaterialCount[i];
            var material = config.MaterialList[i];
            var curCount = GetMaterialAllCount(material);
            var maxCount = config.MaxCountList[i];
            var weight = config.WeightList[i];
            if (curCount >= maxCount)
            {
                lowMaterialList.Add(material);
                lowMaterialCountList.Add(count);
                lowWeightList.Add(weight);
            }
            else
            {
                highMaterialList.Add(material);
                highMaterialCountList.Add(count);
                highWeightList.Add(weight);
            }
        }
        var totalMaterial = 0;
        var totalCount = 0;
        if (highMaterialList.Count > 0)
        {
            var randomIndex = highWeightList.RandomIndexByWeight();
            totalMaterial = highMaterialList[randomIndex];
            totalCount = highMaterialCountList[randomIndex];
        }
        else
        {
            var randomIndex = lowWeightList.RandomIndexByWeight();
            totalMaterial = lowMaterialList[randomIndex];
            totalCount = lowMaterialCountList[randomIndex];
        }
        Storage.OrderOutPutState.Add(order.Id,totalMaterial);
        Storage.OrderOutPutCountState.Add(order.Id,totalCount);
        Debug.LogError("高权重池"+highMaterialList.ToLogString());
        Debug.LogError("低权重池"+lowMaterialList.ToLogString());
    }

    public int GetMaterialAllCount(int materialId)
    {
        var totalCount = GetMaterialCount(materialId);
        foreach (var pair in Storage.OrderOutPutState)
        {
            if (pair.Value == materialId)
            {
                totalCount += Storage.OrderOutPutCountState[pair.Key];
            }
        }
        return totalCount;
    }
    public void CollectOrderReward(StorageTaskItem order, ref List<ResData> resDatas)
    {
        if (!IsOpenPrivate())
            return;
        if (Storage.GetPreheatTime() > 0)
            return;
        if (!Storage.OrderOutPutState.ContainsKey(order.Id))
            return;
        if (!Storage.OrderOutPutCountState.ContainsKey(order.Id))
            return;
        resDatas.Add(new ResData(Storage.OrderOutPutState[order.Id],Storage.OrderOutPutCountState[order.Id]));
        var rewards = new List<ResData>();
        rewards.Add(new ResData(Storage.OrderOutPutState[order.Id],Storage.OrderOutPutCountState[order.Id]));
        Storage.OrderOutPutState.Remove(order.Id);
        Storage.OrderOutPutCountState.Remove(order.Id);
        var reason = new GameBIManager.ItemChangeReasonArgs()
        {
            reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.MainTaskReward
        };
        UserData.Instance.AddRes(rewards,reason);
        
    }

    public ResData GetOrderReward(StorageTaskItem order)
    {
        if (!IsOpenPrivate())
            return null;
        if (Storage.GetPreheatTime() > 0)
            return null;
        if (!Storage.OrderOutPutState.ContainsKey(order.Id))
            return null;
        if (!Storage.OrderOutPutCountState.ContainsKey(order.Id))
            return null;
        return new ResData(Storage.OrderOutPutState[order.Id], Storage.OrderOutPutCountState[order.Id]);
    }

    public void OnCreateOrder(EventCreateMergeOrder evt)
    {
        CreateOrderReward(evt.OrderItem);
    }

    public bool ShowAuxItem()
    {
        if (Storage == null)
            return false;

        return MixMasterUtils.ShowAuxItem(Storage);
    }
    public bool ShowTaskEntrance()
    {
        if (Storage == null)
            return false;

        return Storage.ShowTaskEntrance();
    }
    
}