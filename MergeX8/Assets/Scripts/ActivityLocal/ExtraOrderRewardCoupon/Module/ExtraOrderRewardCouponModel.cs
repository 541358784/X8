using System;
using System.Collections.Generic;
using System.Linq;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Asn1.X509;
using DragonPlus;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;

public class ExtraOrderRewardCouponModel : Manager<ExtraOrderRewardCouponModel>
{
    public static bool IsCouponId(int id)
    {
        if (!Instance.Config.TryGetValue(id,out var config))
        {
            return false;
        }
        return true;
    }
    public static bool IsPayCouponId(int id)
    {  
        if (!Instance.Config.TryGetValue(id,out var config))
        {
            return false;
        }
        return config.canKeep;
    }
    public static bool IsFreeCouponId(int id)
    {  
        if (!Instance.Config.TryGetValue(id,out var config))
        {
            return false;
        }
        return !config.canKeep;
    }
    public bool ShowEntrance()
    {
        return Storage.PayCouponList.Count > 0;
    }

    public string GetTaskItemAssetPath()
    {
        return "Prefabs/Activity/ExtraOrderRewardCoupon/TaskList_ExtraOrderReward";
    }

    protected override void InitImmediately()
    {
        TMatch.Timer.Register(1, UpdateTime, null, true);
        EventDispatcher.Instance.AddEvent<EventExtraOrderRewardCouponStart>(ShowStartCouponPopup);
        EventDispatcher.Instance.AddEvent<EventEnterFsmState>(AutoStartFreeCoupon);
        EventDispatcher.Instance.AddEvent<EventCompleteTask>(SendCompleteTaskBi);
    }

    public void UpdateTime()
    {
        UpdateStorage();
    }

    public void ShowStartCouponPopup(EventExtraOrderRewardCouponStart evt)
    {
        var autoPopup = new AutoPopupManager.AutoPopupManager.AutoPopUI(() =>
        {
            UIExtraOrderRewardGetController.Open(evt.StorageItem);
            return true;
        }, new[] {UINameConst.UIExtraOrderRewardGet});
        AutoPopupManager.AutoPopupManager.Instance.PushExtraPopup(autoPopup);
    }

    public void AutoStartFreeCoupon(EventEnterFsmState evt)
    {
        if (evt.State.Type == StatusType.Game)
        {
            UpdateStorage();
        }
    }

    public void SendCompleteTaskBi(EventCompleteTask evt)
    {
        if (Storage.CurCouponList.Count > 0)
        {
            var activeCoupon = Storage.CurCouponList[0];

            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventMultipleCouponFinishTask,
                data1:activeCoupon.CouponId.ToString(),
                data2:"startTime:"+XUtility.GetTimeStampStr((long)activeCoupon.StartTime/1000)+" endTime:"+XUtility.GetTimeStampStr((long)activeCoupon.EndTime/1000),
                data3:evt.TaskItem.Id.ToString());
        }
    }

    public Dictionary<int, TableExtraOrderRewardCouponConfig> Config=>GlobalConfigManager.Instance.TableExtraOrderRewardCouponConfig;

    public StorageExtraOrderRewardCoupon Storage =>
        StorageManager.Instance.GetStorage<StorageHome>().ExtraOrderRewardCoupon;

    public void UpdateStorage()
    {
        for (var i = 0; i < Storage.FreeCouponList.Count; i++)
        {
            var itemStorage = Storage.FreeCouponList[i];
            var config = Config[itemStorage.CouponId];
            var curDayId = config.GetDayId();
            if (itemStorage.DayId != curDayId)
            {
                Storage.FreeCouponList.RemoveAt(i);
                i--;
            }
        }

        for (var i = 0; i < Storage.PayCouponList.Count; i++)
        {
            var itemStorage = Storage.PayCouponList[i];
            var config = Config[itemStorage.CouponId];
            var curDayId = config.GetDayId();
            if (itemStorage.DayId != curDayId)
            {
                Storage.PayCouponList.RemoveAt(i);
                i--;
            }
        }

        if (Storage.CurCouponList.Count == 0)
        {
            if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Game)
            {
                if (Storage.FreeCouponList.Count > 0)
                {
                    var couponStorage = Storage.FreeCouponList[0];
                    Storage.FreeCouponList.RemoveAt(0);
                    StartCoupon(couponStorage);
                }
            }
        }
        else
        {
            for (var i = 0; i < Storage.CurCouponList.Count; i++)
            {
                var curCoupon = Storage.CurCouponList[i];
                if (APIManager.Instance.GetServerTime() > curCoupon.EndTime)
                {
                    var config = Config[curCoupon.CouponId];
                    Storage.CurCouponList.RemoveAt(i);
                    i--;
                    EventDispatcher.Instance.SendEventImmediately<EventExtraOrderRewardCouponEnd>(
                        new EventExtraOrderRewardCouponEnd(config));
                    if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Game)
                    {
                        if (Storage.FreeCouponList.Count > 0)
                        {
                            var couponStorage = Storage.FreeCouponList[0];
                            Storage.FreeCouponList.RemoveAt(0);
                            StartCoupon(couponStorage);
                        }
                    }
                }
            }
        }
    }

    public void UsePayCoupon()
    {
        if (Storage.PayCouponList.Count > 0)
        {
            var payCoupon = Storage.PayCouponList.Last();
            Storage.PayCouponList.RemoveAt(Storage.PayCouponList.Count - 1);
            Storage.FreeCouponList.Add(payCoupon);
            var config = Config[payCoupon.CouponId];
            EventDispatcher.Instance.SendEventImmediately<EventExtraOrderRewardCouponEnd>(
                new EventExtraOrderRewardCouponEnd(config));
            UpdateStorage();
        }
    }

    public void StartCoupon(StorageExtraOrderRewardCouponItem startCoupon)
    {
        var couponConfig = Config[startCoupon.CouponId];
        var dayLeftTime = couponConfig.GetCurDayLeftTime();
        var couponTime = (ulong) couponConfig.time * XUtility.Second;
        couponTime = Math.Min(couponTime, dayLeftTime);
        var curTime = APIManager.Instance.GetServerTime();
        startCoupon.StartTime = curTime;
        startCoupon.EndTime = curTime + couponTime;
        startCoupon.IsStart = true;
        Storage.CurCouponList.Add(startCoupon);
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventMultipleCouponUse,
            data1:"couponId:"+couponConfig.id,data2:"startTime:"+CommonUtils.FormatLongToTimeStr((long)startCoupon.StartTime)+" endTime:"+CommonUtils.FormatLongToTimeStr((long)startCoupon.EndTime));
        EventDispatcher.Instance.SendEventImmediately<EventExtraOrderRewardCouponStart>(
            new EventExtraOrderRewardCouponStart(startCoupon));
    }

    public void AddCoupon(UserData.ResourceId resourceId, int count, string reason)
    {
        var couponId = (int) resourceId;
        var couponConfig = Config[couponId];
        var dayId = couponConfig.GetDayId();
        var storageItem = new StorageExtraOrderRewardCouponItem()
        {
            DayId = dayId,
            CouponId = couponConfig.id
        };
        for (var i = 0; i < count; i++)
        {
            if (couponConfig.canKeep)
            {
                Storage.PayCouponList.Add(storageItem);
            }
            else
            {
                Storage.FreeCouponList.Add(storageItem);
            }
        }
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventMultipleCouponGet,
            data1:"couponId:"+couponConfig.id+" count:"+count,data2:reason);
        EventDispatcher.Instance.SendEventImmediately<EventExtraOrderRewardCouponGetNewCoupon>(
            new EventExtraOrderRewardCouponGetNewCoupon(couponConfig));
        UpdateStorage();
    }

    public int GetLeftCoupon(UserData.ResourceId resourceId)
    {
        var couponId = (int) resourceId;
        var leftCount = 0;
        for (var i = 0; i < Storage.FreeCouponList.Count; i++)
        {
            var itemStorage = Storage.FreeCouponList[i];
            if (itemStorage.CouponId == couponId)
            {
                leftCount++;
            }
        }

        for (var i = 0; i < Storage.PayCouponList.Count; i++)
        {
            var itemStorage = Storage.PayCouponList[i];
            if (itemStorage.CouponId == couponId)
            {
                leftCount++;
            }
        }

        return leftCount;
    }

    public float GetMultiValue(ExtraOrderRewardCouponType couponType)
    {
        if (Storage.CurCouponList.Count == 0)
            return 1f;
        var multiValue = 1f;
        for (var j = 0; j < Storage.CurCouponList.Count; j++)
        {
            var couponConfig = Config[Storage.CurCouponList[j].CouponId];
            for (var i = 0; i < couponConfig.multiType.Length; i++)
            {
                var curCouponType = (ExtraOrderRewardCouponType) couponConfig.multiType[i];
                if (curCouponType == couponType)
                {
                    multiValue += couponConfig.multiValue[i] - 1f;
                }
            }
        }

        return multiValue;
    }
}