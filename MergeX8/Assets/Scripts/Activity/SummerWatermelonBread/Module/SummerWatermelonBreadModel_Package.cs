using DragonPlus;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;

public partial class SummerWatermelonBreadModel
{
    public SummerWatermelonBreadPackageConfig GetCurrentPackage()
    {
        if (!IsStart)
            return null;
        for (var i = 0; i < PackageConfig.Count; i++)
        {
            var config = PackageConfig[i];
            var packageStartTime = (ulong) StorageSummerWatermelonBread.StartActivityTime +
                                   (ulong) (config.StartHourCount * XUtility.Hour);
            var packageEndTime = (ulong) StorageSummerWatermelonBread.StartActivityTime +
                                 (ulong) (config.EndHourCount * XUtility.Hour);
            if (APIManager.Instance.GetServerTime() >= packageStartTime &&
                APIManager.Instance.GetServerTime() < packageEndTime &&
                (config.BuyLimit < 1 || 
                 !StorageSummerWatermelonBread.BuyPackageShopIdDictionary.ContainsKey(config.ShopId) || 
                 StorageSummerWatermelonBread.BuyPackageShopIdDictionary[config.ShopId] < config.BuyLimit))
            {
                if (IsPackageOpened(config))
                    return config;
            }
        }
        return null;
    }
    public bool IsPackageOpened(SummerWatermelonBreadPackageConfig config)
    {
        if (!StorageSummerWatermelonBread.PackageEnableStateDictionary.ContainsKey(config.Id))
        {
            StorageSummerWatermelonBread.PackageEnableStateDictionary.Add(config.Id,
                UserData.Instance.GetRes(UserData.ResourceId.Diamond) < config.MaxDiamond);
        }
        return StorageSummerWatermelonBread.PackageEnableStateDictionary[config.Id];
    }

    public ulong GetPackageLeftTime(SummerWatermelonBreadPackageConfig curPackage)
    {
        if (curPackage == null || !IsStart)
            return 0;
        var packageEndTime = (ulong) StorageSummerWatermelonBread.StartActivityTime +
                             (ulong) (curPackage.EndHourCount * XUtility.Hour);
        var leftTime = packageEndTime-APIManager.Instance.GetServerTime();
        return leftTime;
    }

    public string GetPackageLeftTimeText(SummerWatermelonBreadPackageConfig curPackage)
    {
        var leftTime = GetPackageLeftTime(curPackage);
        return CommonUtils.FormatLongToTimeStr((long) leftTime);
    }

    public bool IsSummerWatermelonBreadItemId(int itemId)
    {
        if (!IsStart)
            return false;
        var itemConfig = GameConfigManager.Instance.GetItemConfig(itemId);
        if (itemConfig == null)
            return false;
        
        return SummerWatermelonBreadConfig.LineId == itemConfig.in_line;
    }

    public void PurchasePackageSuccess(TableShop shopConfig)
    {
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventMergeactPackageSuccess);
        var openPopup = false;
        var packagePopup = UIManager.Instance.GetOpenedUIByPath<UIPopupSummerWatermelonBreadGiftController>(PackageUIPath);
        if (packagePopup)
        {
            packagePopup.AnimCloseWindow();
            openPopup = true;
        }
        SummerWatermelonBreadPackageConfig curPackageConfig = null;
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
            if (!StorageSummerWatermelonBread.BuyPackageShopIdDictionary.ContainsKey(shopConfig.id))
            {
                StorageSummerWatermelonBread.BuyPackageShopIdDictionary.Add(shopConfig.id,0);
            }
            StorageSummerWatermelonBread.BuyPackageShopIdDictionary[shopConfig.id]++;
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
                if (IsSummerWatermelonBreadItemId(reward.id))
                {
                    for (var j = 0; j < reward.count; j++)
                    {
                        StorageSummerWatermelonBread.UnSetItems.Add(reward.id);
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
                    //     var autoPopup = new BackHomeControl.AutoPopUI(ContinuePopupUI,new[] {PackageUIPath,UINameConst.UIPopupReward,UINameConst.UIPopupSummerWatermelonBreadMain});
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
    public static string packageCoolTimeKey = "SummerWatermelonBreadPackage";
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
    public virtual string PackageUIPath => UINameConst.UIPopupSummerWatermelonBreadGift;
    public UIPopupSummerWatermelonBreadGiftController OpenGiftPopup(SummerWatermelonBreadPackageConfig curPackage)
    {
        if (curPackage == null)
            return null;
        if (!IsStart)
            return null;
        return UIPopupSummerWatermelonBreadGiftController.Open(curPackage);
    }
    public string GetPackageAuxItemAssetPath()
    {
        return "Prefabs/Activity/SummerWatermelonBread/Aux_SummerWatermelonBreadGift";
    }
}