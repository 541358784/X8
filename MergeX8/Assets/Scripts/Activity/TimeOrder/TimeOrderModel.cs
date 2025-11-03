using System.Collections.Generic;
using System.Linq;
using DragonPlus;
using DragonPlus.Config.TimeOrder;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Framework;
using Gameplay;
using Merge.Order;
using UnityEngine;

namespace Activity.TimeOrder
{
    public class TimeOrderModel : ActivityEntityBase
    {
        private static TimeOrderModel _instance;
        public static TimeOrderModel Instance => _instance ?? (_instance = new TimeOrderModel());

        public override string Guid => "OPS_EVENT_TYPE_TIME_ORDER";

        public StorageTimeOrder TimeOrder
        {
            get { return StorageManager.Instance.GetStorage<StorageHome>().TimeOrder; }
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
            TimeOrderConfigManager.Instance.InitConfig(configJson);
            InitServerDataFinish();
        }

        public override void UpdateActivityState()
        {
            InitServerDataFinish();
        }

        protected override void InitServerDataFinish()
        {
            if (ActivityId.IsEmptyString())
                return;

            if (TimeOrder.ActivityId == ActivityId)
            {
                TimeOrder.StartActivityTime = (long)StartTime;
                TimeOrder.ActivityEndTime = (long)EndTime;
                return;
            }

            CheckJoinEnd(true);

            TimeOrder.Clear();
            TimeOrder.ActivityId = ActivityId;
            TimeOrder.StartActivityTime = (long)StartTime;
            TimeOrder.ActivityEndTime = (long)EndTime;
            TimeOrder.PayLevelGroup = PayLevelModel.Instance.GetCurPayLevelConfig().TimeOrder;
        }

        public bool IsJoin()
        {
            return TimeOrder.IsJoin;
        }

        public virtual string GetJoinEndTimeString()
        {
            if (!IsOpened())
                return "";

            if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.TimeOrder))
                return "";

            if (!IsJoin())
                return "";

            var left = (long)TimeOrder.JoinEndTime - (long)APIManager.Instance.GetServerTime();
            if (left < 0)
                left = 0;

            return CommonUtils.FormatLongToTimeStr(left);
        }

        public bool IsTimeEnd()
        {
            if (TimeOrder.IsJoin)
            {
                return (long)APIManager.Instance.GetServerTime() > TimeOrder.JoinEndTime;
            }
            else
            {
                return (long)APIManager.Instance.GetServerTime() > TimeOrder.ActivityEndTime;
            }
        }

        public void CheckJoinEnd(bool isForce = false)
        {
            if (TimeOrder.ActivityId.IsEmptyString())
                return;

            if (!TimeOrder.IsJoin)
                return;

            if (TimeOrder.OrderId == 0)
                return;

            if (!isForce && (long)APIManager.Instance.GetServerTime() < TimeOrder.JoinEndTime)
                return;

            MainOrderManager.Instance.RemoveOrder(TimeOrder.OrderId);

            TimeOrder.OrderId = 0;
            TimeOrder.IsShowGift = false;
        }

        public override bool CanDownLoadRes()
        {
            return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.TimeOrder);
        }

        public void CompleteAllOrder()
        {
            OrderId = 0;
            TimeOrder.JoinEndTime = 0;
        }

        public int OrderId
        {
            get { return TimeOrder.OrderId; }
            set { TimeOrder.OrderId = value; }
        }

        public void PurchaseSuccess(int shopId, int configId)
        {
            TableTimeOrderGift config = null;
            if (configId < 0)
            {
                config = TimeOrderConfigManager.Instance.TableTimeOrderGiftList.Find(a => a.Id == configId);
            }
            else
            {
                config = TimeOrderConfigManager.Instance.TableTimeOrderGiftList.Find(a => a.ShopId == shopId);
            }

            if (config == null)
                config = TimeOrderConfigManager.Instance.TableTimeOrderGiftList.First();

            TimeOrder.IsBuyGift = true;
            UIManager.Instance.CloseUI(UINameConst.UIPopupTimeOrderGift);

            var rewards = CommonUtils.FormatReward(config.RewardId, config.RewardCount);
            foreach (var id in TimeOrder.OrderGiftContent)
            {
                rewards.Add(new ResData(id, 1));
            }
            
            for (int i = 0; i < rewards.Count; i++)
            {
                var reward = rewards[i];
                if (!UserData.Instance.IsResource(reward.id))
                {
                    GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                    {
                        MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonTimeOrderGiftGet,
                        itemAId = reward.id,
                        isChange = true,
                    });
                }

                UserData.Instance.AddRes(reward.id, reward.count, new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.TimeOrderGiftGet), true);
            }

            CommonRewardManager.Instance.PopCommonReward(rewards, CurrencyGroupManager.Instance.currencyController,
                false, new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.TimeOrderGiftGet) { }, () => { });
        }

        public bool CanShowJoin()
        {
            if (!IsOpened())
                return false;

            if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.TimeOrder))
                return false;

            if (IsTimeEnd())
                return false;

            if (TimeOrder.IsJoin)
                return false;

            long openTime = TimeOrderConfigManager.Instance.TableTimeOrderSettingList[0].OpenTime * 60 * 1000;
            if ((long)EndTime - (long)APIManager.Instance.GetServerTime() <= openTime)
                return false;

            foreach (var config in TimeOrderConfigManager.Instance.TableTimeOrderConfigList)
            {
                MainOrderManager.Instance.RemoveOrderByComplete(config.Id);
            }

            UIManager.Instance.OpenUI(UINameConst.UIPopupLimitedTimeTask);

            var timeOrderConfigs = TimeOrderConfigManager.Instance.GetTimeOrderConfigsByPayLevel();

            var level = ExperenceModel.Instance.GetLevel();
            TableTimeOrderConfig adaptConfig = null;
            foreach (var timeOrderConfig in timeOrderConfigs)
            {
                if (level <= timeOrderConfig.Level)
                {
                    adaptConfig = timeOrderConfig;
                    break;
                }
            }

            if (adaptConfig == null)
                adaptConfig = timeOrderConfigs.Last();

            var timeOrderGiftConfigs = TimeOrderConfigManager.Instance.GetTimeOrderGiftConfigsByPayLevel();


            TimeOrder.OrderId = adaptConfig.Id;
            TimeOrder.GiftId = timeOrderGiftConfigs.First().Id;
            MainOrderCreateTime.TryCreateOrder(adaptConfig);

            TimeOrder.IsJoin = true;
            TimeOrder.JoinStartTime = (long)APIManager.Instance.GetServerTime();
            TimeOrder.JoinEndTime = (long)APIManager.Instance.GetServerTime() + TimeOrderConfigManager.Instance.TableTimeOrderSettingList[0].OpenTime * 60 * 1000;
            return true;
        }

        private const string coolTimeKey = "ctk_timeordergift";

        public bool CanShowGift()
        {
            return CanShowGift(-1);
        }

        public bool CanShowGift(int index)
        {
            if (!IsOpened())
                return false;

            if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.TimeOrder))
                return false;

            if (IsTimeEnd())
                return false;

            if (!TimeOrder.IsJoin)
                return false;

            if (TimeOrder.IsBuyGift)
                return false;
            
            if (index < 0)
            {
                if (CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.LossTime, coolTimeKey))
                    return false;

                if (IsOrderFinish())
                    return false;

                if (TimeOrder.OrderGiftContent.Count == 0)
                    return false;

                CoroutineManager.Instance.StartCoroutine(CommonUtils.DelayWork(0.5f, () =>
                {
                    UIManager.Instance.OpenUI(UINameConst.UIPopupTimeOrderGift);
                }));
                CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.LossTime, coolTimeKey, CommonUtils.GetTimeStamp(),
                    TimeOrderConfigManager.Instance.TableTimeOrderSettingList[0].GiftPopTimeCD * 60 * 1000);
                return true;
            }

            if (IsOrderFinish())
                return false;

            if (TimeOrder.OrderGiftIndex.Contains(index))
                return false;

            if (TimeOrder.OrderGiftContent.Count == 0)
            {
                var storageOrder = MainOrderManager.Instance.CurTaskList.Find(a => a.Slot == (int)MainOrderCreateTime._orgSlot);

                for (var i = 0; i < storageOrder.ItemIds.Count; i++)
                {
                    int id = storageOrder.ItemIds[i];
                    if (i == index)
                        continue;

                    TimeOrder.OrderGiftContent.Add(id);
                }
            }

            TimeOrder.IsShowGift = true;    
            CoroutineManager.Instance.StartCoroutine(CommonUtils.DelayWork(0.5f, () =>
            {
                UIManager.Instance.OpenUI(UINameConst.UIPopupTimeOrderGift);
            }));
            CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.LossTime, coolTimeKey, CommonUtils.GetTimeStamp(),
                TimeOrderConfigManager.Instance.TableTimeOrderSettingList[0].GiftPopTimeCD * 60 * 1000);
            return true;
        }

        public bool IsOrderFinish()
        {
            var storageOrder = MainOrderManager.Instance.CurTaskList.Find(a => a.Slot == (int)MainOrderCreateTime._orgSlot);
            if (storageOrder == null)
                return true;

            foreach (var id in storageOrder.ItemIds)
            {
                if (!MainOrderCreatorRandomCommon.HavenEnoughMerge(id, 1))
                    return false;
            }

            return true;
        }
    }
}