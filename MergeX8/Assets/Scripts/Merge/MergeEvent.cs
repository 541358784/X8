using DragonU3DSDK.Storage;

public partial class EventEnum
{
    public const string BreakBubble = "BreakBubble";
}
public class EventBreakBubble : BaseEvent
{
    public StorageStoreItem StoreItem;

    public EventBreakBubble() : base(EventEnum.BreakBubble) { }
}