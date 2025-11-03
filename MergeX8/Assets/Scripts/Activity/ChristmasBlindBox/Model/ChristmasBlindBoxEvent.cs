using System.Collections.Generic;
using DragonU3DSDK.Storage;

public partial class EventEnum
{
    public const string ChristmasBlindBoxBuy = "ChristmasBlindBoxBuy";
}
public class EventChristmasBlindBoxBuy : BaseEvent
{
    public List<BlindBoxItemConfig> ItemConfigs;
    public EventChristmasBlindBoxBuy() : base(EventEnum.ChristmasBlindBoxBuy) { }

    public EventChristmasBlindBoxBuy(List<BlindBoxItemConfig> itemConfigs) : base(EventEnum.ChristmasBlindBoxBuy)
    {
        ItemConfigs = itemConfigs;
    }
}