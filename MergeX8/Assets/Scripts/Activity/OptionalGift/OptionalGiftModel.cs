using System;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.ExtraEnergy;
using DragonPlus.Config.OptionalGift;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using UnityEngine;

namespace OptionalGift
{
    public class OptionGiftModel : ActivityEntityBase
    {
        public override string Guid => "OPS_EVENT_TYPE_OPTIONAL_GIFT";

        public override bool CanDownLoadRes()
        {
            return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.OptionalGift);
        }

        private static OptionGiftModel _instance;
        public static OptionGiftModel Instance => _instance ?? (_instance = new OptionGiftModel());
        
        private StorageOptionalGift _storageOptionalGift;
        public StorageOptionalGift StorageOptionalGift
        {
            get
            {
                if (_storageOptionalGift == null)
                    _storageOptionalGift = StorageManager.Instance.GetStorage<StorageHome>().OptionalGift;

                return _storageOptionalGift;
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
            OptionalGiftConfigManager.Instance.InitConfig(configJson);
            InitServerDataFinish();
        }
        protected override void InitServerDataFinish()
        {
            base.InitServerDataFinish();
            if (StorageOptionalGift.ActivityId != ActivityId)
            {
                StorageOptionalGift.Clear();
                StorageOptionalGift.ActivityId = ActivityId;
            }
        }
        
        public override bool IsOpened(bool hasLog = false)
        {
            if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.OptionalGift))
                return false;
            bool isOpen = base.IsOpened();
            if (!isOpen)
                return false;
            if (StorageOptionalGift.IsBuy)
                return false;
            return base.IsOpened(hasLog);
        }

        public OptionalGiftActivityConfig GetOptionalGiftActivityConfig()
        {
            int optionalGiftGroupId = PayLevelModel.Instance.GetCurPayLevelConfig().OptionalGiftGroupId;
            return OptionalGiftConfigManager.Instance.OptionalGiftActivityConfigList.Find(a=>a.Id==optionalGiftGroupId);
        }
        
        public int GetSelect(int index)
        {
            if (StorageOptionalGift.SelectItem.ContainsKey(index))
                return StorageOptionalGift.SelectItem[index];
            return 0;
        }     
        public void AddSelect(int index,int selectItem)
        {
            if (StorageOptionalGift.SelectItem.ContainsKey(index))
                StorageOptionalGift.SelectItem[index] = selectItem;
            else
            {
                StorageOptionalGift.SelectItem.Add(index,selectItem);
            }
        }

        public bool IsSelectAll()
        {
            var config= GetOptionalGiftActivityConfig();
            int needSelectCount = 0;
            if (config.Type1 != 1)
                needSelectCount++;        
            if (config.Type2 != 1)
                needSelectCount++;        
            if (config.Type3 != 1)
                needSelectCount++;

            return needSelectCount <= StorageOptionalGift.SelectItem.Count;
        }

        public void PurchaseSuccess(TableShop tableShop)
        {
            var config= GetOptionalGiftActivityConfig();
            if (config == null)
                return;
            List<int> selectItem = new List<int>();
            List<int> canSelectItem = new List<int>();
            List<ResData> resDatas = new List<ResData>();
            if (config != null)
            {
                if (config.Type1 == 1)
                {
                    resDatas.Add(new ResData(config.Item1[0],config.Count1[0]));
                }
                else
                {
                    canSelectItem.AddRange(config.Item1);
                    if (StorageOptionalGift.SelectItem.ContainsKey(0))
                    {
                        selectItem.Add(StorageOptionalGift.SelectItem[0]);
                        resDatas.Add(new ResData(StorageOptionalGift.SelectItem[0],config.Count1[config.Item1.IndexOf(StorageOptionalGift.SelectItem[0])]));
                    }
                }
                if (config.Type2 == 1)
                {
                    resDatas.Add(new ResData(config.Item2[0],config.Count2[0]));
                }
                else
                {
                    if (StorageOptionalGift.SelectItem.ContainsKey(1))
                    {
                        resDatas.Add(new ResData(StorageOptionalGift.SelectItem[1],config.Count2[config.Item2.IndexOf(StorageOptionalGift.SelectItem[1])]));
                        selectItem.Add(StorageOptionalGift.SelectItem[1]);
                    }
                    canSelectItem.AddRange(config.Item2);
                }
                if (config.Type3 == 1)
                {
                    resDatas.Add(new ResData(config.Item3[0],config.Count3[0]));
                }
                else
                {
                    canSelectItem.AddRange(config.Item3);

                    if (StorageOptionalGift.SelectItem.ContainsKey(2))
                    {
                        resDatas.Add(new ResData(StorageOptionalGift.SelectItem[2],config.Count3[config.Item3.IndexOf(StorageOptionalGift.SelectItem[2])]));
                        selectItem.Add(StorageOptionalGift.SelectItem[2]);

                    }
                }
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventPrviategittChoose,config.ShopId.ToString(),String.Join(",", selectItem),String.Join(",", canSelectItem));
                PopReward(resDatas);
                EventDispatcher.Instance.DispatchEvent(EventEnum.PURCHASE_SUCCESS_REWARD, resDatas);
                StorageOptionalGift.IsBuy = true;
                StorageOptionalGift.SelectItem.Clear();
                var controller= UIManager.Instance.GetOpenedUIByPath<UIPopupOptionalGiftMainController>(UINameConst.UIPopupOptionalGiftMain);
                if(controller!=null)
                    controller.AnimCloseWindow();
            }
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
                    EventDispatcher.Instance.DispatchEvent(EventEnum.LUCKY_GOLDEN_EGG_PURCHASE);
                });
        }
        private static string coolTimeKey = "Optional";
        public static bool CanShowUI()
        {
            if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.OptionalGift))
                return false;


            if (!Instance.IsOpened())
                return false;

            if (!CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, coolTimeKey))
            {
                UIManager.Instance.OpenUI(UINameConst.UIPopupOptionalGiftMain);
                CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey, CommonUtils.GetTimeStamp());
                return true;
            }
            return false;
        }
    }
}
