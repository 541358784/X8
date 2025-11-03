public partial class EventEnum
{
    public const string BuyDiamondTicketBagChange = "BuyDiamondTicketBagChange";
}
public class EventBuyDiamondTicketBagChange : BaseEvent
{
    public EventBuyDiamondTicketBagChange() : base(EventEnum.BuyDiamondTicketBagChange) { }
}