using System;
using System.Collections.Generic;
using Activity.Base;
using DragonPlus;
using DragonPlus.Config.LuckyGoldenEgg;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using SomeWhere;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Activity.LuckyGoldenEgg
{
    public class LuckyGoldenEggModel : ActivityEntityBase,I_ActivityStatus
    {
        private static LuckyGoldenEggModel _instance;
        public static LuckyGoldenEggModel Instance => _instance ?? (_instance = new LuckyGoldenEggModel());

        public override string Guid => "OPS_EVENT_TYPE_LUCKY_GOLDEN_EGG";
        
        public StorageTreasureHunt LuckyGoldenEgg
        {
            get { return StorageManager.Instance.GetStorage<StorageHome>().LuckyGoldenEgg; }
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
            LuckyGoldenEggConfigManager.Instance.InitConfig(configJson);
            InitServerDataFinish();
        }

        public override void UpdateActivityState()
        {
            InitServerDataFinish();
        }
        public bool CanBreak=true;
        public override bool CanDownLoadRes()
        {
            return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.LuckyGoldenEgg);
        }

        protected override void InitServerDataFinish()
        {
            if (ActivityId.IsEmptyString())
                return;
            if (LuckyGoldenEgg.ActivityId != ActivityId)
            {
                LuckyGoldenEgg.Clear();
                LuckyGoldenEgg.ActivityId = ActivityId;
                LuckyGoldenEgg.Hammer = 1;
                LuckyGoldenEgg.PayLevel = PayLevelModel.Instance.GetCurPayLevelConfig().LuckyEggGroupId;
            }
        }

        public int PayLevelGroup()
        {
            return LuckyGoldenEgg.PayLevel;
        }
        
        public bool IsOpen()
        {
            if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.LuckyGoldenEgg))
                return false;
            if (!IsOpened())
                return false;
            if (LuckyGoldenEgg.IsFinish)
                return false;
            return true;
        }

        public void Start()
        {
            LuckyGoldenEgg.IsStart = true;
            SetActivityStatus(I_ActivityStatus.ActivityStatus.Incomplete);
        }
        public bool IsStart()
        {
            if (!IsOpen())
                return false;
            if (IsPreheating())
                return false;
            return true;
        }
        public bool IsPreheating()
        {
            ulong serverTime = APIManager.Instance.GetServerTime();
            if (IsSkipActivityPreheating())
                return false;
            if (LuckyGoldenEggConfigManager.Instance.luckyGoldenEggActivityConfig == null)
                return false;
            if (serverTime - StartTime <= (ulong) LuckyGoldenEggConfigManager.Instance.luckyGoldenEggActivityConfig.PreheatTime * 3600 * 1000)
                return true;

            return false;
        }

        // 活动剩余预热时间的字符串显示
        public virtual string GetActivityPreheatLeftTimeString()
        {
            return CommonUtils.FormatLongToTimeStr((long) GetActivityPreheatLeftTime());
        }

        public ulong GetActivityPreheatLeftTime()
        {
            if (LuckyGoldenEggConfigManager.Instance.luckyGoldenEggActivityConfig == null)
                return 0;
            var left = (ulong) LuckyGoldenEggConfigManager.Instance.luckyGoldenEggActivityConfig.PreheatTime * 3600 * 1000 -
                       (APIManager.Instance.GetServerTime() - StartTime);
            if (left < 0)
                left = 0;
            return left;
        }

        public void AddGoldenEgg(int count)
        {
            LuckyGoldenEgg.Hammer += count;
        }
        public int GetGoldenEgg()
        {
            return LuckyGoldenEgg.Hammer ;
        }

        public float GetCostProgress()
        {
            return (float) LuckyGoldenEgg.EnergyCost / GetCastEnergy();
        }

        public int GetCastEnergy()
        {
            int index = 0;
            if (LuckyGoldenEggConfigManager.Instance.GetSettingConfig().Limit == null || LuckyGoldenEggConfigManager.Instance.GetSettingConfig().Energy == null)
                return 100;
            for (int i = 0; i < LuckyGoldenEggConfigManager.Instance.GetSettingConfig().Limit.Count; i++)
            {
                if (LuckyGoldenEgg.TaskHummer >= LuckyGoldenEggConfigManager.Instance.GetSettingConfig().Limit[i])
                    index = i;
            }

            return LuckyGoldenEggConfigManager.Instance.GetSettingConfig().Energy[index];
        }
     
        public void TryProductHammer(int index, int id, MergeBoard board, int doubleEnergyTimes) //尝试生成活动棋子
        {
            if (!IsStart())
                return;
            int energyCount = (int) Mathf.Pow(2, doubleEnergyTimes);
            LuckyGoldenEgg.EnergyCost += energyCount;
            if (LuckyGoldenEgg.EnergyCost >=GetCastEnergy())//
            {
                LuckyGoldenEgg.EnergyCost = 0;
                LuckyGoldenEgg.TaskHummer ++;
                LuckyGoldenEgg.Hammer+=LuckyGoldenEggConfigManager.Instance.luckyGoldenEggActivityConfig.HammerCount; 
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventLuckygoldeneggHammerGet,
                    LuckyGoldenEggConfigManager.Instance.luckyGoldenEggActivityConfig.HammerCount.ToString(),"energy");
                FlyHammer( Vector2.zero, 
                    MergeTaskTipsController.Instance._MergeLuckyGoldenEgg.transform, 0.5f, true,
                    () =>
                    {
                        MergeTaskTipsController.Instance._MergeLuckyGoldenEgg.RefreshView();
                    });
            }
           

        }
        
        private static GameObject _flyItemObj;
        public static GameObject FlyItemObj
        {
            get
            {
                if (_flyItemObj == null)
                {
                    var prefab = ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/Activity/LuckyGoldenEgg/FlyItem");
                    _flyItemObj = GameObject.Instantiate(prefab);
                }
                return _flyItemObj;
            }
        }
        public static void FlyHammer(Vector2 srcPos,Transform starTransform, float time, bool showEffect, Action action = null)
        {
            Transform target = starTransform;
            float delayTime = 0f;
            Vector3 position = target.position;
            FlyItemObj.SetActive(true);
            FlyGameObjectManager.Instance.FlyObjectUpStraight(FlyItemObj, srcPos, position, showEffect, time, 0.5f,delayTime, () =>
            {
                FlyGameObjectManager.Instance.PlayHintStarsEffect(position);
                ShakeManager.Instance.ShakeLight();
                action?.Invoke();
            },controlY:0.5f,scale:2.5f);
            FlyItemObj.SetActive(false);
        }

        
        public void PurchaseSuccess(TableShop cfg)
        {

            var config = LuckyGoldenEggConfigManager.Instance.GetLuckyGoldenEggStoreConfig(cfg.id);
            var res= CommonUtils.FormatReward(config.RewardId, config.RewardCount);
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventLuckygoldeneggHammerGet,
                config.RewardCount[0].ToString(),"purchase");
            EventDispatcher.Instance.DispatchEvent(EventEnum.PURCHASE_SUCCESS_REWARD, res);
            PopReward(res);
           
        }
        public void PopReward(List<ResData> listResData)
        {
            if (listResData == null || listResData.Count <= 0)
                return;
            int count = listResData.Count > 8 ? 8 : listResData.Count;
            var list = listResData.GetRange(0, count);
            listResData.RemoveRange(0, count);
            var reasonArgs = new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.Iap);
            CommonRewardManager.Instance.PopCommonReward(list, CurrencyGroupManager.Instance.GetCurrencyUseController(), true, reasonArgs, animEndCall:
                () =>
                {
                    PopReward(listResData);
                    EventDispatcher.Instance.DispatchEvent(EventEnum.LUCKY_GOLDEN_EGG_PURCHASE);
                });
        }
        public bool IsBreak(int index)
        {
            return LuckyGoldenEgg.BreakItems.Contains(index);
        }
        public void BreakItem(int index,TableLuckyGoldenEggLevelConfig levelConfig)
        {
         
            CanBreak=false;
            LuckyGoldenEgg.Hammer--;
            LuckyGoldenEgg.BreakItems.Add(index);
            LuckyGoldenEgg.BreakCount++;
            bool isFindKey = false;
            if (LuckyGoldenEgg.BreakCount >= levelConfig.UnderLimit)
            {
                int leftCount=  levelConfig.ItemCount - LuckyGoldenEgg.BreakCount+1;
                int pre = 100 / leftCount;
                if (Random.Range(0, 100) <= pre)
                    isFindKey = true;
            }

            int randomReward = 0;
            int randomCount = 0;
            if (isFindKey)
            {
               
                var res = CommonUtils.FormatReward(levelConfig.FinishReward, levelConfig.FinishRewardCount);
                UserData.Instance.AddRes(res,new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.LuckyGoldenEggGet));
                LuckyGoldenEgg.Level++;
                LuckyGoldenEgg.BreakCount = 0;
                LuckyGoldenEgg.BreakItems.Clear();
                LuckyGoldenEgg.RandomReward.Clear();
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventLuckygoldeneggReward,
                    LuckyGoldenEgg.Level.ToString());
                //完成了所有
                if (LuckyGoldenEgg.Level>= LuckyGoldenEggConfigManager.Instance.TableLuckyGoldenEggLevelConfigListByPayLevel.Count)
                {
                    LuckyGoldenEgg.IsFinish = true;
                    SetActivityStatus(I_ActivityStatus.ActivityStatus.Completed);
                    var res2 = CommonUtils.FormatReward(LuckyGoldenEggConfigManager.Instance.luckyGoldenEggActivityConfig.Reward, LuckyGoldenEggConfigManager.Instance.luckyGoldenEggActivityConfig.Count);
                    UserData.Instance.AddRes(res2,new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug));
                }
            }
            else
            {
                //小奖
                if (LuckyGoldenEgg.RandomReward.Count < levelConfig.RandomReward.Count)
                {
         
                    List<int> ind = new List<int>();
                    for (int i = 0; i < levelConfig.RandomReward.Count; i++)
                    {
                        if (!LuckyGoldenEgg.RandomReward.Contains(i))
                        {
                            ind.Add(i);
                        }
                    }
                    if(ind.Count<=0)
                        return ;
                    int leftCount=  levelConfig.ItemCount - LuckyGoldenEgg.BreakCount+1;
                    int pre = 100*ind.Count / leftCount;
                    if (Random.Range(0, 100) <= pre)
                    {
                        int randomIndex = ind.RandomPickOne();
                        randomReward=levelConfig.RandomReward[randomIndex];
                        randomCount=levelConfig.RandomRewardCount[randomIndex];
                        if (randomReward > 0)
                        {
                            UserData.Instance.AddRes(randomReward,randomCount,new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.LuckyGoldenEggGet));
                            LuckyGoldenEgg.RandomReward.Add(randomIndex);
                        }
                    }
                }
            }
            EventDispatcher.Instance.DispatchEvent(EventEnum.LUCKY_GOLDEN_EGG_ITEM_BREAK, index,isFindKey,randomReward,randomCount);
            int biv = isFindKey ? 1 : randomReward;
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventLuckygoldeneggHammerUse,
                GetGoldenEgg().ToString(),biv.ToString(),LuckyGoldenEgg.Level.ToString());           

                
        }

        public Transform GetFlyTarget()
        {
            var dlg = UIManager.Instance.GetOpenedUIByPath<UILuckyGoldenEggMainController>(UINameConst.UILuckyGoldenEggMain);
            if (dlg != null)
            {
                return dlg._hammerBtn.transform;
            }

            return null;
        }
        public void OpenMainPopup()
        {
            if (Instance.LuckyGoldenEgg.IsStart)
            {
                UIManager.Instance.OpenUI(UINameConst.UILuckyGoldenEggMain);

            }
            else
            {
                UIManager.Instance.OpenUI(UINameConst.UIPopupLuckyGoldenEggStart);
            }
        }
        
        private static string coolTimeKey = "LuckyGoldenEgg";
        private static string coolTimeKey_Preheating = "coolTimeKey_Preheating";

        public static bool CanShowUI()
        {
            if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.LuckyGoldenEgg))
                return false;


            if (!Instance.IsOpened())
                return false;
            if (Instance.LuckyGoldenEgg.IsFinish)
                return false;

            if (Instance.IsPreheating())
            {
                if (!CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, coolTimeKey_Preheating))
                {
                    UIManager.Instance.OpenUI(UINameConst.UIPopupLuckyGoldenEggStart);
                    CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey_Preheating, CommonUtils.GetTimeStamp());
                    return true;
                }
            }
            else
            {
                if (!CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, coolTimeKey))
                {
                    if (Instance.LuckyGoldenEgg.IsStart)
                    {
                        UIManager.Instance.OpenUI(UINameConst.UILuckyGoldenEggMain);

                    }
                    else
                    {
                        UIManager.Instance.OpenUI(UINameConst.UIPopupLuckyGoldenEggStart);
                    }
                    CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey, CommonUtils.GetTimeStamp());
                    return true;
                }
            }

            return false;
        }

        public I_ActivityStatus.ActivityStatus GetActivityStatus()
        {
            return (I_ActivityStatus.ActivityStatus)LuckyGoldenEgg.ActivityStatus;
        }
        
        public void SetActivityStatus(I_ActivityStatus.ActivityStatus status)
        {
            LuckyGoldenEgg.ActivityStatus = (int)status;
        }
    }
}