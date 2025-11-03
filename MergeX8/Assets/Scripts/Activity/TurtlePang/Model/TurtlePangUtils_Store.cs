using System;
using System.Collections.Generic;
using System.Linq;
using Deco.World;
using Decoration;
using DragonPlus;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Farm.Model;
using UnityEngine;

public static partial class TurtlePangUtils
{
    public enum TurtlePangStoreItemType
    {
        MergeItem = 1,
        BuildItem = 2,
    }
    public static bool BuyStoreItem(this StorageTurtlePang storage, TurtlePangStoreItemConfig storeItemConfig)
    {
        if (storage.Score < storeItemConfig.Price)
            return false;

        if (!TurtlePangModel.Instance.IsOpenPrivate())
            return false;
        if (storage.GetPreheatTime() > 0 || storage.IsTimeOut())
            return false;
        TurtlePangModel.Instance.AddScore(-storeItemConfig.Price, "BuyItem");
        GameBIManager.Instance.SendGameEvent(
            BiEventAdventureIslandMerge.Types.GameEventType.GameEventTurtlePangRadishExchange,
            storeItemConfig.Id.ToString(), storage.GetCurStoreLevel().Id.ToString(), storage.Score.ToString());
        storage.FinishStoreItemList.Add(storeItemConfig.Id);
        if ((TurtlePangStoreItemType) storeItemConfig.Type == TurtlePangStoreItemType.BuildItem)
        {
            var unOwnNode = false;
            var showItemList = new List<int>();
            foreach (var buildItemId in storeItemConfig.RewardId)
            {
                DecoManager.Instance.UnlockDecoBuilding(buildItemId,false);
                var decoItem = DecoWorld.ItemLib[buildItemId];
                if (decoItem._node.IsOwned)
                {
                    showItemList.Add(buildItemId);
                }
                else
                {
                    unOwnNode = true;
                }
            }

            if (showItemList.Count > 0)
            {
                var hasShopUI = false;
                var hasMainUI = false;
                {
                    var shopUI =
                        UIManager.Instance
                            .GetOpenedUIByPath<UITurtlePangShopController>(UINameConst.UITurtlePangShop);
                    if (shopUI)
                    {
                        hasShopUI = true;
                        shopUI.CloseWindowWithinUIMgr(true);
                    }

                    var mainUI =
                        UIManager.Instance
                            .GetOpenedUIByPath<UITurtlePangMainController>(UINameConst.UITurtlePangMain);
                    if (mainUI)
                    {
                        hasMainUI = true;
                        mainUI.CloseWindowWithinUIMgr(true);
                    }
                }
                Action Callback = () =>
                {
                    if (unOwnNode)
                        CommonUtils.OpenCommonConfirmWindow(new NoticeUIData
                        {
                            DescString = LocalizationManager.Instance.GetLocalizedString("ui_easter_node_lock_tips"),
                        });
                    if (hasMainUI)
                    {
                        UITurtlePangMainController.Open(storage);
                    }
                    if (hasShopUI)
                    {
                        UITurtlePangShopController.Open(storage);
                    }
                };

                if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Game || FarmModel.Instance.IsFarmModel())
                {
                    SceneFsm.mInstance.ChangeState(StatusType.Transition, StatusType.BackHome,
                        DecoOperationType.Install,
                        showItemList, Callback);
                }
                else
                {
                    DecoManager.Instance.InstallItem(showItemList, Callback);
                }
            }
            else
            {
                if (unOwnNode)
                    CommonUtils.OpenCommonConfirmWindow(new NoticeUIData
                    {
                        DescString = LocalizationManager.Instance.GetLocalizedString("ui_easter_node_lock_tips"),
                    });
            }
        }
        else
        {
            var rewardData = CommonUtils.FormatReward(storeItemConfig.RewardId, storeItemConfig.RewardNum);
            var reasonArgs = new GameBIManager.ItemChangeReasonArgs()
            {
                reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.TurtlePangGet
            };
            CommonRewardManager.Instance.PopCommonReward(rewardData, CurrencyGroupManager.Instance.currencyController,
                true,
                reasonArgs);
        }

        EventDispatcher.Instance.SendEventImmediately(new EventTurtlePangBuyStoreItem(storeItemConfig));
        return true;
    }
    public static TurtlePangStoreLevelConfig GetCurStoreLevel(this StorageTurtlePang storage)
    {
        if (!TurtlePangModel.Instance.IsInitFromServer())
        {
            return null;
        }

        var levelConfig = TurtlePangModel.Instance.StoreLevelConfig;
        for (var i = 0; i < levelConfig.Count; i++)
        {
            var storeItemList = levelConfig[i].StoreItemList;
            for (var i1 = 0; i1 < storeItemList.Count; i1++)
            {
                if (!storage.FinishStoreItemList.Contains(storeItemList[i1]))
                {
                    return levelConfig[i];
                }
            }
        }

        return levelConfig.Last();
    }
}