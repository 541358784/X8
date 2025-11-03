using System;
using System.Collections.Generic;
using System.Linq;
using DragonPlus;
using DragonPlus.Config.DiamondReward;
using DragonPlus.Config.Turntable;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using SRF;
using UnityEngine;

namespace Activity.DiamondRewardModel.Model
{
    public partial class DiamondRewardModel : ActivityEntityBase
    {
        private static DiamondRewardModel _instance;
        public static DiamondRewardModel Instance => _instance ?? (_instance = new DiamondRewardModel());

        public override string Guid => "OPS_EVENT_TYPE_DIAMOND_REWARD";
        public int Level => DiamondReward.Level;
        public StorageDiamondReward DiamondReward
        {
            get { return StorageManager.Instance.GetStorage<StorageHome>().DiamondReward; }
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
            DiamondRewardConfigManager.Instance.InitConfig(configJson);
            InitServerDataFinish();
        }

        public override void UpdateActivityState()
        {
            InitServerDataFinish();
        }

        protected override void InitServerDataFinish()
        {
            base.InitServerDataFinish();
            
            if(ActivityId.IsEmptyString())
                return;
            
            if (DiamondReward.ActivityId == ActivityId)
            {
                DiamondReward.StartActivityTime = (long)StartTime;
                DiamondReward.ActivityEndTime = (long)EndTime;
                return;
            }

            var lastLevel = Level;
            
            if (DiamondReward.CanUpgrade)
            {
                lastLevel++;
            }

            var findLevel = -1;
            var curDiamondCount = UserData.Instance.GetRes(UserData.ResourceId.Diamond);
            foreach (var config in DiamondSettingConfigList)
            {
                if (config.LowRange <= curDiamondCount && config.HighRange >= curDiamondCount)
                {
                    findLevel = config.Id;
                }
            }
            if (findLevel < 0)
                findLevel = DiamondSettingConfigList.Last().Id;
            var finalLevel = Math.Max(findLevel, lastLevel);
            Debug.LogError("钻石抽奖等级变更 重定位="+findLevel+" 历史="+lastLevel+" 结果="+finalLevel);
            DiamondReward.Level = finalLevel;
            
            DiamondReward.ActivityId = ActivityId;
            DiamondReward.IsGetAllReward = false;
            DiamondReward.StartActivityTime = (long)StartTime;
            DiamondReward.ActivityEndTime =(long)EndTime;
            DiamondReward.PoolId = DiamondPoolConfigLevelList.Random().Id;
            DiamondReward.PoolIndex = 0;
            DiamondReward.PoolStatus.Clear();
            DiamondReward.IsIgnorePopUI = false;
            DiamondReward.CanUpgrade = false;
            
            foreach (var id in DiamondResultConfigLevelDic.Keys.ToList())
            {
                DiamondReward.PoolStatus.Add(id, -1);   
            }
        }
   
        public override bool CanDownLoadRes()
        {
            return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.DiamondReward);
        }
        
        public ulong GetActivityLeftTime()
        {
            long extra = 0;

            var left = (long)DiamondReward.ActivityEndTime - (long)APIManager.Instance.GetServerTime();
            if (left < 0)
                left = 0;
            return (ulong)left;
        }

        // 活动剩余时间的字符串显示
        public virtual string GetActivityLeftTimeString()
        {
            return CommonUtils.FormatLongToTimeStr((long)GetActivityLeftTime());
        }

        public override bool IsOpened(bool hasLog = false)
        {
            if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.DiamondReward))
                return false;

            if (DiamondReward.IsGetAllReward)
                return false;
            
            return base.IsOpened(hasLog);
        }

        private string constPlaceId = "diamondreward";
        public bool CanShowUI()
        {
            if (!IsOpened())
                return false;
            
            if (CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, constPlaceId))
                return false;
        
            if (UIManager.Instance.GetOpenedUIByPath(UINameConst.UIDiamondRewardMain))
                return false;

        
            CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, constPlaceId,CommonUtils.GetTimeStamp());

            UIManager.Instance.OpenUI(UINameConst.UIDiamondRewardMain);
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventDiamondrewardEnter,"0",
                DiamondRewardModel.Instance.DiamondReward.Level.ToString());

            return true;
        }

        public bool IsGetReward(int id)
        {
            return DiamondReward.PoolStatus.ContainsValue(id);
        }
        
        public bool IsOpenConch(int index)
        {
            if (!DiamondReward.PoolStatus.ContainsKey(index))
                return false;

            return DiamondReward.PoolStatus[index] > 0;
        }

        public ResData OpenConch(int index)
        {
            var config = DiamondPoolConfigLevelPool;
            DiamondReward.PoolStatus[index] = config.ResultPool[DiamondReward.PoolIndex];
            DiamondReward.PoolIndex++;
            if (!DiamondReward.CanUpgrade && DiamondReward.PoolIndex >= DiamondSettingConfigLevel.UpgradeCount)//达成升级目标
                DiamondReward.CanUpgrade = true;

            DiamondReward.IsGetAllReward = DiamondReward.PoolIndex >= DiamondPoolConfigLevelPool.ResultPool.Count;
            
            var resultConfig = DiamondResultConfigLevelDic[DiamondReward.PoolStatus[index]];

            ResData resData;
            resData = new ResData(resultConfig.RewardId, resultConfig.RewardNum);
            return resData;
        }

        public ResData GetBoxData(int index)
        {
            if (!DiamondReward.PoolStatus.ContainsKey(index))
                return null;

            int id = DiamondReward.PoolStatus[index];
            if (id < 0)
                return null;

            var resultConfig = DiamondResultConfigLevelDic[id];
            ResData resData;
            resData = new ResData(resultConfig.RewardId, resultConfig.RewardNum);
            return resData;
        }

        public int GetConsume()
        {
            return DiamondSettingConfigLevel.Price;
        }
        private string constIgnorePlaceId = "diamondrewardignorekey";
        public bool IsIgnorePopUI
        {
            get
            {
                return DiamondReward.IsIgnorePopUI;
            }
            set
            {
                DiamondReward.IsIgnorePopUI = value;
                if (DiamondReward.IsIgnorePopUI)
                {
                    CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, constIgnorePlaceId,CommonUtils.GetTimeStamp());
                }
            }
        }

        public void UpdateIgnorePopUIStatus()
        {
            if(!DiamondReward.IsIgnorePopUI)
                return;
            
            if (CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, constIgnorePlaceId))
                return;

            IsIgnorePopUI = false;
        }
    }
}