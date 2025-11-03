using System;
using System.Collections.Generic;
using System.Linq;
using Dlugin;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Framework;
using Gameplay;
using Gameplay.UI;
using Newtonsoft.Json;
using UnityEngine;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

namespace Activity.BattlePass_2
{
    public partial class BattlePassModel : ActivityEntityBase
    {
        private static BattlePassModel _instance;
        public static BattlePassModel Instance => _instance ?? (_instance = new BattlePassModel());

        public override string Guid => "OPS_EVENT_TYPE_BATTLEPASS_2";

        private const string CONFIG_KEY_ACTIVE = "battlepassconfig";
        private const string CONFIG_KEY_REWARD = "battlepassreward";

        private List<TableBattlePassConfig> battlePassActiveConfig = null;
        private List<TableBattlePassReward> battlePassRewardConfig = null;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InitAuto()
        {
            Instance.Init();
        }

        private int totalScoreRatio = 0;

        public int TotalScoreRatio
        {
            get { return totalScoreRatio; }
            private set { totalScoreRatio = value; }
        }

        private int curScoreRatio = 0;

        public int CurScoreRatio
        {
            get { return curScoreRatio; }
            private set
            {
                curScoreRatio = value;
                if (storageBattlePass == null)
                    return;
                storageBattlePass.ActivityScore = curScoreRatio;
            }
        }

        public int PayLevelGroup()
        {
            if (storageBattlePass == null)
                return 0;

            return storageBattlePass.PayLevelGroup;
        }

        public TableBattlePassConfig BattlePassActiveConfig
        {
            get
            {
                if (battlePassActiveConfig == null || battlePassActiveConfig.Count == 0)
                    return null;

                return battlePassActiveConfig[0];
            }
        }

        public TableBattlePassShopConfig BattlePassShopConfig
        {
            get
            {
                return BattlePassConfigManager.Instance.GetShopConfig();
            }
        }

        public List<TableBattlePassReward> BattlePassRewardConfig
        {
            get { return battlePassRewardConfig; }
        }

        public StorageBattlePass storageBattlePass = null;

        public bool isActivityEndBuy = false;

        public override void InitFromServerData(string activityId, string activityType, ulong startTime,
            ulong endTime, ulong rewardEndTime, bool manualEnd, string configJson,
            string activitySubType)
        {
            base.InitFromServerData(activityId, activityType, startTime, endTime, rewardEndTime, manualEnd, configJson,
                activitySubType);

            InitConfig(configJson);
            InitBattlePass();
            DeepCopyConfig();
            InitTotalScoreRatio();

            BattlePassTaskModel.Instance.InitTask();
        }

        public void InitConfig(string config)
        {
            if (String.IsNullOrEmpty(config))
            {
                DragonU3DSDK.DebugUtil.LogError("SummerChallengeConfig activity config error!");
                return;
            }

            BattlePassConfigManager.Instance.InitFromServerData(config);
            battlePassActiveConfig = BattlePassConfigManager.Instance._battlePassConfigs;
            battlePassRewardConfig = BattlePassConfigManager.Instance.GetRewardConfig();
        }

        public bool IsOpened()
        {
            if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.BattlePass))
                return false;

            bool isOpen = base.IsOpened();
            if (!isOpen)
                return false;

            return true;
        }

        private void DeepCopyConfig()
        {
            if (storageBattlePass == null)
                return;

            if (storageBattlePass.Reward == null || storageBattlePass.Reward.Count == 0)
            {
                for (int i = 0; i < battlePassRewardConfig.Count; i++)
                {
                    StorageBattlePassRewardConfig config = new StorageBattlePassRewardConfig();
                    TableBattlePassReward copyConfig = battlePassRewardConfig[i];

                    config.Id = copyConfig.id;
                    foreach (var id in copyConfig.keyRewardId)
                    {
                        config.PurchaseRewardIds.Add(id);
                    }

                    foreach (var id in copyConfig.keyRewardNum)
                    {
                        config.PurchaseRewardCounts.Add(id);
                    }

                    foreach (var id in copyConfig.normalRewardId)
                    {
                        config.NormalRewardIds.Add(id);
                    }

                    foreach (var id in copyConfig.normalRewardNum)
                    {
                        config.NormalRewardCounts.Add(id);
                    }

                    config.UnlockScore = copyConfig.exchangeItemNum;

                    storageBattlePass.Reward.Add(config);
                }
            }

            storageBattlePass.StartTime = (long)StartTime;
            storageBattlePass.EndTime = (long)EndTime;
            storageBattlePass.ManualEnd = ManualEnd;
            storageBattlePass.ShopId = BattlePassActiveConfig.shopItemId;
        }

        private void InitBattlePass()
        {
            if (StorageManager.Instance.GetStorage<StorageHome>().BattlePass2.ContainsKey(StorageKey))
            {
                storageBattlePass = StorageManager.Instance.GetStorage<StorageHome>().BattlePass2[StorageKey];
                if (storageBattlePass.LoopRewardConfigToJson.IsEmptyString())
                {
                    storageBattlePass.LoopRewardConfigToJson = JsonConvert.SerializeObject(BattlePassConfigManager.Instance._battlePassLoopRewardConfigs);
                }

                if (storageBattlePass.LoopRewardScore == 0)
                {
                    storageBattlePass.LoopRewardScore = BattlePassActiveConfig.loopRewardScore;
                }

                if (storageBattlePass.LoopRewardList.Count == 0)
                {
                    foreach (var loopRewardId in BattlePassActiveConfig.loopRewardList)
                    {
                        storageBattlePass.LoopRewardList.Add(loopRewardId);
                    }
                }
            }
            else
            {
                storageBattlePass = new StorageBattlePass();
                storageBattlePass.LoopRewardScore = BattlePassActiveConfig.loopRewardScore;
                foreach (var loopRewardId in BattlePassActiveConfig.loopRewardList)
                {
                    storageBattlePass.LoopRewardList.Add(loopRewardId);
                }
                storageBattlePass.LoopRewardCollectTimes = 0;
                storageBattlePass.PayLevelGroup = PayLevelModel.Instance.GetCurPayLevelConfig().BattlePass2GroupId;
                storageBattlePass.LoopRewardConfigToJson = JsonConvert.SerializeObject(BattlePassConfigManager.Instance._battlePassLoopRewardConfigs);
                StorageManager.Instance.GetStorage<StorageHome>().BattlePass2.Add(StorageKey, storageBattlePass);
            }

            CurScoreRatio = storageBattlePass.ActivityScore;
        }

        /// <summary>
        /// 是否在购买延期时间 
        /// </summary>
        /// <returns></returns>
        public bool IsInBuyExtraDay()
        {
            var left = (long)EndTime - (long)APIManager.Instance.GetServerTime() - BattlePassActiveConfig.extraDays * 24 * 60 * 60 * 1000;
            if (left > 0 && left < BattlePassActiveConfig.extraShowDays * 24 * 60 * 60 * 1000)
                return true;
            return false;
        }

        public ulong GetActivityLeftTime()
        {
            long extra = 0;

            var left = (long)storageBattlePass.EndTime - (long)APIManager.Instance.GetServerTime();
            if (left < 0)
                left = 0;
            return (ulong)left;
        }

        // 活动剩余时间的字符串显示
        public virtual string GetActivityLeftTimeString()
        {
            return CommonUtils.FormatLongToTimeStr((long)GetActivityLeftTime());
        }

        public override bool CanDownLoadRes()
        {
            return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.BattlePass);
        }

        public int GetNoProductCount()
        {
            if (storageBattlePass == null)
                return 0;
            return storageBattlePass.NoProductCount;
        }

        public void ReSetNoProductCount()
        {
            if (storageBattlePass == null)
                return;
            storageBattlePass.NoProductCount = 0;
        }

        public void AddNoProductCount()
        {
            if (storageBattlePass == null)
                return;
            storageBattlePass.NoProductCount++;
        }


        public bool IsPurchase()
        {
            if (storageBattlePass == null)
                return false;

            return storageBattlePass.IsPurchase;
        }

        public bool IsLocalPurchasing()
        {
            bool isPurchase = false;
            foreach (var kv in StorageManager.Instance.GetStorage<StorageHome>().BattlePass2)
            {
                StorageBattlePass data = kv.Value;

                if (BattlePassModel.Instance.IsLocalActivityEnd(data))
                    continue;

                if(!isPurchase)
                    isPurchase = data.IsPurchase;
            }

            return isPurchase;
        }
        
        public bool IsLocalActivityEnd(StorageBattlePass data)
        {
            bool endTimeOk = APIManager.Instance.GetServerTime() > (ulong)data.EndTime;

            if (endTimeOk || data.ManualEnd)
                return true;

            return false;
        }
        
        public void AddScore(int score)
        {
            if (!IsOpened())
                return;

            CurScoreRatio += score;
        }

        public int MultipleScore(int score)
        {
            if (!IsOpened())
                return score;

            return score + (int)(storageBattlePass.ScoreMultiple / 100f * score);
        }
        public void DebugSetScore(int score)
        {
            if (!IsOpened())
                return;

            CurScoreRatio = score;
        }

        public void DebugReset()
        {
            if (storageBattlePass == null)
                return;
            CurScoreRatio = 0;
            storageBattlePass.IsPurchase = false;
            for (int i = 0; i < storageBattlePass.Reward.Count; i++)
            {
                storageBattlePass.Reward[i].IsNormalGet = false;
                storageBattlePass.Reward[i].IsPurchaseGet = false;
            }
        }

        public void ClearActivityData()
        {
            DebugReset();
            StorageManager.Instance.GetStorage<StorageHome>().BattlePass2.Clear();
        }

        public TableBattlePassReward GetRewardConfig(int score, out bool isEnd)
        {
            isEnd = false;
            if (battlePassRewardConfig == null || battlePassRewardConfig.Count == 0)
                return null;

            for (int i = 0; i < battlePassRewardConfig.Count; i++)
            {
                if (score < battlePassRewardConfig[i].exchangeItemNum)
                    return battlePassRewardConfig[i];
            }

            isEnd = true;
            return null;
        }

        public bool IsMaxLevel()
        {
            if (battlePassRewardConfig == null || battlePassRewardConfig.Count == 0)
                return false;

            for (int i = 0; i < battlePassRewardConfig.Count; i++)
            {
                if (curScoreRatio < battlePassRewardConfig[i].exchangeItemNum)
                    return false;
            }

            return true;
        }

        public TableBattlePassReward GetRewardConfig(int id)
        {
            if (battlePassRewardConfig == null || battlePassRewardConfig.Count == 0)
                return null;

            for (int i = 0; i < battlePassRewardConfig.Count; i++)
            {
                if (id == battlePassRewardConfig[i].id)
                    return battlePassRewardConfig[i];
            }


            return null;
        }

        public void InitTotalScoreRatio()
        {
            TotalScoreRatio = 0;
            if (battlePassRewardConfig == null || battlePassRewardConfig.Count == 0)
                return;

            TotalScoreRatio = battlePassRewardConfig[battlePassRewardConfig.Count - 1].exchangeItemNum;
        }

        public TableBattlePassReward GetNowRewardConfig()
        {
            if (battlePassRewardConfig == null || battlePassRewardConfig.Count == 0)
                return null;

            TableBattlePassReward config = GetRewardConfig(CurScoreRatio, out var isEnd);
            if (config == null)
                config = battlePassRewardConfig[battlePassRewardConfig.Count - 1];


            return config;
        }

        public TableBattlePassReward GetNextRewardConfig(int score = 0)
        {
            if (battlePassRewardConfig == null || battlePassRewardConfig.Count == 0)
                return null;

            int ScoreRatio = score > 0 ? score : CurScoreRatio;
            TableBattlePassReward config = GetRewardConfig(ScoreRatio, out var isEnd);
            if (config == null)
                return null;

            int index = battlePassRewardConfig.FindIndex(a => a.id == config.id);
            if (index < 0)
                return null;

            if (index + 1 >= battlePassRewardConfig.Count)
                return null;

            return battlePassRewardConfig[index + 1];
        }

        public TableBattlePassReward GetFrontRewardConfig(int score = -1)
        {
            if (battlePassRewardConfig == null || battlePassRewardConfig.Count == 0)
                return null;
            int ScoreRatio = score >= 0 ? score : CurScoreRatio;
            TableBattlePassReward config = GetRewardConfig(ScoreRatio, out var isEnd);
            if (config == null)
                return null;

            int index = battlePassRewardConfig.FindIndex(a => a.id == config.id);
            if (index < 0)
                return null;

            if (index < 1)
                return null;

            return battlePassRewardConfig[index - 1];
        }

        public bool IsGetReward(int id, bool isNormal)
        {
            if (storageBattlePass == null)
                return false;

            StorageBattlePassRewardConfig storageReward = GetStorageReward(id);
            if (storageReward == null)
                return false;

            return isNormal ? storageReward.IsNormalGet : storageReward.IsPurchaseGet;
        }

        public bool CanGetReward()
        {
            if (storageBattlePass == null)
                return false;

            for (int i = 0; i < storageBattlePass.Reward.Count; i++)
            {
                if (storageBattlePass.ActivityScore < storageBattlePass.Reward[i].UnlockScore)
                    continue;

                if (!storageBattlePass.Reward[i].IsNormalGet)
                    return true;

                if (storageBattlePass.IsPurchase && !storageBattlePass.Reward[i].IsPurchaseGet)
                    return true;
            }

            return false;
        }

        public int CanGetRewardCount()
        {
            if (storageBattlePass == null)
                return 0;
            int count = 0;
            for (int i = 0; i < storageBattlePass.Reward.Count; i++)
            {
                if (storageBattlePass.ActivityScore < storageBattlePass.Reward[i].UnlockScore)
                    continue;

                if (!storageBattlePass.Reward[i].IsNormalGet)
                    count++;

                if (storageBattlePass.IsPurchase && !storageBattlePass.Reward[i].IsPurchaseGet)
                    count++;
            }

            if (storageBattlePass.LoopRewardIsOpened() && storageBattlePass.ActivityScore >= storageBattlePass.GetNextLoopRewardScore())
            {
                count+=(storageBattlePass.ActivityScore - storageBattlePass.GetFrontLoopRewardScore())/storageBattlePass.LoopRewardScore;
            }

            return count;
        }

        public int GetActivityLevel()
        {
            if (storageBattlePass == null)
                return 0;
            
            int level = 0;
            for (int i = 0; i < storageBattlePass.Reward.Count; i++)
            {
                if (storageBattlePass.ActivityScore < storageBattlePass.Reward[i].UnlockScore)
                    break;
                
                level++;
            }

            return level;
        }

        public StorageBattlePassRewardConfig GetStorageReward(int id)
        {
            if (storageBattlePass == null)
                return null;

            for (int i = 0; i < storageBattlePass.Reward.Count; i++)
            {
                if (storageBattlePass.Reward[i].Id != id)
                    continue;
                return storageBattlePass.Reward[i];
            }

            return null;
        }

        public void GetReward(int id, bool isNormal)
        {
            if (storageBattlePass == null)
                return;

            if (IsGetReward(id, isNormal))
                return;

            StorageBattlePassRewardConfig storageReward = GetStorageReward(id);
            if (storageReward == null)
                return;

            if (isNormal)
            {
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventBpReward, "1", storageReward.Id.ToString());
                storageReward.IsNormalGet = true;
            }
            else
            {
                storageReward.IsPurchaseGet = true;

                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventBpReward, "2", storageReward.Id.ToString());
            }
        }

        public StorageBattlePass HaveActivityEnd()
        {
            foreach (var kv in StorageManager.Instance.GetStorage<StorageHome>().BattlePass2)
            {
                StorageBattlePass data = kv.Value;

                if (!IsActivityEnd(data))
                    continue;

                return data;
            }

            return null;
        }

        public bool IsActivityEnd(StorageBattlePass data)
        {
            if (data.IsGetAllReward)
                return false;

            bool endTimeOk = APIManager.Instance.GetServerTime() > (ulong)data.EndTime;

            if (endTimeOk || data.ManualEnd)
                return true;

            return false;
        }

        public bool IsCanGetAllReward(StorageBattlePass config)
        {
            if (config == null)
                return false;

            if (!IsActivityEnd(config))
                return false;

            if (config.IsGetAllReward)
                return false;
            if (!config.IsShowStart)
                return false;

            foreach (var kv in config.Reward)
            {
                if (config.ActivityScore < kv.UnlockScore)
                    continue;

                if (!kv.IsNormalGet)
                    return true;

                if (config.IsPurchase && !kv.IsPurchaseGet)
                    return true;
            }

            return false;
        }

        private static List<string> ResList = new List<string>()
        {
            "prefabs/activity/bptwo.ab",
            "spriteatlas/activityatlas/bptwoatlas/hd.ab",
            "spriteatlas/activityatlas/bptwoatlas/sd.ab",
        };

        public static bool ResExist()
        {
            return ActivityManager.Instance.CheckResExist(ResList);
        }

        public static bool CheckActivityEnd()
        {
            List<string> removeActivityKey = new List<string>();
            foreach (var kv in StorageManager.Instance.GetStorage<StorageHome>().BattlePass2)
            {
                StorageBattlePass data = kv.Value;

                if (BattlePassModel.Instance.IsActivityEnd(data))
                    continue;

                if(!data.IsGetAllReward)
                    continue;
                
                removeActivityKey.Add(kv.Key);
            }
            foreach (var key in removeActivityKey)
            {
                StorageManager.Instance.GetStorage<StorageHome>().BattlePass2.Remove(key);
                DebugUtil.LogError("-----------bp2 活动到期且领取所有奖励 删除 " + key);
            }
            
            StorageBattlePass storageBattlePass = BattlePassModel.Instance.GetRewardData();
            if (storageBattlePass != null && ResExist())
            {
                if (StorySubSystem.Instance.IsShowing)
                    return false;
                if (UnlockManager.IsOpen(UnlockManager.MergeUnlockType.BattlePass))
                {
                    UIManager.Instance.OpenUI(UINameConst.UIPopupBattlePass2End, storageBattlePass);
                    return true;
                }
            }

            return false;
        }

        public StorageBattlePass GetRewardData()
        {
            int loopCount = 0;
            int maxLoopCount = StorageManager.Instance.GetStorage<StorageHome>().BattlePass2.Count;
            while (loopCount < maxLoopCount)
            {
                StorageBattlePass battlePass = HaveActivityEnd();
                if (battlePass == null)
                    return null;

                if (IsCanGetAllReward(battlePass) || (!battlePass.IsShowEnd && battlePass.IsShowStart))
                {
                    return battlePass;
                }
                else
                {
                    battlePass.IsGetAllReward = true;
                }

                loopCount++;
            }

            return null;
        }

        public void PurchaseSuccess(TableShop config)
        {
            List<string> keys = StorageManager.Instance.GetStorage<StorageHome>().BattlePass2.Keys.ToList();
            if (keys == null || keys.Count == 0)
                return;

            if (isActivityEndBuy)
            {
                for (int i = keys.Count - 1; i >= 0; i--)
                {
                    string key = keys[i];
                    StorageBattlePass storageBattlePass =
                        StorageManager.Instance.GetStorage<StorageHome>().BattlePass2[key];
                    if (storageBattlePass.IsPurchase)
                        continue;

                    if (IsActivityEnd(storageBattlePass))
                    {
                        storageBattlePass.IsPurchase = true;
                        PopCommonReward(storageBattlePass, config, true);
                        EventDispatcher.Instance.DispatchEventImmediately(EventEnum.BATTLE_PASS_2_PURCHASE, true);
                        break;
                    }
                }
            }
            else
            {
                StorageBattlePass storageBattlePass =
                    StorageManager.Instance.GetStorage<StorageHome>().BattlePass2[StorageKey];
                if(!storageBattlePass.IsPurchase)
                    storageBattlePass.PurchaseTime = (long)APIManager.Instance.GetServerTime();
                
                storageBattlePass.IsPurchase = true;
                if (BattlePassActiveConfig != null)
                {
                    bool isUltimatePurchase = config.id == BattlePassShopConfig.ultimateShopId;
                    if (isUltimatePurchase)
                    {
                        storageBattlePass.IsUltimatePurchase = true;
                        storageBattlePass.ScoreMultiple = BattlePassShopConfig.ultimateScoreMultiple;
                        storageBattlePass.BuyType = (int)BuyType.Ultimate;
                        int score = storageBattlePass.ActivityScore;
                        var curConfig = GetCurrentRewardConfig(score, out var isEnd);
                        if (!isEnd)
                        {
                            var rewardConfig = GetRewardConfig(score, BattlePassShopConfig.ultimateSkipStep,out var leftStep);
                            if (rewardConfig != null)
                            {
                                score = rewardConfig.exchangeItemNum - score;
                                storageBattlePass.ActivityScore = rewardConfig.exchangeItemNum + leftStep*storageBattlePass.LoopRewardScore;
                                CurScoreRatio = storageBattlePass.ActivityScore;
                                EventDispatcher.Instance.DispatchEventImmediately(EventEnum.BATTLE_PASS_2_PURCHASE, true);
                            }   
                        }
                        else
                        {
                            storageBattlePass.ActivityScore += BattlePassShopConfig.ultimateSkipStep *
                                                               storageBattlePass.LoopRewardScore;
                            CurScoreRatio = storageBattlePass.ActivityScore;
                            EventDispatcher.Instance.DispatchEventImmediately(EventEnum.BATTLE_PASS_2_PURCHASE, true);
                        }
                    }
                    else
                    {
                        int[] rewardType;
                        int[] rewardNum;
                        if (IsOldUser())
                        {
                            int index = 0;
                            foreach (var shopId in BattlePassShopConfig.oldUserShopIds)
                            {
                              if(shopId == config.id)   
                                  break;

                              index++;
                            }

                            storageBattlePass.BuyType = index;
                            switch (index)
                            {
                                case 0:
                                {
                                    rewardType = BattlePassShopConfig.oldRewardType_1;
                                    rewardNum = BattlePassShopConfig.oldRewardNum_1;
                                    break;
                                }
                                case 1:
                                {
                                    rewardType = BattlePassShopConfig.oldRewardType_2;
                                    rewardNum = BattlePassShopConfig.oldRewardNum_2;
                                    break;
                                }
                                case 2:
                                {
                                    rewardType = BattlePassShopConfig.oldRewardType_3;
                                    rewardNum = BattlePassShopConfig.oldRewardNum_3;
                                    break;
                                }
                                default:
                                {
                                    rewardType = BattlePassShopConfig.oldRewardType_1;
                                    rewardNum = BattlePassShopConfig.oldRewardNum_1;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            int index = 0;
                            foreach (var shopId in BattlePassShopConfig.newUserShopIds)
                            {
                                if(shopId == config.id)   
                                    break;

                                index++;
                            }

                            storageBattlePass.BuyType = index+1;
                            switch (index)
                            {
                                case 0:
                                {
                                    rewardType = BattlePassShopConfig.newRewardType_1;
                                    rewardNum = BattlePassShopConfig.newRewardNum_1;
                                    break;
                                }
                                case 1:
                                {
                                    rewardType = BattlePassShopConfig.newRewardType_2;
                                    rewardNum = BattlePassShopConfig.newRewardNum_2;
                                    break;
                                }
                                default:
                                {
                                    rewardType = BattlePassShopConfig.newRewardType_1;
                                    rewardNum = BattlePassShopConfig.newRewardNum_1;
                                    break;
                                }
                            }
                        }
                        
                        var resDatas = new List<ResData>();
                        for (var i = 0; i < rewardType.Length; i++)
                        {
                            int type = rewardType[i];
                            int num = rewardNum[i];

                            if (type == 1)
                            {
                                storageBattlePass.ActivityScore += num;
                                CurScoreRatio = storageBattlePass.ActivityScore;
                                storageBattlePass.IsGoldPurchase = true;
                            }
                            else if (type == 2)
                            {
                                storageBattlePass.ScoreMultiple = num;
                            }
                            else
                            {
                                ResData res = new ResData(type, num);
                                resDatas.Add(res);
                            }
                        }
                        // bool isGoldPurchase = config.id == BattlePassActiveConfig.shopGoldId;
                        // storageBattlePass.ScoreMultiple =  isGoldPurchase ? BattlePassActiveConfig.goldScoreMultiple : 0;
                        // if (isGoldPurchase)
                        // {
                        //     storageBattlePass.ActivityScore += BattlePassActiveConfig.goldGetScore;
                        //     CurScoreRatio = storageBattlePass.ActivityScore;
                        //     storageBattlePass.IsGoldPurchase = true;
                        // }
                    
                        var reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.EasterPackageGet;
                        CommonRewardManager.Instance.PopCommonReward(resDatas,
                            CurrencyGroupManager.Instance.GetCurrencyUseController(), true, new GameBIManager.ItemChangeReasonArgs()
                            {
                                reason = reason,
                            }
                            , () => { EventDispatcher.Instance.DispatchEventImmediately(EventEnum.BATTLE_PASS_2_PURCHASE, true); });
                    }
                }
            }

            isActivityEndBuy = false;
            var existExtraView = UIPopupExtraView.CheckExtraViewOpenState<BattlePassExtraView>();
            if (existExtraView)
                GameBIManager.Instance.SendGameEvent(
                    BiEventAdventureIslandMerge.Types.GameEventType.GameEventBpbuyLifeUseup, data1: "1");
        }

        public TableBattlePassReward GetRewardConfig(int score, int step,out int leftStep)
        {
            leftStep = 0;
            if (battlePassRewardConfig == null || battlePassRewardConfig.Count == 0)
                return null;

            int ScoreRatio = score > 0 ? score : CurScoreRatio;
            TableBattlePassReward config = GetCurrentRewardConfig(ScoreRatio, out var isEnd);
            if (config == null)
                return null;

            int index = battlePassRewardConfig.FindIndex(a => a.id == config.id);
            if (index < 0)
                return null;

            bool isFull = score == config.exchangeItemNum;
            step = isFull ? step : step - 1;
            leftStep = step;
            if (index < battlePassRewardConfig.Count - 1)
            {
                for (int i = 0; i < step; i++)
                {
                    index++;
                    leftStep--;
                    if (index >= battlePassRewardConfig.Count - 1)
                        break;
                }   
            }

            return battlePassRewardConfig[index];
        }
        
        public TableBattlePassReward GetCurrentRewardConfig(int score, out bool isEnd)
        {
            isEnd = false;
            if (battlePassRewardConfig == null || battlePassRewardConfig.Count == 0)
                return null;

            for (int i = 0; i < battlePassRewardConfig.Count; i++)
            {
                if (score <= battlePassRewardConfig[i].exchangeItemNum)
                    return battlePassRewardConfig[i];
            }

            isEnd = true;
            return null;
        }
        
        public void PopCommonReward(StorageBattlePass storageBattlePass, TableShop config, bool isFirst = false)
        {
            var ret = new List<ResData>();
            if (isFirst)
                ret.Add(new ResData(201, 150));
            foreach (var kv in storageBattlePass.Reward)
            {
                if (storageBattlePass.ActivityScore < kv.UnlockScore)
                    continue;

                if (!kv.IsNormalGet)
                {
                    for (int i = 0; i < kv.NormalRewardIds.Count; i++)
                        ret.Add(new ResData(kv.NormalRewardIds[i], kv.NormalRewardCounts[i]));
                    kv.IsNormalGet = true;
                }

                if (ret.Count >= 8)
                    break;

                if (storageBattlePass.IsPurchase && !kv.IsPurchaseGet)
                {
                    for (int i = 0; i < kv.PurchaseRewardIds.Count; i++)
                        ret.Add(new ResData(kv.PurchaseRewardIds[i], kv.PurchaseRewardCounts[i]));
                    kv.IsPurchaseGet = true;
                }

                if (ret.Count >= 8)
                    break;
            }

            if (storageBattlePass.LoopRewardIsOpened())
            {
                while (storageBattlePass.ActivityScore >= storageBattlePass.GetNextLoopRewardScore())
                {
                    if (ret.Count >= 8)
                        break;
                    var rewards = storageBattlePass.GetCurLoopRewards();
                    storageBattlePass.LoopRewardCollectTimes++;
                    foreach (var reward in rewards)
                    {
                        ret.Add(reward);
                    }
                }   
            }
            if (ret.Count <= 0)
            {
                storageBattlePass.IsGetAllReward = true;
                return;
            }


            var reasonArgs =
                new GameBIManager.ItemChangeReasonArgs(BiEventCooking.Types.ItemChangeReason.BpEnd);
            EventDispatcher.Instance.DispatchEvent(EventEnum.PURCHASE_SUCCESS_REWARD, ret);
            CommonRewardManager.Instance.PopCommonReward(ret, CurrencyGroupManager.Instance.currencyController, true,
                reasonArgs, () => { PopCommonReward(storageBattlePass, config); });
            foreach (var res in ret)
            {
                if (!UserData.Instance.IsResource(res.id))
                {
                    var itemConfig = GameConfigManager.Instance.GetItemConfig(res.id);
                    if (itemConfig != null)
                    {
                        GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                        {
                            MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonBpEnd,
                            itemAId = itemConfig.id,
                            isChange = true,
                        });
                    }
                }
            }
        }

        // private static string constPlaceId = "battlepass2";

        public bool CanShow()
        {
            if (!IsOpened())
                return false;

            // if (CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, constPlaceId))
            //     return false;

            string openView = UINameConst.UIBattlePass2Main;
            if (!storageBattlePass.IsShowStart)
            {
                if (UIManager.Instance.GetOpenedUIByPath(UINameConst.UIPopupBattlePass2Start))
                    return false;

                openView = UINameConst.UIPopupBattlePass2Start;
            }
            else
            {
                return false;
                // if (UIManager.Instance.GetOpenedUIByPath(UINameConst.UIBattlePass2Main))
                //     return false;
            }

            // CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, constPlaceId,
            //     CommonUtils.GetTimeStamp());

            //RecordShowStart();
            UIManager.Instance.OpenUI(openView);
            return true;
        }

        public void RecordShowStart()
        {
            storageBattlePass.IsShowStart = true;
        }
        
        
        public bool CanShowRefresh()
        {
            if (!IsOpened())
                return false;
            
            if (Activity.BattlePass_2.BattlePassTaskModel.Instance.battlePassTask.RefreshTime == CommonUtils.GetTomorrowTimestamp2((long)APIManager.Instance.GetServerTime()))
                return false;
            
            return Activity.BattlePass_2.UIPopupBattlePassRefreshController.CanShow();
        }
        
        public bool CanShowUltimatePurchase()
        {
            if (!IsOpened())
                return false;

            if (!IsPurchase())
                return false;

            if (BattlePassActiveConfig == null)
                return false;

            if (storageBattlePass.IsUltimatePurchase)
                return false;

            if (((long)APIManager.Instance.GetServerTime() - storageBattlePass.StartTime) / 1000 < BattlePassActiveConfig.ultimateShowHour * 60)
                return false;

            return true;
        }

        public bool IsOldUser()
        {
            return !StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig.ContainsKey("BattlePass");
        }

        public int GetShopId(BuyType type)
        {
            if (IsOldUser())
            {
                return BattlePassShopConfig.oldUserShopIds[(int)type];
            }
            else
            {
                if (type == BuyType.Copper)
                    return -1;
                return BattlePassShopConfig.newUserShopIds[(int)type-1];
            }
        }
    }
}