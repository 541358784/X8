using System.Collections.Generic;
using DragonU3DSDK.Storage;

public partial class EventEnum
{
    public const string ZUMA_SCORE_CHANGE = "ZUMA_SCORE_CHANGE";
    public const string ZUMA_DICE_COUNT_CHANGE = "ZUMA_DICE_COUNT_CHANGE";
    public const string ZUMA_BOMB_COUNT_CHANGE = "ZUMA_BOMB_COUNT_CHANGE";
    public const string ZUMA_WILD_COUNT_CHANGE = "ZUMA_WILD_COUNT_CHANGE";
    public const string ZUMA_NEW_LEVEL = "ZUMA_NEW_LEVEL";
    public const string ZUMA_BUY_STORE_ITEM = "ZUMA_BUY_STORE_ITEM";
}
public class EventZumaScoreChange : BaseEvent
{
    public int ChangeValue;
    public bool NeedWait;

    public EventZumaScoreChange() : base(EventEnum.ZUMA_SCORE_CHANGE) { }

    public EventZumaScoreChange(int changeValue,bool needWait = false) : base(EventEnum.ZUMA_SCORE_CHANGE)
    {
        ChangeValue = changeValue;
        NeedWait = needWait;
    }
}

public class EventZumaDiceCountChange : BaseEvent
{
    public int ChangeValue;
    public int TotalValue;
    public EventZumaDiceCountChange() : base(EventEnum.ZUMA_DICE_COUNT_CHANGE) { }

    public EventZumaDiceCountChange(int changeValue,int totalValue) : base(EventEnum.ZUMA_DICE_COUNT_CHANGE)
    {
        ChangeValue = changeValue;
        TotalValue = totalValue;
    }
}
public class EventZumaBombCountChange : BaseEvent
{
    public int ChangeValue;
    public int TotalValue;
    public EventZumaBombCountChange() : base(EventEnum.ZUMA_BOMB_COUNT_CHANGE) { }

    public EventZumaBombCountChange(int changeValue,int totalValue) : base(EventEnum.ZUMA_BOMB_COUNT_CHANGE)
    {
        ChangeValue = changeValue;
        TotalValue = totalValue;
    }
}
public class EventZumaLineCountChange : BaseEvent
{
    public int ChangeValue;
    public int TotalValue;
    public EventZumaLineCountChange() : base(EventEnum.ZUMA_WILD_COUNT_CHANGE) { }

    public EventZumaLineCountChange(int changeValue,int totalValue) : base(EventEnum.ZUMA_WILD_COUNT_CHANGE)
    {
        ChangeValue = changeValue;
        TotalValue = totalValue;
    }
}
public class EventZumaBuyStoreItem : BaseEvent
{
    public ZumaStoreItemConfig StoreItemConfig;

    public EventZumaBuyStoreItem() : base(EventEnum.ZUMA_BUY_STORE_ITEM) { }

    public EventZumaBuyStoreItem(ZumaStoreItemConfig storeItemConfig) : base(EventEnum.ZUMA_BUY_STORE_ITEM)
    {
        StoreItemConfig = storeItemConfig;
    }
}

public class EventZumaNewLevel : BaseEvent
{
    public EventZumaNewLevel() : base(EventEnum.ZUMA_NEW_LEVEL) { }
}