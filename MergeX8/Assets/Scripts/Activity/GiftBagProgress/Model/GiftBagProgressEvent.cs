using DragonU3DSDK.Storage;

public partial class EventEnum
{
    public const string GiftBagProgressBuyStateChange = "GiftBagProgressBuyStateChange";
    public const string GiftBagProgressCollectTask = "GiftBagProgressCollectTask";
    public const string GiftBagProgressCompleteTask = "GiftBagProgressCompleteTask";
}

public class EventGiftBagProgressBuyStateChange : BaseEvent
{
    public StorageGiftBagProgress Storage;
    public EventGiftBagProgressBuyStateChange() : base(EventEnum.GiftBagProgressBuyStateChange) { }

    public EventGiftBagProgressBuyStateChange(StorageGiftBagProgress storage) : base(EventEnum.GiftBagProgressBuyStateChange)
    {
        Storage = storage;
    }
}

public class EventGiftBagProgressCollectTask : BaseEvent
{
    public StorageGiftBagProgress Storage;
    public GiftBagProgressTaskConfig TaskConfig;
    public EventGiftBagProgressCollectTask() : base(EventEnum.GiftBagProgressCollectTask) { }

    public EventGiftBagProgressCollectTask(StorageGiftBagProgress storage,GiftBagProgressTaskConfig config) : base(EventEnum.GiftBagProgressCollectTask)
    {
        Storage = storage;
        TaskConfig = config;
    }
}
public class EventGiftBagProgressCompleteTask : BaseEvent
{
    public StorageGiftBagProgress Storage;
    public GiftBagProgressTaskConfig TaskConfig;
    public EventGiftBagProgressCompleteTask() : base(EventEnum.GiftBagProgressCompleteTask) { }

    public EventGiftBagProgressCompleteTask(StorageGiftBagProgress storage,GiftBagProgressTaskConfig config) : base(EventEnum.GiftBagProgressCompleteTask)
    {
        Storage = storage;
        TaskConfig = config;
    }
}