using System.Collections.Generic;
using Activity.LimitTimeOrder;
using DragonPlus.Config.CrazeOrder;
using DragonPlus.Config.CrazeOrder;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using Merge.Order;
using UnityEngine;

namespace Activity.CrazeOrder.Model
{
    public class CrazeOrderModel : ActivityEntityBase
    {
        private static CrazeOrderModel _instance;
        public static CrazeOrderModel Instance => _instance ?? (_instance = new CrazeOrderModel());
    
        public override string Guid => "OPS_EVENT_TYPE_CRAZE_ORDER";

        public StorageCrazeOrder CrazeOrder
        {
            get
            {
                return StorageManager.Instance.GetStorage<StorageHome>().CrazeOrder;
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
            
            CrazeOrderConfigManager.Instance.InitConfig(configJson);
            CrazeOrderConfigManager.Instance.Trim();
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

            if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.CrazeOrder))
                return "";
            
            if (!IsJoin())
                return "";

            var group = CrazeOrderConfigManager.Instance.GetStageConfigs();
            if (group == null)
                return "";

            var currentStage = group.Find(a => a.Id == CrazeOrder.Stage);
            if (currentStage == null)
                return "";
            
            var preStage = group.Find(a => a.Id == CrazeOrder.Stage-1);
            
            int diffNum = preStage != null ? preStage.OrderNum : 0;
            
            return (CompleteNum-diffNum) + "/" + (currentStage.OrderNum-diffNum);
        }

        public float GetProgress()
        {
            if (!IsOpened())
                return 0f;

            if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.CrazeOrder))
                return 0f;
            
            if (!IsJoin())
                return 0f;

            return 1.0f*CrazeOrder.CompleteNum/GetGroupOrderCount();
        }
        
        public int GetGroupOrderCount()
        {
            if (!IsOpened())
                return 0;

            if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.CrazeOrder))
                return 0;
            
            if (!IsJoin())
                return 0;
            
            var group = CrazeOrderConfigManager.Instance.GetOrderConfig( CrazeOrder.GroupId);
            if (group == null)
                return 0;
            
            return group.Count;
        }

        public void UpdateJoinTime()
        {
            CrazeOrder.JoinStartTime = (long)APIManager.Instance.GetServerTime();
            CrazeOrder.JoinEndTime = (long)APIManager.Instance.GetServerTime() + CrazeOrderConfigManager.Instance.CrazeOrderSettingList[0].OpenTime*60*1000;
        }
        
        protected override void InitServerDataFinish()
        {
            if(ActivityId.IsEmptyString())
                return;

            if (CrazeOrder.ActivityId == ActivityId)
            {
                CrazeOrder.StartActivityTime = (long)StartTime;
                CrazeOrder.ActivityEndTime = (long)EndTime;
                return;
            }

            CheckJoinEnd(true);
            
            CrazeOrder.ActivityId = ActivityId;
            CrazeOrder.IsJoin = false;
            CrazeOrder.JoinStartTime = 0;
            CrazeOrder.JoinEndTime = 0;
            CrazeOrder.StartActivityTime = (long)StartTime;
            CrazeOrder.ActivityEndTime = (long)EndTime;
            CrazeOrder.CompleteNum = 0;
            CrazeOrder.GroupId = 0;
            CrazeOrder.Stage = 0;
            CrazeOrder.AnimStage = 0;
            CrazeOrder.PayLevelGroup = PayLevelModel.Instance.GetCurPayLevelConfig().CrazeOrder;
        }

        public bool IsJoin()
        {
            return CrazeOrder.IsJoin;
        }
        
        public virtual string GetJoinEndTimeString()
        {
            if (!IsOpened())
                return "";

            if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.CrazeOrder))
                return "";
            
            if (!IsJoin())
                return "";
            
            var left = (long) CrazeOrder.JoinEndTime - (long) APIManager.Instance.GetServerTime();
            if (left < 0)
                left = 0;
            
            return CommonUtils.FormatLongToTimeStr(left);
        }

        public bool IsTimeEnd()
        {
            if (CrazeOrder.IsJoin)
            {
                return (long)APIManager.Instance.GetServerTime() > CrazeOrder.JoinEndTime;
            }
            else
            {
                return (long)APIManager.Instance.GetServerTime() > CrazeOrder.ActivityEndTime;
            }
        }
        public void CheckJoinEnd(bool isForce = false)
        {
            if(CrazeOrder.ActivityId.IsEmptyString())
                return;
            
            if(!CrazeOrder.IsJoin)
                return;
            
            if(CrazeOrder.GroupId == 0)
                return;
            
            if(!isForce && (long) APIManager.Instance.GetServerTime() < CrazeOrder.JoinEndTime)
                return;

            MainOrderManager.Instance.RemoveOrder(MainOrderType.Craze);
            
            CrazeOrder.GroupId = 0;
        }
        public override bool CanDownLoadRes()
        {
            return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.CrazeOrder);
        }

        public void CompleteAllOrder()
        {
            GroupId = 0;
            CrazeOrder.JoinEndTime = 0;
        }
        public int CompleteNum
        {
            get { return CrazeOrder.CompleteNum;}
            set { CrazeOrder.CompleteNum = value;}
        }
        public int Stage
        {
            get { return CrazeOrder.Stage;}
            set { CrazeOrder.Stage = value;}
        }
        public int AnimStage
        {
            get { return CrazeOrder.AnimStage;}
            set { CrazeOrder.AnimStage = value;}
        }
        
        public int GroupId
        {
            get { return CrazeOrder.GroupId;}
            private set { CrazeOrder.GroupId = value; }
        }
        
            
        public bool CanShowJoin()
        {
            if (!IsOpened())
                return false;

            if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.CrazeOrder))
                return false;
            
            if (IsTimeEnd())
                return false;
            
            if (CrazeOrder.IsJoin)
                return false;

            var level = ExperenceModel.Instance.GetLevel();
            var payLevel = PayLevelModel.Instance.GetPayLevel();
            int adaptLevel = CrazeOrderConfigManager.Instance.AdaptOrderLevel(level);
            var configGroup = CrazeOrderConfigManager.Instance.GetOrderConfig(adaptLevel);
            
            int openTime = CrazeOrderConfigManager.Instance.CrazeOrderSettingList[0].OpenTime * 60 * 1000;
            if ((long)EndTime - (long)APIManager.Instance.GetServerTime() <= openTime)
                return false;

            CrazeOrder.GroupId = adaptLevel;
            CrazeOrder.IsJoin = true;
            CrazeOrder.Stage = 1;
            UpdateJoinTime();
            
            UIManager.Instance.OpenUI(UINameConst.UICrazeOrderMain);

            MainOrderCreateCraze.TryCreateOrder(configGroup[0], true);
            return true;
        }
    
        public CrazeStageConfig GetStageConfig()
        {
            var config = CrazeOrderConfigManager.Instance.GetStageConfigs().Find(a=>a.Id == Stage);
            if (config == null)
                return null;

            return config;
        }
    }
}