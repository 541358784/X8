public partial class EventEnum
{
    public const string ShopExtraRewardEnd = "ShopExtraRewardEnd";
}
public class EventShopExtraRewardEnd : BaseEvent
{
    public EventShopExtraRewardEnd() : base(EventEnum.ShopExtraRewardEnd) { }
}