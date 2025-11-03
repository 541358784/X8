using System;
using System.Collections.Generic;
using System.Linq;
using DragonPlus;
using DragonPlus.Config.LimitOrderLine;
using DragonPlus.Config.TimeOrder;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Framework;
using Gameplay;
using Merge.Order;
using UnityEngine;

namespace Activity.LimitTimeOrder
{
    public class LimitTimeOrderModel : ActivityEntityBase
    {
        private static LimitTimeOrderModel _instance;
        public static LimitTimeOrderModel Instance => _instance ?? (_instance = new LimitTimeOrderModel());
    
        public override string Guid => "OPS_EVENT_TYPE_LIMIT_ORDER_LINE";

        public StorageLimitOrderLine LimitOrderLine
        {
            get
            {
               return StorageManager.Instance.GetStorage<StorageHome>().LimitOrderLine;
            }
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
            LimitOrderLineConfigManager.Instance.InitConfig(configJson);
            InitServerDataFinish();
        }

        public override void UpdateActivityState()
        {
            InitServerDataFinish();
        }

        public string GetOrderProgress()
        {
            if (!IsOpened())
                return "";

            if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.LimitOrderLine))
                return "";
            
            if (!IsJoin())
                return "";

            return LimitOrderLine.CompleteNum + "/" + GetGroupOrderCount();
        }

        public float GetProgress()
        {
            if (!IsOpened())
                return 0f;

            if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.LimitOrderLine))
                return 0f;
            
            if (!IsJoin())
                return 0f;

            return 1.0f*LimitOrderLine.CompleteNum/GetGroupOrderCount();
        }
        
        public int GetGroupOrderCount()
        {
            if (!IsOpened())
                return 0;

            if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.LimitOrderLine))
                return 0;
            
            if (!IsJoin())
                return 0;
            
            var group = LimitOrderLineConfigManager.Instance.TableTimeOrderLineGroupList.Find(a=>a.Id == LimitOrderLine.GroupId);
            if (group == null)
                return 0;
            
            return group.OrderIds.Count;
        }

        public void UpdateJoinTime(TableTimeOrderLineConfig config)
        {
            if(config == null)
                return;
            
            LimitOrderLine.JoinStartTime = (long)APIManager.Instance.GetServerTime();
            LimitOrderLine.JoinEndTime = (long)APIManager.Instance.GetServerTime() + config.OpenTime*60*1000;
        }
        
        protected override void InitServerDataFinish()
        {
            if(ActivityId.IsEmptyString())
                return;

            if (LimitOrderLine.ActivityId == ActivityId)
            {
                LimitOrderLine.StartActivityTime = (long)StartTime;
                LimitOrderLine.ActivityEndTime = (long)EndTime;
                return;
            }

            CheckJoinEnd(true);
            
            LimitOrderLine.Clear();
            LimitOrderLine.ActivityId = ActivityId;
            LimitOrderLine.StartActivityTime = (long)StartTime;
            LimitOrderLine.ActivityEndTime = (long)EndTime;
            LimitOrderLine.PayLevelGroup = PayLevelModel.Instance.GetCurPayLevelConfig().LimitOrderLine;
        }

        public bool IsJoin()
        {
            return LimitOrderLine.IsJoin;
        }
        
        public virtual string GetJoinEndTimeString()
        {
            if (!IsOpened())
                return "";

            if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.LimitOrderLine))
                return "";
            
            if (!IsJoin())
                return "";
            
            var left = (long) LimitOrderLine.JoinEndTime - (long) APIManager.Instance.GetServerTime();
            if (left < 0)
                left = 0;
            
            return CommonUtils.FormatLongToTimeStr(left);
        }

        public bool IsTimeEnd()
        {
            if (LimitOrderLine.IsJoin)
            {
                return (long)APIManager.Instance.GetServerTime() > LimitOrderLine.JoinEndTime;
            }
            else
            {
                return (long)APIManager.Instance.GetServerTime() > LimitOrderLine.ActivityEndTime;
            }
        }
        public void CheckJoinEnd(bool isForce = false)
        {
            if(LimitOrderLine.ActivityId.IsEmptyString())
                return;
            
            if(!LimitOrderLine.IsJoin)
                return;
            
            if(LimitOrderLine.OrderId == 0)
                return;
            
            if(!isForce && (long) APIManager.Instance.GetServerTime() < LimitOrderLine.JoinEndTime)
                return;

            MainOrderManager.Instance.RemoveOrder(LimitOrderLine.OrderId);
            
            LimitOrderLine.OrderId = 0;
        }
        public override bool CanDownLoadRes()
        {
            return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.LimitOrderLine);
        }

        public void CompleteAllOrder()
        {
            OrderId = 0;
            LimitOrderLine.JoinEndTime = 0;
        }
        public int OrderId
        {
            get { return LimitOrderLine.OrderId;}
            set { LimitOrderLine.OrderId = value;}
        }
        public int CompleteNum
        {
            get { return LimitOrderLine.CompleteNum;}
            set { LimitOrderLine.CompleteNum = value;}
        }
        public int AnimNum
        {
            get { return LimitOrderLine.AnimNum;}
            set { LimitOrderLine.AnimNum = value;}
        }
        
        public int GroupId
        {
            get { return LimitOrderLine.GroupId;}
        }
        public bool CanShowJoin()
        {
            if (!IsOpened())
                return false;

            if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.LimitOrderLine))
                return false;
            
            if (IsTimeEnd())
                return false;
            
            if (LimitOrderLine.IsJoin)
                return false;

            var timeOrderConfigs = LimitOrderLineConfigManager.Instance.GetTimeOrderConfigsByPayLevel();
            var level = ExperenceModel.Instance.GetLevel();

            TableTimeOrderLineGroup configGroup = timeOrderConfigs.Last();
            
            foreach (var groupConfig in timeOrderConfigs)
            {
                if (level <= groupConfig.Level)
                {
                    configGroup = groupConfig;
                    break;
                }
            }
            
            int openTime = 0;
            foreach (var orderId in configGroup.OrderIds)
            {
                var orderconfig = LimitOrderLineConfigManager.Instance.TableTimeOrderLineConfigList.Find(a => a.Id == orderId);
                if(orderconfig == null)
                    continue;

                openTime += orderconfig.OpenTime;
            }
            openTime = openTime * 60 * 1000;
            if ((long)EndTime - (long)APIManager.Instance.GetServerTime() <= openTime)
                return false;

            foreach (var config in LimitOrderLineConfigManager.Instance.TableTimeOrderLineConfigList)
            {
                MainOrderManager.Instance.RemoveOrderByComplete(config.Id);
            }
            
            int firstOrderId = configGroup.OrderIds[0];
            var orderConfig = LimitOrderLineConfigManager.Instance.TableTimeOrderLineConfigList.Find(a => a.Id == firstOrderId);
           
            var configs = LimitOrderLineConfigManager.Instance.GetTimeOrderGiftConfigsByPayLevel();

            LimitOrderLine.GiftId = configs.First().Id;
            LimitOrderLine.OrderId = firstOrderId;
            LimitOrderLine.GroupId = configGroup.Id;
            LimitOrderLine.IsJoin = true;
            UpdateJoinTime(orderConfig);
            
            UIManager.Instance.OpenUI(UINameConst.UIPopupLimitOrder);

            MainOrderCreateLimitLine.TryCreateOrder(orderConfig, true);
            return true;
        }
    
        public TableTimeOrderLineGroup GetGroupConfig()
        {
            var config = LimitOrderLineConfigManager.Instance.TableTimeOrderLineGroupList.Find(a => a.Id == LimitTimeOrderModel.Instance.GroupId);
            if (config == null)
                return null;

            return config;
        }
        
        
        public void PurchaseSuccess(int shopId, int configId)
        {
            TableTimeOrderLineGift config = null;
            if (configId < 0)
            {
                config = LimitOrderLineConfigManager.Instance.TableTimeOrderLineGiftList.Find(a => a.Id == configId);
            }
            else
            {
                config = LimitOrderLineConfigManager.Instance.TableTimeOrderLineGiftList.Find(a => a.ShopId == shopId);
            }

            if (config == null)
                config = LimitOrderLineConfigManager.Instance.TableTimeOrderLineGiftList.First();

            LimitOrderLine.IsBuyGift = true;
            UIManager.Instance.CloseUI(UINameConst.UIPopupLimitOrderGift);

            var rewards = CommonUtils.FormatReward(config.RewardId, config.RewardCount);
            foreach (var id in LimitOrderLine.OrderGiftContent)
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
                        MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonLimitTimeOrderGiftGet,
                        itemAId = reward.id,
                        isChange = true,
                    });
                }

                UserData.Instance.AddRes(reward.id, reward.count, new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.LimitTimeOrderGiftGet), true);
            }

            CommonRewardManager.Instance.PopCommonReward(rewards, CurrencyGroupManager.Instance.currencyController,
                false, new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.LimitTimeOrderGiftGet) { }, () => { });
        }
        
        
        
        private const string coolTimeKey = "ctk_limittimeordergift";

        public bool CanShowGift()
        {
            return CanShowGift(-1);
        }

        public bool CanShowGift(int index)
        {
            if (!IsOpened())
                return false;

            if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.LimitOrderLine))
                return false;

            if (IsTimeEnd())
                return false;

            if (!LimitOrderLine.IsJoin)
                return false;

            if (LimitOrderLine.IsBuyGift)
                return false;

            if (!IsLastOrder())
                return false;
            
            if (index < 0)
            {
                if (CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.LossTime, coolTimeKey))
                    return false;

                if (IsOrderFinish())
                    return false;

                if (LimitOrderLine.OrderGiftContent.Count == 0)
                    return false;

                CoroutineManager.Instance.StartCoroutine(CommonUtils.DelayWork(0.5f, () =>
                {
                    UIManager.Instance.OpenUI(UINameConst.UIPopupLimitOrderGift);
                }));
                CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.LossTime, coolTimeKey, CommonUtils.GetTimeStamp(),
                    LimitOrderLineConfigManager.Instance.TableTimeOrderLineSettingList[0].GiftPopTimeCD * 60 * 1000);
                return true;
            }

            if (IsOrderFinish())
                return false;

            if (LimitOrderLine.OrderGiftIndex.Contains(index))
                return false;

            if (LimitOrderLine.OrderGiftContent.Count == 0)
            {
                var storageOrder = MainOrderManager.Instance.CurTaskList.Find(a => a.Slot == (int)MainOrderCreateLimitLine._orgSlot);

                for (var i = 0; i < storageOrder.ItemIds.Count; i++)
                {
                    int id = storageOrder.ItemIds[i];
                    if (i == index)
                        continue;

                    LimitOrderLine.OrderGiftContent.Add(id);
                }
            }

            LimitOrderLine.IsShowGift = true;    
            CoroutineManager.Instance.StartCoroutine(CommonUtils.DelayWork(0.5f, () =>
            {
                UIManager.Instance.OpenUI(UINameConst.UIPopupLimitOrderGift);
            }));
            CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.LossTime, coolTimeKey, CommonUtils.GetTimeStamp(),
                LimitOrderLineConfigManager.Instance.TableTimeOrderLineSettingList[0].GiftPopTimeCD * 60 * 1000);
            return true;
        }

        public bool IsLastOrder()
        {
            if (LimitOrderLine.OrderId <= 0)
                return false;
            
            var config = LimitOrderLineConfigManager.Instance.TableTimeOrderLineGroupList.Find(a => a.Id == LimitTimeOrderModel.Instance.GroupId);
            if (config == null)
                return false;

            if (CompleteNum != config.OrderIds.Count - 1)
                return false;

            return true;
        }
        
        public bool IsOrderFinish()
        {
            var storageOrder = MainOrderManager.Instance.CurTaskList.Find(a => a.Slot == (int)MainOrderCreateLimitLine._orgSlot);
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