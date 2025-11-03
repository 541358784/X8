using DragonU3DSDK.Storage;

public partial class EventEnum
{
    public const string StarrySkyCompassUpdateRedPoint = "StarrySkyCompassUpdateRedPoint";
    public const string StarrySkyCompassBuyShopItem = "StarrySkyCompassBuyShopItem";
    public const string StarrySkyCompassScoreChange = "StarrySkyCompassScoreChange";
    public const string StarrySkyCompassRocketCountChange = "StarrySkyCompassRocketCountChange";
    public const string StarrySkyCompassHappyValueChange = "StarrySkyCompassHappyValueChange";
}
public class EventStarrySkyCompassUpdateRedPoint : BaseEvent
{
    public EventStarrySkyCompassUpdateRedPoint() : base(EventEnum.StarrySkyCompassUpdateRedPoint) { }
}

public class EventStarrySkyCompassBuyShopItem : BaseEvent
{
    public StarrySkyCompassShopConfig ShopItemConfig;

    public EventStarrySkyCompassBuyShopItem() : base(EventEnum.StarrySkyCompassBuyShopItem) { }
    public EventStarrySkyCompassBuyShopItem(StarrySkyCompassShopConfig shopItemConfig) : base(EventEnum.StarrySkyCompassBuyShopItem)
    {
        ShopItemConfig = shopItemConfig;
    }
}
public class EventStarrySkyCompassScoreChange : BaseEvent
{
    public int ChangeValue;
    public bool NeedWait;

    public EventStarrySkyCompassScoreChange() : base(EventEnum.StarrySkyCompassScoreChange) { }

    public EventStarrySkyCompassScoreChange(int changeValue,bool needWait = false) : base(EventEnum.StarrySkyCompassScoreChange)
    {
        ChangeValue = changeValue;
        NeedWait = needWait;
    }
}

public class EventStarrySkyCompassRocketCountChange : BaseEvent
{
    public int ChangeValue;

    public EventStarrySkyCompassRocketCountChange() : base(EventEnum.StarrySkyCompassRocketCountChange) { }

    public EventStarrySkyCompassRocketCountChange(int changeValue) : base(EventEnum.StarrySkyCompassRocketCountChange)
    {
        ChangeValue = changeValue;
    }
}

public class EventStarrySkyCompassHappyValueChange : BaseEvent
{
    public int ChangeValue;
    public EventStarrySkyCompassHappyValueChange() : base(EventEnum.StarrySkyCompassHappyValueChange) { }

    public EventStarrySkyCompassHappyValueChange(int changeValue) : base(EventEnum.StarrySkyCompassHappyValueChange)
    {
        ChangeValue = changeValue;
    }
}