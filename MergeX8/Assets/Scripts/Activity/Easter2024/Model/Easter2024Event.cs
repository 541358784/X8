public partial class EventEnum
{
    public const string EASTER_2024_SCORE_CHANGE = "EASTER_2024_SCORE_CHANGE";
    public const string EASTER_2024_EGG_COUNT_CHANGE = "EASTER_2024_EGG_COUNT_CHANGE";
    public const string EASTER_2024_LUCKY_POINT_COUNT_CHANGE = "EASTER_2024_LUCKY_POINT_COUNT_CHANGE";
    public const string EASTER_2024_CARD_COUNT_CHANGE = "EASTER_2024_CARD_COUNT_CHANGE";
    public const string EASTER_2024_BUY_STORE_ITEM = "EASTER_2024_BUY_STORE_ITEM";
}
public class EventEaster2024ScoreChange : BaseEvent
{
    public int ChangeValue;
    public bool NeedWait;

    public EventEaster2024ScoreChange() : base(EventEnum.EASTER_2024_SCORE_CHANGE) { }

    public EventEaster2024ScoreChange(int changeValue,bool needWait = false) : base(EventEnum.EASTER_2024_SCORE_CHANGE)
    {
        ChangeValue = changeValue;
        NeedWait = needWait;
    }
}

public class EventEaster2024EggCountChange : BaseEvent
{
    public int ChangeValue;

    public EventEaster2024EggCountChange() : base(EventEnum.EASTER_2024_EGG_COUNT_CHANGE) { }

    public EventEaster2024EggCountChange(int changeValue) : base(EventEnum.EASTER_2024_EGG_COUNT_CHANGE)
    {
        ChangeValue = changeValue;
    }
}
public class EventEaster2024LuckyPointCountChange : BaseEvent
{
    public int ChangeValue;

    public EventEaster2024LuckyPointCountChange() : base(EventEnum.EASTER_2024_LUCKY_POINT_COUNT_CHANGE) { }

    public EventEaster2024LuckyPointCountChange(int changeValue) : base(EventEnum.EASTER_2024_LUCKY_POINT_COUNT_CHANGE)
    {
        ChangeValue = changeValue;
    }
}
public class EventEaster2024CardCountChange : BaseEvent
{
    public Easter2024CardState CardState;
    public int ChangeCount;

    public EventEaster2024CardCountChange() : base(EventEnum.EASTER_2024_CARD_COUNT_CHANGE) { }

    public EventEaster2024CardCountChange(Easter2024CardState cardState,int changeCount) : base(EventEnum.EASTER_2024_CARD_COUNT_CHANGE)
    {
        CardState = cardState;
        ChangeCount = changeCount;
    }
}
public class EventEaster2024BuyStoreItem : BaseEvent
{
    public Easter2024StoreItemConfig StoreItemConfig;

    public EventEaster2024BuyStoreItem() : base(EventEnum.EASTER_2024_BUY_STORE_ITEM) { }

    public EventEaster2024BuyStoreItem(Easter2024StoreItemConfig storeItemConfig) : base(EventEnum.EASTER_2024_BUY_STORE_ITEM)
    {
        StoreItemConfig = storeItemConfig;
    }
}