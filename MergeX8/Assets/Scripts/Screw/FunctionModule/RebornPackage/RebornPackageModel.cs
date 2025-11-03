using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.Screw;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;

public partial class EventEnum
{
    public const string ScrewBuyRebornPackage = "ScrewBuyRebornPackage";
}
public class EventScrewBuyRebornPackage : BaseEvent
{
    public TableRebornPackageConfig Config;
    public EventScrewBuyRebornPackage() : base(EventEnum.ScrewBuyRebornPackage) { }
    public EventScrewBuyRebornPackage(TableRebornPackageConfig config) : base(EventEnum.ScrewBuyRebornPackage)
    {
        Config = config;
    }
}
namespace Screw
{
    public class RebornPackageModel:Singleton<RebornPackageModel>
    {
        public List<TableRebornPackageConfig> Configs=>DragonPlus.Config.Screw.GameConfigManager.Instance.TableRebornPackageConfigList;
        public void OnPurchase(DragonPlus.Config.Screw.TableShop cfg)
        {
            var config = Configs.Find(a => a.Id == cfg.Id);
            var rewards = CommonUtils.FormatReward(config.RewardId, config.RewardNum);
            UserData.UserData.Instance.AddRes(rewards,new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.Iap),false);
            EventDispatcher.Instance.DispatchEvent(EventEnum.PURCHASE_SUCCESS_REWARD, rewards);
            EventDispatcher.Instance.SendEventImmediately(new EventScrewBuyRebornPackage(config));
        }
    }
}