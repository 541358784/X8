using System;
using System.Collections.Generic;
using Activity.Base;
using DragonPlus;
using DragonPlus.Config.JumpGrid;
using UnityEngine;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;

namespace Activity.JumpGrid
{
    public class JumpGridModel : ActivityEntityBase, I_ActivityStatus
    {
        public bool ShowEntrance()
        {
            return IsOpened() && IsShowStart() && !StorageJumpGrid.IsShowEndView;
        }

        private static JumpGridModel _instance;
        public static JumpGridModel Instance => _instance ?? (_instance = new JumpGridModel());

        public StorageCoinCompetition StorageJumpGrid
        {
            get
            {
                return StorageManager.Instance.GetStorage<StorageHome>().JumpGrid;
                ;
            }
        }

        public override string Guid => "OPS_EVENT_TYPE_JUMP_GRID";

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
            JumpGridConfigManager.Instance.InitConfig(configJson);
            InitServerDataFinish();
            IsInited = true;
        }

        public bool GetIsInit()
        {
            return IsInited;
        }

        public override void UpdateActivityState()
        {
            InitServerDataFinish();
        }

        public int PayLevelGroup()
        {
            return StorageJumpGrid.PayLevelGroup;
        }
        
        protected override void InitServerDataFinish()
        {
            if(ActivityId.IsEmptyString())
                return;
            
            if (StorageJumpGrid.ActivityId != ActivityId)
            {
                StorageJumpGrid.Clear();
                StorageJumpGrid.ActivityId = ActivityId;
                StorageJumpGrid.PayLevelGroup = PayLevelModel.Instance.GetCurPayLevelConfig().JumpGridGroupId;
            }
            StorageJumpGrid.StartActivityTime = (long)StartTime;
            StorageJumpGrid.ActivityEndTime = (long)EndTime;
        }

        public bool IsOpened()
        {
            if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.JumpGrid))
                return false;
            bool isOpen = base.IsOpened();
            if (!isOpen)
                return false;

            return true;
        }

        public bool IsStart()
        {
            if (!IsOpened())
                return false;

            if (!StorageJumpGrid.IsShowStartView)
                return false;

            return true;
        }

        // 活动剩余预热时间的字符串显示
        public virtual string GetActivityPreheatLeftTimeString()
        {
            return CommonUtils.FormatLongToTimeStr((long)GetActivityPreheatLeftTime());
        }

        public ulong GetActivityPreheatLeftTime()
        {
            var config = JumpGridConfigManager.Instance.GetSettingConfig();
            if (config == null)
                return 0;
            var left = (ulong)config.PreheatTime * 3600 * 1000 - (APIManager.Instance.GetServerTime() - StartTime);
            if (left < 0)
                left = 0;
            return left;
        }

        public string GetActivityLeftTimeString()
        {
            return CommonUtils.FormatLongToTimeStr((long)GetActivityLeftTime());
        }


        public bool IsShowStart()
        {
            return StorageJumpGrid.IsShowStartView;
        }

        public void AddScore(int score)
        {
            StorageJumpGrid.TotalScore += score;
            EventDispatcher.Instance.DispatchEventImmediately(EventEnum.JUMP_GRID_ADD_COIN, score);

            var configs = JumpGridConfigManager.Instance.GetRewards();
            if (configs != null)
            {
                foreach (var config in configs)
                {
                    if (StorageJumpGrid.TotalScore >= config.Score &&
                        !StorageJumpGrid.CollectRewardsLevelList.ContainsKey(config.Id))
                    {
                        StorageJumpGrid.CollectRewardsLevelList.Add(config.Id, true);
                        var rewardList = CommonUtils.FormatReward(config.RewardId, config.RewardNum);
                        foreach (var finialReward in rewardList)
                        {
                            if (!StorageJumpGrid.UnCollectRewards.ContainsKey(finialReward.id))
                            {
                                StorageJumpGrid.UnCollectRewards.Add(finialReward.id, 0);
                            }

                            StorageJumpGrid.UnCollectRewards[finialReward.id] += finialReward.count;
                        }
                    }
                }
            }
        }

        public int GetScore()
        {
            return StorageJumpGrid.TotalScore;
        }

        public bool IsClaimed(int rID)
        {
            return StorageJumpGrid.Reward.ContainsKey(rID);
        }

        public void RecordExchange(TableJumpGridReward rewardConfig)
        {
            var rID = rewardConfig.Id;
            StorageJumpGrid.CurIndex = rID;
            if (!StorageJumpGrid.Reward.ContainsKey(rID))
                StorageJumpGrid.Reward.Add(rID, true);
            if (StorageJumpGrid.CollectRewardsLevelList.ContainsKey(rID))
            {
                var rewardList = CommonUtils.FormatReward(rewardConfig.RewardId, rewardConfig.RewardNum);
                foreach (var reward in rewardList)
                {
                    if (StorageJumpGrid.UnCollectRewards.ContainsKey(reward.id))
                    {
                        StorageJumpGrid.UnCollectRewards[reward.id] -= reward.count;
                        if (StorageJumpGrid.UnCollectRewards[reward.id] <= 0)
                        {
                            StorageJumpGrid.UnCollectRewards.Remove(reward.id);
                        }
                    }
                }
            }
        }

        public bool IsCanClaim()
        {
            var configs = JumpGridConfigManager.Instance.GetRewards();
            if (configs == null)
                return false;
            foreach (var config in configs)
            {
                if (StorageJumpGrid.TotalScore >= config.Score && !StorageJumpGrid.Reward.ContainsKey(config.Id))
                    return true;
            }

            return false;
        }

        public TableJumpGridReward GetFontReward()
        {
            var rewards = JumpGridConfigManager.Instance.GetRewards();
            var currentReward = GetCurrentReward();
            if (currentReward == null)
                return null;
            if (currentReward.Id == 1)
                return null;
            return rewards[currentReward.Id - 2];
        }

        public TableJumpGridReward GetCurrentReward()
        {
            var rewards = JumpGridConfigManager.Instance.GetRewards();
            if (rewards == null || rewards.Count <= 0)
                return null;
            for (int i = 0; i < rewards.Count; i++)
            {
                if (!StorageJumpGrid.Reward.ContainsKey(rewards[i].Id))
                    return rewards[i];
            }

            return rewards[rewards.Count - 1];
        }

        public TableJumpGridReward GetNextReward()
        {
            var rewards = JumpGridConfigManager.Instance.GetRewards();
            var currentReward = GetCurrentReward();
            if (currentReward == null)
                return null;
            if (currentReward.Id == rewards.Count)
                return null;
            return rewards[currentReward.Id];
        }

        public int GetLevelHaveStore(int level)
        {
            var font = GetRewardStoreByLevel(level - 1);
            return Mathf.Max(0, StorageJumpGrid.TotalScore - font);
        }

        public int GetLevelNeedStore(int level)
        {
            var cur = GetRewardStoreByLevel(level);
            var font = GetRewardStoreByLevel(level - 1);
            return cur - font;
        }

        public int GetRewardStoreByLevel(int level)
        {
            var rewards = JumpGridConfigManager.Instance.GetRewards();
            if (level < 1)
                return 0;
            if (level > rewards.Count)
                level = rewards.Count;
            return rewards[level - 1].Score;
        }

        public TableJumpGridReward GetRewardConfigByLevel(int level)
        {
            var rewards = JumpGridConfigManager.Instance.GetRewards();
            if (level < 1)
                level = 1;
            if (level > rewards.Count)
                level = rewards.Count;
            return rewards[level - 1];
        }

        public float GetCurrentProgress()
        {
            var cur = GetCurrentReward();
            if (cur == null)
                return 0;
            var font = GetFontReward();
            int fontStore = font == null ? 0 : font.Score;

            return (float)(StorageJumpGrid.TotalScore - fontStore) / (cur.Score - fontStore);
        }

        public string GetCurrentProgressStr()
        {
            var cur = GetCurrentReward();
            if (cur == null)
                return "";
            var font = GetFontReward();
            int fontStore = font == null ? 0 : font.Score;

            return (StorageJumpGrid.TotalScore - fontStore) + "/" + (cur.Score - fontStore);
        }

        public void StartActivity()
        {
            StorageJumpGrid.IsShowStartView = true;
            SetActivityStatus(I_ActivityStatus.ActivityStatus.Incomplete);
        }

        public bool IsExchangeAll()
        {
            var rewards = JumpGridConfigManager.Instance.GetRewards();
            if (rewards != null && StorageJumpGrid.CurIndex >= JumpGridConfigManager.Instance.GetRewards().Count)
            {
                SetActivityStatus(I_ActivityStatus.ActivityStatus.Completed);
                return true;
            }
            
            return false;
        }

        public bool IsPreheating()
        {
            if (IsSkipActivityPreheating())
                return false;
            ulong serverTime = APIManager.Instance.GetServerTime();
            var config = JumpGridConfigManager.Instance.GetSettingConfig();
            if (config == null)
                return false;
            if (serverTime - StartTime <= (ulong)config.PreheatTime * 3600 * 1000)
                return true;

            return false;
        }

        public void Claim(Action cb, bool isInMain = false)
        {
            List<ResData> list = new List<ResData>();
            var currentReward = GetCurrentReward();
            if (StorageJumpGrid.TotalScore < currentReward.Score)
                return;
            RecordExchange(currentReward);
            EventDispatcher.Instance.DispatchEventImmediately(EventEnum.JUMP_GRID_REFRESH);
            
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventJumpGridGet, currentReward.Id.ToString());


            for (int i = 0; i < currentReward.RewardId.Count; i++)
            {
                list.Add(new ResData(currentReward.RewardId[i], currentReward.RewardNum[i]));
                var mergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonJumpGridGet;
                if (!UserData.Instance.IsResource(currentReward.RewardId[i]))
                {
                    var itemConfig = GameConfigManager.Instance.GetItemConfig(currentReward.RewardId[i]);
                    if (itemConfig != null)
                    {
                        GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                        {
                            MergeEventType = mergeEventType,
                            itemAId = itemConfig.id,
                            isChange = true,
                        });
                    }
                }
            }

            var reason =  BiEventAdventureIslandMerge.Types.ItemChangeReason.JumpGridGet;
            CommonRewardManager.Instance.PopCommonReward(list,
                CurrencyGroupManager.Instance.GetCurrencyUseController(), true, new GameBIManager.ItemChangeReasonArgs()
                {
                    reason = reason,
                }
                , () =>
                {
                    if (isInMain)
                        cb?.Invoke();
                    else
                    {
                        UIManager.Instance.OpenUI(UINameConst.UIJumpGridMain);
                    }
                });
        }

        private static string Preheating = "JumpGridPreheating";
        private static string coolTimeKey = "JumpGridCD";

        public static bool CanShowUI()
        {
            if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.JumpGrid))
                return false;

            if (!JumpGridModel.Instance.IsOpened())
                return false;

            if (JumpGridModel.Instance.StorageJumpGrid.IsShowEndView)
                return false;
            
            if (!CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, Preheating))
            {
                if (JumpGridModel.Instance.IsPreheating())
                {
                    UIManager.Instance.OpenUI(UINameConst.UIPopupJumpGridStart);
                    CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, Preheating, CommonUtils.GetTimeStamp());
                    return true;
                }
            }

            if (!CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, coolTimeKey))
            {
                if (!JumpGridModel.Instance.IsPreheating())
                {
                    if (!JumpGridModel.Instance.IsShowStart())
                    {
                        UIManager.Instance.OpenUI(UINameConst.UIPopupJumpGridStart);
                        CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey, CommonUtils.GetTimeStamp());
                        return true;
                    }
                    else
                    {
                        CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey, CommonUtils.GetTimeStamp());
                        UIManager.Instance.OpenUI(UINameConst.UIJumpGridMain);
                        return true;
                    }
                }
            }

            return false;
        }

        public void Clear()
        {
            StorageJumpGrid.Clear();
            CoolingTimeManager.Instance.RemoveCooling(CoolingTimeManager.CDType.OtherDay, coolTimeKey);
            CoolingTimeManager.Instance.RemoveCooling(CoolingTimeManager.CDType.OtherDay, Preheating);
        }

        public override bool CanDownLoadRes()
        {
            return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.JumpGrid);
        }

        #region UnCollectRewards

        public List<ResData> GetUnCollectRewards()
        {
            var unCollectRewardsList = new List<ResData>();
            var storage = StorageManager.Instance.GetStorage<StorageHome>().JumpGrid;
            if (IsActivityEnd(storage))
            {
                foreach (var pair in storage.UnCollectRewards)
                {
                    if (pair.Value > 0)
                    {
                        unCollectRewardsList.Add(new ResData(pair.Key, pair.Value));
                    }
                }
            }

            return unCollectRewardsList;
        }

        static bool IsActivityEnd(StorageCoinCompetition storageCoinCompetition)
        {
            return (long)APIManager.Instance.GetServerTime() >= storageCoinCompetition.ActivityEndTime ||
                   (long)APIManager.Instance.GetServerTime() < storageCoinCompetition.StartActivityTime;
        }

        public static bool CanShowUnCollectRewardsUI()
        {
            var unCollectRewards = Instance.GetUnCollectRewards();
            if (unCollectRewards == null || Instance.GetUnCollectRewards().Count == 0)
                return false;
            
            var reasonArgs = new GameBIManager.ItemChangeReasonArgs() { reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.JumpGridGet };
            CommonRewardManager.Instance.PopActivityUnCollectReward(unCollectRewards, reasonArgs, null, () =>
            {
                for (int i = 0; i < unCollectRewards.Count; i++)
                {
                    if (!UserData.Instance.IsResource(unCollectRewards[i].id))
                    {
                        GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                        {
                            MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonJumpGridGet,
                            isChange = false,
                            itemAId = unCollectRewards[i].id
                        });
                    }
                }
                var storage = StorageManager.Instance.GetStorage<StorageHome>().JumpGrid;
                if (IsActivityEnd(storage))
                {
                    storage.UnCollectRewards.Clear();
                }
            });

            return true;
        }

        #endregion

        public I_ActivityStatus.ActivityStatus GetActivityStatus()
        {
            return (I_ActivityStatus.ActivityStatus)StorageJumpGrid.ActivityStatus;
        }

        public void SetActivityStatus(I_ActivityStatus.ActivityStatus status)
        {
            StorageJumpGrid.ActivityStatus = (int)status;
        }
    }
}