using System.Collections.Generic;
using ABTest;
using DragonPlus;
using DragonPlus.Config.Matreshkas;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Merge.Order;
using UnityEngine;

namespace Activity.Matreshkas.Model
{
    public class MatreshkasModel : ActivityEntityBase
    {
        private static MatreshkasModel _instance;
        public static MatreshkasModel Instance => _instance ?? (_instance = new MatreshkasModel());
    
        public override string Guid => "OPS_EVENT_TYPE_MATRESHKAS";

        public StorageMatreshkas Matreshkas
        {
            get
            {
                return StorageManager.Instance.GetStorage<StorageHome>().Matreshkas;
            }
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InitAuto()
        {
            Instance.Init();
            Instance.InitEvent();
        }

        public void InitEvent()
        {
            EventDispatcher.Instance.AddEventListener(EventEnum.DEATH_PRODUCT_ITEM, DeathProductItem);
        }
        public int Stage
        {
            get { return Matreshkas.Stage;}
            set { Matreshkas.Stage = value;}
        }
        public int AnimStage
        {
            get { return Matreshkas.AnimStage;}
            set { Matreshkas.AnimStage = value;}
        }
        
        public int GroupId
        {
            get { return Matreshkas.GroupId;}
            private set { Matreshkas.GroupId = value; }
        }

        
        public override void InitFromServerData(string activityId, string activityType, ulong startTime, ulong endTime,
            ulong rewardEndTime, bool manualEnd, string configJson, string activitySubType)
        {
            base.InitFromServerData(activityId, activityType, startTime, endTime, rewardEndTime, manualEnd, configJson,
                activitySubType);
            
            MatreshkasConfigManager.Instance.InitConfig(configJson);
            MatreshkasConfigManager.Instance.Trim();
            InitServerDataFinish();
        }

        public override void UpdateActivityState()
        {
            InitServerDataFinish();
        }

        public void UpdateJoinTime()
        {
            Matreshkas.JoinStartTime = (long)APIManager.Instance.GetServerTime();
            Matreshkas.JoinEndTime = (long)APIManager.Instance.GetServerTime() + MatreshkasConfigManager.Instance.MatreshkasSettingList[0].OpenTime*60*1000;
        }
        
        protected override void InitServerDataFinish()
        {
            if(ActivityId.IsEmptyString())
                return;

            if (Matreshkas.ActivityId == ActivityId)
            {
                Matreshkas.StartActivityTime = (long)StartTime;
                Matreshkas.ActivityEndTime = (long)EndTime;
                return;
            }

            CheckJoinEnd(true);
            
            Matreshkas.ActivityId = ActivityId;
            Matreshkas.IsJoin = false;
            Matreshkas.JoinStartTime = 0;
            Matreshkas.JoinEndTime = 0;
            Matreshkas.StartActivityTime = (long)StartTime;
            Matreshkas.ActivityEndTime = (long)EndTime;
            Matreshkas.CompleteNum = 0;
            Matreshkas.GroupId = 0;
            Matreshkas.Stage = 0;
            Matreshkas.AnimStage = 0;
            Matreshkas.RecycleIds.Clear();
            Matreshkas.RecycleIds.AddRange(MatreshkasConfigManager.Instance.MatreshkasSettingList[0].RecycleMergeIds);
            Matreshkas.PayLevelGroup = PayLevelModel.Instance.GetCurPayLevelConfig().Matreshkas;
        }

        public bool IsJoin()
        {
            return Matreshkas.IsJoin;
        }
        
        public virtual string GetJoinEndTimeString()
        {
            if (!IsOpened())
                return "";

            if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.Matreshkas))
                return "";
            
            if (!IsJoin())
                return "";
            
            var left = (long) Matreshkas.JoinEndTime - (long) APIManager.Instance.GetServerTime();
            if (left < 0)
                left = 0;
            
            return CommonUtils.FormatLongToTimeStr(left);
        }

        public bool IsTimeEnd()
        {
            if (Matreshkas.IsJoin)
            {
                return (long)APIManager.Instance.GetServerTime() > Matreshkas.JoinEndTime;
            }
            else
            {
                return (long)APIManager.Instance.GetServerTime() > Matreshkas.ActivityEndTime;
            }
        }
        public void CheckJoinEnd(bool isForce = false)
        {
            if(Matreshkas.ActivityId.IsEmptyString())
                return;
            
            if(!Matreshkas.IsJoin)
                return;
            
            // if(Matreshkas.GroupId == 0)
            //     return;
            
            if(!isForce && (long) APIManager.Instance.GetServerTime() < Matreshkas.JoinEndTime)
                return;

            MergeManager.Instance.RemoveBoardItems(MergeBoardEnum.Main, Matreshkas.RecycleIds, true, "Matreshkas");
            
            Matreshkas.GroupId = 0;
        }
        public override bool CanDownLoadRes()
        {
            return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.Matreshkas);
        }

        public bool CanShowJoin()
        {
            if (!IsOpened())
                return false;

            if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.Matreshkas))
                return false;
            
            if (IsTimeEnd())
                return false;
            
            if (Matreshkas.IsJoin)
                return false;
            
            int openTime = MatreshkasConfigManager.Instance.MatreshkasSettingList[0].OpenTime * 60 * 1000;
            if ((long)EndTime - (long)APIManager.Instance.GetServerTime() <= openTime)
                return false;
            var level = ExperenceModel.Instance.GetLevel();
            int adaptLevel = MatreshkasConfigManager.Instance.AdaptMatreshkasLevel(level);
            Matreshkas.GroupId = adaptLevel;
            Matreshkas.IsJoin = true;
            Matreshkas.Stage = 0;

            GenerateEatBuildId();
            
            UpdateJoinTime();
            
            UIManager.Instance.OpenUI(UINameConst.UIPopupMatreshkas);

            InitMergeItem(MatreshkasConfigManager.Instance.MatreshkasSettingList[0].InitMergeId);
            
            return true;
        }

        private void InitMergeItem(int id)
        {
            var mergeItem = MergeManager.Instance.GetEmptyItem();
            mergeItem.Id = id;
            mergeItem.State = 1;
            MergeManager.Instance.AddRewardItem(mergeItem, MergeBoardEnum.Main,1, true);
            GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
            {
                MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonAdaptOldUser,
                isChange = true,
            });
        }

        private int GenerateEatBuildId()
        {
            var config = GetMatreshkasConfig();

            int minDifficulty = config.MinDifficulty;
            int maxDifficulty = config.MaxDifficulty;
            
            Matreshkas.EatBuildId = RandomAvailableItem(minDifficulty, maxDifficulty);

            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventMatreshkasTaskFinish,Matreshkas.Stage.ToString(), Matreshkas.EatBuildId.ToString());
            
            return Matreshkas.EatBuildId;
        }

        private int RandomAvailableItem(int minDifficulty, int maxDifficulty)
        {
            var items = GetAvailableItems(minDifficulty, maxDifficulty);

            if (items.Count == 0)
            {
                for(int i = 1; i < 6; i++)
                {
                    items = GetAvailableItems(minDifficulty, (int)(1.0f*maxDifficulty * (1f+i*0.25f)));
                    if(items.Count > 0)
                        break;
                }
            }
            
            if(items.Count == 0)
                items = GetAvailableItems(0, (int)Difficulty.maxDifficulty);
            
            int itemId = MainOrderCreatorRandomCommon.RandomAvailableItem(items);

            return itemId;
        }

        private List<AvailableItem> GetAvailableItems(int minDifficulty, int maxDifficulty)
        {
            List<AvailableItem> availableItems = new List<AvailableItem>();
            
            int level = ExperenceModel.Instance.GetLevel();
            foreach (var kv in OrderConfigManager.Instance._orderItems)
            {
                var itemCode = kv.Key;
                var itemConfig = kv.Value;

                if(itemConfig == null)
                    continue;
                
                if(itemConfig.isRecycle)
                    continue;
                
                if (level < itemConfig.unlockLevel)
                    continue;
                
                if(itemConfig.difficulty < minDifficulty || itemConfig.difficulty > maxDifficulty)
                    continue;
                
                if (itemConfig.progressUnlock == 1 && !MainOrderManager.Instance.IsUnlockMergeItem(itemCode))
                    continue;

                if (itemConfig.completeOrderId > 0)
                {
                    if(!MainOrderManager.Instance.IsCompleteOrder(itemConfig.completeOrderId))
                        continue;
                }
                
                if (itemConfig.generators != null && itemConfig.generators.Length > 0)
                {
                    bool isUnlock = false;
                    for (int i = 0; i < itemConfig.generators.Length; i++)
                    {
                        if (MainOrderManager.Instance.IsUnlockMergeItem(itemConfig.generators[i]))
                        {
                            isUnlock = true;
                            break;
                        }
                    }
                
                    if(!isUnlock)
                        continue;
                }
                
                
                if (ABTestManager.Instance.IsOpenOrderABTest() && itemConfig.discardBuilds != null && itemConfig.discardBuilds.Length > 0)
                {
                    bool isHave = false;
                    for (int i = 0; i < itemConfig.discardBuilds.Length; i++)
                    {
                        if (MainOrderManager.Instance.IsUnlockMergeItem(itemConfig.discardBuilds[i]))
                        {
                            isHave = true;
                            break;
                        }
                    }
                
                    if(isHave)
                      continue;
                }
                
                
                AvailableItem availableItem = new AvailableItem();
                availableItem._orderItemId = itemCode;
                availableItem._weight = itemConfig.diffWeight;
                availableItems.Add(availableItem);
            }

            return availableItems;
        }
        
        public MatreshkasConfig GetMatreshkasConfig(int index = -1)
        {
            index = index < 0 ? Matreshkas.Stage : index;
            
            var groups = MatreshkasConfigManager.Instance.GetMatreshkasConfigs( Matreshkas.GroupId);
            MatreshkasConfig config = null;
            
            if (index >= 0 && index <= groups.Count-1)
                return  groups[index];
           
            return  groups[groups.Count-1];
        }

        public List<int> GetMatreshkasPresetQueue(int index = -1)
        {
            var config = GetMatreshkasConfig();

            return config.PresetQueue;
        }

        public void FinishCurrentStage()
        {
            Matreshkas.Stage++;

            if (Matreshkas.Stage >= MatreshkasConfigManager.Instance.GetMatreshkasConfigs(Matreshkas.GroupId).Count)
            {
                CheckJoinEnd(true);
                GroupId = 0;
                Matreshkas.JoinEndTime = 0;
                UIManager.Instance.OpenUI(UINameConst.UIPopupMatreshkas, true);
                return;
            }

            GenerateEatBuildId();
        }
        
        private int[] _produceCos;
        public int[] GetProduceCost()
        {
            if (!IsOpened())
                return null;
            
            if (IsTimeEnd())
                return null;
            
            if(_produceCos == null)
                _produceCos =  new[] { Matreshkas.EatBuildId };
            else if (_produceCos[0] != Matreshkas.EatBuildId)
                _produceCos[0] = Matreshkas.EatBuildId;

            return _produceCos;
        }

        private void DeathProductItem(BaseEvent e)
        {
            if(e == null || e.datas == null || e.datas.Length <= 0)
                return;

            var config = (TableMergeItem)e.datas[0];
            
            if (config.subType != (int)SubType.Matreshkas)
                return;
            
            FinishCurrentStage();
        }

        public int GetTotalStage()
        {
            var configs = MatreshkasConfigManager.Instance.GetMatreshkasConfigs(Matreshkas.GroupId);
            if (configs == null)
                return 0;

            return configs.Count;
        }
    }
}