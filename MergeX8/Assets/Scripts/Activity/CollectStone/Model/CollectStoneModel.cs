using System.Collections.Generic;
using Activity.CollectStone.View;
using Activity.JungleAdventure.Controller;
using DragonPlus;
using DragonPlus.Config.CollectStone;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Dynamic;
using UnityEngine;

namespace Activity.CollectStone.Model
{
    public class CollectStoneModel: ActivityEntityBase
    {
        private static CollectStoneModel _instance;
        public static CollectStoneModel Instance => _instance ?? (_instance = new CollectStoneModel());
    
        public override string Guid => "OPS_EVENT_TYPE_COLLECT_STONE";

        public StorageCollectStone CollectStone
        {
            get
            {
                return StorageManager.Instance.GetStorage<StorageHome>().CollectStone;
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
            
            CollectStoneConfigManager.Instance.InitConfig(configJson);
            InitServerDataFinish();
        }
        
        public override void UpdateActivityState()
        {
            InitServerDataFinish();
        }
        
        protected override void InitServerDataFinish()
        {
            if(ActivityId.IsEmptyString())
                return;

            if (CollectStone.ActivityId == ActivityId)
            {
                CollectStone.StartTime = (long)StartTime;
                CollectStone.EndTime = (long)EndTime;
                return;
            }

            CollectStone.Clear();
            CollectStone.ActivityId = ActivityId;
            CollectStone.StartTime = (long)StartTime;
            CollectStone.EndTime = (long)EndTime;
            CollectStone.PayLevelGroup = PayLevelModel.Instance.GetCurPayLevelConfig().CollectStone;
            CollectStone.LevelId = CollectStoneConfigManager.Instance.GetCollectSetting(CollectStone.PayLevelGroup).Id;
        }
        
        public string GetEndTimeString()
        {
            if (!IsOpened())
                return "";

            if (IsTimeEnd())
                return "";
            
            if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.CollectStone))
                return "";
            
            var left = (long) CollectStone.EndTime - (long) APIManager.Instance.GetServerTime();
            if (left < 0)
                left = 0;
            
            return CommonUtils.FormatLongToTimeStr(left);
        }
        
        public bool IsTimeEnd()
        {
            return (long)APIManager.Instance.GetServerTime() > CollectStone.EndTime;
        }
        
        public override bool CanDownLoadRes()
        {
            return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.CollectStone);
        }

        public override bool IsOpened(bool hasLog = false)
        {
            if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.CollectStone))
                return false;

            return base.IsOpened();
        }

        public void AddStone(int num, string source = null)
        {
            if(!IsOpened())
                return;
            
            if(IsTimeEnd())
                return;

            CollectStone.StoneNum += num;
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventCollectStoneGet,num.ToString(),CollectStone.PayLevelGroup.ToString(), source);
        }

        public int GetStone()
        {
            return CollectStone.StoneNum;
        }

        public bool IsHave(int index)
        {
            return CollectStone.State.Contains(index);
        }

        public void NextLevel()
        {
            var setting = GetCollectSetting();

            CollectStone.State.Clear();
            while (true)
            {
                if (CollectStone.IsLoop)
                {
                    var array = setting.LoopReward;
                    CollectStone.LoopIndex++;

                    if (CollectStone.LoopIndex >= array.Count)
                        CollectStone.LoopIndex = 0;

                    return;
                }
                else
                {
                    var array = setting.FixReward;
                    
                    CollectStone.FixIndex++;
                    if (CollectStone.FixIndex >= array.Count)
                        CollectStone.IsLoop = true;
                    // else
                        return;
                }
            }
        }
        public TableCollectSetting GetCollectSetting()
        {
           return CollectStoneConfigManager.Instance.TableCollectSettingList.Find(a => a.Id == CollectStoneModel.Instance.CollectStone.LevelId);
        }
        public TableCollectReward GetCollectReward()
        {
            var setting = GetCollectSetting();

            int index;
            List<int> array;
            
            if (CollectStone.IsLoop)
            {
                array = setting.LoopReward;
                index = CollectStone.LoopIndex;
            }
            else
            {
                array = setting.FixReward;
                index = CollectStone.FixIndex;
            }

            index = Mathf.Clamp(index, 0, array.Count - 1);
            int rewardId = array[index];

            return CollectStoneConfigManager.Instance.TableCollectRewardList.Find(a => a.Id == rewardId);
        }
        
        public Transform GetCommonFlyTarget()
        {
            if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
            {
                return MergeMainController.Instance.backTrans;
            }
            else
            {
                var entrance = DynamicEntryManager.Instance.GetDynamicEntry<DynamicEntry_Home_CollectStone>();
                if (entrance)
                    return entrance.transform;
                else
                    return UIHomeMainController.mainController.MainPlayTransform;
            }
        }
        
        
        private static string CoolTimeKey = "CollectStone";
        public static bool CanShowUI()
        {
            if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.CollectStone))
                return false;

            if (!Instance.IsOpened())
                return false;

            if (!CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, CoolTimeKey))
            {
                CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, CoolTimeKey, CommonUtils.GetTimeStamp());
                UIManager.Instance.OpenUI(UINameConst.UIPopupCollectStoneMain);
                
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventCollectStonePop,CollectStoneModel._instance.CollectStone.PayLevelGroup.ToString());
                return true;
            }

            return false;
        }
    }
}