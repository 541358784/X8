using System.Collections.Generic;
using System.Linq;
using ActivityLocal.LevelUpPackage.UI;
using DragonPlus;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Dynamic;
using Gameplay;

public class LevelUpPackageModel: Manager<LevelUpPackageModel>
{
    public string GetAuxItemAssetPath()
    {
        return "Prefabs/Home/Aux_LevelUpPackage";
    }
    public StorageLevelUpPackage Storage => StorageManager.Instance.GetStorage<StorageHome>().LevelUpPackage;
    private Dictionary<int, Dictionary<int, TableLevelUpPackageContentConfig>> ConfigDic;//key1为分层id,key2为礼包等级

    public Dictionary<int, TableLevelUpPackageContentConfig> ContentConfig =>
        GlobalConfigManager.Instance.TableLevelUpPackageContentConfig;
    public void InitConfig()
    {
        ConfigDic = new Dictionary<int, Dictionary<int, TableLevelUpPackageContentConfig>>();
        var packageLevelDic = GlobalConfigManager.Instance.TableLevelUpPackageLevelConfig;
        foreach (var pairPackageLevel in packageLevelDic)
        {
            var packageLevelConfig = new Dictionary<int, TableLevelUpPackageContentConfig>();
            var packageId = pairPackageLevel.Value.packageList;
            foreach (var levelConfig in GameConfigManager.Instance.LevelList)
            {
                if (levelConfig.levelUpPackage)
                {
                    var showLevel = levelConfig.lv;
                    var packageContentConfig = ContentConfig[packageId];
                    packageLevelConfig.Add(showLevel,packageContentConfig);
                }
            }
            ConfigDic.Add(pairPackageLevel.Key,packageLevelConfig);
        }
    }
    protected override void InitImmediately()
    {
        TMatch.Timer.Register(1, UpdateTime, null, true);
    }
    
    public void UpdateTime()
    {
        var rightUI =
            UIHomeMainController.GetAuxController(UIHomeMainController.AuxUIType.Right) as MainAux_RightController;
        if (!rightUI)
            return;
        UpdateStorage();
    }
    
    public void UpdateStorage()
    {
        CheckUpdateLevel();
        var curTime = APIManager.Instance.GetServerTime();
        for (var i = 0; i < Storage.PackageList.Count; i++)
        {
            var package = Storage.PackageList[i];
            if (curTime > package.EndTime)
            {
                Storage.PackageList.RemoveAt(i);
                i--;
                EventDispatcher.Instance.SendEventImmediately(new EventLevelUpPackageEnd(package));
            }
        }
    }

    public void CheckUpdateLevel()
    {
        var level = ExperenceModel.Instance.GetLevel();
        if (Storage.CueLevel == 0)//初始化时不触发升级礼包
        {
            Storage.CueLevel = ExperenceModel.Instance.GetLevel();
        }
        if (Storage.CueLevel != level)
        {
            var configLevel = PayLevelModel.Instance.GetCurPayLevelConfig().LevelUpPackageGroupId;
            if (ConfigDic.TryGetValue(configLevel, out var packageLevelConfig))
            {
                Storage.CueLevel = level;
                var curTime = APIManager.Instance.GetServerTime();
                if (packageLevelConfig.TryGetValue(level, out var packageContentConfig))
                {
                    var activeTime = (ulong)packageContentConfig.activeTime * XUtility.Min;
                    var newPackageStorage = new StorageLevelUpPackageSinglePackage()
                    {
                        PackageId = packageContentConfig.id,
                        BuyTimes = 0,
                        StartTime = curTime,
                        EndTime = curTime + activeTime,
                        Level = level,
                    };
                    Storage.PackageList.Add(newPackageStorage);
                    EventDispatcher.Instance.SendEventImmediately(new EventLevelUpPackageStart(newPackageStorage));
                    RegisterDynamicEntry(newPackageStorage);
                }
            }
        }
    }

    public void InitAux()
    {
        foreach (var newPackageStorage in Storage.PackageList)
        {
            RegisterDynamicEntry(newPackageStorage);
        }
    }

    private void RegisterDynamicEntry(StorageLevelUpPackageSinglePackage storage)
    {
        DynamicEntry_Home_LevelUp dynamicEntry = new DynamicEntry_Home_LevelUp(storage);
        DynamicEntryManager.Instance.RegisterDynamicEntry<DynamicEntry_Home_LevelUp>(storage, dynamicEntry);
    }

    public static bool CanShowLevelUpPackage()
    {
        for (var i = Instance.Storage.PackageList.Count - 1; i >= 0; i--)
        {
            var package = Instance.Storage.PackageList[i];
            if (package.IsActive())
            {
                UIPopupLevelUpPackageController.Open(package);
                return true;
            }
        }
        return false;
    }

    public void PurchaseSuccess(TableShop shopConfig,StorageLevelUpPackageSinglePackage package)
    {
        List<ResData> rewards = null;
        if (package == null)
        {
            package = Storage.PackageList.Find(a => ContentConfig[a.PackageId].shopId == shopConfig.id);   
        }
        if (package != null)
        {
            var contentConfig = ContentConfig[package.PackageId];
            package.BuyTimes++;
            if (package.BuyTimes >= contentConfig.buyTimes)
            {
                Storage.PackageList.Remove(package);
                EventDispatcher.Instance.SendEventImmediately(new EventLevelUpPackageEnd(package));
            }
            rewards = CommonUtils.FormatReward(contentConfig.rewardId, contentConfig.rewardNum);
        }
        else
        {
            foreach (var pair in ContentConfig)
            {
                if (pair.Value.shopId == shopConfig.id)
                {
                    rewards = CommonUtils.FormatReward(pair.Value.rewardId, pair.Value.rewardNum);
                    break;
                }
            }
        }

        if (rewards != null)
        {
            foreach (var reward in rewards)
            {
                if (!UserData.Instance.IsResource(reward.id))
                {
                    GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                    {
                        MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonLevelUpPackageGet,
                        itemAId = reward.id,
                        data1 = reward.count.ToString(),
                        isChange = true,
                    }); 
                }
            }
            var popup = UIManager.Instance.GetOpenedUIByPath<UIPopupLevelUpPackageController>(UINameConst.UIPopupLevelUpPackage);
            if (popup)
                popup.AnimCloseWindow();
            EventDispatcher.Instance.DispatchEvent(EventEnum.PURCHASE_SUCCESS_REWARD, rewards);
            CommonRewardManager.Instance.PopCommonReward(rewards,CurrencyGroupManager.Instance.GetCurrencyUseController(), 
                true, new GameBIManager.ItemChangeReasonArgs()
                {
                    reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.LevelUpPackageGet,
                }
                , () =>
                {
                    PayRebateModel.Instance.OnPurchaseAniFinish();
                    PayRebateLocalModel.Instance.OnPurchaseAniFinish();
                });
        }
    }
}