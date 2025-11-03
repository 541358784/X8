using System;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.ExtraEnergy;
using DragonPlus.Config.OptionalGift;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using UnityEngine;

    public partial class MixMasterModel
    {
        public StorageOptionalGift StorageOptionalGift => Storage.GiftBag;

        public MixMasterGiftBagConfig GetOptionalGiftActivityConfig()
        {
            return MixMasterModel.Instance.GiftBagConfig[0];
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
                Storage.BuyTimes++;
                StorageOptionalGift.SelectItem.Clear();
                var giftBagConfig = GiftBagConfig[0];
                StorageOptionalGift.SelectItem.Add(0,giftBagConfig.Item1[0]);
                StorageOptionalGift.SelectItem.Add(1,giftBagConfig.Item2[0]);
                StorageOptionalGift.SelectItem.Add(2,giftBagConfig.Item3[0]);
                var controller= UIManager.Instance.GetOpenedUIByPath<UIPopupMixMasterShopController>(UINameConst.UIPopupMixMasterShop);
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
                });
        }
    }
