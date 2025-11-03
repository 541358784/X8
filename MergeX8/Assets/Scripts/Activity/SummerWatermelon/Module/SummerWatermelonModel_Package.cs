using DragonPlus;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;

public partial class SummerWatermelonModel
{
    public SummerWatermelonPackageConfig GetCurrentPackage()
    {
        if (!IsStart)
            return null;
        for (var i = 0; i < PackageConfig.Count; i++)
        {
            var config = PackageConfig[i];
            var packageStartTime = (ulong) StorageSummerWatermelon.StartActivityTime +
                                   (ulong) (config.StartHourCount * XUtility.Hour);
            var packageEndTime = (ulong) StorageSummerWatermelon.StartActivityTime +
                                 (ulong) (config.EndHourCount * XUtility.Hour);
            if (APIManager.Instance.GetServerTime() >= packageStartTime &&
                APIManager.Instance.GetServerTime() < packageEndTime &&
                (config.BuyLimit < 1 || 
                 !StorageSummerWatermelon.BuyPackageShopIdDictionary.ContainsKey(config.ShopId) || 
                 StorageSummerWatermelon.BuyPackageShopIdDictionary[config.ShopId] < config.BuyLimit))
            {
                if (IsPackageOpened(config))
                    return config;
            }
        }
        return null;
    }

    public bool IsPackageOpened(SummerWatermelonPackageConfig config)
    {
        if (!StorageSummerWatermelon.PackageEnableStateDictionary.ContainsKey(config.Id))
        {
            StorageSummerWatermelon.PackageEnableStateDictionary.Add(config.Id,
                UserData.Instance.GetRes(UserData.ResourceId.Diamond) < config.MaxDiamond);
        }
        return StorageSummerWatermelon.PackageEnableStateDictionary[config.Id];
    }

    public ulong GetPackageLeftTime(SummerWatermelonPackageConfig curPackage)
    {
        if (curPackage == null || !IsStart)
            return 0;
        var packageEndTime = (ulong) StorageSummerWatermelon.StartActivityTime +
                             (ulong) (curPackage.EndHourCount * XUtility.Hour);
        var leftTime = packageEndTime-APIManager.Instance.GetServerTime();
        return leftTime;
    }

    public string GetPackageLeftTimeText(SummerWatermelonPackageConfig curPackage)
    {
        var leftTime = GetPackageLeftTime(curPackage);
        return CommonUtils.FormatLongToTimeStr((long) leftTime);
    }

    public bool IsSummerWatermelonItemId(int itemId)
    {
        if (!IsStart)
            return false;
        var itemConfig = GameConfigManager.Instance.GetItemConfig(itemId);
        if (itemConfig == null)
            return false;
        
        return SummerWatermelonConfig.LineId == itemConfig.in_line;
    }

    public void PurchasePackageSuccess(TableShop shopConfig)
    {
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventMergeactPackageSuccess);
        var openPopup = false;
        var packagePopup = UIManager.Instance.GetOpenedUIByPath<UIPopupSummerWatermelonGiftController>(PackageUIPath);
        if (packagePopup)
        {
            packagePopup.AnimCloseWindow();
            openPopup = true;
        }
        SummerWatermelonPackageConfig curPackageConfig = null;
        for (var i = 0; i < PackageConfig.Count; i++)
        {
            var config = PackageConfig[i];
            if (config.ShopId == shopConfig.id)
            {
                curPackageConfig = config;
            }
        }

        if (IsStart)
        {
            if (!StorageSummerWatermelon.BuyPackageShopIdDictionary.ContainsKey(shopConfig.id))
            {
                StorageSummerWatermelon.BuyPackageShopIdDictionary.Add(shopConfig.id,0);
            }
            StorageSummerWatermelon.BuyPackageShopIdDictionary[shopConfig.id]++;
        }

        var reasonArgs = new GameBIManager.ItemChangeReasonArgs()
        {
            reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.MergeactPackageSuccess,
        };
        var rewards = CommonUtils.FormatReward(curPackageConfig.RewardId, curPackageConfig.RewardNum);
        for (int i = 0; i < rewards.Count; i++)
        {
            var reward = rewards[i];
            if (!UserData.Instance.IsResource(reward.id))
            {
                GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                {
                    MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonSummerGet,
                    itemAId = reward.id,
                    isChange = true,
                });
                if (IsSummerWatermelonItemId(reward.id))
                {
                    for (var j = 0; j < reward.count; j++)
                    {
                        StorageSummerWatermelon.UnSetItems.Add(reward.id);
                        UnSetItemsCount++;
                        MainView?.RefreshBtnView();
                    }

                    continue;
                }
            }

            UserData.Instance.AddRes(reward.id, reward.count,
                reasonArgs, true);
        }
        EventDispatcher.Instance.DispatchEvent(EventEnum.PURCHASE_SUCCESS_REWARD, rewards);
        CommonRewardManager.Instance.PopCommonReward(rewards, CurrencyGroupManager.Instance.currencyController,
            false, reasonArgs, () =>
            {
                if (IsStart)
                {
                    // var nextPackage = GetCurrentPackage();
                    // if (nextPackage != null)
                    // {
                    //     var autoPopup = new BackHomeControl.AutoPopUI(ContinuePopupUI,new[] {PackageUIPath,UINameConst.UIPopupReward,UINameConst.UIPopupSummerWatermelonMain});
                    //     BackHomeControl.PushExtraPopup(autoPopup);
                    // }
                    // else
                    // {
                        if (!MainView && openPopup)
                        {
                            OpenMainPopup();
                        }   
                    // }
                }
            });
    }

    public bool ContinuePopupUI()
    {
        var curPackage = Instance.GetCurrentPackage();
        if (curPackage != null)
        {
            Instance.OpenGiftPopup(curPackage);
            return true;
        }
        return false;
    }
    public static string packageCoolTimeKey = "SummerWatermelonPackage";
    public static bool CanShowPackagePopupEachDay()
    {
        if (CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, packageCoolTimeKey))
            return false;
        var curPackage = Instance.GetCurrentPackage();
        if (curPackage != null)
        {
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventMergeactPackagePop);
            Instance.OpenGiftPopup(curPackage);
            CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, packageCoolTimeKey,CommonUtils.GetTimeStamp());
            return true;
        }
        return false;
    }
    public virtual string PackageUIPath => UINameConst.UIPopupSummerWatermelonGift;
    public UIPopupSummerWatermelonGiftController OpenGiftPopup(SummerWatermelonPackageConfig curPackage)
    {
        if (curPackage == null)
            return null;
        if (!IsStart)
            return null;
        return UIPopupSummerWatermelonGiftController.Open(curPackage);
    }
    public string GetPackageAuxItemAssetPath()
    {
        return "Prefabs/Activity/SummerWatermelonNormal/Aux_SummerWatermelonGift";
    }
}