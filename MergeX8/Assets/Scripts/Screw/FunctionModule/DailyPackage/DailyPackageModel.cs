using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.Screw;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;

public partial class EventEnum
{
    public const string ScrewBuyDailyPackage = "ScrewBuyDailyPackage";
    public const string ScrewRefreshDailyPackage = "ScrewRefreshDailyPackage";
}
public class EventScrewBuyDailyPackage : BaseEvent
{
    public TableDailyPackageConfig Config;
    public EventScrewBuyDailyPackage() : base(EventEnum.ScrewBuyDailyPackage) { }
    public EventScrewBuyDailyPackage(TableDailyPackageConfig config) : base(EventEnum.ScrewBuyDailyPackage)
    {
        Config = config;
    }
}
public class EventScrewRefreshDailyPackage : BaseEvent
{
    public EventScrewRefreshDailyPackage() : base(EventEnum.ScrewRefreshDailyPackage) { }
}
namespace Screw
{
    public class DailyPackageModel:Singleton<DailyPackageModel>
    {
        public List<TableDailyPackageConfig> Configs=>DragonPlus.Config.Screw.GameConfigManager.Instance.TableDailyPackageConfigList;

        public DailyPackageModel()
        {
            TMatch.Timer.Register(1, UpdateTime, null, true);
        }
        public StorageScrewDailyPackage Storage => StorageManager.Instance.GetStorage<StorageScrew>().DailyPackage;
        public void OnPurchase(DragonPlus.Config.Screw.TableShop cfg)
        {
            var config = Configs.Find(a => a.Id == cfg.Id);
            Storage.BuyState.TryAdd(config.Id, 0);
            Storage.BuyState[config.Id]++;
            
            var rewards = CommonUtils.FormatReward(config.RewardId, config.RewardNum);
            EventDispatcher.Instance.DispatchEvent(EventEnum.PURCHASE_SUCCESS_REWARD, rewards);

            UserData.UserData.Instance.AddRes(rewards,new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.Iap),false);
            EventDispatcher.Instance.SendEventImmediately(new EventScrewBuyDailyPackage(config));
        }

        public bool CanBuy(TableDailyPackageConfig cfg)
        {
            if (Storage.BuyState.TryGetValue(cfg.Id, out var buyTimes) && buyTimes >= cfg.BuyLimitTimes)
                return false;
            return true;
        }

        public void UpdateTime()
        {
            var curDayId = (int)(APIManager.Instance.GetServerTime() / XUtility.DayTime);
            if (curDayId > Storage.DayId)
            {
                Storage.DayId = curDayId;
                Storage.BuyState.Clear();
                EventDispatcher.Instance.SendEventImmediately(new EventScrewRefreshDailyPackage());
            }
        }

        public long GetLeftTime()
        {
            return (long)(XUtility.DayTime - (APIManager.Instance.GetServerTime() % XUtility.DayTime));
        }

        public string GetLeftTimeText()
        {
            return CommonUtils.FormatLongToTimeStr(GetLeftTime());
        }
    }
}