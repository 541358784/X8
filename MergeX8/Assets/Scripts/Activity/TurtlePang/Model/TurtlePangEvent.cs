using DragonU3DSDK.Storage;

public partial class EventEnum
{
    public const string TurtlePangUpdateRedPoint = "TurtlePangUpdateRedPoint";
    public const string TurtlePangBuyStoreItem = "TurtlePangBuyStoreItem";
    public const string TurtlePangScoreChange = "TurtlePangScoreChange";
    public const string TurtlePangPackageCountChange = "TurtlePangPackageCountChange";
    public const string TurtlePangBagItemChange = "TurtlePangBagItemChange";
}

public class EventTurtlePangBagItemChange : BaseEvent
{
    public EventTurtlePangBagItemChange() : base(EventEnum.TurtlePangBagItemChange) { }
}
public class EventTurtlePangUpdateRedPoint : BaseEvent
{
    public EventTurtlePangUpdateRedPoint() : base(EventEnum.TurtlePangUpdateRedPoint) { }
}

public class EventTurtlePangBuyStoreItem : BaseEvent
{
    public TurtlePangStoreItemConfig StoreItemConfig;

    public EventTurtlePangBuyStoreItem() : base(EventEnum.TurtlePangBuyStoreItem) { }
    public EventTurtlePangBuyStoreItem(TurtlePangStoreItemConfig storeItemConfig) : base(EventEnum.TurtlePangBuyStoreItem)
    {
        StoreItemConfig = storeItemConfig;
    }
}
public class EventTurtlePangScoreChange : BaseEvent
{
    public int ChangeValue;
    public bool NeedWait;

    public EventTurtlePangScoreChange() : base(EventEnum.TurtlePangScoreChange) { }

    public EventTurtlePangScoreChange(int changeValue,bool needWait = false) : base(EventEnum.TurtlePangScoreChange)
    {
        ChangeValue = changeValue;
        NeedWait = needWait;
    }
}

public class EventTurtlePangPackageCountChange : BaseEvent
{
    public int ChangeValue;

    public EventTurtlePangPackageCountChange() : base(EventEnum.TurtlePangPackageCountChange) { }

    public EventTurtlePangPackageCountChange(int changeValue) : base(EventEnum.TurtlePangPackageCountChange)
    {
        ChangeValue = changeValue;
    }
}