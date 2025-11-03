using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.CatchFish;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using SomeWhere;
using UnityEngine;

public partial class CatchFishModel : ActivityEntityBase
{
    #region 基础属性
    public override string Guid => "OPS_EVENT_TYPE_PILLOW_WHEEL";

    public override bool CanDownLoadRes()
    {
        return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.CatchFish);
    }

    private static CatchFishModel _instance;
    public static CatchFishModel Instance => _instance ?? (_instance = new CatchFishModel());

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitAuto()
    {
        Instance.Init();
    }
    public override void InitFromServerData(string activityId, string activityType, ulong startTime, ulong endTime, ulong rewardEndTime,
        bool manualEnd, string configJson, string activitySubType)
    {
        base.InitFromServerData(activityId, activityType, startTime, endTime, rewardEndTime, manualEnd, configJson, activitySubType);
        CatchFishConfigManager.Instance.InitConfig(configJson);
        InitStorage();
    }
    public StorageCatchFish Storage => StorageManager.Instance.GetStorage<StorageHome>().CatchFish;
    public void InitStorage()
    {
        if (Storage.ActivityId != ActivityId)
        {
            Storage.Clear();
            Storage.ActivityId = ActivityId;
            Storage.PayLevelGroup = 0;
        }
        Storage.StartTime = (long)StartTime;
        Storage.PreheatCompleteTime = (long)StartTime + (long)((ulong)GlobalConfig.PreheatTime * XUtility.Hour);;
        Storage.EndTime = (long)EndTime;
    }
    public bool IsUnlock => UnlockManager.IsOpen(UnlockManager.MergeUnlockType.CatchFish);

    public override bool IsOpened(bool hasLog = false)
    {
        return base.IsOpened(hasLog) && IsUnlock; //当前当前周的配置;
    }
    public bool IsPrivateOpened()
    {
        return IsOpened() &&!Storage.IsTimeOut();
    }
    public bool IsStart()
    {
        return IsPrivateOpened() && Storage.GetPreheatLeftTime() == 0;
    }
    public Transform GetCommonFlyTarget()
    {
        var storage = Storage;
        if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
        {
            var entrance = MergeCatchFish.Instance;
            if (entrance)
                return entrance.transform;
            else
                return MergeMainController.Instance.rewardBtnTrans;
        }
        else
        {
            var auxItem = Aux_CatchFish.Instance;
            if (auxItem != null && auxItem.gameObject.activeInHierarchy)
                return auxItem.transform;
            else
                return UIHomeMainController.mainController.MainPlayTransform;
        }
    }
    public string GetAuxItemAssetPath()
    {
        return "Prefabs/Activity/CatchFish/Aux_CatchFish";
    }

    public string GetTaskItemAssetPath()
    {
        return "Prefabs/Activity/CatchFish/TaskList_CatchFish";
    }
    public bool ShowAuxItem()
    {
        if (!IsOpened())
            return false;
        return true;
    }
    public bool ShowTaskEntrance()
    {
        if (!IsStart())
            return false;
        return true;
    }
    #endregion

    public int GetItem()
    {
        return Storage.ItemCount;
    }

    public void AddItem(int count, GameBIManager.ItemChangeReasonArgs reason)
    {
        Storage.ItemCount += count;
        if (count > 0)
        {
            Storage.TotalItemCount += count;
        }
        GameBIManager.Instance.SendItemChangeEvent(UserData.ResourceId.CatchFish, count, (ulong)Storage.ItemCount,
            reason);
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventCatchFishRadishChange,count.ToString(),Storage.ItemCount.ToString(),reason.ToString());
        EventDispatcher.Instance.SendEventImmediately(new EventCatchFishItemChange(count));
    }
    public void PurchaseSuccess(TableShop tableShop)
    {
        if (tableShop == null)
            return;
        var shopConfig = ShopConfigList[0];
        if (tableShop.id != shopConfig.ShopId)
            return;
        var rewards = CommonUtils.FormatReward(shopConfig.RewardId, shopConfig.RewardNum);
        var itemChangeReason = new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.Iap);
        UserData.Instance.AddRes(rewards,itemChangeReason);
        CommonRewardManager.Instance.PopCommonReward(rewards, CurrencyGroupManager.Instance.currencyController, false,
            itemChangeReason,animEndCall: () =>
            {
            });
    }
    public int GetTaskValue(StorageTaskItem taskItem)
    {
        if (!IsInitFromServer())
            return 0;
        int tempPrice = 0;
        for (var i = 0; i < taskItem.RewardTypes.Count; i++)
        {
            if (taskItem.RewardTypes[i] == (int)UserData.ResourceId.Coin || taskItem.RewardTypes[i] == (int)UserData.ResourceId.RareDecoCoin)
            {
                if(taskItem.RewardNums.Count > i)
                    tempPrice = taskItem.RewardNums[i];
                
                break;
            }
        }
        if (tempPrice == 0)
        {
            foreach (var itemId in taskItem.ItemIds)
            {
                tempPrice += OrderConfigManager.Instance.GetItemPrice(itemId);
            }
        }
        var value = 0;
        var configs = TaskRewardConfigList;
        if (configs != null && configs.Count > 0)
        {
            foreach (var config in configs)
            {
                if (tempPrice <= config.Max_value)
                {
                    value = config.Output;
                    break;
                }
            }
        }
        return value;
    }
    
    public static bool CanShowPreheatPopup()
    {
        if (Instance.IsPrivateOpened() && Instance.Storage.GetPreheatLeftTime() > 0)
        {
            UIPopupCatchFishPreviewController.Open();
            return true;
        }
        return false;
    }
    public const string coolTimeKey = "CatchFish";
    public static bool CanShowPreheatPopupEachDay()
    {
        if (CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, coolTimeKey))
            return false;
        if (CanShowPreheatPopup())
        {
            CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey,CommonUtils.GetTimeStamp());
            return true;
        }
        return false;
    }
}