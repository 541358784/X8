using DragonU3DSDK.Storage;

public partial class EventEnum
{
    public const string BlindBoxRecycleItem = "BlindBoxRecycleItem";
}

public class EventBlindBoxRecycleItem : BaseEvent
{
    public StorageBlindBox Storage;
    public EventBlindBoxRecycleItem() : base(EventEnum.BlindBoxRecycleItem) { }
    public EventBlindBoxRecycleItem(StorageBlindBox storage) : base(EventEnum.BlindBoxRecycleItem)
    {
        Storage = storage;
    }
}