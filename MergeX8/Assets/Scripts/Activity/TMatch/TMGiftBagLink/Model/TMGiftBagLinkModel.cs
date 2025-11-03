using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.TMatch;
using DragonPlus.ConfigHub.Ad;
using UnityEngine;
using DragonU3DSDK;
using DragonU3DSDK.Storage;
using Gameplay;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

namespace TMatch
{
    public class TMGiftBagLinkModel : ActivityEntityBase
    {
        private static TMGiftBagLinkModel _instance;
        public static TMGiftBagLinkModel Instance => _instance ?? (_instance = new TMGiftBagLinkModel());


        public StorageGiftBagLink StorageGiftBagLink
        {
            get
            {
               return StorageManager.Instance.GetStorage<StorageHome>().TmGiftBagLink;
            }
        }

        
        public bool IsUnlock => TMatchModel.Instance.GetMainLevel() >= TMatchConfigManager.Instance.GlobalList[0].GiftBagLineUnlock;
        
        public override string Guid => "OPS_EVENT_TYPE_TM_GIFT_BAG_LINK";

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
            TMGiftBagLinkConfigManager.Instance.InitFromServerData(configJson);
            InitServerDataFinish();
        }

        public override void UpdateActivityState()
        {
            InitServerDataFinish();
        }

        protected override void InitServerDataFinish()
        {
            if (!IsUnlock)
                return;
        }

        public override bool IsOpened(bool hasLog = false)
        {
            if (!IsUnlock)
                return false;

            bool isOpen = base.IsOpened();
            if (!isOpen)
                return false;

            List<TMGiftBagLinkResourceConfig> linkResources = GetGiftBagLinkResources();
            if (linkResources == null)
                return false;

            if (GetCurIndex() + 1 >= linkResources.Count)
                return false;

            return true;
        }

        public int GetCurIndex()
        {
            if (!StorageGiftBagLink.GiftBagLinkiIndexs.ContainsKey(StorageKey))
                StorageGiftBagLink.GiftBagLinkiIndexs.Add(StorageKey, 0);

            return StorageGiftBagLink.GiftBagLinkiIndexs[StorageKey];
        }

        public void AddCurIndex()
        {
            int index = GetCurIndex() + 1;

            StorageGiftBagLink.GiftBagLinkiIndexs[StorageKey] = index;
        }

        public List<TMGiftBagLinkResourceConfig> GetGiftBagLinkResources()
        {
            return TMGiftBagLinkConfigManager.Instance._giftBagLinkResourceConfigList;
        }

        public bool CanGetCurGift()
        {
            List<TMGiftBagLinkResourceConfig> linkResources = GetGiftBagLinkResources();
            if (linkResources == null)
                return false;

            int index = GetCurIndex();
            if (index < 0 || index + 1 >= linkResources.Count)
                return false;

            TMGiftBagLinkResourceConfig data = linkResources[index];
            if (data == null)
                return false;

            return data.ConsumeType == 1;
        }

        public void PurchaseSuccess(TableShop tableShop)
        {
            if (tableShop == null)
                return;

            var listData = GetGiftBagLinkResources();
            for (int index = 0; index < listData.Count; index++)
            {
                TMGiftBagLinkResourceConfig shopData = listData[index];

                if (shopData.ConsumeType != 2 || shopData.ConsumeAmount != tableShop.id)
                    continue;

                if (index < GetCurIndex())
                    continue;
                
                int loopIndex = 0;
                for (int i = 0; i < shopData.RewardID.Count; i++)
                {
                    int id = shopData.RewardID[i];
                    int num = shopData.Amount[i];

                    FlySystem.Instance.FlyItem(id,
                        num,
                        Vector2.zero, 
                        FlySystem.Instance.GetTargetTransform(id).position,
                        () =>
                        {
                            loopIndex++;
                            if (loopIndex == shopData.RewardID.Count)
                            {
                                EventDispatcher.Instance.DispatchEvent(EventEnum.TM_GIFTBAGLINK_PURCHASE_REFRESH, index, shopData);
                                PayRebateModel.Instance.OnPurchaseAniFinish();
                                PayRebateLocalModel.Instance.OnPurchaseAniFinish();
                            }
                        });
                        
                    ItemModel.Instance.Add(id, num, new DragonPlus.GameBIManager.ItemChangeReasonArgs()
                    {
                        reason = BiEventCooking.Types.ItemChangeReason.GiftBagLinkGetTm,
                    });
                }

                AddCurIndex();
                return;
            }
        }

        public bool CanShowUI()
        {
            if (!IsOpened())
                return false;

            return CanGetCurGift();
        }
        public override bool CanDownLoadRes()
        {
            return TMatchModel.Instance.GetMainLevel() >= TMatchConfigManager.Instance.GlobalList[0].GiftBagLineUnlock - 10;
        }
    }
}