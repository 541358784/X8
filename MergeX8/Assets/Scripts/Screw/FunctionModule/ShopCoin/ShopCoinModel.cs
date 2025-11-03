using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.Screw;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;

public partial class EventEnum
{
    public const string ScrewBuyShopCoin = "ScrewBuyShopCoin";
}
public class EventScrewBuyShopCoin : BaseEvent
{
    public TableCoinShopConfig Config;
    public EventScrewBuyShopCoin() : base(EventEnum.ScrewBuyShopCoin) { }
    public EventScrewBuyShopCoin(TableCoinShopConfig config) : base(EventEnum.ScrewBuyShopCoin)
    {
        Config = config;
    }
}
namespace Screw
{
    public class ShopCoinModel:Singleton<ShopCoinModel>
    {
        public List<TableCoinShopConfig> Configs=>DragonPlus.Config.Screw.GameConfigManager.Instance.TableCoinShopConfigList;
        public void OnPurchase(DragonPlus.Config.Screw.TableShop cfg)
        {
            var config = Configs.Find(a => a.Id == cfg.Id);
            var rewards = CommonUtils.FormatReward(config.RewardId, config.RewardNum);
            UserData.UserData.Instance.AddRes(rewards,new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.Iap),false);
            EventDispatcher.Instance.SendEventImmediately(new EventScrewBuyShopCoin(config));
            EventDispatcher.Instance.DispatchEvent(EventEnum.PURCHASE_SUCCESS_REWARD, rewards);

        }
    }
}