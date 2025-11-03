using System;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.Turntable;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;

namespace Activity.Turntable.Model
{
    public class TurntableModel : ActivityEntityBase
    {
        private static TurntableModel _instance;
        public static TurntableModel Instance => _instance ?? (_instance = new TurntableModel());

        public override string Guid => "OPS_EVENT_TYPE_TURNTABLE";
        
        public StorageTurntable Turntable
        {
            get { return StorageManager.Instance.GetStorage<StorageHome>().Turntable; }
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
            TurntableConfigManager.Instance.InitConfig(configJson);
            InitServerDataFinish();
        }

        public override void UpdateActivityState()
        {
            InitServerDataFinish();
        }

        protected override void InitServerDataFinish()
        {
            base.InitServerDataFinish();
            
            if (Turntable.ActivityId == ActivityId)
            {
                Turntable.StartActivityTime = (long)StartTime;
                Turntable.ActivityEndTime = (long)EndTime;
                return;
            }
            
            Turntable.ActivityId = ActivityId;
            Turntable.IsStart = false;
            Turntable.StartActivityTime = (long)StartTime;
            Turntable.ActivityEndTime =(long)EndTime;
            Turntable.ActivityCoin = 0;
            Turntable.PoolNum.Clear();
        }

        public override bool CanDownLoadRes()
        {
            return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.Turntable);
        }
        
        public ulong GetActivityLeftTime()
        {
            long extra = 0;

            var left = (long)Turntable.ActivityEndTime - (long)APIManager.Instance.GetServerTime();
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
            if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.Turntable))
                return false;

            return base.IsOpened(hasLog);
        }

        public bool CanShowUI()
        {
            if (!IsOpened())
                return false;

            if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.Turntable))
                return false;
            
            if (Turntable.IsStart)
                return false;
            
            UIManager.Instance.OpenUI(UINameConst.UIPopupTurntableMain);

            Turntable.ActivityCoin = 0;
            Turntable.IsStart = true;
            return true;
        }

        public void SetCoin(int coin, string reason)
        {
            if(!IsOpened())
                return;
            
            Turntable.ActivityCoin += coin;
            Turntable.ActivityCoin = Math.Max(Turntable.ActivityCoin, 0);

            if (coin > 0)
            {
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventTurntableGetActivityCoin, coin.ToString(), Turntable.ActivityCoin.ToString());
            }
            else
            {
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventTurntableConsumeActivityCoin, coin.ToString(), Turntable.ActivityCoin.ToString());
            }
        }

        public int GetCoin()
        {
            return Turntable.ActivityCoin;
        }

        public int GetTaskValue(StorageTaskItem taskItem, bool isMul)
        {
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
        
            var configs = TurntableConfigManager.Instance.TurntableOrderConfigList;
            if (configs != null && configs.Count > 0)
            {
                int value = 0;
                foreach (var config in configs)
                {
                    if (tempPrice <= config.Max_value)
                    {
                        value = config.Output;
                        break;
                    }
                }
                return value;   
            }
            else
            {
                int coin = ((tempPrice/20)+1);
                coin = Math.Min(coin, 8);
                return coin;
            }
        }

        public int GetPoolIndex()
        {
            List<int> poolResult = new List<int>();
            List<int> poolWeight = new List<int>();

            var poolConfig = TurntableConfigManager.Instance.TurntablePoolConfigList[0];
            for (var i = 0; i < poolConfig.ResultPool.Count; i++)
            {
                int result = poolConfig.ResultPool[i];
                int num = poolConfig.NumPool[i];
                int weight = poolConfig.WeightPool[i];

                int poolNum = 0;

                if (Turntable.PoolNum.ContainsKey(result))
                    poolNum = Turntable.PoolNum[result];


                int residueNum = num - poolNum;
                if (residueNum <= 0)
                    continue;

                poolResult.Add(result);
                poolWeight.Add(residueNum * weight);
            }

            if (poolResult.Count == 0)
            {
                Turntable.PoolNum.Clear();

                return GetPoolIndex();
            }

            int randomResult = poolResult.RandomOneByWeight((a) =>
            {
                int index = poolResult.FindIndex(b => a == b);
                return poolWeight[index];
            });

            if(!Turntable.PoolNum.ContainsKey(randomResult))
                Turntable.PoolNum.Add(randomResult, 0);
            Turntable.PoolNum[randomResult]++;
            
            return randomResult - 1;//转成索引
        }
    }
}