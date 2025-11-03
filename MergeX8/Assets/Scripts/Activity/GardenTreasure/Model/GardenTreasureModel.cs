using System;
using System.Collections.Generic;
using System.Linq;
using Activity.Base;
using DragonPlus;
using DragonPlus.Config.GardenTreasure;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using SomeWhere;
using UnityEngine;

namespace Activity.GardenTreasure.Model
{
    public class GardenTreasureModel : ActivityEntityBase,I_ActivityStatus
    {
        private static GardenTreasureModel _instance;
        public static GardenTreasureModel Instance => _instance ?? (_instance = new GardenTreasureModel());
    
        public override string Guid => "OPS_EVENT_TYPE_GARDEN_TREASURE";

        public StorageGardenTreasure GardenTreasure
        {
            get
            {
                return StorageManager.Instance.GetStorage<StorageHome>().GardenTreasure;
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
            GardenTreasureConfigManager.Instance.InitConfig(configJson);
            GardenTreasureConfigManager.Instance.Trim();
            InitServerDataFinish();
            //GardenTreasureLeaderBoardModel.Instance.InitFromServerData();
        }

        public override void UpdateActivityState()
        {
            InitServerDataFinish();
        }

        protected override void InitServerDataFinish()
        {
            if(ActivityId.IsEmptyString())
                return;
            
            if (GardenTreasure.ActivityId == ActivityId)
            {
                GardenTreasure.StartActivityTime = (long)StartTime;
                GardenTreasure.ActivityEndTime = (long)EndTime;
                //GardenTreasureLeaderBoardModel.Instance.CreateStorage(GardenTreasure);
                return;
            }
            
            GardenTreasure.Clear();
            GardenTreasure.ActivityId = ActivityId;
            GardenTreasure.StartActivityTime = (long)StartTime;
            GardenTreasure.ActivityEndTime = (long)EndTime;
            GardenTreasure.PreheatEndTime = GardenTreasure.StartActivityTime + GardenTreasureConfigManager.Instance.GetSettingConfig()[0].PreheatTime * 60 * 1000;

            GardenTreasure.PayLevelGroup = PayLevelModel.Instance.GetCurPayLevelConfig().GardenTreasureGroupId;
            
            GardenTreasure.NormalLevelId = GardenTreasureConfigManager.Instance._normalLevelConfigs[0].Id;
            GardenTreasure.RandomLevelId = GardenTreasureConfigManager.Instance._randomLevelConfig.Id;
            GardenTreasure.ShowLevelId = 1;
            GardenTreasure.OpenGrids.Clear();
            GardenTreasure.GetShapes.Clear();
            GardenTreasure.IsRandomLevel = false;
            GardenTreasure.BoardId = GardenTreasureConfigManager.Instance._normalLevelConfigs[0].BoardConfigIds.Random();
            GardenTreasure.EnergyCost = 0;
            GardenTreasure.RecordEnterLevelCount.Clear();
            GardenTreasure.RecordConsume.Clear();
            
            //GardenTreasureLeaderBoardModel.Instance.CreateStorage(GardenTreasure);
            
            CleanUserData();
            
            UserData.Instance.AddRes((int)UserData.ResourceId.GardenShovel, GardenTreasureConfigManager.Instance.GetSettingConfig()[0].InitShovel, new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.GardenTreasureInitGet));
            
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventGardenTreasureCreateBoard, GetCurrentLevelId().ToString(), GardenTreasure.BoardId.ToString());
        }

        private void CleanUserData()
        {
            int shovel = UserData.Instance.GetRes(UserData.ResourceId.GardenShovel);
            UserData.Instance.ConsumeRes(UserData.ResourceId.GardenShovel, shovel, new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug));
        
            int bomb = UserData.Instance.GetRes(UserData.ResourceId.GardenBomb);
            UserData.Instance.ConsumeRes(UserData.ResourceId.GardenBomb, bomb, new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug));
        }

        public int PayLevelGroup()
        {
            return GardenTreasure.PayLevelGroup;
        }
        
        public string GetEndTimeString()
        {
            if (!IsOpened())
                return "";
            
            var left = (long) GardenTreasure.ActivityEndTime - (long) APIManager.Instance.GetServerTime();
            if (left < 0)
                left = 0;
            
            return CommonUtils.FormatLongToTimeStr(left);
        }

        public bool IsPreheatEnd()
        {
            if (!IsOpened())
                return false;

            if (IsSkipActivityPreheating())
                return true;
            
            return (long)APIManager.Instance.GetServerTime() - GardenTreasure.PreheatEndTime > 0;
        }
        
        public string GetPreheatEndTimeString()
        {
            if (!IsOpened())
                return "";
            
            var left = GardenTreasure.PreheatEndTime - (long) APIManager.Instance.GetServerTime();
            if (left < 0)
                left = 0;
            
            return CommonUtils.FormatLongToTimeStr(left);
        }
        
        public bool IsTimeEnd()
        {
            return (long)APIManager.Instance.GetServerTime() > GardenTreasure.ActivityEndTime;
        }
        
        public override bool CanDownLoadRes()
        {
            return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.GardenTreasure);
        }

        public override bool IsOpened(bool hasLog = false)
        {
            if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.GardenTreasure))
                return false;

            if (Instance.GardenTreasure.IsRandomLevel)
                return false;
            
            return base.IsOpened(hasLog);
        }

        public bool IsRandomLevel()
        {
            return GardenTreasure.IsRandomLevel;
        }
        public GardenTreasureLevelConfig GetCurrentLevelConfig()
        {
            int levelId = GetCurrentLevelId();

            return GardenTreasureConfigManager.Instance.GardenTreasureLevelConfigListByPayLevel.Find(a => a.Id == levelId);
        }

        public float GetNormalProgress()
        {
            int index = GetNormalIndex();
            index -= 1;

            return 1.0f + index + GetProgress();
        }

        public int GetNormalIndex()
        {
            int index = GardenTreasureConfigManager.Instance._normalLevelConfigs.FindIndex(a => a.Id == GardenTreasure.NormalLevelId);
            return index;
        }
        

        public float GetRandomProgress()
        {
            return GetProgress();
        }

        public float GetProgress()
        {
            var boardData = GardenTreasureConfigManager.Instance.GetBoardData(GardenTreasure.BoardId);
            int progress = 0;
            foreach (var data in boardData._shapeDatas)
            {
                if (GardenTreasure.GetShapes.Contains(data._index))
                    progress++;
            }
            
            return 1.0f * progress / boardData._shapeDatas.Count;;
        }
        
        public void PurchaseSuccess(TableShop tableShop)
        {
            var config =GardenTreasureConfigManager.Instance.GardenTreasurePackageConfigList.Find(a => a.ShopId == tableShop.id);
            var res= CommonUtils.FormatReward(config.RewardId, config.RewardCount);
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
                    EventDispatcher.Instance.DispatchEvent(EventEnum.GARDEN_TREASURE_PURCHASE);
                });
        }

        public float GetCostProgress()
        {
            return (float)GardenTreasure.EnergyCost / GardenTreasureConfigManager.Instance.GetSettingConfig()[0].Energy;
        }
        
        public void TryProductShovel(int index, int id, MergeBoard board, int doubleEnergyTimes) //尝试生成活动棋子
        {
            if (!IsOpened())
                return;
            
            if(!IsPreheatEnd())
                return;
            
            int energyCount = (int) Mathf.Pow(2, doubleEnergyTimes);
            GardenTreasure.EnergyCost += energyCount;
            if (GardenTreasure.EnergyCost >= GardenTreasureConfigManager.Instance.GetSettingConfig()[0].Energy)
            {
                GardenTreasure.EnergyCost -= GardenTreasureConfigManager.Instance.GetSettingConfig()[0].Energy;
                UserData.Instance.AddRes((int)UserData.ResourceId.GardenShovel, 1, new GameBIManager.ItemChangeReasonArgs()
                {
                   reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.GardenTreasureGet
                }, true);
                
                FlyShovel( Vector2.zero, 
                    MergeTaskTipsController.Instance.MergeGardenTreasureEntry.transform, 0.5f, true,
                    () =>
                    {
                        MergeTaskTipsController.Instance.MergeGardenTreasureEntry.RefreshView();
                    });
            }
        }
        
        public static void FlyShovel(Vector2 srcPos,Transform starTransform, float time, bool showEffect, Action action = null)
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
        
        private static GameObject _flyItemObj;
        public static GameObject FlyItemObj
        {
            get
            {
                if (_flyItemObj == null)
                {
                    _flyItemObj = ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/Activity/GardenTreasure/FlyItem");
                }
                return _flyItemObj;
            }
        }

        public void RecordEnterLevelCount()
        {
            int levelId = GetCurrentLevelId();

            if (!GardenTreasure.RecordEnterLevelCount.ContainsKey(levelId))
                GardenTreasure.RecordEnterLevelCount[levelId] = 0;

            GardenTreasure.RecordEnterLevelCount[levelId]++;
        }

        public int GetEnterCurrentLevelCount()
        {
            int levelId = GetCurrentLevelId();

            if (!GardenTreasure.RecordEnterLevelCount.ContainsKey(levelId))
                return 0;

            return GardenTreasure.RecordEnterLevelCount[levelId];
        }

        public int GetTotalEnterLevelCount()
        {
            int totalCount = 0;
            foreach (var kv in GardenTreasure.RecordEnterLevelCount)
            {
                totalCount += kv.Value;
            }

            return totalCount;
        }

        public void RecordConsume(int resourcesId)
        {
            if (!GardenTreasure.RecordConsume.ContainsKey(resourcesId))
                GardenTreasure.RecordConsume[resourcesId] = 0;

            GardenTreasure.RecordConsume[resourcesId]++;
        }

        public int GetRecordConsume(int resourcesId)
        {
            if (!GardenTreasure.RecordConsume.ContainsKey(resourcesId))
                return 0;

            return GardenTreasure.RecordConsume[resourcesId];
        }

        public int GetCurrentLevelId()
        {
            int levelId = GardenTreasure.NormalLevelId;
            if (GardenTreasure.IsRandomLevel)
                levelId = GardenTreasure.RandomLevelId;

            return levelId;
        }
        
        private static string coolTimeKey = "GardenTreasureKey";
        public bool CanShowUI()
        {
            if (!IsOpened())
                return false;

            if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.GardenTreasureStart))
                return false;
            
            if (CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, coolTimeKey))
                return false;

            CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey, CommonUtils.GetTimeStamp());

            if (GardenTreasure.ActivityStatus < (int)I_ActivityStatus.ActivityStatus.Incomplete)
            {
                SetActivityStatus(I_ActivityStatus.ActivityStatus.Incomplete);
            }
            
            if (IsPreheatEnd())
            {
                UIManager.Instance.OpenUI(UINameConst.UIGardenTreasureMain);
            }
            else
            {
                UIManager.Instance.OpenUI(UINameConst.UIPopupGardenTreasureStart);
            }
            return true;
        }

        public I_ActivityStatus.ActivityStatus GetActivityStatus()
        {
            return (I_ActivityStatus.ActivityStatus)GardenTreasure.ActivityStatus;
        }
        
        public void SetActivityStatus(I_ActivityStatus.ActivityStatus status)
        {
            GardenTreasure.ActivityStatus = (int)status;
        }
    }
}