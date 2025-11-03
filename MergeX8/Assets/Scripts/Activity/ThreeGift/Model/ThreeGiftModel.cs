
using System;
using System.Collections.Generic;
using System.Linq;
using Deco.Item;
using Deco.World;
using Decoration;
using DragonPlus;
using DragonPlus.Config.ThreeGift;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Framework;
using Gameplay;
using UnityEngine;


namespace ThreeGift
{
    public class ThreeGiftModel : ActivityEntityBase
    {
        public override string Guid => "OPS_EVENT_TYPE_THREE_GIFT";

        public override bool CanDownLoadRes()
        {
            return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.ThreeGift);
        }

        private static ThreeGiftModel _instance;
        public static ThreeGiftModel Instance => _instance ?? (_instance = new ThreeGiftModel());

        private StorageThreeGift _storageThreeGift;

        public StorageThreeGift StorageThreeGift
        {
            get
            {
                if (_storageThreeGift == null)
                    _storageThreeGift = StorageManager.Instance.GetStorage<StorageHome>().ThreeGift;

                return _storageThreeGift;
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
            ThreeGiftConfigManager.Instance.InitConfig(configJson);
            InitServerDataFinish();
        }

        public ThreeGiftLevelConfig ThreeGiftLevelConfigList
        {
            get
            {
                if (ThreeGiftConfigManager.Instance.ThreeGiftLevelConfigList == null ||
                    ThreeGiftConfigManager.Instance.ThreeGiftLevelConfigList.Count <= 0)
                    return null;
                if (StorageThreeGift.GroupId == 0)
                    return ThreeGiftConfigManager.Instance.ThreeGiftLevelConfigList[0];
                var config  = ThreeGiftConfigManager.Instance.ThreeGiftLevelConfigList.Find(a =>
                    a.id == StorageThreeGift.GroupId);
                if (config == null)
                    config = ThreeGiftConfigManager.Instance.ThreeGiftLevelConfigList.First();
                return config;
            }
        }

        public List<ThreeGiftConfig> ThreeGiftList
        {
            get { return ThreeGiftConfigManager.Instance.ThreeGiftConfigList; }
        }

        public ThreeGiftConfig GetThreeGiftConfigById(int id)
        {
            return ThreeGiftList.Find(a => a.id == id);
        }

        protected override void InitServerDataFinish()
        {
            base.InitServerDataFinish();
            if (!string.IsNullOrEmpty(ActivityId) && StorageThreeGift.ActivityId != ActivityId)
            {
                StorageThreeGift.ActivityId = ActivityId;
                StorageThreeGift.IsPurchase = false;
                StorageThreeGift.GroupId = PayLevelModel.Instance.GetCurPayLevelConfig().ThreeGiftGroupId;
            }
        }

        public override bool IsOpened(bool hasLog = false)
        {
            if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.ThreeGift))
                return false;
            bool isOpen = base.IsOpened();
            if (!isOpen)
                return false;
            
            if (!CheckLineActivity())
                return false;
            
            if (StorageThreeGift.IsPurchase)
                return false;

            return base.IsOpened(hasLog);
        }

        private bool CheckLineActivity()
        {
            if (ThreeGiftList == null)
                return false;
            
            foreach (var threeGiftConfig in ThreeGiftList)
            {
                foreach (var id in threeGiftConfig.contain)
                {
                    if (CardCollectionModel.Instance.IsCardPackageResourceId(id))
                    {
                        if (!CardCollectionActivityModel.Instance.IsOpened())
                            return false;
                    }

                    if (id == (int)UserData.ResourceId.Easter2024Egg)
                    {
                        if (!Easter2024Model.Instance.IsOpened())
                            return false;
                    }
                    if (id == (int)UserData.ResourceId.SnakeLadderTurntable)
                    {
                        if (!SnakeLadderModel.Instance.IsOpened())
                            return false;
                    }
                    if (id == (int)UserData.ResourceId.MonopolyDice)
                    {
                        if (!MonopolyModel.Instance.IsStart())
                            return false;
                    }
                }
            }

            return true;
        }
        
        public void PurchaseSuccess(int shopId)
        {
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventOneorallDealsSuccess, shopId.ToString());

            StorageThreeGift.IsPurchase = true;
            List<ResData> listResData = new List<ResData>();
            if (ThreeGiftLevelConfigList != null)
            {
                var config = ThreeGiftLevelConfigList;
                var packages = new List<ThreeGiftConfig>();
                for (int i = 0; i < config.packageList.Length; i++)
                {
                    var giftConfig = GetThreeGiftConfigById(config.packageList[i]);
                    packages.Add(giftConfig);
                }
                if (ThreeGiftLevelConfigList.shopId == shopId)
                {
                    for (int i = 0; i < ThreeGiftLevelConfigList.packageList.Length; i++)
                    {
                        var giftConfig = packages.Find(a => a.id == ThreeGiftLevelConfigList.packageList[i]);
                        for (int j = 0; j < giftConfig.contain.Length; j++)
                        {
                            ResData res = new ResData(giftConfig.contain[j], giftConfig.containCount[j]);
                            listResData.Add(res);
                            if (!UserData.Instance.IsResource(res.id))
                            {
                                TableMergeItem mergeItemConfig = GameConfigManager.Instance.GetItemConfig(res.id);
                                if (mergeItemConfig != null)
                                {
                                    GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                                    {
                                        MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeItemChangeIapBundle,
                                        itemAId = mergeItemConfig.id,
                                        ItemALevel = mergeItemConfig.level,
                                        isChange = true,
                                    });
                                }
                            }
                        }
                    }
                }
                else
                {
                    var giftConfig = packages.Find(a => a.shopId == shopId);
                    for (int j = 0; j < giftConfig.contain.Length; j++)
                    {
                        ResData res = new ResData(giftConfig.contain[j], giftConfig.containCount[j]);
                        listResData.Add(res);
                        if (!UserData.Instance.IsResource(res.id))
                        {
                            TableMergeItem mergeItemConfig = GameConfigManager.Instance.GetItemConfig(res.id);
                            if (mergeItemConfig != null)
                            {
                                GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                                {
                                    MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeItemChangeIapBundle,
                                    itemAId = mergeItemConfig.id,
                                    ItemALevel = mergeItemConfig.level,
                                    isChange = true,
                                });
                            }
                        }
                    }
                }
            }
            UserData.Instance.AddRes(listResData,new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.Iap));
            PopReward(listResData);
            EventDispatcher.Instance.DispatchEvent(EventEnum.THREE_GIFT_PURCHASE_SUCCESS);
        }

        public void PopReward(List<ResData> listResData)
        {
            if (listResData == null || listResData.Count <= 0)
                return;
            int count = listResData.Count > 8 ? 8 : listResData.Count;
            var list = listResData.GetRange(0, count);
            listResData.RemoveRange(0, count);
            var reasonArgs = new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.Iap);
            EventDispatcher.Instance.DispatchEvent(EventEnum.PURCHASE_SUCCESS_REWARD, list);
            CommonRewardManager.Instance.PopCommonReward(list, CurrencyGroupManager.Instance.GetCurrencyUseController(), false, reasonArgs, animEndCall: () => { PopReward(listResData); });
        }

        private static string coolTimeKey = "threeGift";

        public static bool CanShowUI()
        {
            if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.ThreeGift))
                return false;


            if (!Instance.IsOpened())
                return false;

            if (!CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, coolTimeKey))
            {
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventOneorallDealsPop);
                UIManager.Instance.OpenUI(UINameConst.UIPopupThreeGift);
                CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey, CommonUtils.GetTimeStamp());
                return true;
            }

            return false;
        }
    }
}
