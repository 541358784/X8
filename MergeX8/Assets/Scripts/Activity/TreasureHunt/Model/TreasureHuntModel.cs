using System;
using System.Collections.Generic;
using System.Linq;
using DragonPlus;
using DragonPlus.Config.TreasureHunt;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using Merge.Order;
using SomeWhere;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Activity.TreasureHuntModel
{
    public class TreasureHuntModel : ActivityEntityBase
    {
        private static TreasureHuntModel _instance;
        public static TreasureHuntModel Instance => _instance ?? (_instance = new TreasureHuntModel());

        public override string Guid => "OPS_EVENT_TYPE_TREASURE_HUNT";
        
        public StorageTreasureHunt TreasureHunt
        {
            get { return StorageManager.Instance.GetStorage<StorageHome>().TreasureHunt; }
        }

        private TreasureHuntActivityConfig _treasureHuntActivityConfig;

        public TreasureHuntActivityConfig TreasureHuntActivityConfig
        {
            get
            {
                if (_treasureHuntActivityConfig == null)
                    _treasureHuntActivityConfig = TreasureHuntConfigManager.Instance.TreasureHuntActivityConfigList[0];
                return _treasureHuntActivityConfig;
            }
        }

        public TreasureHuntLevelConfig GetTreasureHuntLevelConfig()
        {
            if (TreasureHunt.Level >= TreasureHuntConfigManager.Instance.TreasureHuntLevelConfigList.Count)
                return TreasureHuntConfigManager.Instance.TreasureHuntLevelConfigList.LastOrDefault();
            return TreasureHuntConfigManager.Instance.TreasureHuntLevelConfigList[TreasureHunt.Level];
        }
        
        public TreasureHuntStoreConfig GetTreasureHuntStoreConfig(int shopId)
        {
            return TreasureHuntConfigManager.Instance.TreasureHuntStoreConfigList.Find(a=>a.ShopId==shopId);
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
            TreasureHuntConfigManager.Instance.InitConfig(configJson);
            InitServerDataFinish();
        }

        public override void UpdateActivityState()
        {
            InitServerDataFinish();
        }
        public bool CanBreak=true;
        public override bool CanDownLoadRes()
        {
            return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.TreasureHunt);
        }

        protected override void InitServerDataFinish()
        {
            if (ActivityId.IsEmptyString())
                return;
            if (TreasureHunt.ActivityId != ActivityId)
            {
                TreasureHunt.Clear();
                TreasureHunt.ActivityId = ActivityId;
                TreasureHunt.Hammer = 1;
            }
        }

        public bool IsOpen()
        {
            if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.TreasureHunt))
                return false;
            if (!IsOpened())
                return false;
            if (TreasureHunt.IsFinish)
                return false;
            return true;
        }

        public void Start()
        {
            TreasureHunt.IsStart = true;
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
            if (TreasureHuntActivityConfig == null)
                return false;
            if (serverTime - StartTime <= (ulong) TreasureHuntActivityConfig.PreheatTime * 3600 * 1000)
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
            if (TreasureHuntActivityConfig == null)
                return 0;
            var left = (ulong) TreasureHuntActivityConfig.PreheatTime * 3600 * 1000 -
                       (APIManager.Instance.GetServerTime() - StartTime);
            if (left < 0)
                left = 0;
            return left;
        }

        public void AddHammer(int count)
        {
            TreasureHunt.Hammer += count;
        }
        public int GetHammer()
        {
            return TreasureHunt.Hammer ;
        }

        public float GetCostProgress()
        {
            return (float) TreasureHunt.EnergyCost / GetCastEnergy();
        }

        public int GetCastEnergy()
        {
            int index = 0;
            if (TreasureHuntActivityConfig.Limit == null || TreasureHuntActivityConfig.Energy == null)
                return 100;
            for (int i = 0; i < TreasureHuntActivityConfig.Limit.Count; i++)
            {
                if (TreasureHunt.TaskHummer >= TreasureHuntActivityConfig.Limit[i])
                    index = i;
            }

            return TreasureHuntActivityConfig.Energy[index];
        }
     
        public void TryProductHammer(int index, int id, MergeBoard board, int doubleEnergyTimes) //尝试生成活动棋子
        {
            if (!IsStart())
                return;
            int energyCount = (int) Mathf.Pow(2, doubleEnergyTimes);
            TreasureHunt.EnergyCost += energyCount;
            if (TreasureHunt.EnergyCost >=GetCastEnergy())//
            {
                TreasureHunt.EnergyCost = 0;
                TreasureHunt.TaskHummer ++;
                TreasureHunt.Hammer+=TreasureHuntActivityConfig.HammerCount; 
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventTreasurehuntHammerGet,
                    TreasureHuntActivityConfig.HammerCount.ToString(),"energy");
                FlyHammer( Vector2.zero, 
                    MergeTaskTipsController.Instance._MergeTreasureHunt.transform, 0.5f, true,
                    () =>
                    {
                        MergeTaskTipsController.Instance._MergeTreasureHunt.RefreshView();
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
                    var prefab = ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/Activity/TreasureHunt/FlyItem");
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

            var config = GetTreasureHuntStoreConfig(cfg.id);
            var res= CommonUtils.FormatReward(config.RewardId, config.RewardCount);
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventTreasurehuntHammerGet,
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
                    EventDispatcher.Instance.DispatchEvent(EventEnum.TREASURE_HUNT_PURCHASE);
                });
        }
        public bool IsBreak(int index)
        {
            return TreasureHunt.BreakItems.Contains(index);
        }
        public void BreakItem(int index,TreasureHuntLevelConfig levelConfig)
        {
         
            CanBreak=false;
            TreasureHunt.Hammer--;
            TreasureHunt.BreakItems.Add(index);
            TreasureHunt.BreakCount++;
            bool isFindKey = false;
            if (TreasureHunt.BreakCount >= levelConfig.UnderLimit)
            {
                int leftCount=  levelConfig.ItemCount - TreasureHunt.BreakCount+1;
                int pre = 100 / leftCount;
                if (Random.Range(0, 100) <= pre)
                    isFindKey = true;
            }

            int randomReward = 0;
            int randomCount = 0;
            if (isFindKey)
            {
               
                var res = CommonUtils.FormatReward(levelConfig.FinishReward, levelConfig.FinishRewardCount);
                UserData.Instance.AddRes(res,new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.TreasurehuntGet));
                TreasureHunt.Level++;
                TreasureHunt.BreakCount = 0;
                TreasureHunt.BreakItems.Clear();
                TreasureHunt.RandomReward.Clear();
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventTreasurehuntReward,
                    TreasureHunt.Level.ToString());
                //完成了所有
                if (TreasureHunt.Level>=TreasureHuntConfigManager.Instance.TreasureHuntLevelConfigList.Count)
                {
                    TreasureHunt.IsFinish = true;
                    var res2 = CommonUtils.FormatReward(TreasureHuntActivityConfig.Reward, TreasureHuntActivityConfig.Count);
                    UserData.Instance.AddRes(res2,new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug));
                }
            }
            else
            {
                //小奖
                if (TreasureHunt.RandomReward.Count < levelConfig.RandomReward.Count)
                {
         
                    List<int> ind = new List<int>();
                    for (int i = 0; i < levelConfig.RandomReward.Count; i++)
                    {
                        if (!TreasureHunt.RandomReward.Contains(i))
                        {
                            ind.Add(i);
                        }
                    }
                    if(ind.Count<=0)
                        return ;
                    int leftCount=  levelConfig.ItemCount - TreasureHunt.BreakCount+1;
                    int pre = 100*ind.Count / leftCount;
                    if (Random.Range(0, 100) <= pre)
                    {
                        int randomIndex = ind.RandomPickOne();
                        randomReward=levelConfig.RandomReward[randomIndex];
                        randomCount=levelConfig.RandomRewardCount[randomIndex];
                        if (randomReward > 0)
                        {
                            UserData.Instance.AddRes(randomReward,randomCount,new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.TreasurehuntGet));
                            TreasureHunt.RandomReward.Add(randomIndex);
                        }
                    }
                }
            }
            EventDispatcher.Instance.DispatchEvent(EventEnum.TREASURE_HUNT_ITEM_BREAK, index,isFindKey,randomReward,randomCount);
            int biv = isFindKey ? 1 : randomReward;
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventTreasurehuntHammerUse,
                GetHammer().ToString(),biv.ToString(),TreasureHunt.Level.ToString());           

                
        }

        public Transform GetFlyTarget()
        {
            var dlg = UIManager.Instance.GetOpenedUIByPath<UITreasureHuntMainController>(UINameConst.UITreasureHuntMain);
            if (dlg != null)
            {
                return dlg._hammerBtn.transform;
            }

            return null;
        }
        public void OpenMainPopup()
        {
            if (Instance.TreasureHunt.IsStart)
            {
                UIManager.Instance.OpenUI(UINameConst.UITreasureHuntMain);

            }
            else
            {
                UIManager.Instance.OpenUI(UINameConst.UIPopupTreasureHuntStart);
            }
        }
        
        private static string coolTimeKey = "TreasureHunt";

        public static bool CanShowUI()
        {
            if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.TreasureHunt))
                return false;


            if (!Instance.IsOpened())
                return false;
            if (Instance.TreasureHunt.IsFinish)
                return false;

            if (!CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, coolTimeKey))
            {
                if (Instance.TreasureHunt.IsStart)
                {
                    UIManager.Instance.OpenUI(UINameConst.UITreasureHuntMain);

                }
                else
                {
                    UIManager.Instance.OpenUI(UINameConst.UIPopupTreasureHuntStart);
                }
                CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey, CommonUtils.GetTimeStamp());
                return true;
            }

            return false;
        }
    }
}