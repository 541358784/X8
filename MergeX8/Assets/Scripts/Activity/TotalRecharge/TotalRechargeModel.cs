using System;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.TotalRecharge;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using TotalRecharge_New;
using UnityEngine;

namespace Activity.TotalRecharge
{
    public class TotalRechargeModel : ActivityEntityBase
    {
        private static TotalRechargeModel _instance;
        public static TotalRechargeModel Instance => _instance ?? (_instance = new TotalRechargeModel());

        public override string Guid => "OPS_EVENT_TYPE_TOTAL_RECHARGE";
        
        public StorageTotalRecharge StorageTotalRecharge
        {
            get
            {
                var storage = StorageManager.Instance.GetStorage<StorageHome>().TotalRecharges;
                if (!storage.ContainsKey(StorageKey))
                {
                    var newStorage = new StorageTotalRecharge();
                  
                    if (IsInitFromServer())
                    {
                        newStorage.ActivityEndTime = (long)EndTime;
                    }
                    storage.Add(StorageKey,newStorage);
                }
                return storage[StorageKey];
            }
        }
   
        public TotalRechargeModel()
        {
            EventDispatcher.Instance.AddEventListener(EventEnum.PURCHASE_SUCCESS_REWARD,OnPaySuccess);
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
            TotalRechargeConfigManager.Instance.InitConfig(configJson);
            InitServerDataFinish();
        }

        public override void UpdateActivityState()
        {
            InitServerDataFinish();
        }
        
        public override bool CanDownLoadRes()
        {
            return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.TotalRecharge);
        }

        protected override void InitServerDataFinish()
        {
            if (ActivityId.IsEmptyString())
                return;
            if (StorageTotalRecharge.ActivityId != ActivityId)
            {
                StorageTotalRecharge.ActivityId = ActivityId;
                StorageTotalRecharge.TotalRecharge = 0;
                StorageTotalRecharge.CollectGroups.Clear();
                StorageTotalRecharge.GruopId = PayLevelModel.Instance.GetCurPayLevelConfig().TotalRechargeGroupId;
                if (!StorageTotalRecharge.IsOpenNewbie)
                    StorageTotalRecharge.IsOpenNewbie = TotalRechargeModel_New.Instance.IsOpen();
                var configs = TotalRechargeRewards();
                for (int i = 0; i < configs.Count; i++)
                {
                    StorageTotalRechargeReward reward = new StorageTotalRechargeReward();
                    reward.Ids = configs[i].Id;
                    reward.RewardIds.AddRange( configs[i].RewardId);
                    reward.RewardCounts.AddRange( configs[i].RewardNum);
                    reward.Store = configs[i].Score;
                    StorageTotalRecharge.Configs.Add(reward);
                }
            }
        }

        public List<TotalRechargeReward> TotalRechargeRewards()
        {
            return TotalRechargeConfigManager.Instance.TotalRechargeRewardList.FindAll(a=>a.Group==StorageTotalRecharge.GruopId);
        }

        public TotalRechargeReward GetTotalRechargeReward(int id)
        {
            return TotalRechargeRewards().Find(a=>a.Id==id);
        }
        public void OnPaySuccess(BaseEvent baseEvent)
        {
            if (!IsOpen())
                return;
            if (baseEvent.datas == null || baseEvent.datas.Length <= 0)
                return;
            List<ResData> resDatas =(List<ResData>) baseEvent.datas[0];
            foreach (var resData in resDatas)
            {
                if (resData.id ==(int) UserData.ResourceId.Diamond)
                {
                    StorageTotalRecharge.TotalRecharge += resData.count;
                    GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventTotalRechargeScore,StorageTotalRecharge.TotalRecharge.ToString(),StorageTotalRecharge.GruopId.ToString(),ActivityId);
                }
            }
        }
        public bool IsClaimed(int id)
        {
            return StorageTotalRecharge.CollectGroups.Contains(id);
        }

        public bool IsCanClaim(int id)
        {
            var config = GetTotalRechargeReward(id);
            if (config == null)
                return false;
            return StorageTotalRecharge.TotalRecharge >= config.Score && !IsClaimed(id);
        }

        public void Claim(int id)
        {
            var config = GetTotalRechargeReward(id);
            if (config == null)
                return ;
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventTotalRechargeReward,config.Id.ToString(),StorageTotalRecharge.GruopId.ToString());
            
            StorageTotalRecharge.CollectGroups.Add(id);
            var resDatas = CommonUtils.FormatReward(config.RewardId, config.RewardNum);
            PopReward(resDatas);
        }

        public bool IsHaveCanClaim()
        {
            var configs = TotalRechargeRewards();
            foreach (var cfg in configs)
            {
                if (IsCanClaim(cfg.Id))
                    return true;
            }
            return false;
        }
        
        public void PopReward(List<ResData> listResData)
        {
            if (listResData == null || listResData.Count <= 0)
                return;
            int count = listResData.Count > 8 ? 8 : listResData.Count;
            var list = listResData.GetRange(0, count);
            listResData.RemoveRange(0, count);
            var reasonArgs = new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.TotalRechargeRewardGet);
            CommonRewardManager.Instance.PopCommonReward(list, CurrencyGroupManager.Instance.GetCurrencyUseController(), true, reasonArgs, animEndCall:
                () =>
                {
                    PopReward(listResData);
                    EventDispatcher.Instance.DispatchEvent(EventEnum.BUTTERFLY_WORKSHOP_PURCHASE);
                });
        }
        public bool IsOpen()
        {
            if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.TotalRecharge))
                return false;
            if (!IsOpened())
                return false;
            if (TotalRechargeModel_New.Instance.IsOpen())
                return false;
            if (StorageTotalRecharge.IsOpenNewbie == false && TotalRechargeModel_New.Instance.StorageTotalRechargeNew.JoinTime<=0)
                return false;
            return true;
        }
        static bool IsActivityStorageEnd(StorageTotalRecharge storage)
        {
            return (long)APIManager.Instance.GetServerTime() >= storage.ActivityEndTime;
        }
        public static void RemoveUselessStorage()
        {
            List<string> keys = new List<string>(StorageManager.Instance.GetStorage<StorageHome>().TotalRecharges.Keys);
            for (int i = keys.Count - 1; i >= 0; i--)
            {
                var storage = StorageManager.Instance.GetStorage<StorageHome>().TotalRecharges[keys[i]];
                if (IsActivityStorageEnd(storage) && storage.IsFinish)
                {
                    StorageManager.Instance.GetStorage<StorageHome>().SummerWatermelon.Remove(keys[i]);
                }
            }
        }
        public static List<ResData> GetAllUnCollectRewards()
        {
            var unCollectRewardsList = new List<ResData>();
            List<string> keys = new List<string>(StorageManager.Instance.GetStorage<StorageHome>().TotalRecharges.Keys);
            for (int i = keys.Count - 1; i >= 0; i--)
            {
                var storage = StorageManager.Instance.GetStorage<StorageHome>().TotalRecharges[keys[i]];
                if (!storage.IsFinish &&IsActivityStorageEnd(storage))
                {
                    for (int j = 0; j < storage.Configs.Count; j++)
                    {
                        var config = storage.Configs[j];
                        if (storage.TotalRecharge >= config.Store && !storage.CollectGroups.Contains(config.Ids))
                        {
                            for (int k = 0; k < config.RewardIds.Count; k++)
                            {
                                unCollectRewardsList.Add(new ResData(config.RewardIds[k],config.RewardCounts[k]));
                            }
                        }
                    }
                    storage.IsFinish = true;
                }
            }

            return unCollectRewardsList;
        }

        public static bool CanShowUnCollectRewardsUI()
        {
            if (!APIManager.Instance.HasNetwork)
                return false;
            var allUnCollectRewards = GetAllUnCollectRewards();
            if (allUnCollectRewards.Count > 0)
            {
                var reasonArgs = new GameBIManager.ItemChangeReasonArgs(){reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.SummerGet};
                var unCollectRewards = allUnCollectRewards;
                CommonRewardManager.Instance.PopActivityUnCollectReward(unCollectRewards, reasonArgs, null, () =>
                {
                    foreach (var reward in unCollectRewards)
                    {
                        if (!UserData.Instance.IsResource(reward.id))
                        {
                            GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs()
                            {
                                MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonSummerGet,
                                itemAId = reward.id,
                                isChange = true,
                            });
                        }
                    }
                    
                    RemoveUselessStorage();
                });
                return true;
            }
            return false;
        }
        private static string constPlaceId = "totalRecharge";
        public static bool CanShowUI()
        {
            if (CanShowUnCollectRewardsUI())
                return true;
            if (!Instance.IsOpen())
                return false;
            if (CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, constPlaceId))
                return false;
        
            CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, constPlaceId,CommonUtils.GetTimeStamp());
            UIManager.Instance.OpenUI(UINameConst.UIPopupTotalRecharge);
            return true;
        }
        
    }
}