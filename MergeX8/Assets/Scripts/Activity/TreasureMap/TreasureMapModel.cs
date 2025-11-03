using System;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.TimeOrder;
using DragonPlus.Config.TreasureMap;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using Merge.Order;
using SomeWhere;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Activity.TreasureMap
{
    public class TreasureMapModel : ActivityEntityBase
    {
        private static TreasureMapModel _instance;
        public static TreasureMapModel Instance => _instance ?? (_instance = new TreasureMapModel());
    
        public override string Guid => "OPS_EVENT_TYPE_TREASURE_MAP";

        public StorageTreasureMap TreasureMap 
        {
            get
            {
               return StorageManager.Instance.GetStorage<StorageHome>().TreasureMap ;
            }
        }

        private TreasureMapActivityConfig _treasureMapActivityConfig;
        public TreasureMapActivityConfig TreasureMapActivityConfig
        {
            get
            {
                if (_treasureMapActivityConfig == null)
                    _treasureMapActivityConfig = TreasureMapConfigManager.Instance.TreasureMapActivityConfigList[0];
                return _treasureMapActivityConfig;
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
            TreasureMapConfigManager.Instance.InitConfig(configJson);
            InitServerDataFinish();
        }

        public override void UpdateActivityState()
        {
            InitServerDataFinish();
        }

        public override bool CanDownLoadRes()
        {
            return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.TreasureMap);
        }

        protected override void InitServerDataFinish()
        {
            if(ActivityId.IsEmptyString())
                return;
            if (TreasureMap.ActivityId!=ActivityId)
            {
                TreasureMap.Clear();
                TreasureMap.ActivityId = ActivityId;
                TreasureMap.MapId = TreasureMapActivityConfig.MapList[0];
               
            }
        }

        public bool IsOpen()
        {
            return IsOpened();
        }
        public TreasureMapConfig GetTreasureMapConfig(int mapId)
        {
            return TreasureMapConfigManager.Instance.TreasureMapConfigList.Find(a => a.Id == mapId);
        }
        public TreasureMapLimitConfig GetTreasureMapLimitConfig()
        {
            if (TreasureMap.CollectedChip.Count >= TreasureMapConfigManager.Instance.TreasureMapLimitConfigList.Count)
                return null;
            return TreasureMapConfigManager.Instance.TreasureMapLimitConfigList[TreasureMap.CollectedChip.Count];
        }
        public TreasureMapConfig GetCurrentTreasureMapConfig()
        {
            if(TreasureMap.MapId ==0)
                TreasureMap.MapId = TreasureMapActivityConfig.MapList[0];
            return GetTreasureMapConfig(TreasureMap.MapId);
        }

        public List<int> GetCurrentMapChips()
        {
                
            return TreasureMap.CollectedChip;
        }

        public bool IsCollectChip(int chipId)
        {
            return GetCurrentMapChips().Contains(chipId);
        }

        public bool FinishTask(int mapExp=1)
        {
            if(!IsInitFromServer())
                return false;
            TreasureMap.FinishTaskCount+=mapExp;
            var config= GetCurrentTreasureMapConfig();
            int collectedChipCount = TreasureMap.CollectedChip.Count;
            if (config.ChipCount > collectedChipCount)//未完成收集
            {
                var limitConfig = TreasureMapConfigManager.Instance.TreasureMapLimitConfigList[collectedChipCount];
                if (limitConfig.LimitMin <= TreasureMap.FinishTaskCount) //任务次数够
                {
                    var p = limitConfig.Probability;
                    var random=Random.Range(0, 100);
                    if (random <= limitConfig.Probability||TreasureMap.FinishTaskCount>=limitConfig.LimitMax)
                    {
                        AddChip();
                        TreasureMap.FinishTaskCount = 0;

                        return true;
                    }
                }
            }

            return false;
        }
        public void AddChip()
        {
            if(!IsOpen())
                return;
            var chips = TreasureMap.CollectedChip;
            List<int> _randomList = new List<int>();
            for (int i = 1; i <=GetCurrentTreasureMapConfig().ChipCount; i++)
            {
                if (!chips.Contains(i))
                    _randomList.Add(i);
            }

            int newChip = _randomList.RandomPickOne();
            TreasureMap.NewChip = newChip;
            TreasureMap.CollectedChip.Add(newChip);
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventKeepPetTreasureMapGet,
                newChip.ToString(),TreasureMap.FinishTaskCount.ToString());
        }

        public bool CanGetReward()
        {
            if( TreasureMap.CollectedChip.Count>= GetCurrentTreasureMapConfig().ChipCount)
            {
                return true;
            }

            return false;
        }

        public void GetReward(int mapId)
        {
            var config= GetTreasureMapConfig(mapId);
            
            var resDatas = CommonUtils.FormatReward(config.Reward, config.Count);
            UserData.Instance.AddRes(resDatas,new GameBIManager.ItemChangeReasonArgs()
            {
                reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.DogTreasureMapCollect
            });
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventKeepPetTreasureMapCollect,
                mapId.ToString());
            //
            // CommonRewardManager.Instance.PopCommonReward(resDatas, CurrencyGroupManager.Instance.currencyController,
            //     true, new GameBIManager.ItemChangeReasonArgs()
            //     {
            //
            //     }, () =>
            //     {
            //         cb?.Invoke();
            //     });

            TreasureMap.IsFinish = true;
        }
        
    }
}