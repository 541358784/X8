using System.Collections.Generic;
using DragonPlus.Config.FarmTimeOrder;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using Farm.Model;
using Farm.Order;
using SomeWhere;
using UnityEngine;

namespace Activity.FarmTimeOrder
{
    public class FarmTimeLimitOrderModel: ActivityEntityBase
    {
        private static FarmTimeLimitOrderModel _instance;
        public static FarmTimeLimitOrderModel Instance => _instance ?? (_instance = new FarmTimeLimitOrderModel());
    
        public override string Guid => "OPS_EVENT_TYPE_FARM_TIME_LIMIT_ORDER";

        public StorageFarmTimeOrder FarmTimeLimitOrder
        {
            get
            {
               return StorageManager.Instance.GetStorage<StorageHome>().FarmTimeOrder;
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
            FarmTimeOrderConfigManager.Instance.InitConfig(configJson);
            InitServerDataFinish();
        }

        public override void UpdateActivityState()
        {
            InitServerDataFinish();
        }

        protected override void InitServerDataFinish()
        {
            if(ActivityId.IsEmptyString())
                return;

            if (FarmTimeLimitOrder.ActivityId == ActivityId)
            {
                FarmTimeLimitOrder.StartActivityTime = (long)StartTime;
                FarmTimeLimitOrder.ActivityEndTime = (long)EndTime;
                return;
            }

            FarmOrderManager.Instance.RemoveOrder((int)OrderSlot.Activity_TimeOrder);
            
            FarmTimeLimitOrder.ActivityId = ActivityId;
            FarmTimeLimitOrder.IsJoin = false;
            FarmTimeLimitOrder.OrderId = 0;
            FarmTimeLimitOrder.JoinStartTime = 0;
            FarmTimeLimitOrder.JoinEndTime = 0;
            FarmTimeLimitOrder.StartActivityTime = (long)StartTime;
            FarmTimeLimitOrder.ActivityEndTime = (long)EndTime;
        }

        public bool IsJoin()
        {
            return FarmTimeLimitOrder.IsJoin;
        }
        
        public virtual string GetJoinEndTimeString()
        {
            if (!IsOpened())
                return "";

            if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.Farm_TiemOrder))
                return "";
            
            if (!IsJoin())
                return "";
            
            var left = (long) FarmTimeLimitOrder.JoinEndTime - (long) APIManager.Instance.GetServerTime();
            if (left < 0)
                left = 0;
            
            return CommonUtils.FormatLongToTimeStr(left);
        }

        public bool IsTimeEnd()
        {
            if (FarmTimeLimitOrder.IsJoin)
            {
                return (long)APIManager.Instance.GetServerTime() > FarmTimeLimitOrder.JoinEndTime;
            }
            else
            {
                return (long)APIManager.Instance.GetServerTime() > FarmTimeLimitOrder.ActivityEndTime;
            }
        }
        public void CheckJoinEnd(bool isForce = false)
        {
            if(FarmTimeLimitOrder.ActivityId.IsEmptyString())
                return;
            
            if(FarmTimeLimitOrder.OrderId == 0)
                return;
            
            if(!isForce && (long) APIManager.Instance.GetServerTime() < FarmTimeLimitOrder.JoinEndTime)
                return;

            FarmOrderManager.Instance.RemoveOrder((int)OrderSlot.Activity_TimeOrder);
            
            FarmTimeLimitOrder.OrderId = 0;
        }
        public override bool CanDownLoadRes()
        {
            return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.Farm_TiemOrder);
        }

        public void CompleteAllOrder()
        {
            OrderId = 0;
            FarmTimeLimitOrder.JoinEndTime = 0;
        }
        public int OrderId
        {
            get { return FarmTimeLimitOrder.OrderId;}
            set { FarmTimeLimitOrder.OrderId = value;}
        }
        
        public bool CanShowJoin()
        {
            if (!IsOpened())
                return false;

            if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.Farm_TiemOrder))
                return false;
            
            if (IsTimeEnd())
                return false;
            
            if (FarmTimeLimitOrder.IsJoin)
                return false;

            long openTime = FarmTimeOrderConfigManager.Instance.TableFarmTimeOrderGroupList[0].OpenTime * 1000;
            if ((long)EndTime - (long)APIManager.Instance.GetServerTime() <= openTime)
                return false;
            
            
            UIManager.Instance.OpenUI(UINameConst.UIPopupFarmTimeLimitOrder);

            var level = FarmModel.Instance.GetLevel();
            TableFarmTimeOrderGroup adaptConfig = FarmTimeOrderConfigManager.Instance.TableFarmTimeOrderGroupList[FarmTimeOrderConfigManager.Instance.TableFarmTimeOrderGroupList.Count-1];
           for(int i = 0; i < FarmTimeOrderConfigManager.Instance.TableFarmTimeOrderGroupList.Count; i++)
            {
                if (FarmTimeOrderConfigManager.Instance.TableFarmTimeOrderGroupList[i].Level >= level)
                {
                    adaptConfig = FarmTimeOrderConfigManager.Instance.TableFarmTimeOrderGroupList[i];
                    break;
                }
            }

            var orderId = adaptConfig.OrderIds.Random();
            var orderConfig = FarmTimeOrderConfigManager.Instance.TableFarmTimeOrderConfigList.Find(a => a.Id == orderId);
            
            FarmTimeLimitOrder.OrderId = orderConfig.Id;
            var order = FarmOrderManager.Instance.CreatorOrder(FarmTimeLimitOrder.OrderId, orderConfig.RequireMents, orderConfig.RequireNums, OrderSlot.Activity_TimeOrder, orderConfig.RewardIds, orderConfig.RewardNums);
            if (order != null)
            {
                EventDispatcher.Instance.DispatchEvent(EventEnum.FARM_ORDER_REFRESH, null, new List<StorageFarmOrderItem>(){order});
            }
            
            FarmTimeLimitOrder.IsJoin = true;
            FarmTimeLimitOrder.JoinStartTime = (long)APIManager.Instance.GetServerTime();
            FarmTimeLimitOrder.JoinEndTime = (long)APIManager.Instance.GetServerTime() + FarmTimeOrderConfigManager.Instance.TableFarmTimeOrderGroupList[0].OpenTime*1000;
            return true;
        }

        
    }
}