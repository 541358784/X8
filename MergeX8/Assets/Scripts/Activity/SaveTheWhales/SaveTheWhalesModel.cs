using System;
using System.Linq;
using DragonPlus;
using DragonPlus.Config.SaveTheWhales;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using EpForceDirectedGraph.cs;
using Framework;
using Gameplay;
using UnityEngine;

namespace Activity.SaveTheWhales
{
    public class SaveTheWhalesModel : ActivityEntityBase
    {
        private static SaveTheWhalesModel _instance;
        public static SaveTheWhalesModel Instance => _instance ?? (_instance = new SaveTheWhalesModel());

        public override string Guid => "OPS_EVENT_TYPE_SAVE_THE_WHALES";
        
        public StorageSaveTheWhales StorageSaveTheWhales
        {
            get { return StorageManager.Instance.GetStorage<StorageHome>().SaveTheWhales; }
        }
        public SaveTheWhalesModel()
        {
    
            EventDispatcher.Instance.AddEvent<EventUserDataConsumeRes>((e) =>
            {
                OnConsumeRes(e.ResId,e.Count);
            });
        }

        private SaveTheWhalesActivityConfig _saveTheWhalesActivityConfig;

        public SaveTheWhalesActivityConfig SaveTheWhalesActivityConfig
        {
            get
            {
                int group = Math.Max(StorageSaveTheWhales.GroupId - 1, 0);
                if (group >= SaveTheWhalesConfigManager.Instance.SaveTheWhalesActivityConfigList.Count)
                    group = SaveTheWhalesConfigManager.Instance.SaveTheWhalesActivityConfigList.Count - 1;
                return  SaveTheWhalesConfigManager.Instance.SaveTheWhalesActivityConfigList[group];;
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
            SaveTheWhalesConfigManager.Instance.InitConfig(configJson);
            InitServerDataFinish();
        }

        public override void UpdateActivityState()
        {
            InitServerDataFinish();
        }
        
        public override bool CanDownLoadRes()
        {
            return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.SaveTheWhales);
        }

        protected override void InitServerDataFinish()
        {
            if (ActivityId.IsEmptyString())
                return;
            if (StorageSaveTheWhales.ActivityId != ActivityId)
            {
                StorageSaveTheWhales.Clear();
                StorageSaveTheWhales.ActivityId = ActivityId;
                StorageSaveTheWhales.GroupId =
                    SaveTheWhalesConfigManager.Instance.SaveTheWhalesActivityConfigList.First().Id;
                StorageSaveTheWhales.ConsumeEnergyTime = (long)(APIManager.Instance.GetServerTime() / XUtility.DayTime);
            }
        }

        public bool IsOpen()
        {
            if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.SaveTheWhales))
                return false;
            if (!IsOpened() && !IsLocalOpen())
                return false;
            
            return true;
        }

        public void InitConfigLocal()
        {
            LocalOpen = true;
            SaveTheWhalesConfigManager.Instance.InitConfig();
            if (StorageSaveTheWhales.ActivityId.IsEmptyString())
            {
                StorageSaveTheWhales.Clear();
                StorageSaveTheWhales.ActivityId = "Local";
                StorageSaveTheWhales.GroupId =
                    SaveTheWhalesConfigManager.Instance.SaveTheWhalesActivityConfigList.First().Id;
                StorageSaveTheWhales.ConsumeEnergyTime = (long)(APIManager.Instance.GetServerTime() / XUtility.DayTime);
            }
        }

        public bool LocalOpen = false;
        public bool IsLocalOpen()
        {
            return LocalOpen;
        }
        public  long GetLeftTime()
        {
            return Math.Max(StorageSaveTheWhales.JoinEndTime - (long) APIManager.Instance.GetServerTime(), 0);
        }
        public  string GetLeftTimeStr()
        {
            return CommonUtils.FormatLongToTimeStr(GetLeftTime());
        }
       
        public bool IsJoin()
        {
            if (!IsOpen())
                return false;
            if (IsTimeOut())
                return false;
            if (StorageSaveTheWhales.IsFinish)
                return false;
            
            return StorageSaveTheWhales.IsJoin;
        }

        public bool IsInCd()
        {
            var curDay = APIManager.Instance.GetServerTime()/XUtility.DayTime;
            var lastDay = StorageSaveTheWhales.JoinDay;
            var config = SaveTheWhalesActivityConfig;
            return (long)curDay < ( lastDay + config.CoolingCd);
        }

        public bool IsTimeOut()
        {
            return (long)APIManager.Instance.GetServerTime() > StorageSaveTheWhales.JoinEndTime;
        }
        public void CheckJoinTimeOut()
        {
            if (StorageSaveTheWhales.IsJoin && IsTimeOut() && !IsInCd())
            {
                StorageSaveTheWhales.IsJoin = false;
                if (!StorageSaveTheWhales.IsFinish)
                    AddFailCount(1);
            }
        }

        public void AddFailCount(int failCount)
        {
            Debug.LogError("鲸鱼失败次数+"+failCount);
            StorageSaveTheWhales.FailCount+=failCount;
            if (SaveTheWhalesActivityConfig.Downgrade > 0 &&
                StorageSaveTheWhales.FailCount >= SaveTheWhalesActivityConfig.Downgrade)
            {
                StorageSaveTheWhales.GroupId--;
                StorageSaveTheWhales.FailCount = 0;
                StorageSaveTheWhales.SuccessCount = 0;
                Debug.LogError("鲸鱼触发降级 新等级为"+StorageSaveTheWhales.GroupId);
            }
        }
        public void AddSuccessCount()
        {
            Debug.LogError("鲸鱼成功次数+"+1);
            StorageSaveTheWhales.SuccessCount++;
            if (SaveTheWhalesActivityConfig.Upgrade > 0 &&
                StorageSaveTheWhales.SuccessCount >= SaveTheWhalesActivityConfig.Upgrade)
            {
                StorageSaveTheWhales.GroupId++;
                StorageSaveTheWhales.FailCount = 0;
                StorageSaveTheWhales.SuccessCount = 0;
                Debug.LogError("鲸鱼触发升级 新等级为"+StorageSaveTheWhales.GroupId);
            }
        }

        // public void ReSetCD()
        // {
        //     var time = (long) APIManager.Instance.GetServerTime() - StorageSaveTheWhales.JoinEndTime;
        //     if (StorageSaveTheWhales.IsJoin)
        //     {
        //         if (time >= SaveTheWhalesActivityConfig.CoolingCd * 60 * 1000)
        //         {
        //             StorageSaveTheWhales.IsJoin = false;
        //         }
        //     }
        //     //降档逻辑
        //     if (StorageSaveTheWhales.JoinEndTime>0 && time - SaveTheWhalesActivityConfig.CoolingCd * 60 * 1000 - SaveTheWhalesActivityConfig.JumpDays*24 * 60 * 1000 > 0)
        //     {
        //         if (StorageSaveTheWhales.GroupId > 0)
        //             StorageSaveTheWhales.GroupId--;
        //         StorageSaveTheWhales.JoinEndTime = 0;
        //     }
        // }
        
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

            var configs = SaveTheWhalesConfigManager.Instance.SaveTheWhalesTaskConfigList;
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
        public void OnConsumeRes(UserData.ResourceId resId,int count)
        {
            if (!IsOpen())
                return;
            if (resId == UserData.ResourceId.Energy)
            {
                CheckEnergyReset();
                StorageSaveTheWhales.ConsumeEnergy += count;
                CanShowJoin();
            }
        }

        public void CheckEnergyReset()
        {
            var curDay = (long)(APIManager.Instance.GetServerTime() / XUtility.DayTime);
            if (StorageSaveTheWhales.ConsumeEnergyTime != curDay)
            {
                Debug.LogError("鲸鱼重置体力消耗");
                StorageSaveTheWhales.ConsumeEnergy = 0;
                var lastDay = StorageSaveTheWhales.JoinDay;
                var config = SaveTheWhalesActivityConfig;
                var cdEndDay =  lastDay + config.CoolingCd;
                if (IsTimeOut() && curDay > cdEndDay)
                {
                    AddFailCount((int)(curDay-cdEndDay));
                }
                StorageSaveTheWhales.ConsumeEnergyTime = curDay;
            }
        }
        public void AddWater(int addCount,string reason)
        {
            if (!IsJoin())
                return;
            StorageSaveTheWhales.CollectCount += addCount;
        
        }    
        public int GetWater()
        {
            return  StorageSaveTheWhales.CollectCount ;
        }
        public void CheckFinish()
        {
            if (!IsJoin())
                return;
            if (StorageSaveTheWhales.CollectCount >= SaveTheWhalesActivityConfig.CollectCount)
            {
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventSaveTheWhalesReward,
                    SaveTheWhalesModel.Instance.StorageSaveTheWhales.GroupId.ToString());
                UIManager.Instance.OpenUI(UINameConst.UISaveTheWhalesReward,true);
                StorageSaveTheWhales.IsFinish = true;
                AddSuccessCount();
            }
        }
        public bool CanShowJoin()
        {
            if (!IsOpen())
                return false;
            // ReSetCD();
            CheckEnergyReset();
            CheckJoinTimeOut();
            if (StorageSaveTheWhales.IsJoin)
                return false;
            if (StorageSaveTheWhales.ConsumeEnergy < SaveTheWhalesActivityConfig.CostEnergy)
                return false;
            long openTime = SaveTheWhalesActivityConfig.LimitTime * 60 * 1000;
            // if (IsInitFromServer())//只有活动模式下结束判断生效
            // {
            //     if ((long)EndTime - (long)APIManager.Instance.GetServerTime() <= openTime)
            //         return false;
            // }
            if (StorageSaveTheWhales.IsFinish)
            {
                StorageSaveTheWhales.IsFinish = false;
            }
           
            StorageSaveTheWhales.JoinTime = (long)APIManager.Instance.GetServerTime();
            StorageSaveTheWhales.JoinEndTime = (long)APIManager.Instance.GetServerTime()+openTime;
            StorageSaveTheWhales.JoinDay = (long)(APIManager.Instance.GetServerTime()/XUtility.DayTime);
            StorageSaveTheWhales.IsJoin = true;
            StorageSaveTheWhales.CollectCount = 0;
            UIManager.Instance.OpenUI(UINameConst.UISaveTheWhalesReward);
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventSaveTheWhalesStart ,StorageSaveTheWhales.GroupId.ToString());
            return true;
        }
        
        public string GetTaskItemAssetPath()
        {
            return "Prefabs/Activity/SaveTheWhales/TaskList_SaveTheWhales";
        }
        
    }
}